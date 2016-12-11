using System.IO;

namespace SqlJuxt.TestDatabase
{
    internal static class Scripts
    {
        public static Script CreateDatabaseNoFiles(string databaseName)
        {
            return GetScript("CreateDatabaseNoFiles.sql", databaseName);
        }

        public static Script GetDatabaseNames()
        {
            return GetScript("GetDatabaseNames.sql");
        }

        public static Script GetDatabaseFiles(string databaseName)
        {
            return GetScript("GetDatabaseFiles.sql", databaseName);
        }

        public static Script CreateDatabaseWithFiles(string databaseName, DirectoryInfo dataDirectory,
            DirectoryInfo logDirectory)
        {
            return GetScript("CreateDatabaseWithFiles.sql", databaseName, dataDirectory.FullName, logDirectory.FullName);
        }

        public static Script DropDatabase(string databaseName)
        {
            return GetScript("DropDatabase.sql", databaseName);
        }

        public static Script GetTableNames(string databaseName)
        {
            return GetScript("GetDatabaseTableNames.sql", databaseName);
        }

        private static Script GetScript(string scriptFileName, params object[] parameters)
        {
            return Script.FromThisAssembly(scriptFileName, parameters);
        }
    }
}