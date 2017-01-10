namespace SqlJuxtFunctionalTests.Scenarios

module CompareTableScenarios =

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
    let ``should return missing table names when table is missing``() =
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
        result.missingTables.Head.name |> should equal "TestTable"

    [<Fact>]
    let ``should return missing table name when table exists but has a different name``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> Build 
        
        runScript left leftTable

        let rightTable = CreateTable "OtherTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> Build 
        
        runScript right rightTable
        
        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.missingTables.Length |> should equal 1
        result.missingTables.Head.name |> should equal "OtherTable"

    [<Fact>]
    let ``should return different table names when a table is different because of different column name``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithNullableInt "Column2"
                        |> Build 
        
        runScript right rightTable
        
        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Fact>]
    let ``should return different table names when a table is different because of different column type``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithVarchar "Column1" 20
                        |> Build 
        
        runScript right rightTable
        
        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Fact>]
    let ``should return different table names when a table is different because column is non nullable vs nullable``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> Build 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> Build 
        
        runScript right rightTable
        
        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Fact>]
    let ``should return different table names when a table is different because columns are the same but declared in a different order``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> Build 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithVarchar "Column2" 20
                        |> WithInt "Column1"
                        |> Build 
        
        runScript right rightTable
        
        let result = loadSchema left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    
               
        

        
         





        
