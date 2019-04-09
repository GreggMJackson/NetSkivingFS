module Program

open System.Windows.Forms
open System
open System.Windows
open System.Diagnostics

    ////Form1-------------------------------------------------
    //let form1 = new ``01 - IO``.Form1()
    //let form2 = new ``01 - IO``.Form2(new System.Windows.Forms.Button(), 
    //                                new System.Windows.Forms.Button(), 
    //                                new System.Windows.Forms.TextBox())
    //let fs:FileStream = null
    ////Form2-------------------------------------------------
    //form2.btnReadAsync.Click.Add( form2.btnReadAsync_Click ) 
    //form2.btnReadSync.Click.Add( form2.btnReadSync_Click)
    ////Form3-------------------------------------------------
    //let form3 = new ``01 - IO``.Form3(new System.Windows.Forms.Button())
    //form3.btnRead.Click.Add(form3.btnRead_Click)
    ////Form4-------------------------------------------------
    //let form4 = new ``01 - IO``.Form4()
    //form4.btnWrite.Click.Add(form4.btnWrite_Click)
    ////Form5 ------------------------------------------------
    //let form5 = new ``01 - IO``.Form5()
    //form5.vendor.Name    <- "ACME Inc."       
    //form5.vendor.Address <- "Pffff"           
    //form5.vendor.Phone   <- "01234567890"     
    //form5.buyer.Name     <- "Wiley E. Coyote" 
    //form5.buyer.Address  <- "The desert"      
    //form5.buyer.Phone    <- "0987654321"      
    //form5.btnSerialize.Click.Add(form5.btnSerialize_Click) 
    //form5.btnDeserialize.Click.Add(form5.btnDeserialize_Click) 
    ////Form6-------------------------------------------------
    //let form6 = new Form6()
    //form6.Load.Add( form6.Form6_Load);
    //form1.ShowDialog() |> ignore
    //form2.ShowDialog() |> ignore
    //form3.ShowDialog() |> ignore
    //form4.ShowDialog() |> ignore
    //form5.ShowDialog() |> ignore
    //form6.ShowDialog() |> ignore
    //do app.Run window8

////open ``01 - IO``
open ``02 - Sockets``


//open TcpClientServer

[<EntryPoint; STAThread>]
let main argv = 

    //System.Diagnostics.Process.Start(@"C:\Users\Dragon\Coding\Networks\NetStat") |> ignore
    let app = new Application()
    Process.Start(@"C:\Users\Dragon\Coding\Networks\NetStat") |> ignore
    //do app.Run UdpClientServer.UdpClientServerApp |> ignore
    do app.Run TcpClientServer.TcpClientServerApp |> ignore
    //do app.Run SocketServerTcpClients.SocketServerApp |> ignore
    //do app.Run SocketServerCallbacks.SocketServerCallbacksApp |> ignore
    0


