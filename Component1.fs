namespace CommonLib
[<AutoOpen>]
module Core =
    let MAX_PACKET_SIZE = (1024*1024)

        //p.Dispose()
    //From common.h
    type Daemon = 
        |Meta = 0
        |Backend = 1
        |Disk = 2
        |User = 3
        |Page = 4
        |Log = 5
        |Cli = 6
        |Pattern = 7
    type PayloadVerb =
        |Get = 0
        |GotPage = 1
        |GotFile = 2
        |Res404 = 3
    type Packet = 
        {payloadsize : int; payloadverb : PayloadVerb; payload : byte[]}
        member x.ToByteArray() = Array.concat [|System.BitConverter.GetBytes(x.payloadsize);System.BitConverter.GetBytes(x.payloadverb|>int);x.payload|]
    let read (p:System.IO.Pipes.NamedPipeServerStream) = 
        p.WaitForConnection()
        let b = Array.zeroCreate MAX_PACKET_SIZE
        p.Read(b,0,8)|>ignore
        let payloadsize = System.BitConverter.ToInt32(b,0)
        p.Read(b,8,MAX_PACKET_SIZE-8)|>ignore
        {
            payloadsize = payloadsize; 
            payloadverb = System.BitConverter.ToInt32(b,4) |> enum; 
            payload = if payloadsize = 0 then [||] else b.[8..7+payloadsize]
        }
    let write (p:System.IO.Pipes.NamedPipeServerStream) (packet:Packet) = 
        p.WaitForConnection()
        let b = packet.ToByteArray()
        p.Write(b,0,b.Length)|>ignore
    let rec flush (p:System.IO.Pipes.NamedPipeServerStream) = ()
        //try
            //let rec inner i = 
                //Async.RunSynchronously(async{p.ReadByte()},10)|>ignore
                //inner(i+1)
                //flush(p)
            //inner 0
        //with |_ -> ()
        //p.CopyTo(System.IO.Stream.Null)
        //Async.RunSynchronously(async{p.AsyncRead(MAX_PACKET_SIZE)|>Async.Ignore|>Async.RunSynchronously;flush(p)},1000)
