using System;
using System.Collections.Generic;
using System.IO;
using vcFramework.IO;

namespace LolBackup
{
    /// <summary>
    /// Preforms backup operations
    /// </summary>
    public class BackupProcess
    {
        #region FIELDS

        bool _busy;

        readonly string _sourceDirectory;

        readonly string _targetDirectory;

        readonly List<string> _allowedFileExtensions = new List<string>();

        readonly List<string> _blockedFileExtensions = new List<string>();

        readonly List<string> _blockedFolders = new List<string>();

        readonly WriteMessageDelegate _writeMessageDelegate;

        /// <summary>
        /// If not external message receiver is defined, messages will be placed in this collection.
        /// </summary>
        readonly IList<string> _messages = new List<string>();

        #endregion

        #region PROPERTIES

        public IList<string> Messages
        {
            get
            {
                return _messages;
            }
        }

        public string SourceDirectory
        {
            get
            {
                return _sourceDirectory;
            }
        }

        public string TargetDirectory
        {
            get
            {
                return _targetDirectory;
            }
        }

        public bool VersionFiles {get;set;}

        public bool DeleteOrphans {get;set;}

        #endregion

        #region CTORS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectory">Directory to back up.</param>
        /// <param name="targetDirectory">Directory to back up to.</param>
        /// <param name="allowedFileExtensions">Optional (can be null or empty). List of file extensions to allow. If any set, only allowed extensions will be backed up. Extensions must not be preceeded by dots</param>
        /// <param name="blockedFileExtensions">Optional (can be null or empty). List of file extensions to ignore. Extensions must not be preceeded by dots</param>
        /// <param name="blockedFolders">Optional (can be null or empty). List of folders to ignore.</param>
        /// <param name="writeMessageDelegate">Optional. Delegate to write messages to.</param>
        public BackupProcess(
            string sourceDirectory,
            string targetDirectory,
            List<string> allowedFileExtensions,
            List<string> blockedFileExtensions,
            List<string> blockedFolders,
            WriteMessageDelegate writeMessageDelegate
            )
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;

            if (allowedFileExtensions != null)
            {
                _allowedFileExtensions = allowedFileExtensions;
            }

            if (blockedFileExtensions != null)
            {
                _blockedFileExtensions = blockedFileExtensions;
            }

            if (blockedFolders != null)
            {
                _blockedFolders = blockedFolders;
            }

            if (writeMessageDelegate == null)
                _writeMessageDelegate = this.WriteSelf;
            else
                _writeMessageDelegate = writeMessageDelegate;


            this.DeleteOrphans = false;
            this.VersionFiles = false;
        }


        #endregion

