namespace FSharp.Json

module internal Utils =
    let someOrDefault<'T> (defaultValue:'T) (arg:'T option): 'T =
        match arg with
        | Some arg -> arg
        | None -> defaultValue

module internal Conversions =
    open System
    open System.Globalization

    let AsDateTimeOffset cultureInfo (text: string) = 
        // Parse ISO 8601 format, fixing time zone if needed 
        let dateTimeStyles = DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind ||| DateTimeStyles.AssumeUniversal 
        match DateTimeOffset.TryParse(text, cultureInfo, dateTimeStyles) with 
        | true, d -> Some d 
        | _ -> None 
