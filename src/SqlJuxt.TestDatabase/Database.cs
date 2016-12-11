using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SqlJuxt.TestDatabase
{
    public sealed class Database : Disposable
    {
        public static readonly string[] SystemDatabaseNames = { "master", "model", "msdb", "tempdb" };

        public Database(IServerInstance serverInstance, string name)
        {
            ServerInstance = serverInstance;
            Name = name;
        }

        private IServerInstance ServerInstance { get; set; }

        public string Name { get; private set; }

        public bool IsSystem
        {
            get { return SystemDatabaseNames.Any(x => x.Equals(Name, StringComparison.InvariantCultureIgnoreCase)); }
        }

        public FileInfo[] GetFiles()
        {
            var files = new List<FileInfo>();

            using (var connection = OpenConnection())
            {
                using (var reader = connection.ExecuteReader(Scripts.GetDatabaseFiles(Name)))
                {
                    while (reader.Read())
                    {
                        files.Add(new FileInfo(reader.GetString(0)));
                    }
                }
            }

            return files.ToArray();
        }

        public string GetConnectionString()
        {
            var builder = new ConnectionStringBuilder();

            if (!ServerInstance.IntegratedSecurity)
            {
                builder.WithUserId(ServerInstance.UserId).WithPassword(ServerInstance.Password);
            }

            builder.WithServer(ServerInstance.Name).WithDatabaseName(Name);

            return builder.Build();
        }

        public SqlConnection OpenConnection()
        {
            var connectionString = GetConnectionString();
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}