        #region METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Number of files copied. Returns -1 if process aborted because busy.
        /// </returns>
        public int Process(
            )
        {
            try
            {
                if (_busy)
                    return -1;

                _busy = true;

                int filesCopied = 0;
                try
                {

                    // #######################################################
                    // Error checking
                    // #######################################################
                    bool abort = false;
                    if (!Directory.Exists(_sourceDirectory))
                    {
                        abort = true;
                        _writeMessageDelegate.Invoke(string.Format("Source directory '{0}' does not exist. Backup process failed.", _sourceDirectory));
                    }

                    if (!Directory.Exists(_targetDirectory))
                    {
                        abort = true;
                        _writeMessageDelegate.Invoke(string.Format("Target directory '{0}' does not exist. Backup process failed.", _targetDirectory));
                    }

                    if (FileSystemLib.IsAChildOf(_sourceDirectory, _targetDirectory))
                    {
                        abort = true;
                        _writeMessageDelegate.Invoke(string.Format("The target directory '{0}' is nested under source directory '{1}', which is not allowed. Please place the target directory so it does not fall under the source. Backup process failed.", _targetDirectory, _sourceDirectory));
                    }

                    if (abort)
                        return 0;


                    // #######################################################
                    // get list of all files in source directory, regardless
                    // of nesting depth
                    // #######################################################
                    _writeMessageDelegate.Invoke(string.Format("Scanning {0} for changes...", _sourceDirectory));
                    string[] sourceFiles = FileSystemLib.GetFilesUnder(_sourceDirectory, _blockedFolders);

                    foreach (string sourceFile in sourceFiles)
                    {
                        try
                        {
                            // map source file to target file
                            string targetFile = FileSystemLib.MapPaths(
                                new [] { sourceFile }, _sourceDirectory, _targetDirectory)[0];

                            string extension = Path.GetExtension(sourceFile).ToLower();
                            // remove leading "." on extensions
                            while (extension.StartsWith("."))
                                extension = extension.Substring(1, extension.Length - 1);

                            // skip file if it is now allowed, or blocked
                            if (_allowedFileExtensions.Count > 0 && !_allowedFileExtensions.Contains(extension))
                                continue;
                            if (_blockedFileExtensions.Contains(extension))
                                continue;

                            // check if file should be copied
                            bool copyFile = false;
                            if (!File.Exists(targetFile))
                                copyFile = true;
                            else
                            {
                                // if file already exists in target location, check last modify dates
                                if (File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
                                    copyFile = true;
                            }

                            if (!copyFile)
                                continue;

                            // determine date + incrementor suffix for rename of outdated target file
                            string suffix = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                            int i = 0;
                            while (true)
                            {
                                if (!File.Exists(targetFile + "." + suffix + "_" + i + "_backup"))
                                {
                                    suffix += "_" + i + "_backup";
                                    break;
                                }
                                i++;
                            }

                            // rename outdated target file if neccessary
                            if (this.VersionFiles && File.Exists(targetFile))
                                File.Move(targetFile, targetFile + "." + suffix);

                            // create directory if necessary
                            string directory = Path.GetDirectoryName(targetFile);
                            if (!Directory.Exists(directory))
                                Directory.CreateDirectory(directory);

                            // copy file, always overwriting
                            File.Copy(sourceFile, targetFile, true);
                            _writeMessageDelegate.Invoke("backed up " + sourceFile);
                            filesCopied++;

                        }
                        catch (UnauthorizedAccessException)
                        { 
                            // suppress these
                        }
                        catch (IOException)
                        {
                            // suppress these
                        }
                        catch (Exception ex)
                        {
                            // handles unexpected exceptions at the individual file level
                            // log error and continue
                            _writeMessageDelegate.Invoke("An unexpected error occured while processing file '" + sourceFile + "' : " + ex);
                        }

                    } // foreach


                    // delete orphans
                    if (this.DeleteOrphans)
                    {

                        // deletes empty folders before files, this can save a lot of unnecessary checking if child
                        // content if the parent folder is deleted first.
                        IEnumerable<string> folders = new List<string>();

                        try
                        {
                            _writeMessageDelegate.Invoke(string.Format("Scanning {0} for orphan folders...", _targetDirectory));
                            folders = FileSystemLib.GetFoldersUnder(_targetDirectory);
                        }
                        catch (Exception ex)
                        {
                            _writeMessageDelegate.Invoke(string.Format("Unexpected error scanning for orphan folders : {0}", ex));
                        }

                        foreach (string folder in folders)
                        {
                            // folder might already have been deleted if parent was deleted
                            if (!Directory.Exists(folder))
                                continue;

                            string sourceDirectory = FileSystemLib.MapPaths(
                                new[] { folder }, _targetDirectory, _sourceDirectory)[0];

                            if (Directory.Exists(sourceDirectory))
                                continue;

                            try
                            {
                                Directory.Delete(folder, true);
                                _writeMessageDelegate.Invoke(string.Format("Cleaned up folder : {0}", folder));
                            }
                            catch (Exception ex)
                            {
                                _writeMessageDelegate.Invoke(string.Format("Unexpected error deleting empty folder : {0}", ex));
                            }
                        }

                        //get orphan files
                        IEnumerable<string> targetFiles = new List<string>();

                        try
                        {
                            _writeMessageDelegate.Invoke(string.Format("Scanning {0} for orphan files...", _targetDirectory));
                            targetFiles = FileSystemLib.GetFilesUnder(_targetDirectory);
                        }
                        catch (Exception ex)
                        {
                            _writeMessageDelegate.Invoke(string.Format("Unexpected error scanning for orphan files: {0}", ex));
                        }
                        

                        foreach (string targetFile in targetFiles)
                        {
                            try
                            {
                                if (targetFile.ToLower().EndsWith("_backup"))
                                    continue;
                                string sourceFile = FileSystemLib.MapPaths(
                                    new [] { targetFile }, _targetDirectory, _sourceDirectory)[0];

                                if (!File.Exists(sourceFile))
                                {
                                    try
                                    {
                                        File.Delete(targetFile);
                                    }
                                    catch (UnauthorizedAccessException)
                                    {
                                        // try to strip readonly from file
                                        File.SetAttributes(targetFile, FileAttributes.Normal);
                                        File.Delete(targetFile);
                                    }

                                    _writeMessageDelegate.Invoke("Cleaned up file : " + targetFile);
                                }
                            }

                            catch (MappingException ex)
                            {
                                _writeMessageDelegate.Invoke("The file '" + ex.File + "' has an unsupported path. File skipped.");
                            }
                            catch (Exception ex)
                            {
                                _writeMessageDelegate.Invoke("An unexpected error occured while attempting to delete orphan file '" + targetFile + "' : " + ex.Message);
                            }
                        }
                        
                    }

                    return filesCopied;

                }
                catch (Exception ex)
                {
                    //  handles unexpected errors occuring before individual file handling is reached
                    _writeMessageDelegate.Invoke("An unexpected error '" + ex.Message + "' occured. Backup process for '" + _sourceDirectory + "' aborted. ");
                    return filesCopied;
                }
            }
            finally
            {
                _busy = false;
            }
        }

        /// <summary>
        /// Fall back for creating a backup process without an external message receiver.
        /// </summary>
        /// <param name="message"></param>
        private void WriteSelf(string message) 
        {
            _messages.Add(message);
        }

        #endregion
    }
}
