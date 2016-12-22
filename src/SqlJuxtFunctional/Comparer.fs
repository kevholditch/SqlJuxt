namespace SqlJuxtFunctional

module Comparer =
    open System.Data.SqlClient
    open DapperFSharp
    open System
    open SqlJuxtFunctional.DatabaseTypes

    type ColumnEntity = {Schema:string; TableName:string; ColumnName:string; OrdinalPosition:int; Default:string; IsNullable:string; DataType:string; CharacterMaxLength:Nullable<int>; }            
    

    let getColumns connection =
        connection
        |> dapperQuery<ColumnEntity> @"select TABLE_SCHEMA as ""Schema"", TABLE_NAME as ""TableName"",   COLUMN_NAME as ""ColumnName"", ORDINAL_POSITION as ""OrdinalPosition"",
		                                                                            COLUMN_DEFAULT as ""Default"", IS_NULLABLE as ""IsNullable"", DATA_TYPE as ""DataType"", CHARACTER_MAXIMUM_LENGTH as ""CharacterMaxLength""
                                                                             from INFORMATION_SCHEMA.COLUMNS
                                                                             order by TABLE_NAME, ORDINAL_POSITION"
        
    let getColumn (c:ColumnEntity) =

        let isNullable = match c.IsNullable with
                           | "YES" -> true
                           | _ -> false

        match c.DataType with
            | "int" -> IntColumn {name = c.ColumnName; isNullable = isNullable}
            | "varchar" -> VarColumn {name = c.ColumnName; isNullable = isNullable; length = c.CharacterMaxLength.Value}
            | _ -> failwithf "unknown column type %s" c.DataType

    let buildSchema columns  =
        let tables = columns |> Seq.groupBy(fun c -> c.TableName)
                             |> Seq.map(fun (tableName, cols) -> {name = tableName; columns = cols |> Seq.map(getColumn) |> Seq.toList})
                             |> Seq.toList

        {tables = tables}

    let loadSchema connectionString =        
        use connection = new SqlConnection(connectionString)
        connection |> getColumns
                   |> buildSchema  

  
    let compareDatabases leftSchema rightSchema =
        match leftSchema = rightSchema with
            | true -> IsMatch
            | false -> Differences { missingTables = []}

    let compareWith rightConnString leftSchema =
        let rightSchema = loadSchema rightConnString
        compareDatabases leftSchema rightSchema 

    

    


    

