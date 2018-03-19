namespace FSharp.Json

/// Contains popular implementations of [ITypeTransform]
module Transforms =
    open System

    /// Implementation of [ITypeTransform] converting DateTime into int64 as epoch time.
    type DateTimeEpoch() =
        interface ITypeTransform with
            member x.targetType () = (fun _ -> typeof<int64>) ()
            member x.toTargetType value = (fun (v: obj) -> int64(((v :?> DateTime) - DateTime(1970, 1, 1)).TotalSeconds) :> obj) value
            member x.fromTargetType value = (fun (v: obj) -> DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds(float(v :?> int64))) :> obj) value

    /// Implementation of [ITypeTransform] converting DateTimeOffset into int64 as epoch time.
    type DateTimeOffsetEpoch() =
        interface ITypeTransform with
            member x.targetType () = (fun _ -> typeof<int64>) ()
            member x.toTargetType value = (fun (v: obj) -> int64(((v :?> DateTimeOffset) - DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan(0L))).TotalSeconds) :> obj) value
            member x.fromTargetType value = (fun (v: obj) -> DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan(0L)).Add(TimeSpan.FromSeconds(float(v :?> int64))) :> obj) value

    /// Implementation of [ITypeTransform] converting Uri into string.
    type UriTransform() =
        interface ITypeTransform with
            member x.targetType () = (fun _ -> typeof<string>) ()
            member x.toTargetType value = (fun (v: obj) -> (v:?> System.Uri).ToString() :> obj) value
            member x.fromTargetType value = (fun (v: obj) -> Uri(v :?> string) :> obj) value