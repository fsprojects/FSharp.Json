namespace FSharp.Json.Benchmarks

open System.IO
open System.Threading

open BenchmarkDotNet.Attributes
open FSharp.Json

type ISerializer =
    abstract Name : string
    abstract Serialize : Stream -> 'T -> unit
    abstract Deserialize : Stream -> 'T

type JsonDotNetSerializer () =
    let jdn = Newtonsoft.Json.JsonSerializer.Create()
        
    interface ISerializer with
        member __.Name = "Json.Net"
        member __.Serialize (stream : Stream) (x : 'T) =
            use writer = new StreamWriter(stream)
            jdn.Serialize(writer, x)
            writer.Flush()

        member __.Deserialize (stream : Stream) : 'T =
            use reader = new StreamReader(stream)
            jdn.Deserialize(reader, typeof<'T>) :?> 'T
            
type FSharpJsonSerializer() =
    let serializerConfig : JsonConfig =
        {
            unformatted = true
            serializeNone = SerializeNone.Null
            deserializeOption = DeserializeOption.AllowOmit
            jsonFieldNaming = id
            allowUntyped = true
            enumValue = EnumMode.Name
        }
    
    interface ISerializer with
        member __.Name = "FSharp.Json"
        member __.Serialize (stream : Stream) (x : 'T) =
            let json = FSharp.Json.Json.serializeEx serializerConfig x
            let bytes = System.Text.Encoding.ASCII.GetBytes json
            stream.Write(bytes, 0, bytes.Length)
            
        member __.Deserialize (stream : Stream) : 'T =
            use sr = new StreamReader(stream)
            let json = sr.ReadToEnd()
            FSharp.Json.Json.deserializeEx serializerConfig json
            
module Serializer =

    let memoryStream = 
        new ThreadLocal<_>(fun () -> 
            { new MemoryStream() with
                override __.Close() = () })

    let roundTrip (serializer : ISerializer) (value : 'T) =
        let m = memoryStream.Value
        m.Position <- 0L
        serializer.Serialize m value
        m.Position <- 0L
        let _ = serializer.Deserialize<'T> m
        ()
        
[<AbstractClass>]
[<MarkdownExporter; CsvMeasurementsExporter; HtmlExporter; RPlotExporter>]
type RoundtripBenchmark<'T>(value : 'T) =
    let nsj = new JsonDotNetSerializer()
    let jfs = new FSharpJsonSerializer()
    
    [<Benchmark(Description = "Newtonsoft.Json")>]
    member __.NewtonsoftJson() = Serializer.roundTrip nsj value
    [<Benchmark(Description = "FSharp.Json")>]
    member __.FSharpJson() = Serializer.roundTrip jfs value