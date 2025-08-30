namespace FSharp.Json

module internal JsonValueHelpers =
    open FSharp.Json.Internalized.FSharp.Data
    open Conversions

    let raiseWrongType path typeName jValue =
        raise(JsonDeserializationError(path, $"Path: %A{path} Expected type %s{typeName} is incompatible with jValue: %A{jValue}"))

    let getInt16 (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> int16 value
        | JsonValue.Float value -> int16 value
        | _ -> raiseWrongType path "int16" jValue

    let getUInt16 (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> uint16 value
        | JsonValue.Float value -> uint16 value
        | _ -> raiseWrongType path "uint16" jValue
        
    let getInt (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> int value
        | JsonValue.Float value -> int value
        | _ -> raiseWrongType path "int" jValue

    let getUInt32 (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> uint32 value
        | JsonValue.Float value -> uint32 value
        | _ -> raiseWrongType path "uint32" jValue
    
    let getInt64 (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> int64 value
        | JsonValue.Float value -> int64 value
        | _ -> raiseWrongType path "int64" jValue

    let getUInt64 (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> uint64 value
        | JsonValue.Float value -> uint64 value
        | _ -> raiseWrongType path "uint64" jValue

    let getBigint (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> bigint value
        | JsonValue.Float value -> bigint value
        | _ -> raiseWrongType path "bigint" jValue

    let getSingle (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Float value -> single value
        | JsonValue.Number value -> single value
        | _ -> raiseWrongType path "single" jValue
    
    let getFloat (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Float value -> value
        | JsonValue.Number value -> float value
        | _ -> raiseWrongType path "float" jValue

    let getDecimal (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> value
        | JsonValue.Float value -> decimal value
        | _ -> raiseWrongType path "decimal" jValue

    let getByte (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> byte value
        | JsonValue.Float value -> byte value
        | _ -> raiseWrongType path "byte" jValue

    let getSByte (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Number value -> sbyte value
        | JsonValue.Float value -> sbyte value
        | _ -> raiseWrongType path "sbyte" jValue
    
    let getBool (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Boolean value -> value
        | _ -> raiseWrongType path "bool" jValue

    let getString (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value -> value
        | _ -> raiseWrongType path "string" jValue

    let getChar (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value ->
            match value.Length with
            | 1 -> value.Chars(0)
            | _ -> raise(JsonDeserializationError(path, sprintf "Expected string with single character, got jvalue: %s" value))
        | _ -> raiseWrongType path "char" jValue

    let getDateTime cultureInfo (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value -> 
            let jValue = TextConversions.AsDateTime cultureInfo value
            match jValue with
            | Some jValue -> jValue
            | None -> raiseWrongType path "DateTime" jValue
        | _ -> raiseWrongType path "DateTime" jValue

    let getDateTimeOffset cultureInfo (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value -> 
            let jValue = AsDateTimeOffset cultureInfo value
            match jValue with
            | Some jValue -> jValue
            | None -> raiseWrongType path "DateTimeOffset" jValue
        | _ -> raiseWrongType path "DateTimeOffset" jValue

    let getTimeSpan (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value -> 
            let jValue = TextConversions.AsTimeSpan value
            match jValue with
            | ValueSome jValue -> jValue
            | ValueNone -> raiseWrongType path "TimeSpan" jValue
        | _ -> raiseWrongType path "TimeSpan" jValue
    
    let getGuid (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.String value -> 
            let jValue = TextConversions.AsGuid value
            match jValue with
            | ValueSome jValue -> jValue
            | ValueNone -> raiseWrongType path "Guid" jValue
        | _ -> raiseWrongType path "Guid" jValue

    let getArray (path: JsonPath) (jValue: JsonValue) =
        match jValue with
        | JsonValue.Array arr -> arr
        | _ -> raiseWrongType path "array" jValue
