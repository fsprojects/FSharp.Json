namespace FSharp.Json

module Default =
    open System
    open NUnit.Framework

    type AnnotatedRecord = {
        [<JsonField(DefaultValue = "The default value")>]
        Value: string
    }

    [<Test>]
    let ``Default value for omitted field`` () =
        let expected = { AnnotatedRecord.Value = "The default value" }
        let json = "{}"
        let actual = Json.deserialize<AnnotatedRecord> json
        Assert.AreEqual(expected, actual)
