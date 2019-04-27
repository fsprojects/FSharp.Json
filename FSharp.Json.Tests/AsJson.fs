namespace FSharp.Json

module AsJson =
    open System
    open NUnit.Framework

    type AsJsonRecord = {
        [<JsonField(AsJson=true)>]
        value: string
    }

    [<Test>]
    let ``AsJson member serialization - object`` () =
        let value = { AsJsonRecord.value = """{"property":"The value"}""" }
        let actual = Json.serializeU value
        let expected = """{"value":{"property":"The value"}}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``AsJson member deserialization - object`` () =
        let json = """{"value":{"property":"The value"}}"""
        let actual = Json.deserialize<AsJsonRecord> json
        let expected = { AsJsonRecord.value = """{"property":"The value"}"""}
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``AsJson member serialization - string`` () =
        let expected = """{"value":"The value"}"""
        let value = { AsJsonRecord.value = "The value" }
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``AsJson member deserialization - string`` () =
        let json = """{"value":"The value"}"""
        let expected = { AsJsonRecord.value = "The value" }
        let actual = Json.deserialize<AsJsonRecord> json
        Assert.AreEqual(expected, actual)

    type AsJsonOptionalRecord = {
        [<JsonField(AsJson=true)>]
        value: string option
    }

    [<Test>]
    let ``AsJson member serialization - None`` () =
        let value = { AsJsonOptionalRecord.value = None }
        let actual = Json.serializeU value
        let expected = """{"value":null}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``AsJson member deserialization - null`` () =
        let actual = Json.deserialize<AsJsonOptionalRecord> """{"value":null}"""
        let expected = { AsJsonOptionalRecord.value = None }
        Assert.AreEqual(expected, actual)
