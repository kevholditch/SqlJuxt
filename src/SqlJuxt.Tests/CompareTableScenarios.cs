using FluentAssertions;
using Xbehave;
using Script = SqlJuxt.Scripting.Script;

namespace SqlJuxt.Tests
{
    public class CompareTableScenarios : LocalDbFixture
    {
        
        [Scenario]
        public void ShouldReturnIdenticalWhenSingleIdeniticalTableExistsOnBothSides()
        {
            var leftDatabase = CreateDatabase();
            var rightDatabase = CreateDatabase();
            var result = default(SchemaComparisonResult);
            var table = default(Script);

            "Given a table"
                ._(() => table = Sql.BuildScript()
                                    .WithTableNamed("MyTable", t => t.WithColumns(c => c.NullableVarchar("First", 23)
                                                                                        .NullableInt("Second"))));

            "And the table is created on the left database"
                ._(() => leftDatabase.RunScript(table));

            "And the table is created on the right database"
                ._(() => rightDatabase.RunScript(table));

            "When the two database schemas are compared"
                ._(() => result = Sql.CompareSchemas(leftDatabase.GetConnectionString(), rightDatabase.GetConnectionString()));

            "Then the schemas should be identical"
                ._(() => result.IsIdentical.Should().BeTrue());
        }
    }
}
