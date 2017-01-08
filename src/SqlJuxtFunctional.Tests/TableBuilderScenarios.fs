namespace SqlJuxtFunctionalTests


open FsUnit
open SqlJuxtFunctional.DatabaseBuilder.TableBuilder

    module TableBuilderTests =
        open Xunit
        open SqlJuxtFunctional.DatabaseTypes

        [<Fact>]
        let ``should be able to build a table with a single nullable int column``() =
            CreateTable "TestTable"
                |> WithNullableInt "Column1"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single non nullable int column``() =
            CreateTable "TestTable"
                |> WithInt "Column1"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[TestTable]( [Column1] [int] NOT NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single nullable varchar column``() =
            CreateTable "VarTable"
                |> WithNullableVarchar "MyVarchar" 45
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](45) NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a single non nullable varchar column``() =
            CreateTable "VarTable"
                |> WithVarchar "MyVarchar" 10
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[VarTable]( [MyVarchar] [varchar](10) NOT NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a mixture of columns``() =
            CreateTable "MultiColumnTable"
                |> WithVarchar "MyVarchar" 10
                |> WithInt "MyInt"
                |> WithNullableVarchar "NullVarchar" 55
                |> WithNullableInt "NullInt"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[MultiColumnTable]( [MyVarchar] [varchar](10) NOT NULL, [MyInt] [int] NOT NULL, [NullVarchar] [varchar](55) NULL, [NullInt] [int] NULL )
GO"

        [<Fact>]
        let ``should be able to build a table with a clustered primary key on a single column asc``() =
            CreateTable "MyPrimaryKeyTable"
                |> WithInt "MyKeyColumn"
                |> WithPrimaryKeyNamed "PK_MyPrimaryKey" [("MyKeyColumn", ASC)]
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[MyPrimaryKeyTable]( [MyKeyColumn] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyPrimaryKeyTable] ADD CONSTRAINT [PK_MyPrimaryKey] PRIMARY KEY CLUSTERED ([MyKeyColumn] ASC)
GO"
        [<Fact>]
        let ``should be able to build a table with a clustered primary key on a single column desc``() =
            CreateTable "MyPrimaryKeyTable"
                |> WithInt "MyKeyOtherColumn"
                |> WithPrimaryKeyNamed "PK_MyPrimaryKey" [("MyKeyOtherColumn", DESC)]
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[MyPrimaryKeyTable]( [MyKeyOtherColumn] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyPrimaryKeyTable] ADD CONSTRAINT [PK_MyPrimaryKey] PRIMARY KEY CLUSTERED ([MyKeyOtherColumn] DESC)
GO"
        