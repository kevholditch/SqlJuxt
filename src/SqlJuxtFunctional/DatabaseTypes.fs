﻿namespace SqlJuxtFunctional

module DatabaseTypes =

    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = IntColumn of IntegerColumn | VarColumn of VarcharColumn
    type SortDirection = ASC | DESC
    type Clustering = CLUSTERED | NONCLUSTERED
    type Constraint = {name: string; columns: (Column * SortDirection)  list; Clustering: Clustering}
    type Table = {schema: string; name: string; columns: Column list; primaryKey: Constraint option}
    type Schema = {tables: Table list}

    type TableDifference = {left: Table; right: Table}
    type DatabaseDifferences = {missingTables: Table list; differentTables: TableDifference list}
    type ComparisonResult = IsMatch | Differences of DatabaseDifferences

    let getColumnName c =
        match c with
            | IntColumn i -> i.name
            | VarColumn v -> v.name

    let getColumnNames c =
            c |> List.map(getColumnName)