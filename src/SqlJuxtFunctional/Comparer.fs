namespace SqlJuxtFunctional

module Comparer =
    open System.Data.SqlClient
    open DapperFSharp
    open System

    type Column = { Schema:string; TableName:string; ColumnName:string; OrdinalPosition:int; Default:string; IsNullable:string; DataType:string; CharacterMaxLength:Nullable<int>; }
    type Table = {Name:string; Columns: Column list}
    type Schema = {Tables: Table list}

    let private getColumns connection =
        connection
        |> dapperQuery<Column> @"select TABLE_SCHEMA as ""Schema"", TABLE_NAME as ""TableName"",   COLUMN_NAME as ""ColumnName"", ORDINAL_POSITION as ""OrdinalPosition"",
		                                                                            COLUMN_DEFAULT as ""Default"", IS_NULLABLE as ""IsNullable"", DATA_TYPE as ""DataType"", CHARACTER_MAXIMUM_LENGTH as ""CharacterMaxLength""
                                                                             from INFORMATION_SCHEMA.COLUMNS
                                                                             order by TABLE_NAME, ORDINAL_POSITION"
        

    
    let private getSchema connectionString =
        let tables =
            use connection = new SqlConnection(connectionString)
            connection |> getColumns
                |> Seq.groupBy(fun c -> c.TableName)
                |> Seq.map(fun (tableName, columns) -> { Table.Name = tableName; Columns = columns |> Seq.toList})
                |> Seq.toList
            
        {Schema.Tables = tables}

    let Compare (left:string) (right:string) =

        let leftSchema = getSchema left
        let rightSchema = getSchema right

        leftSchema = rightSchema

