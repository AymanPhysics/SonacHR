Imports System.Data

Public Class EmpCloseShift
    Public MainTableName As String = "Employees"
    Public MainSubId As String = "Id"
    Public MainSubName As String = "Name"

    Public TableName As String = "EmpCloseShift"
    Public MainId As String = "EmpId"
    Public MainId2 As String = "DayDate"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Dim m As MainWindow = Application.Current.MainWindow
    Public Flag As Integer = 0
    Public WithImage As Boolean = False
    Public ReLoadMenue As Boolean = False

    Private Sub BasicForm2_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        If WithImage Then
            btnSetImage.Visibility = Visibility.Visible
            btnSetNoImage.Visibility = Visibility.Visible
            Image1.Visibility = Visibility.Visible
        End If

        bm.FillCombo(MainTableName, CboMain, "")

        CboMain.SelectedValue = Md.UserName
        CboMain.IsEnabled = False ' Md.Manager
        DayDate.IsEnabled = False ' Md.Manager
        DayDate.SelectedDate = bm.MyGetDate.Date

        lblBal0.Visibility = Visibility.Hidden
        Bal0.Visibility = Visibility.Hidden
        lblDiff.Visibility = Visibility.Hidden
        Diff.Visibility = Visibility.Hidden
        lblIncome.Visibility = Visibility.Hidden
        Income.Visibility = Visibility.Hidden
        lblOutcome.Visibility = Visibility.Hidden
        Outcome.Visibility = Visibility.Hidden

        Value.Clear()
        Notes.Clear()

    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Val(Value.Text) = 0 Or CboMain.SelectedValue.ToString = 0 Then
            Return
        End If


        dt = bm.ExecuteAdapter("GetEmpShiftNet", {"UserId"}, {Md.UserName})
        If dt.Rows.Count > 0 Then
            Income.Text = dt.Rows(0)("Income")
            Outcome.Text = dt.Rows(0)("Outcome")
            Bal0.Text = dt.Rows(0)("Net")
        End If

        Income.Text = Val(Income.Text)
        Outcome.Text = Val(Outcome.Text)
        Value.Text = Val(Value.Text)
        Bal0.Text = Val(Bal0.Text)
        Diff.Text = Val(Diff.Text)

        Bal0_TextChanged(Nothing, Nothing)

        Dim CurrentShift As Integer = Val(bm.ExecuteScalar("select CurrentShift from Employees where Id='" & Md.UserName & "'"))
        bm.DefineValues()
        'If bm.IF_Exists("select * from " & TableName & " where " & MainId & " ='" & CboMain.SelectedValue.ToString & "' and " & MainId2 & "='" & bm.ToStrDate(DayDate.SelectedDate) & "'") Then
        '    bm.ShowMSG("عملية خاطئة .. تم إغلاق الوردية من قبل")
        '    Return
        'End If
        If bm.ExecuteNonQuery("insert " & TableName & "(" & MainId & "," & MainId2 & ",Value,Bal0,Diff,Income,Outcome,Notes,CurrentShift,UserName,MyGetDate) select '" & CboMain.SelectedValue.ToString & "','" & bm.ToStrDate(DayDate.SelectedDate) & "','" & Val(Value.Text) & "','" & Val(Bal0.Text) & "','" & Val(Diff.Text) & "','" & Val(Income.Text) & "','" & Val(Outcome.Text) & "','" & Notes.Text.Replace("'", "''") & "','" & CurrentShift & "','" & Md.UserName & "',GetDate()") Then
            'bm.ShowMSG("Saved Successfuly")
            bm.ExecuteNonQuery("update Employees set CurrentShift=" & (CurrentShift + 1) & " where Id='" & CboMain.SelectedValue.ToString & "'    update EmpOutcome set IsClosed=1,CurrentShift=" & CurrentShift & "  where IsClosed=0 and " & MainId & " ='" & CboMain.SelectedValue.ToString & "'              update Reservations set IsClosed=1,CurrentShift=" & CurrentShift & "  where IsClosed=0 and EmpIdReservation='" & CboMain.SelectedValue.ToString & "'              update Services set IsClosed=1,CurrentShift=" & CurrentShift & "  where IsClosed=0 and EmpIdReservation='" & CboMain.SelectedValue.ToString & "'")
            BasicForm2_Loaded(Nothing, Nothing)

            Dim rpt As New ReportViewer
            rpt.paraname = New String() {"@UserId", "Header"}
            rpt.paravalue = New String() {CboMain.SelectedValue.ToString, CType(Parent, Page).Title}
            rpt.Rpt = "EmpCloseShiftOne.rpt"
            rpt.Show()
        End If

    End Sub

    Private Sub txtID2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Value.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub
    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Close Shift")
    End Sub

    Private Sub Bal0_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Bal0.TextChanged, Value.TextChanged
        Diff.Text = Val(Value.Text) - Val(Bal0.Text)
    End Sub
End Class

