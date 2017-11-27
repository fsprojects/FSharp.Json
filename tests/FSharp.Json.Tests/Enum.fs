namespace FSharp.Json

module Enum =
    open NUnit.Framework
    
    type NumberEnum =
    | One = 1
    | Two = 2
    | Three = 3

    type TheNumberEnum = {
        value: NumberEnum
    }

    [<Test>]
    let ``Enum serialization/deserialization`` () =
        let expected = { TheNumberEnum.value = NumberEnum.Three }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheNumberEnum> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Enum deserialization from string`` () =
        let expected = { TheNumberEnum.value = NumberEnum.One }
        let json = """{"value":"One"}"""
        let actual = Json.deserialize<TheNumberEnum> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Enum serialization to string`` () =
        let value = { TheNumberEnum.value = NumberEnum.Two }
        let actual = Json.serializeU value
        let expected = """{"value":"Two"}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Enum serialization - config setting`` () =
        let value = { TheNumberEnum.value = NumberEnum.Two }
        let config = JsonConfig.create(unformatted = true, enumValue = EnumMode.Value)
        let actual = Json.serializeEx config value
        let expected = """{"value":2}"""
        Assert.AreEqual(expected, actual)

    type LetterEnum =
    | LetterA = 'a'
    | LetterB = 'b'
    | LetterC = 'c'

    type TheAttributedLetterEnum = {
        [<JsonField(EnumValue = EnumMode.Value)>]
        value: LetterEnum
    }

    [<Test>]
    let ``Letter Enum value deserialization`` () =
        let expected = { TheAttributedLetterEnum.value = LetterEnum.LetterB }
        let json = """{"value":"b"}"""
        let actual = Json.deserialize<TheAttributedLetterEnum> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Letter Enum value serialization`` () =
        let value = { TheAttributedLetterEnum.value = LetterEnum.LetterC }
        let actual = Json.serializeU value
        let expected = """{"value":"c"}"""
        Assert.AreEqual(expected, actual)

    type TheAttributedNumberEnum = {
        [<JsonField(EnumValue = EnumMode.Value)>]
        value: NumberEnum
    }

    [<Test>]
    let ``Number Enum value deserialization`` () =
        let expected = { TheAttributedNumberEnum.value = NumberEnum.Two }
        let json = """{"value":2}"""
        let actual = Json.deserialize<TheAttributedNumberEnum> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Number Enum value serialization`` () =
        let value = { TheAttributedNumberEnum.value = NumberEnum.Three }
        let actual = Json.serializeU value
        let expected = """{"value":3}"""
        Assert.AreEqual(expected, actual)
