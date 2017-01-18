namespace SqlJuxtFunctionalTests.Tests.TableBuilder


open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes

    module CreateColumnTests =
           
        [<Test>]
        let ``should be able to build a table with a single nullable int column``() =
            CreateTable "TestTable"
                |> WithNullableInt "Column1"
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NULL )
GO"

        [<Test>]
        let ``should be able to build a table with a single non nullable int column``() =
            CreateTable "TestTable"
                |> WithInt "Column1"
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NOT NULL )
GO"

        [<Test>]
        let ``should be able to build a table with a single nullable varchar column``() =
            CreateTable "VarTable"
                |> WithNullableVarchar "MyVarchar" 45
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](45) NULL )
GO"

        [<Test>]
        let ``should be able to build a table with a single non nullable varchar column``() =
            CreateTable "VarTable"
                |> WithVarchar "MyVarchar" 10
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](10) NOT NULL )
GO"

        [<Test>]
        let ``should be able to build a table with a mixture of columns``() =
            CreateTable "MultiColumnTable"
                |> WithVarchar "MyVarchar" 10
                |> WithInt "MyInt"
                |> WithNullableVarchar "NullVarchar" 55
                |> WithNullableInt "NullInt"
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MultiColumnTable]( [MyVarchar] [varchar](10) NOT NULL, [MyInt] [int] NOT NULL, [NullVarchar] [varchar](55) NULL, [NullInt] [int] NULL )
GO"

    module PrimaryKeyTests =

        [<Test>]
        let ``should be able to build a table with a clustered primary key on a single column asc``() =
            CreateTable "MyPrimaryKeyTable"
                |> WithInt "MyKeyColumn"
                |> WithClusteredPrimaryKey [("MyKeyColumn", ASC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyPrimaryKeyTable]( [MyKeyColumn] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyPrimaryKeyTable] ADD CONSTRAINT [PK_MyPrimaryKeyTable] PRIMARY KEY CLUSTERED ([MyKeyColumn] ASC)
GO"
        [<Test>]
        let ``should be able to build a table with a clustered primary key on a single column desc``() =
            CreateTable "MyPrimaryKeyTable"
                |> WithInt "MyKeyOtherColumn"                
                |> WithClusteredPrimaryKey [("MyKeyOtherColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyPrimaryKeyTable]( [MyKeyOtherColumn] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyPrimaryKeyTable] ADD CONSTRAINT [PK_MyPrimaryKeyTable] PRIMARY KEY CLUSTERED ([MyKeyOtherColumn] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a clustered primary key on a mulitple columns``() =
            CreateTable "MyPrimaryKeyTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithVarchar "ThirdCol" 50
                |> WithVarchar "ForthCol" 10
                |> WithClusteredPrimaryKey [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC); ("ThirdCol", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyPrimaryKeyTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL, [ThirdCol] [varchar](50) NOT NULL, [ForthCol] [varchar](10) NOT NULL )
GO

ALTER TABLE [dbo].[MyPrimaryKeyTable] ADD CONSTRAINT [PK_MyPrimaryKeyTable] PRIMARY KEY CLUSTERED ([MyKeyColumn] ASC, [SecondKeyColumn] DESC, [ThirdCol] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a non clustered primary key on a mulitple columns``() =
            CreateTable "RandomTableName"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithVarchar "ThirdCol" 50
                |> WithVarchar "ForthCol" 10
                |> WithNonClusteredPrimaryKey [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC); ("ThirdCol", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[RandomTableName]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL, [ThirdCol] [varchar](50) NOT NULL, [ForthCol] [varchar](10) NOT NULL )
GO

ALTER TABLE [dbo].[RandomTableName] ADD CONSTRAINT [PK_RandomTableName] PRIMARY KEY NONCLUSTERED ([MyKeyColumn] ASC, [SecondKeyColumn] DESC, [ThirdCol] DESC)
GO"

    module CreateMultipleTableTests =
        [<Test>]
        let ``should be create multiple tables``() =
            CreateCatalog ()
                |> WithTable (CreateTable "MyTable" |> WithInt "MyColumn")
                |> WithTable (CreateTable "MySecondTable" |> WithVarchar "MyVar" 10)
                |> Script
                |> should equal @"CREATE TABLE [dbo].[MyTable]( [MyColumn] [int] NOT NULL )
GO
CREATE TABLE [dbo].[MySecondTable]( [MyVar] [varchar](10) NOT NULL )
GO"


        