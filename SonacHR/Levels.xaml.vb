Imports System.Data

Public Class Levels
    Public TableName As String = "Levels"
    Public SubId As String = "Id"
    Public SubName As String = "Name"



    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Public Flag As Integer = 0
    Public WithImage As Boolean = False

    Private Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.TestSecurity(Me, {btnSave}, {btnDelete}, {btnFirst, btnNext, btnPrevios, btnLast}, {})
        LoadResource()
        bm.Fields = New String() {SubId, SubName}
        bm.control = New Control() {txtID, txtName}
        bm.KeyFields = New String() {SubId}
        bm.Table_Name = TableName
        LoadTree()
        btnNew_Click(sender, e)
    End Sub




    Sub LoadTree()
        Dim Main As New MainPage
        bm.TestIsLoaded(Main)
        Main.Lvl = True
        Main.Load()
        TreeView1.Items.Clear()
        TreeView1.Items.Add(New TreeViewItem With {.Header = New CheckBox With {.Content = Resources.Item("Contents"), .IsTabStop = False}})
        AddHandler CType(TreeView1.Items(0).Header, CheckBox).Checked, AddressOf CheckedChanged
        AddHandler CType(TreeView1.Items(0).Header, CheckBox).Unchecked, AddressOf CheckedChanged
        For i As Integer = 0 To Main.tab.Items.Count - 1
            Try
                Dim item As TabItem
                item = Main.tab.Items(i)

                If item.Tag Is Nothing OrElse item.Tag = "" Then Continue For

                If item.Tag Is Nothing Then Continue For
                If item.Tag = "" Then Continue For
                Dim nn As New TreeViewItem
                nn.Name = item.Name
                nn.Header = New CheckBox With {.Content = item.Header, .IsTabStop = False}
                TreeView1.Items(0).Items.Add(nn)
                loadNode(item, nn)
                AddHandler CType(nn.Header, CheckBox).Checked, AddressOf CheckedChanged
                AddHandler CType(nn.Header, CheckBox).Unchecked, AddressOf CheckedChanged
            Catch
            End Try
        Next
        CType(TreeView1.Items(0), TreeViewItem).IsExpanded = True
    End Sub

    Sub loadNode(ByVal item As TabItem, ByVal nn As TreeViewItem)
        For i As Integer = 0 To CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children.Count - 1
            Try
                Dim item2 As New Object
                item2 = CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children(i)
                'If Not item2.Tag Is Nothing Then Continue For
                Dim nn2 As New TreeViewItem
                nn2.Name = item2.Name
                If TypeOf (item2.Content) Is TranslateTextAnimationExample Then
                    nn2.Header = New CheckBox With {.Content = CType(item2.Content, TranslateTextAnimationExample).RealText.Text, .IsTabStop = False}
                    loadNodeSub(item, nn2)
                ElseIf TypeOf (item2.Content) Is TranslateText Then
                    nn2.Header = New CheckBox With {.Content = CType(item2.Content, TranslateText).RealText.Text, .IsTabStop = False}
                ElseIf TypeOf (item2) Is Label Then
                    nn2.Header = New CheckBox With {.Content = "***** " & CType(item2, Label).Content.ToString.Trim & " *****", .IsTabStop = False}
                End If
                nn.Items.Add(nn2)
                AddHandler CType(nn2.Header, CheckBox).Checked, AddressOf CheckedChanged
                AddHandler CType(nn2.Header, CheckBox).Unchecked, AddressOf CheckedChanged
            Catch
            End Try
        Next
    End Sub

    Sub loadNode(ByVal item As MenuItem, ByVal nn As TreeViewItem)
        For i As Integer = 0 To item.Items.Count - 1
            Try
                Dim item2 As MenuItem
                item2 = item.Items(i)
                If Not item2.Tag Is Nothing And Not item2.Tag = "" Then Continue For
                Dim nn2 As New TreeViewItem
                nn2.Name = item2.Name
                nn2.Header = New CheckBox With {.Content = item2.Header, .IsTabStop = False}
                nn.Items.Add(nn2)
                loadNode(item2, nn2)
                AddHandler CType(nn2.Header, CheckBox).Checked, AddressOf CheckedChanged
                AddHandler CType(nn2.Header, CheckBox).Unchecked, AddressOf CheckedChanged
            Catch
            End Try
            Try
                Dim item2 As Separator
                item2 = item.Items(i)
                If Not item2.Tag Is Nothing And Not item2.Tag = "" Then Continue For
                Dim nn2 As New TreeViewItem
                nn2.Name = item2.Name
                nn2.Header = (New CheckBox With {.Content = "ـــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ", .IsTabStop = False})
                nn.Items.Add(nn2)
            Catch
            End Try
        Next
    End Sub


    Sub loadNodeSub(ByVal item As TabItem, ByVal nn As TreeViewItem)
        Try
            Dim nn1 As New TreeViewItem With {.Name = "AllowEdit", .Header = New CheckBox With {.Content = "تعديل", .IsTabStop = False}}
            nn.Items.Add(nn1)

            Dim nn2 As New TreeViewItem With {.Name = "AllowDelete", .Header = New CheckBox With {.Content = "مسح", .IsTabStop = False}}
            nn.Items.Add(nn2)

            Dim nn3 As New TreeViewItem With {.Name = "AllowNavigate", .Header = New CheckBox With {.Content = "إبحار", .IsTabStop = False}}
            nn.Items.Add(nn3)

            Dim nn4 As New TreeViewItem With {.Name = "AllowPrint", .Header = New CheckBox With {.Content = "طباعة", .IsTabStop = False}}
            nn.Items.Add(nn4)

            AddHandler CType(nn1.Header, CheckBox).Checked, AddressOf CheckedChanged
            AddHandler CType(nn1.Header, CheckBox).Unchecked, AddressOf CheckedChanged

            AddHandler CType(nn2.Header, CheckBox).Checked, AddressOf CheckedChanged
            AddHandler CType(nn2.Header, CheckBox).Unchecked, AddressOf CheckedChanged

            AddHandler CType(nn3.Header, CheckBox).Checked, AddressOf CheckedChanged
            AddHandler CType(nn3.Header, CheckBox).Unchecked, AddressOf CheckedChanged

            AddHandler CType(nn4.Header, CheckBox).Checked, AddressOf CheckedChanged
            AddHandler CType(nn4.Header, CheckBox).Unchecked, AddressOf CheckedChanged
        Catch
        End Try
    End Sub


    Sub FillControls()
        bm.FillControls(Me)
        Dim dtLevelsMenuitems As DataTable = bm.ExecuteAdapter("select * from LevelsMenuitems where LevelId='" & txtID.Text & "'")
        Dim dtLevelsTabs As DataTable = bm.ExecuteAdapter("select * from LevelsTabs where LevelId='" & txtID.Text & "'")
        Try
            CType(TreeView1.Items(0).Header, CheckBox).IsChecked = True
            For Each nn As TreeViewItem In TreeView1.Items(0).Items
                Try
                    CType(nn.Header, CheckBox).IsChecked = dtLevelsTabs.Select("Id=" & nn.Name.Replace("tab", "")).Length > 0
                Catch ex As Exception
                    CType(nn.Header, CheckBox).IsChecked = False
                End Try
                FillSubNode(dtLevelsMenuitems, nn)
            Next
        Catch
        End Try
    End Sub

    Sub FillSubNode(ByVal dt As DataTable, ByVal nn As TreeViewItem)
        If nn.Name.Contains("menuitem") Then Return
        Try
            For Each nn2 As TreeViewItem In nn.Items
                Try
                    If dt.Select("Id=" & nn2.Name.Replace("menuitem", "")).Length > 0 Then
                        CType(nn2.Header, CheckBox).IsChecked = True
                        'AllowEdit, AllowDelete, AllowNavigate, AllowPrint
                        If nn2.Items.Count > 0 Then
                            CType(nn2.Items(0).Header, CheckBox).IsChecked = IIf(dt.Select("Id=" & nn2.Name.Replace("menuitem", "")).ToList(0)("AllowEdit") = 1, True, False)
                            CType(nn2.Items(1).Header, CheckBox).IsChecked = IIf(dt.Select("Id=" & nn2.Name.Replace("menuitem", "")).ToList(0)("AllowDelete") = 1, True, False)
                            CType(nn2.Items(2).Header, CheckBox).IsChecked = IIf(dt.Select("Id=" & nn2.Name.Replace("menuitem", "")).ToList(0)("AllowNavigate") = 1, True, False)
                            CType(nn2.Items(3).Header, CheckBox).IsChecked = IIf(dt.Select("Id=" & nn2.Name.Replace("menuitem", "")).ToList(0)("AllowPrint") = 1, True, False)
                        End If
                    Else
                        CType(nn2.Header, CheckBox).IsChecked = False
                    End If
                Catch ex As Exception
                    CType(nn2.Header, CheckBox).IsChecked = False
                End Try
                FillSubNode(dt, nn2)
            Next
        Catch ex As Exception
        End Try
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
        bm.DefineValues()
        If Not bm.Save(New String() {SubId}, New String() {txtID.Text.Trim}) Then Return
        SaveTree()
        btnNew_Click(sender, e)
    End Sub

    Sub SaveTree()
        Dim ss As String = " delete LevelsMenuitems where LevelId=" & txtID.Text
        ss &= " delete LevelsTabs where LevelId=" & txtID.Text

        Dim Chk As CheckBox
        For Each nn As TreeViewItem In TreeView1.Items(0).Items
            If nn.Name = "" Then Continue For
            Chk = nn.Header
            If Chk.IsChecked Then
                ss &= " insert LevelsTabs(LevelId,Id) select " & txtID.Text & "," & nn.Name.Replace("tab", "") 
            End If
            GetSubNode(nn, ss)
        Next

        bm.ExecuteNonQuery(ss)
    End Sub

    Sub GetSubNode(ByVal nn As TreeViewItem, ByRef ss As String)
        Dim Chk As CheckBox
        For Each nn2 As TreeViewItem In nn.Items
            Chk = nn2.Header
            If Chk.IsChecked Then
                ss &= " insert LevelsMenuitems(LevelId,Id,AllowEdit,AllowDelete,AllowNavigate,AllowPrint) select " &
