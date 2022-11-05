Imports System.Data

Public Class HelpMultiColumns2
    Dim bm As New BasicMethods
    Public Statement As String = ""
    Public MyColumnNames() As String
    Dim MyTextBoxes() As TextBox = {}

    Dim dt As New DataTable
    Dim dv As New DataView
    Public Header As String = ""
    Public LinkFile As Integer
    Public Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Banner1.StopTimer = True
        Banner1.Header = Header
        dt = bm.ExecuteAdapter(Statement)
        dt.TableName = "tbl"
        DataGridView1.Foreground = System.Windows.Media.Brushes.Black
        dv.Table = dt
        DataGridView1.ItemsSource = dv

        Dim t As New System.Windows.Threading.DispatcherTimer With {.IsEnabled = True, .Interval = New System.TimeSpan(0, 0, 1)}
        AddHandler t.Tick, AddressOf t_Tick
        't_Tick(Nothing, Nothing)
    End Sub

    Private Sub txt_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            dv.Sort = MyColumnNames(sender.Tag)
        Catch
        End Try
    End Sub

    Private Sub txt_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            dv.RowFilter = " 1=1"
            For i As Integer = 0 To dt.Columns.Count - 1
                dv.RowFilter &= " and [" & dt.Columns(i).ColumnName & "] like '%" & MyTextBoxes(i).Text & "%' "
            Next
        Catch
        End Try
    End Sub

    Public SelectedId As String = 0
    Public SelectedName As String = ""
    Public SelectedRow As System.Data.DataRowView
    Private Sub DataGridView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyDown
        Try
            If e.Key = System.Windows.Input.Key.Enter Then
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
            SelectedId = DataGridView1.Items(DataGridView1.SelectedIndex)(0)
            SelectedName = DataGridView1.Items(DataGridView1.SelectedIndex)(1)
            SelectedRow = DataGridView1.Items(DataGridView1.SelectedIndex)
            Close()
        Catch ex As Exception
        End Try
    End Sub


    Private Sub LoadResource()
    End Sub

    Private Sub t_Tick(sender As Object, e As EventArgs)
        If Not sender Is Nothing Then CType(sender, System.Windows.Threading.DispatcherTimer).Stop()

        Dim CurrentActualWidth As Integer = 0
        Try
            For i As Integer = 0 To dt.Columns.Count - 1
                If MyColumnNames.Length > i Then
                    dt.Columns(i).ColumnName = IIf(Application.Current.MainWindow.Resources.Item(MyColumnNames(i)) Is Nothing, MyColumnNames(i), Application.Current.MainWindow.Resources.Item(MyColumnNames(i)))
                End If

                Dim txt As New TextBox With {.Height = 30, .Margin = New Thickness(23 + CurrentActualWidth, 10, 10, 10)}
                ReDim Preserve MyTextBoxes(MyTextBoxes.Length + 1)
                MyTextBoxes(i) = txt
                Dim d = DataGridView1.Columns(i).ActualWidth
                txt.Width = d
                CurrentActualWidth += DataGridView1.Columns(i).ActualWidth
                txt.Tag = i
                txt.TabIndex = i
                txt.HorizontalAlignment = Windows.HorizontalAlignment.Left
                txt.VerticalAlignment = Windows.VerticalAlignment.Top
                SC.Children.Add(txt)
                AddHandler txt.GotFocus, AddressOf txt_Enter
                AddHandler txt.TextChanged, AddressOf txt_TextChanged
            Next
            DataGridView1.SelectedIndex = 0
        Catch
        End Try
        DataGridView1.IsReadOnly = True

    End Sub

End Class