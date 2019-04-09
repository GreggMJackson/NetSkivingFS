module ``02 - Sockets``

    open System
    open System.Net.Sockets
    open System.Collections.Generic

    //Monitors------------------------------------------
    let monitor_sockets = new Object()
    let monitor_receiveFile = new Object()

    //Other stuff---------------------------------------
    let Sockets = new List<Socket>()