txtID.Text & "," & nn2.Name.Replace("menuitem", "")
                If nn2.Items.Count > 0 AndAlso CType(CType(nn2.Items(0), TreeViewItem).Header, CheckBox).IsChecked Then
                    ss &= ",1"
                Else
                    ss &= ",0"
                End If
                If nn2.Items.Count > 0 AndAlso CType(CType(nn2.Items(1), TreeViewItem).Header, CheckBox).IsChecked Then
                    ss &= ",1"
                Else
                    ss &= ",0"
                End If
                If nn2.Items.Count > 0 AndAlso CType(CType(nn2.Items(2), TreeViewItem).Header, CheckBox).IsChecked Then
                    ss &= ",1"
                Else
                    ss &= ",0"
                End If
                If nn2.Items.Count > 0 AndAlso CType(CType(nn2.Items(3), TreeViewItem).Header, CheckBox).IsChecked Then
                    ss &= ",1"
                Else
                    ss &= ",0"
                End If
            End If
        Next
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
        Try
            txtName.Clear()
            txtID.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName)
            If txtID.Text = "" Then txtID.Text = "1"
            CType(CType(TreeView1.Items(0), TreeViewItem).Header, CheckBox).IsChecked = True
            CType(CType(TreeView1.Items(0), TreeViewItem).Header, CheckBox).IsChecked = False
            'TreeView1_AfterCheck(Nothing, Nothing)
            txtName.Focus()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ClearSubNode(ByVal nn As TreeViewItem)
        Try
            For Each nn2 As TreeViewItem In nn.Items
                CType(nn2.Header, CheckBox).IsChecked = False
                ClearSubNode(nn2)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExecuteNonQuery("delete from " & TableName & " where " & SubId & "='" & txtID.Text.Trim & "'")
            btnNew_Click(sender, e)
        End If
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {SubId}, New String() {txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False

    Private Sub txtID_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {SubId}, New String() {txtID.Text.Trim}, dt)
        If dt.Rows.Count = 0 Then
            ClearControls()
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


    Private Sub txtID_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyUp
        If bm.ShowHelp(CType(Parent, Page).Title, txtID, txtName, e, "select cast(Id as varchar(100)) Id,Name from " & TableName, TableName) Then
            txtName.Focus()
        End If
    End Sub


    Dim lop As Boolean = False
    Private Sub CheckedChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ch As CheckBox = sender
        Dim p As TreeViewItem = ch.Parent

        If Not lop Then
            p.Focus()
            For Each n As TreeViewItem In p.Items
                CType(n.Header, CheckBox).IsChecked = ch.IsChecked
            Next
        End If

        If p.Parent.GetType.ToString = "System.Windows.Controls.TreeViewItem" Then
            lop = True
            Dim PP As TreeViewItem = p.Parent
            If ch.IsChecked Then CType(PP.Header, CheckBox).IsChecked = True
            lop = False
        End If
    End Sub





    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")

        lblId.SetResourceReference(Button.ContentProperty, "Id")
        lblName.SetResourceReference(Button.ContentProperty, "Name")

    End Sub
End Class
