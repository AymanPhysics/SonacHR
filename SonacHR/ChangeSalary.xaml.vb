Imports System.Data

Public Class ChangeSalary
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        'If Val(Perc.Text) = 0 Then
        '    bm.ShowMSG("برجاء تحديد النسبة")
        '    Return
        'End If
        If bm.ShowDeleteMSG("هل أنت متأكد من تعديل المرتب؟") Then
            If bm.ExecuteNonQuery("ChangeSalary", {"DayDate", "Perc", "PercFixed", "PercChanged", "SalaryMin", "SalaryMax", "SalaryFixedMin", "SalaryFixedMax", "SalaryChangedMin", "SalaryChangedMax", "SalaryAdd", "FixedAdd", "ChangedAdd"}, {bm.ToStrDate(DayDate.SelectedDate), Val(Perc.Text), Val(PercFixed.Text), Val(PercChanged.Text), Val(SalaryMin.Text), Val(SalaryMax.Text), Val(SalaryFixedMin.Text), Val(SalaryFixedMax.Text), Val(SalaryChangedMin.Text), Val(SalaryChangedMax.Text), Val(SalaryAdd.Text), Val(FixedAdd.Text), Val(ChangedAdd.Text)}) Then
                bm.ShowMSG("تمت العملية بنجاح")
            End If
        End If
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()

        Dim MyNow As DateTime = bm.MyGetDate()
        DayDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)


    End Sub

    Private Sub LoadResource()
        
    End Sub

    Private Sub Button2_Copy_Click(sender As Object, e As RoutedEventArgs) Handles Button2_Copy.Click
        Dim rpt As New ReportViewer
        rpt.paraname = New String() {"@DayDate", "@Perc", "@PercFixed", "@PercChanged", "@SalaryMin", "@SalaryMax", "@SalaryFixedMin", "@SalaryFixedMax", "@SalaryChangedMin", "@SalaryChangedMax", "@SalaryAdd", "@FixedAdd", "@ChangedAdd", "Header"}
        rpt.paravalue = New String() {bm.ToStrDate(DayDate.SelectedDate), Val(Perc.Text), Val(PercFixed.Text), Val(PercChanged.Text), Val(SalaryMin.Text), Val(SalaryMax.Text), Val(SalaryFixedMin.Text), Val(SalaryFixedMax.Text), Val(SalaryChangedMin.Text), Val(SalaryChangedMax.Text), Val(SalaryAdd.Text), Val(FixedAdd.Text), Val(ChangedAdd.Text), CType(Parent, Page).Title}
        rpt.Rpt = "ChangeSalaryTemp.rpt"
        rpt.Show()
    End Sub
End Class