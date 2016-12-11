using System.IO;

namespace SqlJuxt.TestDatabase
{
    public interface IServerInstance
    {
        string UserId { get; }
        string Password { get; }
        string Name { get; }
        bool IntegratedSecurity { get; }

        Database[] GetDatabases();
        void DropDatabase(Database database);
        Database CreateDatabase(string name);
        Database CreateDatabase(string name, DirectoryInfo dataDirectory, DirectoryInfo logDirectory);
    }
}