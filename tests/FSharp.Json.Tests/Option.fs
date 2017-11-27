namespace FSharp.Json

module Option = 
    open NUnit.Framework

    type TheOption = {
        value: string option
    }

    [<Test>]
    let ``Option None serialization/deserialization`` () =
        let expected = { TheOption.value = None }
        let json = Json.serialize expected
        let actual = Json.deserialize<TheOption> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Option None deserialization from null`` () =
        let expected = { TheOption.value = None }
        let json = """{"value":null}"""
        let actual = Json.deserialize<TheOption> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Option None deserialization from omitted`` () =
        let expected = { TheOption.value = None }
        let json = """{}"""
        let actual = Json.deserialize<TheOption> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Option None serialization into null by default`` () =
        let actual = Json.serializeU { TheOption.value = None }
        let expected = """{"value":null}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Option Some serialization/deserialization`` () =
        let expected = { TheOption.value = Some "The string" }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheOption> json
        Assert.AreEqual(expected, actual)
        
    type TheNonOption = {
        value: string
    }

    [<Test>]
    let ``The null value is not allowed for non option type`` () =
        let json = """{"value":null}"""
        let ex = Assert.Throws<JsonDeserializationError>(fun () -> Json.deserialize<TheNonOption> json |> ignore)
        Assert.IsNotNull(ex)
        let expectedPath = "value"
        Assert.AreEqual(expectedPath, ex.Path.toString())

    [<Test>]
    let ``Omitted value is not allowed for non option type`` () =
        let json = """{}"""
        let ex = Assert.Throws<JsonDeserializationError>(fun () -> Json.deserialize<TheNonOption> json |> ignore)
        Assert.IsNotNull(ex)
        let expectedPath = "value"
        Assert.AreEqual(expectedPath, ex.Path.toString())

    let assertDeserializationThrows<'T> (config: JsonConfig) (json: string) =
        Assert.Throws<JsonDeserializationError>(fun () -> Json.deserializeEx<'T> config json |> ignore)

    [<Test>]
    let ``JsonConfig.deserializeOption - member with None value can't be omitted`` () =
        let json = """{}"""
        let config = JsonConfig.create(deserializeOption = DeserializeOption.RequireNull)
        let ex = assertDeserializationThrows<TheOption> config json
        let expectedPath = "value"
        Assert.AreEqual(expectedPath, ex.Path.toString())

    [<Test>]
    let ``JsonConfig.deserializeOption - member with None value is ommitted`` () =
        let json = """{}"""
        let expected = { TheOption.value = None }
        let config = JsonConfig.create(deserializeOption = DeserializeOption.AllowOmit)
        let actual = Json.deserializeEx<TheOption> config json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``JsonConfig.deserializeOption - member is null`` () =
        let json = """{"value":null}"""
        let expected = { TheOption.value = None }
        let config = JsonConfig.create(deserializeOption = DeserializeOption.RequireNull)
        let actual = Json.deserializeEx<TheOption> config json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``JsonConfig.serializeNone - member with None value as null`` () =
        let config = JsonConfig.create(unformatted = true, serializeNone = SerializeNone.Null)
        let actual = Json.serializeEx config { TheOption.value = None }
        let expected = """{"value":null}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``JsonConfig.serializeNone - member with None value is ommitted`` () =
        let config = JsonConfig.create(unformatted = true, serializeNone = SerializeNone.Omit)
        let actual = Json.serializeEx config { TheOption.value = None }
        let expected = """{}"""
        Assert.AreEqual(expected, actual)