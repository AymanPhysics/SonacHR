Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Controls
Imports System.Diagnostics
Imports System.Management
Imports System.Text
Imports System.Security.Cryptography
Imports System.Net.Mail
Imports System.Net
Imports System.IO.Compression

Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Windows.Controls.Primitives
'Imports System.Runtime.integereropServices
Imports System.Reflection
Imports Microsoft.Office.Interop
Imports System.Threading
Imports System.Security.AccessControl
Imports System.Security.Principal


Public Class BasicMethods
    '''''''''''''''''''''''''''''''''''''''''''''''''''
    Public stat As String = ""
    Dim da As New SqlDataAdapter()

    Public AppendWhere As String = ""
    Public DiscountKeyFiels As Integer

    Public Table_Name As String = ""
    Public Fields() As String = {}
    Public GeneralString As String = ""
    Public KeyFields() As String = {}
    Public control() As Control = {}
    Public ReturnedValues(,) As String = {}
    Public Values() As String = {}
    Private CurrentIDValue() As String = {}
    Public IsLoaded As Boolean = False
    Public SelectedRow As System.Data.DataRowView
    Dim Path As String = System.Windows.Forms.Application.StartupPath & "\Temp\"

    Public Function GetAltScreenshot(Optional C As Grid = Nothing) As Integer
        Try
            If Not C Is Nothing Then
                Dim printscreen As Bitmap = New Bitmap(CType(C.ActualWidth, Integer) - 20, CType(C.ActualHeight, Integer) - 95)
                Dim graphics As Graphics = graphics.FromImage(printscreen)
                graphics.CopyFromScreen(30, 120, 0, 0, New Drawing.Size(printscreen.Size.Width, printscreen.Size.Height))
                IO.Directory.CreateDirectory("C:\Temp")
                printscreen.Save("C:\Temp\printscreen.jpg", ImageFormat.Jpeg)
                Return InsertImage()
            End If
        Catch ex As Exception
            ShowMSG(ex.Message) '"Failed ...!"
            Return 0
        End Try



        'Try
        '    Clipboard.Clear()
        '    Forms.SendKeys.SendWait("%{PRTSC}")
        '    While Not Clipboard.ContainsImage()
        '        Thread.Sleep(500)
        '    End While

        '    'Dim SC As New ScreenShot.ScreenCapture
        '    ''captures entire desktop straight to file
        '    'SC.CaptureScreenToFile("c:\accops\test\desktop2.jpg", Imaging.ImageFormat.Jpeg)


        '    Return Clipboard.GetImage()
        'Catch ex As Exception
        '    ShowMSG("Failed ...!")
        'End Try
    End Function

    Public Sub SetStyle(ByVal x As Button, Optional MyWidth As Double = 180, Optional MyHeight As Double = 24)
        x.Style = Application.Current.FindResource("GlossyCloseButton")
        x.VerticalContentAlignment = Windows.VerticalAlignment.Center
        x.Width = MyWidth
        x.Height = MyHeight
        x.Margin = New Thickness(5, 5, 0, 0)
    End Sub


    Public Sub PrintTbl(ByVal Header As String, ByVal tbl As String, Optional ByVal maintbl As String = "", Optional ByVal mainfield As String = "", Optional Where As String = "")
        Dim frm As New ReportViewer
        frm.Rpt = IIf(maintbl = "", "PrintTbl.rpt", "PrintTbl2.rpt")
        frm.paraname = {"Header", "@tbl", "@Where", "@maintbl", "@mainfield"}
        frm.paravalue = {Header, tbl, Where, maintbl, mainfield}
        frm.Show()
    End Sub

    'SetModem=======================================================================================================================
    Public port As Ports.SerialPort = New Ports.SerialPort("COM3", 9600, Ports.Parity.None, 8, Ports.StopBits.One) With {.BaudRate = 115200, .DtrEnable = True, .RtsEnable = True}
    Dim MyPort As Ports.SerialPort
    Public Sub SetModem()
        'If Not Md.MyProjectType = ProjectType.CabiWithModem Then Return

        Try
            If Not port.IsOpen Then port.Open()
            port.WriteLine("AT#CID=1" + System.Environment.NewLine)
            AddHandler port.DataReceived, AddressOf port_DataReceived
        Catch
        End Try

        Try
            'Dim m As New ManagementObjectSearcher("select * from Win32_SerialPort where Caption='USB Data Fax Voice Modem'")
            Dim m As New ManagementObjectSearcher("select * from Win32_SerialPort where Caption Like'%Modem%'")

            For Each mm As ManagementObject In m.Get()
                Try
                    MyPort = New Ports.SerialPort(mm("DeviceID"), 9600, Ports.Parity.None, 8, Ports.StopBits.One)
                    If Not MyPort.IsOpen Then MyPort.Open()
                    AddHandler MyPort.DataReceived, AddressOf port_DataReceived
                    MyPort.WriteLine("ATZ" + vbCrLf)
                    'MyPort.WriteLine("AT#CID=7" + vbCrLf)
                    'MyPort.WriteLine("AT+VCID=1" + vbCrLf)

                    MyPort.WriteLine("AT#CID=1" + vbCrLf)
                    'MyPort.WriteLine("AT#CID=2" + vbCrLf)
                    MyPort.WriteLine("AT#CC1" + vbCrLf)
                    MyPort.WriteLine("AT+VCID=1" + vbCrLf)
                    MyPort.WriteLine("AT%CCID=1" + vbCrLf)
                    'MyPort.WriteLine("AT%CCID=2" + vbCrLf)
                    MyPort.WriteLine("AT*ID1" + vbCrLf)
                    MyPort.WriteLine("AT#CLS=8#CID=1" + vbCrLf)

                    'MyPort.WriteLine("ATQ0V1E0" + vbCrLf)
                    'MyPort.WriteLine("AT+GMM" + vbCrLf)
                    'MyPort.WriteLine("AT+FCLASS" + vbCrLf)
                    'MyPort.WriteLine("AT#CLS" + vbCrLf)
                    'MyPort.WriteLine("AT+GCI" + vbCrLf)
                    'MyPort.WriteLine("ATI1" + vbCrLf)
                    'MyPort.WriteLine("ATI2" + vbCrLf)
                    'MyPort.WriteLine("ATI3" + vbCrLf)
                    'MyPort.WriteLine("ATI4" + vbCrLf)
                    'MyPort.WriteLine("ATI5" + vbCrLf)
                    'MyPort.WriteLine("ATI6" + vbCrLf)
                    'MyPort.WriteLine("ATI7" + vbCrLf)

                Catch
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

    Sub port_DataReceived(sender As Object, e As Ports.SerialDataReceivedEventArgs)
        Try
            Dim data As String = CType(sender, Ports.SerialPort).ReadExisting().Replace(" ", "")
            If data.Contains("NMBR=") Then
                data = data.Substring(data.IndexOf("NMBR=") + 5).Split("\")(0).Split(vbCrLf)(0)
                Application.Current.Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background,
                New Action(Sub()
                               Dim MyContent As New CallCenter With {.Flag = 1, .MyCallerId = data}
                               Dim frm As New Window With {.Title = data, .Content = MyContent}
                               frm.Show()
                               frm.Activate()
                               frm.WindowState = WindowState.Maximized
                           End Sub))
            End If
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub

    Public Sub SetModemMessage(Mobile As String, MsgContent As String)
        If Mobile.Trim = "" Then Return
        Try
            Dim m As New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity Where Description ='HUAWEI Mobile Connect - 3G PC UI Interface'")
            For Each mm As ManagementObject In m.Get()
                Try
                    Dim MyPortName As String = mm("Caption").ToString.Replace(mm("Description").ToString, "").Replace("(", "").Replace(")", "").Replace(" ", "")
                    MyPort = New Ports.SerialPort(MyPortName, 9600, Ports.Parity.None, 8, Ports.StopBits.One)
                    If Not MyPort.IsOpen Then MyPort.Open()
                    AddHandler MyPort.DataReceived, AddressOf port_DataReceived

                    MyPort.Write("AT" + vbCrLf)
                    MyPort.Write("AT+CMGF=1" + vbCrLf)
                    'MyPort.Write("AT+CSCS=""UCS2""" + vbCrLf)
                    MyPort.Write("AT+CSMP=1,167,0,8" + vbCrLf)
                    MyPort.Write("AT+CMGL=""ALL""" + vbCrLf)
                    MyPort.Write("AT+CMGS=""" & Mobile & """" & vbCrLf)

                    MyPort.DiscardOutBuffer()
                    MyPort.DiscardInBuffer()

                    MyPort.Encoding = Encoding.Unicode  ' Encoding.GetEncoding("iso-8859-15")
                    'MyPort.Encoding = Encoding.ASCII

                    MyPort.Write(StrToHex(MsgContent) & Chr(26))
                    Thread.Sleep(1000)

                    MyPort.Close()
                Catch ex As Exception
                    Dim s As String = ex.Message
                    If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
                    MyPort.Close()
                End Try
            Next
        Catch ex As Exception
            ShowMSG(ex.Message)
        End Try

    End Sub
    Public Function Str2Hex(strMessage As String) As String
        Dim ba As Byte() = Encoding.BigEndianUnicode.GetBytes(strMessage)
        Dim strHex As String = BitConverter.ToString(ba)
        strHex = strHex.Replace("-", "")
        Return strHex
    End Function

    Public Function StrToHex(MyData As String) As String
        Dim Data As String = MyData
        Dim sVal As String
        Dim sHex As String = ""
        While Data.Length > 0
            sVal = Conversion.Hex(Strings.Asc(Data.Substring(0, 1).ToString()))
            Data = Data.Substring(1, Data.Length - 1)
            sHex = sHex & sVal
        End While
        Return "> " & sHex
    End Function


    'End SetModem=======================================================================================================================

    Delegate Sub MyDelegate(Parameters() As Object)
    Public Sub MyInvoke(CurrentDelegate As MyDelegate, Optional Parameters() As Object = Nothing)
        Application.Current.Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, New Action(Sub() CurrentDelegate(Parameters)))
    End Sub


    Public Function TestField(txt As TextBox, lbl As Label) As Boolean
        If Val(txt.Text) = 0 Then
            ShowMSG("برجاء تحديد " & lbl.Content)
            txt.Focus()
            Return False
        End If
        Return True
    End Function

    Public Function TestField(txt As DatePicker, lbl As Label) As Boolean
        If txt.SelectedDate Is Nothing Then
            ShowMSG("برجاء تحديد " & lbl.Content)
            txt.Focus()
            Return False
        End If
        Return True
    End Function

    Public Function TestField(txt As ComboBox, lbl As Label) As Boolean
        If txt.SelectedIndex < 1 Then
            ShowMSG("برجاء تحديد " & lbl.Content)
            txt.Focus()
            Return False
        End If
        Return True
    End Function




    Dim tempclean As Thread
    Dim tempFolderPath As String = System.IO.Path.GetTempPath()

    Public Sub EmptyTemp()
        tempclean = New System.Threading.Thread(AddressOf clean)
        tempclean.Start()
    End Sub
    Sub clean()
        For Each filePath In Directory.GetFiles(tempFolderPath)
            Try
                If Not filePath.Contains(".NETFramework") Then File.Delete(filePath)
            Catch
            End Try
        Next
        For Each filePath In Directory.GetDirectories(tempFolderPath)
            Try
                Directory.Delete(filePath, True)
            Catch
            End Try
        Next
    End Sub


    Public Sub TextToSpeech(str As String)
        Return
        'Try

        '    Dim WMP As WMPLib.WindowsMediaPlayer = CType(Application.Current.MainWindow, MainWindow).WMP

        '    Dim web As New System.Net.WebClient()
        '    web.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible MSIE 9.0 Windows)")
        '    Dim encstr As String
        '    Dim filename As String = GetNewTempName("tts.mp3")
        '    encstr = str.Replace(" ", "%20").Replace("ء", "%D8%A1").Replace("آ", "%D8%A2").Replace("أ", "%D8%A3").Replace("ؤ", "%D8%A4").Replace("إ", "%D8%A5").Replace("ئ", "%D8%A6").Replace("ا", "%D8%A7").Replace("ب", "%D8%A8").Replace("ت", "%D8%AA").Replace("ث", "%D8%AB").Replace("ج", "%D8%AC").Replace("ح", "%D8%AD").Replace("خ", "%D8%AE").Replace("د", "%D8%AF").Replace("ذ", "%D8%B0").Replace("ر", "%D8%B1").Replace("ز", "%D8%B2").Replace("س", "%D8%B3").Replace("ش", "%D8%B4").Replace("ص", "%D8%B5").Replace("ض", "%D8%B6").Replace("ط", "%D8%B7").Replace("ظ", "%D8%B8").Replace("ع", "%D8%B9").Replace("غ", "%D8%BA").Replace("ف", "%D9%81").Replace("ق", "%D9%82").Replace("ك", "%D9%83").Replace("ل", "%D9%84").Replace("م", "%D9%85").Replace("ن", "%D9%86").Replace("ه", "%D9%87").Replace("و", "%D9%88").Replace("ي", "%D9%8A").Replace("ى", "%D9%89") ' "This string cannot be more than 100 characters."
        '    web.DownloadFile("http://translate.google.com/translate_tts?tl=ar&q=" + encstr, filename)
        '    'CType(Application.Current.MainWindow, MainWindow).WMP.URL = filename
        '    Dim m As WMPLib.IWMPMedia = WMP.newMedia(filename)
        '    Try
        '        WMP.currentPlaylist.removeItem(m)
        '    Catch ex As Exception
        '    End Try
        '    WMP.currentPlaylist.appendItem(m)
        '    If Not WMP.status.StartsWith("Playing") Then
        '        WMP.controls.currentItem = m
        '    End If
        '    WMP.controls.play()

        '    'WMP.openPlayer(filename)
        '    'WMP.settings.setMode("loop", False)
        '    'AddHandler WMP.PlayStateChange, AddressOf WMP_PlayStateChange

        '    'Dim a As New Process() 
        '    'a.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        '    'a.StartInfo.UseShellExecute = True
        '    'a.StartInfo.FileName = filename
        '    'a.Start()
        '    'a.WaitForExit()
        '    'System.Threading.Thread.Sleep(WMP.currentMedia.duration)
        'Catch ex As Exception
        'End Try
    End Sub


    Sub AccNoLostFocus(ByVal txtId As TextBox, ByVal txtName As TextBox, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0, Optional HideLinkedAccs As Boolean = False, Optional WithoutId2 As Boolean = True)
        If txtId.Visibility = Visibility.Hidden Then Return
        If txtId.Text.Trim = "" Or txtId.Text.Trim = "0" Or txtId.Visibility = Visibility.Hidden Then
            txtId.Clear()
            txtName.Clear()
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from Chart where " & IIf(WithoutId2, "Id not in(select Id2 from chart) and ", "") & "Id='" & txtId.Text.Trim() & "'")
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            txtId.Clear()
            txtName.Clear()
            Return
        End If

        Dim s As String = IIf(HideLinkedAccs, -1, 0)
        dt = ExecuteAdapter("select Name from Chart where " & IIf(WithoutId2, "Id not in(select Id2 from chart) and ", "") & "Id='" & txtId.Text.Trim() & "' and (SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=" & s & ")")
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Account")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Account")
            End If
            txtId.Clear()
            txtName.Clear()
            Return
        End If

        txtName.Text = dt.Rows(0)(0).ToString
    End Sub

    Public Function AccNoShowHelp(ByVal txtId As TextBox, ByVal txtName As TextBox, ByVal e As System.Windows.Input.KeyEventArgs, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0, Optional HideLinkedAccs As Boolean = False, Optional WithoutId2 As Boolean = True) As Boolean
        Dim s As String = IIf(HideLinkedAccs, -1, 0)
        Return ShowHelp("Accounts", txtId, txtName, e, "select cast(Id as varchar(100)) ID,Name from Chart where " & IIf(WithoutId2, "Id not in(select Id2 from chart) and ", "") & "(SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=" & s & ")")
    End Function


    Sub AccNoLostFocusGrid(ByVal Cell As Forms.DataGridViewCell, ByVal txtName As Label, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0)
        If Val(Cell.Value) = 0 Or Cell.ReadOnly Then
            Cell.Value = ""
            txtName.Content = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from Chart where Id not in(select Id2 from chart) and Id='" & Cell.Value.ToString & "'")
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Cell.Value = ""
            txtName.Content = ""
            Return
        End If

        dt = ExecuteAdapter("select Name from Chart where Id not in(select Id2 from chart) and Id='" & Cell.Value.ToString & "' and (SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=0)")
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Account")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Account")
            End If
            Cell.Value = ""
            txtName.Content = ""
            Return
        End If
        txtName.Content = dt.Rows(0)(0).ToString
    End Sub

    Sub AccNoLostFocusGrid(ByVal Cell As Forms.DataGridViewCell, ByVal CellName As Forms.DataGridViewCell, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0)
        If Val(Cell.Value) = 0 Or Cell.ReadOnly Then
            Cell.Value = ""
            CellName.Value = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from Chart where Id not in(select Id2 from chart) and Id='" & Cell.Value.ToString & "'")
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Cell.Value = ""
            CellName.Value = ""
            Return
        End If

        dt = ExecuteAdapter("select Name from Chart where Id not in(select Id2 from chart) and Id='" & Cell.Value.ToString & "' and (SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=0)")
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Account")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Account")
            End If
            Cell.Value = ""
            CellName.Value = ""
            Return
        End If
        CellName.Value = dt.Rows(0)(0).ToString
    End Sub


    Public Function AccNoShowHelpGrid(ByVal Column1 As Forms.DataGridViewCell, txtName As Label, ByVal e As System.Windows.Forms.KeyEventArgs, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0, Optional HideLinkedAccs As Boolean = False) As Boolean
        Dim s As String = IIf(HideLinkedAccs, -1, 0)
        Return ShowHelpGrid("Accounts", Column1, txtName, e, "select cast(Id as varchar(100)) ID,Name from Chart where Id not in(select Id2 from chart) and (SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=" & s & ")")
    End Function

    Public Function AccNoShowHelpGrid(ByVal Column1 As Forms.DataGridViewCell, Column2 As Forms.DataGridViewCell, ByVal e As System.Windows.Forms.KeyEventArgs, Optional ByVal SubType As Integer = 1, Optional ByVal LinkFile As Integer = 0, Optional HideLinkedAccs As Boolean = False) As Boolean
        Dim s As String = IIf(HideLinkedAccs, -1, 0)
        Return ShowHelpGrid("Accounts", Column1, Column2, e, "select cast(Id as varchar(100)) ID,Name from Chart where Id not in(select Id2 from chart) and (SubType=" & SubType & " or " & SubType & "=-1) and (LinkFile=" & LinkFile & " or " & LinkFile & "=" & s & ")")
    End Function

    Sub CostCenterIdLostFocus(ByVal txtId As TextBox, ByVal txtName As TextBox, Optional ByVal SubType As Integer = 1, Optional ByVal MyTableName As String = "CostCenters")
        If txtId.Text.Trim = "" Or txtId.Text.Trim = "0" Or txtId.Visibility = Visibility.Hidden Then
            txtId.Clear()
            txtName.Clear()
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from " & MyTableName & " where Id=" & txtId.Text.Trim())
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            txtId.Clear()
            txtName.Clear()
            Return
        End If

        dt = ExecuteAdapter("select Name from " & MyTableName & " where Id=" & txtId.Text.Trim() & " and SubType=" & SubType)
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Cost Center")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Cost Center")
            End If
            txtId.Clear()
            txtName.Clear()
            Return
        End If

        txtName.Text = dt.Rows(0)(0).ToString
    End Sub

    Sub AnalysisIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal txtName As Label)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from Analysis where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        txtName.Content = dt.Rows(0)(0).ToString
    End Sub

    Sub AnalysisIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from Analysis where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Column2.Value = dt.Rows(0)(0).ToString
    End Sub

    Sub CostCenterIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal txtName As Label, Optional ByVal SubType As Integer = 1)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from CostCenters where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If

        dt = ExecuteAdapter("select Name from CostCenters where Id=" & Val(Column1.Value) & " and SubType=" & SubType)
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Cost Center")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Cost Center")
            End If
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        txtName.Content = dt.Rows(0)(0).ToString
    End Sub

    Sub CostCenterIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell, Optional ByVal SubType As Integer = 1)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from CostCenters where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If

        dt = ExecuteAdapter("select Name from CostCenters where Id=" & Val(Column1.Value) & " and SubType=" & SubType)
        If dt.Rows.Count = 0 Then
            If SubType = 1 Then
                ShowMSG("Please, Select a Valid Cost Center")
            ElseIf SubType = 0 Then
                ShowMSG("This isn't a General Cost Center")
            End If
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Column2.Value = dt.Rows(0)(0).ToString
    End Sub

    Sub CostCenterSubIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal txtName As Label)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from CostCenterSubs where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        txtName.Content = dt.Rows(0)(0).ToString
    End Sub

    Sub CostCenterSubIdLostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter("select Name from CostCenterSubs where Id=" & Val(Column1.Value))
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Column2.Value = dt.Rows(0)(0).ToString
    End Sub

    Public Function CostCenterIdShowHelp(ByVal txtId As TextBox, ByVal txtName As TextBox, ByVal e As System.Windows.Input.KeyEventArgs, Optional ByVal SubType As Integer = 1, Optional MyTableName As String = "CostCenters") As Boolean
        Return ShowHelp(MyTableName, txtId, txtName, e, "select cast(Id as varchar(100)) ID,Name from " & MyTableName & " where SubType=" & SubType)
    End Function


    Public Function TestIsLoaded(M As Object, Optional Fource As Boolean = False) As Boolean

        If Not M Is Nothing Then
            M.Resources = Md.DictionaryCurrent
            M.FlowDirection = Application.Current.MainWindow.FlowDirection
        End If

        'If Md.MyProject = Client.ClothesRed Then
        '    SetColor(M.Content)
        'End If

        If IsLoaded Then Return True
        If Fource Then IsLoaded = True
        Return False
    End Function

    Public Function ShowHelp(ByVal Header As String, ByVal txtId As TextBox, ByVal txtName As TextBox, ByVal e As System.Windows.Input.KeyEventArgs, ByVal Statement As String, Optional ByVal TableName As String = "", Optional MyFields() As String = Nothing, Optional MyValues() As String = Nothing, Optional ByVal FirstColumn As String = "", Optional ByVal SecondColumn As String = "") As Boolean
        If e Is Nothing OrElse e.Key = System.Windows.Input.Key.F1 Then
            Dim hh As New Help
            SelectedRow = Nothing
            hh.Header = Application.Current.MainWindow.Resources.Item(Header)
            If FirstColumn <> "" Then hh.FirstColumn = FirstColumn
            If SecondColumn <> "" Then hh.SecondColumn = SecondColumn
            If hh.Header = "" Then hh.Header = Header
            hh.MyFields = MyFields
            hh.MyValues = MyValues
            hh.Statement = Statement
            hh.TableName = TableName
            hh.ShowDialog()
            SelectedRow = hh.SelectedRow
            If hh.SelectedId = 0 Then Return False
            txtId.Text = hh.SelectedId
            If Not txtId Is txtName Then txtName.Text = hh.SelectedName
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowHelpMultiColumns(ByVal Header As String, ByVal txtId As TextBox, ByVal txtName As TextBox, ByVal e As System.Windows.Input.KeyEventArgs, ByVal Statement As String) As Boolean
        If e Is Nothing OrElse e.Key = System.Windows.Input.Key.F1 Then
            Dim hh As New HelpMultiColumns
            SelectedRow = Nothing
            hh.Header = Application.Current.MainWindow.Resources.Item(Header)
            If hh.Header = "" Then hh.Header = Header
            hh.Statement = Statement
            hh.ShowActivated = True
            hh.ShowDialog()
            SelectedRow = hh.SelectedRow
            If Val(hh.SelectedId) = 0 Then Return False
            txtId.Text = hh.SelectedId
            If Not txtId Is txtName Then txtName.Text = hh.SelectedName
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowHelpGridMultiColumns(ByVal Header As String, ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell, ByVal e As Forms.KeyEventArgs, ByVal Statement As String) As Boolean
        If e.KeyCode = Forms.Keys.F1 Then
            Dim hh As New HelpMultiColumns
            SelectedRow = Nothing
            hh.Header = Application.Current.MainWindow.Resources.Item(Header)
            If hh.Header = "" Then hh.Header = Header
            hh.Statement = Statement
            hh.ShowActivated = True
            hh.ShowDialog()
            SelectedRow = hh.SelectedRow
            If hh.SelectedId = 0 Then Return False
            Column1.Value = hh.SelectedId
            If Not Column1 Is Column2 Then Column2.Value = hh.SelectedName
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowHelpPermissions(ByVal Header As String, ByVal EmpId As String, ByVal Statement As String, Optional ByVal TableName As String = "", Optional LinkFile As Integer = 0, Optional MyFields() As String = Nothing, Optional MyValues() As String = Nothing, Optional ByVal FirstColumn As String = "", Optional ByVal SecondColumn As String = "", Optional ByVal ThirdColumn As String = "") As Boolean
        Dim hh As New Help
        SelectedRow = Nothing
        hh.Header = Application.Current.MainWindow.Resources.Item(Header)
        If FirstColumn <> "" Then hh.FirstColumn = FirstColumn
        If SecondColumn <> "" Then hh.SecondColumn = SecondColumn
        If ThirdColumn <> "" Then hh.ThirdColumn = ThirdColumn
        If hh.Header = "" Then hh.Header = Header
        hh.IsPermissions = True
        hh.EmpId = EmpId
        hh.LinkFile = LinkFile
        hh.MyFields = MyFields
        hh.MyValues = MyValues
        hh.Statement = Statement
        hh.TableName = TableName
        hh.ShowDialog()
        Return True
    End Function

    Public Function ShowHelp(ByVal Header As String, ByVal cbo As ComboBox, TableName As String) As Boolean
        Dim hh As New Help
        SelectedRow = Nothing
        hh.Header = Application.Current.MainWindow.Resources.Item(Header)
        If hh.Header = "" Then hh.Header = Header
        hh.Statement = "select cast(Id as varchar(100)) Id,Name from " & TableName
        hh.TableName = TableName
        hh.ShowDialog()
        SelectedRow = hh.SelectedRow
        FillCombo(TableName, cbo, "")
        If hh.SelectedId = 0 Then Return False
        cbo.SelectedValue = hh.SelectedId
        Return True
    End Function

    Public Function ShowHelpGrid(ByVal Header As String, ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell, ByVal e As Forms.KeyEventArgs, ByVal Statement As String, Optional ByVal TableName As String = "", Optional ByVal FirstColumn As String = "", Optional ByVal SecondColumn As String = "") As Boolean
        If e.KeyCode = Forms.Keys.F1 Then
            Dim hh As New Help
            hh.Header = Header
            If FirstColumn <> "" Then hh.FirstColumn = FirstColumn
            If SecondColumn <> "" Then hh.SecondColumn = SecondColumn
            hh.Statement = Statement
            hh.TableName = TableName
            hh.ShowDialog()
            If hh.SelectedId = 0 Then Return True
            Column1.Value = hh.SelectedId
            If Not Column1 Is Column2 Then Column2.Value = hh.SelectedName
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowHelpGridItemBal(ByVal Column1 As Forms.DataGridViewCell, ByVal Column2 As Forms.DataGridViewCell, ByVal e As Forms.KeyEventArgs, ByVal Statement As String, Optional ByVal TableName As String = "") As Boolean
        If e.KeyCode = Forms.Keys.F12 Then
            Dim hh As New Help
            hh.Header = "الأرصدة"
            hh.Statement = Statement
            hh.TableName = TableName
            hh.txtID.Visibility = Visibility.Hidden
            hh.txtName.Visibility = Visibility.Hidden
            hh.Show()
            hh.DataGridView1.Columns(0).Visibility = Visibility.Hidden
            hh.DataGridView1.Columns(2).Visibility = Visibility.Hidden
            hh.DataGridView1.Columns(4).Visibility = Visibility.Hidden
            hh.Hide()
            hh.ShowDialog()
            If hh.SelectedId = 0 Then Return True
            'If ShowDeleteMSG("هل تريد عمل طلب تحويل؟") Then
            '    ExecuteNonQuery("" & hh.SelectedRow(0) & hh.SelectedRow(2))
            '    Column1.Value = hh.SelectedId
            '    Column2.Value = hh.SelectedName
            'End If


            Return True
        Else
            Return False
        End If
    End Function

    Public Function ShowHelpGrid(ByVal Header As String, ByVal Column1 As Forms.DataGridViewCell, ByVal txtName As Label, ByVal e As Forms.KeyEventArgs, ByVal Statement As String, Optional ByVal TableName As String = "", Optional ByVal FirstColumn As String = "", Optional ByVal SecondColumn As String = "") As Boolean
        If e.KeyCode = Forms.Keys.F1 Then
            Dim hh As New Help
            If FirstColumn <> "" Then hh.FirstColumn = FirstColumn
            If SecondColumn <> "" Then hh.SecondColumn = SecondColumn
            hh.Header = Header
            hh.Statement = Statement
            hh.TableName = TableName
            hh.ShowDialog()
            If hh.SelectedId = 0 Then Return True
            Column1.Value = hh.SelectedId
            txtName.Content = hh.SelectedName
            Return True
        Else
            Return False
        End If
    End Function



    Sub LostFocus(ByVal txtId As TextBox, ByVal txtName As TextBox, ByVal Statement As String, Optional IgnoreVisibility As Boolean = False, Optional IgnoreShowMSG As Boolean = False)
        If txtId.Text.Trim = "" Or txtId.Text.Trim = "0" Then
            txtId.Clear()
            txtName.Clear()
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter(Statement)
        If dt.Rows.Count = 0 Then
            'If (Not txtId.Visibility = Visibility.Visible Or Not IgnoreVisibility) Then ShowMSG("Invalid Id")
            If (txtId.Visibility = Visibility.Visible Or Not IgnoreVisibility) Then ShowMSG("Invalid Id")
            txtId.Clear()
            txtName.Clear()
            Return
        End If
        txtName.Text = dt.Rows(0)(0).ToString
    End Sub

    Sub LostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, ByVal txtName As Label, ByVal Statement As String, Optional IgnoreVisibility As Boolean = False)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter(Statement)
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            txtName.Content = ""
            Return
        End If
        txtName.Content = dt.Rows(0)(0).ToString
    End Sub

    Sub LostFocusGrid(ByVal Column1 As Forms.DataGridViewCell, Column2 As Forms.DataGridViewCell, ByVal Statement As String, Optional IgnoreVisibility As Boolean = False)
        If Val(Column1.Value) = 0 Or Column1.ReadOnly Then
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Dim dt As DataTable = ExecuteAdapter(Statement)
        If dt.Rows.Count = 0 Then
            ShowMSG("Invalid Id")
            Column1.Value = ""
            Column2.Value = ""
            Return
        End If
        Column2.Value = dt.Rows(0)(0).ToString
    End Sub

    Public Sub ApplyOpenCombobox(ByVal cc As ComboBox())
        For Each c As ComboBox In cc
            'AddHandler c.KeyDown, AddressOf MyOpenCombobox
            AddHandler c.KeyUp, AddressOf MyFilterCombobox
            AddHandler c.GotFocus, AddressOf MyGotFocus
        Next
    End Sub

    Public Sub MyKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        If e.Key = System.Windows.Input.Key.Enter Then
            e.Handled = True
            'CType(sender, Control).MoveFocus(New System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next))

            InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) With {.RoutedEvent = Keyboard.KeyDownEvent})

        End If
    End Sub


    Private Sub MyGetFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            If sender.GetType().ToString() = "System.Windows.Controls.PasswordBox" Then
                CType(sender, PasswordBox).SelectAll()
            ElseIf sender.GetType().ToString() = "System.Windows.Controls.TextBox" Then
                CType(sender, TextBox).SelectAll()
            End If
        Catch ex As Exception
        End Try
    End Sub


    Public Sub MyKeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs, Optional ByVal IsDecimal As Boolean = False)
        If e.Key = System.Windows.Input.Key.Enter Or e.Key = System.Windows.Input.Key.Tab Or e.Key = System.Windows.Input.Key.F5 Then Return
        If IsDecimal Then
            Dim s As Controls.TextBox = sender
            Dim ddd = Chr(e.Key)
            If (e.Key = Windows.Input.Key.OemPeriod Or e.Key = Windows.Input.Key.Decimal) AndAlso Not s.Text.Contains(".") Then
                Return
            End If
            If (e.Key = Windows.Input.Key.OemMinus Or e.Key = Windows.Input.Key.Subtract) Then
                s.Text = -Val(s.Text)
                e.Handled = True
                Return
            End If
        End If
        If Not e.Key = Key.F1 AndAlso Not IsNumeric(e.Key.ToString.Replace("NumPad", "").Replace("D", "")) AndAlso Not e.Key = Key.Space Then
            e.Handled = True
            'CType(sender, TextBox).Undo()
        End If
    End Sub



    Public Structure SYSTEMTIME
        Public wYear As UInt16
        Public wMonth As UInt16
        Public wDayOfWeek As UInt16
        Public wDay As UInt16
        Public wHour As UInt16
        Public wMinute As UInt16
        Public wSecond As UInt16
        Public wMilliseconds As UInt16
    End Structure

    Public Sub SetTime()
        Try
            Dim dd As DateTime = CType(ExecuteScalar("select getdate()"), DateTime)
            Today = dd
            TimeOfDay = dd
        Catch ex As Exception
            ShowMSG("Please, Run As Administrator" & vbCrLf & "to enable Time Updating")
        End Try
    End Sub

    Public Function MyGetDate() As DateTime
        Try
            Return CType(ExecuteScalar("select dbo.MyGetDate()"), DateTime)
        Catch
            Return Now
        End Try
    End Function

    Public Function MyGetTime() As String
        Return CType(ExecuteScalar("select GetDate()"), DateTime).ToLongTimeString
    End Function


    Public Function Physics_processorId() As String
        Dim s As String = ""
        Dim search As New ManagementObjectSearcher(New SelectQuery("Win32_processor"))
        For Each info As ManagementObject In search.Get()
            Try
                s &= info("processorId").ToString()
            Catch ex As Exception
            End Try
        Next
        Return s.ToUpper
    End Function

    Public Function EnName(ByVal ArName As String, ByVal dtName As DataTable) As String
        ArName = ArName.Trim
        While ArName.Contains("  ")
            ArName = ArName.Replace("  ", " ")
        End While
        Dim s() As String
        s = ArName.Split(" ")
        Dim En As String = ""
        For i As Integer = 0 To s.Length - 1

            'Dim a As String = ExecuteScalar("select top 1 English_Name from Names where Arabic_Name='" & s(i) & "'")
            Dim a As String = ""
            Try
                a = (dtName.Select("Arabic_Name='" & s(i) & "'")(0))(1)
            Catch ex As Exception
                a = s(i)
            End Try
            If a = "" Then
                En &= s(i)
            Else
                En &= a
            End If
            En &= " "
        Next
        Return En.Trim
    End Function


    Public Function Physics_SerialNumber() As String
        Dim s As String = ""
        Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia")
        For Each wmi_HD As ManagementObject In searcher.Get()
            If wmi_HD("SerialNumber") <> Nothing Then
                s &= wmi_HD("SerialNumber").ToString()
                Exit For
            End If
        Next
        Return s.ToUpper
    End Function


    Public Function Physics_BaseBoard() As String
        Dim s As String = ""
        Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard")
        For Each wmi_HD As ManagementObject In searcher.Get()
            'If wmi_HD("SerialNumber") <> Nothing Then
            '    s &= wmi_HD.Properties("SerialNumber").ToString()
            'End If
            If wmi_HD("Product") <> Nothing Then
                s &= wmi_HD.Properties("Product").Value.ToString().Trim()
            End If
        Next

        searcher.Dispose()
        s = s.Split(".")(s.Split(".").Length - 1)
        Return s.ToUpper
    End Function

    Public Function Physics_VolumeSerialNumber(ByVal Volume As String) As String
        Dim s As String = ""
        Dim disk As ManagementObject = New ManagementObject(String.Format("win32_logicaldisk.deviceid=""{0}:""", Volume))
        Try
            disk.Get()
        Catch ex As Exception
        End Try
        Try
            s &= disk("VolumeSerialNumber").ToString()
        Catch ex As Exception
        End Try
        Return s
    End Function

    Public Function Physics_MACAddress() As String
        Dim s As String = ""
        Dim mo2 As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_NetworkAdapterConfiguration")
        For Each mac As ManagementObject In mo2.Get()
            Try
                s &= mac("MACAddress").ToString.Replace(":", "")
            Catch ex As Exception
            End Try
            If s.Length > 0 Then Exit For
        Next
        Return s
    End Function

    Public Function Physics_MacAddress2() As String
        Dim s As String = ""
        Dim mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
        Dim moc As ManagementObjectCollection = mc.GetInstances()
        Dim MACAddress As String = ""
        For Each mo As ManagementObject In moc
            If (MACAddress.Equals(String.Empty)) Then
                If CBool(mo("IPEnabled")) Then MACAddress = mo("MacAddress").ToString()
                mo.Dispose()
            End If
            Try
                s &= MACAddress.Replace(":", String.Empty)
            Catch ex As Exception
            End Try
            If s.Length > 0 Then Exit For
        Next
        Return s
    End Function

    Public Function Protect(ByVal s As String) As String
        Return s.Replace(" ", "").Replace("0", "P").Replace("1", "K").Replace("2", "U").Replace("3", "G").Replace("4", "R").Replace("5", "D").Replace("6", "W").Replace("7", "Q").Replace("8", "A").Replace("9", "Z").ToUpper()
    End Function

    Public Function ProtectionSerial() As String
        Return (Physics_BaseBoard() & Physics_processorId()).Trim() '& Physics_MACAddress() & Physics_SerialNumber()
    End Function
    Public Sub TestProtection()
        Dim frm As New Form1
        frm.BackgroundWorker1.RunWorkerAsync()
    End Sub

    Public Sub TestDemo()
        If Val(ExecuteScalar("select (select count(*) from Attendance)+(select count(*) from BankCash)+(select count(*) from Cases)+(select count(*) from CustomerInvoices)+(select count(*) from Employees)+(select count(*) from Invoices)+(select count(*) from Entry)+(select count(*) from SalaryHistory)+(select count(*) from SalesMaster)+(select count(*) from Services)+(select count(*) from Reservations)+(select count(*) from Buildings)")) > 100 Then
            ShowMSG("عفوا .. انتهت فترة الاستخدام المجانى")
            Application.Current.Shutdown()
        End If
    End Sub


    Enum CloseState
        Yes
        No
        Cancel
    End Enum

    Public Function RequestDelete() As CloseState
        If Md.FourceExit Then Return CloseState.No
        Dim frm As New CloseForm
        frm.ShowDialog()
        Return frm.State
    End Function


    Public Function ShowForm(ByVal parent As Window, ByVal frm As Window, ByVal Caption As String, ByVal p As Point)
        Return frm

    End Function

    Public Sub CloseTab(ByVal TabName)
        Dim MW As MainWindow = Application.Current.MainWindow
        Dim TI As TabItem
        For I As Integer = 0 To MW.TabControl1.Items.Count - 1
            TI = MW.TabControl1.Items(I)
            If TI.Name = TabName Then
                MW.TabControl1.Items.RemoveAt(I)
                Exit Sub
            End If
        Next
    End Sub

    Sub HandleKeyDown(ByVal frm As Window)
        'For Each C As Control In frm.Controls
        '    If C.GetType Is GetType(TextBox) Then
        '        Dim cc As TextBox = C
        '        If cc.Multiline Then
        '            Continue For
        '        End If
        '    End If
        '    AddHandler C.KeyPress, AddressOf _KeyPress
        'Next
    End Sub


    Public Sub _KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If Asc(e.KeyChar) = 13 Then e.Handled = True
    End Sub
    Public Sub DefineValues()
        ReDim Values(control.Length)
        Dim type As String = "", nn As String = ""
        For i As Integer = 0 To control.Length - 1
            'val = CType(control(i), Control).Text.Trim()
            type = control(i).GetType().ToString().Trim
            nn = control(i).Name
            If (type = "System.Windows.Controls.ComboBox") Then
                Dim cbo As New ComboBox
                cbo = control(i)
                If cbo.SelectedValuePath = "" Then
                    Values(i) = cbo.SelectedIndex.ToString().Trim
                Else
                    If IsNothing(cbo.SelectedValue) Then Values(i) = 0 Else Values(i) = cbo.SelectedValue.ToString().Trim
                End If
            ElseIf (type = "System.Windows.Controls.CheckBox") Then
                Dim chk As New CheckBox()
                chk = control(i)
                If (chk.IsChecked) Then
                    Values(i) = "1"
                Else
                    Values(i) = "0"
                End If

            ElseIf (type = "System.Windows.Controls.RadioButton") Then
                Dim rbd As New RadioButton()
                rbd = control(i)
                If (rbd.IsChecked) Then
                    Values(i) = "1"
                Else
                    Values(i) = "0"
                End If

            ElseIf type = "System.Windows.Controls.DatePicker" Then

                Dim dd As DatePicker = control(i)
                If dd.SelectedDate Is Nothing Then
                    Values(i) = Nothing
                Else
                    Values(i) = ToStrDate(dd.SelectedDate)
                End If


            ElseIf type = "System.Windows.Controls.PasswordBox" Then
                Values(i) = Encrypt(CType(control(i), PasswordBox).Password.Trim())
            ElseIf type = "System.Windows.Controls.RichTextBox" Then
                Values(i) = GetString(CType(control(i), RichTextBox))
            ElseIf type = "WpfApplication1.TimePicker" Then
                Values(i) = CType(control(i), TimePicker).Time
            ElseIf Table_Name = "PCs" And control(i).Name = "txtName" Then
                Values(i) = Encrypt(CType(control(i), TextBox).Text.Trim())
            Else
                Values(i) = CType(control(i), TextBox).Text.Trim()
            End If


            Try
                Values(i) = Values(i).Replace("'", "''")
            Catch
            End Try
        Next

    End Sub

    Enum SaveState
        All
        Insert
        Update
        Print
        Close
    End Enum

    Function GetString(rtb As RichTextBox) As String

        Dim txt As New TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd)
        Return txt.Text
    End Function


    Public Function Save(ByVal ID() As String, ByVal IDValue() As String, Optional ByVal State As SaveState = SaveState.All) As Boolean
        ' DefineValues()
        If Not StopPro() Then Return False
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            'main.sqlConnection1.Open()
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Clear()
            MyCmd.CommandText = ""
            If State <> SaveState.Insert Then
                MyCmd.CommandText = "IF Exists(Select *  From " & Table_Name & " T Where " & ID(0) & "='" & IDValue(0) & "'"
                For i As Integer = 1 To ID.Length - 1
                    MyCmd.CommandText &= " and " & ID(i) & "='" & IDValue(i) & "'"
                Next

                MyCmd.CommandText &= " " & AppendWhere & ")"
                MyCmd.CommandText &= " Update " & Table_Name & "  Set UserName='" & Md.UserName & "',MyGetDate=GetDate(),"
                For i As Integer = 0 To Fields.Length - 1
                    MyCmd.CommandText &= Fields(i) & "='" & Values(i) & "'"
                    If Not i = Fields.Length - 1 Then
                        MyCmd.CommandText &= ","
                    End If
                Next
                MyCmd.CommandText &= " Where " & ID(0) & "='" & IDValue(0) & "'"

                For i As Integer = 1 To ID.Length - 1
                    MyCmd.CommandText &= " and " & ID(i) & "='" & IDValue(i) & "'"
                Next
                MyCmd.CommandText &= AppendWhere
            End If

            If State = SaveState.All Then
                MyCmd.CommandText &= " Else "
            End If

            If State <> SaveState.Update Then
                MyCmd.CommandText &= "Insert into " & Table_Name & "(UserName,MyGetDate," & Table_Fields() & " ) Values ('" & Md.UserName & "',GetDate(),'"
                For i As Integer = 0 To Fields.Length - 1

                    MyCmd.CommandText &= Values(i)
                    If Not i = Fields.Length - 1 Then
                        MyCmd.CommandText &= "', '"
                    Else : MyCmd.CommandText &= "')"
                    End If
                Next
            End If

            CurrentIDValue = IDValue
            MyCmd.ExecuteNonQuery()
            MyCmd.Connection.Close()
            Return True

        Catch ex As Exception
            If TypeOf (ex) Is System.Data.SqlClient.SqlException Then
                If DirectCast(ex, System.Data.SqlClient.SqlException).Number = 2627 Then
                    ShowMSG("Please, try again later.")
                End If
            Else
                If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
            End If
            Return False
        Finally
            c = Nothing
        End Try
    End Function

    Public Sub SearchTable(ByVal ID() As String, ByVal IDValue() As String, Cbo As ComboBox, Optional SpName As String = "", Optional UsedControls() As Control = Nothing, Optional OrderByStatement As String = "", Optional IncludeDate As Boolean = False)
        Try
            Dim str As String = ""

            str = "Select " & ID(ID.Length - 1) & " Id,cast(" & IIf(SpName <> "", SpName, ID(ID.Length - 1)) & " as varchar(100)) Name  From " & Table_Name & " T Where 1=1 "
            For i As Integer = KeyFields.Length To Fields.Length - 1
                If (Val(Values(i)) = 0 OrElse IsDate(Values(i))) AndAlso Not IncludeDate Then Continue For
                If UsedControls Is Nothing OrElse UsedControls.Contains(control(i)) Then
                    str &= " and " & Fields(i) & "='" & Values(i) & "'"
                End If
            Next

            For i As Integer = 0 To ID.Length - 2
                str &= " and " & ID(i) & "='" & IDValue(i) & "'"
            Next
            str &= AppendWhere & " union select 0 Id,'-' Name" & " " & OrderByStatement

            FillCombo(str, Cbo)
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub

    Public Function SaveGrid(ByVal Grid As Forms.DataGridView, ByVal TableName As String, ByVal KeysFields() As String, ByVal KeysValues() As String, ByVal MyFields() As String, ByVal ColumnsNames() As String, ByVal Type() As VariantType, ByVal ColumnsKeys() As String, Optional ByVal LineField As String = "", Optional ByVal LineColumnName As String = "", Optional MasterAndDetailsAreInOneTable As Boolean = False) As Boolean
        'SaveGrid(New DataGridView, "", New Integer() {}, New VariantType() {VariantType.String})
        Dim Statement As String = "Delete From " & TableName & " where 1=1" & vbCrLf
        For i As Integer = 0 To KeysFields.Length - 1
            Statement &= " and " & KeysFields(i) & "='" & KeysValues(i) & "'" & vbCrLf
        Next

        Dim Statement2 As String = ""
        For i As Integer = 0 To Grid.Rows.Count - 1 - IIf(Grid.AllowUserToAddRows, 1, 0)
            Dim Statement3 As String = ""
            Dim InsertIdentity As Boolean = True
            If LineField <> "" Then
                If GetCellValue(VariantType.Integer, Grid.Rows(i).Cells(LineField).Value) = 0 Then
                    Statement3 &= " set identity_insert " & TableName & " off "
                    InsertIdentity = True
                Else
                    Statement3 &= " set identity_insert " & TableName & " on "
                    InsertIdentity = False
                End If
            End If

            Statement3 &= " Insert " & TableName & "(UserName,MyGetDate,"
            If Not MasterAndDetailsAreInOneTable Then
                For x As Integer = 0 To KeysFields.Length - 1
                    Statement3 &= KeysFields(x) & ","
                Next
            End If
            If Not InsertIdentity Then Statement3 &= LineField & ","
            For x As Integer = 0 To MyFields.Length - 1
                Statement3 &= MyFields(x) & ","
            Next
            If MasterAndDetailsAreInOneTable Then
                For x As Integer = 0 To Fields.Length - 1
                    Statement3 &= Fields(x) & ","
                Next
            End If

            Statement3 = Mid(Statement3, 1, Len(Statement3) - 1)
            Statement3 &= ") values " & vbCrLf

            For x As Integer = 0 To ColumnsKeys.Length - 1
                If Grid.Rows(i).Cells(ColumnsKeys(x)).Value Is Nothing OrElse Grid.Rows(i).Cells(ColumnsKeys(x)).Value.ToString = "" Then GoTo EndFor 'OrElse Val(Grid.Rows(i).Cells(ColumnsKeys(x)).Value.ToString) = 0
            Next

            'Statement2 &= IIf(Statement2 = "", "", ",")

            Statement3 &= "("
            If Grid.Columns.Contains("UserName") Then
                If Grid.Rows(i).Cells("UserName").Value Is Nothing OrElse Grid.Rows(i).Cells("UserName").Value.ToString = "" Then
                    Statement3 &= "'" & Md.UserName & "',"
                Else
                    Statement3 &= "'" & Grid.Rows(i).Cells("UserName").Value & "',"
                End If
            Else
                Statement3 &= "'" & Md.UserName & "',"
            End If

            If Grid.Columns.Contains("MyGetDate") Then
                If Grid.Rows(i).Cells("MyGetDate").Value Is Nothing OrElse Grid.Rows(i).Cells("MyGetDate").Value.ToString = "" Then
                    Statement3 &= "GetDate(),"
                Else
                    Statement3 &= "'" & Grid.Rows(i).Cells("MyGetDate").Value & "',"
                End If
            Else
                Statement3 &= "GetDate(),"
            End If


            If Not MasterAndDetailsAreInOneTable Then
                For x As Integer = 0 To KeysValues.Length - 1
                    Statement3 &= "'" & KeysValues(x) & "',"
                Next
            End If
            If Not InsertIdentity Then Statement3 &= GetCellValue(VariantType.Integer, Grid.Rows(i).Cells(LineField).Value) & ","
            For x As Integer = 0 To ColumnsNames.Length - 1
                Statement3 &= GetCellValue(Type(x), Grid.Rows(i).Cells(ColumnsNames(x)).Value) & ","
            Next
            If MasterAndDetailsAreInOneTable Then
                For x As Integer = 0 To Fields.Length - 1
                    Statement3 &= "'" & Values(x) & "',"
                Next
            End If

            Statement3 = Mid(Statement3, 1, Len(Statement3) - 1)
            Statement3 &= ")"

            Statement2 &= Statement3
EndFor:
        Next

        Return ExecuteNonQuery(Statement & Statement2)
    End Function

    Function GetCellValue(ByVal Type As VariantType, ByVal Value As Object) As String
        Try
            Value = Value.ToString.Replace("'", "''")
        Catch ex As Exception
        End Try
        Select Case Type
            Case VariantType.Date
                Try
                    If Value Is Nothing Then Return "null"
                    Return "'" & ToStrDate(DateTime.Parse(Value)) & "'"
                Catch ex As Exception
                    Return "null"
                End Try
            Case VariantType.Integer
                Return Val(Value)
            Case VariantType.Decimal
                Return Val(Value)
            Case VariantType.Boolean
                Return IIf(Value, 1, 0)
            Case VariantType.String
                Return "'" & Value & "'"
            Case Else
                Return "''"
        End Select
    End Function

    '___________________________Check IF Record Whith Condition is Exist__________________
    Public Function IF_Exists() As Boolean

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim dt As New DataTable
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandText = ""
            MyCmd.CommandText = " IF Exists ( Select * From " & Table_Name & " " & WhereKeyFields() & " ) select '1' else select '0'"

            da.SelectCommand = MyCmd
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            If dt.Rows(0)(0).ToString().Trim = "1" Then
                Return True
            Else : Return False
            End If

        Catch
            Return False
        End Try
    End Function
    Public Function IF_Exists(ByVal SqlStatment As String) As Boolean

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim dt As New DataTable
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = ""
            MyCmd.CommandText = " IF Exists (" & SqlStatment & ") select '1' else select '0'"

            da.SelectCommand = MyCmd
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            If dt.Rows(0)(0).ToString().Trim = "1" Then

                Return True
            Else : Return False
            End If

        Catch
            Return False
        Finally
            c = Nothing
        End Try
    End Function

    Public Sub FillControls(frm As Object)

        Try

            Dim Type As String = ""
            For i As Integer = 0 To Fields.Length - 1
                Type = control(i).GetType().ToString().Trim
                Dim nn As String = control(i).Name
                If Type = "System.Windows.Controls.ComboBox" Then
                    Dim cbo As ComboBox = control(i)
                    If (cbo.ItemsSource Is Nothing) Then    'for save integereger no. in DB. from combox
                        Dim index As String = ReturnedValues(0, i)
                        If Not (ReturnedValues(0, i).Trim() = "") Then
                            Try
                                cbo.SelectedIndex = Integer.Parse(ReturnedValues(0, i))
                            Catch ex As Exception
                            End Try
                        End If
                    Else
                        Try
                            cbo.SelectedIndex = 0
                            cbo.SelectedValue = ReturnedValues(0, i)
                        Catch ex As Exception
                            cbo.SelectedIndex = -1
                        End Try
                    End If
                ElseIf (Type = "System.Windows.Controls.CheckBox") Then

                    Dim chk As CheckBox = control(i)
                    Dim val As String = ReturnedValues(0, i)
                    If (ReturnedValues(0, i) = "1" Or ReturnedValues(0, i) = "True") Then
                        chk.IsChecked = True
                    Else : chk.IsChecked = False
                    End If
                ElseIf (Type = "System.Windows.Controls.RadioButton") Then
                    Dim rbt As RadioButton = control(i)
                    If (ReturnedValues(0, i) = "1") Then
                        rbt.IsChecked = True

                    Else : rbt.IsChecked = False

                    End If
                ElseIf (Type = "System.Windows.Controls.PasswordBox") Then
                    Dim txt As PasswordBox = control(i)
                    Try
                        CType(control(i), PasswordBox).Password = Decrypt(ReturnedValues(0, i))
                    Catch
                        CType(control(i), PasswordBox).Password = ""
                    End Try
                ElseIf (Type = "WpfApplication1.TimePicker") Then
                    CType(control(i), TimePicker).Time = ReturnedValues(0, i)
                ElseIf (Type = "System.Windows.Controls.DatePicker") Then
                    Dim txt As DatePicker = control(i)
                    Try
                        If DateTime.Parse(ReturnedValues(0, i)) = DateTime.Parse("01/01/1900") Then
                            CType(control(i), DatePicker).SelectedDate = Nothing
                        Else
                            Try
                                CType(control(i), DatePicker).SelectedDate = ReturnedValues(0, i)
                            Catch ex As Exception
                                CType(control(i), DatePicker).SelectedDate = Nothing
                            End Try
                        End If
                    Catch ex As Exception
                    End Try
                ElseIf (Table_Name = "PCs" And control(i).Name = "txtName") Then
                    Dim txt As TextBox = control(i)
                    CType(control(i), TextBox).Text = Decrypt(ReturnedValues(0, i))
                Else : CType(control(i), TextBox).Text = ReturnedValues(0, i)
                End If
            Next

        Catch ex As Exception
        End Try


        If TypeOf frm.Parent Is MyPage Then
            For Each c As Control In MyEditControls
                c.IsEnabled = CType(frm.Parent, MyPage).MySecurityType.AllowEdit
            Next
        ElseIf TypeOf frm.Parent Is MyWindow Then
            For Each c As Control In MyEditControls
                c.IsEnabled = CType(frm.Parent, MyWindow).MySecurityType.AllowEdit
            Next
        End If

    End Sub
    Public Sub RetrieveAll(ByVal ID() As String, ByVal IDValue() As String, ByRef dt As DataTable)   ' k is the control focus if not exists

        '			if(!TestEmpt())
        '				return

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            dt = New DataTable("Tbl")
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.Parameters.Clear()
            MyCmd.CommandText = ""
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText &= "Select " & Table_Fields() & " From " & Table_Name & " T "
            MyCmd.CommandText &= " Where " & ID(0) & "='" & IDValue(0) & "'" & AppendWhere

            For i As Integer = 1 To ID.Length - 1
                MyCmd.CommandText &= " and " & ID(i) & "='" & IDValue(i) & "'"
            Next

            da.SelectCommand = MyCmd
            dt = New DataTable("Tbl")
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            FillValues(dt)

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        Finally
            c = Nothing
        End Try

    End Sub



    '___________________________________________________________________________________
    '___________________________Fill Retuened Values ___________________________________
    Public Sub FillValues(ByVal dt As DataTable)

        'Dim ReturnedValues(0, Fields.Length) As String
        If (dt.Rows.Count = 0) Then
            Return
        End If

        ReDim ReturnedValues(dt.Rows.Count, Fields.Length)
        For j As Integer = 0 To dt.Rows.Count - 1
            For i As Integer = 0 To Fields.Length - 1
                ReturnedValues(j, i) = dt.Rows(j)(i).ToString().Trim()
            Next
        Next
    End Sub

    Public Sub FirstLast(ByVal ID() As String, ByVal MaxOrMin As String, ByRef dt As DataTable)
        CurrentIDValue = {}
        DefineValues()
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            dt = New DataTable("Tbl")
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Clear()
            MyCmd.CommandText = "Select " & Table_Fields() & " From " & Table_Name & " T "

            Dim MyKeys As String = ""

            DiscountKeyFiels = 1
            MyCmd.CommandText &= WhereKeyFields()

            Dim KeyField As String = ID(KeyFields.Length - 1).Replace("T.", "") 'KeyFields[KeyFields.Length-1]
            MyCmd.CommandText &= " " & AppendWhere & " and " & KeyField & " =(select " & MaxOrMin & "(" & KeyField & ") From " & Table_Name & " " & WhereKeyFields() & " " & AppendWhere & " ) "

            DiscountKeyFiels = 0
            da.SelectCommand = MyCmd
            dt = New DataTable("Tbl")
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()
            FillValues(dt)

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        Finally
            c = Nothing
        End Try
    End Sub

    Public Sub NextPrevious(ByVal ID() As String, ByVal Value() As String, ByVal NextOrBack As String, ByRef dt As DataTable)

        If CurrentIDValue.Length > 0 Then Value = CurrentIDValue
        CurrentIDValue = {}

        DefineValues()
        dt = New DataTable("Tbl")
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            Dim MaxOrMin As String = "min"
            Dim Compare As String = ">"
            If (NextOrBack = "Next") Or (NextOrBack = "next") Then

                MaxOrMin = "min"
                Compare = " > "
            ElseIf (NextOrBack = "Back") Or (NextOrBack = "back") Then
                MaxOrMin = "max"
                Compare = " < "

            Else : Return
            End If

            dt = New DataTable("Tbl")
            MyCmd.CommandText = "Select " & Table_Fields() & " From " & Table_Name & " T "
            Dim KeyField As String = ID(KeyFields.Length - 1)
            KeyField = KeyField.Replace("T.", "")

            DiscountKeyFiels = 1
            MyCmd.CommandText &= WhereKeyFields() & AppendWhere

            MyCmd.CommandText &= " and " & KeyFields(KeyFields.Length - 1) & "=("
            MyCmd.CommandText &= "Select " & MaxOrMin & "(" & KeyField & ") From " & Table_Name
            MyCmd.CommandText &= WhereKeyFields()
            DiscountKeyFiels = 0

            If (Value(KeyFields.Length - 1) = "") Then
                Compare = " > "
            End If
            MyCmd.CommandText &= " and " & KeyFields(KeyFields.Length - 1) & Compare & "'" & Value(KeyFields.Length - 1) & "' " & AppendWhere & ")"

            da.SelectCommand = MyCmd
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()
            If (dt.Rows.Count = 0) Then
                Return
            End If
            FillValues(dt)

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        Finally
            c = Nothing
        End Try
    End Sub

    Dim EventHasAdded As Boolean = False
    Public Sub ClearControls(Optional ByVal Focus As Boolean = True)

        If Not EventHasAdded AndAlso control.Length > 0 Then
            EventHasAdded = True
            Addcontrol_MouseDoubleClick(control)
        End If

        Dim type As String = ""
        For i As Integer = 0 To control.Length - 1
            Dim s As String = control(i).Name

            For n As Integer = 0 To KeyFields.Length - 1
                If KeyFields(n) = Fields(i) Then
                    GoTo A
                End If
            Next

            type = control(i).GetType().ToString().Trim
            If (type = "System.Windows.Controls.ComboBox") Then

                Dim cbo As ComboBox = control(i)
                Try
                    cbo.SelectedIndex = 0
                Catch
                End Try
            ElseIf (type = "System.Windows.Controls.CheckBox") Then

                Dim chk As CheckBox = control(i)
                chk.IsChecked = False

            ElseIf (type = "System.Windows.Controls.PictureBox") Then

                'Dim chk As PictureBox = control(i)
                'chk.Image = Image.FromFile(Application.Current.StartupUri.ToString & "\Images\no.photo.gif")

            ElseIf (type = "System.Windows.Controls.RadioButton") Then
                Dim chk As RadioButton = control(i)
                chk.IsChecked = False
            ElseIf (type = "System.Windows.Controls.DatePicker") Then
                Dim chk As DatePicker = control(i)
                chk.SelectedDate = Nothing
            ElseIf (type = "WpfApplication1.TimePicker") Then
                Dim chk As TimePicker = control(i)
                chk.Time = "12:00 AM"
            ElseIf (type = "System.Windows.Controls.PasswordBox") Then
                Dim chk As PasswordBox = control(i)
                chk.Password = ""
            Else : CType(control(i), TextBox).Text = ""
            End If
A:
        Next
        If Focus Then control(0).Focus()

        For Each c As Control In MyEditControls
            c.IsEnabled = True
        Next

    End Sub




    '----------------------Select Items ON Conditions-----------------------------------
    Private Function WhereKeyFields() As String

        GeneralString = ""
        GeneralString &= " Where 1=1 "
        For j As Integer = 0 To KeyFields.Length - DiscountKeyFiels - 1
            GeneralString &= " and " & KeyFields(j) & "='"
            For i As Integer = 0 To Fields.Length - 1
                If Fields(i) = KeyFields(j) Then
                    GeneralString &= Values(i)
                End If
            Next
            GeneralString &= "' "
        Next
        Return GeneralString
    End Function


    Private Function WhereLastKeyFields() As String

        GeneralString = ""

        GeneralString &= KeyFields(KeyFields.Length - 1) & "='"
        For i As Integer = 0 To Fields.Length - 1
            If Fields(i) = KeyFields(KeyFields.Length - 1) Then
                GeneralString &= Values(i)
            End If
        Next
        GeneralString &= "'"

        Return GeneralString
    End Function


    '----------------------------------------------------------------------------------
    ' still not used
    Private Function ValueFields() As String
        GeneralString = ""
        For k As Integer = 0 To KeyFields.Length - 1
            GeneralString &= Values(k)
            If (k = KeyFields.Length - 2) Then
                GeneralString &= ""
            Else : GeneralString &= "+"
            End If
        Next
        Return GeneralString
    End Function
    '--------------------------Select Items From Table---------------------------------
    Private Function Table_Fields() As String

        GeneralString = ""
        'GeneralString&="Select "
        For i As Integer = 0 To Fields.Length - 1

            GeneralString &= Fields(i)
            If Not i = Fields.Length - 1 Then
                GeneralString &= " , "
            End If
        Next
        Return GeneralString
    End Function

    Public Function RetrieveNameOnly(ByVal SqlStatment As String) As String

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandText = SqlStatment
            Dim ss As String = MyCmd.ExecuteScalar().ToString().Trim
            MyCmd.Connection.Close()
            If ss.Trim() = "" Then

                Return -1
            End If
            Return ss.ToString().Trim

        Catch
            Return -1
        Finally
            c = Nothing
        End Try
    End Function

    Public Function GetMax(ByVal Length As Integer) As String

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            DefineValues()
            Dim kf As Integer = KeyFields.Length

            DiscountKeyFiels = 1
            If (kf > 1) Then
                MyCmd.CommandText = " select max(" & KeyFields(kf - 1) & ") from " & Table_Name & " T " & WhereKeyFields()
            Else : MyCmd.CommandText = " select max(" & KeyFields(kf - 1) & ") from " & Table_Name
            End If

            Dim ss1 As String = MyCmd.ExecuteScalar().ToString().Trim
            MyCmd.Connection.Close()

            Dim ss As Integer = 0

            DiscountKeyFiels = 0
            If (ss1.Trim() = "") Then
                ss = 1
            Else

                ss = Integer.Parse(ss1)
                ss += 1
            End If
            Return ss.ToString().Trim.PadLeft(Length, "0")

        Catch ex As Exception
            Return ex.Message.ToString()
        Finally
            c = Nothing
        End Try
    End Function

    Public Function LoadConnString(ByVal file As String) As String
        Dim ConnectionString As String = ""
        Dim sr As StreamReader = New StreamReader(file)
        sr.ReadLine()
        sr.ReadLine()
        For i As Integer = 0 To 19
            sr.Read()
        Next
        ConnectionString = sr.ReadToEnd()
        sr.Close()
        Return ConnectionString
    End Function

    Public Function IIf(Exp As Boolean, TrueValue As Object, FlaseValue As Object)
        If Exp Then
            Return TrueValue
        Else
            Return FlaseValue
        End If
    End Function
    Public Function FillDate(ByVal dd As DateTime) As String

        Dim dd1 As String = dd.Month.ToString().Trim
        dd1 &= "/" & dd.Day.ToString() & "/" & dd.Year.ToString()
        Return dd1
    End Function

    Public Sub FillCombo(ByVal TableName As String, ByVal cbo As ComboBox, ByVal Condition As String, Optional ByVal c0 As String = "-", Optional OrderById As Boolean = False, Optional IgnoreUnion As Boolean = False)

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            cbo.IsTextSearchEnabled = True
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = IIf(Not IgnoreUnion, "select 0 Id, '" & c0 & "' Name union ", "") & "Select Id,Name From " & TableName & "  " & Condition & " order by " & IIf(OrderById, "Id", "Name")
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim ds As New DataSet
            da.SelectCommand = MyCmd
            da.Fill(ds, TableName)
            MyCmd.Connection.Close()

            Dim d As Integer = ds.Tables(0).Rows.Count
            Dim dv As New DataView
            dv.Table = ds.Tables(0)
            Try
                cbo.ItemsSource = Nothing
            Catch ex As Exception
            End Try
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
            c = Nothing
        End Try
    End Sub


    Public Sub FillCombo(ByVal SqlStatment As String, ByVal cbo As ComboBox)

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            cbo.IsTextSearchEnabled = True
            Try
                cbo.SelectedIndex = -1
            Catch
            End Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = SqlStatment
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim ds As New DataSet()
            da.SelectCommand = MyCmd
            da.Fill(ds, "Table1")
            MyCmd.Connection.Close()
            Dim d As Integer = ds.Tables(0).Rows.Count
            Dim dv As New DataView
            dv.Table = ds.Tables(0)
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
            c = Nothing
        End Try
    End Sub

    Public Sub FillCombo(ByVal ddtt As DataTable, ByVal cbo As ComboBox)
        Try
            cbo.IsTextSearchEnabled = True
            Try
                cbo.SelectedIndex = -1
            Catch
            End Try
            Dim dv As New DataView
            dv.Table = ddtt
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
        End Try
    End Sub

    Public Sub FillListBox(ByVal SqlStatment As String, ByVal Lst As ListBox)

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = SqlStatment
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim ds As New DataSet()
            da.SelectCommand = MyCmd
            da.Fill(ds, "Table1")
            MyCmd.Connection.Close()
            Dim d As Integer = ds.Tables(0).Rows.Count
            Lst.ItemsSource = ds.Tables("Table1")
            Lst.DisplayMemberPath = "NAME"
            'Lst.ValueMember = "Id"
            Lst.SelectedIndex = 0
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
            c = Nothing
        End Try
    End Sub

    Public Sub FillListBox(ByVal DT As DataTable, ByVal Lst As ListBox)

        Try
            'Lst.DataSource = DT
            'Lst.DisplayMember = "NAME"
            'Lst.ValueMember = "Id"
            Lst.SelectedIndex = 0
        Catch ex As Exception
            Dim s As String = ex.Message
        End Try
    End Sub

    Public Sub FillCombo(ByVal SqlStatment As String, ByVal cbo As Forms.DataGridViewComboBoxColumn)

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = SqlStatment
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            da.SelectCommand = MyCmd
            Dim dt As New DataTable
            dt.Columns.Add("Id")
            dt.Columns.Add("Name")
            da.Fill(dt)
            MyCmd.Connection.Close()

            cbo.DataSource = dt
            cbo.ValueMember = "Id"
            cbo.DisplayMember = "NAME"
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
            c = Nothing
        End Try
    End Sub

    Public Sub FillCombo(ByVal SqlStatment As String, ByVal cbo As Forms.DataGridViewComboBoxCell)

        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = SqlStatment
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim ds As New DataSet()
            da.SelectCommand = MyCmd
            da.Fill(ds, "Table1")
            MyCmd.Connection.Close()

            Dim d As Integer = ds.Tables(0).Rows.Count
            cbo.DataSource = ds.Tables("Table1")
            cbo.DisplayMember = "NAME"
            cbo.ValueMember = "Id"
        Catch ex As Exception
            Dim s As String = ex.Message
        Finally
            c = Nothing
        End Try
    End Sub

    Public Function AddItemToTable(ByVal tbl As String, ByVal str As String, Optional ByVal p() As String = Nothing, Optional ByVal v() As String = Nothing) As Boolean
        Dim str1 As String = ""
        Dim str2 As String = ""
        Dim str3 As String = ""
        If Not IsNothing(p) Then
            For i As Integer = 0 To p.Length - 1
                str1 &= " and " & p(i) & "='" & v(i) & "'"
                str2 &= "," & p(i)
                str3 &= ",'" & v(i) & "'"
            Next
        End If
        Return ExecuteNonQuery("if not exists(select * from " & tbl & " where Name='" & str.Trim & "') insert " & tbl & "(Id,Name,UserName,MyGetDate" & str2 & ") values(isnull((select MAX(Id)+1 from " & tbl & " where 1=1 " & str1 & "),1),'" & str.Trim & "'," & Md.UserName & ",GETDATE()" & str3 & ")")
    End Function

    Function TestNames(ArName As TextBox, EnName As TextBox, Optional Fource As Boolean = True) As Boolean

        ArName.Text = ArName.Text.Trim
        EnName.Text = EnName.Text.Trim
        While ArName.Text.Contains("  ")
            ArName.Text = ArName.Text.Replace("  ", " ")
        End While
        While EnName.Text.Contains("  ")
            EnName.Text = EnName.Text.Replace("  ", " ")
        End While

        Dim Ar() As String
        Ar = ArName.Text.Split(" ")
        Dim En() As String
        En = EnName.Text.Split(" ")
        If Fource AndAlso Ar.Length <> En.Length Then
            ShowMSG("Arabic Name Length must be EQUALE English Name Length")
            ArName.Focus()
            Return False
        End If

        Dim x As Integer = 0
        For i As Integer = 0 To Ar.Length - 1
            If Fource AndAlso Ar(i) = En(i) And Not IsNumeric(Ar(i)) Then
                ShowMSG("Arabic Name could not be EQUALE English Name")
                EnName.Select(x, En(i).Length)
                EnName.Focus()
                Return False
            End If
            x += En(i).Length + 1
        Next


        For i As Integer = 0 To Ar.Length - 1
            Dim a As String = ExecuteScalar("delete from Names  where Arabic_Name='" & Ar(i) & "' insert into Names (Arabic_Name,English_Name) values ('" & Ar(i) & "','" & En(i) & "')")
        Next

        Return True
    End Function


    Public Function GetEnName(ArName As String) As String
        ArName = ArName.Trim
        While ArName.Contains("  ")
            ArName = ArName.Replace("  ", " ")
        End While
        Dim s() As String
        s = ArName.Split(" ")
        Dim EnName As String = ""
        For i As Integer = 0 To s.Length - 1
            Dim a As String = ExecuteScalar("select top 1 English_Name from Names where Arabic_Name='" & s(i) & "'")
            If a = "" Then
                EnName &= s(i)
            Else
                EnName &= a
            End If
            EnName &= " "
        Next
        Return EnName.Trim
    End Function


    Public Function AddItemToTable(ByVal tbl As String, ByVal Fld As String(), ByVal str As String(), Optional ByVal p() As String = Nothing, Optional ByVal v() As String = Nothing) As Boolean
        Dim str1 As String = ""
        Dim str2 As String = ""
        Dim str3 As String = ""
        Dim str4 As String = ""
        Dim str5 As String = ""
        If Not IsNothing(p) Then
            For i As Integer = 0 To p.Length - 1
                str1 &= " and " & p(i) & "='" & v(i) & "'"
                str2 &= "," & p(i)
                str3 &= ",'" & v(i) & "'"
            Next
        End If
        For i As Integer = 0 To Fld.Length - 1
            str4 &= "," & Fld(i)
            str5 &= ",'" & str(i) & "'"
        Next
        Return ExecuteNonQuery("insert " & tbl & "(Id,UserName,MyGetDate" & str2 & str4 & ") values(-1," & Md.UserName & ",GETDATE()" & str3 & str5 & ") update " & tbl & " set Id=isnull((select ISNULL(MAX(Id),0)+1 from " & tbl & " where Id>0 " & str1 & "),1) where id=-1")
    End Function

    Public Function ExecuteNonQuery(ByVal sqlstatment As String, Optional IncludeTran As Boolean = True) As Boolean

        If Not StopPro() Then Return False
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text

            If IncludeTran Then
                MyCmd.CommandText = "SET XACT_ABORT ON BEGIN TRAN " & vbCrLf & sqlstatment & vbCrLf & " COMMIT"
            Else
                MyCmd.CommandText = sqlstatment
            End If

            MyCmd.ExecuteNonQuery()
            MyCmd.Connection.Close()
        Catch ex As Exception
            Dim s As String = ex.Message.ToString()
            c = Nothing
            Return False
        Finally
            c = Nothing
        End Try
        Return True

    End Function
    Public Function StopPro() As Boolean
        If Not Md.StopProfiler Then Return True
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = "StopPro"
            MyCmd.ExecuteNonQuery()
            MyCmd.Connection.Close()
        Catch ex As Exception
            Dim s As String = ex.Message.ToString()
            c = Nothing
            Return False
        Finally
            c = Nothing
        End Try
        Return True

    End Function

    Public Function ExecuteAdapter(ByVal sqlstatment As String) As DataTable

        Dim dt As New DataTable("Tbl")
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            dt = New DataTable("Tbl")
            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = sqlstatment
            da.SelectCommand = MyCmd

            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            Dim x As Integer = dt.Rows.Count
            Return dt

        Catch ex As Exception
            Dim SS As String = ex.Message.ToString()
            Return dt
        Finally
            c = Nothing
        End Try
    End Function

    Public Function ExecuteAdapter(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String) As DataTable

        Dim dt As New DataTable("Tbl")
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName
            da.SelectCommand = MyCmd

            MyCmd.Parameters.Clear()
            For i As Integer = 0 To ParaName.Length - 1
                MyCmd.Parameters.Add("@" & ParaName(i), SqlDbType.VarChar)
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
            Next
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            Return dt

        Catch ex As Exception
            Dim ss As String = ex.Message.ToString()
            Return dt
        Finally
            c = Nothing
        End Try
    End Function


    'Private Delegate Sub RateChangeNotification(table As DataTable)
    'Dim dependency As SqlDependency
    'Public Sub StartNotification(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String)
    '    Dim dt As New DataTable( "Tbl")
    '    Dim c As New SqlConnection(con.ConnectionString)
    '    Try
    '        SqlDependency.Start(c.ConnectionString, "QueueName")
    '        If c.State = ConnectionState.Closed Then c.Open()

    '        Dim MyCmd As SqlCommand = c.CreateCommand()
    '        If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

    '        MyCmd.CommandTimeout = 72000000
    '        MyCmd.Parameters.Clear()
    '        MyCmd.CommandType = CommandType.StoredProcedure
    '        MyCmd.CommandText = StoredName

    '        MyCmd.Parameters.Clear()
    '        For i As Integer = 0 To ParaName.Length - 1
    '            MyCmd.Parameters.Add("@" & ParaName(i), SqlDbType.VarChar)
    '            MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
    '        Next

    '        dependency = New SqlDependency(MyCmd)
    '        AddHandler dependency.OnChange, AddressOf dependency_OnChange

    '    Catch ex As Exception
    '    End Try
    'End Sub

    'Private Sub dependency_OnChange(sender As Object, e As SqlNotificationEventArgs)
    '    MessageBox.Show(e.Info)
    '    MessageBox.Show(e.Info)
    'End Sub

    'Public Sub StopNotification()
    '    SqlDependency.Stop(con.ConnectionString, "QueueName")
    'End Sub



    Public Sub SendEMail(ByVal Bath As String)
        Dim demoThread As New System.ComponentModel.BackgroundWorker
        AddHandler demoThread.DoWork, AddressOf MyDoWrok
        ' AddHandler demoThread.RunWorkerCompleted, AddressOf MyWorkerCompleted
        MyBath = Bath
        demoThread.RunWorkerAsync()
    End Sub
    Dim MyBath As String = ""
    Sub MyDoWrok(ByVal sender As Object, ByVal ev As EventArgs)
        SendEMail2(MyBath)
    End Sub

    Public Sub BackupAndSendEMail()
        Dim demoThread As New System.ComponentModel.BackgroundWorker
        AddHandler demoThread.DoWork, AddressOf MyBackupDoWrok
        AddHandler demoThread.RunWorkerCompleted, AddressOf MyWorkerCompletedBackupAndSendEMail
        demoThread.RunWorkerAsync()
    End Sub
    Sub MyBackupDoWrok(ByVal sender As Object, ByVal ev As EventArgs)
        Try
            If Not IO.Directory.Exists(Path) Then IO.Directory.CreateDirectory(Path)
            Dim MyPath As String = ExecuteScalar("bk", New String() {"Bath"}, New String() {Path})
            SendEMail2(MyPath)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub MyWorkerCompletedBackupAndSendEMail(sender As Object, e As ComponentModel.RunWorkerCompletedEventArgs)
        Dim s As String = 0
        s = 1
    End Sub

    Public Sub SendEMail2(ByVal Bath As String)
        Dim mailMsg As New MailMessage()
        mailMsg.From = New MailAddress("PhysicsAdvansed@gmail.com")
        mailMsg.To.Add("Mr_Aymans@Yahoo.com")
        mailMsg.To.Add("PhysicsAdvansed@Yahoo.com")
        mailMsg.To.Add("Mr_Aymans@Hotmail.com")
        Dim MyNow As DateTime = ExecuteScalar("Select dbo.MyGetDate()")
        mailMsg.Subject = Bath.Replace(Application.Current.StartupUri.ToString & "\", "") & " " & MyNow.ToShortDateString & " " & MyNow.ToShortTimeString
        mailMsg.IsBodyHtml = True
        mailMsg.BodyEncoding = Encoding.UTF8
        mailMsg.Attachments.Add(New System.Net.Mail.Attachment(Bath))
        mailMsg.Body = "Thanks, Mr. Physics"
        mailMsg.Priority = MailPriority.High
        ' Smtp configuration
        Dim client As New SmtpClient()
        client.Credentials = New NetworkCredential("PhysicsAdvansed@gmail.com", "0000000000")
        client.Port = 587 'or use 465
        client.Host = "smtp.gmail.com"
        client.EnableSsl = True
        client.Timeout = 72000000
        Dim userState = mailMsg
        Try
            'you can also call client.Send(mailMsg)
            client.Send(mailMsg)
            client.SendAsync(mailMsg, userState)
        Catch ex As Exception

        End Try
    End Sub

    Public Function ExecuteNonQuery(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String) As Boolean

        If Not StopPro() Then Return False
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName
            For i As Integer = 0 To ParaName.Length - 1

                MyCmd.Parameters.Add("@" & ParaName(i), SqlDbType.VarChar)
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
            Next
            MyCmd.ExecuteNonQuery()
            MyCmd.Connection.Close()
            Return True
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
            Return False
        Finally
            c = Nothing
        End Try
    End Function

    Public Function ExecuteAdapter(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String, ByVal Type() As SqlDbType) As DataTable

        Dim dt As New DataTable("Tbl")
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName
            da.SelectCommand = MyCmd

            For i As Integer = 0 To ParaName.Length - 1

                MyCmd.Parameters.Add("@" & ParaName(i), Type(i))
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
            Next
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            Return dt
        Catch ee As Exception
            Dim ss As String = ee.Message.ToString()
            Return dt
        Finally
            c = Nothing
        End Try
    End Function

    Public Function ExecuteNonQuery(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As Object, ByVal Type() As SqlDbType) As Boolean

        If Not StopPro() Then Return False
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName

            For i As Integer = 0 To ParaName.Length - 1
                MyCmd.Parameters.Add("@" & ParaName(i), Type(i))
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
                MyCmd.Parameters("@" & ParaName(i)).DbType = DbType.Object  'Type(i)
            Next
            MyCmd.ExecuteNonQuery()
            MyCmd.Connection.Close()
            Return True
        Catch ee As Exception
            Dim ss As String = ee.Message.ToString()
            Return False
        Finally
            c = Nothing
        End Try
    End Function

    Public Function ean13(chaine As String)
        chaine = Left(chaine, 12)
        While Len(chaine) < 12
            chaine = IIf(Len(chaine) = 11, "1", "0") & chaine
        End While
        Dim i As Integer
        Dim checksum As Integer
        ean13 = ""
        If Len(chaine) = 12 Then
            For i = 1 To 12
                If Asc(Mid(chaine, i, 1)) < 48 Or Asc(Mid(chaine, i, 1)) > 57 Then
                    i = 0
                    Exit For
                End If
            Next
            If i = 13 Then
                For i = 2 To 12 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                checksum = checksum * 3
                For i = 1 To 11 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                chaine = chaine & (10 - checksum Mod 10) Mod 10
                ean13 = chaine
            End If
        End If
    End Function

    Public Function ean13Code(chaine As String)
        chaine = Left(chaine, 12)
        While Len(chaine) < 12
            chaine = "0" & chaine
        End While
        Dim i As Integer
        Dim checksum As Integer
        Dim first As Integer
        Dim CodeBarre As String
        Dim tableA As Boolean
        ean13Code = ""
        If Len(chaine) = 12 Then
            For i = 1 To 12
                If Asc(Mid(chaine, i, 1)) < 48 Or Asc(Mid(chaine, i, 1)) > 57 Then
                    i = 0
                    Exit For
                End If
            Next
            If i = 13 Then
                For i = 2 To 12 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                checksum = checksum * 3
                For i = 1 To 11 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                chaine = chaine & (10 - checksum Mod 10) Mod 10
                CodeBarre = Left(chaine, 1) & Chr(65 + Val(Mid(chaine, 2, 1)))
                first = Val(Left(chaine, 1))
                For i = 3 To 7
                    tableA = False
                    Select Case i
                        Case 3
                            Select Case first
                                Case 0 To 3
                                    tableA = True
                            End Select
                        Case 4
                            Select Case first
                                Case 0, 4, 7, 8
                                    tableA = True
                            End Select
                        Case 5
                            Select Case first
                                Case 0, 1, 4, 5, 9
                                    tableA = True
                            End Select
                        Case 6
                            Select Case first
                                Case 0, 2, 5, 6, 7
                                    tableA = True
                            End Select
                        Case 7
                            Select Case first
                                Case 0, 3, 6, 8, 9
                                    tableA = True
                            End Select
                    End Select
                    If tableA Then
                        CodeBarre = CodeBarre & Chr(65 + Val(Mid(chaine, i, 1)))
                    Else
                        CodeBarre = CodeBarre & Chr(75 + Val(Mid(chaine, i, 1)))
                    End If
                Next
                CodeBarre = CodeBarre & "*"
                For i = 8 To 13
                    CodeBarre = CodeBarre & Chr(97 + Val(Mid(chaine, i, 1)))
                Next
                CodeBarre = CodeBarre & "+"
                ean13Code = CodeBarre
            End If
        End If
    End Function


    Public Function ExecuteScalar(ByVal sqlstatment As String) As String

        If Not StopPro() Then Return ""
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = sqlstatment

            Dim s As String = MyCmd.ExecuteScalar().ToString().Trim
            MyCmd.Connection.Close()
            Return s
        Catch ex As Exception
            Dim s As String = ex.Message.ToString()
            Return ""
        Finally
            c = Nothing
        End Try
    End Function


    Public Function ExecuteScalar(ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String) As String

        If Not StopPro() Then Return ""
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName
            For i As Integer = 0 To ParaName.Length - 1
                MyCmd.Parameters.Add("@" & ParaName(i), SqlDbType.VarChar)
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
            Next
            Dim s As String = MyCmd.ExecuteScalar().ToString().Trim
            MyCmd.Connection.Close()
            Return s
        Catch ee As Exception
            Dim ss As String = ee.Message.ToString()
            Return ""
        Finally
            c = Nothing
        End Try
    End Function


    Public Function ToStrDate(ByVal d As Object) As String
        Try
            If d = Nothing Then Return "1900/01/01"
            Dim dd As DateTime = DateTime.Parse(d)
            Return dd.Year.ToString() & "/" & dd.Month.ToString().PadLeft(2, "0") & "/" & dd.Day.ToString().PadLeft(2, "0")
        Catch ex As Exception
            Return "1900/01/01"
        End Try
    End Function

    Public Function ToStrDateTimeFormated(ByVal d As Object) As String
        Try
            If d Is Nothing OrElse d.ToString = "" Then Return ""
            Dim dd As DateTime = CType(d, DateTime)
            If dd = "1900-1-1" Then Return ""
            Return dd.Year.ToString().PadLeft(4, "0") & "-" & dd.Month.ToString().PadLeft(2, "0") & "-" & dd.Day.ToString().PadLeft(2, "0") & " " & dd.Hour.ToString().PadLeft(2, "0") & ":" & dd.Minute.ToString().PadLeft(2, "0") & ":" & dd.Second.ToString().PadLeft(2, "0")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function ToStrDateTime(ByVal dd As DateTime) As String
        Return (dd.Month.ToString().PadLeft(2, "0") & "/" & dd.Day.ToString().PadLeft(2, "0") & "/" & dd.ToString.Substring(6)).Replace("ص", "am").Replace("م", "pm")
    End Function

    Public Sub FillCombo(ByVal cbo As ComboBox, ByVal tbl As String, ByVal SubId As String, ByVal SubName As String)
        Try
            cbo.IsTextSearchEnabled = True
            Dim dt As DataTable = ExecuteAdapter("select " & SubId & " 'Id'," & SubName & " 'Name From " & tbl & " union select 0 Id,'-' Name")

            Dim dv As New DataView
            dv.Table = dt
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub

    Public Sub FillCombo(ByVal cbo As ComboBox, ByVal tbl As String, ByVal SubId As String, ByVal SubName As String, ByVal where As String)
        Try
            cbo.IsTextSearchEnabled = True
            Dim dt As DataTable = ExecuteAdapter("select " & SubId & " 'Id'," & SubName & " 'Name' From " & tbl & " " & where & " union select 0 Id,'-' Name")
            Dim dv As New DataView
            dv.Table = dt
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub
    Public Sub FillCombo(ByVal cbo As ComboBox, ByVal StoredName As String, ByVal ParaName() As String, ByVal ParaValue() As String)

        Dim dt As New DataTable("Tbl")
        Dim c As New SqlConnection(con.ConnectionString)
        Try
            cbo.IsTextSearchEnabled = True
            Dim MyCmd As SqlCommand = c.CreateCommand()
            If MyCmd.Connection.State = ConnectionState.Closed Then MyCmd.Connection.Open()

            MyCmd.CommandTimeout = 72000000
            MyCmd.Parameters.Clear()
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.CommandText = StoredName
            da.SelectCommand = MyCmd

            MyCmd.Parameters.Clear()
            For i As Integer = 0 To ParaName.Length - 1

                MyCmd.Parameters.Add("@" & ParaName(i), SqlDbType.VarChar)
                MyCmd.Parameters("@" & ParaName(i)).Value = ParaValue(i)
            Next
            dt = New DataTable("Tbl")
            da.Fill(dt)
            MyCmd.Connection.Close()

            Dim dv As New DataView
            dv.Table = dt
            cbo.ItemsSource = dv
            cbo.SelectedValuePath = "Id"
            cbo.DisplayMemberPath = "Name"
            cbo.SelectedIndex = 0
        Catch ex As Exception
        Finally
            c = Nothing
        End Try
    End Sub

    Public Sub SaveImage(ByVal tbl As String, ByVal ID() As String, ByVal IDValue() As String, ByVal Field As String, ByVal PicPox As Controls.Image)
        Try

            Dim stream As New MemoryStream()
            'Dim encoder As New PngBitmapDecoder 
            Dim encoder As New BmpBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(PicPox.Source))
            encoder.Save(stream)
            Dim ImageBytes As Byte()
            ImageBytes = stream.ToArray

            Dim statement As String = "update " & tbl & " set " & Field & "=@MyImage Where 1=1 "
            For i As Integer = 0 To ID.Length - 1
                statement &= " and " & ID(i) & "='" & IDValue(i) & "'"
            Next

            Dim com As New SqlClient.SqlCommand(statement, con)
            con.Open()
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            com.ExecuteNonQuery()

        Catch ex As Exception

        End Try
        con.Close()
    End Sub

    Public Function InsertImage(Optional MyPath As String = "C:\Temp\printscreen.jpg") As Integer
        Dim i As Integer = 0
        Try
            Dim ImageBytes As Byte() = File.ReadAllBytes(MyPath)
            Dim statement As String = "insert ImageTemp select @MyImage select @@IDENTITY"
            Dim com As New SqlClient.SqlCommand(statement, con)
            con.Open()
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            i = com.ExecuteScalar
        Catch ex As Exception
        End Try
        con.Close()
        Return i
    End Function

    Public Function InsertImageTemp(ByVal PicPox As Controls.Image) As Integer
        Try

            Dim stream As New MemoryStream()
            Dim encoder As New BmpBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(PicPox.Source))
            encoder.Save(stream)
            Dim ImageBytes As Byte()
            ImageBytes = stream.ToArray

            Dim statement As String = "insert ImageTemp select @MyImage select @@IDENTITY"
            Dim com As New SqlClient.SqlCommand(statement, con)
            con.Open()
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            Return com.ExecuteScalar()

        Catch ex As Exception
            Return 0
        End Try
        con.Close()
    End Function

    Public Sub GetImage(ByVal tbl As String, ByVal ID() As String, ByVal IDValue() As String, ByVal Field As String, ByVal PicPox As Controls.Image)
        Try
            Dim myCommand As SqlClient.SqlCommand
            Dim statement As String = "select " & Field & " from " & tbl & " Where 1=1 "
            For i As Integer = 0 To ID.Length - 1
                statement &= " and " & ID(i) & "='" & IDValue(i) & "'"
            Next

            myCommand = New SqlClient.SqlCommand(statement, con)
            con.Open()
            Dim imagedata() As Byte = CType(myCommand.ExecuteScalar(), Byte())
            Dim stmBLOBData As IO.MemoryStream = New IO.MemoryStream(imagedata)

            Dim bi3 As New BitmapImage
            bi3.BeginInit()
            bi3.StreamSource = stmBLOBData
            'bi3.DecodePixelWidth = 300
            'bi3.DecodePixelHeight = 300
            bi3.EndInit()
            PicPox.Source = bi3
        Catch ex As Exception
            SetNoImage(PicPox)
        End Try
        con.Close()
    End Sub


    Public Sub SaveFile(ByVal tbl As String, ByVal KeyId As String, ByVal KeyValue As String, ByVal KeyId2 As String, ByVal KeyValue2 As String, ByVal Field As String, ByVal Path As String)
        Try
            Dim ImageBytes As Byte() = File.ReadAllBytes(Path)
            Dim com As New SqlClient.SqlCommand("if exists (select * from " & tbl & " where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "') update " & tbl & " set " & Field & "=@MyImage,LastUpdate=getdate() where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' else insert into " & tbl & "( " & KeyId & "," & KeyId2 & "," & Field & ",LastUpdate) select '" & KeyValue & "','" & KeyValue2 & "',@MyImage,getdate()", con)
            com.CommandTimeout = 720000
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            If con.State = ConnectionState.Closed Then con.Open()
            com.ExecuteNonQuery()

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub


    Public Sub SaveText(ByVal tbl As String, ByVal KeyId As String, ByVal KeyValue As String, ByVal KeyId2 As String, ByVal KeyValue2 As String, ByVal KeyId3 As String, ByVal KeyValue3 As String, ByVal Field As String, ByVal Str As String)
        Try
            Dim com As New SqlClient.SqlCommand("if not exists (select * from " & tbl & " where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' and " & KeyId3 & "='" & KeyValue3 & "' and " & Field & "='" & Str & "')  insert into " & tbl & "( " & KeyId & "," & KeyId2 & "," & KeyId3 & "," & Field & ",MyGetDate) select '" & KeyValue & "','" & KeyValue2 & "','" & KeyValue3 & "',@MyImage,getdate()", con)
            com.CommandTimeout = 720000
            com.Parameters.Add("@MyImage", SqlDbType.VarChar)
            com.Parameters("@MyImage").Value = Str
            If con.State = ConnectionState.Closed Then con.Open()
            com.ExecuteNonQuery()

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub


    Public Sub SaveFile(ByVal tbl As String, ByVal KeyId As String, ByVal KeyValue As String, ByVal KeyId2 As String, ByVal KeyValue2 As String, ByVal KeyId3 As String, ByVal KeyValue3 As String, ByVal Field As String, ByVal Path As String)
        Try
            Dim ImageBytes As Byte() = File.ReadAllBytes(Path)
            Dim com As New SqlClient.SqlCommand("if exists (select * from " & tbl & " where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' and " & KeyId3 & "='" & KeyValue3 & "') update " & tbl & " set " & Field & "=@MyImage,MyGetDate=getdate() where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' and " & KeyId3 & "='" & KeyValue3 & "' else insert into " & tbl & "( " & KeyId & "," & KeyId2 & "," & KeyId3 & "," & Field & ",MyGetDate) select '" & KeyValue & "','" & KeyValue2 & "','" & KeyValue3 & "',@MyImage,getdate()", con)
            com.CommandTimeout = 720000
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            If con.State = ConnectionState.Closed Then con.Open()
            com.ExecuteNonQuery()

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub


    Public Sub SaveFile(ByVal tbl As String, ByVal KeyId As String, ByVal KeyValue As String, ByVal KeyId2 As String, ByVal KeyValue2 As String, ByVal KeyId3 As String, ByVal KeyValue3 As String, ByVal KeyId4 As String, ByVal KeyValue4 As String, ByVal Field As String, ByVal Path As String)
        Try
            Dim ImageBytes As Byte() = File.ReadAllBytes(Path)
            Dim com As New SqlClient.SqlCommand("if exists (select * from " & tbl & " where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' and " & KeyId3 & "='" & KeyValue3 & "' and " & KeyId4 & "='" & KeyValue4 & "') update " & tbl & " set " & Field & "=@MyImage,MyGetDate=getdate() where " & KeyId & "='" & KeyValue & "' and " & KeyId2 & "='" & KeyValue2 & "' and " & KeyId3 & "='" & KeyValue3 & "' and " & KeyId4 & "='" & KeyValue4 & "' else insert into " & tbl & "( " & KeyId & "," & KeyId2 & "," & KeyId3 & "," & KeyId4 & "," & Field & ",MyGetDate) select '" & KeyValue & "','" & KeyValue2 & "','" & KeyValue3 & "','" & KeyValue4 & "',@MyImage,getdate()", con)
            com.CommandTimeout = 720000
            com.Parameters.Add("@MyImage", SqlDbType.Image)
            com.Parameters("@MyImage").Value = ImageBytes
            If con.State = ConnectionState.Closed Then con.Open()
            com.ExecuteNonQuery()

        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub

    Public Function Round(Str As String) As Double
        Return Math.Round(Val(Str), 2, MidpointRounding.AwayFromZero)
    End Function

    Public Function Round(Value As Double) As Double
        Return Math.Round(Value, 2, MidpointRounding.AwayFromZero)
    End Function

    Public Sub AllowDorp(ByVal picBox As Controls.Image)
        picBox.AllowDrop = True
        AddHandler picBox.DragLeave, AddressOf pictureBox_DragDrop
        AddHandler picBox.DragEnter, AddressOf pictureBox_DragEnter
    End Sub
    Private Sub pictureBox_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
        e.Effects = e.AllowedEffects
    End Sub

    Private Sub pictureBox_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
        Dim ss() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim a As Windows.Media.ImageSource
        Dim dp As DependencyProperty
        a.SetCurrentValue(dp, ss(0))
        CType(sender, Controls.Image).Source = a
    End Sub

    Public Sub ShowMSG(ByVal MSG As String, Optional AutoHide As Boolean = False)
        Dim mm As New MSG
        mm.AutoHide = AutoHide
        mm.MSG = Application.Current.MainWindow.Resources.Item(MSG)
        If mm.MSG = "" Then mm.MSG = MSG
        mm.DelMsg = False
        mm.Topmost = True
        mm.ShowDialog()
    End Sub

    Dim MsgOnScreen As MSG
    Public Sub ShowMsgOnScreen(ByVal MSG As String)
        If Not MsgOnScreen Is Nothing Then
            MsgOnScreen.Close()
            MsgOnScreen = Nothing
        End If
        If MSG = "" Then Return
        MsgOnScreen = New MSG
        MsgOnScreen.WindowStyle = WindowStyle.ToolWindow
        MsgOnScreen.MSG = Application.Current.MainWindow.Resources.Item(MSG)
        If MsgOnScreen.MSG = "" Then MsgOnScreen.MSG = MSG
        MsgOnScreen.DelMsg = False
        MsgOnScreen.Topmost = True
        MsgOnScreen.Show()
        MsgOnScreen.Activate()
    End Sub

    Public Function ShowDeleteMSG(Optional MSG As String = "MsgDelete", Optional lblYes As String = "Yes", Optional lblNo As String = "No") As Boolean
        Dim mm As New MSG
        mm.MSG = Application.Current.MainWindow.Resources.Item(MSG)
        If mm.MSG = "" Then mm.MSG = MSG
        mm.DelMsg = True
        mm.lblYes = lblYes
        mm.lblNo = lblNo
        mm.ShowDialog()
        Return mm.Ok
    End Function

    Public Function ShowMSGNo(MSG As String, ByRef Value As String) As Boolean
        Dim mm As New MSGNo
        mm.MSG = Application.Current.MainWindow.Resources.Item(MSG)
        If mm.MSG = "" Then mm.MSG = MSG
        mm.DelMsg = True
        mm.Value = Value
        mm.ShowDialog()
        Value = mm.Value
        Return mm.Ok
    End Function

    Public Function Encrypt(ByVal plainText As String) As String

        Dim passPhrase As String
        Dim saltValue As String
        Dim hashAlgorithm As String
        Dim passwordIterations As Integer
        Dim initVector As String
        Dim keySize As Integer
        passPhrase = "KoKopr@se"        ' can be any string
        saltValue = "N!Les@t"        ' can be any string
        hashAlgorithm = "SHA1"             ' can be "MD5"
        passwordIterations = 5                  ' can be any number
        initVector = "@1B2h3D$e%F^g7H8" ' must be 16 bytes
        keySize = 256                ' can be 192 or 128

        Dim initVectorBytes As Byte()
        initVectorBytes = Encoding.ASCII.GetBytes(initVector)

        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.ASCII.GetBytes(saltValue)
        Dim plainTextBytes As Byte()
        plainTextBytes = Encoding.UTF8.GetBytes(plainText)
        Dim password As PasswordDeriveBytes
        password = New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)

        Dim keyBytes As Byte()
        keyBytes = password.GetBytes(keySize / 8)

        Dim symmetricKey As RijndaelManaged
        symmetricKey = New RijndaelManaged()
        symmetricKey.Mode = CipherMode.CBC
        Dim encryptor As ICryptoTransform
        encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
        Dim memoryStream As MemoryStream
        memoryStream = New MemoryStream()
        Dim cryptoStream As CryptoStream
        cryptoStream = New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)

        cryptoStream.FlushFinalBlock()

        Dim cipherTextBytes As Byte()
        cipherTextBytes = memoryStream.ToArray()

        memoryStream.Close()
        cryptoStream.Close()
        Return Convert.ToBase64String(cipherTextBytes)

    End Function
    Public Function Decrypt(ByVal cipherText As String) As String

        Dim passPhrase As String
        Dim saltValue As String
        Dim hashAlgorithm As String
        Dim passwordIterations As Integer
        Dim initVector As String
        Dim keySize As Integer

        passPhrase = "KoKopr@se"        ' can be any string
        saltValue = "N!Les@t"        ' can be any string
        hashAlgorithm = "SHA1"             ' can be "MD5"
        passwordIterations = 5                  ' can be any number
        initVector = "@1B2h3D$e%F^g7H8" ' must be 16 bytes
        keySize = 256                ' can be 192 or 128



        Dim initVectorBytes As Byte()
        initVectorBytes = Encoding.ASCII.GetBytes(initVector)

        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

        Dim cipherTextBytes As Byte()
        cipherTextBytes = Convert.FromBase64String(cipherText)
        Dim password As PasswordDeriveBytes
        password = New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)
        Dim keyBytes As Byte()
        keyBytes = password.GetBytes(keySize / 8)
        Dim symmetricKey As RijndaelManaged
        symmetricKey = New RijndaelManaged()
        symmetricKey.Mode = CipherMode.CBC
        Dim decryptor As ICryptoTransform
        decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)
        Dim memoryStream As MemoryStream
        memoryStream = New MemoryStream(cipherTextBytes)
        Dim cryptoStream As CryptoStream
        cryptoStream = New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)

        Dim plainTextBytes As Byte()
        ReDim plainTextBytes(cipherTextBytes.Length)
        Dim decryptedByteCount As Integer
        decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

        memoryStream.Close()
        cryptoStream.Close()
        Return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)

    End Function

    Sub SetNoImage(ByVal image As Controls.Image, Optional ByVal IsPerson As Boolean = False, Optional ByVal Ask As Boolean = False)
        If Not Ask OrElse ShowDeleteMSG("Are you sure you want to cancel this photo?") Then
            Dim bi3 As New BitmapImage
            bi3.BeginInit()
            bi3.UriSource = New Uri(IIf(IsPerson, "Images\no.photo.gif", "Images\cancel.png"), UriKind.RelativeOrAbsolute)
            bi3.EndInit()
            image.Source = bi3
        End If
    End Sub

    Sub SetImage(Img As ImageBrush, MyUri As String)
        Try
            Dim bi3 As New BitmapImage
            bi3.BeginInit()
            bi3.UriSource = New Uri("pack://application:,,,/SonacHR;component/Images/" & MyUri, UriKind.RelativeOrAbsolute)
            bi3.EndInit()
            Img.ImageSource = bi3
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
            Application.Current.Shutdown()
        End Try
    End Sub

    Sub SetImage(Img As Controls.Image, MyUri As String)
        Try
            Dim bi3 As New BitmapImage
            bi3.BeginInit()
            bi3.UriSource = New Uri("pack://application:,,,/SonacHR;component/Images/" & MyUri, UriKind.RelativeOrAbsolute)
            bi3.EndInit()
            Img.Source = bi3
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
            Application.Current.Shutdown()
        End Try
    End Sub

    Sub SetImageFromScanner(ByVal image As Controls.Image, Optional Shrink As Boolean = True)
        Try
            Dim bi3 As New BitmapImage
            bi3.BeginInit()
            Dim x As String = GetNewTempName("Scr")
            Dim WIACommonDialog As WIA.CommonDialog = New WIA.CommonDialog
            WIACommonDialog.ShowAcquireImage(WIA.WiaDeviceType.UnspecifiedDeviceType, WIA.WiaImageIntent.GrayscaleIntent, WIA.WiaImageBias.MinimizeSize, "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}", False, False, False).FileData.ImageFile.SaveFile(x)
            bi3.UriSource = New Uri(x, UriKind.RelativeOrAbsolute)
            If Shrink Then
                bi3.DecodePixelWidth = 300
                bi3.DecodePixelHeight = 300
            End If
            bi3.EndInit()
            image.Source = bi3
        Catch ex As Exception
            If ex.InnerException Is Nothing Then ShowMSG(ex.ToString().Trim) Else ShowMSG(ex.InnerException.Message)
        End Try
    End Sub

    Sub SetImage(ByVal image As Controls.Image, Optional Shrink As Boolean = True, Optional DecodePixelWidth As Integer = 300, Optional DecodePixelHeight As Integer = 300)
        Try
            Dim OFD As New System.Windows.Forms.OpenFileDialog
            OFD.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            If OFD.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim bi3 As New BitmapImage
                bi3.BeginInit()
                bi3.UriSource = New Uri(OFD.FileName, UriKind.RelativeOrAbsolute)
                If Shrink Then
                    bi3.DecodePixelWidth = DecodePixelWidth
                    bi3.DecodePixelHeight = DecodePixelHeight
                End If
                bi3.EndInit()
                image.Source = bi3
            End If
        Catch
        End Try
    End Sub

    Function GetNewTempName(ByVal FileName As String) As String
        If Not IO.Directory.Exists(Path) Then IO.Directory.CreateDirectory(Path)
        Dim i As Integer = 0, s As String = ""
        While True
            i += 1
            s = Path & i & "." & FileName.Split(".").Last
            If Not IO.File.Exists(s) Then
                Exit While
            End If
        End While
        Return s
    End Function

    Sub ClearTemp()
        Try
            'Dim rpt As New ReportViewer
            'rpt.Rpt = "Blank.rpt"
            'rpt.ReportViewer_Load(Nothing, Nothing)
            'rpt.ReportViewer_FormClosing(Nothing, Nothing)
        Catch ex As Exception
        End Try
        Try
            Try
                IO.Directory.Delete(Path, True)
            Catch ex As Exception
            End Try
            If Not IO.Directory.Exists(Path) Then Return
            For Each f As String In Directory.GetFiles(Path)
                Try
                    File.Delete(f)
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MyOpenCombobox(ByVal sender As Object, ByVal e As KeyEventArgs)
        Try
            If Not e.Key = System.Windows.Input.Key.Enter Then
                CType(sender, ComboBox).IsDropDownOpen = True
            End If
        Catch ex As Exception
        End Try
    End Sub

    Dim LopFilter As Boolean = False
    Private Sub MyFilterCombobox(ByVal sender As Object, ByVal e As KeyEventArgs)
        Try
            Return
            If LopFilter OrElse e.Key = Key.Enter OrElse e.Key = Key.Left OrElse e.Key = Key.Right Then Return

            If CType(sender, ComboBox).SelectedIndex = -1 Then
                Dim ss As String = sender.Text
                Try : CType(sender, ComboBox).SelectedIndex = 0 : Catch : End Try
                sender.Text = ss
            End If
            If e.Key = Key.Up OrElse e.Key = Key.Down Then Return
            LopFilter = True
            GoTo A
            If e.Key = Key.Down Then
                Try
                    If CType(sender, ComboBox).SelectedIndex = 0 Then
                        Dim ss As String = sender.Text
                        CType(sender, ComboBox).SelectedIndex += 1
                        CType(sender, ComboBox).SelectedIndex -= 1
                        sender.Text = ss
                    End If
                Catch ex As Exception
                End Try
            ElseIf e.Key = Key.Up Then
                Try
                    If CType(sender, ComboBox).SelectedIndex = 0 Then
                        Dim ss As String = sender.Text
                        CType(sender, ComboBox).SelectedIndex -= 1
                        CType(sender, ComboBox).SelectedIndex += 1
                        sender.Text = ss
                    End If
                Catch ex As Exception
                End Try
            End If
A:
            Dim s As String = sender.Text
            CType(sender.ItemsSource, DataView).RowFilter = "Name like '" & sender.Text & "%'"
            If Not CType(sender, ComboBox).IsDropDownOpen Then CType(sender, ComboBox).IsDropDownOpen = True

            sender.Text = CType(CType(sender, ComboBox).SelectedItem, DataRowView)("Name")
        Catch ex As Exception
        End Try
        LopFilter = False
    End Sub

    Private Sub MyGotFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
        CType(sender, ComboBox).IsDropDownOpen = True
    End Sub


    Public Sub SetPickerDate(DP As DatePicker, V As Object)
        If V.ToString = "01/01/1900 12:00:00 AM" Then
            DP.SelectedDate = Nothing
        Else
            Try
                DP.SelectedDate = V.ToString
            Catch ex As Exception
                DP.SelectedDate = Nothing
            End Try
        End If
    End Sub

    Sub ResetComboboxContent(cbo As ComboBox)
        Dim b As Boolean = True
        For i As Integer = 0 To cbo.Items.Count - 1
            Try
                CType(cbo.Items(i), ComboBoxItem).SetResourceReference(ComboBoxItem.BackgroundProperty, "BgDelete")
                If Md.DictionaryCurrent.Item(CType(cbo.Items(i), ComboBoxItem).Content) <> "" Then
                    CType(cbo.Items(i), ComboBoxItem).SetResourceReference(ComboBoxItem.ContentProperty, CType(cbo.Items(i), ComboBoxItem).Content)
                End If
            Catch ex As Exception
            End Try
        Next

    End Sub

    Sub SetColor(C As Object)
        Try
            C.Background = System.Windows.Media.Brushes.Red
            If TypeOf (C) Is Grid Then
                For Each CC In CType(C, Grid).Children
                    If TypeOf (CC) Is Border OrElse TypeOf (CC) Is Controls.Image OrElse TypeOf (CC) Is System.Windows.Forms.Integration.WindowsFormsHost Then
                        Continue For
                    ElseIf TypeOf (CC) Is Label OrElse TypeOf (CC) Is TextBox OrElse TypeOf (CC) Is DatePicker OrElse TypeOf (CC) Is ComboBox OrElse TypeOf (CC) Is Button OrElse TypeOf (CC) Is PasswordBox Then
                        CType(CC, Control).Background = System.Windows.Media.Brushes.White
                        CType(CC, Control).Foreground = System.Windows.Media.Brushes.Red

                    ElseIf TypeOf (CC) Is RadioButton OrElse TypeOf (CC) Is CheckBox Then
                        CType(CC, Control).Background = System.Windows.Media.Brushes.Red
                        CType(CC, Control).Foreground = System.Windows.Media.Brushes.White

                    ElseIf TypeOf (CC) Is Grid OrElse TypeOf (CC) Is GroupBox OrElse TypeOf (CC) Is TabControl OrElse TypeOf (CC) Is TabItem OrElse TypeOf (CC) Is WrapPanel OrElse TypeOf (CC) Is Border OrElse TypeOf (CC) Is DockPanel Then
                        SetColor(CC)
                    Else
                        CType(CC, Control).Background = System.Windows.Media.Brushes.Red
                        CType(CC, Control).Foreground = System.Windows.Media.Brushes.White
                    End If
                Next
            ElseIf TypeOf (C) Is TabControl Then
                For Each CC In CType(C, TabControl).Items
                    SetColor(CC)
                Next
            ElseIf TypeOf (C) Is Border Then
                SetColor(CType(C, Border).Child)
            ElseIf TypeOf (C) Is TabItem Then
                SetColor(CType(C, TabItem).Content)
            ElseIf TypeOf (C) Is GroupBox Then
                SetColor(CType(C, GroupBox).Content)
            ElseIf TypeOf (C) Is WrapPanel Then
                For Each CC In CType(C, WrapPanel).Children
                    SetColor(CC)
                Next
            ElseIf TypeOf (C) Is DockPanel Then
                For Each CC In CType(C, DockPanel).Children
                    SetColor(CC)
                Next
            ElseIf TypeOf (C) Is TabItem Then
                SetColor(CType(C, TabItem).Content)
            ElseIf TypeOf (C) Is GroupBox Then
                SetColor(CType(C, GroupBox).Content)
            Else
                C.Background = System.Windows.Media.Brushes.Red
                C.Foreground = System.Windows.Media.Brushes.White
            End If

            If TypeOf (C) Is UserControl Then
                SetColor(CType(C, UserControl).Content)
            End If


        Catch ex As Exception
        End Try
    End Sub

    Sub OpenWord(txt As TextBox)
        'Try
        '    Dim doc As Word.Document
        '    Dim wordApp As New Word.Application
        '    Dim allText As String
        '    Dim ofd As New Forms.OpenFileDialog With {.Filter = "Word Files(*.Doc;*.Docx)|*.Doc;*.Docx|All files (*.*)|*.*"}
        '    If Not ofd.ShowDialog = Forms.DialogResult.OK Then Return
        '    doc = wordApp.Documents.Open(ofd.FileName)
        '    allText = doc.Range.Text()
        '    doc.Close()
        '    txt.Text = allText
        'Catch
        '    'error            
        'End Try
    End Sub

    Public Sub GetCurrent()
        Try
            Dim dt As New DataTable

            dt = ExecuteAdapter("GetCurrent")

            If dt.Rows.Count > 0 Then
                Md.CurrentDate = dt.Rows(0)("CurrentDate").ToString
                Md.CurrentShiftId = Val(dt.Rows(0)("CurrentShift").ToString)
                Md.CurrentShiftName = dt.Rows(0)("CurrentShiftName").ToString
                Md.CompanyName = dt.Rows(0)("CompanyName").ToString
            End If
        Catch ex As Exception
        End Try
    End Sub


    Public Sub Addcontrol_MouseDoubleClick(MyControls As Controls.Control())
        For i As Integer = 0 To MyControls.Length - 1
            RemoveHandler MyControls(i).MouseDoubleClick, AddressOf control_MouseDoubleClick
            AddHandler MyControls(i).MouseDoubleClick, AddressOf control_MouseDoubleClick
        Next
    End Sub

    Private Sub control_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.F1) With {.RoutedEvent = Keyboard.KeyUpEvent})
    End Sub

    Function FormatDate(MyDate As DateTime) As String
        Try
            Return (String.Format("{0:yyyy/MM/dd}", MyDate).Replace("-", "/"))
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Function GetControlPosit(myControl As Control) As System.Windows.Point
        Dim locationToScreen As System.Windows.Point = myControl.PointToScreen(New System.Windows.Point(0, 0))
        Dim source As PresentationSource = PresentationSource.FromVisual(myControl)
        Return source.CompositionTarget.TransformFromDevice.Transform(locationToScreen)
    End Function

    Dim MyEditControls() As Control = {}
    Sub TestSecurity(frm As Object, EditControls() As Control, DeleteControls() As Control, NavigateControls() As Control, PrintControls() As Control)
        MyEditControls = EditControls
        If TypeOf frm.Parent Is MyPage Then
            'For Each c As Control In EditControls
            '    c.Visibility = IIf(CType(frm.Parent, MyPage).MySecurityType.AllowEdit, c.Visibility, Visibility.Hidden)
            'Next
            For Each c As Control In DeleteControls
                c.Visibility = IIf(CType(frm.Parent, MyPage).MySecurityType.AllowDelete, c.Visibility, Visibility.Hidden)
            Next
            For Each c As Control In NavigateControls
                c.Visibility = IIf(CType(frm.Parent, MyPage).MySecurityType.AllowNavigate, c.Visibility, Visibility.Hidden)
            Next
            For Each c As Control In PrintControls
                c.Visibility = IIf(CType(frm.Parent, MyPage).MySecurityType.AllowPrint, c.Visibility, Visibility.Hidden)
            Next
        ElseIf TypeOf frm.Parent Is MyWindow Then
            'For Each c As Control In EditControls
            '    c.Visibility = IIf(CType(frm.Parent, MyWindow).MySecurityType.AllowEdit, c.Visibility, Visibility.Hidden)
            'Next
            For Each c As Control In DeleteControls
                c.Visibility = IIf(CType(frm.Parent, MyWindow).MySecurityType.AllowDelete, c.Visibility, Visibility.Hidden)
            Next
            For Each c As Control In NavigateControls
                c.Visibility = IIf(CType(frm.Parent, MyWindow).MySecurityType.AllowNavigate, c.Visibility, Visibility.Hidden)
            Next
            For Each c As Control In PrintControls
                c.Visibility = IIf(CType(frm.Parent, MyWindow).MySecurityType.AllowPrint, c.Visibility, Visibility.Hidden)
            Next
        End If
    End Sub

    Sub MakeGridCombo(G As MyGrid, cboHeader As String, cboName As String, statement As String, Optional MyFillWeight As Double = 300)
        Dim cbo As New Forms.DataGridViewComboBoxColumn
        cbo.HeaderText = cboHeader
        cbo.Name = cboName
        FillCombo(statement, cbo)
        cbo.AutoComplete = True
        cbo.DropDownWidth = MyFillWeight
        cbo.DisplayStyle = Forms.DataGridViewComboBoxDisplayStyle.ComboBox
        G.Columns.Add(cbo)
        G.Columns(cboName).FillWeight = MyFillWeight
    End Sub

    Sub SubAccNoLostFocus(AccNo As TextBox, SubAccNo As TextBox, SubAccName As TextBox)
        If Val(AccNo.Text) = 0 Or Not SubAccNo.IsEnabled Then
            SubAccNo.Clear()
            SubAccName.Clear()
            Return
        End If

        Dim dt As DataTable = ExecuteAdapter("select * from LinkFile where Id=(select C.LinkFile from Chart C where C.Id='" & AccNo.Text & "')")
        LostFocus(SubAccNo, SubAccName, "select Name from " & dt.Rows(0)("TableName") & " where Id=" & SubAccNo.Text.Trim() & " and AccNo='" & AccNo.Text & "'")
    End Sub


    Public Function TestDateValidity(MyDayDate As DatePicker) As Boolean
        If Not Md.AllowPreviousYearsForNonManager And Not Md.Manager Then
            Dim d As Date = MyGetDate()
            If MyDayDate.SelectedDate.Value.Year <> d.Year Then
                ShowMSG("التاريخ غير صحيح")
                'MyDayDate.SelectedDate = d
                Return False
            End If
        End If
        Return True
    End Function

    Sub SetMySecurityType(frm As MyWindow, Id As Integer)
        frm.MySecurityType.AllowEdit = dtLevelsMenuitems.Select("Id=" & Id).ToList(0)("AllowEdit") = 1
        frm.MySecurityType.AllowDelete = dtLevelsMenuitems.Select("Id=" & Id).ToList(0)("AllowDelete") = 1
        frm.MySecurityType.AllowNavigate = dtLevelsMenuitems.Select("Id=" & Id).ToList(0)("AllowNavigate") = 1
        frm.MySecurityType.AllowPrint = dtLevelsMenuitems.Select("Id=" & Id).ToList(0)("AllowPrint") = 1
    End Sub

    Sub AddThousandSeparator(col As Forms.DataGridViewColumn)
        col.ValueType = GetType(Double) ' or Integer, Single, etc.
        col.DefaultCellStyle.Format = "###,###,###.##" ' N(zero) for no digits to the right of the 
    End Sub

End Class
