Imports System.Data
Imports System.Windows.Controls.Primitives

Public Class Login

    Dim bm As New BasicMethods
    Public Flag As Integer = 1
    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLogin.Click

        If Username.Text.Trim = "" Or Username.Text.Trim = "-" Or Username.SelectedIndex < 0 Then
            Username.Focus()
            Return
        End If

        If Password.Password.Trim = "" Then
            Password.Focus()
            Return
        End If

        Dim dt As DataTable

        If Not bm.StopPro() Then Return
        Dim paraname() As String = {"Id", "Password"}
        Dim paravalue() As String = {Username.SelectedValue.ToString, bm.Encrypt(Password.Password)}
        dt = bm.ExecuteAdapter("TestLogin", paraname, paravalue)
        If dt.Rows.Count = 0 Then
            bm.ShowMSG("Invalid Password ...")
            Password.Focus()
            Password.SelectAll()
            Exit Sub
        End If
        Md.UserName = Username.SelectedValue.ToString
        Md.ArName = dt.Rows(0)("Name").ToString
        Md.EnName = dt.Rows(0)("EnName").ToString
        Md.LevelId = dt.Rows(0)("LevelId").ToString
        Md.Password = bm.Decrypt(dt.Rows(0)("Password").ToString)
        Md.CompanyName = dt.Rows(0)("CompanyName").ToString
        Md.CompanyTel = dt.Rows(0)("CompanyTel").ToString
        Md.Manager = IIf(dt.Rows(0)("Manager").ToString() = "1", True, False)
        Md.Nurse = dt.Rows(0)("Nurse").ToString
        Md.Receptionist = IIf(dt.Rows(0)("Receptionist").ToString() = "1", True, False)
        Md.DefaultStore = dt.Rows(0)("DefaultStore")
        Md.DefaultSave = dt.Rows(0)("DefaultSave")
        Md.DefaultBank = dt.Rows(0)("DefaultBank")
        Select Case Md.MyProjectType
            Case Else
                'bm.TextToSpeech(Md.ArName)
        End Select

        If Md.Manager Then
            'bm.BackupAndSendEMail()
        End If

        Dim m As MainWindow = Application.Current.MainWindow
        m.LoadTabs(New MainPage)
        IsLogedIn = True

        SaveSetting("SonacHR", "Login", "AccYear", AccYear.Text)
        SaveSetting("SonacHR", "Login", "Username", Username.Text)
    End Sub

    Private Sub Login_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()

        CompanyLogo.Visibility = Windows.Visibility.Hidden
        
        Dim dt As New DataTable("ddtt")
        dt.Columns.Add("Id")
        dt.Columns.Add("Name")
        'For Each file In IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath)
        '    If file.ToLower.EndsWith(".udl") AndAlso Not file.ToLower.EndsWith("connect.udl") Then
        '        dt.Rows.Add(file.ToLower.Split("\").Last.Replace(".udl", ""), file.ToLower.Split("\").Last.Replace(".udl", ""))
        '    End If
        'Next
        For Each file In IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath)
            If file.ToLower.EndsWith(".udldll") Then
                dt.Rows.Add(file.ToLower.Split("\").Last.Replace(".udldll", ""), file.ToLower.Split("\").Last.Replace(".udldll", ""))
            End If
        Next
        bm.FillCombo(dt, AccYear)

        If dt.Rows.Count <= 1 Then
            lblAccYear.Visibility = Windows.Visibility.Hidden
            AccYear.Visibility = Windows.Visibility.Hidden
            LoadEmployees()
        Else
            lblAccYear.Visibility = Windows.Visibility.Visible
            AccYear.Visibility = Windows.Visibility.Visible

            Dim MyAccYear As String = GetSetting("SonacHR", "Login", "AccYear")
            If MyAccYear = "" Then
                AccYear.SelectedIndex = AccYear.Items.Count - 1
            Else
                AccYear.Text = MyAccYear
            End If
        End If


        LoadResource()
    End Sub

    Private Sub LoadResource()
        btnLogin.SetResourceReference(Button.ContentProperty, "Login")
        lblUsername.SetResourceReference(Label.ContentProperty, "Username")
        lblPassword.SetResourceReference(Label.ContentProperty, "Password")
        lblAccYear.SetResourceReference(Label.ContentProperty, "AccYear")
    End Sub

    Private Sub LoadEmployees()
        Dim CboName As String = Resources.Item("CboName")
        bm.FillCombo("select Id," & CboName & " Name from Employees where SystemUser='1' and Stopped='0' union select 0 Id,'-' Name order by Name", Username)
        Username.Text = GetSetting("SonacHR", "Login", "Username")
        If Username.Text.Trim = "" Then
            Username.Focus()
        Else
            Password.Focus()
        End If
    End Sub


    Private Sub AccYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles AccYear.SelectionChanged
        Try
            Dim m As MainWindow = Application.Current.MainWindow
            Md.UdlName = AccYear.SelectedValue
            If con.State = ConnectionState.Open Then con.Close()
            Dim x As Integer = 0
            While Not m.LoadConnection()
                If x > 0 Then
                    Application.Current.Shutdown()
                    Return
                End If
                Dim frm As New EditConnection
                frm.Show()
                frm.Hide()
                frm.AccYear.Text = Md.UdlName
                frm.ServerName.Text = con.DataSource
                frm.Database.Text = con.Database
                frm.ShowDialog()
                x += 1
            End While
            
            bm = New BasicMethods
            LoadEmployees()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnEditCurrentConnection_Click(sender As Object, e As RoutedEventArgs) Handles btnEditCurrentConnection.Click
        Dim frm As New EditConnection
        frm.Show()
        frm.Hide()
        frm.AccYear.Text = Md.UdlName
        frm.ShowDialog()
        Login_Loaded(Nothing, Nothing)
        AccYear.Text = Md.UdlName
        AccYear_SelectionChanged(Nothing, Nothing)
    End Sub

End Class
