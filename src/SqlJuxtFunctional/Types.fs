namespace SqlJuxtFunctional

open System

type Column = { Schema:string; TableName:string; ColumnName:string; OrdinalPosition:int; Default:string; IsNullable:string; DataType:string; CharacterMaxLength:Nullable<int>; }
type Table = {Name:string; Columns: Column list}
type Schema = {Tables: Table list}
    
    