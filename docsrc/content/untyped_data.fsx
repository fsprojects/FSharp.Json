(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Untyped Data
============

Using obj type in F# code is bad code smell.
Though FSharp.Json can serialize and deserialize structures without type information.
For allowing obj type in serialization/deserialization allowUntyped flag should be set to `true` on [JsonConfig](reference/fsharp-json-jsonconfig.html).

Serialization of obj
--------------------

When serializing obj FSharp.Json uses real run time type.

Check this example:
*)

#r "FSharp.Json.dll"
open FSharp.Json

// Record type with obj member
type ObjectRecord = {
    value: obj
}

// Set string value to obj member
let data = { ObjectRecord.value = "The string" }

// Need to allow untyped data
let config = JsonConfig.create(allowUntyped = true)

let json = Json.serializeEx config data
// json is """{"value": "The string"}"""
// value was serialized as string because it was assigned to string

(**
Deserialization of obj
----------------------

When deserializing obj FSharp.Json assumes the type from JSON.

See example below:
*)

#r "FSharp.Json.dll"
open FSharp.Json

// Record type with obj member
type ObjectRecord = {
    value: obj
}

// value is assigned to string
let json = """{"value": "The string"}"""

// Need to allow untyped data
let config = JsonConfig.create(allowUntyped = true)

let data = Json.deserializeEx<ObjectRecord> config json
// data is { ObjectRecord.value = "The string" }
// value was deserialized as string because it was string in JSON
