Public Class MyPage

    Public MySecurityType As New SecurityType
    Public Structure SecurityType
        Public AllowEdit As Boolean
        Public AllowDelete As Boolean
        Public AllowNavigate As Boolean
        Public AllowPrint As Boolean
    End Structure

    Private Sub MyPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        

    End Sub

End Class
