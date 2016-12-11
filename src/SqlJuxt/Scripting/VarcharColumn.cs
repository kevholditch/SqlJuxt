namespace SqlJuxt.Scripting
{
    public class IntSqlType : SqlType
    {
        public override string AsText()
        {
            return "[int]";
        }
    }
}