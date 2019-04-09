module ``01 - IO``

open System.Windows.Forms
open System.Drawing
open System
open System.Text
open System.IO
open System.Threading
open System.Runtime.Serialization
open System.Runtime.Serialization.Formatters.Binary
open System.Data.SQLite
open System.Data

//// Delegates----------------------------------------
//type updateDelegate = delegate of string -> unit

// Classes (ish)------------------------------------
[<Serializable();DataContract>]
type purchaseOrderStates =
    | ISSUED
    | DELIVERED
    | INVOICED
    | PAID
[<Serializable();DataContract>]
type company = 
    {
    [<IgnoreDataMember>]
        mutable Name : string;
    [<IgnoreDataMember>]
        mutable Address: string;
    [<IgnoreDataMember>]
        mutable Phone : string
    }
[<Serializable();DataContract>]
type lineItem =
    {
    [<IgnoreDataMember>]
        mutable Description:string;
    [<IgnoreDataMember>]
        mutable Quantity:int64;
    [<IgnoreDataMember>]
        mutable Cost:decimal;
    }
[<Serializable();DataContract;KnownType(typeof<purchaseOrder>)>]
type purchaseOrder() =
    [<IgnoreDataMember>]
    let mutable _issuanceDate: DateTime = DateTime.Now
    [<IgnoreDataMember>]
    let mutable _deliveryDate: DateTime= new DateTime()
    [<IgnoreDataMember>]
    let mutable _invoiceDate: DateTime= new DateTime()
    [<IgnoreDataMember>]
    let mutable _paymentDate: DateTime= new DateTime()
    [<IgnoreDataMember>]
    let mutable _purchaseOrderStatus: purchaseOrderStates= purchaseOrderStates.ISSUED
    [<IgnoreDataMember>]
    let _items: lineItem[]= [|{Description =""; Quantity = (int64)0 ;Cost =(decimal)0}|]
    [<IgnoreDataMember>]
    let _vendor: company= {Name="";Address="";Phone=""}
    [<IgnoreDataMember>]
    let _buyer: company={Name="";Address="";Phone=""}
    
    [<IgnoreDataMember>]
    member x.issuanceDate         =_issuanceDate
    [<IgnoreDataMember>]
    member x.deliveryDate         =_deliveryDate
    [<IgnoreDataMember>]
    member x.invoiceDate          =_invoiceDate
    [<IgnoreDataMember>]
    member x.paymentDate          =_paymentDate
    [<IgnoreDataMember>]
    member x.purchaseOrderStatus  =_purchaseOrderStatus
    [<IgnoreDataMember>]
    member x.items                =_items
    [<IgnoreDataMember>]
    member x.vendor               =_vendor
    [<IgnoreDataMember>]
    member x.buyer                =_buyer

    member x.recordDelivery() = 
        if x.purchaseOrderStatus = purchaseOrderStates.ISSUED then do
            x.purchaseOrderStatus = purchaseOrderStates.DELIVERED |> ignore
            x.deliveryDate        = DateTime.Now                  |> ignore
        
    member x.recordInvoice() = 
        if x.purchaseOrderStatus = purchaseOrderStates.DELIVERED then do
            x.purchaseOrderStatus = purchaseOrderStates.INVOICED |> ignore
            x.invoiceDate = DateTime.Now                         |> ignore
    
    member x.recordPayment() = 
        if x.purchaseOrderStatus = purchaseOrderStates.INVOICED then do
            x.purchaseOrderStatus = purchaseOrderStates.PAID |> ignore
            x.paymentDate = DateTime.Now                     |> ignore

