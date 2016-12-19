namespace SqlJuxtFunctionalTests


open FsUnit
open SqlJuxtFunctional.DatabaseBuilder.TableBuilder

    module TableBuilderTests =
        open Xunit

        [<Fact>]
        let ``should be able to build a table with a single nullable int column``() =
            Create "TestTable"
                |> WithNullableInt "Column1"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single non nullable int column``() =
            Create "TestTable"
                |> WithInt "Column1"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NOT NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single nullable varchar column``() =
            Create "VarTable"
                |> WithNullableVarchar "MyVarchar" 45
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](45) NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single non nullable varchar column``() =
            Create "VarTable"
                |> WithVarchar "MyVarchar" 10
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](10) NOT NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a mixture of columns``() =
            Create "MultiColumnTable"
                |> WithVarchar "MyVarchar" 10
                |> WithInt "MyInt"
                |> WithNullableVarchar "NullVarchar" 55
                |> WithNullableInt "NullInt"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[MultiColumnTable]( [MyVarchar] [varchar](10) NOT NULL, [MyInt] [int] NOT NULL, [NullVarchar] [varchar](55) NULL, [NullInt] [int] NULL )
GO"


