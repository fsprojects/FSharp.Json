(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Customizing JSON Fields Names
=============================

When record type instance is serialized into JSON members names are used as JSON fields names.
In some scenarios the JSON should have different fields names then F# record type.
This page describes how FSharp.Json library provides JSON customization abilities.

Change JSON field name
----------------------

JSON field name could be easily customized with JsonField attribute:
*)
#r "FSharp.Json.dll"
open FSharp.Json

// attribute stringMember with JsonField
type RecordType = {
    [<JsonField("different_name")>]
    stringMember: string
    intMember: int
}

let data: RecordType = { stringMember = "The string"; intMember = 123 }

let json = Json.serialize data
printfn "%s" json
// json is """{ "different_name": "The string", "intMember": 123 }"""
// pay attention that "different_name" was used as JSON field name

let deserialized = Json.deserialize<RecordType> json
printfn "%A" deserialized
// deserialized is {stringMember = "some value"; intMember = 123;}

(**
In example above JsonField attribute affects both serialization and deserialization.

Change all fields names
-----------------------

What if all fields names should be different in JSON compared to F# member names?
This could be needed for example if naming convention used in F# does not match JSON naming convention.
FSharp.Json allows to map fields names with naming function.

In the example below all camel case F# record members are mapped into snake case JSON fields:
*)

#r "FSharp.Json.dll"
open FSharp.Json

// attribute stringMember with JsonField
type RecordType = {
    stringMember: string
    intMember: int
}

let data: RecordType = { stringMember = "The string"; intMember = 123 }

// create config with JSON field naming setting
let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)

let json = Json.serializeEx config data
printfn "%s" json
// json is """{ "string_member": "The string", "int_member": 123 }"""
// pay attention that "string_member" and "int_member" are in snake case

let deserialized = Json.deserializeEx<RecordType> config json
printfn "%A" deserialized
// deserialized is {stringMember = "some value"; intMember = 123;}

(**
The `Json` module defines two naming functions that could be used out of the box: snakeCase and lowerCamelCase.
The one can define it's own naming rule - it's just a function `string -> string`.
*)
