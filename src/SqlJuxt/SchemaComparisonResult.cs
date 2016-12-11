namespace SqlJuxt
{
    public class SchemaComparisonResult
    {
        public SchemaComparisonResult(bool isIdentical)
        {
            IsIdentical = isIdentical;
        }

        public bool IsIdentical { get; private set; }
    }
}