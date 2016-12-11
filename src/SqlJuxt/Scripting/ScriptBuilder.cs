using System;
using System.Collections.Generic;
using System.Text;

namespace SqlJuxt.Scripting
{
    public class ScriptBuilder
    {
        private readonly Dictionary<string, Func<TableBuilder, string>> _tables = new Dictionary<string, Func<TableBuilder, string>>();
        public ScriptBuilder WithTableNamed(string name, Func<TableBuilder, string> buildTable)
        {
            _tables.Add(name, buildTable);
            return this;
        }

        public Script Build()
        {
            var stringBuilder = new StringBuilder();

            foreach(var tableName in _tables.Keys)
            {
                stringBuilder.AppendLine(_tables[tableName](new TableBuilder(tableName)));
            }

            return new Script(stringBuilder.ToString());
        }

        public static implicit operator Script(ScriptBuilder scriptBuilder)
        {
            return scriptBuilder.Build();
        }
        
    }
}