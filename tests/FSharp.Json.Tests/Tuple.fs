namespace FSharp.Json

module Tuple =
    open NUnit.Framework

    type TheTuple = {
        value: string*int*bool
    }

    [<Test>]
    let ``Tuple serialization/deserialization`` () =
        let expected = { TheTuple.value = ("The string", 123, true) }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheTuple> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Tuple serialized as array`` () =
        let value = { TheTuple.value = ("The string", 123, true) }
        let actual = Json.serializeU value
        let expected = """{"value":["The string",123,true]}"""
        Assert.AreEqual(expected, actual)

    type TheTupleWithOption = {
        value: string*int option*bool
    }

    [<Test>]
    let ``Tuple with optional serialization`` () =
        let value = { TheTupleWithOption.value = ("The string", None, true) }
        let actual = Json.serializeU value
        let expected = """{"value":["The string",null,true]}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Tuple with optional deserialization`` () =
        let expected = { TheTupleWithOption.value = ("The string", None, true) }
        let json = """{"value":["The string",null,true]}"""
        let actual = Json.deserialize<TheTupleWithOption> json
        Assert.AreEqual(expected, actual)
