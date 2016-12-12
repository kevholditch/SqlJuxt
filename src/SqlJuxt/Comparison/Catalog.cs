using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace SqlJuxt.Comparison
{
    public class Catalog
    {
        public Catalog(string name, List<Table> tables)
        {
            Name = name;
            Tables = new ReadOnlyCollection<Table>(tables);
        }

        public string Name { get; }

        public ReadOnlyCollection<Table> Tables { get; }
    }
}