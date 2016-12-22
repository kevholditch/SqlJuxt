namespace SqlJuxtFunctional

module DatabaseTypes =

    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = IntColumn of IntegerColumn | VarColumn of VarcharColumn
    type Table = {name: string; columns: Column list}
    type Schema = {tables: Table list}