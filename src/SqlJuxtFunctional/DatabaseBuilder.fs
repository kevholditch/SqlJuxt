namespace SqlJuxtFunctional



module DatabaseBuilder =

    module TableBuilder =

        open System
        open DatabaseTypes
        open DatabaseScripter

        let CreateTable name =
            {schema = "dbo"; name = name; columns = []; primaryKey = None}

        let private withInt name nullable table =
            let c = IntColumn {name = name; isNullable = nullable}
            {table with Table.columns = c::table.columns}

        let WithNullableInt name table =
            withInt name true table

        let WithInt name table =
            withInt name false table

        let private withVarchar name nullable length table =
            let c = VarColumn {name = name; isNullable = nullable; length = length}
            {table with Table.columns = c::table.columns}

        let WithNullableVarchar name length table =
            withVarchar name true length table

        let WithVarchar name length table =
            withVarchar name false length table

        let private getColumnsByNames (columnNames: (string * SortDirection) list) table =
            columnNames |> List.map(fun (c,d) -> let column = table.columns |> List.tryFind(fun col ->  match col with
                                                                                            | IntColumn i when i.name = c -> true 
                                                                                            | VarColumn v when v.name = c -> true
                                                                                            | _ -> false)
                                                 match column with 
                                                    | Some col -> (col, d)
                                                    | None -> failwithf "no column named %s exists on table %s" c table.name )                                                 
                                                              
        let WithPrimaryKey clustering columns table =
            let cs = getColumnsByNames columns table
            {table with primaryKey = Some {name = sprintf "PK_%s" table.name; columns = cs; Clustering = clustering}}
              
        let WithClusteredPrimaryKey = WithPrimaryKey CLUSTERED
        let WithNonClusteredPrimaryKey = WithPrimaryKey NONCLUSTERED

        let Build table =
            ScriptTable table



           
                       
