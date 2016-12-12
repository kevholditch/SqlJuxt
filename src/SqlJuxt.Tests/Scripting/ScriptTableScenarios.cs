using FluentAssertions;
using Xunit;

namespace SqlJuxt.Tests.Scripting
{
    public class ScriptTableScenarios
    {
        [Fact]
        public void CanBuildTableWithSingleNullableIntColumn()
        {
            string result = Sql.BuildScript()
                               .WithTableNamed("TestTable", t => t.WithColumns(c => c.NullableInt("Column1")))
                               .Build();

            result.Should().Be(@"CREATE TABLE [dbo].[TestTable](
[Column1] [int] NULL
)
GO

");
        }

        [Fact]
        public void CanBuildTableWithSingleNonNullableIntColumn()
        {
            string result = Sql.BuildScript()
                               .WithTableNamed("TestTable", t => t.WithColumns(c => c.NonNullableInt("Column1")))
                               .Build();

            result.Should().Be(@"CREATE TABLE [dbo].[TestTable](
[Column1] [int] NOT NULL
)
GO

");
        }

        [Fact]
        public void CanBuildTableWithSingleNullableVarcharColumn()
        {
            string result = Sql.BuildScript()
                               .WithTableNamed("TestTable", t => t.WithColumns(c => c.NullableVarchar("Column1", 50)))
                               .Build();

            result.Should().Be(@"CREATE TABLE [dbo].[TestTable](
[Column1] [varchar](50) NULL
)
GO

");
        }
        [Fact]
        public void CanBuildTableWithSingleNonNullableVarcharColumn()
        {
            string result = Sql.BuildScript()
                               .WithTableNamed("TestTable", t => t.WithColumns(c => c.NonNullableVarchar("Column1", 12)))
                               .Build();

            result.Should().Be(@"CREATE TABLE [dbo].[TestTable](
[Column1] [varchar](12) NOT NULL
)
GO

");
        }

        [Fact]
        public void CanBuildTableWithMulitpleColumns()
        {
            string result = Sql.BuildScript()
                               .WithTableNamed("TestTable", t => t.WithColumns(c => c.NonNullableInt("Column1")
                                                                                     .NullableInt("Column2")
                                                                                     .NonNullableVarchar("Column3", 30)
                                                                                     .NullableVarchar("Column4", 25)))
                               .Build();

            result.Should().Be(@"CREATE TABLE [dbo].[TestTable](
[Column1] [int] NOT NULL,
[Column2] [int] NULL,
[Column3] [varchar](30) NOT NULL,
[Column4] [varchar](25) NULL
)
GO

");
        }
    }
}
