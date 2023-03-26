namespace FSharp.Json.GiraffeSerializer

open FSharp.Control.Tasks
open FSharp.Json
open Giraffe.Serialization.Json
open System.IO
open System.Text
open System.Threading.Tasks

type JsonSerializer () =
    interface IJsonSerializer with
        member this.Deserialize<'T>(encodingToDeserialize: byte []): 'T =
            let toDeserialize = Encoding.UTF8.GetString(encodingToDeserialize)
            (this :> IJsonSerializer).Deserialize<'T>(toDeserialize)
        member this.Deserialize<'T>(toDeserialize: string): 'T =
            Json.deserialize<'T> toDeserialize
        member this.DeserializeAsync(streamToDeserialize) = task {
            use streamReader = new StreamReader(streamToDeserialize, Encoding.UTF8)
            let! toDeserialize = streamReader.ReadToEndAsync()
            return (this :> IJsonSerializer).Deserialize<'T>(toDeserialize)
        }
        member this.SerializeToBytes(toSerialize) =
            Encoding.UTF8.GetBytes((this :> IJsonSerializer).SerializeToString(toSerialize))
        member this.SerializeToStreamAsync(toSerialize) (stream) =
            task {
                use streamWriter = new StreamWriter(stream, Encoding.UTF8)
                let toWrite = (this :> IJsonSerializer).SerializeToString(toSerialize)
                do! streamWriter.WriteAsync(toWrite)
            } :> Task
        
        member this.SerializeToString(toSerialize) =
            Json.serialize(toSerialize)



