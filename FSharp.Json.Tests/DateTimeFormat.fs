namespace FSharp.Json

module DateTimeFormat =
    open System
    open NUnit.Framework

    type TheDateTimeOffset = {
        value: DateTimeOffset
    }

    [<Test>]
    let ``DateTimeOffset member serialization Newtonsoft format`` () =
        let expected = """{"value":"2017-11-04T22:50:45.1230000-04:00"}"""
        let value = new DateTimeOffset(2017, 11, 4, 22, 50, 45, 123, new TimeSpan(-4, 0, 0))
        let theRecord = { TheDateTimeOffset.value = value }
        let actual = Json.serializeU(theRecord)
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``DateTimeOffset member deserialization without offset`` () =
        let json = """{"value":"2017-11-04T22:50:45"}"""
        let value = new DateTimeOffset(2017, 11, 4, 22, 50, 45, new TimeSpan(0L))
        let expected = { TheDateTimeOffset.value = value }
        let actual = Json.deserialize<TheDateTimeOffset> json
        Assert.AreEqual(expected, actual)

    type DateTimeFormat = {
        [<JsonField(DateTimeFormat="yyyy-MM-ddTHH:mm:ss")>]
        value: DateTime
    }

    [<Test>]
    let ``DateTime member serialization custom format`` () =
        let expected = """{"value":"2017-11-04T22:50:45"}"""
        let value = Json.deserialize<DateTimeFormat> expected
        let actual = Json.serializeU(value)
        Assert.AreEqual(expected, actual)