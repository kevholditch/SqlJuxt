namespace SqlJuxtFunctionalTests.Scenarios

open FsUnit
open NUnit.Framework

open SqlJuxtFunctional.Comparer
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes
open SqlJuxtFunctional.TestLocalDatabase
open SqlJuxtFunctionalTests.TestModule

module CompareTableIndexScenarios =
    [<Test>]
    let ``should return different table names when one table has a unique clustered index and one unique non clustered``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentIndexTable"
        result.differentTables.Head.right.name |> should equal "DifferentIndexTable"

    [<Test>]
    let ``should return different table names when one table has a unique clustered index and one non unique clustered index``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex NONUNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentIndexTable"
        result.differentTables.Head.right.name |> should equal "DifferentIndexTable"

    [<Test>]
    let ``should return different table names when tables have indexes that differ by columns``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentIndexTable"
        result.differentTables.Head.right.name |> should equal "DifferentIndexTable"

    [<Test>]
    let ``should return different table names when one table has an index and the other does not``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3"
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "DifferentIndexTable"
        result.differentTables.Head.right.name |> should equal "DifferentIndexTable"

    [<Test>]
    let ``should return match when tables have the same unique clustered index``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "DifferentIndexTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> should equal IsMatch
    
    [<Test>]
    let ``should return match when tables have the same non unique clustered index``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex NONUNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithClusteredIndex NONUNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> should equal IsMatch

    [<Test>]
    let ``should return match when tables have the same non unique nonclustered index``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredIndex NONUNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredIndex NONUNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> should equal IsMatch
        
    [<Test>]
    let ``should return match when tables have the same unique nonclustered index``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "MyTable"
                        |> WithInt "Column1"
                        |> WithInt "Column2" 
                        |> WithInt "Column3" 
                        |> WithNonClusteredIndex UNIQUE [("Column1", ASC); ("Column2", ASC)]
                        |> ScriptTable 
        
        runScript right rightTable
        
        loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> should equal IsMatch

