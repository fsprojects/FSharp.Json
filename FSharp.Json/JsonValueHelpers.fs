namespace FSharp.Json

module internal JsonValueHelpers =
    open System
    open FSharp.Json.Internalized.FSharp.Data
    open Conversions

    let raiseWrongType path typeName jvalue =
        raise(JsonDeserializationError(path, sprintf "Expected type %s is incompatible with jvalue: %A" typeName jvalue))

    [<return: Struct>]
    let (|GetInt16|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (int16 value)
        | JsonValue.Float value -> ValueSome (int16 value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetUInt16|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (uint16 value)
        | JsonValue.Float value -> ValueSome (uint16 value)
        | _ -> ValueNone
        
    [<return: Struct>]
    let (|GetInt|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (int value)
        | JsonValue.Float value -> ValueSome (int value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetUInt32|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (uint32 value)
        | JsonValue.Float value -> ValueSome (uint32 value)
        | _ -> ValueNone
    
    [<return: Struct>]
    let (|GetInt64|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (int64 value)
        | JsonValue.Float value -> ValueSome (int64 value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetUInt64|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (uint64 value)
        | JsonValue.Float value -> ValueSome (uint64 value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetBigint|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (bigint value)
        | JsonValue.Float value -> ValueSome (bigint value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetSingle|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Float value -> ValueSome (single value)
        | JsonValue.Number value -> ValueSome (single value)
        | _ -> ValueNone
    
    [<return: Struct>]
    let (|GetFloat|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Float value -> ValueSome value
        | JsonValue.Number value -> ValueSome (float value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetDecimal|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome value
        | JsonValue.Float value -> ValueSome (decimal value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetByte|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (byte value)
        | JsonValue.Float value -> ValueSome (byte value)
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetSByte|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> ValueSome (sbyte value)
        | JsonValue.Float value -> ValueSome (sbyte value)
        | _ -> ValueNone
    
    [<return: Struct>]
    let (|GetBool|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Boolean value -> ValueSome value
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetString|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> ValueSome value
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetChar|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value ->
            match value.Length with
            | 1 -> ValueSome (value.Chars 0)
            | _ -> ValueNone
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetDateTime|_|) cultureInfo (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = TextConversions.AsDateTime cultureInfo value
            jvalue
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetDateTimeOffset|_|) cultureInfo (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = AsDateTimeOffset cultureInfo value
            jvalue
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetGuid|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = TextConversions.AsGuid value
            jvalue
        | _ -> ValueNone

    [<return: Struct>]
    let (|GetArray|_|) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Array arr -> ValueSome arr
        | _ -> ValueNone
