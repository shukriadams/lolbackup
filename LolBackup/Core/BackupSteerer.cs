using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace LolBackup
{
    /// <summary>
    /// Thin wrapper class which holds individual backup jobs (BackupProcesses). Actual backup logic contained in BackupProcesses.
    /// </summary>
    public class BackupSteerer
    {
        #region FIELDS

        readonly List<BackupProcess> _jobs = new List<BackupProcess>();

        WriteMessageDelegate _writeMessageDelegate;

        #endregion

        #region PROPERTIES

        public WriteMessageDelegate WriteMessageDelegate
        {
            get 
            {
                return _writeMessageDelegate;
            }
            set 
            {
                _writeMessageDelegate = value;
            }
        }

        #endregion

        #region CTORS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobList"></param>
        /// <param name="writeMessageDelegate">Leave null if not required.</param>
        public BackupSteerer(
            XmlDocument jobList,
            WriteMessageDelegate writeMessageDelegate
            ) 
        {
            _writeMessageDelegate = writeMessageDelegate;
            XmlNodeList jobs = jobList.DocumentElement.SelectNodes("process");
            foreach (XmlNode job in jobs)
            {
                List<string> allowedFileExtensions = new List<string>();
                List<string> blockedFileExtensions = new List<string>();
                List<string> blockedfolders = new List<string>();

                // get allowed/blocked types
                XmlNodeList types = job.SelectNodes("allowList/type");
                foreach (XmlNode t in types)
                {
                    string s = t.InnerText.ToLower();
                    while (s.StartsWith("."))
                        s = s.Substring(1, s.Length - 1);
                    allowedFileExtensions.Add(s);
                }
                types = job.SelectNodes("blockList/type");
                foreach (XmlNode t in types)
                {
                    string s = t.InnerText.ToLower();
                    while (s.StartsWith("."))
                        s = s.Substring(1, s.Length - 1);
                    blockedFileExtensions.Add(s);
                }

                // get blocked folders
                XmlNodeList folders = job.SelectNodes("blockedFolders/blockedFolder");
                foreach (XmlNode f in folders)
                {
                    string s = f.InnerText.ToLower();
                    blockedfolders.Add(s);
                }

                BackupProcess p = new BackupProcess(
                    job.Attributes["sourceDirectory"].Value,
                    job.Attributes["targetDirectory"].Value,
                    allowedFileExtensions,
                    blockedFileExtensions,
                    blockedfolders,
                    writeMessageDelegate);

                if (job.Attributes["versionFiles"] != null)
                    p.VersionFiles = Boolean.Parse(job.Attributes["versionFiles"].Value);
                if (job.Attributes["deleteOrphans"] != null)
                    p.DeleteOrphans = Boolean.Parse(job.Attributes["deleteOrphans"].Value);

                _jobs.Add(p);
            }
        }

        #endregion

        #region METHODS

        public void Process(
            )
        {
            int count = 0;
            foreach (BackupProcess p in _jobs)
                count += p.Process();
            
            if (_writeMessageDelegate != null)
                _writeMessageDelegate.Invoke("Files backed up : " + count);

        }

        #endregion
    }
}
