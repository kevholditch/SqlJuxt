using System;
using System.Collections.Generic;
using SqlJuxt.TestDatabase;

namespace SqlJuxt.Tests
{
    public class LocalDbFixture : Disposable
    {
        protected LocalDbInstance LocalDbInstance { get; } = new LocalDbInstance();
        private readonly List<Database> _databases = new List<Database>();
        private const string Domain = "SqlJuxt";
        protected Database CreateDatabase()
        {
            var database = LocalDbInstance.CreateDatabase(new TestDatabaseName(Domain, TestingType.Acceptance, DateTime.UtcNow, Guid.NewGuid()));

            _databases.Add(database);

            return database;
        }

        protected override void Dispose(bool disposing)
        {
            if (LocalDbInstance != null)
            {
                foreach(var database in _databases)
                    LocalDbInstance.DropDatabase(database);
            }

            Drop.TestDatabases(LocalDbInstance, Domain)
                .OfTestingType(TestingType.Acceptance)
                .OlderThan(TimeSpan.FromHours(1))
                .Go();
            

        }
    }
}