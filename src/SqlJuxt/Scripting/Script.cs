using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace SqlJuxt.Scripting
{
    public class Script
    {
        
        public Script(string sqlText)
        {
            SqlText = sqlText;
        }

        public string SqlText { get; }

        public static Script FromThisAssembly(string scriptFileName, params object[] parameters)
        {
            var sqlText = AssemblyResource.InThisAssembly(scriptFileName).GetText().MergeParameters(parameters);
            return new Script(sqlText);
        }

        public static Script FromAssembly(Assembly assembly, string scriptFileName, params object[] parameters)
        {
            var sqlText = AssemblyResource.InAssembly(assembly, scriptFileName).GetText().MergeParameters(parameters);
            return new Script(sqlText);
        }

        public static Script FromCallingAssembly(string scriptFileName, params object[] parameters)
        {
            return FromAssembly(Assembly.GetCallingAssembly(), scriptFileName, parameters);
        }

        public ReadOnlyCollection<string> GetScriptBlocks()
        {
            var scriptBlocks = new List<string>();

            var reader = new StringReader(SqlText);

            var scriptBlockBuilder = new StringBuilder();

            string line;

            while ((line = reader.ReadLine()) != null)
            {

                if (line.Trim().Equals("GO", StringComparison.InvariantCultureIgnoreCase))
                {
                    scriptBlocks.Add(scriptBlockBuilder.ToString());
                    scriptBlockBuilder.Clear();
                }
                else
                {
                    scriptBlockBuilder.AppendLine(line);
                }

            }

            if (scriptBlockBuilder.Length > 0)
            {
                scriptBlocks.Add(scriptBlockBuilder.ToString());
            }

            return new ReadOnlyCollection<string>(scriptBlocks);
        }

        public static implicit operator string(Script script)
        {
            return script.SqlText;
        }
    }
}