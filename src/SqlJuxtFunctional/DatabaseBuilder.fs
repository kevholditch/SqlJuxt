namespace SqlJuxtFunctional

open System
open DatabaseTypes
open DatabaseScripter

module DatabaseBuilder =
        
        let CreateCatalog () =
            {tables = []}

        let WithTable table catalog =
            {catalog with tables = table::catalog.tables}
        
        let CreateTableInSchema schema name =
            {schema = schema; name = name; columns = []; primaryKey = None; indexes = []}

        let CreateTable = CreateTableInSchema "dbo"

        let private withInt nullable name table =
            let c = IntColumn {name = name; isNullable = nullable}
            {table with Table.columns = c::table.columns}

        let WithNullableInt = withInt true 
        let WithInt = withInt false 

        let private withVarchar nullable name length table =
            let c = VarColumn {name = name; isNullable = nullable; length = length}
            {table with Table.columns = c::table.columns}

        let WithNullableVarchar = withVarchar true
        let WithVarchar = withVarchar false

        let private withIndex (clustering:Clustering) (uniqueness:Uniqueness) (columns:(string * SortDirection) list) (table:Table) =
            let cs = columns |> List.map(fun (c,d) -> let column = getColumnByName c table.columns
                                                      match column with
                                                        | Some col -> (col, d)
                                                        | None -> failwithf "no column named %s exists on table %s" c table.name )
            let columnNames = cs |> List.map(fun(c,d) -> (getColumnName c)) |> fun cols -> String.Join("_", cols)
            let indexName = sprintf "IDX_%s_%s" table.name columnNames
            let indexNames = table.indexes |> List.map(fun i -> i.name)
            let newIndexName = getNextAvailableName indexName indexNames
            let index = {Constraint.name = newIndexName; columns = cs; clustering = clustering; uniqueness = uniqueness; constraintType = INDEX}
            
            match (table.primaryKey, table.indexes |> List.filter(fun i -> i.clustering = CLUSTERED)) with 
                | (Some key, _) when key.clustering = CLUSTERED -> failwithf "clustered index not allowed as table %s already contains clustered primary key" table.name
                | (_, x::xs) -> failwithf "clustered index not allowed as table %s already contains clustered index" table.name
                | _ -> {table with indexes = index::table.indexes}
            

        let WithClusteredIndex = withIndex CLUSTERED
        let WithNonClusteredIndex = withIndex NONCLUSTERED
                                                              
        let WithPrimaryKey clustering columns table =
            let cs = columns |> List.map(fun (c,d) -> let column = getColumnByName c table.columns
                                                      match column with
                                                        | Some col when isColumnNullable col -> failwithf "column named %s is nullable, nullable columns are not allowed as part of a primary key" c 
                                                        | Some col -> (col, d)
                                                        | None -> failwithf "no column named %s exists on table %s" c table.name )  
            {table with primaryKey = Some {name = sprintf "PK_%s" table.name; columns = cs; clustering = clustering; uniqueness = UNIQUE; constraintType = PRIMARYKEY}}
              
        let WithClusteredPrimaryKey = WithPrimaryKey CLUSTERED
        let WithNonClusteredPrimaryKey = WithPrimaryKey NONCLUSTERED





           
                       
