Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO

Public Class CallCenter
    Dim bm As New BasicMethods
    Dim dt As New DataTable

    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        'If CategoryId.SelectedIndex < 1 Then
        '    bm.ShowMSG("Please, Choose a Category ..")
        '    CategoryId.Focus()
        '    Return
        'End If
        EndTime.Content = bm.MyGetTime
        CallerId.Text = CallerId.Text.Trim.Replace("'", "''")
        CallerName.Text = CallerName.Text.Trim.Replace("'", "''")
        Address.Text = Address.Text.Trim.Replace("'", "''")
        Notes.Text = Notes.Text.Trim.Replace("'", "''")
        bm.ExecuteNonQuery("insert CallCenter(EmpId,DayDate,StartTime,EndTime,CategoryId,CallerId,CallerName,Address,Notes,UserName,MyGetDate) select '" & EmpId.Text & "','" & bm.ToStrDate(DayDate.SelectedDate) & "','" & StartTime.Content & "','" & EndTime.Content & "','" & Val(CategoryId.SelectedValue) & "','" & CallerId.Text & "','" & CallerName.Text & "','" & Address.Text & "','" & Notes.Text & "','" & Md.UserName & "',GetDate()")
        bm.ShowMSG("Done Successfuly")
        If Not Parent Is Nothing Then
            Try
                CType(Parent, Window).Close()
            Catch ex As Exception
            End Try
        End If

    End Sub

    Public MyCallerId As String = ""
    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        bm.Addcontrol_MouseDoubleClick({EmpId})
        DayDate.SelectedDate = bm.MyGetDate()
        bm.FillCombo("CallCenterCategories", CategoryId, "")

        EmpId.Text = Md.UserName
        EmpId_LostFocus(Nothing, Nothing)

        DayDate.IsEnabled = False
        EmpId.IsEnabled = False
        StartTime.Content = bm.MyGetTime

        CallerId.Text = MyCallerId
        CallerId_LostFocus(Nothing, Nothing)
    End Sub
    Private Sub LoadResource()
        Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Save")
        lblEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        lblDayDate.SetResourceReference(Label.ContentProperty, "DayDate")
        lblStartTime.SetResourceReference(Label.ContentProperty, "Start Time")
        lblEndTime.SetResourceReference(Label.ContentProperty, "End Time")
        lblCategoryId.SetResourceReference(Label.ContentProperty, "Category")
        lblCallerId.SetResourceReference(Label.ContentProperty, "Caller Tel")
        lblCallerName.SetResourceReference(Label.ContentProperty, "Caller Name")
        lblAddress.SetResourceReference(Label.ContentProperty, "Address")
        lblNotes.SetResourceReference(Label.ContentProperty, "Notes")
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        If bm.ShowHelp("Employees", EmpId, EmpName, e, "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees") Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If
        bm.LostFocus(EmpId, EmpName, "select Name from Employees where Id=" & EmpId.Text.Trim())
    End Sub

    Private Sub CallerId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CallerId.LostFocus
        dt = bm.ExecuteAdapter("select top 1 * from Contacts where '" & CallerId.Text.Trim & "' in(CallerId,CallerId2,CallerId3)")
        If dt.Rows.Count > 0 Then
            CallerName.Text = dt.Rows(0)("CallerName")
            Address.Text = dt.Rows(0)("Address1")
            Notes.Focus()
        End If
    End Sub
End Class