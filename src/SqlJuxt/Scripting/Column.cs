namespace SqlJuxt.Scripting
{
    public class Column
    {
        public Column(string name, SqlType type, bool isNullable)
        {
            Name = name;
            IsNullable = isNullable;
            Type = type;
        }

        public string Name { get; }
        public bool IsNullable { get; }
        public SqlType Type { get; }

        public string AsSqlText()
        {
            var nullAttribute = IsNullable ? "NULL" : "NOT NULL";
            return $"[{Name}] {Type.AsText()} {nullAttribute}";
        }
    }
}