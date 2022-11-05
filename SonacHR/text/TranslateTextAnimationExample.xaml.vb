Imports System.Windows.Media.Animation

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Xml


Partial Public Class TranslateTextAnimationExample
    Inherits UserControl

    Dim m As MainWindow = Application.Current.MainWindow
    Dim bm As New BasicMethods

    Private Sub Grid1_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Grid1.SizeChanged
        ReflectedText.Visibility = Windows.Visibility.Collapsed
        Grid1.Height = RealText.ActualWidth * 0.4
    End Sub

    Public Sub SetBackground(MyTag As Integer)
        Select Case MyTag
            Case 1
                Background = MenuItem1.Background
            Case 2
                Background = MenuItem2.Background
            Case 3
                Background = MenuItem3.Background
            Case 4
                Background = MenuItem4.Background
            Case 5
                Background = MenuItem5.Background
        End Select
        CType(Parent, RadioButton).Background = Background
        CType(Parent, RadioButton).Content.Background = System.Windows.Media.Brushes.Transparent
    End Sub
    Private Sub MenuItem1_Click(sender As Object, e As RoutedEventArgs) Handles MenuItemHigh.Click, MenuItemLow.Click, MenuItemHigh2.Click, MenuItemLow2.Click, MenuItem1.Click, MenuItem2.Click, MenuItem3.Click, MenuItem4.Click, MenuItem5.Click
        If sender Is MenuItemHigh Then
            CType(Parent, Control).Width *= 2
            save("w" & CType(Parent, Control).Name.Replace("menuitem", ""), CType(Parent, Control).Width)
        ElseIf sender Is MenuItemLow Then
            CType(Parent, Control).Width /= 2
            save("w" & CType(Parent, Control).Name.Replace("menuitem", ""), CType(Parent, Control).Width)
        ElseIf sender Is MenuItemHigh2 Then
            CType(Parent, Control).Height *= 2
            save("h" & CType(Parent, Control).Name.Replace("menuitem", ""), CType(Parent, Control).Height)
        ElseIf sender Is MenuItemLow2 Then
            CType(Parent, Control).Height /= 2
            save("h" & CType(Parent, Control).Name.Replace("menuitem", ""), CType(Parent, Control).Height)
        Else
            SetBackground(sender.tag)
            save("c" & CType(Parent, Control).Name.Replace("menuitem", ""), sender.tag)
        End If

    End Sub

    Sub save(f As String, v As String)
        bm.ExecuteNonQuery("if not exists(select id from PLevels where id=" & Md.UserName & " and f='" & f & "') insert PLevels(id,f) select " & Md.UserName & ",'" & f & "' update PLevels set v='" & v & "' where id=" & Md.UserName & " and f='" & f & "'")
    End Sub

End Class

