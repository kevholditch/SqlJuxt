using System;
using System.Collections.Generic;
using System.Text;

namespace SqlJuxt.Scripting
{
    public class TableBuilder
    {
        public readonly List<Column> Columns = new List<Column>();

        public TableBuilder(string name)
        {
            Name = name;
        }


        public TableBuilder WithColumns(Func<ColumnsBuilder, List<Column>> buildColumns)
        {
            Columns.AddRange(buildColumns(new ColumnsBuilder()));
            return this;
        }

        public string Name { get; }

        public string Build()
        {
            var stringBuilder = new StringBuilder($"CREATE TABLE [dbo].[{Name}](");
            stringBuilder.AppendLine();

            for (var i = 0; i < Columns.Count; i++)
            {
                stringBuilder.Append(Columns[i].AsSqlText());
                if (i != Columns.Count - 1)
                    stringBuilder.Append(",");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(")");
            stringBuilder.AppendLine("GO");

            return stringBuilder.ToString();
        }

        public static implicit operator string(TableBuilder tableBuilder)
        {
            return tableBuilder.Build();
        }

    }
}