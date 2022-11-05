Imports System.Data

Public Class EditAttendance2
    Dim dt As New DataTable
    Dim bm As New BasicMethods
    WithEvents G As New MyGrid


    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return


        LoadResource()
        LoadWFH()
        DayDate.SelectedDate = bm.MyGetDate()

        CompanyId_LostFocus(Nothing, Nothing)



        btnDelete.Visibility = Visibility.Hidden
        DayDate.IsEnabled = Md.Manager
    End Sub

    Structure GC
        Shared EmpId As String = "EmpId"
        Shared EmpName As String = "EmpName"
        Shared LoginDateTime As String = "LoginDateTime"
        Shared LogOutDateTime As String = "LogOutDateTime"
        Shared Attend As String = "Attend"
        Shared Attend2 As String = "Attend2"
        Shared Attend2Value As String = "Attend2Value"
        Shared Shift0 As String = "Shift0"
        Shared Shift1 As String = "Shift1"
        Shared Shift2 As String = "Shift2"
        Shared Shift3 As String = "Shift3"
    End Structure


    Private Sub LoadWFH()

        WFH.Child = G
        G.IgnoreRecordNo = True

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue
        G.Columns.Add(GC.EmpId, "كود الموظف")
        G.Columns.Add(GC.EmpName, "اسم الموظف")

        G.Columns.Add(GC.LoginDateTime, "حضور")
        G.Columns.Add(GC.LogOutDateTime, "انصراف")

        Dim GCC As New Forms.DataGridViewComboBoxColumn
        GCC.HeaderText = "الحضور"
        GCC.Name = GC.Attend
        bm.FillCombo("select 0 Id,'-' Name union Select Id,Name from Attends where Id=1", GCC)
        G.Columns.Add(GCC)

        Dim GCC2 As New Forms.DataGridViewComboBoxColumn
        GCC2.HeaderText = "الإضافي"
        GCC2.Name = GC.Attend2
        bm.FillCombo("select 0 Id,'-' Name union Select Id,Name from Attends where Id=1", GCC2)
        G.Columns.Add(GCC2)

        G.Columns.Add(GC.Attend2Value, "عدد ساعات الإضافي")
        G.Columns.Add(GC.Shift0, "حضور مبكر")
        G.Columns.Add(GC.Shift1, "سهرة أولى")
        G.Columns.Add(GC.Shift2, "سهرة ثانية")
        G.Columns.Add(GC.Shift3, "سهرة ثالثة")

        G.Columns(GC.EmpId).ReadOnly = True
        G.Columns(GC.EmpName).ReadOnly = True
        'G.Columns(GC.LoginDateTime).ReadOnly = True
        'G.Columns(GC.LogOutDateTime).ReadOnly = True
        G.Columns(GC.Shift0).ReadOnly = True
        G.Columns(GC.Shift1).ReadOnly = True
        G.Columns(GC.Shift2).ReadOnly = True
        G.Columns(GC.Shift3).ReadOnly = True

        G.Columns(GC.EmpName).FillWeight = 300
        G.Columns(GC.LoginDateTime).FillWeight = 300
        G.Columns(GC.LogOutDateTime).FillWeight = 300
        G.Columns(GC.Attend).FillWeight = 150
        G.Columns(GC.Attend2).FillWeight = 150

        G.AutoSizeColumnsMode = Forms.DataGridViewAutoSizeColumnsMode.Fill
        G.AllowUserToDeleteRows = False
        G.AllowUserToAddRows = False

        G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
        G.TabStop = False
        AddHandler G.CellDoubleClick, AddressOf G_CellDoubleClick
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If DayDate.SelectedDate Is Nothing Then
            DayDate.Focus()
            Return
        End If
        G.EndEdit()

        bm.ExecuteNonQuery("delete AttendanceLog where (dbo.GetEmpCompanyId(EmpId)=" & Val(CompanyId.Text) & " or " & Val(CompanyId.Text) & "=0) and cast(DayDate as date)='" & bm.ToStrDate(DayDate.SelectedDate) & "'")

        Dim str As String = "Insert AttendanceLog(EmpId,DayDate,Attend,Attend2,Attend2Value,Shift0,Shift1,Shift2,Shift3,LoginDateTime,LogOutDateTime) values "
        For i As Integer = 0 To G.Rows.Count - 1
            Try
                str &= "('" & G.Rows(i).Cells(GC.EmpId).Value.ToString & "','" & bm.ToStrDate(DayDate.SelectedDate) & "','" & G.Rows(i).Cells(GC.Attend).Value.ToString & "','" & G.Rows(i).Cells(GC.Attend2).Value.ToString & "','" & G.Rows(i).Cells(GC.Attend2Value).Value.ToString & "','" & G.Rows(i).Cells(GC.Shift0).Value.ToString & "','" & G.Rows(i).Cells(GC.Shift1).Value.ToString & "','" & G.Rows(i).Cells(GC.Shift2).Value.ToString & "','" & G.Rows(i).Cells(GC.Shift3).Value.ToString & "','" & G.Rows(i).Cells(GC.LoginDateTime).Value.ToString & "','" & G.Rows(i).Cells(GC.LogOutDateTime).Value.ToString & "'),"
            Catch ex As Exception
            End Try
        Next
        str = str.Substring(0, str.Length - 1)

        If Not bm.ExecuteNonQuery(str) Then Return

        btnNew_Click(sender, e)

    End Sub


    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click, DayDate.SelectedDateChanged
        G.Rows.Clear()
        dt = bm.ExecuteAdapter("GetNewAttendanceLog", {"CompanyId", "DayDate"}, {Val(CompanyId.Text), bm.ToStrDate(DayDate.SelectedDate)})
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add({dt.Rows(i)(0).ToString, dt.Rows(i)(1).ToString, bm.ToStrDateTimeFormated(dt.Rows(i)(2).ToString), bm.ToStrDateTimeFormated(dt.Rows(i)(3).ToString), dt.Rows(i)(4).ToString, dt.Rows(i)(5).ToString, dt.Rows(i)(6), dt.Rows(i)(7), dt.Rows(i)(8), dt.Rows(i)(9), dt.Rows(i)(10)})
        Next
        G.RefreshEdit()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExecuteNonQuery("delete AttendanceLog where (dbo.GetEmpCompanyId(EmpId)=" & Val(CompanyId.Text) & " or " & Val(CompanyId.Text) & "=0) and cast(DayDate as date)='" & bm.ToStrDate(DayDate.SelectedDate) & "'")
            btnNew_Click(sender, e)
        End If
    End Sub




    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")
        
    End Sub

    Private Sub G_CellDoubleClick(sender As Object, e As Forms.DataGridViewCellEventArgs)
        Dim Shift0 As Decimal = 0
        Dim Shift1 As Decimal = 0
        Dim Shift2 As Decimal = 0
        Dim Shift3 As Decimal = 0


        dt = bm.ExecuteAdapter("select Shift0,Shift1,Shift2,Shift3 from Departments where Id=(select top 1 DepartmentId from EmpSalary E where E.Id=" & Val(G.Rows(e.RowIndex).Cells(GC.EmpId).Value) & " order by Line desc)")
        If dt.Rows.Count > 0 Then
            Shift0 = Val(dt.Rows(0)("Shift0").ToString)
            Shift1 = Val(dt.Rows(0)("Shift1").ToString)
            Shift2 = Val(dt.Rows(0)("Shift2").ToString)
            Shift3 = Val(dt.Rows(0)("Shift3").ToString)
        End If



        If e.ColumnIndex = G.Columns(GC.Shift0).Index Then
            If G.Rows(e.RowIndex).Cells(GC.Shift0).Value = 0 Then
                G.Rows(e.RowIndex).Cells(GC.Shift0).Value = Shift0
            Else
                G.Rows(e.RowIndex).Cells(GC.Shift0).Value = 0
            End If
        ElseIf e.ColumnIndex = G.Columns(GC.Shift1).Index Then
            If G.Rows(e.RowIndex).Cells(GC.Shift1).Value = 0 Then
                G.Rows(e.RowIndex).Cells(GC.Shift1).Value = Shift1
            Else
                G.Rows(e.RowIndex).Cells(GC.Shift1).Value = 0
                G.Rows(e.RowIndex).Cells(GC.Shift2).Value = 0
                G.Rows(e.RowIndex).Cells(GC.Shift3).Value = 0
            End If
        ElseIf e.ColumnIndex = G.Columns(GC.Shift2).Index Then
            If G.Rows(e.RowIndex).Cells(GC.Shift2).Value = 0 Then
                G.Rows(e.RowIndex).Cells(GC.Shift1).Value = Shift1
                G.Rows(e.RowIndex).Cells(GC.Shift2).Value = Shift2
            Else
                G.Rows(e.RowIndex).Cells(GC.Shift2).Value = 0
                G.Rows(e.RowIndex).Cells(GC.Shift3).Value = 0
            End If
        ElseIf e.ColumnIndex = G.Columns(GC.Shift3).Index Then
            If G.Rows(e.RowIndex).Cells(GC.Shift3).Value = 0 Then
                G.Rows(e.RowIndex).Cells(GC.Shift1).Value = Shift1
                G.Rows(e.RowIndex).Cells(GC.Shift2).Value = Shift2
                G.Rows(e.RowIndex).Cells(GC.Shift3).Value = Shift3
            Else
                G.Rows(e.RowIndex).Cells(GC.Shift3).Value = 0
            End If
        End If
    End Sub


    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CompanyId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub CompanyId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CompanyId.KeyUp
        bm.ShowHelp("Companies", CompanyId, CompanyName, e, "select cast(Id as varchar(100)) Id,Name from Companies", "Companies")
    End Sub

    Private Sub CompanyId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CompanyId.LostFocus
        bm.LostFocus(CompanyId, CompanyName, "select Name from Companies where Id=" & CompanyId.Text.Trim())
        btnNew_Click(Nothing, Nothing)
    End Sub


    Private Sub btnSave_Copy_Click(sender As Object, e As RoutedEventArgs) Handles btnSave_Copy.Click
        For i As Integer = 0 To G.Rows.Count - 1
            G.Rows(i).Cells(GC.Attend).Value = "1"
        Next
        G.EndEdit()
        G.CurrentCell = G.Rows(0).Cells(0)
    End Sub

    Private Sub btnSave_Copy1_Click(sender As Object, e As RoutedEventArgs) Handles btnSave_Copy1.Click
        For i As Integer = 0 To G.Rows.Count - 1
            G.Rows(i).Cells(GC.Attend).Value = "0"
        Next
        G.EndEdit()
        G.CurrentCell = G.Rows(0).Cells(0)
    End Sub

    Private Sub btnSave_Copy2_Click(sender As Object, e As RoutedEventArgs) Handles btnSave_Copy2.Click
        For i As Integer = 0 To G.Rows.Count - 1
            G.Rows(i).Cells(GC.Attend2).Value = "1"
        Next
        G.EndEdit()
        G.CurrentCell = G.Rows(0).Cells(0)
    End Sub

    Private Sub btnSave_Copy3_Click(sender As Object, e As RoutedEventArgs) Handles btnSave_Copy3.Click
        For i As Integer = 0 To G.Rows.Count - 1
            G.Rows(i).Cells(GC.Attend2).Value = "0"
        Next
        G.EndEdit()
        G.CurrentCell = G.Rows(0).Cells(0)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim rpt As New ReportViewer
        rpt.paraname = New String() {"@DayDate", "@CompanyId", "Header"}
        rpt.paravalue = New String() {bm.ToStrDate(DayDate.SelectedDate), Val(CompanyId.Text), CType(Parent, Page).Title}
        rpt.Rpt = "NewAttendanceLog.rpt"
        rpt.Show()
    End Sub
End Class
