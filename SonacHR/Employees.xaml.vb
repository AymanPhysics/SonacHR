Imports System.Data
Imports System.IO
Imports System.ComponentModel

Public Class Employees
    Public TableName As String = "Employees"
    Public SubId As String = "Id"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Public Flag As Integer = 0
    Dim WithEvents BackgroundWorker1 As New BackgroundWorker
    WithEvents G As New MyGrid

    Private Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.TestSecurity(Me, {btnSave}, {btnDelete}, {btnFirst, btnNext, btnPrevios, btnLast}, {})


        LoadResource()
        LoadWFH()

        Try
            Dim c = G.Parent
            G.Parent = Nothing
            G.Parent = c
        Catch ex As Exception
        End Try
        bm.Fields = New String() {SubId, "Name", "Address", "DateOfBirth", "Notes", "Manager", "SystemUser", "NationalId", "HomePhone", "Mobile", "Email", "Password", "EnName", "LevelId", "Stopped", "HiringDate", "Waiter", "BADGENUMBER", "UserCanRptExportButton", "CompanyId", "ExitDate", "IsTemp", "PensionDate", "IsPensionDate", "PensionDate2", "InsNo", "Menha", "CountryId", "QualificationId", "GenderId", "RegionId", "IsShift1", "LoginTime", "LogoutTime", "Shift1"}
        bm.control = New Control() {txtID, ArName, Address, DateOfBirth, Notes, Manager, SystemUser, NationalId, HomePhone, Mobile, Email, Password, EnName, LevelId, Stopped, HiringDate, Waiter, BADGENUMBER, UserCanRptExportButton, CompanyId, ExitDate, IsTemp, PensionDate, IsPensionDate, PensionDate2, InsNo, Menha, CountryId, QualificationId, GenderId, RegionId, IsShift1, LoginTime, LogoutTime, Shift1}
        bm.KeyFields = New String() {SubId}
        bm.Table_Name = TableName

        btnNew_Click(sender, e)

    End Sub


    Structure GC
        Shared DepartmentId As String = "DepartmentId"
        Shared DayDate As String = "DayDate"
        Shared Salary As String = "Salary"
        Shared SalaryFixed As String = "SalaryFixed"
        Shared SalaryChanged As String = "SalaryChanged"
        Shared Line As String = "Line"
    End Structure

    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        Dim GCC As New Forms.DataGridViewComboBoxColumn
        GCC.HeaderText = "الوظيفة"
        GCC.Name = GC.DepartmentId
        bm.FillCombo("select 0 Id,'-' Name union Select Id,Name from Departments", GCC)
        G.Columns.Add(GCC)

        G.Columns.Add(GC.DayDate, "التاريخ")
        G.Columns.Add(GC.Salary, "الأساسي")
        G.Columns.Add(GC.SalaryFixed, "أساسي التأمينات")
        G.Columns.Add(GC.SalaryChanged, "متغير التأمينات")
        G.Columns.Add(GC.Line, "Line")

        G.Columns(GC.DepartmentId).FillWeight = 300

        G.Columns(GC.Line).Visible = False

        AddHandler G.UserAddedRow, AddressOf G_UserAddedRow
    End Sub


    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {SubId}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Dim loplop As Boolean = False
    Sub FillControls()
        loplop = True
        bm.FillControls(Me)
        LoadTree()
        bm.GetImage(TableName, New String() {SubId}, New String() {txtID.Text.Trim}, "Image", Image1)

        LevelId_LostFocus(Nothing, Nothing)
        CompanyId_LostFocus(Nothing, Nothing)
        CountryId_LostFocus(Nothing, Nothing)
        QualificationId_LostFocus(Nothing, Nothing)
        GenderId_LostFocus(Nothing, Nothing)
        RegionId_LostFocus(Nothing, Nothing)


        Dim dt As DataTable = bm.ExecuteAdapter("select * from EmpSalary where Id=" & txtID.Text & " order by Line")
        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add()
            G.Rows(i).Cells(GC.DepartmentId).Value = dt.Rows(i)("DepartmentId").ToString
            G.Rows(i).Cells(GC.DayDate).Value = dt.Rows(i)("DayDate").ToString.Substring(0, 10)
            G.Rows(i).Cells(GC.Salary).Value = dt.Rows(i)("Salary").ToString
            G.Rows(i).Cells(GC.SalaryFixed).Value = dt.Rows(i)("SalaryFixed").ToString
            G.Rows(i).Cells(GC.SalaryChanged).Value = dt.Rows(i)("SalaryChanged").ToString
            G.Rows(i).Cells(GC.Line).Value = dt.Rows(i)("Line").ToString
        Next
        G.CurrentCell = G.Rows(G.Rows.Count - 1).Cells(0)

        loplop = False
    End Sub
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {SubId}, New String() {txtID.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Dim AllowSave As Boolean = False
    Dim DontClear As Boolean = False
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        AllowSave = False
        If ArName.Text.Trim = "" OrElse Not bm.TestNames(ArName, EnName) Then
            ArName.Focus()
            Return
        End If

        G.EndEdit()

        bm.DefineValues()
        If Not bm.Save(New String() {SubId}, New String() {txtID.Text.Trim}) Then Return

        bm.SaveImage(TableName, New String() {SubId}, New String() {txtID.Text.Trim}, "Image", Image1)

        bm.SaveGrid(G, "EmpSalary", New String() {"Id"}, New String() {txtID.Text}, New String() {"DayDate", "DepartmentId", "Salary", "SalaryFixed", "SalaryChanged"}, New String() {GC.DayDate, GC.DepartmentId, GC.Salary, GC.SalaryFixed, GC.SalaryChanged}, New VariantType() {VariantType.Date, VariantType.Integer, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal}, New String() {}, "Line", GC.Line)

        If Not DontClear Then btnNew_Click(sender, e)
        AllowSave = True

    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click

        bm.FirstLast(New String() {SubId}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        ClearControls()
    End Sub


    Sub ClearControls()
        bm.ClearControls()
        LoadTree()

        bm.SetNoImage(Image1, True)

        G.Rows.Clear()

        IsPensionDate_Checked(Nothing, Nothing)
        CountryId_LostFocus(Nothing, Nothing)
        QualificationId_LostFocus(Nothing, Nothing)
        GenderId_LostFocus(Nothing, Nothing)
        RegionId_LostFocus(Nothing, Nothing)


        Password.Password = 123
        LoginTime.Text = 8
        LogoutTime.Text = 14
        Shift1.Text = 18

        LevelName.Clear()
        CompanyName.Clear()
        ArName.Clear()
        txtID.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName)
        If txtID.Text = "" Then txtID.Text = "1"

        BADGENUMBER.Text = txtID.Text

        ArName.Focus()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExecuteNonQuery("delete from " & TableName & " where " & SubId & "='" & txtID.Text.Trim & "'")
            bm.ExecuteNonQuery("delete from EmpSalary where Id='" & txtID.Text.Trim & "'")
            btnNew_Click(sender, e)
        End If
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {SubId}, New String() {txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False


    Private Sub txtID_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyUp
        If bm.ShowHelp("Employees", txtID, ArName, e, "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees") Then
            txtID_LostFocus(sender, Nothing)
        End If
    End Sub


    Private Sub txtID_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {SubId}, New String() {txtID.Text.Trim}, dt)
        If dt.Rows.Count = 0 Then
            Dim s As String = txtID.Text
            ClearControls()
            txtID.Text = s
            ArName.Focus()
            lv = False
            Return
        End If
        FillControls()
        lv = False
        ArName.SelectAll()
        ArName.Focus()
        ArName.SelectAll()
        ArName.Focus()
        'arName.Text = dt(0)("Name")
    End Sub

    Private Sub LevelId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles LevelId.KeyUp
        bm.ShowHelp("Security Levels", LevelId, LevelName, e, "select cast(Id as varchar(100)) Id,Name from Levels")
    End Sub

    Private Sub CompanyId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CompanyId.KeyUp
        bm.ShowHelp("Companies", CompanyId, CompanyName, e, "select cast(Id as varchar(100)) Id,Name from Companies", "Companies")
    End Sub

    Private Sub CountryId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CountryId.KeyUp
        bm.ShowHelp("Countries", CountryId, CountryName, e, "select cast(Id as varchar(100)) Id,Name from Countries", "Countries")
    End Sub

    Private Sub QualificationId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles QualificationId.KeyUp
        bm.ShowHelp("Qualifications", QualificationId, QualificationName, e, "select cast(Id as varchar(100)) Id,Name from Qualifications", "Qualifications")
    End Sub

    Private Sub GenderId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles GenderId.KeyUp
        bm.ShowHelp("Gender", GenderId, GenderName, e, "select cast(Id as varchar(100)) Id,Name from Gender", "Gender")
    End Sub

    Private Sub RegionId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles RegionId.KeyUp
        bm.ShowHelp("Regions", RegionId, RegionName, e, "select cast(Id as varchar(100)) Id,Name from Regions", "Regions")
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyDown, LevelId.KeyDown, BADGENUMBER.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID_KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        bm.MyKeyPress(sender, e, True)
    End Sub




    Private Sub LevelId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles LevelId.LostFocus
        bm.LostFocus(LevelId, LevelName, "select Name from Levels where Id=" & LevelId.Text.Trim())
    End Sub

    Private Sub CompanyId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CompanyId.LostFocus
        bm.LostFocus(CompanyId, CompanyName, "select Name from Companies where Id=" & CompanyId.Text.Trim())
    End Sub

    Private Sub CountryId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CountryId.LostFocus
        bm.LostFocus(CountryId, CountryName, "select Name from Countries where Id=" & CountryId.Text.Trim())
    End Sub

    Private Sub QualificationId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles QualificationId.LostFocus
        bm.LostFocus(QualificationId, QualificationName, "select Name from Qualifications where Id=" & QualificationId.Text.Trim())
    End Sub

    Private Sub GenderId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles GenderId.LostFocus
        bm.LostFocus(GenderId, GenderName, "select Name from Gender where Id=" & GenderId.Text.Trim())
    End Sub

    Private Sub RegionId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles RegionId.LostFocus
        bm.LostFocus(RegionId, RegionName, "select Name from Regions where Id=" & RegionId.Text.Trim())
    End Sub


    Private Sub btnSetImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetImage.Click
        DontClear = True
        btnSave_Click(btnSave, Nothing)
        DontClear = False
        If Not AllowSave Then Return
        bm.SetImage(Image1)
    End Sub

    Private Sub btnSetNoImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetNoImage.Click
        bm.SetNoImage(Image1, True, True)
    End Sub

    Private Sub ArName_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ArName.LostFocus
        ArName.Text = ArName.Text.Trim
        EnName.Text = bm.GetEnName(ArName.Text.Trim)
    End Sub





    Private Sub LoadResource()
        btnSetImage.SetResourceReference(Button.ContentProperty, "Change")
        btnSetNoImage.SetResourceReference(Button.ContentProperty, "Cancel")

        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")

        Manager.SetResourceReference(CheckBox.ContentProperty, "Manager")
        SystemUser.SetResourceReference(CheckBox.ContentProperty, "SystemUser")
        Stopped.SetResourceReference(CheckBox.ContentProperty, "Stopped")

        lblId.SetResourceReference(Label.ContentProperty, "Id")

        lblAddress.SetResourceReference(Label.ContentProperty, "Address")
        lblArName.SetResourceReference(Label.ContentProperty, "ArName")
        lblDateOfBirth.SetResourceReference(Label.ContentProperty, "DateOfBirth")
        lblEmail.SetResourceReference(Label.ContentProperty, "Email")
        lblEnName.SetResourceReference(Label.ContentProperty, "EnName")


        lblHiringDate.SetResourceReference(Label.ContentProperty, "HiringDate")
        lblLevelId.SetResourceReference(Label.ContentProperty, "Security Level")
        lblMobile.SetResourceReference(Label.ContentProperty, "Mobile")
        lblMobile.SetResourceReference(Label.ContentProperty, "Mobile")
        lblNationalID.SetResourceReference(Label.ContentProperty, "National ID")
        lblNotes.SetResourceReference(Label.ContentProperty, "Notes")
        lblPassword.SetResourceReference(Label.ContentProperty, "Password")
        lblTel.SetResourceReference(Label.ContentProperty, "Tel")
        lblBADGENUMBER.SetResourceReference(Label.ContentProperty, "BADGENUMBER")


    End Sub

    Dim lop As Boolean = False



    Private Sub IsPensionDate_Checked(sender As Object, e As RoutedEventArgs) Handles IsPensionDate.Checked, IsPensionDate.Unchecked
        If IsPensionDate.IsChecked Then
            PensionDate2.Visibility = Windows.Visibility.Visible
        Else
            PensionDate2.Visibility = Windows.Visibility.Hidden
        End If
    End Sub



    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDownload.Click
        Try
            MyImagedata = Nothing
            If CType(TreeView1.SelectedItem, TreeViewItem).FontSize <> 18 Then Return
            Dim s As New Forms.SaveFileDialog With {.Filter = "All files (*.*)|*.*"}
            s.FileName = CType(TreeView1.SelectedItem, TreeViewItem).Header

            If IsNothing(sender) Then
                MyBath = bm.GetNewTempName(s.FileName)
            Else
                If Not s.ShowDialog = Forms.DialogResult.OK Then Return
                MyBath = s.FileName
            End If

            btnDownload.IsEnabled = False
            F1 = txtID.Text
            F2 = CType(TreeView1.SelectedItem, TreeViewItem).Tag
            BackgroundWorker1.RunWorkerAsync()
        Catch ex As Exception
        End Try
    End Sub
    Dim F2 As String = "", F1 As String = ""
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            Dim myCommand As SqlClient.SqlCommand
            myCommand = New SqlClient.SqlCommand("select Image from EmployeesAttachments where " & SubId & "='" & F1 & "' and AttachedName='" & F2 & "'" & bm.AppendWhere, con)
            If con.State <> ConnectionState.Open Then con.Open()
            MyImagedata = CType(myCommand.ExecuteScalar(), Byte())
        Catch ex As Exception
        End Try
        con.Close()
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            File.WriteAllBytes(MyBath, MyImagedata)
            Process.Start(MyBath)
        Catch ex As Exception
        End Try
        btnDownload.IsEnabled = True
    End Sub

    Dim MyImagedata() As Byte
    Dim MyBath As String = ""
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDeleteFile.Click
        Try
            If CType(TreeView1.SelectedItem, TreeViewItem).FontSize = 18 Then
                If bm.ShowDeleteMSG("MsgDeleteFile") Then
                    bm.ExecuteNonQuery("delete from EmployeesAttachments where " & SubId & "='" & txtID.Text & "' and AttachedName='" & TreeView1.SelectedItem.Header & "'" & bm.AppendWhere)
                    LoadTree()
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadTree()
        Dim dt As DataTable = bm.ExecuteAdapter("select AttachedName from EmployeesAttachments where " & SubId & "=" & txtID.Text & bm.AppendWhere)
        TreeView1.Items.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim nn As New TreeViewItem
            nn.Foreground = Brushes.DarkRed
            nn.FontSize = 18
            nn.Tag = dt.Rows(i)(0).ToString
            nn.Header = dt.Rows(i)(0).ToString
            TreeView1.Items.Add(nn)
        Next
    End Sub


    Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAttach.Click
        DontClear = True
        btnSave_Click(btnSave, Nothing)
        DontClear = False
        If Not AllowSave Then Return

        Dim o As New Forms.OpenFileDialog
        o.Multiselect = True
        If o.ShowDialog = Forms.DialogResult.OK Then
            For i As Integer = 0 To o.FileNames.Length - 1
                bm.SaveFile("EmployeesAttachments", SubId, txtID.Text, "AttachedName", (o.FileNames(i).Split("\"))(o.FileNames(i).Split("\").Length - 1), "Image", o.FileNames(i))
            Next
        End If
        LoadTree()
    End Sub


    Private Sub TreeView1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeView1.MouseDoubleClick
        Button4_Click(Nothing, Nothing)
    End Sub

    Private Sub G_UserAddedRow(sender As Object, e As Forms.DataGridViewRowEventArgs)
        Try
            G.Rows(e.Row.Index - 1).Cells(GC.DayDate).Value = Now.Date.ToString.Substring(0, 10)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DateOfBirth_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles DateOfBirth.SelectedDateChanged
        If DateOfBirth.SelectedDate Is Nothing Then Return
        PensionDate.SelectedDate = DateOfBirth.SelectedDate.Value.AddYears(60)
    End Sub
End Class
