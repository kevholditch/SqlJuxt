namespace SqlJuxtFunctional

open System
open DatabaseTypes
open DatabaseScripter

module DatabaseBuilder =
        
        let CreateCatalog () =
            {tables = []}

        let WithTable table catalog =
            {catalog with tables = table::catalog.tables}
        
        let CreateTable name =
            {schema = "dbo"; name = name; columns = []; primaryKey = None; indexes = []}

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
            let cs = columns |> List.map(fun (c,d) -> let column = table.columns |> List.tryFind(fun col ->  match col with
                                                                                                                | IntColumn i when i.name = c -> true 
                                                                                                                | VarColumn v when v.name = c -> true
                                                                                                                | _ -> false)
                                                      match column with
                                                        | Some col -> (col, d)
                                                        | None -> failwithf "no column named %s exists on table %s" c table.name )
            let columnNames = cs |> List.map(fun(c,d) -> (getColumnName c)) |> fun cols -> String.Join("_", cols)
            let indexName = sprintf "IDX_%s_%s" table.name columnNames
            let index = {Constraint.name = indexName; columns = cs; clustering = clustering; uniqueness = uniqueness; constraintType = INDEX}
            {table with indexes = index::table.indexes}

        let WithClusteredIndex = withIndex CLUSTERED
        let WithNonClusteredIndex = withIndex NONCLUSTERED                           
                                                              
        let WithPrimaryKey clustering columns table =
            let cs = columns |> List.map(fun (c,d) -> let column = table.columns |> List.tryFind(fun col ->  match col with
                                                                                                                | IntColumn i when i.name = c -> true 
                                                                                                                | VarColumn v when v.name = c -> true
                                                                                                                | _ -> false)
                                                      match column with
                                                        | Some col when isColumnNullable col -> failwithf "column named %s is nullable, nullable columns are not allowed as part of a primary key" c 
                                                        | Some col -> (col, d)
                                                        | None -> failwithf "no column named %s exists on table %s" c table.name )  
            {table with primaryKey = Some {name = sprintf "PK_%s" table.name; columns = cs; clustering = clustering; uniqueness = UNIQUE; constraintType = PRIMARYKEY}}
              
        let WithClusteredPrimaryKey = WithPrimaryKey CLUSTERED
        let WithNonClusteredPrimaryKey = WithPrimaryKey NONCLUSTERED





           
                       
