module TcpClientServer

    open System.Windows
    open System.Windows.Markup
    open System.Reflection
    open System.Threading.Tasks

    let TcpClientServerApp =

        //SERVER --------------------------------------------
        //ViewStuff -----------------------------------------
        let winServer = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpServerWindow.xaml")) :?> Window
        winServer.Top<-(float)10
        winServer.Left<-(float)10
        winServer.Show()

        //ViewModel stuff -----------------------------------
        let tcpServer_VM = new TcpServer_VM.TcpServer_VM(winServer, 8080)
        Task.Factory.StartNew( fun _ -> tcpServer_VM.ListenerThread ) |> ignore

        //CLIENT---------------------------------------------
        //ViewStuff -----------------------------------------
        let winClient = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpClientWindow.xaml")) :?> Window

        //ViewModel stuff -----------------------------------
        let tcpClient_VM = new TcpClient_VM.TcpClient_VM(winClient, 8080)
        tcpClient_VM.btnBrowse.Command <- new TcpClient_VM.btnBrowseCommand(tcpClient_VM)
        tcpClient_VM.btnSend.Command <- new TcpClient_VM.btnSendCommand(tcpClient_VM)
        winClient.Top <- (float)10
        winClient.Left <- winServer.Left + winServer.Width + (float)10
        winClient.Show()
        winClient
        
