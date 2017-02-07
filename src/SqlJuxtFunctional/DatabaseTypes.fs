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

        let getNumberFromEndOfString (s:string)  =

            let rec getNumberFromEndOfStringInner (s1:string) (n: int option) =
                match s1.Length with
                    | 0 -> n
                    | _ -> match getNumber s1.[s1.Length-1] with
                            | None -> n
                            | Some m -> match n with 
                                            | Some n1 -> let newN = Some (Convert.ToInt32(m.ToString() + n1.ToString()))
                                                         let newS = s1.Substring(0, s.Length - 1)
                                                         getNumberFromEndOfStringInner newS newN
                                            | None -> let newS = s1.Substring(0, s1.Length - 1)
                                                      getNumberFromEndOfStringInner newS (Some m) 
            let num = getNumberFromEndOfStringInner s None
            match num with
                | Some num' -> let rest = s.Substring(0, s.Length - num'.ToString().Length)
                               (rest, num)
                | None -> (s, num)
            

        let result = names |> List.tryFind(fun x -> x = name)
        match result with
            | Some r -> let (n, r) = getNumberFromEndOfString name
                        match r with 
                            | Some r' -> let newName = n + (r'+1).ToString()
                                         getNextAvailableName newName names
                            | None -> getNextAvailableName (n + "2") names
                        
            | None -> name