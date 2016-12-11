using System.Configuration;
using System.IO;
using System.Linq;

namespace SqlJuxt.TestDatabase
{
    public class LocalDbInstance : ServerInstance
    {
        public LocalDbInstance() : base(GetInstalledServerInstanceName())
        {
        }

        public static string GetInstalledServerInstanceName()
        {
            var sqlServerDirectory = new DirectoryInfo(@"C:\Program Files\Microsoft SQL Server");
            var localDbDirectories = sqlServerDirectory.GetDirectories("LocalDb", SearchOption.AllDirectories).OrderByDescending(x => x.Parent.Name);

            if (!localDbDirectories.Any())
            {
                throw new ConfigurationErrorsException("No instance of local db is installed on this machine.");
            }

            var localDbDirectory = localDbDirectories.First();
            int versionNumber;

            if (!int.TryParse(localDbDirectory.Parent.Name, out versionNumber))
            {
                throw new ConfigurationErrorsException("No instance of local db is installed on this machine.");
            }

            return versionNumber == 110 ? @"(LocalDB)\v11.0" : @"(LocalDB)\MSSQLLocalDB";
        }
    }
}