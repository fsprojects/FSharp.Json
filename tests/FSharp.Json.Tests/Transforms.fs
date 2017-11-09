namespace FSharp.Json

module Transforms =
    open System
    open NUnit.Framework

    type DateTimeRecord = {
        [<JsonField(Transform=typeof<Transforms.DateTimeEpoch>)>]
        value: DateTime
    }

    [<Test>]
    let ``DateTime as Epoch serialization`` () =
        let actual = Json.serializeU { DateTimeRecord.value = new DateTime(2017, 11, 5, 22, 50, 45) }
        let expected = """{"value":1509922245}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``DateTime as Epoch deserialization`` () =
        let expected = { DateTimeRecord.value = new DateTime(2017, 11, 5, 22, 50, 45) }
        let json = """{"value":1509922245}"""
        let actual = Json.deserialize<DateTimeRecord> json
        Assert.AreEqual(expected, actual)

    type DateTimeOffsetRecord = {
        [<JsonField(Transform=typeof<Transforms.DateTimeOffsetEpoch>)>]
        value: DateTimeOffset
    }

    [<Test>]
    let ``DateTimeOffset as Epoch serialization`` () =
        let actual = Json.serializeU { DateTimeOffsetRecord.value = new DateTimeOffset(2017, 11, 5, 22, 50, 45, TimeSpan(0L)) }
        let expected = """{"value":1509922245}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``DateTimeOffset as Epoch deserialization`` () =
        let expected = { DateTimeOffsetRecord.value = new DateTimeOffset(2017, 11, 5, 22, 50, 45, TimeSpan(0L)) }
        let json = """{"value":1509922245}"""
        let actual = Json.deserialize<DateTimeOffsetRecord> json
        Assert.AreEqual(expected, actual)
