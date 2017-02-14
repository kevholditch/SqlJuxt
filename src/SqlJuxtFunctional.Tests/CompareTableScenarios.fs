namespace SqlJuxtFunctionalTests.Scenarios

open FsUnit
open NUnit.Framework

open SqlJuxtFunctional.Comparer
open SqlJuxtFunctional.DatabaseBuilder
open SqlJuxtFunctional.DatabaseScripter
open SqlJuxtFunctional.DatabaseTypes
open SqlJuxtFunctional.TestLocalDatabase
open SqlJuxtFunctionalTests.TestModule

module CompareTableScenarios =

    [<Test>]
    let ``should return identical when two tables are the same``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let table = CreateTable "TestTable"
                        |> WithInt "Column1"
                        |> WithVarchar "Column2" 10
                        |> WithInt "Column3"
                        |> WithClusteredPrimaryKey [("Column1", ASC); ("Column3", DESC)]
                        |> ScriptTable 

        runScript left table
        runScript right table
        
        loadCatalog left.ConnectionString
            |> compareWith right.ConnectionString
            |> should equal IsMatch

     
    [<Test>]
    let ``should return missing table names when table is missing``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let table = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> ScriptTable 
        
        runScript right table
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.missingTables.Length |> should equal 1
        result.missingTables.Head.name |> should equal "TestTable"

    [<Test>]
    let ``should return missing table name when table exists but has a different name``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftTable = CreateTable "TestTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> ScriptTable 
        
        runScript left leftTable

        let rightTable = CreateTable "OtherTable"
                        |> WithNullableInt "Column1"
                        |> WithVarchar "Column2" 20
                        |> ScriptTable 
        
        runScript right rightTable
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.missingTables.Length |> should equal 1
        result.missingTables.Head.name |> should equal "OtherTable"
  

    [<Test>]
    let ``should return differences for multiple tables where some are missing and some are different``() =
        use left = createDatabase()
        use right = createDatabase()
     
        let leftCatalog = CreateCatalog()
                            |> WithTable (CreateTable "TableA"
                                                |> WithInt "Column1"
                                                |> WithInt "Column2" 
                                                |> WithVarchar "Column3" 10
                                                |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                            |> WithTable (CreateTable "TableB"
                                                |> WithInt "Column1"
                                                |> WithInt "Column2" 
                                                |> WithInt "Column3" 
                                                |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                            |> Script 
        
        runScript left leftCatalog

        let rightCatalog = CreateCatalog()
                            |> WithTable (CreateTable "TableA"
                                                |> WithInt "Column1"
                                                |> WithInt "Column2" 
                                                |> WithInt "Column3" 
                                                |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                            |> WithTable (CreateTable "TableC"
                                                |> WithInt "Column1"
                                                |> WithInt "Column2" 
                                                |> WithInt "Column3" 
                                                |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                            |> Script 
        
        runScript right rightCatalog
        
        let result = loadCatalog left.ConnectionString
                        |> compareWith right.ConnectionString
                        |> extractDifferences
        
        result.differentTables.Length |> should equal 1
        result.differentTables.Head.left.name |> should equal "TableA"
        result.differentTables.Head.right.name |> should equal "TableA"
        result.missingTables.Length |> should equal 1
        result.missingTables.Head.name |> should equal "TableC"


    
               
        

        
         





        
