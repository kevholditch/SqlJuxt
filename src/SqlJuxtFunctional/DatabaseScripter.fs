namespace SqlJuxtFunctional

open DatabaseTypes
open System

module DatabaseScripter =

    let rev xs = Seq.fold (fun acc x -> x::acc) [] xs

    let clusteredString c = match c with
                                | CLUSTERED -> "CLUSTERED"
                                | NONCLUSTERED -> "NONCLUSTERED"

    let ScriptTable table =
        let tableNameWithSchema = sprintf "[%s].[%s]" table.schema table.name

        let openScript = sprintf "CREATE TABLE %s(" tableNameWithSchema 
        let columnScript = table.columns 
                        |> rev
                        |> Seq.map(fun c -> match c with
                                            | IntColumn col when col.isNullable -> sprintf "[%s] [int] NULL" col.name 
                                            | IntColumn col                     -> sprintf "[%s] [int] NOT NULL" col.name 
                                            | VarColumn col when col.isNullable -> sprintf "[%s] [varchar](%i) NULL" col.name col.length
                                            | VarColumn col                     -> sprintf "[%s] [varchar](%i) NOT NULL" col.name col.length)
                        |> fun cols -> String.Join(", ", cols)

        let scriptColumnDirections columnDirections =
            columnDirections |> List.map(fun (c, d) -> sprintf "[%s] %s" (getColumnName c) (getDirectionString d))
                             |> fun c -> String.Join(", ", c)
                             |> fun s -> sprintf "(%s)" s

        let primaryKeyScript = match table.primaryKey with
                                | Some key ->   let cols = key.columns |> scriptColumnDirections
                                                let clustered = key.clustering |> clusteredString
                                                sprintf "%s%sALTER TABLE %s ADD CONSTRAINT [%s] PRIMARY KEY %s %s%sGO" Environment.NewLine Environment.NewLine tableNameWithSchema key.name clustered cols Environment.NewLine
                                | None -> ""

        let scriptIndex (index:Constraint) =
            let columnDirections = scriptColumnDirections index.columns
            let clustering = clusteredString index.clustering
            let unique = match index.uniqueness with
                            | UNIQUE -> "UNIQUE "
                            | _ -> ""
            sprintf "%s%sCREATE %s%s INDEX %s ON %s %s%sGO" Environment.NewLine Environment.NewLine unique clustering index.name tableNameWithSchema columnDirections Environment.NewLine

        let indexScript =
            table.indexes |> List.map(fun i -> scriptIndex i) |> fun s -> String.Join(Environment.NewLine, s)

        openScript + " " + columnScript + " )" + Environment.NewLine + "GO" + primaryKeyScript + indexScript

    let Script catalog =
        String.Join(Environment.NewLine, catalog.tables |> List.rev |> List.map(ScriptTable))

