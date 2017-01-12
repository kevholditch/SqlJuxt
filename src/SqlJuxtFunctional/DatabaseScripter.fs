namespace SqlJuxtFunctional

open DatabaseTypes
open System

module DatabaseScripter =

    let ScriptTable table =
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
                                | Some key ->   let cols = key.columns 
                                                                |> List.map(fun (c,d) -> sprintf "[%s] %s" (getColumnName c) (getDirectionString d))              
                                                                |> fun c -> String.Join(", ", c)
                                                let clustered = match key.Clustering with
                                                                    | CLUSTERED -> "CLUSTERED"
                                                                    | NONCLUSTERED -> "NONCLUSTERED"
                                                sprintf "%s%sALTER TABLE [dbo].[%s] ADD CONSTRAINT [%s] PRIMARY KEY %s (%s)%sGO" Environment.NewLine Environment.NewLine table.name key.name clustered cols Environment.NewLine
                                | None -> ""

        openScript + " " + columnScript + " )" + Environment.NewLine + "GO" + primaryKeyScript

    let Script catalog =
        String.Join(Environment.NewLine, catalog.tables |> List.rev |> List.map(ScriptTable))

