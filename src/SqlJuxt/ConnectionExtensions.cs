using System.Data;
using SqlJuxt.Scripting;

namespace SqlJuxt
{
    public static class ConnectionExtensions
    {
        public static int ExecuteSql(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql.MergeParameters(parameters);
                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery();
            }
        }

        public static T ExecuteScalar<T>(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql.MergeParameters(parameters);
                command.CommandType = CommandType.Text;
                var value = command.ExecuteScalar();
                return (T)value;
            }
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql.MergeParameters(parameters);
                command.CommandType = CommandType.Text;
                return command.ExecuteReader();
            }
        }

        public static void ExecuteScript(this IDbConnection connection, Script script)
        {
            foreach (var scriptBlock in script.GetScriptBlocks())
            {
                connection.ExecuteSql(scriptBlock);
            }
        }

        internal static string MergeParameters(this string sql, params object[] parameters)
        {
            return parameters.Length == 0 ? sql : string.Format(sql, parameters);
        }
    }
}