' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Threading

Namespace EmployeeTracker
    ''' <summary>
    ''' Interaction logic for '.xaml
    ''' </summary>
    Partial Public Class BlankBanner
        Inherits UserControl

        Dim t As New DispatcherTimer With {.IsEnabled = True, .Interval = New TimeSpan(0, 0, 0, 0, 100)}

        Public Sub New()
            InitializeComponent()
            AddHandler t.Tick, AddressOf Tick
        End Sub

        Sub Tick()
            Try
                lblTime.TextAlignment = TextAlignment.Right
                lblTime.Text = Application.Current.MainWindow.Resources.Item("O M E G A") & "    |   " & Now.ToLongDateString & "     " & Now.ToLongTimeString
            Catch ex As Exception
            End Try
        End Sub

        Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        End Sub
    End Class
End Namespace
