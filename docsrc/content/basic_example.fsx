(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Basic Usage Example
===================

Here's basic example of FSharp.Json usage:
*)
#r "FSharp.Json.dll"
open FSharp.Json

// Your record type
type RecordType = {
    stringMember: string
    intMember: int
}

let data: RecordType = { stringMember = "The string"; intMember = 123 }

// serialize record into JSON
let json = Json.serialize data
printfn "%s" json
// json is """{ "stringMember": "The string", "intMember": 123 }"""

// deserialize from JSON to record
let deserialized = Json.deserialize<RecordType> json
printfn "%A" deserialized
// deserialized is {stringMember = "some value"; intMember = 123;}
