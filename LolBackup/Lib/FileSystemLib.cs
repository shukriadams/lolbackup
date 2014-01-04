using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LolBackup
{
    public class FileSystemLib
    {
        /// <summary> 
        /// Returns true if the suspected child dir is nested under the suspected parent .
        /// </summary>
        /// <param name="suspectedParentDirectory"></param>
        /// <param name="suspectChildDirectory"></param>
        /// <returns></returns>
        public static bool IsAChildOf(
            string suspectedParentDirectory,
            string suspectChildDirectory
            )
        {
            bool isChild = false;

            IsAChildOfInternal(
                suspectedParentDirectory,
                suspectChildDirectory,
                ref isChild);

            return isChild;
        }


        /// <summary>
        /// Recursive method behind IsAChildOf();
        /// </summary>
        /// <param name="suspectedParentDirectoryCurrentLevel"></param>
        /// <param name="suspectChildDirectory"></param>
        /// <param name="isChild"></param>
        private static void IsAChildOfInternal(
            string suspectedParentDirectoryCurrentLevel,
            string suspectChildDirectory,
            ref bool isChild
            )
        {
            try
            {
                // kills concurrent recursing processes
                // if a child confirmation is reported elsewhere
                if (isChild)
                    return;

                DirectoryInfo dirInfo = new DirectoryInfo(suspectedParentDirectoryCurrentLevel);
                DirectoryInfo[] folders = dirInfo.GetDirectories();

                // processes current level folders
                foreach (DirectoryInfo dir in folders)
                {
                    if (Path.GetDirectoryName(dir.FullName) == Path.GetDirectoryName(suspectChildDirectory))
                    {
                        isChild = true;
                        return;
                    }
                }

                // recursion happens here
                foreach (DirectoryInfo dir in folders)
                    IsAChildOfInternal(
                        dir.FullName,
                        suspectChildDirectory,
                        ref isChild);

            }
            catch (UnauthorizedAccessException)
            {
                // suppress these, they will be hit a lot
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to search in</param>
        /// <param name="fileTypes">File types to filter for. Nullable.</param>
        /// <param name="ignoreFolders">List of folders to ignore. Case-insensitive. Nullable.</param>
        /// <param name="files">Holder of files to return</param>
        private static void GetFilesUnderInternal(
            string path,
            IEnumerable<string> fileTypes,
            ICollection<string> ignoreFolders,
            ICollection<string> files
            )
        {
            try
            {

                // handles files
                if (!Directory.Exists(path))
                    return;

                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] filesInDir = null;

                if (fileTypes == null)
                    filesInDir = dir.GetFiles();
                else
                {
                    string search = "";

                    foreach (string fileType in fileTypes)
                        search += "*." + fileType;

                    filesInDir = dir.GetFiles(search);
                }

                foreach (FileInfo file in filesInDir)
                {
                    try
                    {
                        files.Add(file.FullName);
                    }
                    catch (PathTooLongException)
                    {
                        // suppress these
                    }
                }


                // handles folders
                DirectoryInfo[] dirs = dir.GetDirectories();

                foreach (DirectoryInfo child in dirs)
                    // check for both full path and short name match
                    if (ignoreFolders == null || (!ignoreFolders.Contains(child.FullName.ToLower()) && !ignoreFolders.Contains(child.Name)))
                        GetFilesUnderInternal(
                            child.FullName,
                            fileTypes,
                            ignoreFolders,
                            files);
            }
            catch (UnauthorizedAccessException ex)
            {
                // suppress these exceptions
            }
        }


        /// <summary>
        /// Gets a list of file names for files nested under a given path. Only files with
        /// the given filetype (extension) are returned
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ignorefolders"></param>
        /// <returns></returns>
        public static string[] GetFilesUnder(
            string path,
            List<string> ignorefolders
            )
        {
            if (ignorefolders != null)
                for (int i = 0; i < ignorefolders.Count; i++)
                    ignorefolders[i] = ignorefolders[i].ToLower();

            List<string> files = new List<string>();

            GetFilesUnderInternal(path, null, ignorefolders, files);

            return files.ToArray();

        }


        /// <summary>
        /// Gets a list of file names for files nested under a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetFilesUnder(string path)
        {

            List<string> files = new List<string>();

            GetFilesUnderInternal(path, null, null, files);

            return files.ToArray();

        }

        /// <summary>
        /// Maps an array of paths from one location to another.
        /// </summary>
        /// <returns></returns>
        public static string[] MapPaths(
            string[] sourceFiles,
            string srcRoot,
            string targetRoot
            )
        {
            srcRoot = srcRoot.ToLower();
            targetRoot = targetRoot.ToLower();

            string[] targetPaths = new string[sourceFiles.Length];

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                sourceFiles[i] = sourceFiles[i].ToLower();

                // ensure that the source root on the current path is valid
                if (!sourceFiles[i].StartsWith(srcRoot))
                    throw new MappingException(sourceFiles[i]);

                // needs to remove any trailing "\" from paths
                if (srcRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    srcRoot = srcRoot.Substring(0,
                        srcRoot.Length - Path.DirectorySeparatorChar.ToString().Length);
                if (targetRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    targetRoot = targetRoot.Substring(0,
                        targetRoot.Length - Path.DirectorySeparatorChar.ToString().Length);

                targetPaths[i] = sourceFiles[i].Replace(
                    srcRoot,
                    targetRoot);
            }

            return targetPaths;

        }

        /// <summary>
        /// Gets a list of folders nested in a path. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFoldersUnder(string path)
        {
            IList<string> folders = new List<string>();
            GetFoldersUnderInternal(path, folders);
            return folders;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folders"></param>
        private static void GetFoldersUnderInternal(
            string path,
            ICollection<string> folders
            )
        {
            try
            {
                if (!Directory.Exists(path))
                    return;

                folders.Add(path);

                // not empty, find folders and handle them
                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] dirs = dir.GetDirectories();

                foreach (DirectoryInfo child in dirs)
                    GetFoldersUnderInternal(
                        child.FullName,
                        folders);
            }
            catch (UnauthorizedAccessException ex)
            {
                // suppress these exceptions
            }
        }

        ///
        /// Deletes everything in a folder
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>True on no errors, false if there were errors.</returns>
        public static bool ClearDirectory(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (!dir.Exists)
                return false;

            bool noErrors = true;
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    noErrors = false;
                }
            }
            foreach (DirectoryInfo subDirectory in dir.GetDirectories())
            {
                try
                {
                    subDirectory.Delete(true);
                }
                catch
                {
                    noErrors = false;
                }
            }

            return noErrors;
        }

        /// <summary>
        /// Returns true of directory or any child regardless of depth contains children.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ContainsFiles(string path)
        {
            int fileCount = (from file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                             select file).Count();
            if (fileCount > 0)
                return true;

            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dir.GetDirectories();

            bool childrenContains = false;
            foreach (DirectoryInfo child in dirs)
            {
                if (ContainsFiles(child.FullName))
                {
                    childrenContains = true;
                    break;
                }
            }

            return childrenContains;
        }
    }
}
