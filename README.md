# SqlJuxt
Sql comparison API written in F#.

SqlJuxt is a Sql Server comparision and scripting API.  It allows you to compare two database schemas and fluently script a database.

To compare two database schemas:
```
let leftConnString = "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=catalog1;"
let rightConnString = "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=catalog2;"

let result = loadCatalog leftConnString
                 |> compareWith rightConnString
```
result is a `ComparisonResult` which is either a `IsMatch` in which case the database schemas are the same or it is a `Differences` in which case the database schemas are different.  The full match result is defined as:

```
type TableDifference = {left: Table; right: Table}
type DatabaseDifferences = {missingTables: Table list; differentTables: TableDifference list}
type ComparisonResult = IsMatch | Differences of DatabaseDifferences
```

In the `TableDifference` type the left property is the first database you passed in and right is the database you compared it to.

To build a database use the `DatabaseBuilder` module inside the SqlJuxtFunctional namespace.  To script the database out use the built database and pass it to the `DatabaseScripter.Script` function.  An example of building and scripting a catalog:

```
let myCatalog = CreateCatalog()
                              |> WithTable (CreateTable "TableA"
                                                  |> WithInt "Column1"
                                                  |> WithInt "Column2" 
                                                  |> WithInt "Column3" 
                                                  |> WithNonClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                              |> WithTable (CreateTable "TableB"
                                                  |> WithInt "Column1"
                                                  |> WithVarchar "Column2" 20 
                                                  |> WithInt "Column3" 
                                                  |> WithClusteredPrimaryKey [("Column1", ASC); ("Column2", ASC)])
                              |> Script
```
The `myCatalog` variable will contain the following script after execution:
```
CREATE TABLE [dbo].[TableA]( [Column1] [int] NOT NULL, [Column2] [int] NOT NULL, [Column3] [int] NOT NULL )
GO

ALTER TABLE [dbo].[TableA] ADD CONSTRAINT [PK_TableA] PRIMARY KEY NONCLUSTERED ([Column1] ASC, [Column2] ASC)
GO

CREATE TABLE [dbo].[TableB]( [Column1] [int] NOT NULL, [Column2] [varchar](20) NOT NULL, [Column3] [int] NOT NULL )
GO

ALTER TABLE [dbo].[TableB] ADD CONSTRAINT [PK_TableB] PRIMARY KEY CLUSTERED ([Column1] ASC, [Column2] ASC)
GO
```





