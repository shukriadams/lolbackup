//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri.adams@gmail.com									//
//																					//
// vcFramework : A reuseable library of utility classes                             //
// Copyright (C)																	//
//////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;
using vcFramework.Parsers;
using System.Collections.Generic;

namespace vcFramework.IO
{
    /// <summary> 
    /// Static library of file io-related methods 
    /// </summary>
    public class FileSystemLib
    {
        #region METHODS

        /// <summary>
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
        /// Gets a count of all directories under the given one, not matter how far down they may be nested 
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="intFolderCount"></param>
        public static void CountAllDirectoriesUnder(
            string strPath,
            ref int intFolderCount
            )
        {
            string[] childDirectories = null;

            intFolderCount++;

            //gets list of all directories inside current directories
            try
            {
                childDirectories = Directory.GetDirectories(strPath);
            }
            catch
            {
                //do nothing
            }

            //calls this method again for all subdirectories in this directory. this is where recursion occurs
            for (int i = 0; i < childDirectories.Length; i++)
            {
                try
                {
                    CountAllDirectoriesUnder(childDirectories[i], ref intFolderCount);
                }
                catch
                {
                    // do nothing
                }
            }

        }


        /// <summary> 
        /// Gets a count of all files under a given folder, no matter how far down tehy may be nested 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileCount"></param>
        public static void CountAllFilesUnder(
            string path,
            ref int fileCount
            )
        {
            string[] fileList = null;
            string[] childDirectories = null;

            // gets number of files and adds to file count
            try
            {
                fileList = Directory.GetFiles(path);
                fileCount += fileList.Length;
            }
            catch
            {
                // suppress
            }


            //gets list of all directories inside current directories
            try
            {
                childDirectories = Directory.GetDirectories(path);
            }
            catch
            {
                // suppress
            }

            //calls this method again for all subdirectories in this directory. this is where recursion occurs
            for (int i = 0; i < childDirectories.Length; i++)
            {
                try
                {
                    CountAllFilesUnder(childDirectories[i], ref fileCount);
                }
                catch
                {
                    // suppress
                }
            }
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
            catch (PathTooLongException)
            {
                // suppress these, they will be hit a lot
            }
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
            catch (UnauthorizedAccessException)
            {
                // suppress these exceptions
            }
            catch (PathTooLongException)
            {
                // suppress these, they will be hit a lot
            }
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
            // needs to remove any trailing "\" from paths
            if (srcRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                srcRoot = srcRoot.Substring(0,
                    srcRoot.Length - Path.DirectorySeparatorChar.ToString().Length);

            if (targetRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                targetRoot = targetRoot.Substring(0,
                    targetRoot.Length - Path.DirectorySeparatorChar.ToString().Length);

            // user lower case comparers to avoid false negatives
            string srcRootComparer = srcRoot.ToLower();

            string[] targetPaths = new string[sourceFiles.Length];

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                string sourceFileComparer = sourceFiles[i].ToLower();

                // ensure that the source root on the current path is valid
                if (!sourceFileComparer.StartsWith(srcRootComparer))
                    throw new MappingException(sourceFiles[i]);

                targetPaths[i] = ParserLib.ReplaceNoCase(sourceFiles[i], srcRoot, targetRoot);
            }

            return targetPaths;

        }


        #endregion
    }
}
