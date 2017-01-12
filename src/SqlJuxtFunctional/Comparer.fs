namespace SqlJuxtFunctional

module Comparer =
    open System.Data.SqlClient
    open DapperFSharp
    open System
    open SqlJuxtFunctional.DatabaseTypes

    type ColumnEntity = {Schema:string; TableName:string; ColumnName:string; OrdinalPosition:int; Default:string; IsNullable:string; DataType:string; CharacterMaxLength:Nullable<int>; }            
    type PrimaryKeyEntity = {Schema:string; TableName:string; PrimaryKeyName:string; KeyOrdinal:byte; ColumnName:string; IsDescending:bool; IsClustered: bool; }

    let private getColumns connection =
        connection
        |> dapperQuery<ColumnEntity> @"select TABLE_SCHEMA as ""Schema"", TABLE_NAME as ""TableName"",   COLUMN_NAME as ""ColumnName"", ORDINAL_POSITION as ""OrdinalPosition"",
		                                                                            COLUMN_DEFAULT as ""Default"", IS_NULLABLE as ""IsNullable"", DATA_TYPE as ""DataType"", CHARACTER_MAXIMUM_LENGTH as ""CharacterMaxLength""
                                                                             from INFORMATION_SCHEMA.COLUMNS
                                                                             order by TABLE_NAME, ORDINAL_POSITION"
        |> Seq.toList
        
    let private mapColumnEntity (c:ColumnEntity) =

        let isNullable = match c.IsNullable with
                           | "YES" -> true
                           | _ -> false

        match c.DataType with
            | "int" -> IntColumn {name = c.ColumnName; isNullable = isNullable}
            | "varchar" -> VarColumn {name = c.ColumnName; isNullable = isNullable; length = c.CharacterMaxLength.Value}
            | _ -> failwithf "unknown column type %s" c.DataType

    let private getPrimaryKeys connection =
        connection 
        |> dapperQuery<PrimaryKeyEntity> @"SELECT  OBJECT_SCHEMA_NAME(o.parent_object_id) AS [Schema], 
		OBJECT_NAME(o.parent_object_id) AS TableName, 
		o.name AS PrimaryKeyName, 
		ic.key_ordinal ""KeyOrdinal"", 
		c.name AS ColumnName, 
		ic.is_descending_key ""IsDescending"",
		CASE WHEN i.type_desc = 'CLUSTERED' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS ""IsClustered""
from sys.objects o
inner join sys.indexes i on i.object_id = o.parent_object_id and i.is_primary_key = 1
inner join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
inner join sys.columns c on c.object_id = o.parent_object_id and c.column_id = ic.column_id
inner join sys.types t ON t.user_type_id = c.user_type_id
where o.type = 'PK'
ORDER BY OBJECT_SCHEMA_NAME(o.parent_object_id), OBJECT_NAME(o.parent_object_id), ic.key_ordinal"
        |> Seq.toList

    let private buildCatalog (columns: ColumnEntity list) (primaryKeys: PrimaryKeyEntity list)  =
        
        let tables = columns |> List.groupBy(fun c -> (c.Schema, c.TableName))
                             |> List.map(fun ((schema, tableName), cols) -> 
                                    let columns = cols |> List.map(mapColumnEntity)
                                    let tableKeys = primaryKeys |> List.filter(fun k -> k.TableName = tableName && k.Schema = schema)
                                    let primaryKey = match tableKeys with
                                                            | x::xs -> Some {
                                                                                name = x.PrimaryKeyName; 
                                                                                columns = x::xs |> List.map(fun k -> let col = columns |> List.find(fun c -> (getColumnName c) = k.ColumnName)
                                                                                                                     let dir = match k.IsDescending with
                                                                                                                                 | true -> DESC
                                                                                                                                 | false -> ASC
                                                                                                                     (col, dir))
                                                                                Clustering = match x.IsClustered with
                                                                                                | true -> CLUSTERED
                                                                                                | false -> NONCLUSTERED
                                                                            }
                                                            | _ -> None
                                    {schema = schema; name = tableName; columns = columns; primaryKey = primaryKey})                      
        {tables = tables}

    let loadCatalog connectionString =        
        use connection = new SqlConnection(connectionString)
        let columns = connection |> getColumns
        let primaryKeys = connection |> getPrimaryKeys
        buildCatalog columns primaryKeys

    let compareDatabases left right =
        match left = right with
            | true -> IsMatch
            | false ->  let matches = right.tables|> List.map (fun r -> (left.tables |> List.tryFind(fun l -> l.name = r.name), r)) 
                        let s = matches |> List.filter (fun (l,r) -> match  (l, r ) with 
                                                                                | (Some l, r ) -> true
                                                                                | _ -> false)
                                        |> List.map (fun (l, r) -> match (l, r ) with 
                                                                    | (Some l, r ) -> (l, r )
                                                                    | _ -> failwith "Found a missing table")


                        Differences {
                                     missingTables = matches |> List.filter (fun (l, r) -> match (l, r) with 
                                                                                            | (Some l, r) -> false
                                                                                            | _ -> true) |> List.map(fun (l, r) -> r);
                                     differentTables = s |> List.filter(fun (l, r) -> l <> r) 
                                                         |> List.map( fun(l, r) -> {left = l; right = r } )
                                     }

    let compareWith rightConnString left =
        let right = loadCatalog rightConnString
        compareDatabases left right 



