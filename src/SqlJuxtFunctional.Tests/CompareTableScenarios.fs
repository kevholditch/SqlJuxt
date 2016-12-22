module CompareTableScenarios

    open Xunit
    open SqlJuxtFunctional.DatabaseBuilder.TableBuilder
    open SqlJuxtFunctional.Comparer
    open FsUnit
    open FsUnit.Xunit
    open SqlJuxtFunctional.DatabaseTypes
    open SqlJuxtFunctional.TestLocalDatabase
    
    let extractDifferences r =
        match r with
        | Differences(r') -> r'
        | _ -> failwith "expected databases to be different but they are a match"

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
            |> should equal IsMatch

     
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
                        |> extractDifferences
        
        result.missingTables.Length |> should equal 1
               
        
        
        
        

        
        
         





        
