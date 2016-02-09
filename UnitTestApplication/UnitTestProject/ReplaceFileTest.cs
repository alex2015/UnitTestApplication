using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
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

            var backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backup");
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SourceFile.txt");
            var targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TargetFile.txt");


            r.Replace(sourceFilePath, Path.Combine(backupDirectory, "TargetFile.txt"), targetFilePath);



            //var md5Hasher = MD5.Create();


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
