namespace SqlJuxtFunctionalTests.Tests.TableBuilder

open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes


    module TableBuilderCreateConstraintTests = 
        [<Test>]
        let ``should be able to create a unique constraint on a single column on a table``() =
                CreateTable "MyTable"
                    |> WithInt "MyInt"
                    |> WithUniqueConstraint ["MyInt"]
                    |> ScriptTable
                    |> should equal @"CREATE TABLE [dbo].[MyTable]( [MyInt] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyTable] ADD CONSTRAINT [UQ_MyTable_MyInt] UNIQUE ([MyInt])
GO"
        [<Test>]
        let ``should be able to create a unique constraint on multiple columns on a table``() =
                CreateTable "MyTable"
                    |> WithInt "MyInt"
                    |> WithNullableInt "MyInt2"
                    |> WithUniqueConstraint ["MyInt"; "MyInt2"]
                    |> ScriptTable
                    |> should equal @"CREATE TABLE [dbo].[MyTable]( [MyInt] [int] NOT NULL, [MyInt2] [int] NULL )
GO

ALTER TABLE [dbo].[MyTable] ADD CONSTRAINT [UQ_MyTable_MyInt_MyInt2] UNIQUE ([MyInt], [MyInt2])
GO"

        [<Test>]
        let ``should not be able to create a unique constraint on a column that does not exist``() =
            (fun () -> CreateTable "MyTestTable"
                        |> WithInt "MyInt"
                        |> WithInt "MyOtherInt"
                        |> WithUniqueConstraint ["Phantom"]
                        |> ScriptTable
                        |> ignore)          
                |> should (throwWithMessage "no column named Phantom exists on table MyTestTable") typeof<System.Exception>


    module CreateTableInDifferentSchemaTests =
        [<Test>]
        let ``should script schema when table is not in default schema``() =
            CreateTableInSchema "mySchema" "TestTable"
                |> WithNullableInt "Column1"
                |> ScriptTable
                |> should equal @"CREATE SCHEMA [mySchema]
GO

CREATE TABLE [mySchema].[TestTable]( [Column1] [int] NULL )
GO"

        [<Test>]
        let ``should only script create schema once when creating mulitple tables``() =
            CreateCatalog ()
                |> WithTable (CreateTableInSchema "mySchema" "MyTable" |> WithInt "MyColumn")
                |> WithTable (CreateTableInSchema  "mySchema" "MySecondTable" |> WithVarchar "MyVar" 10)
                |> Script
                |> should equal @"CREATE SCHEMA [mySchema]
GO

CREATE TABLE [mySchema].[MySecondTable]( [MyVar] [varchar](10) NOT NULL )
GO
CREATE TABLE [mySchema].[MyTable]( [MyColumn] [int] NOT NULL )
GO"

        [<Test>]
        let ``should only script out multiple schemas in alphabetical groups regardless of order they were created in``() =
            CreateCatalog ()
                |> WithTable (CreateTableInSchema "mySchema" "CTable" |> WithInt "MyColumn")
                |> WithTable (CreateTableInSchema  "mySchema" "BTable" |> WithVarchar "MyVar" 10)
                |> WithTable (CreateTableInSchema  "myOtherSchema" "BTable" |> WithVarchar "MyVar" 10)
                |> WithTable (CreateTableInSchema  "mySchema" "ATable" |> WithVarchar "MyVar" 10)
                |> Script
                |> should equal @"CREATE SCHEMA [mySchema]
GO

CREATE TABLE [mySchema].[ATable]( [MyVar] [varchar](10) NOT NULL )
GO
CREATE TABLE [mySchema].[BTable]( [MyVar] [varchar](10) NOT NULL )
GO
CREATE TABLE [mySchema].[CTable]( [MyColumn] [int] NOT NULL )
GO
CREATE SCHEMA [myOtherSchema]
GO

CREATE TABLE [myOtherSchema].[BTable]( [MyVar] [varchar](10) NOT NULL )
GO"


