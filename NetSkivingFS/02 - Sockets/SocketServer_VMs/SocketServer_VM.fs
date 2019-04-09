module SocketServer_VM

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.Net.Sockets
open System.Net
open System.Text

    type SocketServer_VM(mainWindow:Window) =

        let _mainWindow = mainWindow
        let _txtArea = _mainWindow.FindName("txtArea") :?> TextBox
        let _btnListen = _mainWindow.FindName("btnListen") :?> Button
        let _btnClear = _mainWindow.FindName("btnClear") :?> Button

        let _listenerSocket1 = new Socket( AddressFamily.InterNetworkV6,
                                           SocketType.Stream,
                                           ProtocolType.Tcp)
        let _listenerSocket2 = new Socket( AddressFamily.InterNetworkV6,
                                           SocketType.Stream,
                                           ProtocolType.Tcp)
        let IPHost = Dns.GetHostEntry(Dns.GetHostName())
        let ipepServer1 = new IPEndPoint(IPHost.AddressList.[0], 8080)
        let ipepServer2 = new IPEndPoint(IPHost.AddressList.[0], 8081)

        do  _listenerSocket1.Bind(ipepServer1)
            _listenerSocket2.Bind(ipepServer2)

        let rec _updateTextBox s clear =
            match Application.Current.Dispatcher.CheckAccess() with
            | false -> Application.Current.Dispatcher.Invoke(
                       new All.updateDelegateClearable(_updateTextBox),s, clear) |> ignore
            | _     -> lock ``02 - Sockets``.monitor_sockets (fun _-> (
                            match clear with 
                            | true -> _txtArea.Text <- ""
                            | _    -> true |> ignore
                            _txtArea.Text <- _txtArea.Text + s
                       )) |> ignore

        member x.updateTextBox s clear = _updateTextBox s clear

        member x.Listen =
            Async.Start(async{
            let mutable _bytesReceived1 = 0
            let recv = [|new byte()|]
            _listenerSocket1.Listen(-1)
            let clientSocket = _listenerSocket1.Accept()
            let rec receive() = 
                match clientSocket.Connected with
                | true -> match _bytesReceived1 with
                          | 0 -> true |> ignore
                          | _ -> _bytesReceived1 <- clientSocket.Receive( recv )
                                 _updateTextBox (Encoding.ASCII.GetString recv ) false 
                                 receive |> ignore
                | _ -> false |> ignore
            receive()})
            Async.Start(async{
            let mutable _bytesReceived1 = 0
            let recv = [|new byte()|]
            _listenerSocket2.Listen(-1)
            let clientSocket = _listenerSocket2.Accept()
            let rec receive() = 
                match clientSocket.Connected with
                | true -> match _bytesReceived1 with
                          | 0 -> true |> ignore
                          | _ -> _bytesReceived1 <- clientSocket.Receive( recv )
                                 _updateTextBox (Encoding.ASCII.GetString recv ) false 
                                 receive |> ignore
                | _ -> false |> ignore
            receive()})

    type btnClearCommand( vm:SocketServer_VM ) =
        let canExecuteChanged = new Event<EventHandler,EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged = canExecuteChanged.Publish
            member x.CanExecute(obj) = true
            member x.Execute(obj) = vm.updateTextBox "" true
    
    type btnListenCommand( vm:SocketServer_VM )=
        let canExecuteChanged = new Event<EventHandler, EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged = canExecuteChanged.Publish
            member x.CanExecute(obj) = true
            member x.Execute(obj) = vm.Listen