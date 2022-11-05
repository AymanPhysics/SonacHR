Imports System.Windows.Media.Animation

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text


Partial Public Class TranslateText
    Inherits UserControl

    Dim bm As New BasicMethods

    Private Sub Grid1_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Grid1.SizeChanged
        Grid1.Height = RealText.ActualWidth * 0.6
    End Sub

    Private Sub TranslateTextAnimationExample_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'cm.Items.Clear()
        'ContextMenu = Nothing
        'CType(Parent, RadioButton).Content.ContextMenu = ContextMenu

        'CType(Parent, RadioButton).Content.Background = System.Windows.Media.Brushes.Transparent()

    End Sub



    'Private Sub TranslateTextAnimationExample_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseRightButtonDown
    'Dim colorDialog1 As New Forms.ColorDialog
    '   If colorDialog1.ShowDialog() = Forms.DialogResult.OK Then
    '      Background = New SolidColorBrush(System.Windows.Media.Color.FromArgb(colorDialog1.Color.A, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B))
    ' End If
    'End Sub

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

