namespace FSharp.Json

module Record =
    open System
    open NUnit.Framework

    type TheRecord = {
        theString: string
        theDecimal: decimal
        theBool: bool
        theByte: byte
        theSByte: sbyte
        theInt16: int16
        theUInt16: uint16
        theInt: int
        theUInt: uint32
        theInt64: int64
        theUInt64: uint64
        theBigint: bigint
        theSingle: single
        theFloat: float
        theGuid: Guid
        theDateTime: DateTime
        theDateTimeOffset: DateTimeOffset
        theChar: char
    }

    [<Test>]
    let ``Plain fields serialization/deserialization`` () =
        let expected = {
            theString = "The string"
            theDecimal = 123M
            theBool = true
            theByte = 123uy
            theSByte = 123y
            theInt16 = 123s
            theUInt16 = 123us
            theInt = 123
            theUInt = 123u 
            theInt64 = 123L
            theUInt64 = 123UL
            theBigint = 123I
            theSingle = 123.0f
            theFloat = 123.123
            theGuid = Guid.NewGuid()
            theDateTime = DateTime.Now
            theDateTimeOffset = DateTimeOffset.Now
            theChar = 'a'
        }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheRecord> json
        Assert.AreEqual(expected, actual)

    type InnerRecord = {
        value: string
    }

    type OuterRecord = {
        inner: InnerRecord
    }

    [<Test>]
    let ``Record field serialization/deserialization`` () =
        let expected = { OuterRecord.inner = { InnerRecord.value = "The string" } }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<OuterRecord> json
        Assert.AreEqual(expected, actual)

    type AnnotatedRecord = {
        [<JsonField(Name="value")>]
        Value: string
    }

    [<Test>]
    let ``Record custom field name serialization`` () =
        let expected = """{"value":"The string"}"""
        let value = { AnnotatedRecord.Value = "The string" }
        let actual = Json.serializeU value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Record custom field name deserialization`` () =
        let json = """{"value":"The string"}"""
        let expected = { AnnotatedRecord.Value = "The string" }
        let actual = Json.deserialize<AnnotatedRecord> json
        Assert.AreEqual(expected, actual)

    type UpperCaseRecord = {
        SomeValue: string
    }

    [<Test>]
    let ``Record field name serialization with snake case naming`` () =
        let expected = """{"some_value":"The string"}"""
        let value = { UpperCaseRecord.SomeValue = "The string" }
        let config = JsonConfig.create(unformatted = true, jsonFieldNaming = Json.snakeCase)
        let actual = Json.serializeEx config value
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Record field name deserialization with snake case naming`` () =
        let json = """{"some_value":"The string"}"""
        let expected = { UpperCaseRecord.SomeValue = "The string" }
        let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)
        let actual = Json.deserializeEx<UpperCaseRecord> config json
        Assert.AreEqual(expected, actual)
