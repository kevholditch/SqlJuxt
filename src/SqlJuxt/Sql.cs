using SqlJuxt.Comparison;
using SqlJuxt.Scripting;

namespace SqlJuxt
{
    public static class Sql
    {

        public static SchemaComparisonResult CompareSchemas(string leftCatalogConnectionString,
            string rightCatalogConnectionString)
        {
            var leftSchema = CatalogLoader.Load(leftCatalogConnectionString);
            var rightSchema = CatalogLoader.Load(rightCatalogConnectionString);

            if (leftSchema.Tables.Count != rightSchema.Tables.Count)
                return new SchemaComparisonResult(false);

        

            return new SchemaComparisonResult(true);
        }

        public static ScriptBuilder BuildScript()
        {
            return new ScriptBuilder();
        }
    }

}
