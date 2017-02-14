namespace SqlJuxtFunctionalTests.Scenarios

open FsUnit
open NUnit.Framework

open SqlJuxtFunctional.Comparer
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes
open SqlJuxtFunctional.TestLocalDatabase
open SqlJuxtFunctionalTests.TestModule

module CompareTableStructureScenarios =

    [<Test>]
    let ``should return different table names when a table is different because of different column name``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithNullableInt "Column2"
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different table names when a table is different because of different column type``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithVarchar "Column1" 20
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different table names when a table is different because column is non nullable vs nullable``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different table names when a table is different because columns are the same but declared in a different order``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithVarchar "Column2" 20
                        |> WithInt "Column1"
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

