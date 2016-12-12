namespace SqlJuxt.Comparison
{
    public class ColumnEntity
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int OrdinalPosition { get; set; }
        public string Default { get; set; }
        public string IsNullable { get; set; }
        public string DataType { get; set; }
        public int? CharacterMaxLength { get; set; }

    }
}
