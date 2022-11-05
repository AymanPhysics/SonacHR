Imports System
Imports System.Threading
Imports System.Windows.Threading

Public Class MSG
    Dim bm As New BasicMethods
    Public Ok As Boolean
    Public DelMsg As Boolean = False
    Public MSG As String

    Public AutoHide As Boolean = False
    Public lblYes As String = "Yes"
    Public lblNo As String = "No"

    Dim t As New DispatcherTimer With {.IsEnabled = False, .Interval = New TimeSpan(0, 0, 0, 0, 100)}
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

        AddHandler t.Tick, AddressOf Tick
        If AutoHide Then
            t.Start()
        End If
        'lblMSG.wrap()
    End Sub

    Public Sub Tick()
        Dim f As Window = Parent
        Opacity -= 0.1
        If Math.Round(Opacity, 2) = 0.1 Then
            Close()
        End If
    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNo.Click
        Ok = False
        Close()
    End Sub

    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnYes.Click
        Ok = True
        Close()
    End Sub

    Private Sub LoadResource()
        btnYes.SetResourceReference(Button.ContentProperty, lblYes)
        btnNo.SetResourceReference(Button.ContentProperty, lblNo)

    End Sub
End Class
