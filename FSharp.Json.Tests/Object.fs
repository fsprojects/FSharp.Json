namespace FSharp.Json

module Object =
    open System
    open NUnit.Framework

    type ObjectRecord = {
        value: obj
    }

    [<Test>]
    let ``Object serialization/deserialization`` () =
        let expected = { ObjectRecord.value = "The string" }
        let config = JsonConfig.create(allowUntyped = true, unformatted = true)
        let json = Json.serializeEx config expected
        let actual = Json.deserializeEx<ObjectRecord> config json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Object with null serialization/deserialization`` () =
        let expectedMap =
            Map.empty
            |> Map.add "stringValue" ("The string" :> obj)
            |> Map.add "intValue" (42M :> obj)
            |> Map.add "nullValue" null
        let expected = { ObjectRecord.value = expectedMap }
        let config = JsonConfig.create(allowUntyped = true, unformatted = true)
        let json = Json.serializeEx config expected
        let actual = Json.deserializeEx<ObjectRecord> config json
        Assert.AreEqual(expected, actual)
