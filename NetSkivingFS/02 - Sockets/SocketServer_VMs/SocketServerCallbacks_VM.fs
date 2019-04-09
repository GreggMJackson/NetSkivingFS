module SocketServerCallbacks_VM

open System
open System.Windows
open System.Windows.Controls
open System.Net.Sockets
open System.Net
open System.Text
open System.Windows.Input

    type SocketServerCallbacks_VM(mainWindow:Window) =

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
        let ipepServer1 = new IPEndPoint(IPHost.AddressList.[0], 80)
        let ipepServer2 = new IPEndPoint(IPHost.AddressList.[0], 8081)

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

        do  _listenerSocket1.Bind(ipepServer1)
            _listenerSocket2.Bind(ipepServer2)

        member x.receiveHandler (asyncResult:IAsyncResult) =
            let mutable bytesReceived = 0
            match (asyncResult.AsyncState :?> List<obj>) with
            | clientSocket :: receiveCallback :: data -> bytesReceived <- (clientSocket :?> Socket).EndReceive(asyncResult)
                                                         match bytesReceived with
                                                         | 0 -> true |> ignore
                                                         | _ -> _updateTextBox (Encoding.UTF8.GetString(data.Head :?> byte [])) false
                                                                let recv = [|new byte()|] 
                                                                (clientSocket :?> Socket).BeginReceive(recv, 0, 1, SocketFlags.None, 
                                                                 receiveCallback :?> AsyncCallback, [ clientSocket; receiveCallback; recv:>obj ]) |> ignore
            | _ -> true |> ignore

        member x.acceptHandler (asyncResult:IAsyncResult) =
            let receiveCallback = new AsyncCallback(x.receiveHandler)
            let clientSocket:Socket = (asyncResult.AsyncState :?> Socket).EndAccept(asyncResult)
            let recv = [|new byte()|]
            clientSocket.BeginReceive(recv, 0, 1, SocketFlags.None, receiveCallback,
                [clientSocket:>obj; receiveCallback:>obj; recv:>obj]) |> ignore

        member x.Listen =
            Async.Start(async{
                try
                    _listenerSocket1.Listen(-1)
                with
                | :? System.Exception as ex -> MessageBox.Show("Listening error: " + ex.Message) |> ignore
                _listenerSocket1.BeginAccept(new AsyncCallback(x.acceptHandler), _listenerSocket1) |> ignore
            })
            Async.Start(async{
                try
                    _listenerSocket2.Listen(-1)
                with
                | :? System.Exception as ex -> MessageBox.Show("Listening error: " + ex.Message) |> ignore
                _listenerSocket2.BeginAccept(new AsyncCallback(x.acceptHandler), _listenerSocket2) |> ignore
            })

        member x.updateTextBox s clear = _updateTextBox s clear
        member x.btnListen= _btnListen
        member x.btnClear = _btnClear

    type btnClearCommand( vm:SocketServerCallbacks_VM ) =
        let canExecuteChanged = new Event<EventHandler,EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged = canExecuteChanged.Publish
            member x.CanExecute(obj) = true
            member x.Execute(obj) = vm.updateTextBox "" true
    
    type btnListenCommand( vm:SocketServerCallbacks_VM )=
        let canExecuteChanged = new Event<EventHandler, EventArgs>()
        interface ICommand with
            [<CLIEvent>]
            member x.CanExecuteChanged = canExecuteChanged.Publish
            member x.CanExecute(obj) = true
            member x.Execute(obj) = vm.Listen        