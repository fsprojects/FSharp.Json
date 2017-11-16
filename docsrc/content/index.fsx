(*** hide ***)
#I "../../bin/FSharp.Json"

(**
FSharp.Json: JSON Serialization Library
=======================================

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

(**
Documentation
-------------

The library comes with following documentation. 

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library.
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation.

The library is available under Public Domain license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/vsapronov/FSharp.Json/tree/master/docsrc/content
  [gh]: https://github.com/vsapronov/FSharp.Json
  [issues]: https://github.com/vsapronov/FSharp.Json/issues
  [license]: https://github.com/vsapronov/FSharp.Json/blob/master/LICENSE.txt
*)
