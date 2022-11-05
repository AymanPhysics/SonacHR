Imports System.Data

Public Class Companies
    Public TableName As String = "Companies"
    Public SubId As String = "Id"
    Public SubName As String = "Name"


    Public TableDetailsName As String = "DoctorsCompanies"
    Public TableDetailsName2 As String = "ServiceCompanies"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    WithEvents G As New MyGrid
    WithEvents G2 As New MyGrid
    Dim m As MainWindow = Application.Current.MainWindow
    Public Flag As Integer = 0
    Public WithImage As Boolean = False

    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
     
        If bm.TestIsLoaded(Me) Then Return
        bm.TestSecurity(Me, {btnSave}, {btnDelete}, {btnFirst, btnNext, btnPrevios, btnLast}, {btnPrint})
        LoadResource()
        LoadWFH()
        LoadWFH2()
        If WithImage Then
            btnSetImage.Visibility = Visibility.Visible
            btnSetNoImage.Visibility = Visibility.Visible
            Image1.Visibility = Visibility.Visible
        End If
        bm.Fields = New String() {SubId, SubName, "Name2", "Perc", "CasePerc"}
        bm.control = New Control() {txtID, txtName, txtName2, Perc, CasePerc}
        bm.KeyFields = New String() {SubId}
        bm.Table_Name = TableName
        btnNew_Click(sender, e)
        
        bm.SetImage(Img, "attchmentfile.jpg")

    End Sub

    Structure GC
        Shared VisitingTypeId As String = "VisitingTypeId"
        Shared VisitingTypeName As String = "VisitingTypeName"
        Shared DegreeId As String = "DegreeId"
        Shared DegreeName As String = "DegreeName"
        Shared Price As String = "Price"
        Shared Payed As String = "Payed"
        Shared Remaining As String = "Remaining"
        Shared Notes As String = "Notes"
    End Structure

    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        G.Columns.Add(GC.VisitingTypeId, "VisitingTypeId")
        G.Columns.Add(GC.VisitingTypeName, "النوع")
        G.Columns.Add(GC.DegreeId, "DegreeId")
        G.Columns.Add(GC.DegreeName, "الدرجة")
        
        G.Columns.Add(GC.Price, "السعر")
        G.Columns.Add(GC.Payed, "حصة المريض")
        G.Columns.Add(GC.Remaining, "حصة الشركة")
        G.Columns.Add(GC.Notes, "ملاحظات")


        G.Columns(GC.VisitingTypeName).FillWeight = 200
        G.Columns(GC.DegreeName).FillWeight = 200
        G.Columns(GC.Notes).FillWeight = 400

        G.Columns(GC.VisitingTypeId).ReadOnly = True
        G.Columns(GC.VisitingTypeName).ReadOnly = True
        G.Columns(GC.DegreeId).ReadOnly = True
        G.Columns(GC.DegreeName).ReadOnly = True
        G.Columns(GC.VisitingTypeId).Visible = False
        G.Columns(GC.DegreeId).Visible = False
        G.Columns(GC.Remaining).ReadOnly = True

        G.AllowUserToAddRows = False
        G.AllowUserToDeleteRows = False
        
        AddHandler G.CellEndEdit, AddressOf GridCalcRow
        AddHandler G.PreviewKeyDown, AddressOf G_PreviewKeyDown
    End Sub

    Structure GC2
        Shared ServiceGroupId As String = "ServiceGroupId"
        Shared ServiceGroupName As String = "ServiceGroupName"
        Shared Id As String = "Id"
        Shared Name As String = "Name"
        Shared Price As String = "Price"
        Shared Payed As String = "Payed"
        Shared Remaining As String = "Remaining"
        Shared Notes As String = "Notes"
    End Structure

    Private Sub LoadWFH2()
        WFH2.Child = G2

        G2.Columns.Clear()
        G2.ForeColor = System.Drawing.Color.DarkBlue

        G2.Columns.Add(GC2.ServiceGroupId, "كود المجموعة")
        G2.Columns.Add(GC2.ServiceGroupName, "المجموعة")
        G2.Columns.Add(GC2.Id, "كود الخدمة")
        G2.Columns.Add(GC2.Name, "الخدمة")
        G2.Columns.Add(GC2.Price, "السعر")
        G2.Columns.Add(GC2.Payed, "حصة المريض")
        G2.Columns.Add(GC2.Remaining, "حصة الشركة")
        G2.Columns.Add(GC2.Notes, "ملاحظات")

        G2.Columns(GC2.ServiceGroupName).FillWeight = 200
        G2.Columns(GC2.Name).FillWeight = 200
        G2.Columns(GC2.Notes).FillWeight = 400

        G2.Columns(GC2.ServiceGroupId).ReadOnly = True
        G2.Columns(GC2.Id).ReadOnly = True
        G2.Columns(GC2.ServiceGroupName).ReadOnly = True
        G2.Columns(GC2.Name).ReadOnly = True
        G2.Columns(GC2.Remaining).ReadOnly = True
        G2.Columns(GC2.ServiceGroupId).Visible = False
        G2.Columns(GC2.Id).Visible = False

        G2.AllowUserToAddRows = False
        G2.AllowUserToDeleteRows = False
        
        AddHandler G2.CellEndEdit, AddressOf GridCalcRow2
        AddHandler G2.PreviewKeyDown, AddressOf G2_PreviewKeyDown
    End Sub


    Sub FillControls()
        bm.FillControls(Me)
        If WithImage Then bm.GetImage(TableName, New String() {SubId}, New String() {txtID.Text.Trim}, "Image", Image1)
        FillGrid()
        FillGrid2()

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        CType(Application.Current.MainWindow, MainWindow).TabControl1.Items.Remove(Me.Parent)
    End Sub

    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {SubId}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {SubId}, New String() {txtID.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If txtName.Text.Trim = "" Then
            txtName.Focus()
            Return
        End If

        G.EndEdit()
        G2.EndEdit()

        Perc.Text = Val(Perc.Text)
        CasePerc.Text = Val(CasePerc.Text)

        bm.DefineValues()
        If Not bm.Save(New String() {SubId}, New String() {txtID.Text.Trim}) Then Return
        If WithImage Then bm.SaveImage(TableName, New String() {SubId}, New String() {txtID.Text.Trim}, "Image", Image1)

        If Not bm.SaveGrid(G, TableDetailsName, New String() {"CompanyId"}, New String() {txtID.Text}, New String() {"VisitingTypeId", "DegreeId", "Price", "Payed", "Remaining", "Notes"}, New String() {GC.VisitingTypeId, GC.DegreeId, GC.Price, GC.Payed, GC.Remaining, GC.Notes}, New VariantType() {VariantType.Integer, VariantType.Integer, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.String}, New String() {GC.VisitingTypeId, GC.DegreeId}) Then Return

        If Not bm.SaveGrid(G2, TableDetailsName2, New String() {"CompanyId"}, New String() {txtID.Text}, New String() {"ServiceGroupId", "Id", "Price", "Payed", "Remaining", "Notes"}, New String() {GC2.ServiceGroupId, GC2.Id, GC2.Price, GC2.Payed, GC2.Remaining, GC2.Notes}, New VariantType() {VariantType.Integer, VariantType.Integer, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.String}, New String() {GC2.ServiceGroupId, GC2.Id}) Then Return

        btnNew_Click(sender, e)
        
    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click

        bm.FirstLast(New String() {SubId}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        bm.ClearControls()
        ClearControls()
    End Sub


    Sub ClearControls()
        If lv Then Return
        bm.SetNoImage(Image1)
        bm.ClearControls(False)
        CheckBox2.IsChecked = True
        txtID.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName)
        If txtID.Text = "" Then txtID.Text = "1"
        'FillGrid()
        G.Rows.Clear()
        'FillGrid2()
        G2.Rows.Clear()

        txtName.Focus()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExecuteNonQuery("delete from " & TableName & " where " & SubId & "='" & txtID.Text.Trim & "'")
            DeleteDetails()
            btnNew_Click(sender, e)
        End If
    End Sub

    Sub DeleteDetails()
        bm.ExecuteNonQuery("delete from " & TableDetailsName & " where CompanyId='" & txtID.Text.Trim & "'      delete from " & TableDetailsName2 & " where CompanyId='" & txtID.Text.Trim & "'")

    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {SubId}, New String() {txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False

    Private Sub txtID_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyUp
        If bm.ShowHelp(CType(Parent, Page).Title, txtID, txtName, e, "select cast(Id as varchar(100)) Id,Name from " & TableName, TableName) Then
            txtName.Focus()
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
            txtName.Focus()
            lv = False
            Return
        End If
        FillControls()
        lv = False
        txtName.SelectAll()
        txtName.Focus()
        txtName.SelectAll()
        txtName.Focus()
        'txtName.Text = dt(0)("Name")
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Perc.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub


    
    
    Private Sub btnSetImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetImage.Click
        bm.SetImage(Image1)
    End Sub

    Private Sub btnSetNoImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetNoImage.Click
        bm.SetNoImage(Image1, False, True)
    End Sub


    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")
        btnPrint.SetResourceReference(Button.ContentProperty, "Print")

        lblId.SetResourceReference(Label.ContentProperty, "Id")
        lblName.SetResourceReference(Label.ContentProperty, "Name")
        lblPerc.SetResourceReference(Label.ContentProperty, "DedPerc")
        lblCasePerc.SetResourceReference(Label.ContentProperty, "CasePerc")
    End Sub

    Private Sub FillGrid()
        dt = bm.ExecuteAdapter("GetDoctorsCompanies", {"CompanyId"}, {Val(txtID.Text)})
        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add({dt.Rows(i)("VisitingTypeId"), dt.Rows(i)("VisitingTypeName"), dt.Rows(i)("DegreeId"), dt.Rows(i)("DegreeName"), dt.Rows(i)("Price"), dt.Rows(i)("Payed"), dt.Rows(i)("Remaining"), dt.Rows(i)("Notes")})
        Next
    End Sub


    Private Sub FillGrid2()
        dt = bm.ExecuteAdapter("GetServiceCompanies", {"CompanyId"}, {Val(txtID.Text)})
        'dt = bm.ExecuteAdapter("select C.ServiceGroupId,dbo.GetServiceGroupName(C.ServiceGroupId)ServiceGroupName,C.Id,dbo.GetServiceTypeName(C.ServiceGroupId,C.Id)ServiceTypeName,isnull(T.Price,C.DrValue+C.CsValue+C.CoValue)Price,isnull(T.Payed,C.DrValue+C.CsValue+C.CoValue)Payed,isnull(T.Remaining,0)Remaining,isnull(T.Notes,'')Notes from ServiceTypes C left join ServiceCompanies T on(T.CompanyId='" & txtID.Text.Trim & "' and T.ServiceGroupId=C.ServiceGroupId and T.Id=C.Id)")
        G2.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G2.Rows.Add({dt.Rows(i)("ServiceGroupId"), dt.Rows(i)("ServiceGroupName"), dt.Rows(i)("Id"), dt.Rows(i)("ServiceTypeName"), dt.Rows(i)("Price"), dt.Rows(i)("Payed"), dt.Rows(i)("Remaining"), dt.Rows(i)("Notes")})
        Next
    End Sub


    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        If bm.ShowDeleteMSG("سيتم تحديث البيانات .. استمرار؟") Then
            DeleteDetails()
            FillGrid()
            FillGrid2()
            For i As Integer = 0 To G2.Rows.Count - 1
                G2.Rows(i).Cells(GC2.Price).Value *= (100 - Val(Perc.Text)) / 100
                G2.Rows(i).Cells(GC2.Payed).Value = G2.Rows(i).Cells(GC2.Price).Value
                G2.Rows(i).Cells(GC2.Remaining).Value = 0
            Next

            Dim s As String = txtID.Text
            btnSave_Click(Nothing, Nothing)
            txtID.Text = s
            txtID_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub GridCalcRow2(sender As Object, e As Forms.DataGridViewCellEventArgs)
        Try
            If G2.CurrentCell.ColumnIndex = G2.Columns(GC2.Price).Index OrElse G2.CurrentCell.ColumnIndex = G2.Columns(GC2.Payed).Index Then
                G2.Rows(G2.CurrentCell.RowIndex).Cells(GC2.Price).Value = Val(G2.Rows(G2.CurrentCell.RowIndex).Cells(GC2.Price).Value)
                G2.Rows(G2.CurrentCell.RowIndex).Cells(GC2.Remaining).Value = Val(G2.Rows(G2.CurrentCell.RowIndex).Cells(GC2.Price).Value) - Val(G2.Rows(G2.CurrentCell.RowIndex).Cells(GC2.Payed).Value)
            End If
        Catch
        End Try
    End Sub

    Private Sub btnUpdate2_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate2.Click
        For i As Integer = 0 To G2.Rows.Count - 1
            G2.Rows(i).Cells(GC2.Payed).Value = G2.Rows(i).Cells(GC2.Price).Value * Val(CasePerc.Text) / 100
            G2.Rows(i).Cells(GC2.Remaining).Value = Val(G2.Rows(i).Cells(GC2.Price).Value) - Val(G2.Rows(i).Cells(GC2.Payed).Value)
        Next
    End Sub

    Private Sub G_PreviewKeyDown(sender As Object, e As Forms.PreviewKeyDownEventArgs)
        If e.KeyCode = Forms.Keys.Delete AndAlso G.CurrentRow.Index >= 0 AndAlso bm.ShowDeleteMSG() Then
            G.Rows.Remove(G.CurrentRow)
        End If
    End Sub

    Private Sub G2_PreviewKeyDown(sender As Object, e As Forms.PreviewKeyDownEventArgs)
        If e.KeyCode = Forms.Keys.Delete AndAlso G2.CurrentRow.Index >= 0 AndAlso bm.ShowDeleteMSG() Then
            G2.Rows.Remove(G2.CurrentRow)
        End If
    End Sub

    Private Sub GridCalcRow(sender As Object, e As Forms.DataGridViewCellEventArgs)
        Try
            If G.CurrentCell.ColumnIndex = G.Columns(GC.Price).Index OrElse G.CurrentCell.ColumnIndex = G.Columns(GC.Payed).Index Then
                G.Rows(G.CurrentCell.RowIndex).Cells(GC.Price).Value = Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.Price).Value)
                G.Rows(G.CurrentCell.RowIndex).Cells(GC.Remaining).Value = Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.Price).Value) - Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.Payed).Value)
            End If
        Catch
        End Try
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim frm As New ReportViewer
        frm.Rpt = "CompanyPrices.rpt"
        frm.paraname = {"@CompanyId", "Header"}
        frm.paravalue = {Val(txtID.Text), txtName.Text}
        frm.Show()
    End Sub

End Class
