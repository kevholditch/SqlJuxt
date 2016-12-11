using System.Collections.Generic;

namespace SqlJuxt.Scripting
{

    public class ColumnsBuilder
    {
        private readonly List<Column> _columns = new List<Column>();

        public ColumnsBuilder NonNullableInt(string name)
        {
            _columns.Add(new Column(name, new IntSqlType(), false));
            return this;
        }

        public ColumnsBuilder NullableInt(string name)
        {
            _columns.Add(new Column(name, new IntSqlType(), true));
            return this;
        }

        public ColumnsBuilder NonNullableVarchar(string name, int length)
        {
            _columns.Add(new Column(name, new VarcharSqlType(length), false));
            return this;
        }

        public ColumnsBuilder NullableVarchar(string name, int length)
        {
            _columns.Add(new Column(name, new VarcharSqlType(length), true));
            return this;
        }

        public List<Column> Build()
        {
            return _columns;
        }

        public static implicit operator List<Column>(ColumnsBuilder builder)
        {
            return builder.Build();
        }
    }
}