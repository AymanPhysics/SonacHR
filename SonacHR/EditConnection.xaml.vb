Imports System
Imports System.Threading
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.AccessControl

Public Class EditConnection
    Dim bm As New BasicMethods
    Public Ok As Boolean

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        Dim dt As New DataTable("ddtt")
        dt.Columns.Add("Id")
        dt.Columns.Add("Name")
        For Each file In IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath)
            If file.ToLower.EndsWith(".udldll") Then
                dt.Rows.Add(file.ToLower.Split("\").Last.Replace(".udldll", ""), file.ToLower.Split("\").Last.Replace(".udldll", ""))
            End If
        Next
        bm.FillCombo(dt, AccYear)


        'If bm.TestIsLoaded(Me) Then Return
        Ok = False
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        Ok = False
        Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Try
            Dim con As New SqlConnection(ConnectionString)
            con.Open()
            con.Close()

            Dim m As MainWindow = Application.Current.MainWindow

            Try
                Md.UdlName = AccYear.Text
                Dim st As New StreamWriter(AccYear.Text & ".udldll")
                st.WriteLine(bm.Encrypt(ConnectionString))
                st.Flush()
                st.Close()
                st.Dispose()
            Catch ex As Exception
            End Try

            m.LoadConnection()

            Ok = True
            Close()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ServerName_DropDownOpened(sender As Object, e As EventArgs) Handles ServerName.DropDownOpened
        ServerName.Items.Clear()
        Dim table As DataTable = SqlDataSourceEnumerator.Instance.GetDataSources()
        For Each server As DataRow In table.Rows
            ServerName.Items.Add(server(table.Columns("ServerName")).ToString())
        Next
    End Sub

    Private Sub Database_DropDownOpened(sender As Object, e As EventArgs) Handles Database.DropDownOpened
        Database.Items.Clear()
        Dim Adp As New SqlDataAdapter("select name from sys.databases Where [name] NOT IN ('master', 'tempdb', 'model', 'msdb') order by name", ConnectionString("master"))
        Dim dt As New DataTable()
        Try
            Adp.Fill(dt)
        Catch ex As Exception
        End Try
        For i As Integer = 0 To dt.Rows.Count - 1
            Database.Items.Add(dt.Rows(i)(0).ToString())
        Next
    End Sub

    Function ConnectionString(Optional db As String = "") As String
        If db = "" Then db = Database.Text
        Return "Persist Security Info=True;Data Source=" & ServerName.Text & ";User ID=" & Username.Text & ";Password=" & Password.Password & ";Initial Catalog=" & db & ";"
    End Function

    Private Sub AccYear_LostFocus(sender As Object, e As RoutedEventArgs) Handles AccYear.LostFocus
        Try
            Dim st As New StreamReader(AccYear.Text & ".udldll")
            Dim s As String = st.ReadLine
            st.Close()
            st.Dispose()
            con.ConnectionString = bm.Decrypt(s)
            Dim cb As New SqlClient.SqlConnectionStringBuilder(con.ConnectionString)

            ServerName.Text = cb.DataSource
            Database.Text = cb.InitialCatalog

        Catch
        End Try
    End Sub

    Private Sub btnAttach_Click(sender As Object, e As RoutedEventArgs) Handles btnAttach.Click
        Try

            Dim mdf As New Forms.OpenFileDialog With {.Filter = "MDF files|*.mdf", .Title = "Data File"}
            Dim ldf As New Forms.OpenFileDialog With {.Filter = "LDF files|*.ldf", .Title = "Log File"}
            If Not mdf.ShowDialog() = Forms.DialogResult.OK Then
                Return
            End If
            If Not ldf.ShowDialog() = Forms.DialogResult.OK Then
                Return
            End If

            Dim FS As FileSecurity = File.GetAccessControl(mdf.FileName)
            FS.AddAccessRule(New FileSystemAccessRule("EVERYONE", FileSystemRights.FullControl, AccessControlType.Allow))
            File.SetAccessControl(mdf.FileName, FS)
            Dim FS2 As FileSecurity = File.GetAccessControl(ldf.FileName)
            FS2.AddAccessRule(New FileSystemAccessRule("EVERYONE", FileSystemRights.FullControl, AccessControlType.Allow))
            File.SetAccessControl(ldf.FileName, FS2)

            Dim myCon0 As New SqlConnectionStringBuilder
            myCon0.DataSource = ServerName.Text
            myCon0.InitialCatalog = "master"
            myCon0.UserID = Username.Text
            myCon0.Password = Password.Password

            Dim myCon As New SqlConnection(myCon0.ConnectionString)
            Try
                Dim myCommand As SqlClient.SqlCommand
                myCommand = New SqlClient.SqlCommand("ALTER LOGIN [sa] WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF", myCon)
                If myCon.State <> ConnectionState.Open Then myCon.Open()
                myCommand.ExecuteNonQuery()
            Catch ex As Exception
                bm.ShowMSG(ex.Message)
            End Try

            Try
                Dim myCommand As SqlClient.SqlCommand
                myCommand = New SqlClient.SqlCommand("master.dbo.sp_attach_db @dbname = N'" & mdf.FileName.Split("\").Last.Split(".").First & "',@filename1 = N'" & mdf.FileName & "',@filename2 = N'" & ldf.FileName & "'", myCon)
                If myCon.State <> ConnectionState.Open Then myCon.Open()
                myCommand.ExecuteNonQuery()
            Catch ex As Exception
                bm.ShowMSG(ex.Message)
            End Try
            myCon.Close()
            Database.Text = mdf.FileName.Split("\").Last.Split(".").First

        Catch ex As Exception
            bm.ShowMSG(ex.Message)
        End Try
    End Sub
End Class
