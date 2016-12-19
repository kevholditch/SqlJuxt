namespace SqlJuxtFunctional

open System

module DatabaseBuilder =


    type IntegerColumn = {name: string; isNullable: bool}
    type VarcharColumn = {name: string; isNullable: bool; length: int}
    type Column = I of IntegerColumn | V of VarcharColumn
    type Table = {name: string; columns: Column list}

    module TableBuilder =

        let Create name =
            {name = name; columns = []}

        let private withInt name nullable table =
            let c = I {name = name; isNullable = nullable}
            {name = table.name; columns = c::table.columns}

        let WithNullableInt name table =
            withInt name true table

        let WithInt name table =
            withInt name false table

        let private withVarchar name nullable length table =
            let c = V {name = name; isNullable = nullable; length = length}
            {name = table.name; columns = c::table.columns}

        let WithNullableVarchar name length table =
            withVarchar name true length table

        let WithVarchar name length table =
            withVarchar name false length table

        let Build table =
            ""



           
                       
