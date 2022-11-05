Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Windows.Forms

Public Class CalcSalary
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Hdr As String = ""
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If Val(txtMonth.Text) = 0 OrElse Val(txtYear.Text) = 0 Then Return

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                bm.ExecuteNonQuery("CalcSalary", New String() {"Month", "Year"}, New String() {txtMonth.Text, txtYear.Text})
                bm.ShowMSG("Done Successfuly")
                Return
            Case 2
                rpt.Rpt = "SalaryHistoryShifts.rpt"
            Case 3
                rpt.Rpt = "SalaryHistoryNotShifts.rpt"
            Case 4
                LoadLog()
                Return
            Case 5
                rpt.Rpt = "SalaryHistoryAllTax.rpt"
            Case 6
                If bm.ShowDeleteMSG("إغلاق الوردية لا يمكنك من إعادة فتحها مرة أخرى" & vbCrLf & vbCrLf & "هل أنت متأكد من إغلاق الوردية؟") Then
                    bm.ExecuteNonQuery("exec CloseShift")
                
                    bm.ShowMSG("تم إغلاق الوردية")
                    Try
                        Forms.Application.Restart()
                        Application.Current.Shutdown()
                    Catch ex As Exception
                    End Try
                End If
                Return
            Case 7
                Dim maintbl As String = "ServiceGroups"
                rpt.Rpt = IIf(maintbl = "", "PrintTbl.rpt", "PrintTbl2.rpt")
                rpt.paraname = {"Header", "@tbl", "@maintbl", "@mainfield"}
                rpt.paravalue = {CType(Parent, Page).Title, "ServiceTypes", maintbl, "ServiceGroupId"}
                rpt.Show()
                Return
            Case 8
                rpt.Rpt = "Employees.rpt"
            Case 9
                bm.ExecuteNonQuery("CalcAssetsDepreciation", New String() {"Month", "Year"}, New String() {txtMonth.Text, txtYear.Text})
                bm.ShowMSG("Done Successfuly")
                Return
            Case 10
                bm.ExecuteNonQuery("SHUTDOWN ", False)
                bm.ShowMSG("Done Successfuly")
                Return
            Case 11
                rpt.Rpt = "SalaryStatistics.rpt"
            Case 12, 13, 14
                rpt.Rpt = "SalaryStatistics2.rpt"
        End Select

        rpt.paraname = New String() {"@AccNo", "@Month", "@Period", "@Year", "Header", "Perc", "Flag"}
        rpt.paravalue = New String() {TaxAccNo.Text.Trim, Val(txtMonth.Text), Val(txtMonth.Text), Val(txtYear.Text), CType(Parent, Page).Title, Val(Perc.Text), Flag}
        rpt.Show()

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.Addcontrol_MouseDoubleClick({TaxAccNo})
        LoadResource()
        Dim MyNow As DateTime = bm.MyGetDate()
        txtMonth.Text = MyNow.Month
        txtYear.Text = MyNow.Year

        Perc.Text = 25

        If Flag = 6 Then
            GG.Children.Clear()
            Button2.Content = "إغلاق الوردية"
        ElseIf Flag = 7 Then
            GG.Children.Clear()
        ElseIf Flag = 8 OrElse Flag = 11 OrElse Flag = 12 OrElse Flag = 13 OrElse Flag = 14 Then
            GG.Children.Clear()
            If Flag = 11 Then
                GG.Children.Add(lblPerc)
                GG.Children.Add(lblPerc2)
                GG.Children.Add(Perc)
            End If
            Button2.Content = "عرض التقرير"
        ElseIf Flag = 10 Then
            GG.Children.Clear()
            Button2.Content = "SHUTDOWN"
        End If

        If Flag <> 11 Then
            lblPerc.Visibility = Windows.Visibility.Hidden
            lblPerc2.Visibility = Windows.Visibility.Hidden
            Perc.Visibility = Windows.Visibility.Hidden
        End If

        If Flag = 5 Then
            TaxAccNo.Text = bm.ExecuteScalar("select dbo.GetTaxAcc()")
            TaxAccNo_LostFocus(Nothing, Nothing)
            Select Case MyNow.Month
                Case Is <= 3
                    txtMonth.Text = 1
                Case Is <= 6
                    txtMonth.Text = 2
                Case Is <= 9
                    txtMonth.Text = 3
                Case Else
                    txtMonth.Text = 4
            End Select
        End If
    End Sub
    Private Sub LoadResource()

        lblTaxAcc.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "TaxAcc")
        lblFromDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Month")
        lblFromDate_Copy.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Year")

        Select Case Flag
            Case 1
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Calculate")
            Case 2
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "View Report")
            Case 3
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "View Report")
            Case 4
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Import Attendance")
            Case 5
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "View Report")
                lblFromDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Period")
        End Select

        If Flag <> 5 Then
            lblTaxAcc.Visibility = Windows.Visibility.Hidden
            TaxAccNo.Visibility = Windows.Visibility.Hidden
            TaxAccName.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtMonth.KeyDown, txtYear.KeyDown, TaxAccNo.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub LoadLog()
        Dim oo As New OpenFileDialog
        oo.Filter = "1_attlog.dat|1_attlog.dat"
        oo.FileName = "1_attlog.dat"
        If oo.ShowDialog() = DialogResult.Cancel Then Return
        Dim path As String = oo.FileName
        If Not File.Exists(path) Then
            bm.ShowMSG("Invalid path")
            Return
        End If

        Dim st As New StreamReader(path)
        Dim s As String = ""

        Dim AttendanceLogDT As New DataTable
        AttendanceLogDT.Columns.Add("EmpId")
        AttendanceLogDT.Columns.Add("DayDate")
        AttendanceLogDT.Columns.Add("State")

        Try
            While True
                s = st.ReadLine()
                If Val(s.Substring(10, 4)) = Val(txtYear.Text) AndAlso Val(s.Substring(15, 2)) = Val(txtMonth.Text) Then
                    AttendanceLogDT.Rows.Add({s.Substring(1, 8), s.Substring(10, 19), s.Substring(32, 1)})
                End If
            End While
        Catch ex As Exception
        End Try
        If bm.ExecuteNonQuery("SaveAttandanceLog", {"AttendanceLog"}, {AttendanceLogDT}, {SqlDbType.Structured}) Then
            bm.ShowMSG("Saved Successfuly")
        Else
            bm.ShowMSG("Faild to be Saved")
        End If

    End Sub

    Private Sub TaxAccNo_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles TaxAccNo.LostFocus
        bm.AccNoLostFocus(TaxAccNo, TaxAccName, , , )
    End Sub

    Private Sub TaxAccNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TaxAccNo.KeyUp
        bm.AccNoShowHelp(TaxAccNo, TaxAccName, e, , , )
    End Sub

End Class