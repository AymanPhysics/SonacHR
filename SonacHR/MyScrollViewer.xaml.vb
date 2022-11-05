Public Class MyScrollViewer

    Private Sub ScrollViewer_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs) Handles MyBase.PreviewMouseWheel
        If (e.Delta > 0) Then
            LineLeft()
            LineLeft()
            LineLeft()
            LineLeft()
            LineLeft()
        Else
            LineRight()
            LineRight()
            LineRight()
            LineRight()
            LineRight()
        End If
        e.Handled = True
    End Sub
End Class
