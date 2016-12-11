namespace SqlJuxt.Scripting
{
    public class VarcharSqlType : SqlType
    {
        public VarcharSqlType(int length)
        {
            Length = length;
        }

        public int Length { get; }
        public override string AsText()
        {
            return $"[varchar]({Length})";
        }
    }
}