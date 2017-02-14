namespace SqlJuxtFunctionalTests.Scenarios

open FsUnit
open NUnit.Framework

open SqlJuxtFunctional.Comparer
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes
open SqlJuxtFunctional.TestLocalDatabase
open SqlJuxtFunctionalTests.TestModule

module CompareTablePrimaryKeyScenarios =
    [<Test>]
    let ``should return different table names when one table has a primary key and the other does not``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> WithClusteredPrimaryKey [("Column1", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TestTable"
        result.differentTables.Head.right.name |> should equal "TestTable"

    [<Test>]
    let ``should return different table names when tables have different primary keys differing by number of columns``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentKeyTable"
        result.differentTables.Head.right.name |> should equal "DifferentKeyTable"

    [<Test>]
    let ``should return different table names when tables have different primary keys differing by column order``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column2", ASC); ("Column1", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentKeyTable"
        result.differentTables.Head.right.name |> should equal "DifferentKeyTable"

    [<Test>]
    let ``should return different table names when tables have different primary keys differing by column sort order``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", DESC); ("Column2", DESC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentKeyTable"
        result.differentTables.Head.right.name |> should equal "DifferentKeyTable"

    [<Test>]
    let ``should return different table names when tables have different primary keys differing by clustered / non clustered``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentKeyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentKeyTable"
        result.differentTables.Head.right.name |> should equal "DifferentKeyTable"

