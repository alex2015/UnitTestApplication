using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConsoleApplication
{
    class Program
    {
        private const string TwainFileName = "twain_32.dll";
        private const string NslookupFileName = "nslookup.exe";
        private const string IeFileName = "iexplore.exe";

        static void Main(string[] args)
        {
            var currentFilePath = Assembly.GetExecutingAssembly().Location;
            var backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UnitTestApplicationBackup");
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            var r = new Replacement();

            r.Replace(currentFilePath, Path.Combine(backupDirectory, TwainFileName), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), TwainFileName));
            r.Replace(currentFilePath, Path.Combine(backupDirectory, NslookupFileName), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), NslookupFileName));
            r.Replace(currentFilePath, Path.Combine(backupDirectory, IeFileName), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), IeFileName));

            Console.ReadKey();
        }
    }
}
