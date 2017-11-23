(**
FSharp.Json: JSON Serialization Library
=======================================

FSharp.Json is F# JSON serialization library based on Reflection it's written in F# for F#.

Find [basic usage example](basic_example.html) to get an idea how to use the library.

Why?
----

Why we need yet another F# JSON serialization library?
Well, if you happy with the library that you are using currently then probably you do not need another one.
There are several available options to choose from:

 * [JSON Type Provider](http://fsharp.github.io/FSharp.Data/library/JsonProvider.html)
 * [Json.Net aka Newtonsoft.Json](https://www.newtonsoft.com/json)
 * [Chiron](https://github.com/xyncro/chiron)
 * [JsonFSharp](https://github.com/PeteProgrammer/JsonFSharp)
 * Let me know what library I missed here

While all of these libraries do provide some good functionality all of them seem to have a weakness.
Some of them are written in C# without out of the box support for F# types.
Additionally null safety is alien concept in C# however it is a must in F#.
Other libraries provide only low-level functionality leaving a lot of cumbersome coding work to library user.
The type provider library forces developer to define JSON schema in form of json examples which is far from convenient way of defining schemas. 

FSharp.Json was developed with these values in mind:

 * Easy to use API
 * Automatic mapping of F# types into JSON
 * Out of the box support for F# types
 * Null safety

How?
----

FSharp.Json is pure F# library.
It uses [Reflection][reflection] to get information about F# types at runtime.
The code from [FSharp.Data library][fsharp_data] is used for parsing JSON.
The [JsonValue type][jsonvalue_type] is internalized in the FSharp.Json library.
The core of FSharp.Json library is located in single [Core.fs file][core].

  [reflection]: https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection
  [fsharp_data]: http://fsharp.github.io/FSharp.Data/
  [jsonvalue_type]: http://fsharp.github.io/FSharp.Data/reference/fsharp-data-jsonvalue.html
  [core]: https://github.com/vsapronov/FSharp.Json/blob/master/src/FSharp.Json/Core.fs

Documentation
-------------

Following pages are describing main features and aspects of FSharp.Json:

 * [Basic Usage Example](basic_example.html)
 * [Supported Types](supported_types.html)
 * [Fields Naming](fields_naming.html)
 * [Null Safety](null_safety.html)

Each feature of FSharp.Json is thoroughly covered by [unit tests][unit_tests].

[API Reference](reference/index.html) contains automatically generated documentation for all types, modules
and functions in the library.

  [unit_tests]: https://github.com/vsapronov/FSharp.Json/tree/master/tests/FSharp.Json.Tests

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