// Forms--------------------------------------------
type Form1() = 
    inherit Form()
    let button1 = new Button()
    let textBox1 = new TextBox()
    let webBrowser1 = new WebBrowser()

    do  base.Controls.Add(webBrowser1)
        base.Controls.Add(textBox1)
        base.Controls.Add(button1)

        button1.Location <- new Point(23, 67)
        button1.Name <- "button1"
        button1.Size <- new Size(75, 23)
        button1.TabIndex <- 0
        button1.Text <- "button1"
        button1.UseVisualStyleBackColor <- true
        button1.Click.Add( fun _ -> webBrowser1.Navigate(textBox1.Text))
        // 
        textBox1.Location <- new Point(23, 32);
        textBox1.Name <- "textBox1";
        textBox1.Size <- new Size(743, 22);
        textBox1.TabIndex <- 1;
        // 
        webBrowser1.Location <- new Point(119, 67);
        webBrowser1.MinimumSize <- new Size(20, 20);
        webBrowser1.Name <- "webBrowser1";
        webBrowser1.Size <- new Size(647, 354);
        webBrowser1.TabIndex <- 2;

        base.AutoScaleDimensions <- new SizeF((float32)8, (float32)16)
        base.AutoScaleMode <- AutoScaleMode.Font
        base.ClientSize <- new Size(800, 450)
        base.Name <- "Form1"
        base.Text <- "Form1"
type Form2(_btnReadAsync: Button, _btnReadSync:Button, _tbResults:TextBox) =
    inherit Form()

    let mutable fs:FileStream = null
    let mutable fileContents:byte[] = null
    let mutable _ofd:OpenFileDialog = new OpenFileDialog()
    let mutable fileName:string = ""

    do  base.Controls.Add(_btnReadSync)
        base.Controls.Add(_btnReadAsync)
        base.Controls.Add(_tbResults)
        base.ClientSize <- new Size(800, 450);

        _tbResults.Location <- Point(12,12)
        _tbResults.Multiline <- true
        _tbResults.Size <- Size(587, 377)
       
        _btnReadSync.Location <- Point(617,18)
        _btnReadSync.Size<- Size(118,37)
        _btnReadSync.Text<- "Read Sync"
        //btnReadSync.Click.Add(btnReadAsync_Click)
      
        _btnReadAsync.Location<- Point(617, 61)
        _btnReadAsync.Size<- Size(118,37)
        _btnReadAsync.Text<- "Read Async"

        //_btnReadAsync.Click.Add( this.btnReadAsync_Click )
    member this.tbResults = _tbResults
    member this.btnReadSync = _btnReadSync
    member this.btnReadAsync = _btnReadAsync 
    member this.ofd = _ofd
    member this.updateTextBox(s:String) =
        if (this.InvokeRequired) then
            this.Invoke(All.updateDelegate(this.updateTextBox), s) |> ignore
        else
            this.tbResults.Text <- s 
    member this.fs_StateChanged( asyncResult:IAsyncResult ) =
        if (asyncResult.IsCompleted) then
            this.updateTextBox(Encoding.UTF8.GetString(fileContents)) 
        try
            fs.Close() 
        with
            | :? System.Exception as ex -> ()
    member this.btnReadAsync_Click(e: System.EventArgs) =
        this.ofd.ShowDialog()
        let fs = new FileStream( this.ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)
        let callBack = new AsyncCallback( this.fs_StateChanged )
        fileContents <- [|for i in 1..(int)fs.Length do yield new byte()|]
        fs.BeginRead(fileContents, 0, (int)fs.Length, callBack, null)|> ignore
    member this.syncRead() =
        try
            fs <- new FileStream(fileName, FileMode.OpenOrCreate)
            fs.Seek( (int64)0, SeekOrigin.Begin )
            fileContents <- [|for i in 1..(int)fs.Length do yield new byte()|]
            fs.Read(fileContents, 0, (int)fs.Length)
            this.updateTextBox(Encoding.UTF8.GetString(fileContents)) 
            fs.Close() 
        with
            | :? System.Exception as ex -> MessageBox.Show("Bloody error: " + ex.Message) |> ignore
    member this.btnReadSync_Click(e: System.EventArgs) =
        this.ofd.ShowDialog()
        fileName <- this.ofd.FileName
        let thdSyncRead = new Thread(new ThreadStart(this.syncRead))
        thdSyncRead.Start()
