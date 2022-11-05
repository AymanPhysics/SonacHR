Public Class MyWindow

    Public MySecurityType As New SecurityType
    Structure SecurityType
        Shared AllowEdit As Boolean = False
        Shared AllowDelete As Boolean = False
        Shared AllowNavigate As Boolean = False
        Shared AllowPrint As Boolean = False
    End Structure

    Private Sub MyWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If WindowState = Windows.WindowState.Minimized Then
            WindowState = Windows.WindowState.Maximized
        End If
    End Sub
    Private Sub MyWindow_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles Me.PreviewKeyDown
        Try
            If e.Key = System.Windows.Input.Key.Enter Then
                'e.Handled = True
                If FocusManager.GetFocusedElement(Me).GetType = GetType(Button) Then Return
                If FocusManager.GetFocusedElement(Me).GetType = GetType(Forms.Integration.WindowsFormsHost) Then Return
                If FocusManager.GetFocusedElement(Me).GetType = GetType(TextBox) Then
                    If CType(FocusManager.GetFocusedElement(Me), TextBox).VerticalScrollBarVisibility = ScrollBarVisibility.Visible Then Return
                End If
                Dim c As Control = FocusManager.GetFocusedElement(Me)
                InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) With {.RoutedEvent = Keyboard.KeyDownEvent})
                e.Handled = True
                'c.Focus()
                InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter) With {.RoutedEvent = Keyboard.KeyDownEvent})
                'If FocusManager.GetFocusedElement(Me).GetType = GetType(TextBox) AndAlso Not CType(FocusManager.GetFocusedElement(Me), TextBox).VerticalScrollBarVisibility = ScrollBarVisibility.Visible Then CType(FocusManager.GetFocusedElement(Me), TextBox).SelectAll()
            End If
        Catch
        End Try
    End Sub
End Class
