using System;
using System.Linq;
using FluentAssertions;
using SqlJuxt.TestDatabase;
using Xunit;

namespace SqlJuxt.TestDatbase.Tests
{

    public class LocalDbInstanceTests : IDisposable
    {
        private readonly LocalDbInstance _localDbInstance;

        public LocalDbInstanceTests()
        {
            _localDbInstance = new LocalDbInstance();
            _localDbInstance.DropTestDatabasesInDomain("SqlLocalDb").Go();
        }


        [Fact]
        public void ShouldListSystemDatabasesAlongWithAnyUserDatabases()
        {
            var databases = _localDbInstance.GetDatabases();

            databases.Should().NotBeEmpty();
            databases.Where(database => database.IsSystem).Select(x => x.Name).Should().BeEquivalentTo(Database.SystemDatabaseNames);

        }

        [Fact]
        public void ShouldNotAllowYouToDropASystemDatabase()
        {
            var databases = _localDbInstance.GetDatabases();

            var systemDatabases = databases.Where(database => database.Name.Equals("Master", StringComparison.InvariantCultureIgnoreCase)).ToList();
            var masterDatabase = systemDatabases.First();

            Action act = () => _localDbInstance.DropDatabase(masterDatabase);

            act.ShouldThrow<ArgumentException>().WithMessage(Errors.TryingToDropSystemDatabase(masterDatabase).Message);
        }

        [Fact]
        public void ShouldBeAbleToCreateAndThenDropADatabase()
        {

            var name = new TestDatabaseName("SqlLocalDb", TestingType.Integration, DateTime.Now);

            _localDbInstance.DropTestDatabasesInDomain("SqlLocalDb").Go();

            var databases = _localDbInstance.GetDatabases().Where(IsTestDatabase);

            databases.Should().BeEmpty();

            _localDbInstance.CreateDatabase(name.ToString());

            databases = _localDbInstance.GetDatabases().Where(IsTestDatabase);

            databases.Should().NotBeEmpty();

            foreach (var database in databases)
            {
                _localDbInstance.DropDatabase(database);
            }

            databases = _localDbInstance.GetDatabases().Where(IsTestDatabase);
            databases.Should().BeEmpty();

        }

        private static bool IsTestDatabase(Database database)
        {
            TestDatabaseName testDatabaseName;
            if (!TestDatabaseName.TryParse(database.Name, out testDatabaseName))
            {
                return false;
            }

            return testDatabaseName.Domain == "SqlLocalDb";
        }


        public void Dispose()
        {
            _localDbInstance.DropTestDatabasesInDomain("SqlLocalDb").Go();
        }
    }
}
