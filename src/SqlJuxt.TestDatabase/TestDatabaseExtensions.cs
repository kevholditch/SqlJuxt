namespace SqlJuxt.TestDatabase
{
    public static class TestDatabaseExtensions
    {
        public static TestDatabaseBatchDrop DropTestDatabasesInDomain(this IServerInstance instance, string domain)
        {
            return new TestDatabaseBatchDrop(instance, domain);
        }
    }
}