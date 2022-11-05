Imports System.Data

Public Class MyImages

    Public k1 As String = "k1"
    Public k2 As String = "k2"
    Public k3 As String = "k3"

    Public v1 As String = ""
    Public v2 As String = ""
    Public v3 As String = ""

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Dim m As MainWindow = Application.Current.MainWindow
    Public Flag As Integer = 0
    Public WithImage As Boolean = False

    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        dt = bm.ExecuteAdapter("select Notes from MyImages where " & k1 & "='" & v1 & "' and " & k2 & "='" & v2 & "' and " & k3 & "='" & v3 & "'")
        If dt.Rows.Count > 0 Then
            txtName2.Text = dt.Rows(0)(0).ToString
            bm.GetImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img1", Img1)
            bm.GetImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img2", Img2)
            bm.GetImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img3", Img3)
            bm.GetImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img4", Img4)
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click, btnPrint.Click
        bm.ExecuteNonQuery("delete MyImages where " & k1 & "='" & v1 & "' and " & k2 & "='" & v2 & "' and " & k3 & "='" & v3 & "'")
        txtName2.Text = txtName2.Text.Replace("'", "''")
        bm.ExecuteNonQuery("insert MyImages (Notes," & k1 & "," & k2 & "," & k3 & ") select '" & txtName2.Text & "','" & v1 & "','" & v2 & "','" & v3 & "'")

        bm.SaveImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img1", Img1)
        bm.SaveImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img2", Img2)
        bm.SaveImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img3", Img3)
        bm.SaveImage("MyImages", New String() {k1, k2, k3}, New String() {v1, v2, v3}, "Img4", Img4)
        If sender Is btnSave Then CType(Parent, Window).Close()
        If sender Is btnPrint Then
            Dim rpt As New ReportViewer
            rpt.Header = CType(Parent, Window).Title
            rpt.paraname = New String() {"@k1", "@k2", "@k3"}
            rpt.paravalue = New String() {v1, v2, v3}
            rpt.Rpt = "MyImages.rpt"
            rpt.Show()

        End If
    End Sub


    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")

    End Sub


    Private Sub btnSetImage_Copy_Click(sender As Object, e As RoutedEventArgs) Handles btnSetImage_Copy.Click
        bm.SetImage(Img1)
    End Sub

    Private Sub btnSetImage_Copy1_Click(sender As Object, e As RoutedEventArgs) Handles btnSetImage_Copy1.Click
        bm.SetImage(Img2)
    End Sub

    Private Sub btnSetImage_Copy2_Click(sender As Object, e As RoutedEventArgs) Handles btnSetImage_Copy2.Click
        bm.SetImage(Img3)
    End Sub

    Private Sub btnSetImage_Copy3_Click(sender As Object, e As RoutedEventArgs) Handles btnSetImage_Copy3.Click
        bm.SetImage(Img4)
    End Sub

    Private Sub btnSetNoImage_Copy_Click(sender As Object, e As RoutedEventArgs) Handles btnSetNoImage_Copy.Click
        bm.SetNoImage(Img1, , True)
    End Sub

    Private Sub btnSetNoImage_Copy1_Click(sender As Object, e As RoutedEventArgs) Handles btnSetNoImage_Copy1.Click
        bm.SetNoImage(Img2, , True)
    End Sub

    Private Sub btnSetNoImage_Copy2_Click(sender As Object, e As RoutedEventArgs) Handles btnSetNoImage_Copy2.Click
        bm.SetNoImage(Img3, , True)
    End Sub

    Private Sub btnSetNoImage_Copy3_Click(sender As Object, e As RoutedEventArgs) Handles btnSetNoImage_Copy3.Click
        bm.SetNoImage(Img4, , True)
    End Sub


    Private Sub btnOpen_Click(sender As Object, e As RoutedEventArgs) Handles btnOpen.Click
        bm.OpenWord(txtName2)
    End Sub
End Class
