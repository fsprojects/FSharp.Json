# FSharp.Json

**FSharp.Json** is F# JSON serialization library based on Reflection.

See [the home page](http://vsapronov.github.io/FSharp.Json) for details.

Here's example of FSharp.Json usage:

```fsharp
open FSharp.Json

// Your record type
type RecordType = {
	stringmember: string
	intmember: int
}

let data: RecordType = { stringmember = "some value"; intmember = 42 }

// serialize record into JSON
let json = Json.serialize data

// json is """{ "stringmember": "some value", "intmember": 42 }"""

// deserialize from JSON to record
let data = Json.deserialize<RecordType> json
```

## Maintainer(s)

- [@vsapronov](https://github.com/vsapronov)
