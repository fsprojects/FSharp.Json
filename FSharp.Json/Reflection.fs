namespace FSharp.Json

module internal Reflection =
    open System
    open System.Reflection
    open System.Collections.Concurrent
    open Microsoft.FSharp.Reflection    

    let isOption_ (t: Type): bool =
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    let getOptionType_ (t: Type): Type =
        t.GetGenericArguments().[0]

    let isArray_ (t: Type) =
        t.IsArray

    let isList_ (t: Type) =
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<List<_>>

    let getType (o: obj) =
        match o with
        | null -> typeof<unit>
        | _ -> o.GetType()

    let getListType_ (itemType: Type) =
        typedefof<List<_>>.MakeGenericType([| itemType |])

    let getListItemType_ (t: Type) =
        t.GetGenericArguments().[0]

    let getListConstructor_ (t: Type) =
        t.GetMethod ("Cons")

    let getListEmptyProperty_ (t: Type) =
        t.GetProperty("Empty")

    let isMap_ (t: Type) =
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Map<_,_>>

    let getMapKeyType_ (t: Type) =
        t.GetGenericArguments().[0]

    let getMapValueType_ (t: Type) =
        t.GetGenericArguments().[1]

    let getMapKvpTupleType_ (t: Type) =
        t.GetGenericArguments() |> FSharpType.MakeTupleType

    let cacheResult (theFunction:'P -> 'R) =
        let theFunction = new Func<_, _>(theFunction)
        let cache = new ConcurrentDictionary<'P, 'R>()
        fun parameter -> cache.GetOrAdd(parameter, theFunction)

    let isRecord: Type -> bool = FSharpType.IsRecord |> cacheResult
    let getRecordFields: Type -> PropertyInfo [] = FSharpType.GetRecordFields |> cacheResult

    let isUnion: Type -> bool = FSharpType.IsUnion |> cacheResult
    let getUnionCases: Type -> UnionCaseInfo [] = FSharpType.GetUnionCases |> cacheResult

    let isTuple: Type -> bool = FSharpType.IsTuple |> cacheResult
    let getTupleElements: Type -> Type [] = FSharpType.GetTupleElements |> cacheResult

    let isOption: Type -> bool = isOption_ |> cacheResult
    let getOptionType: Type -> Type = getOptionType_ |> cacheResult

    let isArray: Type -> bool = isArray_ |> cacheResult

    let isList: Type -> bool = isList_ |> cacheResult
    let getListType: Type -> Type = getListType_ |> cacheResult
    let getListItemType: Type -> Type = getListItemType_ |> cacheResult
    let getListConstructor: Type -> MethodInfo = getListConstructor_ |> cacheResult
    let getListEmptyProperty: Type -> PropertyInfo = getListEmptyProperty_ |> cacheResult

    let isMap: Type -> bool = isMap_ |> cacheResult
    let getMapKeyType: Type -> Type = getMapKeyType_ |> cacheResult
    let getMapValueType: Type -> Type = getMapValueType_ |> cacheResult
    let getMapKvpTupleType: Type -> Type = getMapKvpTupleType_ |> cacheResult

    let unwrapOption (t: Type) (value: obj): obj option =
        let _, fields = FSharpValue.GetUnionFields(value, t)
        match fields.Length with
        | 1 -> Some fields.[0]
        | _ -> None

    let optionNone (t: Type): obj =
        let casesInfos = getUnionCases t
        FSharpValue.MakeUnion(casesInfos.[0], Array.empty)        

    let optionSome (t: Type) (value: obj): obj =
        let casesInfos = getUnionCases t
        FSharpValue.MakeUnion(casesInfos.[1], [| value |])

    let createList (itemType: Type) (items: obj list) =
        let listType = getListType itemType
        let theConstructor = getListConstructor listType
        let addItem item list = theConstructor.Invoke (null, [| item; list |])
        let theList = (getListEmptyProperty listType).GetValue(null)
        List.foldBack addItem items theList

    let KvpKey (value: obj): obj =
        let keyProperty = value.GetType().GetProperty("Key")
        keyProperty.GetValue(value, null)

    let KvpValue (value: obj): obj =
        let valueProperty = value.GetType().GetProperty("Value")
        valueProperty.GetValue(value, null)

    let CreateMap (mapType: Type) (items: (string*obj) list) =
        let theConstructor = mapType.GetConstructors().[0]
        let tupleType = getMapKvpTupleType mapType
        let listItems = items |> List.map (fun item -> FSharpValue.MakeTuple([|fst item; snd item|], tupleType))
        let theList = createList tupleType listItems
        theConstructor.Invoke([|theList|])
