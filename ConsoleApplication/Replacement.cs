using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using ConsoleApplication.Privileges;

namespace ConsoleApplication
{
    public class Replacement
    {
        public void Replace(string sourcetFilePath, string backupFilePath, string targetFilePath)
        {
            if (File.Exists(targetFilePath))
            {
                File.Copy(targetFilePath, backupFilePath, true);

                using (new PrivilegeEnabler(Process.GetCurrentProcess(), Privilege.TakeOwnership))
                {
                    var fs = File.GetAccessControl(targetFilePath);

                    var ntAccount = new NTAccount(WindowsIdentity.GetCurrent().Name);
                    fs.SetOwner(ntAccount);
                    File.SetAccessControl(targetFilePath, fs);
                    fs = File.GetAccessControl(targetFilePath);
                    fs.AddAccessRule(new FileSystemAccessRule(ntAccount, FileSystemRights.FullControl, AccessControlType.Allow));
                    File.SetAccessControl(targetFilePath, fs);

                    File.Copy(sourcetFilePath, targetFilePath, true);
                }
            }
        }
    }
}
