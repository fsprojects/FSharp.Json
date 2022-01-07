namespace FSharp.Json

module internal Reflection =
    open System
    open System.Reflection
    open System.Collections.Concurrent
    open Microsoft.FSharp.Reflection

    let isOption_ (t: Type) : bool =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<option<_>>

    let getOptionType_ (t: Type) : Type = t.GetGenericArguments().[0]

    let isArray_ (t: Type) = t.IsArray

    let isList_ (t: Type) =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<List<_>>

    let getListType_ (itemType: Type) =
        typedefof<List<_>>.MakeGenericType ([| itemType |])

    let getListItemType_ (t: Type) = t.GetGenericArguments().[0]

    let getListConstructor_ (t: Type) = t.GetMethod("Cons")

    let getListEmptyProperty_ (t: Type) = t.GetProperty("Empty")

    let getIEnumerableType_ (itemType: Type) =
        typedefof<seq<_>>.MakeGenericType ([| itemType |])

    let isSet_ (t: Type) =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<Set<_>>

    let getSetType_ (itemType: Type) =
        typedefof<Set<_>>.MakeGenericType ([| itemType |])

    let getSetItemType_ (t: Type) = t.GetGenericArguments().[0]

    let getSetConstructor_ (setType: Type, enumType: Type) = setType.GetConstructor([| enumType |])
    let getSetAdd_ (t: Type) = t.GetMethod("Add")

    let isResizeArray_ (t: Type) =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<ResizeArray<_>>

    let getResizeArrayType_ (itemType: Type) =
        typedefof<ResizeArray<_>>.MakeGenericType ([| itemType |])

    let getResizeArrayItemType_ (t: Type) = t.GetGenericArguments().[0]

    let getResizeArrayConstructor_ (t: Type) = t.GetConstructor([||])

    let getResizeArrayAdd_ (t: Type) = t.GetMethod("Add")

    let isMap_ (t: Type) =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<Map<_, _>>

    let getMapKeyType_ (t: Type) = t.GetGenericArguments().[0]

    let getMapValueType_ (t: Type) = t.GetGenericArguments().[1]

    let getMapKvpTupleType_ (t: Type) =
        t.GetGenericArguments()
        |> FSharpType.MakeTupleType

    let cacheResult (theFunction: 'P -> 'R) =
        let theFunction = new Func<_, _>(theFunction)
        let cache = new ConcurrentDictionary<'P, 'R>()
        fun parameter -> cache.GetOrAdd(parameter, theFunction)

    let isRecord: Type -> bool = FSharpType.IsRecord |> cacheResult

    let getRecordFields: Type -> PropertyInfo [] =
        FSharpType.GetRecordFields |> cacheResult

    let isUnion: Type -> bool = FSharpType.IsUnion |> cacheResult
    let getUnionCases: Type -> UnionCaseInfo [] = FSharpType.GetUnionCases |> cacheResult

    let isTuple: Type -> bool = FSharpType.IsTuple |> cacheResult

    let getTupleElements: Type -> Type [] =
        FSharpType.GetTupleElements |> cacheResult

    let isOption: Type -> bool = isOption_ |> cacheResult
    let getOptionType: Type -> Type = getOptionType_ |> cacheResult

    let isArray: Type -> bool = isArray_ |> cacheResult

    let isList: Type -> bool = isList_ |> cacheResult
    let getListType: Type -> Type = getListType_ |> cacheResult
    let getListItemType: Type -> Type = getListItemType_ |> cacheResult
    let getListConstructor: Type -> MethodInfo = getListConstructor_ |> cacheResult
    let getListEmptyProperty: Type -> PropertyInfo = getListEmptyProperty_ |> cacheResult

    let getIEnumerableType: Type -> Type = getIEnumerableType_ |> cacheResult

    let isSet: Type -> bool = isSet_ |> cacheResult
    let getSetType: Type -> Type = getSetType_ |> cacheResult
    let getSetItemType: Type -> Type = getSetItemType_ |> cacheResult
    let getSetConstructor: Type * Type -> ConstructorInfo = getSetConstructor_ |> cacheResult
    let getSetAdd: Type -> MethodInfo = getSetAdd_ |> cacheResult

    let isResizeArray: Type -> bool = isResizeArray_ |> cacheResult
    let getResizeArrayType: Type -> Type = getResizeArrayType_ |> cacheResult
    let getResizeArrayItemType: Type -> Type = getResizeArrayItemType_ |> cacheResult
    let getResizeArrayAdd: Type -> MethodInfo = getResizeArrayAdd_ |> cacheResult

    let getResizeArrayConstructor: Type -> ConstructorInfo =
        getResizeArrayConstructor_ |> cacheResult

    let isMap: Type -> bool = isMap_ |> cacheResult
    let getMapKeyType: Type -> Type = getMapKeyType_ |> cacheResult
    let getMapValueType: Type -> Type = getMapValueType_ |> cacheResult
    let getMapKvpTupleType: Type -> Type = getMapKvpTupleType_ |> cacheResult

    let unwrapOption (t: Type) (value: obj) : obj option =
        let _, fields = FSharpValue.GetUnionFields(value, t)

        match fields.Length with
        | 1 -> Some fields.[0]
        | _ -> None

    let optionNone (t: Type) : obj =
        let casesInfos = getUnionCases t
        FSharpValue.MakeUnion(casesInfos.[0], Array.empty)

    let optionSome (t: Type) (value: obj) : obj =
        let casesInfos = getUnionCases t
        FSharpValue.MakeUnion(casesInfos.[1], [| value |])

    let createList (itemType: Type) (items: obj list) =
        let listType = getListType itemType
        let theConstructor = getListConstructor listType

        let addItem item list =
            theConstructor.Invoke(null, [| item; list |])

        let theList =
            (getListEmptyProperty listType).GetValue(null)

        List.foldBack addItem items theList

    let createSet (itemType: Type) (items: obj list) =
        let setType = getSetType itemType
        let enumType = getIEnumerableType itemType
        let setConstructor = getSetConstructor (setType, enumType)

        let listEmpty =
            getListType itemType |> getListEmptyProperty

        let setAdd item set =
            setType.GetMethod("Add").Invoke(set, [| item |])

        let newSet =
            setConstructor.Invoke([| listEmpty.GetValue(null) |])

        List.foldBack setAdd items newSet

    let createResizeArray (itemType: Type) (items: obj list) =
        let resizeArrayType = getResizeArrayType itemType

        let resizeArrayAdd resizeArray item =
            (getResizeArrayAdd resizeArrayType)
                .Invoke(resizeArray, [| item |])
            |> ignore

            resizeArray

        let newResizeArray =
            (getResizeArrayConstructor resizeArrayType)
                .Invoke([||])

        List.fold resizeArrayAdd newResizeArray items

    let KvpKey (value: obj) : obj =
        let keyProperty = value.GetType().GetProperty("Key")
        keyProperty.GetValue(value, null)

    let KvpValue (value: obj) : obj =
        let valueProperty = value.GetType().GetProperty("Value")
        valueProperty.GetValue(value, null)

    let CreateMap (mapType: Type) (items: (string * obj) list) =
        let theConstructor = mapType.GetConstructors().[0]
        let tupleType = getMapKvpTupleType mapType

        let listItems =
            items
            |> List.map (fun item -> FSharpValue.MakeTuple([| fst item; snd item |], tupleType))

        let theList = createList tupleType listItems
        theConstructor.Invoke([| theList |])
