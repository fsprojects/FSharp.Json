API Overview
============

Most of API functions are defined in [Json module](reference/fsharp-json-json.html).

Easiest way to serialize is to call `Json.serialize` function.
It serializes any supported F# type to string containing JSON.

Similarly easiest way to deserialize is to call `Json.deserialize<'T>` function.
It takes string containing JSON and returns instance of type 'T.

Advanced functions
------------------

Functions `Json.serialize` and `Json.deserialize` are using default configuration.
Whenever custom configuration should be used following functions are useful:

 * `Json.serializeEx`
 * `Json.deserializeEx<'T>` 

Prefix `Ex` stands for "extended". Both of these functions take [JsonConfig](reference/fsharp-json-jsonconfig.html) instance as a first parameter.

Configuration
-------------

[JsonConfig](reference/fsharp-json-jsonconfig.html) represents global configuration of serialization.
There's convenient way to override default configuration by using `JsonConfig.create` function.
All parameters of the function are optional and those that are provided override default values.

Examples of how to create and use JsonConfig could be found on pages discussing corresponding features.
For example, custom `jsonFieldNaming` could be found [here](fields_naming.html#Change-all-fields-names). 

Unformatted JSON
----------------

Some products like [Apache Spark](https://spark.apache.org/) require unformatted JSON in a single line.
It is usefull to produce unformatted single line JSON in some other scenarios.
There is a function to produce unformatted JSON: `Json.serializeU`.
`U` stands for "unformatted". It has the same signature as `Json.serialize` function.
The function is a shorthand to using `unformatted` member on [JsonConfig](reference/fsharp-json-jsonconfig.html).
