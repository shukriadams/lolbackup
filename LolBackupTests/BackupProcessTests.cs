using System;
using System.IO;
using LolBackup;
using NUnit.Framework;

namespace LolBackupTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class BackupProcessTests
    {
        private static string _workfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LolBackupTestsTempFolder");

        [TestFixtureSetUp]
        public static void MyClassInitialize() 
        {
            if (!Directory.Exists(_workfolder))
                Directory.CreateDirectory(_workfolder);
        }
       
        [SetUp]
        public void MyTestInitialize() 
        {
            FileSystemLib.ClearDirectory(_workfolder);
        }

       
        [TearDown]
        public void MyTestCleanup() 
        {
            FileSystemLib.ClearDirectory(_workfolder);
        }

        private string CreateSourceFile(string filename, string filecontent) 
        {
            // create source content
            string sourceDir = Path.Combine(_workfolder, "source");
            Directory.CreateDirectory(sourceDir);
            string file = Path.Combine(sourceDir, filename);
            File.WriteAllText(file, filecontent);
            return file;
        }

        private string CreateTargetFile(string filename, string filecontent)
        {
            // create source content
            string sourceDir = Path.Combine(_workfolder, "target");
            Directory.CreateDirectory(sourceDir);
            string file = Path.Combine(sourceDir, filename);
            File.WriteAllText(file, filecontent);
            return file;
        }

        [Test]
        public void BackupFile()
        {
            // create source content
            string sourceDir = Path.GetDirectoryName(CreateSourceFile("test.txt", "test123"));

            // create target
            string targetDir = Path.Combine(_workfolder, "target");
            Directory.CreateDirectory(targetDir);

            // do
            BackupProcess backup = new BackupProcess(sourceDir, targetDir, null, null, null, null);
            backup.Process();

            // test
            string targetFile = Path.Combine(targetDir, "test.txt");
            Assert.IsTrue(File.Exists(targetFile));
            Assert.IsTrue(File.ReadAllText(targetFile) == "test123");
        }

        /// <summary>
        /// tests that an orphan file is deleted
        /// </summary>
        [Test]
        public void DeleteOrphanBackup()
        {
            // create source content
            string sourceDir = Path.GetDirectoryName(CreateSourceFile("test.txt", "test123"));
            string targetDir = Path.GetDirectoryName(CreateTargetFile("test2.txt", "test123"));

            // do
            BackupProcess backup = new BackupProcess(sourceDir, targetDir, null, null, null, null);
            backup.DeleteOrphans = true;
            backup.Process();

            // test
            string targetFile = Path.Combine(targetDir, "test2.txt");
            Assert.IsFalse(File.Exists(targetFile));
        }

        /// <summary>
        /// tests that an existing orphan file is not deleted
        /// </summary>
        [Test]
        public void DoNotOrhanBackup()
        {
            // create source content
            string sourceDir = Path.GetDirectoryName(CreateSourceFile("test.txt", "test123"));
            string targetDir = Path.GetDirectoryName(CreateTargetFile("test2.txt", "test123"));

            // do
            BackupProcess backup = new BackupProcess(sourceDir, targetDir, null, null, null, null);
            backup.Process();

            // test
            string targetFile = Path.Combine(targetDir, "test2.txt");
            Assert.IsTrue(File.Exists(targetFile));
        }

        [Test]
        public void DeleteEmptyOrphanDirectories()
        {
            // create source content
            string sourceDir = Path.GetDirectoryName(CreateSourceFile("test.txt", "test123"));
            string targetDir = Path.GetDirectoryName(CreateTargetFile("test2.txt", "test123"));

            // deleted deep nested folder
            string orphanDirectory = Path.Combine(_workfolder, "target", "orphan", "layer2", "layer3");
            Directory.CreateDirectory(orphanDirectory);

            // do
            BackupProcess backup = new BackupProcess(sourceDir, targetDir, null, null, null, null);
            backup.DeleteOrphans = true;
            backup.Process();

            // test
            Assert.IsFalse(Directory.Exists(orphanDirectory));
        }

        [Test]
        public void DoNotDeleteDeepNestedDirectories()
        {
            // create source content
            string sourceDir = Path.Combine(_workfolder, "source");
            string targetDir = Path.Combine(_workfolder, "target");


            // created a deep nested folder with a file in it
            string deepSourceDirectory = Path.Combine(_workfolder, "source", "orphan", "layer2", "layer3");
            Directory.CreateDirectory(deepSourceDirectory);
            File.WriteAllText(Path.Combine(deepSourceDirectory, "dummy.txt"), "nothing");

            string deepTargetDirectory = Path.Combine(_workfolder, "target", "orphan", "layer2", "layer3");
            Directory.CreateDirectory(deepTargetDirectory);
            File.WriteAllText(Path.Combine(deepTargetDirectory, "dummy.txt"), "nothing");

            // double check that lib can detected nested content
            Assert.IsTrue( FileSystemLib.ContainsFiles(targetDir));

            // do
            BackupProcess backup = new BackupProcess(sourceDir, targetDir, null, null, null, null);
            backup.DeleteOrphans = true;
            backup.Process();

            // test
            Assert.IsTrue(Directory.Exists(deepSourceDirectory));
            Assert.IsTrue(File.Exists(Path.Combine(deepSourceDirectory, "dummy.txt")));
            Assert.IsTrue(Directory.Exists(deepTargetDirectory));
            Assert.IsTrue(File.Exists(Path.Combine(deepTargetDirectory, "dummy.txt")));
        }

    }
}
