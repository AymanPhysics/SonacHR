Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.ComponentModel

Public Class Attachments
    Public Flag As Integer = 1
    Dim bm As New BasicMethods

    Dim WithEvents BackgroundWorker1 As New BackgroundWorker

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAttach.Click
        If ComboBox1.SelectedIndex < 1 Then
            bm.ShowMSG("Please, Select a Type")
            ComboBox1.Focus()
            Return
        End If
        Dim o As New Forms.OpenFileDialog
        o.Multiselect = True
        If o.ShowDialog = Forms.DialogResult.OK Then
            ProgressBar1.Value = 0
            ProgressBar1.Maximum = o.FileNames.Length
            ProgressBar1.Visibility = Visibility.Visible
            For i As Integer = 0 To o.FileNames.Length - 1
                bm.SaveFile("FileStore", "Type", ComboBox1.SelectedValue.ToString, "FileName", (o.FileNames(i).Split("\"))(o.FileNames(i).Split("\").Length - 1), "ImageData", o.FileNames(i))
                ProgressBar1.Value += 1
            Next
            ProgressBar1.Visibility = Visibility.Hidden
        End If
        LoadTree()
    End Sub

    Private Sub UploadFiles_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.TestSecurity(Me, {btnAttach}, {btnDelete}, {}, {btnDownload})
        LoadResource()
        bm.FillCombo("AttachmentTypes", ComboBox1, "")
        LoadTree()
    End Sub

    Private Sub TreeView1_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TreeView1.MouseDoubleClick
        Button4_Click(Nothing, Nothing)
    End Sub

    Private Sub LoadTree()
        TreeView1.Items.Clear()
        TreeView1.Items.Add(New TreeViewItem With {.Header = Resources.Item("Contents")})
        Dim pdf As DataTable = bm.ExecuteAdapter("select Type,FileName from FileStore order by FileName")
        Dim dt As DataTable = bm.ExecuteAdapter("select Id,Name from AttachmentTypes")
        TreeView1.Items(0).Tag = ""
        TreeView1.Items(0).FontSize = 20
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim nn As New TreeViewItem
            nn.Foreground = Brushes.DarkRed
            nn.FontSize = 18
            nn.Name = "Node_" & dt.Rows(i)(0).ToString
            nn.Tag = dt.Rows(i)(0).ToString
            nn.Header = dt.Rows(i)(1).ToString
            TreeView1.Items(0).Items.Add(nn)
            Dim Dr() As DataRow = pdf.Select("Type='" & dt.Rows(i)(0).ToString & "'")
            For ii As Integer = 0 To Dr.Length - 1
                Dim nn2 As New TreeViewItem
                nn2.FontSize = 16
                'nn2.Name = "Node_" & Dr(ii)("FileName").ToString.Replace(".", "").ToString.Replace(" ", "")
                nn2.Tag = Dr(ii)("FileName")
                nn2.Header = Dr(ii)("FileName")
                TreeView1.Items(0).Items(i).Items.Add(nn2)
            Next
            nn.IsExpanded = True
        Next
        CType(TreeView1.Items(0), TreeViewItem).IsExpanded = True
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If CType(TreeView1.SelectedItem, TreeViewItem).FontSize = 16 Then
                If bm.ShowDeleteMSG("Are you sure you want to Delete this file?") Then
                    bm.ExecuteNonQuery("delete from FileStore where Type='" & TreeView1.SelectedItem.Parent.Tag & "' and FileName='" & TreeView1.SelectedItem.Header & "'")
                    LoadTree()
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TreeView1_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles TreeView1.KeyDown
        If e.Key = Key.Delete Then
            Button3_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDownload.Click
        MyImagedata = Nothing
        If CType(TreeView1.SelectedItem, TreeViewItem).FontSize <> 16 Then Return
        Dim s As New Forms.SaveFileDialog With {.Filter = "All files (*.*)|*.*"}
        s.Filter = "All files (*.*)|*.*"
        s.FileName = CType(TreeView1.SelectedItem, TreeViewItem).Header

        If IsNothing(sender) Then
            MyBath = bm.GetNewTempName(s.FileName)
        Else
            If Not s.ShowDialog = Forms.DialogResult.OK Then Return
            MyBath = s.FileName
        End If

        btnDownload.IsEnabled = False
        F1 = CType(TreeView1.SelectedItem.Parent, TreeViewItem).Tag
        F2 = CType(TreeView1.SelectedItem, TreeViewItem).Tag
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Dim MyImagedata() As Byte
    Dim MyBath As String = "", F1 As String = "", F2 As String = ""
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            Dim myCommand As SqlClient.SqlCommand
            myCommand = New SqlClient.SqlCommand("select ImageData from FileStore where Type='" & F1 & "' and FileName='" & F2 & "'", con)
            If con.State <> ConnectionState.Open Then con.Open()
            MyImagedata = CType(myCommand.ExecuteScalar(), Byte())
        Catch ex As Exception
        End Try
        con.Close()
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            File.WriteAllBytes(MyBath, MyImagedata)
            Process.Start(MyBath)
        Catch ex As Exception
        End Try
        btnDownload.IsEnabled = True
    End Sub

    Private Sub LoadResource()
        btnAttach.SetResourceReference(Button.ContentProperty, "Attach")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnDownload.SetResourceReference(Button.ContentProperty, "Download")

        lblMain.SetResourceReference(Label.ContentProperty, "Type")
    End Sub

End Class
