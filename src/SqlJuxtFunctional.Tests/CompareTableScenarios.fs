module CompareTableScenarios

    open Xunit
    open SqlJuxtFunctional.DatabaseBuilder.TableBuilder
    open SqlJuxtFunctional.Comparer
    open FsUnit
    open FsUnit.Xunit
    open SqlJuxtFunctional.TestLocalDatabase


    [<Fact>]
    let ``should return identical when two tables are the same``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let table = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 

        runScript left table
        runScript right table

        loadSchema left.ConnectionString
            |> compareWith right.ConnectionString
            |> should equal true

     
    [<Fact>]
    let ``should return missing table names when tables are missing``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let table = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 
        
        runScript right table

        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
           
        result |> should equal false
        
        

        
        
         





        
