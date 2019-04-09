module SocketServerCallbacks

    open System.Windows
    open System.Windows.Markup
    open System.Reflection

    let SocketServerCallbacksApp =
        //SERVER --------------------------------------------
        //ViewStuff -----------------------------------------
        let winServer = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.SocketServer.SocketServerWindow.xaml")) :?> Window
        winServer.Top <- float 10
        winServer.Left <- float 10
        winServer.Show()

        //ViewModel stuff -----------------------------------
        let socketServerCallbacks_VM = new SocketServerCallbacks_VM.SocketServerCallbacks_VM(winServer)
        socketServerCallbacks_VM.btnListen.Command <- SocketServerCallbacks_VM.btnListenCommand(socketServerCallbacks_VM)
        socketServerCallbacks_VM.btnClear.Command <- SocketServerCallbacks_VM.btnClearCommand(socketServerCallbacks_VM)

        //CLIENT---------------------------------------------
        //ViewStuff -----------------------------------------
        let winClient1 = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpClientWindow.xaml")) :?> Window
        winClient1.Top <- float 10
        winClient1.Left <- winServer.Left + winServer.Width + float 10
        winClient1.Show()

        //ViewModel stuff -----------------------------------
        let tcpClient_VM1 = new TcpClient_VM.TcpClient_VM(winClient1, 8080)
        tcpClient_VM1.btnBrowse.Command <- TcpClient_VM.btnBrowseCommand(tcpClient_VM1)
        tcpClient_VM1.btnSend.Command <- TcpClient_VM.btnSendCommand(tcpClient_VM1)

        //ViewStuff -----------------------------------------
        let winClient2 = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.TcpClientServer.TcpClientWindow.xaml")) :?> Window
        winClient2.Top <- winClient1.Top + winClient1.Height + float 10
        winClient2.Left <- winClient1.Left
        winClient2.Show()

        //ViewModel stuff -----------------------------------
        let tcpClient_VM2 = new TcpClient_VM.TcpClient_VM(winClient2, 8081) 
        tcpClient_VM2.btnBrowse.Command <- TcpClient_VM.btnBrowseCommand(tcpClient_VM2)
        tcpClient_VM2.btnSend.Command <- TcpClient_VM.btnSendCommand(tcpClient_VM2)

        winServer 
