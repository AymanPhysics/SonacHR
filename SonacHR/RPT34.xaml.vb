Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO

Public Class RPT34
    Dim bm As New BasicMethods
    Dim dt As New DataTable

    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                rpt.Rpt = "InvoicesExternalDoctors.rpt"
        End Select

        rpt.paraname = New String() {"@EmpId", "@InEmpId", "@FromDate", "@ToDate", "Header", "IsDetailed"}
        rpt.paravalue = New String() {Val(RefereId.Text), Val(EmpId.Text), FromDate.SelectedDate, ToDate.SelectedDate, CType(Parent, Page).Title, IIf(IsDetailed.IsChecked, 1, 0)}
        rpt.Show()

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        bm.Addcontrol_MouseDoubleClick({RefereId, EmpId})
        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        IsDetailed.Visibility = Visibility.Hidden
        IsDetailed2.Visibility = Visibility.Hidden
      

    End Sub
    Private Sub LoadResource()


        lblInEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        lblEmpId.SetResourceReference(Label.ContentProperty, "Doctor")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")
        Button2.SetResourceReference(Button.ContentProperty, "View Report")
         
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles RefereId.KeyDown, EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub
    Private Sub RefereId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles RefereId.KeyUp
        If bm.ShowHelp("ExternalDoctors", RefereId, RefereName, e, "select cast(Id as varchar(100)) Id,Name from ExternalDoctors") Then
            RefereId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub RefereId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles RefereId.LostFocus
        bm.LostFocus(RefereId, RefereName, "select Name from ExternalDoctors where Id=" & RefereId.Text.Trim())
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        If bm.ShowHelp("Employees", EmpId, EmpName, e, "select cast(Id as varchar(100)) Id,Name from Employees where Doctor=0") Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        bm.LostFocus(EmpId, EmpName, "select Name from Employees where Doctor=0 and Id=" & EmpId.Text.Trim())
    End Sub

    Dim MyFromDate As Date, MyToDate As Date
    Dim WithEvents BackgroundWorker1 As New ComponentModel.BackgroundWorker
    Dim FIFOTABLE As DataTable
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Select Case Flag
           
        End Select
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        
        bm.ShowMSG("Done Successfuly")
        Button2.IsEnabled = True
    End Sub

    Private Sub IsDetailed_Checked(sender As Object, e As RoutedEventArgs) Handles IsDetailed.Checked
        IsDetailed2.IsChecked = False
    End Sub

    Private Sub IsDetailed2_Checked(sender As Object, e As RoutedEventArgs) Handles IsDetailed2.Checked
        IsDetailed.IsChecked = False
    End Sub

End Class