using System;
using System.Threading.Tasks;

namespace SqlJuxt.TestDatabase
{
    public static class Errors
    {
        public static Exception TryingToDropSystemDatabase(Database database)
        {
            return new ArgumentException(Message("You are trying to drop database '{0}'. And its a system databases and that is not allowed", database.Name), "database");
        }

        private static string Message(string text, params object[] args)
        {
            return string.Format(text, args);
        }
    }
}
