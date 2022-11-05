Public Class TabsHeader
    Public MyTabHeader As String
    Public MyTabName As String
    Public WithClose As Visibility
    Dim bm As New BasicMethods

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        bm.CloseTab(MyTabName)
    End Sub

    Dim WithEvents p As TabControl
    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        p = CType(CType(Parent, TabItem).Parent, TabControl)
        TextBlock1.Foreground = New SolidColorBrush(Colors.LightGray)
        TextBlock1.Text = MyTabHeader
        Button1.Visibility = WithClose
        Button1.IsTabStop = False
        If Button1.Visibility = Windows.Visibility.Hidden Then
            TextBlock1.Margin = New Thickness(0)
        End If
    End Sub
    Private Sub TabControl1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles p.SelectionChanged
        Try
            CType(CType(e.AddedItems(0), TabItem).Header, TabsHeader).TextBlock1.Foreground = New SolidColorBrush(Colors.LightGray)
        Catch ex As Exception
        End Try
        Try
            CType(CType(e.RemovedItems(0), TabItem).Header, TabsHeader).TextBlock1.Foreground = New SolidColorBrush(Colors.Black)
        Catch ex As Exception
        End Try
    End Sub

End Class
