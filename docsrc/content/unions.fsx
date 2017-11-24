(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Unions
======

JSON format does not support any data structure similiar to [F# discriminated unions](https://fsharpforfunandprofit.com/posts/discriminated-unions/).
Though it is still possible to represent union in JSON in some reasonable way.
By deafault FSharp.Json serializes union into JSON object with the only one field.
Name of the field corresponds to the union case. Value of the field corresponds to the union case value.

Here's some example of default union serialization:
*)

#r "FSharp.Json.dll"
open FSharp.Json

type TheUnion =
| OneFieldCase of string
| ManyFieldsCase of string*int

let data = OneFieldCase "The string"

let json = Json.serialize data
// json is """{"OneFieldCase":"The string"}"""

let deserialized = Json.deserialize<TheUnion> json
// deserialized is OneFieldCase("The string")

(**
Changing union case key
-----------------------

The string that represents union case key could be changed with [JsonUnionCase attribute](reference/fsharp-json-jsonunioncase.html).

See the example below:
*)

#r "FSharp.Json.dll"
open FSharp.Json

// OneFieldCase is attributed to be "case1" in JSON
type TheUnion =
| [<JsonUnionCase(Case="case1")>] OneFieldCase of string
| ManyFieldsCase of string*int

let data = OneFieldCase "The string"

let json = Json.serialize data
// json is """{"case1":"The string"}"""

let deserialized = Json.deserialize<TheUnion> json
// deserialized is OneFieldCase("The string")

(**
Union modes
-----------

All examples above are describing default serialization of union into JSON.
This mode is known as "case key as a field name" mode.
There's another [union mode](reference/fsharp-json-unionmode.html) that represents union as JSON object with two fields.
One field is for case key and another one is for case value. This mode is called "case key as a field value"
If this mode is used then names of these two field should be provided through [JsonUnion attribute](reference/fsharp-json-jsonunion.html).

See the example below: 
*)

#r "FSharp.Json.dll"
open FSharp.Json

// The union will be represented as JSON object with two fields: casekey and casevalue.
[<JsonUnion(Mode = UnionMode.CaseKeyAsFieldValue, CaseKeyField="casekey", CaseValueField="casevalue")>]
type TheUnion =
| OneFieldCase of string
| ManyFieldsCase of string*int

let data = OneFieldCase "The string"

let json = Json.serialize data
// json is """{"casekey":"OneFieldCase", "casevalue":"The string"}"""

let deserialized = Json.deserialize<TheUnion> json
// deserialized is OneFieldCase("The string")
