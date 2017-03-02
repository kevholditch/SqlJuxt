namespace SqlJuxtFunctionalTests.Tests.TableBuilder


open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes

    
    module TableBuilderCreatePrimaryKeyTests =

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

        [<Test>]
        let ``should not be able to build a table with a nullable column as part of a primary key``() =
            (fun () -> CreateTable "InvalidTable"
                        |> WithNullableInt "NullableCol"
                        |> WithNonClusteredPrimaryKey [("NullableCol", ASC)]
                        |> ScriptTable
                        |> ignore)          
                |> should (throwWithMessage "column named NullableCol is nullable, nullable columns are not allowed as part of a primary key") typeof<System.Exception>


