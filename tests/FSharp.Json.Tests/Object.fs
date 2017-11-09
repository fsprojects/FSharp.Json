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
        let json = Json.serializeU expected
        let actual = Json.deserialize<ObjectRecord> json
        Assert.AreEqual(expected, actual)
