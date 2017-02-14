namespace SqlJuxtFunctionalTests.Scenarios

open FsUnit
open NUnit.Framework

open SqlJuxtFunctional.Comparer
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes
open SqlJuxtFunctional.TestLocalDatabase
open SqlJuxtFunctionalTests.TestModule

module CompareTableConstraintScenarios =
    [<Test>]
    let ``should return different table names when one table has a unique constraint and one does not``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithUniqueConstraint  ["Column1"]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3"
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different table names when tables have a unique constraint on different columns``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithUniqueConstraint  ["Column1"]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3"
                        |> WithUniqueConstraint  ["Column1"; "Column2"]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different match when tables have the same unique constraint``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithUniqueConstraint  ["Column1"; "Column2"]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3"
                        |> WithUniqueConstraint  ["Column1"; "Column2"]
                        |> ScriptTable 
        
        runScript right rightTable
        
        loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> should equal IsMatch

