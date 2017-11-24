(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Null Safety
===========

FSharp.Json is null safe.
This means that library will never deserialize JSON into anything with null value.
FSharp.Json treats option types as an instruction that value could be null in JSON.
For example member of type `string` can't get null value in FSharp.Json, however `string option` member can have `None` value which is translated into null in JSON.
Same logic applies to all types supported by FSharp.Json library.
See examples in sections below.

Deserialization of null
-----------------------

In the example below `stringMember` member can't be assigned to null even though F# allows string to be null:
*)
#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string
}

let json = """{"stringMember":null}"""

// this attempt to deserialize will throw exception 
let deserialized = Json.deserialize<RecordType> json

(**
What if the member `stringMember` could be null in JSON?
In such case the record type should explictly use string option type:
*)

#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string option
}

let json = """{"stringMember":null}"""

// this attempt to deserialize will work fine
let deserialized = Json.deserialize<RecordType> json

// deserialized.stringMember equals to None
printfn "%A" deserialized

(**
If value could be null or missing in JSON then F# type used for corresponding member should be option.

Customization of null deserialization
-------------------------------------

What is the difference between missing field in JSON and field assigned to null?
By default FSharp.Json library treats both cases as None, however you can customize this logic.
Behaviour that is used to treat option types could be configured:
*)

#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string option
}

// this will work fine by default even when option field is missing 
let deserialized1 = Json.deserialize<RecordType> "{}"

printfn "%A" deserialized1
// deserialized1.stringMember equals to None

// create config that require option None to be represented as null
let config = JsonConfig.create(deserializeOption = DeserializeOption.RequireNull)

// this will throw exception since config in use requires null for None deserialization.
let deserialized2 = Json.deserializeEx<RecordType> config "{}"

(**
Serialization of None
---------------------
If some member of `option` type has None value it will be serialized into JSON null by default:
*)

#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string option
}

// stringMember has None value
let data = { RecordType.stringMember = None }
let json = Json.serialize data
printfn "%s" json
// json is: """{ "stringMember": null }"""

(**
Customization of None serialization
-----------------------------------
The one can control how None values are respresented in JSON through config.

Example belows shows how to omit members with None values:
*)

#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string option
}

// stringMember has None value
let data = { RecordType.stringMember = None }

// create config that omits null values
let config = JsonConfig.create(serializeNone = SerializeNone.Omit)

let json = Json.serializeEx config data
printfn "%s" json
// json is: """{}"""
