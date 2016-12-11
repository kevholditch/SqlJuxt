namespace SqlJuxt.TestDatabase
{
    public static class Drop
    {
        public static TestDatabaseBatchDrop TestDatabases(IServerInstance serverInstance, string domain)
        {
            return new TestDatabaseBatchDrop(serverInstance, domain);
        }
    }
}