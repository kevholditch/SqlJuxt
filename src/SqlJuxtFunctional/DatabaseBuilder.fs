namespace SqlJuxtFunctional

module DatabaseBuilder =

    module TableBuilder =

        open System
        open DatabaseTypes

        let CreateTable name =
            {name = name; columns = []}

        let private withInt name nullable table =
            let c = IntColumn {name = name; isNullable = nullable}
            {name = table.name; columns = c::table.columns}

        let WithNullableInt name table =
            withInt name true table

        let WithInt name table =
            withInt name false table

        let private withVarchar name nullable length table =
            let c = VarColumn {name = name; isNullable = nullable; length = length}
            {name = table.name; columns = c::table.columns}

        let WithNullableVarchar name length table =
            withVarchar name true length table

        let WithVarchar name length table =
            withVarchar name false length table

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
            openScript + " " + columnScript + " )" + Environment.NewLine + "GO"



           
                       
