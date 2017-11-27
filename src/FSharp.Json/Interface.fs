namespace FSharp.Json

/// Contains main API functions
module Json =
    open System
    open System.Text.RegularExpressions    
    open FSharp.Json.Internalized.FSharp.Data

    let internal toLower (str: string) = str.ToLower()

    let internal firstCharCapital (str: string) = (str.[0].ToString()).ToUpper() + str.Substring(1)

    /// Converts names into lower camel case. Use in [JsonConfig].
    let lowerCamelCase (name: string): string =
        let regex = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
        let words = Regex.Split(name, regex) |> List.ofArray |> List.map toLower
        let first = List.head words
        let tail = List.tail words |> List.map firstCharCapital
        let parts = [first.ToLower()] @ tail
        String.Join("", parts)

    /// Converts names into snake case. Use in [JsonConfig].
    let snakeCase (name: string): string =
        let regex = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
        let words = Regex.Split(name, regex) |> List.ofArray |> List.map toLower
        String.Join("_", words)

    /// Serailizes F# object into JSON. Uses provided [JsonConfig].
    let serializeEx (config: JsonConfig) (theobj: obj): string =
        let json = Core.serialize config (theobj.GetType()) theobj
        let saveOptions =
            match config.unformatted with
            | true -> JsonSaveOptions.DisableFormatting
            | false -> JsonSaveOptions.None
        json.ToString(saveOptions)

    /// Deserailizes JSON into F# type provided as generic parameter. Uses provided [JsonConfig].
    let deserializeEx<'T> (config: JsonConfig) (json: string): 'T =
        let value = JsonValue.Parse(json)
        (Core.deserialize config JsonPath.Root typeof<'T> value) :?> 'T

    /// Serailizes F# object into JSON. Uses default [JsonConfig].
    let serialize (theobj: obj) = serializeEx JsonConfig.Default theobj

    /// Serailizes F# object into unformatted JSON. Uses default [JsonConfig].
    let serializeU (theobj: obj) =
        let config = JsonConfig.create(unformatted = true)
        serializeEx config theobj

    /// Deserailizes JSON into F# type provided as generic parameter. Uses default [JsonConfig].
    let deserialize<'T> (json: string) = deserializeEx<'T> JsonConfig.Default json
