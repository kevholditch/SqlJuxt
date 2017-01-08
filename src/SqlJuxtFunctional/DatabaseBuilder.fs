namespace SqlJuxtFunctional

module DatabaseBuilder =

    module TableBuilder =

        open System
        open DatabaseTypes

        let CreateTable name =
            {name = name; columns = []; primaryKey = None}

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

        let private getColumnNames columns =
            columns |> List.map(fun c -> match c with
                                            | IntColumn i -> i.name
                                            | VarColumn v -> v.name)

        let private getColumnsByNames columnNames table =
            columnNames |> List.map(fun c -> table.columns |> List.find(fun col ->  match col with
                                                                                        | IntColumn i when i.name = c -> true 
                                                                                        | VarColumn v when v.name = c -> true
                                                                                        | _ -> failwithf "no column exists named %s on table %s" c table.name))

        let WithPrimaryKeyNamed keyName (columnNames:string list) table =
            let cs = getColumnsByNames columnNames table
            {table with primaryKey = Some {name = keyName; columns = cs}}
       

        let Build table =
            let openScript = sprintf "CREATE TABLE [dbo].[%s](" table.name
            let columnScript = table.columns 
                            |> Seq.rev
                            |> Seq.map(fun c -> match c with
                                                | IntColumn col when col.isNullable -> sprintf "[%s] [int] NULL" col.name 
                                                | IntColumn col                     -> sprintf "[%s] [int] NOT NULL" col.name 
                                                | VarColumn col when col.isNullable -> sprintf "[%s] [varchar](%i) NULL" col.name col.length
                                                | VarColumn col                     -> sprintf "[%s] [varchar](%i) NOT NULL" col.name col.length)
                            |> fun cols -> String.Join(", ", cols)

            let primaryKeyScript = match table.primaryKey with
                                    | Some key ->   let columnNames = getColumnNames key.columns |> List.map(fun c -> sprintf "[%s] ASC" c) |> fun c -> String.Join(", ", c)
                                                    sprintf "%s%sALTER TABLE [dbo].[%s] ADD CONSTRAINT [%s] PRIMARY KEY CLUSTERED (%s)%sGO" Environment.NewLine Environment.NewLine table.name key.name columnNames Environment.NewLine
                                    | None -> ""

            openScript + " " + columnScript + " )" + Environment.NewLine + "GO" + primaryKeyScript



           
                       
