namespace SqlJuxtFunctional

module Comparer =
    open System.Data.SqlClient
    open DapperFSharp
    open System
    open SqlJuxtFunctional.DatabaseTypes

    type ColumnEntity = {Schema:string; TableName:string; ColumnName:string; OrdinalPosition:int; Default:string; IsNullable:string; DataType:string; CharacterMaxLength:Nullable<int>; }            
    type ConstraintEntity = {Schema:string; TableName:string; Name:string; KeyOrdinal:byte; ColumnName:string; IsDescending:bool; IsClustered: bool; Type: string; IsUnique: bool}

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
        |> dapperQuery<ConstraintEntity> @"select OBJECT_SCHEMA_NAME(o.object_id) AS [Schema], 
		OBJECT_NAME(o.object_id) AS TableName,
		i.name AS Name,
		ic.key_ordinal ""KeyOrdinal"", 
		c.name AS ColumnName, 
		ic.is_descending_key ""IsDescending"",
		CASE WHEN i.type_desc = 'CLUSTERED' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS ""IsClustered"",
        CASE WHEN i.is_primary_key = 1 THEN 'PRIMARYKEY'
		     WHEN i.is_unique_constraint = 1 THEN 'UNIQUECONSTRAINT'
			 ELSE 'INDEX'
		END AS ""Type"",
		i.is_unique AS ""IsUnique""
from sys.objects o
join sys.indexes i on i.object_id = o.object_id
join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
join sys.columns c on c.object_id = o.object_id and c.column_id = ic.column_id
where OBJECT_SCHEMA_NAME(o.object_id) <> 'sys'
order by OBJECT_SCHEMA_NAME(o.object_id), OBJECT_NAME(o.object_id), i.name, ic.key_ordinal"
        |> Seq.toList

    let private buildCatalog (columns: ColumnEntity list) (constraints: ConstraintEntity list)  =

        let tables = columns |> Seq.groupBy(fun c -> (c.Schema, c.TableName))
                             |> Seq.map(fun ((schema, tableName), cols) -> 
                                    let columns = cols |> Seq.map(mapColumnEntity)
                                    let tableConstraints = constraints |> List.filter(fun k -> k.TableName = tableName && k.Schema = schema)
                                                                       |> Seq.groupBy(fun k -> (k.Name, k.IsClustered, k.IsUnique, k.Type))
                                                                       |> Seq.map(fun ((name, isClustered, isUnique, ``type``), items) -> 
                                                                                            (
                                                                                                    name, 
                                                                                                    items |> Seq.map(fun k -> let col = columns |> Seq.find(fun c -> (getColumnName c) = k.ColumnName)
                                                                                                                              let dir = match k.IsDescending with
                                                                                                                                        | true -> DESC
                                                                                                                                        | false -> ASC 
                                                                                                                              (col, dir)) |> Seq.toList,
                                                                                                    match isClustered with
                                                                                                    | true -> CLUSTERED
                                                                                                    | false -> NONCLUSTERED
                                                                                                    ,
                                                                                                    match isUnique with
                                                                                                      | true -> UNIQUE
                                                                                                      | false -> NONUNIQUE
                                                                                                    ,
                                                                                                    ``type``
                                                                                             ))
                                                                                                        
                                                                        |> Seq.toList

                                    let primaryKey = tableConstraints |> List.map(fun (n, cs, c, u, t) -> match t with
                                                                                                          | "PRIMARYKEY" ->  Some {PrimaryKey.name = n; columns = cs; clustering = c}
                                                                                                          | _ -> None)
                                                                      |> List.choose(fun p -> p)
                                                                      |> fun ps -> match ps with
                                                                                   | [p] -> Some p
                                                                                   | _ -> None

                                    let uniqueContraints = tableConstraints |> List.filter(fun (_, _, _, _, t) -> t = "UNIQUECONSTRAINT")
                                                                            |> List.map(fun  (n, cs, c, u, p) -> {UniqueConstraint.name = n; columns = cs})
                                                                            
                                    let indexes = tableConstraints |> List.filter(fun (_, _, _, _, t) -> t = "INDEX")
                                                                   |> List.map(fun  (n, cs, c, u, p) -> {Index.name = n; columns = cs; clustering = c; uniqueness = u; })

                                    {schema = schema; name = tableName; columns = columns |> Seq.toList; primaryKey = primaryKey; indexes = indexes; uniqueConstraints = uniqueContraints})                      
        {tables = tables |> Seq.toList}

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



