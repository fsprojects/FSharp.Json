namespace FSharp.Json

module Collections =
    open System
    open NUnit.Framework

    [<Test>]
    let ``Array serialization to JSON array`` () =
        let expected = """["some","text"]"""
        let value = [|"some"; "text"|]
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``List serialization to JSON array`` () =
        let expected = """["some","text"]"""
        let value = ["some"; "text"]
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Array serialization/deserialization`` () =
        let expected = [|"some"; "text"|]
        let json = Json.serialize(expected)
        let actual = Json.deserialize<string array> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``List serialization/deserialization`` () =
        let expected = ["some"; "text"]
        let json = Json.serialize(expected)
        let actual = Json.deserialize<string list> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Array empty serialization/deserialization`` () =
        let expected = [||]
        let json = Json.serialize(expected)
        let actual = Json.deserialize<string array> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``List empty serialization/deserialization`` () =
        let expected = []
        let json = Json.serialize(expected)
        let actual = Json.deserialize<string list> json
        Assert.AreEqual(expected, actual)
