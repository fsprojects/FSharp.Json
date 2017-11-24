(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Emums
=====

By default enum value is represented as `string` that is enum member name.

Check example code below:
*)
#r "FSharp.Json.dll"
open FSharp.Json

type NumberEnum =
| One = 1
| Two = 2
| Three = 3

// value will be represented as enum value name in JSON
type TheNumberEnum = {
    value: NumberEnum
}

let data = { TheNumberEnum.value = NumberEnum.Three }

let json = Json.serialize data
// json is """{"value":"Three"}"""

let deserialized = Json.deserialize<TheNumberEnum> json
// data is { TheNumberEnum.value = NumberEnum.Three }

(**

Customizing enum serialization
------------------------------

EnumValue member of [JsonField](reference/fsharp-json-jsonfield.html) attribute could be used to change serialization of enums.
There are two [modes](reference/fsharp-json-enummode.html) supported currently: enum value name and enum value.

Here's an example of custom enum serialization:
*)
#r "FSharp.Json.dll"
open FSharp.Json

type NumberEnum =
| One = 1
| Two = 2
| Three = 3

// value will be represented as enum value in JSON
type TheAttributedNumberEnum = {
    [<JsonField(EnumValue = EnumMode.Value)>]
    value: NumberEnum
}

let data = { TheNumberEnum.value = NumberEnum.Three }

let json = Json.serialize data
// json is """{"value":3}"""

let deserialized = Json.deserialize<TheNumberEnum> json
// data is { TheNumberEnum.value = NumberEnum.Three }
