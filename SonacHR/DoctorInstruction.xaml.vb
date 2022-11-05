Imports System.Data

Public Class DoctorInstruction

    Public MyCase As Integer = 0
    Public MyCaseName As String
    WithEvents G As New MyGrid
    Dim bm As New BasicMethods

    Private Sub InFertility_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        LoadWFH()
        LoadCbos()
        DayDate.SelectedDate = bm.MyGetDate
        If Val(bm.ExecuteScalar("select Doctor from Employees where Id=" & Md.UserName)) = 0 Then
            DayDate.IsEnabled = False
            'AimOfTheDay.Visibility = Windows.Visibility.Hidden
            'Notes.Visibility = Windows.Visibility.Hidden
            'lblAimOfTheDay.Visibility = Windows.Visibility.Hidden
            'lblNotes.Visibility = Windows.Visibility.Hidden
            Add3.Visibility = Windows.Visibility.Hidden
            Drug.Visibility = Windows.Visibility.Hidden
            Dose.Visibility = Windows.Visibility.Hidden
            Route.Visibility = Windows.Visibility.Hidden
            lblDrug.Visibility = Windows.Visibility.Hidden
            lblDose.Visibility = Windows.Visibility.Hidden
            lblRoute.Visibility = Windows.Visibility.Hidden
            btnDelete.Visibility = Windows.Visibility.Hidden
            btnReNew.Visibility = Windows.Visibility.Hidden
            WFH.Margin = New Thickness(WFH.Margin.Left, 200, WFH.Margin.Right, WFH.Margin.Bottom)

            AimOfTheDay.IsReadOnly = True
            Notes.IsReadOnly = True

        End If

        lblRoute.Visibility = Windows.Visibility.Hidden
        Route.Visibility = Windows.Visibility.Hidden

        'GetData()
        
        Try
            Visibility = Windows.Visibility.Hidden
            Visibility = Windows.Visibility.Visible

            Dim c = G.Parent
            G.Parent = Nothing
            G.Parent = c
        Catch ex As Exception
        End Try
    End Sub



    Structure GC
        Shared DrugId As String = "DrugId"
        Shared DoseId As String = "DoseId"
        Shared RouteId As String = "RouteId"
        Shared DoctorId As String = "DoctorId"
        Shared Time As String = "Time"
        Shared NurseId As String = "NurseId"
    End Structure


    Private Sub LoadWFH()
        'WFH.Background = New SolidColorBrush(Colors.LightSalmon)
        'WFH.Foreground = New SolidColorBrush(Colors.Red)
        WFH.Child = G
        G.AllowUserToAddRows = False

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        'Dim GCDrugId As New Forms.DataGridViewComboBoxColumn With {.AutoComplete = True}
        'GCDrugId.HeaderText = "Drug"
        'GCDrugId.Name = GC.DrugId
        'bm.FillCombo("select Id,Name from Drugs", GCDrugId)
        'G.Columns.Add(GCDrugId)

        'Dim GCDoseId As New Forms.DataGridViewComboBoxColumn
        'GCDoseId.HeaderText = "Dose"
        'GCDoseId.Name = GC.DoseId
        'bm.FillCombo("select Id,Name from Doses", GCDoseId)
        'G.Columns.Add(GCDoseId)

        'Dim GCRouteId As New Forms.DataGridViewComboBoxColumn
        'GCRouteId.HeaderText = "Route"
        'GCRouteId.Name = GC.RouteId
        'bm.FillCombo("select Id,Name from Routes", GCRouteId)
        'G.Columns.Add(GCRouteId)

        'Dim GCDoctorId As New Forms.DataGridViewComboBoxColumn
        'GCDoctorId.HeaderText = "Doctor"
        'GCDoctorId.Name = GC.DoctorId
        'bm.FillCombo("select Id,EnName Name from Employees", GCDoctorId)
        'G.Columns.Add(GCDoctorId)

        'G.Columns.Add(GC.Time, "Time")

        'Dim GCNurseId As New Forms.DataGridViewComboBoxColumn
        'GCNurseId.HeaderText = "Nurse"
        'GCNurseId.Name = GC.NurseId
        'bm.FillCombo("select Id,EnName Name from Employees", GCNurseId)
        'G.Columns.Add(GCNurseId)

        G.Columns.Add(GC.DrugId, "Drug")
        G.Columns.Add(GC.DoseId, "Dose")
        G.Columns.Add(GC.RouteId, "Route")
        G.Columns.Add(GC.DoctorId, "Doctor")
        G.Columns.Add(GC.Time, "Time")
        G.Columns.Add(GC.NurseId, "Nurse")


        If Val(bm.ExecuteScalar("select Doctor from Employees where Id=" & Md.UserName)) = 0 Then
            G.Columns(GC.DrugId).ReadOnly = True
            G.Columns(GC.DoseId).ReadOnly = True
            G.Columns(GC.RouteId).ReadOnly = True
        End If
        G.Columns(GC.DoctorId).ReadOnly = True
        G.Columns(GC.Time).ReadOnly = True
        G.Columns(GC.NurseId).ReadOnly = True

        G.Columns(GC.RouteId).Visible = False

        G.Columns(GC.Time).FillWeight = 50
        AddHandler G.CellDoubleClick, AddressOf G_CellDoubleClick
    End Sub


    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        
        G.EndEdit()
        bm.SaveGrid(G, "DoctorInstruction", New String() {"CaseId", "DayDate"}, New String() {MyCase, bm.ToStrDate(DayDate.SelectedDate)}, New String() {"DrugId", "DoseId", "RouteId", "DoctorId", "Time", "NurseId"}, New String() {GC.DrugId, GC.DoseId, GC.RouteId, GC.DoctorId, GC.Time, GC.NurseId}, New VariantType() {VariantType.String, VariantType.String, VariantType.String, VariantType.String, VariantType.String, VariantType.String}, New String() {})

        bm.ExecuteNonQuery("update DoctorInstruction set AimOfTheDay='" & AimOfTheDay.Text.Replace("'", "''") & "',Notes='" & Notes.Text.Replace("'", "''") & "' where CaseId=" & MyCase & " and DayDate='" & bm.ToStrDate(DayDate.SelectedDate) & "'")

        Try
            CType(Me.Parent, Window).Close()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Add3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Add3.Click

        bm.AddItemToTable("Drugs", Drug.Text)
        bm.AddItemToTable("Doses", Dose.Text)
        bm.AddItemToTable("Routes", Route.Text)
        G.Rows.Add(Drug.Text, Dose.Text, Route.Text, Md.EnName, "00:00", "")
        Drug.Text = ""
        Dose.Text = ""
        Route.Text = ""

    End Sub

    Private Sub LoadCbos()
        bm.FillCombo("Doses", Dose, "", "")
        bm.FillCombo("Drugs", Drug, "", "")
        bm.FillCombo("Routes", Route, "", "")
    End Sub

    Private Sub G_CellDoubleClick(sender As Object, e As Forms.DataGridViewCellEventArgs)
        If e.ColumnIndex = G.Columns(GC.Time).Index Then
            G.Rows(e.RowIndex).Cells(GC.Time).Value = Now.TimeOfDay.ToString.Substring(0, 5)
            G.Rows(e.RowIndex).Cells(GC.NurseId).Value = Md.EnName
        End If
    End Sub

    Private Sub GetData() Handles DayDate.SelectedDateChanged
        Dim dt As DataTable = bm.ExecuteAdapter("select * from DoctorInstruction where CaseId=" & MyCase & " and DayDate='" & bm.ToStrDate(DayDate.SelectedDate) & "'")
        AimOfTheDay.Clear()
        Notes.Clear()
        If dt.Rows.Count > 0 Then
            AimOfTheDay.Text = dt.Rows(0)("AimOfTheDay").ToString
            Notes.Text = dt.Rows(0)("Notes").ToString
        End If

        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add()
            G.Rows(i).Cells(GC.DrugId).Value = dt.Rows(i)("DrugId").ToString
            G.Rows(i).Cells(GC.DoseId).Value = dt.Rows(i)("DoseId").ToString
            G.Rows(i).Cells(GC.RouteId).Value = dt.Rows(i)("RouteId").ToString
            G.Rows(i).Cells(GC.DoctorId).Value = dt.Rows(i)("DoctorId").ToString
            G.Rows(i).Cells(GC.Time).Value = dt.Rows(i)("Time").ToString
            G.Rows(i).Cells(GC.NurseId).Value = dt.Rows(i)("NurseId").ToString
        Next

    End Sub

    Private Sub ReNew_Click(sender As Object, e As RoutedEventArgs) Handles btnReNew.Click
        Dim dt As DataTable = bm.ExecuteAdapter("select * from DoctorInstruction where CaseId=" & MyCase & " and DayDate=(select max(DayDate) from DoctorInstruction where CaseId=" & MyCase & " and DayDate<'" & bm.ToStrDate(DayDate.SelectedDate) & "')")
        AimOfTheDay.Clear()
        Notes.Clear()
        If dt.Rows.Count > 0 Then
            AimOfTheDay.Text = dt.Rows(0)("AimOfTheDay").ToString
            Notes.Text = dt.Rows(0)("Notes").ToString
        End If

        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add()
            G.Rows(i).Cells(GC.DrugId).Value = dt.Rows(i)("DrugId").ToString
            G.Rows(i).Cells(GC.DoseId).Value = dt.Rows(i)("DoseId").ToString
            G.Rows(i).Cells(GC.RouteId).Value = dt.Rows(i)("RouteId").ToString
            G.Rows(i).Cells(GC.DoctorId).Value = dt.Rows(i)("DoctorId").ToString
            G.Rows(i).Cells(GC.Time).Value = "00:00"
            G.Rows(i).Cells(GC.NurseId).Value = ""
        Next

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnDelete.Click
        Try
            G.Rows.RemoveAt(G.CurrentRow.Index)
        Catch ex As Exception
        End Try
    End Sub
End Class
