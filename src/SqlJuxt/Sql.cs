using System;
using System.Collections.Generic;
using SqlJuxt.Scripting;

namespace SqlJuxt
{
    public static class Sql
    {
        public static SchemaComparisonResult CompareSchemas(string leftCatalogConnectionString, string rightCatalogConnectionString)
        {
            return null;
        }

        public static ScriptBuilder BuildScript()
        {
            return new ScriptBuilder();
        }
    }

}
