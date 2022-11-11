# FSharp.Json: JSON Serialization Library

FSharp.Json is F# JSON serialization library based on Reflection it's written in F# for F#.

## Basic Usage Example

Here's basic example of FSharp.Json usage:

```fsharp
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
```

## Table of Contents
  - [Basic Usage Example](#basic-usage-example)
  - [Table of Contents](#table-of-contents)
  - [Why?](#why)
  - [How?](#how)
  - [Documentation](#documentation)
  - [API Overview](#api-overview)
      - [Advanced functions](#advanced-functions)
      - [Configuration](#configuration)
      - [Unformatted JSON](#unformatted-json)
  - [Supported Types](#supported-types)
  - [Customizing JSON Fields Names](#customizing-json-fields-names)
      - [Change JSON field name](#change-json-field-name)
      - [Change all fields names](#change-all-fields-names)
  - [Null Safety](#null-safety)
      - [Deserialization of null](#deserialization-of-null)
      - [Customization of null deserialization](#customization-of-null-deserialization)
      - [Serialization of None](#serialization-of-none)
      - [Customization of None serialization](#customization-of-none-serialization)
  - [Enums](#enums)
      - [Customizing enum serialization](#customizing-enum-serialization)
      - [Default enum behaviour](#default-enum-behaviour)
  - [Unions](#unions)
      - [Changing union case key](#changing-union-case-key)
      - [Union case without fields](#union-case-without-fields)
      - [Single case union](#single-case-union)
      - [Union modes](#union-modes)
  - [Type Transform](#type-transform)
      - [DateTime as epoch time](#datetime-as-epoch-time)
      - [System.Uri as string](#systemuri-as-string)
      - [Transform by default](#transform-by-default)
  - [Untyped Data](#untyped-data)
      - [Serialization of obj](#serialization-of-obj)
      - [Deserialization of obj](#deserialization-of-obj)
  - [Release Notes](#release-notes)
  - [Contributing and copyright](#contributing-and-copyright)
  - [Maintainer(s)](#maintainers)

## Why?

Why we need yet another F# JSON serialization library?
Well, if you happy with the library that you are using currently then probably you do not need another one.
There are several available options to choose from:

 * [JSON Type Provider](https://fsprojects.github.io/FSharp.Data/library/JsonProvider.html)
 * [Json.Net aka Newtonsoft.Json](https://www.newtonsoft.com/json)
 * [Fleece](https://github.com/fsprojects/Fleece)
 * [Chiron](https://github.com/xyncro/chiron)
 * [JsonFSharp](https://github.com/PeteProgrammer/JsonFSharp)
 * [Thoth.Json](https://mangelmaxime.github.io/Thoth/json/v2.html#code-sample)
 * [FSharpLu.Json](https://github.com/Microsoft/fsharplu/tree/master/FSharpLu.Json)
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

## How?

FSharp.Json is pure F# library.
It uses [Reflection][reflection] to get information about F# types at runtime.
The code from [FSharp.Data library][fsharp_data] is used for parsing JSON.
The [JsonValue type][jsonvalue_type] is internalized in the FSharp.Json library.
The core of FSharp.Json library is located in single [Core.fs file][core].

  [reflection]: https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection
  [fsharp_data]: https://fsprojects.github.io/FSharp.Data/
  [jsonvalue_type]: https://fsprojects.github.io/FSharp.Data/reference/fsharp-data-jsonvalue.html
  [core]: https://github.com/vsapronov/FSharp.Json/blob/735a312922ef701ef2fcc5379c44d5c483413bae/FSharp.Json/Core.fs

## Documentation

This document describe all details of FSharp.Json library. The source code also has thorough documentation in comments to main types. Each feature of FSharp.Json is thoroughly covered by [unit tests](FSharp.Json.Tests).

## API Overview

Most of API functions are defined in [Json module](FSharp.Json/Interface.fs).

Easiest way to serialize is to call `Json.serialize` function.
It serializes any supported F# type to string containing JSON.

Similarly easiest way to deserialize is to call `Json.deserialize<'T>` function.
It takes string containing JSON and returns instance of type 'T.

#### Advanced functions

Functions `Json.serialize` and `Json.deserialize` are using default configuration.
Whenever custom configuration should be used following functions are useful:

 * `Json.serializeEx`
 * `Json.deserializeEx<'T>` 

Prefix `Ex` stands for "extended". Both of these functions take [JsonConfig](FSharp.Json/InterfaceTypes.fs) instance as a first parameter.

#### Configuration

[JsonConfig](FSharp.Json/InterfaceTypes.fs) represents global configuration of serialization.
There's convenient way to override default configuration by using `JsonConfig.create` function.
All parameters of the function are optional and those that are provided override default values.

For example, custom `jsonFieldNaming` could be found [here](#change-all-fields-names). 

#### Unformatted JSON

Some products like [Apache Spark](https://spark.apache.org/) require unformatted JSON in a single line.
It is usefull to produce unformatted single line JSON in some other scenarios.
There is a function to produce unformatted JSON: `Json.serializeU`.
`U` stands for "unformatted". It has the same signature as `Json.serialize` function.
The function is a shorthand to using `unformatted` member on [JsonConfig](FSharp.Json/InterfaceTypes.fs).

## Supported Types

Here's full list of F# types that are supported by FSharp.Json library.

| F# Type | JSON Type |
|:---|:---|
sbyte<br>byte<br>int16<br>uint16<br>int<br>uint<br>int64<br>uint64<br>bigint<br>single<br>float<br>decimal | number
string | string
char | string
bool | bool
DateTime | string according to [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)<br>number epoch time using [transform](#transform)
DateTimeOffset | string according to [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)<br>number epoch time using [transform](#transform)
Guid | string
Uri| string using [transform](#transform)
Enum | string enum value name<br>number enum value<br>read [Enums](#enums) section
option | option is not represented by itself<br>`None` value might be represented as `null` or omitted<br>read more in [Null Safety](#null-safety) section
tuple | list
record | object
map | object
array<br>list | list
union | object with special structure<br>read more in [Unions](#unions) section
obj | read [Untyped Data](#untyped-data) section

## Customizing JSON Fields Names

When record type instance is serialized into JSON members names are used as JSON fields names.
In some scenarios the JSON should have different fields names then F# record type.
This section describes how FSharp.Json library provides JSON customization abilities.

#### Change JSON field name

JSON field name could be easily customized with JsonField attribute:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

// attribute stringMember with JsonField
type RecordType = {
    [<JsonField("different_name")>]
    stringMember: string
    intMember: int
}

let data: RecordType = { stringMember = "The string"; intMember = 123 }

let json = Json.serialize data
printfn "%s" json
// json is """{ "different_name": "The string", "intMember": 123 }"""
// pay attention that "different_name" was used as JSON field name

let deserialized = Json.deserialize<RecordType> json
printfn "%A" deserialized
// deserialized is {stringMember = "some value"; intMember = 123;}
```

In example above JsonField attribute affects both serialization and deserialization.

#### Change all fields names

What if all fields names should be different in JSON compared to F# member names?
This could be needed for example if naming convention used in F# does not match JSON naming convention.
FSharp.Json allows to map fields names with naming function.

In the example below all camel case F# record members are mapped into snake case JSON fields:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

// attribute stringMember with JsonField
type RecordType = {
    stringMember: string
    intMember: int
}

let data: RecordType = { stringMember = "The string"; intMember = 123 }

// create config with JSON field naming setting
let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)

let json = Json.serializeEx config data
printfn "%s" json
// json is """{ "string_member": "The string", "int_member": 123 }"""
// pay attention that "string_member" and "int_member" are in snake case

let deserialized = Json.deserializeEx<RecordType> config json
printfn "%A" deserialized
// deserialized is {stringMember = "some value"; intMember = 123;}
```

The `Json` module defines two naming functions that could be used out of the box: snakeCase and lowerCamelCase.
The one can define it's own naming rule - it's just a function `string -> string`.

## Null Safety

FSharp.Json is null safe.
This means that library will never deserialize JSON into anything with null value.
FSharp.Json treats option types as an instruction that value could be null in JSON.
For example member of type `string` can't get null value in FSharp.Json, however `string option` member can have `None` value which is translated into null in JSON.
Same logic applies to all types supported by FSharp.Json library.
See examples in sections below.

#### Deserialization of null

In the example below `stringMember` member can't be assigned to null even though F# allows string to be null:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

type RecordType = {
    stringMember: string
}

let json = """{"stringMember":null}"""

// this attempt to deserialize will throw exception 
let deserialized = Json.deserialize<RecordType> json
```

What if the member `stringMember` could be null in JSON?
In such case the record type should explictly use string option type:

```fsharp
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
```

If value could be null or missing in JSON then F# type used for corresponding member should be option.

#### Customization of null deserialization

What is the difference between missing field in JSON and field assigned to null?
By default FSharp.Json library treats both cases as None, however you can customize this logic.
Behaviour that is used to treat option types could be configured:

```fsharp
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
```

#### Serialization of None
If some member of `option` type has None value it will be serialized into JSON null by default:

```fsharp
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
```

#### Customization of None serialization
The one can control how None values are respresented in JSON through config.
Example belows shows how to omit members with None values:

```fsharp
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
```

## Enums

By default enum value is represented as `string` that is enum member name.

Check example code below:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

type NumberEnum =
| One = 1
| Two = 2
| Three = 3

// value will be represented as enum value name in JSON
type TheNumberEnum = {
    value: NumberEnum
}

let data = { TheNumberEnum.value = NumberEnum.Three }

let json = Json.serialize data
// json is """{"value":"Three"}"""

let deserialized = Json.deserialize<TheNumberEnum> json
// data is { TheNumberEnum.value = NumberEnum.Three }
```

#### Customizing enum serialization

EnumValue member of [JsonField](FSharp.Json/InterfaceTypes.fs) attribute could be used to change serialization of enums.
There are two [modes](FSharp.Json/InterfaceTypes.fs) supported currently: enum value name and enum value.

Here's an example of custom enum serialization:
```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

type NumberEnum =
| One = 1
| Two = 2
| Three = 3

// value will be represented as enum value in JSON
type TheAttributedNumberEnum = {
    [<JsonField(EnumValue = EnumMode.Value)>]
    value: NumberEnum
}

let data = { TheNumberEnum.value = NumberEnum.Three }

let json = Json.serialize data
// json is """{"value":3}"""

let deserialized = Json.deserialize<TheNumberEnum> json
// data is { TheNumberEnum.value = NumberEnum.Three }
```

#### Default enum behaviour

Sometimes it's needed always serialize enum value as it's value.
Annotating each member of any enum type would be cumbersome.
[JsonConfig](https://github.com/vsapronov/FSharp.Json/blob/master/FSharp.Json/InterfaceTypes.fs) allows to override default enum behaviour.

Check the example below:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

type NumberEnum =
| One = 1
| Two = 2
| Three = 3

// value will be represented as enum value name in JSON
type TheNumberEnum = {
    value: NumberEnum
}

let data = { TheNumberEnum.value = NumberEnum.Three }

// create configuration instructing to serialize enum as enum value
let config = JsonConfig.create(enumValue = EnumMode.Value)

let json = Json.serializeEx config data
// json is """{"value":3}"""
// value was serialized as enum value which is 3

let deserialized = Json.deserializeEx<TheNumberEnum> config json
// data is { TheNumberEnum.value = NumberEnum.Three }
```

## Unions

JSON format does not support any data structure similiar to [F# discriminated unions](https://fsharpforfunandprofit.com/posts/discriminated-unions/).
Though it is still possible to represent union in JSON in some reasonable way.
By deafault FSharp.Json serializes union into JSON object with the only one field.
Name of the field corresponds to the union case. Value of the field corresponds to the union case value.

Here's some example of default union serialization:

```fsharp
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
```

#### Changing union case key

The string that represents union case key could be changed with [JsonUnionCase attribute](FSharp.Json/InterfaceTypes.fs).

See the example below:

```fsharp
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
```

#### Single case union

Single case union is a special scenario.
Read [here](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/) about single case union usage.
In such case serializing union as JSON object is overkill.
It's more convenient to represent single case union the same way as a wrapped type.

Here's example of single case union serialization:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

// Single case union type
type TheUnion = SingleCase of string

type TheRecord = {
    // value will be just a string - wrapped into union type
    value: TheUnion
}

let data = { TheRecord.value = SingleCase "The string" }

let json = Json.serialize data
// json is """{"value":"The string"}"""

let deserialized = Json.deserialize<TheRecord> json
// deserialized is { TheRecord.value = SingleCase "The string" }
```

#### Union case without fields

When union case does not have fields then the union value is represented by string value of the case name itself.

Here's example of serialization union case without fields:

```fsharp
#r "FSharp.Json.dll"
open FSharp.Json

// Case NoFieldCase does not have any fields
type TheUnion =
| NoFieldCase
| SingleCase of string

type TheRecord = {
    // value will be a string represting NoFieldCase
    value: TheUnion
}

let data = { TheRecord.value = NoFieldCase }

let json = Json.serialize data
// json is """{"value":"NoFieldCase"}"""

let deserialized = Json.deserialize<TheRecord> json
// deserialized is { TheRecord.value = NoFieldCase }
```

#### Union modes

There's [union mode](FSharp.Json/InterfaceTypes.fs) that represents union as JSON object with two fields.
One field is for case key and another one is for case value. This mode is called "case key as a field value"
If this mode is used then names of these two field should be provided through [JsonUnion attribute](FSharp.Json/InterfaceTypes.fs).

See the example below: 

```fsharp
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
```

## Type Transform

[Supported types](#supported-types) section maps F# types into JSON types.
What if some data needed to be represented as a different type then the default JSON type?
If changing type of the member in F# is not an option then type transform can help.

Any data member is translated F# Type -> JSON type by [default](#supported-types) types mapping.
[Type Transform](FSharp.Json/InterfaceTypes.fs) is applied in the middle of this translation: F# Type -> Alternative F# Type -> JSON type.
Alternative F# Type -> JSON type is still done by default types mapping, type transform is responsible for F# Type -> Alternative F# Type.

The [Transforms](FSharp.Json/Transforms.fs) module contains transforms that are defined by FSharp.Json library.
You can define your own transforms by implementing [ITypeTransform interface](FSharp.Json/InterfaceTypes.fs).

#### DateTime as epoch time

Let's imagine that some DateTime member should be represented as [epoch time](https://en.wikipedia.org/wiki/Unix_time) in JSON.
Epoch time is int64 however it is still convenient to work with DateTime in F# code.
In such case [DateTimeEpoch transform](FSharp.Json/Transforms.fs) is useful.

Here's an example of DateTimeEpoch transform usage:

```fsharp
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
```

#### System.Uri as string

This transformer transforms a Uri to/from a string for serialization. On deserialization, the string value is
handed to the System.Uri constructor. When the URI string is malformed, a UriFormatException might be thrown.

Example use:

```fsharp
#r "FSharp.Json.dll"
open System
open FSharp.Json

// value will be represented as epoch time in JSON
type UriRecord = {
    [<JsonField(Transform=typeof<Transforms.UriTransform>)>]
    value: Uri
}

let json = Json.serialize { UriRecord.value = Uri("http://localhost:8080/") }
// json is """{"value":"http://localhost:8080/"}"""

let deserialized = Json.deserialize<UriRecord> json
// deserialized is { UriRecord.value = Uri("http://localhost:8080/") }
```

#### Transform by default

To be developed....

## Untyped Data

Using obj type in F# code is bad code smell.
Though FSharp.Json can serialize and deserialize structures without type information.
For allowing obj type in serialization/deserialization allowUntyped flag should be set to `true` on [JsonConfig](FSharp.Json/InterfaceTypes.fs).

#### Serialization of obj

When serializing obj FSharp.Json uses real run time type.

Check this example:

```fsharp
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
```

#### Deserialization of obj

When deserializing obj FSharp.Json assumes the type from JSON.

See example below:

```fsharp
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
```

## Release Notes

Could be found [here](RELEASE_NOTES.md).

## Contributing and copyright

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding documentation to this [README][readme].

The library is available under Public Domain license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [readme]: README.md 
  [gh]: https://github.com/vsapronov/FSharp.Json
  [issues]: https://github.com/vsapronov/FSharp.Json/issues
  [license]: https://github.com/vsapronov/FSharp.Json/blob/master/LICENSE.txt

## Maintainer(s)

- [@vsapronov](https://github.com/vsapronov)
