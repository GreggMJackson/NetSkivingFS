module TcpServer_VM

open System
open System.Windows
open System.Windows.Controls
open System.Net
open System.Net.Sockets
open System.Threading.Tasks
open System.IO
open System.Linq
open System.Text

    type TcpServer_VM(mainWindow:Window, port:int) =

        let _mainWindow = mainWindow
        let _txtConnections = mainWindow.FindName("txtConnections") :?> TextBox
        let _lblIPAddress = mainWindow.FindName("lblIPAddress") :?> Label
        let _port = port
        let IPHost = Dns.GetHostEntry(Dns.GetHostName())

        let rec UpdateConnectionsTextBox (s:string):unit =
            match Application.Current.Dispatcher.CheckAccess() with
            | false -> Application.Current.Dispatcher.Invoke(new All.updateDelegate(UpdateConnectionsTextBox),s) |> ignore
            | _ -> _txtConnections.Text <- _txtConnections.Text + s

        do _lblIPAddress.Content <- "My IP Address: " + IPHost.AddressList.[0].ToString()

        member x.HandlerThread = 
            let mutable handlerSocket = ``02 - Sockets``.Sockets.Last()
            let networkStream = new NetworkStream(handlerSocket)
            let mutable thisRead = 0
            let blockSize = 1024
            let dataByte = [|for i in 1..blockSize do yield new byte()|]
            lock ``02 - Sockets``.monitor_receiveFile ( fun() ->
                //Only one process can access the file at any given time
                let fs = File.OpenWrite(@"C:\Users\Dragon\Coding\C#\receive.txt")
                let rec readFile()() = fun ()_()-> fun _-> 
                                thisRead <- networkStream.Read(dataByte, 0, blockSize)
                                match thisRead with
                                | 0 -> true |>ignore
                                | _ -> fs.Write(dataByte, 0 , thisRead)
                                       readFile |> ignore
                let almostReadFile = readFile()()
                let nearlyReadFile = almostReadFile()()
                let reallyReadFile = nearlyReadFile()()
                reallyReadFile
                fs.Close())
            UpdateConnectionsTextBox("File written." + Environment.NewLine)
            handlerSocket.Send(Encoding.UTF8.GetBytes("<HTML><BODY>I'm the body.<BR><B>I'm bold too.</B>And brave.</BODY></HTML>")) |> ignore
            handlerSocket<-null

        member x.ListenerThread = 
            let tcpListener = new TcpListener(IPAddress.IPv6Any,_port)
            tcpListener.Start()
            let rec listen() =
                match true with
                    | _ ->  let handlerSocket = tcpListener.AcceptSocket()//Block
                            if handlerSocket.Connected then do
                                UpdateConnectionsTextBox(
                                    handlerSocket.RemoteEndPoint.ToString() + " connected." +
                                    Environment.NewLine)
                            lock ``02 - Sockets``.monitor_sockets (
                                fun _ -> (``02 - Sockets``.Sockets.Add(handlerSocket)))
                            // Start a HandlerThread
                            //Async.Start(async{x.HandlerThread})
                            Task.Factory.StartNew( fun _ -> x.HandlerThread ) |> ignore 
                            listen |> ignore //...go bang eventually?
            listen() |> ignore
