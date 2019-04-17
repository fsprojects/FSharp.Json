namespace FSharp.Json

module internal Conversions =
    open System
    open System.Globalization

    let AsDateTimeOffset cultureInfo (text: string) = 
        // Parse ISO 8601 format, fixing time zone if needed 
        let dateTimeStyles = DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind ||| DateTimeStyles.AssumeUniversal 
        match DateTimeOffset.TryParse(text, cultureInfo, dateTimeStyles) with 
        | true, d -> Some d 
        | _ -> None 
