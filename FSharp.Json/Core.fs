namespace FSharp.Json

module internal Core =
    open System
    open System.Globalization
    open System.Linq
    open System.Collections
    open FSharp.Json.Internalized.FSharp.Data

    open Reflection

    let findAttributeMember<'T> (memberInfo: MemberInfo): 'T option =
        let attributes = memberInfo.GetCustomAttributes(typeof<'T>, false)
        match attributes.Length with
        | 1 -> (attributes.[0]) :?> 'T |> Some
        | _ -> None

    let findAttributeCase<'T> (caseInfo: UnionCaseInfo): 'T option =
        let attributes = caseInfo.GetCustomAttributes typeof<'T>
        match attributes.Length with
        | 1 -> (attributes.[0]) :?> 'T |> Some
        | _ -> None

    let createTransform (transformType: Type): ITypeTransform =
        let theConstructor = transformType.GetConstructors().[0]
        theConstructor.Invoke([||]) :?> ITypeTransform

    let getJsonFieldProperty: PropertyInfo -> JsonField =
        findAttributeMember<JsonField> >> Option.defaultValue JsonField.Default |> cacheResult

    let getJsonUnion: Type -> JsonUnion =
        findAttributeMember<JsonUnion> >> Option.defaultValue JsonUnion.Default |> cacheResult

    let getJsonFieldUnionCase: UnionCaseInfo -> JsonField =
        findAttributeCase<JsonField> >> Option.defaultValue JsonField.Default |> cacheResult

    let getJsonUnionCase: UnionCaseInfo -> JsonUnionCase =
        findAttributeCase<JsonUnionCase> >> Option.defaultValue JsonUnionCase.Default |> cacheResult
    
    let getTransform: Type -> ITypeTransform = createTransform |> cacheResult

    let getJsonFieldName (config: JsonConfig) (attribute: JsonField) (prop: PropertyInfo) =
        match attribute.Name with
        | null -> config.jsonFieldNaming prop.Name
        | fieldName -> fieldName

    let getJsonUnionCaseName (config: JsonConfig) (jsonUnion: JsonUnion) (jsonUnionCase: JsonUnionCase) (caseInfo: UnionCaseInfo) =
        match jsonUnionCase.Case with
        | null -> 
            match jsonUnion.Mode with
            | UnionMode.CaseKeyAsFieldName -> config.jsonFieldNaming caseInfo.Name
            | _ -> caseInfo.Name
        | value -> value

    let transformToTargetType (t: Type) (value: obj) (transform: Type): (Type*obj) =
        match transform with
        | null -> (t, value)
        | converterType ->
            let transform = getTransform converterType
            let value = transform.toTargetType value
            let valueType = getType value
            (valueType, value)

    let getEnumMode (config: JsonConfig) (jsonField: JsonField) =
        match jsonField.EnumValue with
        | EnumMode.Default ->
            match config.enumValue with
            | EnumMode.Default -> EnumMode.Name
            | m -> m
        | m -> m 

    let failSerialization (message: string) =
        raise (new JsonSerializationError(message))

    let rec serialize (config: JsonConfig) (t: Type) (value: obj): JsonValue =
        let serializeEnum (t: Type) (jsonField: JsonField) (value: obj): JsonValue =
            let baseT = Enum.GetUnderlyingType t
            let enumMode = getEnumMode config jsonField
            match enumMode with
            | EnumMode.Value ->
                match baseT with
                | t when t = typeof<int> ->
                    let enumValue = decimal (value :?> int)
                    JsonValue.Number enumValue
                | t when t = typeof<byte> ->
                    let enumValue = decimal (value :?> byte)
                    JsonValue.Number enumValue
                | t when t = typeof<char> ->
                    let enumValue = $"%c{value :?> char}"
                    JsonValue.String enumValue
            | EnumMode.Name ->
                let strvalue = Enum.GetName(t, value)
                JsonValue.String strvalue
            | mode -> failSerialization $"Failed to serialize enum %s{t.Name}, unsupported enum mode: %A{mode}"

        let getUntypedType (t: Type) (value: obj): Type =
            if t = typeof<obj> then
                if config.allowUntyped then
                    getType value
                else
                    failSerialization "Failed to serialize untyped data, allowUntyped set to false"
            else t

        let serializeNonOption (t: Type) (jsonField: JsonField) (value: obj): JsonValue =
            match jsonField.AsJson with
            | false ->
                let t, value = transformToTargetType t value jsonField.Transform
                let t = getUntypedType t value
                match t with
                | t when t = typeof<unit> ->
                    JsonValue.Null
                | t when t = typeof<uint16> ->
                    JsonValue.Number (decimal (value :?> uint16))
                | t when t = typeof<int16> ->
                    JsonValue.Number (decimal (value :?> int16))
                | t when t = typeof<int> ->
                    JsonValue.Number (decimal (value :?> int))
                | t when t = typeof<uint32> ->
                    JsonValue.Number (decimal (value :?> uint32))
                | t when t = typeof<int64> ->
                    JsonValue.Number (decimal (value :?> int64))
                | t when t = typeof<uint64> ->
                    JsonValue.Number (decimal (value :?> uint64))
                | t when t = typeof<bigint> ->
                    JsonValue.Number (decimal (value :?> bigint))
                | t when t = typeof<single> ->
                    JsonValue.Float (float (value :?> single))
                | t when t = typeof<float> ->
                    JsonValue.Float (value :?> float)
                | t when t = typeof<decimal> ->
                    JsonValue.Number (value :?> decimal)
                | t when t = typeof<byte> ->
                    JsonValue.Number (decimal (value :?> byte))
                | t when t = typeof<sbyte> ->
                    JsonValue.Number (decimal (value :?> sbyte))
                | t when t = typeof<bool> ->
                    JsonValue.Boolean (value :?> bool)
                | t when t = typeof<string> ->
                    JsonValue.String (value :?> string)
                | t when t = typeof<char> ->
                    JsonValue.String (string(value :?> char))
                | t when t = typeof<DateTime> ->
                    JsonValue.String ((value :?> DateTime).ToString(jsonField.DateTimeFormat))
                | t when t = typeof<DateTimeOffset> ->
                    JsonValue.String ((value :?> DateTimeOffset).ToString(jsonField.DateTimeFormat))
                | t when t = typeof<TimeSpan> ->
                    JsonValue.String ((value :?> TimeSpan).ToString())                    
                | t when t = typeof<Guid> ->
                    JsonValue.String ((value :?> Guid).ToString())                                    
                | t when t.IsEnum ->
                    serializeEnum t jsonField value
                | t when isTuple t || isList t || isArray t || isMap t || isRecord t || isUnion t ->
                    serialize config t value
                | _ -> failSerialization $"Unknown type: %s{t.Name}"
            | true ->
                let value = value :?> string
                try
                    JsonValue.Parse value
                with ex ->
                    JsonValue.String value
                        
        let serializeUnwrapOption (t: Type) (jsonField: JsonField) (value: obj): JsonValue option =
            match t with
            |  t when isOption t ->
                let unwrapedValue = unwrapOption t value
                match unwrapedValue with
                | Some value -> Some (serializeNonOption (getOptionType t) jsonField value)
                | None -> 
                    match config.serializeNone with
                    | Null -> Some JsonValue.Null
                    | Omit -> None
            | _ -> Some (serializeNonOption t jsonField value)

        let serializeUnwrapOptionWithNull (t: Type) (jsonField: JsonField) (value: obj): JsonValue =
            match t with
            |  t when isOption t ->
                let unwrapedValue = unwrapOption t value
                match unwrapedValue with
                | Some value -> serializeNonOption (getOptionType t) jsonField value
                | None -> JsonValue.Null
            | _ -> serializeNonOption t jsonField value

        let serializeProperty (therec: obj) (prop: PropertyInfo): (string*JsonValue) option =
            let jsonField = getJsonFieldProperty prop
            let propValue = prop.GetValue(therec, Array.empty)
            let name = getJsonFieldName config jsonField prop
            let jvalue = serializeUnwrapOption prop.PropertyType jsonField propValue
            match jvalue with
            | Some jvalue -> Some (name, jvalue)
            | None -> None

        let serializeEnumerable (values: IEnumerable): JsonValue =
            let items =
                values.Cast<Object>()
                |> Seq.map (fun value -> 
                    serializeUnwrapOption (getType value) JsonField.Default value)
                |> Seq.map (Option.defaultValue JsonValue.Null)
            items |> Array.ofSeq |> JsonValue.Array

        let serializeTupleItems (types: Type seq) (values: IEnumerable): JsonValue =
            let items =
                values.Cast<Object>()
                |> Seq.zip types
                |> Seq.map (fun (t, value) -> 
                    serializeUnwrapOption t JsonField.Default value)
                |> Seq.map (Option.defaultValue JsonValue.Null)
            items |> Array.ofSeq |> JsonValue.Array

        let serializeKvpEnumerable (kvps: IEnumerable): JsonValue =
            let props =
                kvps.Cast<Object>()
                |> Seq.map (fun kvp ->
                    let key = KvpKey kvp :?> string
                    let value = KvpValue kvp
                    let jValue = serializeUnwrapOption (getType value) JsonField.Default value
                    (key, Option.defaultValue JsonValue.Null jValue)
                )
            props|> Array.ofSeq |> JsonValue.Record

        let serializeRecord (t: Type) (therec: obj): JsonValue =
            let props: PropertyInfo array = getRecordFields(t)
            let fields = props |> Array.map (serializeProperty therec) |> Array.choose id
            JsonValue.Record fields

        let serializeUnion (t: Type) (theunion: obj): JsonValue =
            let caseInfo, values = FSharpValue.GetUnionFields(theunion, t)
            let jsonUnionCase = getJsonUnionCase caseInfo
            let jsonUnion = getJsonUnion caseInfo.DeclaringType
            let theCase = getJsonUnionCaseName config jsonUnion jsonUnionCase caseInfo

            match values.Length with
            | 0 -> JsonValue.String theCase
            | _ ->
                let jsonField = getJsonFieldUnionCase caseInfo
                let types = caseInfo.GetFields() |> Array.map (fun p -> p.PropertyType)
                let jValue =
                    match values.Length with
                    | 1 ->
                        let caseValue = values.[0]
                        let caseType = types.[0]
                        serializeUnwrapOptionWithNull caseType jsonField caseValue
                    | _ ->
                        serializeTupleItems types values
                let unionCases = getUnionCases caseInfo.DeclaringType
                match unionCases.Length with
                | 1 -> jValue
                | _ ->
                    match jsonUnion.Mode with
                    | UnionMode.CaseKeyAsFieldName -> JsonValue.Record [| (theCase, jValue) |]
                    | UnionMode.CaseKeyAsFieldValue ->
                        let jKey = (jsonUnion.CaseKeyField, JsonValue.String theCase)
                        let jValue = (jsonUnion.CaseValueField, jValue)
                        JsonValue.Record [| jKey; jValue |]
                    | UnionMode.CaseKeyDiscriminatorField ->
                        match jValue with
                        | JsonValue.Record jRecord ->
                            JsonValue.Record (Array.append [| (jsonUnion.CaseKeyField, JsonValue.String theCase) |] jRecord)
                        | _ -> failSerialization $"Failed to serialize union, union mode %A{jsonUnion.Mode} supports only objects as discriminated field could exist only in object"
                    | mode -> failSerialization $"Failed to serialize union, unsupported union mode: %A{mode}"

        match t with
        | t when isRecord t -> serializeRecord t value
        | t when isMap t -> serializeKvpEnumerable (value :?> IEnumerable)
        | t when isArray t -> serializeEnumerable (value :?> IEnumerable)
        | t when isList t -> serializeEnumerable (value :?> IEnumerable)
        | t when isTuple t -> serializeTupleItems (getTupleElements t) (FSharpValue.GetTupleFields value)
        | t when isUnion t -> serializeUnion t value
        | t ->
            let msg = $"Failed to serialize, must be one of following types: record, map, array, list, tuple, union. Type is: %s{t.Name}."
            failSerialization msg 
    
    let failDeserialization (path: JsonPath) (message: string) =
        let message = $"JSON Path: %s{path.toString()}. %s{message}"
        raise (JsonDeserializationError(path, message))
               
    let getTargetType (t: Type) (jsonField: JsonField): Type =
        match jsonField.Transform with
        | null -> t
        | transformType -> (getTransform transformType).targetType ()

    let transformFromTargetType (transform: Type) (value: obj): obj =
        match transform with
        | null -> value
        | converterType -> (getTransform converterType).fromTargetType value

    let getJsonValueType (jvalue: JsonValue): Type =
        match jvalue with
        | JsonValue.String _ -> typeof<string>
        | JsonValue.Number _ -> typeof<decimal>
        | JsonValue.Float _ -> typeof<float>
        | JsonValue.Record _ -> typeof<Map<string, obj>>
        | JsonValue.Array _ -> getListType typeof<obj>
        | JsonValue.Boolean _ -> typeof<bool>
        | _ -> null
                
    let rec deserialize (config: JsonConfig) (path: JsonPath) (t: Type) (jValue: JsonValue): obj =
        let deserializeEnum (path: JsonPath) (t: Type) (jsonField: JsonField) (jValue: JsonValue): obj =
            let baseT = Enum.GetUnderlyingType t
            let enumMode = getEnumMode config jsonField
            match enumMode with
            | EnumMode.Value ->
                match baseT with
                | baseT when baseT = typeof<int> ->
                    let enumValue = JsonValueHelpers.getInt path jValue
                    Enum.ToObject(t, enumValue)
                | baseT when baseT = typeof<byte> ->
                    let enumValue = JsonValueHelpers.getByte path jValue
                    Enum.ToObject(t, enumValue)
                | baseT when baseT = typeof<char> ->
                    let enumValue = JsonValueHelpers.getChar path jValue
                    Enum.ToObject(t, enumValue)
            | EnumMode.Name ->
                let valueStr = JsonValueHelpers.getString path jValue
                Enum.Parse(t, valueStr)
            | mode -> failDeserialization path <| $"Failed to deserialize enum %s{t.Name}, unsupported enum mode: %A{mode}"

        let getUntypedType (path: JsonPath) (t: Type) (jValue: JsonValue): Type =
            match t with
            | t when t = typeof<obj> ->
                if config.allowUntyped then
                    getJsonValueType jValue
                else
                    failDeserialization path <| sprintf "Failed to deserialize object, allowUntyped set to false"
            | t -> t 

        let deserializeNonOption (path: JsonPath) (t: Type) (jsonField: JsonField) (jValue: JsonValue): obj =
            match jsonField.AsJson with
            | true ->
                match jValue with
                | JsonValue.String strValue -> strValue :> obj
                | _ -> jValue.ToString(JsonSaveOptions.DisableFormatting) :> obj
            | false ->
                let t = getTargetType t jsonField
                let t = getUntypedType path t jValue
                let jValue =
                    match t with
                    | t when t = typeof<int16> ->
                        JsonValueHelpers.getInt16 path jValue :> obj
                    | t when t = typeof<uint16> ->
                        JsonValueHelpers.getUInt16 path jValue :> obj
                    | t when t = typeof<int> ->
                        JsonValueHelpers.getInt path jValue :> obj
                    | t when t = typeof<uint32> ->
                        JsonValueHelpers.getUInt32 path jValue :> obj
                    | t when t = typeof<int64> ->
                        JsonValueHelpers.getInt64 path jValue :> obj
                    | t when t = typeof<uint64> ->
                        JsonValueHelpers.getUInt64 path jValue :> obj
                    | t when t = typeof<bigint> ->
                        JsonValueHelpers.getBigint path jValue :> obj
                    | t when t = typeof<single> ->
                        JsonValueHelpers.getSingle path jValue :> obj
                    | t when t = typeof<float> ->
                        JsonValueHelpers.getFloat path jValue :> obj
                    | t when t = typeof<decimal> ->
                        JsonValueHelpers.getDecimal path jValue :> obj
                    | t when t = typeof<byte> ->
                        JsonValueHelpers.getByte path jValue :> obj
                    | t when t = typeof<sbyte> ->
                        JsonValueHelpers.getSByte path jValue :> obj
                    | t when t = typeof<bool> ->
                        JsonValueHelpers.getBool path jValue :> obj
                    | t when t = typeof<string> ->
                        JsonValueHelpers.getString path jValue :> obj
                    | t when t = typeof<char> ->
                        JsonValueHelpers.getChar path jValue :> obj
                    | t when t = typeof<DateTime> ->
                        JsonValueHelpers.getDateTime CultureInfo.InvariantCulture path jValue :> obj
                    | t when t = typeof<DateTimeOffset> ->
                        JsonValueHelpers.getDateTimeOffset CultureInfo.InvariantCulture path jValue :> obj
                    | t when t = typeof<TimeSpan> ->
                        JsonValueHelpers.getTimeSpan path jValue :> obj                        
                    | t when t = typeof<Guid> ->
                        JsonValueHelpers.getGuid path jValue :> obj                                            
                    | t when t.IsEnum ->
                        deserializeEnum path t jsonField jValue
                    | t when isTuple t || isList t || isArray t || isMap t || isRecord t || isUnion t ->
                        deserialize config path t jValue
                    | _ -> failDeserialization path $"Not supported type: %s{t.Name}"
                transformFromTargetType jsonField.Transform jValue

        let deserializeUnwrapOption (path: JsonPath) (t: Type) (jsonField: JsonField) (jvalue: JsonValue option): obj =
            match t with
            | t when isOption t ->
                match jvalue with
                | Some jvalue ->
                    match jvalue with
                    | JsonValue.Null -> optionNone t
                    | _ -> deserializeNonOption path (getOptionType t) jsonField jvalue |> optionSome t
                | None ->
                    match config.deserializeOption with
                    | RequireNull ->
                        failDeserialization path "Option field is not found while using RequireNull option deserialization"
                    | AllowOmit ->
                        optionNone t
            | _ ->
                match jvalue with
                | Some jvalue ->
                    deserializeNonOption path t jsonField jvalue
                | None ->
                    match jsonField.DefaultValue with
                    | null -> failDeserialization path "Non option field is missing"
                    | defaultValue -> defaultValue

        let deserializeMap (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            let itemValueType = getMapValueType t
            match jvalue with
            | JsonValue.Record fields ->
                fields 
                |> Array.map (fun field ->
                    let itemName = fst field
                    let itemJsonValue = snd field
                    let itemPath = JsonPathItem.Field itemName |> path.createNew
                    let itemValue = deserializeUnwrapOption itemPath itemValueType JsonField.Default (Some itemJsonValue)
                    (itemName, itemValue)
                )
                |> List.ofArray |> CreateMap t
            | _ -> failDeserialization path "Failed to parse map from JSON that is not object." 

        let deserializeArrayItems (path: JsonPath) (t: Type) (jvalues: JsonValue array) =
            jvalues |> Array.mapi (fun index jvalue ->
                let itemPath = JsonPathItem.ArrayItem index |> path.createNew
                deserializeUnwrapOption itemPath t JsonField.Default (Some jvalue)
            )

        let deserializeList (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            match jvalue with
            | JsonValue.Array jvalues ->
                let itemType = getListItemType t
                let arrayValues = deserializeArrayItems path itemType jvalues
                arrayValues |> List.ofSeq |> createList itemType
            | _ -> failDeserialization path "Failed to parse list from JSON that is not array."

        let deserializeArray (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            match jvalue with
            | JsonValue.Array jvalues ->
                let itemType = t.GetElementType()
                let arrayValues = deserializeArrayItems path itemType jvalues
                let arr = Array.CreateInstance(itemType, arrayValues.Length)
                arrayValues |> Array.iteri (fun index value -> arr.SetValue(value, index))
                arr :> obj
            | _ -> failDeserialization path "Failed to parse array from JSON that is not array."

        let deserializeTupleElements (path: JsonPath) (types: Type[]) (jvalue: JsonValue): obj[] =
            match jvalue with
            | JsonValue.Array values ->
                if types.Length <> values.Length then
                    failDeserialization path "Failed to parse tuple. Number of values in JSON list does not match number of elements in tuple."
                let tupleValues = (Array.zip types values) |> Array.mapi (fun index (t, value) ->
                    let itemPath = JsonPathItem.ArrayItem index |> path.createNew
                    deserializeUnwrapOption itemPath t JsonField.Default (Some value)
                )
                tupleValues
            | _ -> failDeserialization path "Failed to parse tuple from JSON that is not array."

        let deserializeTuple (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            let types = getTupleElements t
            let tupleValues = deserializeTupleElements path types jvalue
            FSharpValue.MakeTuple (tupleValues, t)

        let deserializeProperty (path: JsonPath) (fields: (string*JsonValue) array) (prop: PropertyInfo): obj =
            let jsonField = getJsonFieldProperty prop
            let name = getJsonFieldName config jsonField prop
            let field = fields |> Seq.tryFind (fun f -> fst f = name)
            let fieldValue = field |> Option.map snd
            let propPath = JsonPathItem.Field name |> path.createNew
            deserializeUnwrapOption propPath prop.PropertyType jsonField fieldValue

        let deserializeRecord (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            match jvalue with
            | JsonValue.Record fields ->
                let props: PropertyInfo array = getRecordFields t
                let propsValues = props |> Array.map (deserializeProperty path fields)
                FSharpValue.MakeRecord(t, propsValues)
            | _ -> failDeserialization path "Failed to parse record from JSON that is not object."

        let getUnionCaseInfo (path: JsonPath) (t: Type) (jCaseName: string): UnionCaseInfo =
            let jsonUnion = t |> getJsonUnion
            let unionCases = t |> getUnionCases
            let caseInfo = unionCases |> Array.tryFind (fun c -> getJsonUnionCaseName config jsonUnion (getJsonUnionCase c) c = jCaseName)
            match caseInfo with
            | Some caseInfo -> caseInfo
            | None -> failDeserialization path $"Failed to parse union, unable to find union case: %s{jCaseName}."
        
        let mustFindField (path: JsonPath) (fieldName: string) (fields: (string * JsonValue)[]): string * JsonValue =
            let caseKeyField = fields |> Seq.tryFind (fun f -> fst f = fieldName)
            match caseKeyField with
            | Some field -> field
            | None -> failDeserialization path $"Failed to parse union, unable to find field: %s{fieldName}."

        let makeUnion (path: JsonPath) (t: Type) (jCaseName: string) (jCaseValue: JsonValue): obj =
            let caseInfo = jCaseName |> getUnionCaseInfo path t
            let casePath = JsonPathItem.Field jCaseName |> path.createNew
            let props: PropertyInfo array = caseInfo.GetFields()
            let fieldAttr = getJsonFieldUnionCase caseInfo
            let values =
                match props with
                | [| prop |] ->
                    [| deserializeUnwrapOption casePath prop.PropertyType fieldAttr (Some jCaseValue) |]
                | _ ->
                    let propsTypes = props |> Array.map (fun p -> p.PropertyType)
                    deserializeTupleElements casePath propsTypes jCaseValue
            FSharpValue.MakeUnion (caseInfo, values)
        
        let deserializeUnion (path: JsonPath) (t: Type) (jvalue: JsonValue): obj =
            let jsonUnion = t |> getJsonUnion
            let unionCases = t |> getUnionCases
            match unionCases with
            | [| caseInfo |] ->
                let fieldAttr = getJsonFieldUnionCase caseInfo
                let props: PropertyInfo array = caseInfo.GetFields()
                let values =
                    match props with
                    | [| prop |] ->
                        [| deserializeUnwrapOption path prop.PropertyType fieldAttr (Some jvalue) |]
                    | _ ->
                        let propsTypes = props |> Array.map (fun p -> p.PropertyType)
                        deserializeTupleElements path propsTypes jvalue
                FSharpValue.MakeUnion (caseInfo, values)
            | _ ->
                match jvalue with
                | JsonValue.String caseName ->
                    FSharpValue.MakeUnion (caseName |> getUnionCaseInfo path t, null)
                | JsonValue.Record fields ->
                    match jsonUnion.Mode with
                    | UnionMode.CaseKeyDiscriminatorField ->
                        let caseKeyFieldName, caseKeyFieldValue = mustFindField path jsonUnion.CaseKeyField fields
                        let caseNamePath = caseKeyFieldName |> JsonPathItem.Field |> path.createNew
                        let jCaseName = caseKeyFieldValue |> JsonValueHelpers.getString caseNamePath
                        makeUnion path t jCaseName jvalue
                    | UnionMode.CaseKeyAsFieldValue ->
                        let caseKeyFieldName, caseKeyFieldValue = mustFindField path jsonUnion.CaseKeyField fields
                        let _, jCaseValue = mustFindField path jsonUnion.CaseValueField fields
                        let caseNamePath = caseKeyFieldName |> JsonPathItem.Field |> path.createNew                        
                        let jCaseName = caseKeyFieldValue |> JsonValueHelpers.getString caseNamePath
                        makeUnion path t jCaseName jCaseValue
                    | UnionMode.CaseKeyAsFieldName ->
                        match fields with
                        | [| (jCaseName, jCaseValue) |] ->
                            makeUnion path t jCaseName jCaseValue
                        | _ ->
                            failDeserialization path $"Failed to parse union from record with %i{fields.Length} fields, should be 1 field."
                | _ -> failDeserialization path "Failed to parse union from JSON that is not object."

        match t with
        | t when isRecord t -> deserializeRecord path t jValue
        | t when isMap t -> deserializeMap path t jValue
        | t when isArray t -> deserializeArray path t jValue
        | t when isList t -> deserializeList path t jValue
        | t when isTuple t -> deserializeTuple path t jValue
        | t when isUnion t -> deserializeUnion path t jValue
        | _ -> failDeserialization path $"Failed to serialize, must be one of following types: record, map, array, list, tuple, union. Type is: %s{t.Name}."
    