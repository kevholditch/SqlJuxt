namespace SqlJuxtFunctionalTests.Tests


open FsUnit
open NUnit.Framework
open SqlJuxtFunctional.DatabaseTypes

    module NextAvailableNameTests =

        [<Test>]
        let ``should return name passed in when name is not in collection as collection is empty``() =
            getNextAvailableName "my_index" []
                |> should equal "my_index"

        [<Test>]
        let ``should return name passed in when name is not in collection``() =
            getNextAvailableName "my_index" ["some_index"; "some_other"]
                |> should equal "my_index"

        [<Test>]
        let ``should return my_index2 when my_index is in collection``() =
            getNextAvailableName "my_index" ["my_index"]
                |> should equal "my_index2"


        [<Test>]
        let ``should return my_index3 when my_index and my_index2 are in collection``() =
            getNextAvailableName "my_index" ["my_index"; "my_index2"]
                |> should equal "my_index3"

        [<Test>]
        let ``should return my_index3 when my_index and my_index2 and my_index33 are in collection``() =
            getNextAvailableName "my_index" ["my_index"; "my_index2"; "my_index33"]
                |> should equal "my_index3"

        [<Test>]
        let ``should return my_index2 when my_index and my_index22 are in collection``() =
            getNextAvailableName "my_index" ["my_index"; "my_index22"]
                |> should equal "my_index2"

        [<Test>]
        let ``should return my_index10 when my_index 2-9 are already in collection``() =
            getNextAvailableName "my_index" ["my_index"; "my_index2"; "my_index3"; "my_index4"; "my_index5"; "my_index6"; "my_index7"; "my_index8"; "my_index9"]
                |> should equal "my_index10"