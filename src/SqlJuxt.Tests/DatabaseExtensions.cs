using System.Data.SqlClient;
using SqlJuxt.TestDatabase;
using Script = SqlJuxt.Scripting.Script;

namespace SqlJuxt.Tests
{
    public static class DatabaseExtensions
    {
        public static void RunScript(this Database database, Script script)
        {
            using (var connection = new SqlConnection(database.GetConnectionString()))
            {
                connection.Open();

                foreach (var scriptBlock in script.GetScriptBlocks())
                {
                    connection.ExecuteSql(scriptBlock);
                }


                connection.Close();
            }
        }
    }
}