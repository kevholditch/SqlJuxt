namespace SqlJuxtFunctional

module LibraryFunctions =

    open System
    open System.Text.RegularExpressions

    let (|ParseRegex|_|) regex str =
        let m = Regex(regex).Match(str)
        match m with 
            | m when m.Success -> Some (List.tail [ for x in m.Groups -> x.Value] )
            | _ -> None

    let (|Integer|_|) (s:string) = 
            match Int32.TryParse(s) with
                | (true, i) -> Some i
                | _ -> None  