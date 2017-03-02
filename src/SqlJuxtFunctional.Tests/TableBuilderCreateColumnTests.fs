namespace SqlJuxtFunctionalTests.Tests.TableBuilder


open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes

    module TableBuilderCreateColumnTests =
           
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

