module UdpClientServer

    open System.Reflection
    open System.Windows
    open System.Windows.Markup

    let UdpClientServerApp =
        //SERVER --------------------------------------------
        //ViewStuff -----------------------------------------
        let winServer = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.UdpClientServer.UdpServerWindow.xaml")) :?> Window
        winServer.Show()
        //ViewModel stuff -----------------------------------
        let udpServer_VM = new UdpServers.UdpServer_VM(winServer)
        Async.Start(async{ udpServer_VM.serverThread })
           
        //CLIENT---------------------------------------------
        //ViewStuff -----------------------------------------
        let winClient:Window = XamlReader.Load(
        Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NetSkivingFS._02___Sockets.UdpClientServer.UdpClientWindow.xaml")) :?> Window

        //ViewModel stuff -----------------------------------
        let udpClient_vm = new UdpClients.UdpClient_VM(winClient)
        udpClient_vm.btnSend.Command <- new UdpClients.btnSendCommand(udpClient_vm)
        winClient.Top <- (float)10
        winClient.Left <- winServer.Left + winServer.Width + (float)10
        winClient.Show()
        winClient
        

