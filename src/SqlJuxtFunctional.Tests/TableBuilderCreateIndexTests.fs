namespace SqlJuxtFunctionalTests.Tests.TableBuilder

open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes

    module TableBuilderCreateIndexTests =
        [<Test>]
        let ``should be able to build a table with a nonunique clustered index``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithClusteredIndex NONUNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE CLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a unique clustered index``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithClusteredIndex UNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE UNIQUE CLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a nonunique nonclustered index``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithNonClusteredIndex NONUNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a unique nonclustered index``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithNonClusteredIndex UNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE UNIQUE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO"

        [<Test>]
        let ``should be able to build a table with a multiple different indexes``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithNonClusteredIndex UNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> WithNonClusteredIndex NONUNIQUE [("MyKeyColumn", ASC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE UNIQUE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO


CREATE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC)
GO"

        [<Test>]
        let ``should name indexes sequentially when there are multiple indexes defined that would generate the same name``() =
            CreateTable "MyIndexedTable"
                |> WithInt "MyKeyColumn"
                |> WithInt "SecondKeyColumn"
                |> WithNonClusteredIndex UNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> WithNonClusteredIndex UNIQUE [("MyKeyColumn", ASC); ("SecondKeyColumn", DESC)]
                |> WithNonClusteredIndex NONUNIQUE [("MyKeyColumn", ASC) ; ("SecondKeyColumn", DESC)]
                |> ScriptTable
                |> should equal @"CREATE TABLE [dbo].[MyIndexedTable]( [MyKeyColumn] [int] NOT NULL, [SecondKeyColumn] [int] NOT NULL )
GO

CREATE UNIQUE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO


CREATE UNIQUE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn2 ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO


CREATE NONCLUSTERED INDEX IDX_MyIndexedTable_MyKeyColumn_SecondKeyColumn3 ON [dbo].[MyIndexedTable] ([MyKeyColumn] ASC, [SecondKeyColumn] DESC)
GO"
        [<Test>]
        let ``should not be able to create a clustered index when there is already a clustered primary key``() =
            (fun () -> CreateTable "InvalidTable"
                        |> WithInt "MyInt"
                        |> WithClusteredPrimaryKey [("MyInt", ASC)]
                        |> WithClusteredIndex UNIQUE [("MyInt", ASC)]
                        |> ScriptTable
                        |> ignore)          
                |> should (throwWithMessage "clustered index not allowed as table InvalidTable already contains clustered primary key") typeof<System.Exception>

        [<Test>]
        let ``should be able to create a clustered index when table has a non clustered primary key``() =
                CreateTable "MyTable"
                    |> WithInt "MyInt"
                    |> WithNonClusteredPrimaryKey [("MyInt", ASC)]
                    |> WithClusteredIndex UNIQUE [("MyInt", ASC)]
                    |> ScriptTable
                    |> should equal @"CREATE TABLE [dbo].[MyTable]( [MyInt] [int] NOT NULL )
GO

ALTER TABLE [dbo].[MyTable] ADD CONSTRAINT [PK_MyTable] PRIMARY KEY NONCLUSTERED ([MyInt] ASC)
GO

CREATE UNIQUE CLUSTERED INDEX IDX_MyTable_MyInt ON [dbo].[MyTable] ([MyInt] ASC)
GO"

        [<Test>]
        let ``should not be able to create more than one clustered index on a table``() =
            (fun () -> CreateTable "InvalidTable"
                        |> WithInt "MyInt"
                        |> WithInt "MyOtherInt"
                        |> WithClusteredIndex UNIQUE [("MyInt", ASC)]
                        |> WithClusteredIndex UNIQUE [("MyInt", ASC); ("MyOtherInt", ASC)]
                        |> ScriptTable
                        |> ignore)          
                |> should (throwWithMessage "clustered index not allowed as table InvalidTable already contains clustered index") typeof<System.Exception>




