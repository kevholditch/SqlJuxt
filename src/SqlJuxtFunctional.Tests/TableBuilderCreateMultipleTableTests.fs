namespace SqlJuxtFunctionalTests.Tests.TableBuilder

open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes


    module TableBuilderCreateMultipleTableTests =
        [<Test>]
        let ``should script multiple tables in alphabetical order``() =
            CreateCatalog ()
                |> WithTable (CreateTable "C" |> WithInt "MyColumn")
                |> WithTable (CreateTable "A" |> WithVarchar "MyVar" 10)
                |> WithTable (CreateTable "B" |> WithVarchar "MyVar" 10)
                |> Script
                |> should equal @"CREATE TABLE [dbo].[A]( [MyVar] [varchar](10) NOT NULL )
GO
CREATE TABLE [dbo].[B]( [MyVar] [varchar](10) NOT NULL )
GO
CREATE TABLE [dbo].[C]( [MyColumn] [int] NOT NULL )
GO"


