module All

    open System.Windows

    // Delegates----------------------------------------
    type updateDelegate = delegate of string -> unit
    type updateDelegateClearable = delegate of string * bool -> unit
    //Other stuff---------------------------------------
    //let (?)(c:obj)(s:string)=
    //    match c with
    //    | :? FrameworkElement as c -> c.FindName(s) :?> 'T
    //    | _ -> failwith "dynamic lookup failed"

        //let window8 =
    ////winClient----------------------------------------------
    //    let winClient= XamlReader.Load(
    //    Assembly.GetExecutingAssembly()
    //        .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpClientWindow.xaml")) :?> Window
    //    let winServer= XamlReader.Load(
    //    Assembly.GetExecutingAssembly()
    //        .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpServerWindow.xaml")) :?> Window
    //    let txtConnections = winServer.FindName("txtConnections") :?> TextBox

    //    let btnBrowse = winClient.FindName("btnBrowse") :?> Button
    //    let btnSend   = winClient.FindName("btnSend") :?> Button
    //    let mutable txtFile   = winClient.FindName("txtFile") :?> TextBox
    //    let mutable txtServer = winClient.FindName("txtServer") :?> TextBox
    //    //txtServer.Text <- "localhost"
    //    let IPHostClient = Dns.GetHostEntry(Dns.GetHostName())
    //    let ipepServerClient = new IPEndPoint(IPHostClient.AddressList.[0],8080)
    //    txtServer.Text <- ipepServerClient.Address.ToString()
    //    let mutable sockets = new List<Socket>()
    //    let monitor_file = new Object()
    //    let monitor_sockets = new Object()
    //    let btnBrowse_Click():unit =
    //        let mutable ofd = new System.Windows.Forms.OpenFileDialog()
    //        ofd.ShowDialog()
    //        txtFile.Text <- ofd.FileName 
    //    btnBrowse.Click.Add(fun _ -> btnBrowse_Click() |> ignore)
    //    let rec updateConnectionsTextBox(s:string) =
    //        if not (Application.Current.Dispatcher.CheckAccess()) then do
    //            Application.Current.Dispatcher.Invoke(new All.updateDelegate(updateConnectionsTextBox), s) |> ignore
    //        else
    //            txtConnections.Text <- txtConnections.Text  + s
    //    let btnSend_Click() =
    //        let mutable fs = File.OpenRead(txtFile.Text)
    //        // Allocate memory space for the file
    //        let mutable fileBuffer = [|for i in 1..(int)fs.Length do yield new byte()|]
    //        fs.Read(fileBuffer, 0, (int)fs.Length)
    //        // Open a TCP/IP connection and send the data
    //        let clientSocket = new TcpClient(txtServer.Text, 8080)//Block
    //        let networkStream = clientSocket.GetStream()
    //        networkStream.Write(fileBuffer, 0, fileBuffer.GetLength(0))
    //        networkStream.Close() |> ignore
    //    btnSend.Click.Add(fun _ -> btnSend_Click())

    //    //winServer----------------------------------------------
    //    let IPHost = Dns.GetHostEntry(Dns.GetHostName())
    //    let mutable lblIPAddress = winServer.FindName("lblIPAddress") :?> Label
    //    lblIPAddress.Content <- IPHost.AddressList.[0].ToString()

    //    let handlerThread = fun _ -> (
    //        let handlerSocket = sockets.Last()
    //        let networkStream = new NetworkStream(handlerSocket)
    //        let mutable thisRead = 0
    //        let blockSize = 1024
    //        let dataByte = [|for i in 1..blockSize do yield new byte()|]

    //        //Only one process can access the same file at any given time
    //        lock monitor_file (fun () ->
    //            let fs = File.OpenWrite(@"C:\Users\Dragon\Coding\C#\receive.txt")
    //            let rec readFile() = 
    //                thisRead <- networkStream.Read(dataByte, 0, blockSize)
    //                match thisRead with
    //                | 0 -> true |> ignore
    //                | _ -> fs.Write(dataByte, 0, thisRead)
    //                       readFile()
    //            readFile() 
    //            fs.Close()
    //        )
    //        updateConnectionsTextBox("File written" + Environment.NewLine)
    //        handlerSocket=null |> ignore
    //    )
    //    let listenerThread = fun _ ->
    //            let tcpListener = new TcpListener(IPAddress.IPv6Any, 8080)
    //            tcpListener.Start()
    //            while true do
    //                let handlerSocket = tcpListener.AcceptSocket() //Block
    //                if handlerSocket.Connected then do
    //                    updateConnectionsTextBox(
    //                        handlerSocket.RemoteEndPoint.ToString() + " Connected" + Environment.NewLine
    //                    )
    //                lock monitor_sockets (fun () -> sockets.Add(handlerSocket) )
    //                //Start a handlerThread
    //                //1.
    //                Task.Factory.StartNew(fun _ -> (handlerThread())) |> ignore
    //                //2.
    //                //let t1 = System.Threading.Thread( fun() -> handlerThread()) 
    //                //t1.Start()
    //                //3.
    //                //Async.Start( async{ handlerThread() } ) 

    //    //1.
    //    Task.Factory.StartNew(fun _ -> (listenerThread())) |> ignore //good,good,good
    //    //2.
    //    //Async.Start(async{listenerThread()})
                    
    //    winServer.Show()
    //    winClient
