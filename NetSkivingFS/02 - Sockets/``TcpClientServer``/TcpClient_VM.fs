module TcpClient_VM
    open System
    open System.Windows
    open System.Windows.Controls
    open System.IO
    open System.Net
    open System.Net.Sockets
    open System.Windows.Input

    type TcpClient_VM(mainWindow:Window, port:int) =
        let _mainWindow = mainWindow
        let _port       = port
        let _btnBrowse  = _mainWindow.FindName("btnBrowse") :?> Button
        let _btnSend    = _mainWindow.FindName("btnSend")   :?> Button
        let _txtFile    = _mainWindow.FindName("txtFile")   :?> TextBox
        let _txtServer  = _mainWindow.FindName("txtServer") :?> TextBox

        member x.btnBrowse = _btnBrowse
        member x.btnSend   = _btnSend

        member x.Browse() =
            let ofd = new System.Windows.Forms.OpenFileDialog()
            ofd.ShowDialog() |> ignore
            _txtFile.Text <- ofd.FileName
            let IPHostClient     = Dns.GetHostEntry(Dns.GetHostName())
            let ipepServerClient = new IPEndPoint(IPHostClient.AddressList.[0],8080)
            _txtServer.Text <- ipepServerClient.Address.ToString()

        member x.Send() =
            let fs = File.OpenRead(_txtFile.Text)
            // Allocate memory space for the file
            let fileBuffer = [|for i in 1..(int)fs.Length do yield new byte()|]
            fs.Read(fileBuffer, 0, (int)fs.Length) |> ignore
            // Open a TCP/IP connection and send the data
            let clientSocket  = new TcpClient(_txtServer.Text, _port)//Block
            let networkStream = clientSocket.GetStream()
            networkStream.Write(fileBuffer, 0, fileBuffer.GetLength(0))
            networkStream.Close()

    type btnBrowseCommand( vm:TcpClient_VM ) =
        let canExecuteChanged = new Event<EventHandler, EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged: IEvent<EventHandler, EventArgs> =
                canExecuteChanged.Publish
            member x.Execute(parameter:obj): unit =
                vm.Browse()
            member x.CanExecute(parameter:obj):bool =
                true

    type btnSendCommand( vm:TcpClient_VM ) =
        let canExecuteChanged = new Event<EventHandler, EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged: IEvent<EventHandler, EventArgs> =
                canExecuteChanged.Publish
            member x.CanExecute(parameter:obj):bool =
                true
            member x.Execute(parameter:obj):unit =
                vm.Send()