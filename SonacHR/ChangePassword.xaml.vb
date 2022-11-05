Public Class ChangePassword
    Dim bm As New BasicMethods
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        If Password.Password = "" Then
            Password.Focus()
            Return
        End If
        If PasswordBox1.Password = "" Then
            PasswordBox1.Focus()
            Return
        End If
        If PasswordBox2.Password = "" Then
            PasswordBox2.Focus()
            Return
        End If

        If Password.Password <> Md.Password Then
            bm.ShowMSG("Invalid Password ...")
            Return
        End If
        If PasswordBox1.Password <> PasswordBox2.Password Then
            bm.ShowMSG("Your new password entries did not match.")
            Return
        End If

        If PasswordBox1.Password.Length < 6 Then
            bm.ShowMSG("Password length MUST be 6 characters at least ...")
            Return
        End If
        bm.ExecuteNonQuery("update Employees set Password='" & bm.Encrypt(PasswordBox1.Password) & "' where Id='" & Md.UserName & "'")
        Md.Password = PasswordBox1.Password
        Password.Clear()
        PasswordBox1.Clear()
        PasswordBox2.Clear()
        Password.Focus()
        bm.ShowMSG("Password Changed Successfully.")
    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        
        lblConfirmPassword.SetResourceReference(Label.ContentProperty, "ConfirmPassword")
        lblCurrentPassword.SetResourceReference(Label.ContentProperty, "CurrentPassword")
        lblNewPassword.SetResourceReference(Label.ContentProperty, "NewPassword")
    End Sub

    Private Sub ChangePassword_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
    End Sub
End Class
