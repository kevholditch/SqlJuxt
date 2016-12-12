using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SqlJuxt.Comparison
{
    public class Table
    {
        public Table(string name, List<ColumnEntity> columns)
        {
            Name = name;
            Columns = new ReadOnlyCollection<ColumnEntity>(columns);
        }

        public string Name { get; }

        public ReadOnlyCollection<ColumnEntity> Columns { get; } 
    }
}