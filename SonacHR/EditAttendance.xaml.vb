Imports System.Data

Public Class EditAttendance
    Dim dt As New DataTable
    Dim bm As New BasicMethods
    WithEvents G As New MyGrid

    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.TestSecurity(Me, {btnSave}, {btnDelete}, {}, {})
        bm.Addcontrol_MouseDoubleClick({EmpId})
        LoadResource()
        LoadWFH()
        Dim MyNow As DateTime = bm.MyGetDate()
        txtMonth.Text = MyNow.Month
        txtYear.Text = MyNow.Year
    End Sub

    Structure GC
        Shared Day As String = "Day"
        Shared Time As String = "Time"
        Shared State As String = "State"
    End Structure


    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue
        G.Columns.Add(GC.Day, "اليوم")
        G.Columns.Add(GC.Time, "الوقت")

        Dim GCState As New Forms.DataGridViewComboBoxColumn
        GCState.HeaderText = "الحالة"
        GCState.Name = GC.State
        bm.FillCombo("select 0 Id,'حضور' Name union select 1 Id,'انصراف' Name", GCState)
        G.Columns.Add(GCState)

        G.AutoSizeColumnsMode = Forms.DataGridViewAutoSizeColumnsMode.Fill
        G.AllowUserToDeleteRows = True
        G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
        G.TabStop = False
        AddHandler G.CellEndEdit, AddressOf GridCalcRow
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If EmpId.Text.Trim = "" Then
            EmpId.Focus()
            Return
        End If
        G.EndEdit()

        Dim str As String = "delete AttendanceLog where EmpId='" & Val(EmpId.Text) & "' and month(DayDate)='" & Val(txtMonth.Text) & "' and year(DayDate)='" & Val(txtYear.Text) & "' Insert AttendanceLog(EmpId,DayDate,State) values "
        For i As Integer = 0 To G.Rows.Count - 1
            Try
                If G.Rows(i).Cells(GC.Day).Value.ToString.Trim = "" OrElse G.Rows(i).Cells(GC.Time).Value.ToString.Trim = "" Then Continue For
                str &= "('" & EmpId.Text & "','" & txtYear.Text & "-" & txtMonth.Text & "-" & G.Rows(i).Cells(GC.Day).Value.ToString & " " & G.Rows(i).Cells(GC.Time).Value.ToString & "','" & G.Rows(i).Cells(GC.State).Value.ToString & "'),"
            Catch ex As Exception
            End Try
        Next
        str = str.Substring(0, str.Length - 1)


        If Not bm.ExecuteNonQuery(str) Then Return

        btnNew_Click(sender, e)

    End Sub


    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        G.Rows.Clear()
        EmpId.Clear()
        EmpName.Clear()
        EmpId.Focus()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExecuteNonQuery("delete AttendanceLog where EmpId='" & Val(EmpId.Text) & "' and month(DayDate)='" & Val(txtMonth.Text) & "' and year(DayDate)='" & Val(txtYear.Text) & "'")
            btnNew_Click(sender, e)
        End If
    End Sub




    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        lblFromDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Month")
        lblFromDate_Copy.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Year")
        lblEmp.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "EmpId")
    End Sub

    Private Sub My_LostFocus(sender As Object, e As RoutedEventArgs) Handles EmpId.LostFocus, txtMonth.LostFocus, txtYear.LostFocus, btnRefresh.Click

        Dim dt As DataTable = bm.ExecuteAdapter("GetAttendanceLog", New String() {"EmpId", "Month", "Year"}, New String() {Val(EmpId.Text), Val(txtMonth.Text), Val(txtYear.Text)})
        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add()
            G.Rows(i).Cells(GC.Day).Value = DateTime.Parse(dt.Rows(i)("DayDate")).Day
            G.Rows(i).Cells(GC.Time).Value = DateTime.Parse(dt.Rows(i)("DayDate")).TimeOfDay.ToString
            G.Rows(i).Cells(GC.State).Value = dt.Rows(i)("State").ToString
        Next
        G.RefreshEdit()
    End Sub

    Private Sub GridCalcRow(sender As Object, e As Forms.DataGridViewCellEventArgs)
        If G.Rows(e.RowIndex).Cells(GC.State).Value Is Nothing Then
            G.Rows(e.RowIndex).Cells(GC.State).Value = "0"
        End If
        If Not G.Rows(e.RowIndex).Cells(GC.Time).Value Is Nothing AndAlso e.ColumnIndex = G.Columns(GC.Time).Index Then
            G.Rows(e.RowIndex).Cells(GC.Time).Value = G.Rows(e.RowIndex).Cells(GC.Time).Value.ToString.Replace(".", ":")
        End If
        If Not G.Rows(e.RowIndex).Cells(GC.Time).Value Is Nothing AndAlso e.ColumnIndex = G.Columns(GC.Time).Index Then
            G.Rows(e.RowIndex).Cells(GC.Time).Value = G.Rows(e.RowIndex).Cells(GC.Time).Value.ToString.Replace(".", ":")
            If Not G.Rows(e.RowIndex).Cells(GC.Time).Value.ToString.Contains(":") Then
                G.Rows(e.RowIndex).Cells(GC.Time).Value = G.Rows(e.RowIndex).Cells(GC.Time).Value.ToString & ":00"
            End If
        End If
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtMonth.KeyDown, txtYear.KeyDown, EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        If bm.ShowHelp("Employees", EmpId, EmpName, e, "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees") Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If
        bm.LostFocus(EmpId, EmpName, "select " & Resources.Item("CboName") & " Name from Employees where Id=" & EmpId.Text.Trim())
        My_LostFocus(sender, Nothing)
    End Sub


End Class
