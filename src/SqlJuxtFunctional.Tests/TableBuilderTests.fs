namespace SqlJuxtFunctionalTests


open FsUnit
open SqlJuxtFunctional.DatabaseBuilder.TableBuilder

    module TablebuilderTests =
        open Xunit

        [<Fact>]
        let ``should be able to builder a table with a single int column``() =
            Create "TestTable"
                |> WithNullableInt "Column1"
                |> Build 
                |> should equal @"CREATE TABLE [dbo].[TestTable](
[Column1] [int] NULL
)
GO
"




