using System.IO;
using ConsoleApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ReplaceFileTest
    {
        [TestMethod]
        [DeploymentItem(@"TestFiles\SourceFile.txt")]
        [DeploymentItem(@"TestFiles\TargetFile.txt")]
        public void TestMethod()
        {
            var r = new Replacement();

            var currentDirectory = Directory.GetCurrentDirectory();

            var backupDirectory = Path.Combine(currentDirectory, "Backup");
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            var sourceFilePath = Path.Combine(currentDirectory, "SourceFile.txt");
            var targetFilePath = Path.Combine(currentDirectory, "TargetFile.txt");
            var backupFilePath = Path.Combine(backupDirectory, "TargetFile.txt");

            var sourceFile = File.ReadAllBytes(sourceFilePath);
            var targetFileBefore = File.ReadAllBytes(targetFilePath);

            r.Replace(sourceFilePath, backupFilePath, targetFilePath);

            var targetFileAfter = File.ReadAllBytes(targetFilePath);
            var backupFile = File.ReadAllBytes(backupFilePath);

            Assert.AreEqual(EqualByteArray(sourceFile, targetFileAfter), true);
            Assert.AreEqual(EqualByteArray(targetFileBefore, backupFile), true);
        }

        private bool EqualByteArray(byte[] tmpHash, byte[] tmpNewHash)
        {
            var bEqual = false;

            if (tmpNewHash.Length == tmpHash.Length)
            {
                int i = 0;
                while ((i < tmpNewHash.Length) && (tmpNewHash[i] == tmpHash[i]))
                {
                    i += 1;
                }
                if (i == tmpNewHash.Length)
                {
                    bEqual = true;
                }
            }

            return bEqual;
        }
    }
}
