module UdpServers

    open System.Windows
    open System.Windows.Controls
    open System.Net.Sockets
    open System.Net
    open System.Text
    open System

    type UdpServer_VM(mainWindow:Window ) =
        let _mainWindow = mainWindow
        let _txtArea = _mainWindow.FindName("ConnectionsTextBox") :?> TextBox

        let rec updateTextBox(s:string) = 
            match Application.Current.Dispatcher.CheckAccess() with
            | false -> Application.Current.Dispatcher.Invoke(new All.updateDelegate(updateTextBox),s ) |> ignore
            | _     ->  _txtArea.Text <- _txtArea.Text + s

        do  _mainWindow.Top <- (float)10
            _mainWindow.Left <- (float)10

        member x.serverThread = 
            let mutable receivedBytes = ""B
            let mutable returnData = ""
            let udpClient = new UdpClient(8080)
            while true do
                let remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 8080)
                receivedBytes <- udpClient.Receive(ref remoteIPEndPoint) //Blocking
                returnData <- remoteIPEndPoint.Address.ToString() + ": " +
                    Encoding.ASCII.GetString(receivedBytes) +
                    Environment.NewLine
                updateTextBox returnData |> ignore

