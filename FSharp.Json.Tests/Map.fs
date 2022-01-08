namespace FSharp.Json

module Map =
    open System
    open NUnit.Framework

    [<Test>]
    let ``Map<string,string> serialization`` () =
        let expected = """{"key":"value"}"""
        let value = Map.ofList [("key", "value")]
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Map<string,string> deserialization`` () =
        let json = """{"key":"value"}"""
        let expected = Map.ofList [("key", "value")]
        let actual = Json.deserialize<Map<string, string>> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Map<string,obj> serialization`` () =
        let expected = """{"key1":"value","key2":123}"""
        let value = Map.ofList [("key1", "value" :> obj); ("key2", 123 :> obj)]
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Map<string,obj> deserialization`` () =
        let json = """{"key1":"value","key2":123}"""
        let expected = Map.ofList [("key1", "value" :> obj); ("key2", 123 :> obj)]
        let config = JsonConfig.create(allowUntyped = true)
        let actual = Json.deserializeEx<Map<string, obj>> config json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Map<string,null> serialization`` () =
        let expected = """{"key1":[null],"key2":null}"""
        let value = Map.ofList [("key1", [()] :> obj); ("key2", null)]
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)