type Form3(_btnRead:Button) =
    inherit Form()

    let mutable _ofd:OpenFileDialog = new OpenFileDialog()
    let mutable fs:FileStream = null
    let mutable sr:StreamReader = null

    do  base.Controls.Add(_btnRead)
        _btnRead.Location <- Point(35,22)
        _btnRead.Size <- Size(151,43)
        _btnRead.Text <- "Read"
    member this.btnRead = _btnRead
    member this.ofd = _ofd
    member this.btnRead_Click(e:System.EventArgs) =
        this.ofd.ShowDialog()
        fs <- new FileStream(this.ofd.FileName, FileMode.OpenOrCreate)
        sr <- new StreamReader(fs)
        let mutable lineCount  =1
        while (sr.ReadLine() <> null ) do
            lineCount<-lineCount+1
        fs.Close()
        MessageBox.Show(String.Format("There are {0} lines in {1}", lineCount, this.ofd.FileName))
        |> ignore
type Form4() =
    inherit Form()

    let _btnWrite = new Button()
    let mutable sfd:SaveFileDialog = new SaveFileDialog()
    let mutable fs:FileStream = null
    let mutable bw:BinaryWriter = null
    let myArray = [|1..1000|]

    do  base.Controls.Add(_btnWrite)
        _btnWrite.Location <- Point(15,21)
        _btnWrite.Size <- Size(163, 48)
        _btnWrite.Text <- "Write"
    member this.btnWrite_Click(e:System.EventArgs) = 
        sfd.ShowDialog()
        fs <- new FileStream(sfd.FileName, FileMode.OpenOrCreate)
        bw <- new BinaryWriter(fs)
        for i in 0..myArray.Length-1 do
            printfn "index: %d" i
            bw.Write(myArray.[i])
        bw.Close |> ignore
    member this.btnWrite = _btnWrite
