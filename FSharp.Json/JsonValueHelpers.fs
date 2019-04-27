namespace FSharp.Json

module internal JsonValueHelpers =
    open System
    open FSharp.Json.Internalized.FSharp.Data
    open Conversions

    let raiseWrongType path typeName jvalue =
        raise(JsonDeserializationError(path, sprintf "Expected type %s is incompatible with jvalue: %A" typeName jvalue))

    let getInt16 (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> int16 value
        | JsonValue.Float value -> int16 value
        | _ -> raiseWrongType path "int16" jvalue

    let getUInt16 (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> uint16 value
        | JsonValue.Float value -> uint16 value
        | _ -> raiseWrongType path "uint16" jvalue
        
    let getInt (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> int value
        | JsonValue.Float value -> int value
        | _ -> raiseWrongType path "int" jvalue

    let getUInt32 (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> uint32 value
        | JsonValue.Float value -> uint32 value
        | _ -> raiseWrongType path "uint32" jvalue
    
    let getInt64 (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> int64 value
        | JsonValue.Float value -> int64 value
        | _ -> raiseWrongType path "int64" jvalue

    let getUInt64 (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> uint64 value
        | JsonValue.Float value -> uint64 value
        | _ -> raiseWrongType path "uint64" jvalue

    let getBigint (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> bigint value
        | JsonValue.Float value -> bigint value
        | _ -> raiseWrongType path "bigint" jvalue

    let getSingle (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Float value -> single value
        | JsonValue.Number value -> single value
        | _ -> raiseWrongType path "single" jvalue
    
    let getFloat (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Float value -> value
        | JsonValue.Number value -> float value
        | _ -> raiseWrongType path "float" jvalue

    let getDecimal (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> value
        | JsonValue.Float value -> decimal value
        | _ -> raiseWrongType path "decimal" jvalue

    let getByte (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> byte value
        | JsonValue.Float value -> byte value
        | _ -> raiseWrongType path "byte" jvalue

    let getSByte (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Number value -> sbyte value
        | JsonValue.Float value -> sbyte value
        | _ -> raiseWrongType path "sbyte" jvalue
    
    let getBool (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Boolean value -> value
        | _ -> raiseWrongType path "bool" jvalue

    let getString (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> value
        | _ -> raiseWrongType path "string" jvalue

    let getChar (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value ->
            match value.Length with
            | 1 -> value.Chars(0)
            | _ -> raise(JsonDeserializationError(path, sprintf "Expected string with single character, got jvalue: %s" value))
        | _ -> raiseWrongType path "char" jvalue

    let getDateTime cultureInfo (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = TextConversions.AsDateTime cultureInfo value
            match jvalue with
            | Some jvalue -> jvalue
            | None -> raiseWrongType path "DateTime" jvalue
        | _ -> raiseWrongType path "DateTime" jvalue

    let getDateTimeOffset cultureInfo (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = AsDateTimeOffset cultureInfo value
            match jvalue with
            | Some jvalue -> jvalue
            | None -> raiseWrongType path "DateTimeOffset" jvalue
        | _ -> raiseWrongType path "DateTimeOffset" jvalue

    let getGuid (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.String value -> 
            let jvalue = TextConversions.AsGuid value
            match jvalue with
            | Some jvalue -> jvalue
            | None -> raiseWrongType path "Guid" jvalue
        | _ -> raiseWrongType path "Guid" jvalue

    let getArray (path: JsonPath) (jvalue: JsonValue) =
        match jvalue with
        | JsonValue.Array arr -> arr
        | _ -> raiseWrongType path "array" jvalue
