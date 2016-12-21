module CompareTableScenarios

    open Xunit
    open SqlJuxtFunctional.DatabaseBuilder.TableBuilder
    open SqlJuxtFunctional.Comparer
    open FsUnit
    open FsUnit.Xunit
    open TestLocalDatabase


    [<Fact>]
    let ``should return identical when two tables are the same``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let table = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 

        runScript left table
        runScript right table

        Compare left.ConnectionString right.ConnectionString 
            |> should equal true
        
        

        
        
         





        
