Public Class TimePicker
    Dim bm As New BasicMethods
    Public WithEvents TPK As New Forms.DateTimePicker
    Dim V As String
    Property Time 
        Get
            Return V
        End Get
        Set(ByVal value)
            V = value
            TPK.Text = value
        End Set
    End Property

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        WindowsFormsHost1.Child = TPK
        TPK.Format = Forms.DateTimePickerFormat.Custom
        TPK.CustomFormat = "hh:mm tt"
        TPK.ShowUpDown = True
        TPK.RightToLeft = Forms.RightToLeft.No
        TPK.Value = bm.MyGetDate()
        AddHandler TPK.ValueChanged, AddressOf TPKValueChanged
    End Sub


    Private Sub TPKValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Time = TPK.Text
        Catch ex As Exception
        End Try
    End Sub

End Class
