module UdpClients
    open System.Windows
    open System.Windows.Controls
    open System.Net.Sockets
    open System.Text
    open System.Windows.Input
    open System

    type UdpClient_VM(mainWindow:Window) =
        let _mainWindow = mainWindow
        let _txtArea = _mainWindow.FindName("HostTextBox") :?> TextBox
        let _btnSend = _mainWindow.FindName("Button1") :?> Button

        do  _mainWindow.Top <- (float)10
            _mainWindow.Left <- (float)10
        
        member x.btnSend = _btnSend
            
        member x.Send =
            let udpClient = new UdpClient()
            udpClient.Connect(_txtArea.Text,8080)
            let sendBytes = Encoding.ASCII.GetBytes("Hello World!")
            udpClient.Send(sendBytes, sendBytes.Length) |> ignore

    type btnSendCommand( vm:UdpClient_VM ) =
        let whatever = new Event<EventHandler, EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged: IEvent<System.EventHandler,System.EventArgs> =
                whatever.Publish
            member x.Execute(parameter: obj): unit = 
                vm.Send
            member x.CanExecute (paramater:obj) =
                true
            
