namespace SqlJuxtFunctionalTests

open SqlJuxtFunctional.DatabaseTypes

    module TestModule =
        let extractDifferences r =
            match r with
            | Differences(r') -> r'
            | _ -> failwith "expected databases to be different but they are a match"

