namespace SqlJuxtFunctional

open LibraryFunctions
open System

module DatabaseTypes =

    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = IntColumn of IntegerColumn | VarColumn of VarcharColumn
    type SortDirection = ASC | DESC
    type Clustering = CLUSTERED | NONCLUSTERED
    type Uniqueness = UNIQUE | NONUNIQUE
    type ConstraintType = PRIMARYKEY | INDEX | UNIQUECONSTRAINT
    type Constraint = {name: string; columns: (Column * SortDirection)  list; clustering: Clustering; uniqueness : Uniqueness; constraintType: ConstraintType}
    type Table = {schema: string; name: string; columns: Column list; primaryKey: Constraint option; indexes: Constraint list; constraints: Constraint list}
    type Catalog = {tables: Table list}

    type TableDifference = {left: Table; right: Table}
    type DatabaseDifferences = {missingTables: Table list; differentTables: TableDifference list}
    type ComparisonResult = IsMatch | Differences of DatabaseDifferences

   
    let getColumnName c =
        match c with
            | IntColumn i -> i.name
            | VarColumn v -> v.name

    let getColumnsAsUnderscoreString cols = 
        cols |> List.map(fun(c,d) -> (getColumnName c)) 
             |> fun cs -> String.Join("_", cs)

    let isColumnNullable c =
        match c with
            | IntColumn i -> i.isNullable
            | VarColumn v -> v.isNullable

    let getColumnNames c =
            c |> List.map(getColumnName)

    let getDirectionString direction =
            match direction with
                | ASC -> "ASC"
                | DESC -> "DESC"

    let getColumnByName name columns =
        columns |> List.tryFind(fun col -> getColumnName col = name)

    let rec getNextAvailableName (name:string) (names: string list) =

        let result = names |> List.tryFind(fun x -> x = name)
        match result with
            | Some r -> let (n, r) = match name with
                                        | ParseRegex "(.*)(\d+)$" [s; Integer i] -> (s, Some i)
                                        | _ -> (name, None)
                        match r with 
                            | Some r' -> getNextAvailableName (n + (r'+1).ToString()) names
                            | None -> getNextAvailableName (n + "2") names
                        
            | None -> name