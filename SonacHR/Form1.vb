Imports System.Data

Public Class Form1

    Public Password As String = ""

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If Not Exists AndAlso My.Computer.Name.ToUpper <> "PHYSICS-PC" Then
            Dim p As New PCs
            p.TextBox1.Text = s
            p.TextBox1.SelectAll()
            p.TextBox1.Focus()
            p.ShowDialog()
            If Not Md.Demo Then
                Application.Current.Shutdown()
            End If
        End If
    End Sub
    Dim s As String = ""
    Dim Exists As Boolean = False
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim bm As New BasicMethods
        s = bm.ProtectionSerial()
        Dim s2 = bm.Encrypt(s)
        Dim dt As DataTable = bm.ExecuteAdapter("select * from PCs")
        For i As Integer = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("name") = s2 Then
                Exists = True
                Exit For
            End If
        Next

        'Exists = bm.IF_Exists("select * from PCs where Name='" & bm.Encrypt(s) & "'")
    End Sub
     
End Class
