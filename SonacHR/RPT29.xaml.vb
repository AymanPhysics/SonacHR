Imports System.Data

Public Class RPT29
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If TypeId.Visibility = Windows.Visibility.Visible AndAlso TypeId.SelectedIndex < 1 Then
            bm.ShowMSG("برجاء تحديد " & lblTypeId.Content)
            TypeId.Focus()
            Return
        End If
        Dim rpt As New ReportViewer
        rpt.paraname = New String() {"Flag", "@FromDate", "@ToDate", "Header", "@CaseId", "@Flag", "@MainId", "@DayDate", "@Id"}
        rpt.paravalue = New String() {TypeId.SelectedIndex, FromDate.SelectedDate, ToDate.SelectedDate, CType(Parent, Page).Title, 0, 0, 0, ToDate.SelectedDate, 0}
        Select Case Flag
            Case 1
                rpt.Rpt = "CasesStatistics1.rpt"
            Case 2
                rpt.Rpt = "CasesStatistics2.rpt"
        End Select
        rpt.Show()
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        If Flag = 1 Then
            lblTypeId.Visibility = Windows.Visibility.Hidden
            TypeId.Visibility = Windows.Visibility.Hidden
        End If
        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, 1, 1, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        If Md.RptFromToday Then FromDate.SelectedDate = ToDate.SelectedDate

    End Sub




    Private Sub LoadResource()
        Button2.SetResourceReference(Button.ContentProperty, "View Report")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")


    End Sub

End Class