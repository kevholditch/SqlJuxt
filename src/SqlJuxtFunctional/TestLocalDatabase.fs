﻿namespace SqlJuxtFunctional

module TestLocalDatabase =

    open System
    open System.IO
    open FSharp.Data
    open System.Data.SqlClient
   
    let rev xs = Seq.fold (fun acc x -> x::acc) [] xs

    let tryHead xs =
        match xs with
        | x::xs -> Some x
        | _ -> None

    let localConnectionString =
        let serverDirectory = new DirectoryInfo(@"C:\Program Files\Microsoft SQL Server")
        let version = serverDirectory.GetDirectories("LocalDb", SearchOption.AllDirectories)
                        |> Seq.map(fun dir -> dir.Parent.Name)
                        |> Seq.sortBy(fun dir -> dir)
                        |> rev
                        |> tryHead

        let dataSource = match version with 
                            | Some v when v = "110" -> @"(LocalDB)\v11.0" 
                            | Some v -> @"(LocalDB)\MSSQLLocalDB"
                            | None -> failwith "No instance of local db is installed on this machine"

        sprintf "Data Source=%s;Integrated Security=True;Pooling=false;" dataSource

    let getCatalogConnectionString catalog =
        localConnectionString + "Initial Catalog=" + catalog + ";"

    let createUniqueDbName () =
        let time = DateTime.UtcNow.ToString("yyyyMMddHHmmssFF") 
        let guid = Guid.NewGuid().ToString("N")
        sprintf "SqlJuxt-%s-%s" time guid
     
    let run connString (script:string) =
        use conn = new SqlConnection(connString)

        try conn.Open() 
        with ex -> failwithf "Failed to connect to DB %s with Error %s " connString ex.Message

        let scripts = script.Split([|"GO" + Environment.NewLine; Environment.NewLine + "GO" |], StringSplitOptions.RemoveEmptyEntries)
            
        for s in scripts do
            use cmd = new SqlCommand( Connection = conn, CommandText = s, CommandType = Data.CommandType.Text )
            cmd.ExecuteNonQuery() |> ignore

    let runScriptOnMaster =
        let masterConnectionString = getCatalogConnectionString "master"
        run masterConnectionString

    let dropDatabase dbName =
        let script = sprintf @"USE master;
ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [%s] ;" dbName dbName
        runScriptOnMaster script |> ignore
        
    type DisposableDatabase(name, connectionString) =

        member this.ConnectionString = connectionString
        member this.Name = name

        interface System.IDisposable with 
            member this.Dispose() = 
                dropDatabase this.Name
           
    let createDatabase () =
        let dbName = createUniqueDbName() 
        let script = sprintf "CREATE DATABASE [%s]" dbName
        
        let result = runScriptOnMaster script

        new DisposableDatabase (dbName, getCatalogConnectionString dbName)

    let runScript (database:DisposableDatabase) script =
        run database.ConnectionString script


            


    