type Form5 () =
    inherit Form()
    //Helpers
    let mutable bf = new BinaryFormatter()
    let mutable ds = null
    let mutable fs = null

    //Controls
    let _btnSerialize = new Button()
    let _btnDeserialize = new Button()
    let gbOptions = new GroupBox()
    let rbBinary = new RadioButton()
    let rbShallow = new RadioButton()

    //Types
    let _vendor = { Name= ""; Address = ""; Phone= "" }
    let _buyer = { Name=""; Address=""; Phone="" }
    let _Goods = {Description ="anti-roadrunner cannon"; Quantity = (int64)1 ;Cost =(decimal)599.99 }
    let mutable _po = new purchaseOrder()
    do  
        //Controls
        base.Controls.Add(_btnSerialize)
        base.Controls.Add(_btnDeserialize)
        base.Controls.Add(gbOptions)
        gbOptions.Controls.Add(rbBinary)
        gbOptions.Controls.Add(rbShallow)
        //rbShallow.Enabled <- false
        _btnSerialize.Location <- Point(12,12)
        _btnSerialize.Size <- Size(137,50)
        _btnSerialize.Text <- "Serialize"
        _btnDeserialize.Location <- Point(12,68)
        _btnDeserialize.Size <- Size(137,50)
        _btnDeserialize.Text <- "Deserialize"
        gbOptions.Location <- Point(384,57)
        gbOptions.Size <- Size(275, 248)
        gbOptions.Text <- "Serialization options"
        rbBinary.Location <- Point(32,40)
        rbBinary.Size <- Size(69,21)
        rbBinary.Text <- "Binary"
        rbShallow.Location <- Point(32,89)
        rbShallow.Size <- Size(77,21)
        rbShallow.Text <- "Shallow"
        //Form stuff
        base.AutoScaleDimensions <-  new SizeF((float32)8, (float32)16) 
        base.AutoScaleMode <- AutoScaleMode.Font
        base.ClientSize <- new Size(800, 450)
        base.Name <- "Form5"
        base.Text <- "Form5"

        //Types

    member x.vendor = _vendor
    member x.buyer = _buyer
    member x.btnSerialize = _btnSerialize
    member x.btnDeserialize = _btnDeserialize
    member x.btnSerialize_Click(e: System.EventArgs) =
        printfn "Button Serialize click"
        Array.set _po.items 0 _Goods
        _po.vendor.Name    <- _vendor.Name 
        _po.vendor.Address <- _vendor.Address 
        _po.vendor.Phone   <- _vendor.Phone
        _po.buyer.Name     <- _buyer.Name
        _po.buyer.Address  <- _buyer.Address
        _po.buyer.Phone    <- _buyer.Phone
        if rbBinary.Checked then do
            bf <- new BinaryFormatter()
            fs <- File.Create(@"C:\Users\Dragon\Coding\po.bin")
            bf.Serialize(fs, _po)
            fs.Close()
            printfn "%s" "whatever"
        else if rbShallow.Checked then do
            ds <- new DataContractSerializer(typeof<DataSet>)
            fs <- File.Create(@"C:\users\dragon\coding\po.xml")
            ds.WriteObject(fs,_po)
            fs.Close()
            printfn "%s" "whatever"
    member x.btnDeserialize_Click(e: System.EventArgs) =
        printfn "Button deserialize click"
        if rbBinary.Checked then do
            bf <- new BinaryFormatter()
            fs <- File.OpenRead(@"C:\Users\Dragon\Coding\po.bin")
            _po <- bf.Deserialize(fs) :?> purchaseOrder
            fs.Close()
            printfn "%A" _po.buyer
        else if rbShallow.Checked then do
            ds <- new DataContractSerializer(typeof<DataSet>)
            fs <- File.OpenRead(@"C:\users\dragon\coding\po.xml")
            _po <- ds.ReadObject(fs) :?> purchaseOrder
            fs.Close()
            printfn "%A" _po.buyer
type Form6() =
    inherit Form()
    let textBox1    = new TextBox()
    let btnQuery    = new Button()
    let webBrowser1 = new WebBrowser()
    do
        base.Controls.Add(textBox1    )
        base.Controls.Add(btnQuery    )
        base.Controls.Add(webBrowser1 )
        textBox1.Location <- new Point(12,12)   
        textBox1.Size     <- new Size(643,22)
        btnQuery.Location <- new Point(661,12)
        btnQuery.Size     <- new Size(127,30)
        btnQuery.Text     <- "Query"
        webBrowser1.Location <- new Point(12,48)
        webBrowser1.Size  <- new Size(772,375)
        base.AutoScaleDimensions <- new SizeF((float32)8, (float32)16)
        base.AutoScaleMode <- AutoScaleMode.Font
        base.ClientSize <- new Size(800, 450)
    member x.Form6_Load(e:System.EventArgs) = webBrowser1.Navigate("about:Blank")
    member x.btnQuery_Click(e: System.EventArgs) =
        try
            let conn=new SQLiteConnection(@"Data Source=C:\Users\Dragon\Coding\purchasing.sqlite")
            //let ds = new DataContractSerialiser(typeof(Dataset))
            let ds = new DataSet()
            conn.Open()
            let cmd = new SQLiteCommand(textBox1.Text, conn)
            let da = new SQLiteDataAdapter(cmd)
            da.Fill(ds)
            let tw = new StreamWriter(@"C:\Users\Dragon\Coding\sql.xml")
            //ds.Serialize(tw,ds)
            tw.Close()
            conn.Close()
            webBrowser1.Navigate(@"C:\users\dragon\coding\sql.xml")
        with
        | :? System.Exception as ex -> MessageBox.Show("An error: " + ex.Message) |> ignore



