Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO

Public Class RPT25
    Dim bm As New BasicMethods
    Dim dt As New DataTable

    Public Flag As Integer = 0 ' if 20 then exec CalcFifoOrAvgcost throw Forms
    Public Tbl As String = ""
    Public IsPosted As Integer = 0

    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                rpt.Rpt = "Loans.rpt"
            Case 2
                rpt.Rpt = "DirectBonus.rpt"
            Case 3
                rpt.Rpt = "DirectCut.rpt"
            Case 4
                rpt.Rpt = "LeaveRequests.rpt"
            Case 5
                rpt.Rpt = "LeaveRequests2.rpt"
            Case 6
                rpt.Rpt = "LoansStatus.rpt"
            Case 7
                rpt.Rpt = "EmpShifts.rpt"
            Case 8, 10, 20, 21, 24
                MyFromDate = FromDate.SelectedDate
                MyToDate = ToDate.SelectedDate
                BtnCalc = sender
                BtnCalc.IsEnabled = False
                If Not BackgroundWorker1.IsBusy Then BackgroundWorker1.RunWorkerAsync()
                Return
            Case 9
                rpt.Rpt = "NurseShiftSummary.rpt"
            Case 12
                rpt.Rpt = "EmpOutcome.rpt"
            Case 13
                rpt.Rpt = "CloseShift.rpt"
                If IsDetailed.IsChecked Then
                    rpt.Rpt = "CloseShiftDetailed.rpt"
                ElseIf IsDetailed2.IsChecked Then
                    rpt.Rpt = "CloseShiftDetailed2.rpt"
                End If
            Case 14
                rpt.Rpt = "AllServicesTypes.rpt"
            Case 15
                bm.PrintTbl(CType(Parent, Page).Title, "Employees", "Departments", "DepartmentId")
                Return
            Case 16
                rpt.Rpt = "CasesAll.rpt"
            Case 17
                rpt.Rpt = "StagnantItems.rpt"
            Case 18
                rpt.Rpt = "StagnantCustomers.rpt"
            Case 19
                rpt.Rpt = "CaseActive.rpt"
            Case 22
                rpt.Rpt = "FnEntryMainAll.rpt"
            Case 23
                Dim d1 As String = bm.ToStrDate(FromDate.SelectedDate)
                Dim d2 As String = bm.ToStrDate(ToDate.SelectedDate)
                Dim Str As String = "select distinct Id from (select M.ToId Id from SalesDetails D left join SalesMaster M on(M.StoreId=D.StoreId and M.Flag=D.Flag and M.InvoiceNo=D.InvoiceNo) where M.Temp=0 and M.Flag in(37,38,47,48) and M.CaseInvoiceNo=0 and M.DayDate between '" & d1 & "' and '" & d2 & "'" & _
        " union all select CaseId from services where Canceled=0 and Flag=2 and CaseInvoiceNo=0 and DayDate between '" & d1 & "' and '" & d2 & "'" & _
        " union all select CaseId from OperationMotions where Canceled=0 and CaseInvoiceNo=0 and DayDate between '" & d1 & "' and '" & d2 & "'" & _
        " union all select CaseId from ClinicsHistory where CaseInvoiceNo=0 and DayDate between '" & d1 & "' and '" & d2 & "'" &  " ) Tbl"
                '& " union all select CaseId from Reservations where DayDate between '" & d1 & "' and '" & d2 & "'" & 

                dt = bm.ExecuteAdapter(Str)
                Dim MyCases As String = ""
                Dim MyId As Integer = Val(bm.ExecuteScalar("select isnull((select max(Id) from CostCenterIds),0)+1"))
                Dim ss As String = ""
                For i As Integer = 0 To dt.Rows.Count - 1
                    'MyCases &= "," & dt.Rows(i)(0).ToString
                    ss &= " insert CostCenterIds(Id,CostCenterId) select " & MyId.ToString & "," & dt.Rows(i)(0).ToString
                Next
                bm.ExecuteNonQuery(ss)
                'bm.PrintTbl(CType(sender, Button).Content, "Cases", , , "Where Id in(0" & MyCases & ")")
                bm.PrintTbl(CType(sender, Button).Content, "Cases", , , "Where Id in (select T.CostCenterId from CostCenterIds T where T.Id=" & MyId.ToString & ")")
                Return
            Case 25
                rpt.Rpt = "Employees2.rpt"
        End Select

        rpt.paraname = New String() {"@EmpId", "@FromDate", "@ToDate", "Header", "IsDetailed"}
        rpt.paravalue = New String() {Val(EmpId.Text), FromDate.SelectedDate, ToDate.SelectedDate, CType(Parent, Page).Title, IIf(IsDetailed.IsChecked, 1, 0)}
        rpt.Show()

    End Sub

    Public Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        bm.Addcontrol_MouseDoubleClick({EmpId})
        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        IsDetailed.Visibility = Visibility.Hidden
        IsDetailed2.Visibility = Visibility.Hidden
        Select Case Flag
            Case 6
                FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
                lblToDate.Visibility = Windows.Visibility.Hidden
            Case 8
                FromDate.SelectedDate = New DateTime(MyNow.Year - 1, 12, 31, 0, 0, 0)
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                lblFromDate.Content = "أول المدة في"
                lblToDate.Content = "آخر المدة في"
            Case 10
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                'lblFromDate.Visibility = Windows.Visibility.Hidden
                'FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Content = "في تاريخ"
                FromDate.SelectedDate = New DateTime(MyNow.Year - 1, 12, 31, 0, 0, 0)
                lblToDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
            Case 11, 17, 18, 19, 22, 23, 24
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                If Flag = 24 Then
                    If IsPosted = 1 Then
                        Button2.Content = "ترحيل"
                    Else
                        Button2.Content = "إلغاء ترحيل"
                    End If
                End If
            Case 13
                IsDetailed.Visibility = Visibility.Visible
                IsDetailed2.Visibility = Visibility.Visible
            Case 14, 15, 16
                FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
                lblToDate.Visibility = Windows.Visibility.Hidden

                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
            Case 20
                FromDate.SelectedDate = New DateTime(MyNow.Year - 1, 12, 31, 0, 0, 0)
            Case 21, 25
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                If Flag = 21 Then
                    ToDate.Visibility = Windows.Visibility.Hidden
                    lblToDate.Visibility = Windows.Visibility.Hidden
                End If
                lblFromDate.Content = "من تاريخ"
                FromDate.SelectedDate = New DateTime(MyNow.Year, 1, 1, 0, 0, 0)
        End Select

    End Sub
    Private Sub LoadResource()

        
        lblEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")

        Select Case Flag
            Case 8, 10
                Button2.SetResourceReference(Button.ContentProperty, "Calculate")
            Case 11
                Button2.SetResourceReference(Button.ContentProperty, "View")
            Case Else
                Button2.SetResourceReference(Button.ContentProperty, "View Report")
        End Select
        
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        Dim str As String = "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees where 1=1 "
        str &= IIf(Flag = 7, " and SalaryOrShifts=1", "")
        str &= IIf(Flag = 12, " and Doctor=0", "")
        str &= IIf(Flag = 13, " and Doctor=0", "")

        If bm.ShowHelp("Employees", EmpId, EmpName, e, str) Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If

        Dim str As String = "select " & Resources.Item("CboName") & " Name from Employees where Id=" & EmpId.Text.Trim()
        str &= IIf(Flag = 7, " and SalaryOrShifts=1", "")
        str &= IIf(Flag = 12, " and Doctor=0", "")
        str &= IIf(Flag = 13, " and Doctor=0", "")

        bm.LostFocus(EmpId, EmpName, str)
    End Sub

    Dim MyFromDate As Date, MyToDate As Date
    Dim WithEvents BackgroundWorker1 As New ComponentModel.BackgroundWorker
    Dim FIFOTABLE As DataTable
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Select Case Flag
            Case 8, 20
                
            Case 10
                'bm.ExecuteNonQuery("CalcImportMessagesOpennedOnly", {}, {})
                bm.ExecuteNonQuery("CalcImportMessageCostSubAll", {"DeliveredDate"}, {bm.ToStrDate(MyFromDate)})
            Case 21
                bm.ExecuteNonQuery("CalcAllImportMessages", {"MyDate"}, {bm.ToStrDate(MyFromDate)})
            Case 24
                bm.ExecuteNonQuery("update " & Tbl & " set IsPosted=" & IsPosted & " where Daydate between '" & bm.ToStrDate(MyFromDate) & "' and '" & bm.ToStrDate(MyToDate) & "'")
        End Select
    End Sub

    Dim BtnCalc As Button
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        BtnCalc.IsEnabled = True
        If Not FIFOTABLE Is Nothing AndAlso FIFOTABLE.Rows.Count > 0 Then
            If Val(FIFOTABLE.Rows(0)(0)) = 0 Then
                bm.ShowMSG("يوجد صرف بدون رصيد" & vbCrLf & FIFOTABLE.Rows(0)("TableDiscription").ToString & vbCrLf & "مخزن: " & FIFOTABLE.Rows(0)("StoreId").ToString & vbCrLf & "مسلسل: " & FIFOTABLE.Rows(0)("InvoiceNo").ToString & vbCrLf & "تاريخ: " & FIFOTABLE.Rows(0)("DayDate").ToString.Substring(0, 10) & vbCrLf & "صنف: " & FIFOTABLE.Rows(0)("ItemId").ToString)
                Return
            End If
        End If
        bm.ShowMSG("Done Successfuly")
    End Sub

    Private Sub IsDetailed_Checked(sender As Object, e As RoutedEventArgs) Handles IsDetailed.Checked
        IsDetailed2.IsChecked = False
    End Sub

    Private Sub IsDetailed2_Checked(sender As Object, e As RoutedEventArgs) Handles IsDetailed2.Checked
        IsDetailed.IsChecked = False
    End Sub

End Class