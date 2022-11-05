Imports System.Data

Public Class EditMenha
    Dim dt As New DataTable
    Dim bm As New BasicMethods
    WithEvents G As New MyGrid


    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return


        LoadResource()
        LoadWFH()
        
        CompanyId_LostFocus(Nothing, Nothing)

    End Sub

    Structure GC
        Shared EmpId As String = "EmpId"
        Shared EmpName As String = "EmpName"
        Shared Menha As String = "Menha"
    End Structure


    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue
        G.Columns.Add(GC.EmpId, "كود الموظف")
        G.Columns.Add(GC.EmpName, "اسم الموظف")
        G.Columns.Add(GC.Menha, "منحة")

        G.Columns(GC.EmpId).ReadOnly = True
        G.Columns(GC.EmpName).ReadOnly = True
        
        G.Columns(GC.EmpName).FillWeight = 300

        G.AutoSizeColumnsMode = Forms.DataGridViewAutoSizeColumnsMode.Fill
        G.AllowUserToDeleteRows = False
        G.AllowUserToAddRows = False

        G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
        G.TabStop = False
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        
        G.EndEdit()

        Dim str As String = ""
        For i As Integer = 0 To G.Rows.Count - 1
            Try
                str &= " update Employees set Menha='" & G.Rows(i).Cells(GC.Menha).Value.ToString & "' where Id='" & G.Rows(i).Cells(GC.EmpId).Value.ToString & "' "
            Catch ex As Exception
            End Try
        Next
        
        If Not bm.ExecuteNonQuery(str) Then Return

        bm.ShowMSG("تم الحفظ بنجاح")

        btnNew_Click(sender, e)

    End Sub


    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        G.Rows.Clear()
        dt = bm.ExecuteAdapter("select Id,Name,Menha from Employees where (CompanyId=" & Val(CompanyId.Text) & " or " & Val(CompanyId.Text) & "=0)")
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add({dt.Rows(i)(0).ToString, dt.Rows(i)(1).ToString, dt.Rows(i)(2).ToString})
        Next
        G.RefreshEdit()
    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

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
            G.Rows(i).Cells(GC.Menha).Value = Val(Value.Text)
        Next
        G.EndEdit()
        G.CurrentCell = G.Rows(0).Cells(0)
    End Sub

End Class
