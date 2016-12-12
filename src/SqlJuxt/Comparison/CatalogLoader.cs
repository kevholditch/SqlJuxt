using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace SqlJuxt.Comparison
{
    public class CatalogLoader
    {

        public static Catalog Load(string catalogConnectionString)
        {
            var columnEntities = default(IEnumerable<ColumnEntity>);

            using (var connection = new SqlConnection(catalogConnectionString))
            {
                columnEntities = connection.Query<ColumnEntity>(@"select TABLE_SCHEMA as ""Schema"", TABLE_NAME as ""TableName"",   COLUMN_NAME as ""ColumnName"", ORDINAL_POSITION as ""OrdinalPosition"",
		                                                                            COLUMN_DEFAULT as ""Default"", IS_NULLABLE as ""IsNullable"", DATA_TYPE as ""DataType"", CHARACTER_MAXIMUM_LENGTH as ""CharacterMaxLength""
                                                                             from INFORMATION_SCHEMA.COLUMNS
                                                                             order by TABLE_NAME, ORDINAL_POSITION");
            }

            var tables = new List<Table>();

            var currentTableName = "";
            var columns = new List<ColumnEntity>();

            foreach (var columnEntity in columnEntities)
            {
                columns.Add(columnEntity);

                if (currentTableName != columnEntity.TableName)
                {
                    if (columns.Count > 0)
                    {
                        tables.Add(new Table(columns.First().TableName, columns));
                    }
                    columns = new List<ColumnEntity>();
                }
            }

            if (columns.Count > 0)
            {
                tables.Add(new Table(columns.First().TableName, columns));
            }

            return new Catalog("", tables);
        }
    }
}