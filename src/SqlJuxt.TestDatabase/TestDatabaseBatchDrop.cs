using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlJuxt.TestDatabase
{
    public class TestDatabaseBatchDrop
    {
        private readonly List<Func<TestDatabaseName, bool>> _predicates;
        private readonly IServerInstance _serverInstance;

        public TestDatabaseBatchDrop(IServerInstance serverInstance, string domain)
        {
            _serverInstance = serverInstance;
            _predicates = new List<Func<TestDatabaseName, bool>> {name => name.Domain == domain};
        }

        public TestDatabaseBatchDrop OfTestingType(TestingType testingType)
        {
            _predicates.Add(name => name.TestingType == testingType);
            return this;
        }

        public TestDatabaseBatchDrop OlderThan(TimeSpan span)
        {
            _predicates.Add(name => name.CreatedAt <= DateTime.Now.Subtract(span));
            return this;
        }

        public void Go()
        {
            var databases = _serverInstance.GetDatabases();

            foreach (var database in databases)
            {
                TestDatabaseName testDatabaseName;

                if (!TestDatabaseName.TryParse(database.Name, out testDatabaseName)) continue;

                if (_predicates.All(compute => compute(testDatabaseName)))
                {
                    _serverInstance.DropDatabase(database);
                }
            }
        }
    }
}