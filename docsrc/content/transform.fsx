(*** hide ***)
#I "../../bin/FSharp.Json"

(**
Type Transform
==============

[Supported types](supported_types.html) page maps F# types into JSON types.
What if some data needed to be represented as a different type then the default JSON type?
If changing type of the member in F# is not an option then type transform can help.

Any data member is translated F# Type -> JSON type by [default](supported_types.html) types mapping.
[Type Transform](reference/fsharp-json-itypetransform.html) is applied in the middle of this translation: F# Type -> Alternative F# Type -> JSON type.
Alternative F# Type -> JSON type is still done by default types mapping, type transform is responsible for F# Type -> Alternative F# Type.

The [Transforms](reference/fsharp-json-transforms.html) module contains transforms that are defined by FSharp.Json library.
You can define your own transforms by implementing [ITypeTransform interface](reference/fsharp-json-itypetransform.html).

DateTime as epoch time
----------------------

Let's imagine that some DateTime member should be represented as [epoch time](https://en.wikipedia.org/wiki/Unix_time) in JSON.
Epoch time is int64 however it is still convenient to work with DateTime in F# code.
In such case [DateTimeEpoch transform](reference/fsharp-json-transforms-datetimeepoch.html) is useful.
You can see it's implementation [here](https://github.com/vsapronov/FSharp.Json/blob/master/src/FSharp.Json/Transforms.fs).

Here's an example of DateTimeEpoch transform usage:
*)
#r "FSharp.Json.dll"
open System
open FSharp.Json

// value will be represented as epoch time in JSON
type DateTimeRecord = {
    [<JsonField(Transform=typeof<Transforms.DateTimeEpoch>)>]
    value: DateTime
}

let json = Json.serialize { DateTimeRecord.value = new DateTime(2017, 11, 5, 22, 50, 45) }
// json is """{"value":1509922245}"""

let deserialized = Json.deserialize<DateTimeRecord> json
// deserialized is { DateTimeRecord.value = new DateTime(2017, 11, 5, 22, 50, 45) }

(**

Transform by default
--------------------

To be developed....
*)
