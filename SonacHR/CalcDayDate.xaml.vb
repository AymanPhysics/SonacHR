Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Windows.Forms

Public Class CalcDayDate
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Hdr As String = ""
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If ToDate.SelectedDate Is Nothing Then Return

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                MyToDate = ToDate.SelectedDate
                Button2.IsEnabled = False
                BackgroundWorker1.RunWorkerAsync()
                Return

        End Select

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Dim MyNow As DateTime = bm.MyGetDate()
        ToDate.SelectedDate = MyNow.Date

    End Sub
    Private Sub LoadResource()

        Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Calculate")
        lblToDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "DayDate")

    End Sub


    Dim MyToDate As Date
    Dim WithEvents BackgroundWorker1 As New System.ComponentModel.BackgroundWorker
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Select Case Flag
            Case 1
                bm.ExecuteNonQuery("CalcClinicsHistory", New String() {"DayDate"}, New String() {bm.ToStrDate(MyToDate)})
        End Select
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        bm.ShowMSG("Done Successfuly")
        Button2.IsEnabled = True
    End Sub



End Class