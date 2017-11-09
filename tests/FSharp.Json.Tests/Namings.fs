namespace FSharp.Json

module LowerCamelCase =
    open System
    open NUnit.Framework

    [<Test>]
    let ``Single word`` () =
        Assert.AreEqual("property", Json.lowerCamelCase("Property"))

    [<Test>]
    let ``No changes needed`` () =
        Assert.AreEqual("property", Json.lowerCamelCase("property"))

    [<Test>]
    let ``Two words`` () =
        Assert.AreEqual("twoWords", Json.lowerCamelCase("TwoWords"))

    [<Test>]
    let ``Abbreviations`` () =
        Assert.AreEqual("somethingTbd", Json.lowerCamelCase("SomethingTBD"))

module SnakeCase =
    open System
    open NUnit.Framework

    [<Test>]
    let ``Single word`` () =
        Assert.AreEqual("property", Json.snakeCase("Property"))

    [<Test>]
    let ``No changes needed`` () =
        Assert.AreEqual("property", Json.snakeCase("property"))

    [<Test>]
    let ``Two words`` () =
        Assert.AreEqual("two_words", Json.snakeCase("TwoWords"))

    [<Test>]
    let ``Abbreviations`` () =
        Assert.AreEqual("something_tbd", Json.snakeCase("SomethingTBD"))
