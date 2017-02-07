namespace SqlJuxtFunctional

open System

module DatabaseTypes =

    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = IntColumn of IntegerColumn | VarColumn of VarcharColumn
    type SortDirection = ASC | DESC
    type Clustering = CLUSTERED | NONCLUSTERED
    type Uniqueness = UNIQUE | NONUNIQUE
    type ConstraintType = PRIMARYKEY | INDEX
    type Constraint = {name: string; columns: (Column * SortDirection)  list; clustering: Clustering; uniqueness : Uniqueness; constraintType: ConstraintType}
    type Table = {schema: string; name: string; columns: Column list; primaryKey: Constraint option; indexes: Constraint list}
    type Catalog = {tables: Table list}

    type TableDifference = {left: Table; right: Table}
    type DatabaseDifferences = {missingTables: Table list; differentTables: TableDifference list}
    type ComparisonResult = IsMatch | Differences of DatabaseDifferences

    let getColumnName c =
        match c with
            | IntColumn i -> i.name
            | VarColumn v -> v.name

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

    let rec getNextAvailableName (name:string) (names: string list) =

        let getNumber (chr:char) =
            match Int32.TryParse(chr.ToString()) with
                | (true, i) -> Some i
                | _ -> None

        let grabLastChar (str:string) =
            str.[str.Length-1]

        let pruneLastChar (str:string) =
            str.Substring(0, str.Length - 1)

        let pruneNumber (str:string) i =
            str.Substring(0, str.Length - i.ToString().Length)

        let getNumberFromEndOfString (s:string)  =

            let rec getNumberFromEndOfStringInner (s1:string) (n: int option) =
                match s1 |> String.IsNullOrWhiteSpace with
                    | true -> n
                    | false -> match s1 |> grabLastChar |> getNumber with
                                | None -> n
                                | Some m ->  let newS = s1 |> pruneLastChar
                                             match n with 
                                                | Some n1 -> let newN = m.ToString() + n1.ToString() |> Convert.ToInt32 |> Some
                                                             getNumberFromEndOfStringInner newS newN
                                                | None -> getNumberFromEndOfStringInner newS (Some m) 
            let num = getNumberFromEndOfStringInner s None
            match num with
                | Some num' -> (s |> pruneNumber <| num', num)
                | None -> (s, num)
            

        let result = names |> List.tryFind(fun x -> x = name)
        match result with
            | Some r -> let (n, r) = getNumberFromEndOfString name
                        match r with 
                            | Some r' -> getNextAvailableName (n + (r'+1).ToString()) names
                            | None -> getNextAvailableName (n + "2") names
                        
            | None -> name