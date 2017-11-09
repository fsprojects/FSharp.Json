# FSharp.Json

**FSharp.Json** is F# JSON serialization library based on Reflection.

See [the home page](http://vsapronov.github.io/FSharp.Json) for details.

Here's basic example of FSharp.Json usage:

```fsharp
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
```

## Maintainer(s)

- [@vsapronov](https://github.com/vsapronov)
