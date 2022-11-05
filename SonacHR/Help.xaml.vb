Imports System.Data

Public Class Help
    Dim bm As New BasicMethods
    Public FirstColumn As String = "ID", SecondColumn As String = "Name", ThirdColumn As String = "Permission", Statement As String = "", TableName As String = ""
    Public MyFields() As String = Nothing, MyValues() As String = Nothing

    Public IsPermissions As Boolean = False
    Public EmpId As Integer = 0
    Dim dt As New DataTable
    Dim dv As New DataView
    Public Header As String = ""
    Public LinkFile As Integer
    Public Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Banner1.StopTimer = True
        Banner1.Header = Header
        If TableName = "" OrElse IsPermissions Then
            btnSave.Visibility = Visibility.Hidden
        End If
        Try
            dt = bm.ExecuteAdapter(Statement & IIf(Statement.ToLower.Contains("Top 10".ToLower), " where Id like '%" & txtID.Text & "%' and name like '%" & txtName.Text & "%'", ""))
            dt.TableName = "tbl"
            dt.Columns(0).ColumnName = IIf(Application.Current.MainWindow.Resources.Item(FirstColumn) Is Nothing, FirstColumn, Application.Current.MainWindow.Resources.Item(FirstColumn))
            dt.Columns(1).ColumnName = IIf(Application.Current.MainWindow.Resources.Item(SecondColumn) Is Nothing, SecondColumn, Application.Current.MainWindow.Resources.Item(SecondColumn))
            
            DataGridView1.Foreground = System.Windows.Media.Brushes.Black
            dv.Table = dt
            DataGridView1.ItemsSource = dv
            DataGridView1.Columns(0).Width = 120
            DataGridView1.Columns(1).Width = 300
            If dt.Columns.Count = 3 Then
                DataGridView1.Columns(0).Width = 100
                DataGridView1.Columns(1).Width = 220
                DataGridView1.Columns(2).Width = 100

            End If
            DataGridView1.SelectedIndex = 0
        Catch
        End Try


        'If Md.MyProject = Client.ClothesRed Then
        '    DataGridView1.Foreground = System.Windows.Media.Brushes.Red

        '    Banner1.R.Fill = System.Windows.Media.Brushes.Red
        '    Banner1.lblMain.Foreground = System.Windows.Media.Brushes.White
        '    Banner1.Background = System.Windows.Media.Brushes.White
        '    Banner1.Foreground = System.Windows.Media.Brushes.Red
        'End If
        Try
            If IsPermissions Then
                dt.Columns(2).ColumnName = IIf(Application.Current.MainWindow.Resources.Item(ThirdColumn) Is Nothing, ThirdColumn, Application.Current.MainWindow.Resources.Item(ThirdColumn))
                DataGridView1.Columns(0).IsReadOnly = True
                DataGridView1.Columns(1).IsReadOnly = True
            Else
                DataGridView1.IsReadOnly = True
                btnSavePermission.Visibility = Windows.Visibility.Hidden
                All.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception

        End Try

        If Not _IsLoaded Then
            _IsLoaded = True
            txtName.Focus()
        End If
    End Sub
    Dim _IsLoaded As Boolean = False

    Private Sub txtId_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.GotFocus
        Try
            dv.Sort = FirstColumn
        Catch
        End Try
    End Sub

    Private Sub txtName_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtName.GotFocus
        Try
            dv.Sort = SecondColumn
        Catch
        End Try
    End Sub

    Private Sub txtId_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.TextChanged, txtName.TextChanged
        Try
            dv.RowFilter = " [" & dt.Columns(0).ColumnName & "] like '%" & txtID.Text & "%' and [" & dt.Columns(1).ColumnName & "] like '%" & txtName.Text & "%'"

            If Statement.ToLower.Contains("Top 10".ToLower) Then Window_Loaded(Nothing, Nothing)
        Catch ex As Exception
            ex = ex
        End Try
    End Sub

    Public SelectedId As String = 0
    Public SelectedName As String = ""
    Public SelectedRow As System.Data.DataRowView
    Private Sub DataGridView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyDown
        Try
            If e.Key = System.Windows.Input.Key.Enter AndAlso Not IsPermissions Then
                SelectedId = DataGridView1.Items(DataGridView1.SelectedIndex)(0)
                SelectedName = DataGridView1.Items(DataGridView1.SelectedIndex)(1)
                SelectedRow = DataGridView1.Items(DataGridView1.SelectedIndex)
                Close()
            ElseIf e.Key = Input.Key.Escape Then
                Close()
            ElseIf e.Key = Input.Key.Up Then
                DataGridView1.SelectedIndex = DataGridView1.SelectedIndex - 1
                DataGridView1.ScrollIntoView(DataGridView1.SelectedItem)
            ElseIf e.Key = Input.Key.Down Then
                DataGridView1.SelectedIndex = DataGridView1.SelectedIndex + 1
                DataGridView1.ScrollIntoView(DataGridView1.SelectedItem)
            End If
        Catch ex As Exception
        End Try
    End Sub


    Private Sub DataGridView1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles DataGridView1.MouseDoubleClick
        Try
            If IsPermissions Then Return
            SelectedId = DataGridView1.Items(DataGridView1.SelectedIndex)(0)
            SelectedName = DataGridView1.Items(DataGridView1.SelectedIndex)(1)
            SelectedRow = DataGridView1.Items(DataGridView1.SelectedIndex)
            Close()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Try
            If txtName.Text.Trim = "" Then Return
            txtID.Clear()
            For i As Integer = 0 To dv.Table.Rows.Count - 1
                If txtName.Text.Trim = dv.Table.Rows(i).Item(1).ToString Then Return
            Next
            txtName.Text = txtName.Text.Trim
            If Not bm.AddItemToTable(TableName, txtName.Text.Trim, MyFields, MyValues) Then Return
            Window_Loaded(Nothing, Nothing)
            DataGridView1.SelectedIndex = dv.Table.Rows.Count - 1
            txtId_TextChanged(Nothing, Nothing)
        Catch
        End Try
    End Sub

    Private Sub LoadResource()
        'lblName.SetResourceReference(Label.ContentProperty, "Name")
    End Sub

    Private Sub btnSavePermission_Click(sender As Object, e As RoutedEventArgs) Handles btnSavePermission.Click
        Dim str As String = "delete " & TableName & " where EmpId=" & EmpId
        If LinkFile <> 0 Then
            str &= " and LinkFile=" & LinkFile
        End If
        bm.ExecuteNonQuery(Str)
        For i As Integer = 0 To DataGridView1.Items.Count - 1
            If DataGridView1.Items(i)(2) Then
                str = "insert " & TableName & " select " & IIf(LinkFile = 0, "", LinkFile.ToString & ",") & EmpId & "," & DataGridView1.Items(i)(0)
                bm.ExecuteNonQuery(str)
            End If
        Next

        Close()
    End Sub

    Private Sub All_Checked(sender As Object, e As RoutedEventArgs) Handles All.Checked, All.Unchecked
        If All.Visibility = Visibility.Hidden Then Return
        For i As Integer = 0 To DataGridView1.Items.Count - 1
            DataGridView1.Items(i)(2) = All.IsChecked
        Next
    End Sub
End Class