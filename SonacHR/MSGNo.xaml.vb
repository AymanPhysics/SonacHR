Imports System
Imports System.Threading
Public Class MSGNo
    Dim bm As New BasicMethods
    Public Ok As Boolean
    Public DelMsg As Boolean = False
    Public MSG As String
    Public Value As String

    Public lblYes As String = "Yes"
    Public lblNo As String = "No"

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Ok = False
        If Not DelMsg Then
            btnNo.Width = 0
            btnNo.Height = 0
            btnYes.Content = Application.Current.MainWindow.Resources.Item("Exit")
            'btnYes.Focus()
        End If
        lblMSG.Text = MSG
        CashValue.Text = Value
        'lblMSG.wrap()
    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNo.Click
        Ok = False
        Close()
    End Sub

    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnYes.Click
        Ok = True
        Value = CashValue.Text
        Close()
    End Sub

    Private Sub LoadResource()
        btnYes.SetResourceReference(Button.ContentProperty, lblYes)
        btnNo.SetResourceReference(Button.ContentProperty, lblNo)

    End Sub

    Private Sub CashValue_TextChanged(sender As Object, e As TextChangedEventArgs) Handles CashValue.TextChanged

    End Sub
End Class
