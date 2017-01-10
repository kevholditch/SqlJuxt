namespace SqlJuxtFunctional

module DatabaseTypes =

    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = IntColumn of IntegerColumn | VarColumn of VarcharColumn
    type SortDirection = ASC | DESC
    type Constraint = {name: string; columns: (Column * SortDirection)  list; isClustered: bool}
    type Table = {name: string; columns: Column list; primaryKey: Constraint option}
    type Schema = {tables: Table list}


    type TableDifference = {left: Table; right: Table}
    
    type DatabaseDifferences = {missingTables: Table list; differentTables: TableDifference list}

    type ComparisonResult = IsMatch | Differences of DatabaseDifferences