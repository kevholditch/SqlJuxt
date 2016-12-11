using System.Collections.Generic;
using System.IO;

namespace SqlJuxt.TestDatabase
{
    public class ServerInstance : IServerInstance
    {
        public ServerInstance(string name)
        {
            Name = name;
        }

        public ServerInstance(string name, string userId, string password)
        {
            Name = name;
            UserId = userId;
            Password = password;
        }

        public string UserId { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }

        public bool IntegratedSecurity
        {
            get { return string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(Password); }
        }

        public Database[] GetDatabases()
        {
            var databases = new List<Database>();

            using (var connection = GetMasterDatabase().OpenConnection())
            {
                using (var reader = connection.ExecuteReader(Scripts.GetDatabaseNames()))
                {
                    while (reader.Read())
                    {
                        var databaseName = reader.GetString(0);
                        databases.Add(new Database(this, databaseName));
                    }
                }

                return databases.ToArray();
            }
        }

        protected Database GetMasterDatabase()
        {
            return new Database(this, "Master");
        }

        public void DropDatabase(Database database)
        {
            if (database.IsSystem)
            {
                throw Errors.TryingToDropSystemDatabase(database);
            }

            using (var connection = GetMasterDatabase().OpenConnection())
            {
                connection.ExecuteScript(Scripts.DropDatabase(database.Name));
            }
        }

        public Database CreateDatabase(string name)
        {
            using (var connection = GetMasterDatabase().OpenConnection())
            {
                connection.ExecuteScript(Scripts.CreateDatabaseNoFiles(name));
                return new Database(this, name);
            }
        }

        public Database CreateDatabase(string name, DirectoryInfo dataDirectory, DirectoryInfo logDirectory)
        {
            using (var connection = GetMasterDatabase().OpenConnection())
            {
                connection.ExecuteScript(Scripts.CreateDatabaseWithFiles(name, dataDirectory, logDirectory));
                return new Database(this, name);
            }
        }
    }
}