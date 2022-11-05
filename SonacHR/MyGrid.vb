Imports System.Windows.Forms

Public Class MyGrid

    Dim MyAutoCompleteMode As Boolean
    Public Property AutoCompleteMode As Boolean
        Get
            RemoveHandler Me.EditingControlShowing, AddressOf Me_EditingControlShowing
            Return MyAutoCompleteMode
        End Get
        Set(value As Boolean)
            MyAutoCompleteMode = value
            AddHandler Me.EditingControlShowing, AddressOf Me_EditingControlShowing
        End Set
    End Property

    Private Sub MyGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellDoubleClick
        If CurrentCell.ReadOnly Then Return
        OnKeyDown(New KeyEventArgs(Keys.F1))
    End Sub

    Private Sub Grid_DataError(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles Me.DataError, MyBase.DataError, MyClass.DataError

    End Sub

    Public Sub New()
        InitializeComponent()
        AllowUserToDeleteRows = False
        'Dim DataGridViewCellStyle1 As New DataGridViewCellStyle
        'Dim DataGridViewCellStyle2 As New DataGridViewCellStyle
        'DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        'DataGridViewCellStyle1.Font = New System.Drawing.Font("Times New Roman", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        'AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        BackgroundColor = System.Drawing.Color.GhostWhite
        'DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        'DataGridViewCellStyle2.BackColor = System.Drawing.Color.Gold
        'DataGridViewCellStyle2.Font = New System.Drawing.Font("Times New Roman", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        'DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        'DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        'DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        'DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        'ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        Dock = System.Windows.Forms.DockStyle.Fill
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
        EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Location = New System.Drawing.Point(0, 0)
        MultiSelect = False
        Name = "Grid"
        'RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        'Size = New System.Drawing.Size(150, 150)
        'TabIndex = 0
        StandardTab = False
        'If Md.MyProject = Client.ClothesRed Then
        '    ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font(ColumnHeadersDefaultCellStyle.Font.FontFamily, 14)
        '    ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.White
        '    ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Red
        '    RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White
        '    RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Red
        'End If

    End Sub

    Public Property BarcodeIndex As Integer = -1

    Dim lop2 As Boolean = False
    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        If lop2 Then
            lop2 = False
            Return False
        End If
        Try
            If Not BarcodeIndex = CurrentCell.ColumnIndex AndAlso keyData = Keys.Enter Then
                Dim rowindex As Integer = CurrentCell.RowIndex
                Dim columnindex As Integer = CurrentCell.ColumnIndex
                firstcolumnenabled(columnindex, rowindex)
                CurrentCell = Rows(rowindex).Cells(columnindex)
                Return True
            ElseIf keyData = Keys.Enter Then
                OnKeyDown(New KeyEventArgs(Keys.Enter))
            ElseIf keyData = Keys.F11 Then
                If AllowUserToDeleteRows Then
                    Rows.Remove(CurrentRow)
                    Return True
                End If
            ElseIf keyData = Keys.F1 Then
                OnKeyDown(New KeyEventArgs(Keys.F1))
            ElseIf keyData = Keys.F12 Then
                OnKeyDown(New KeyEventArgs(Keys.F12))
            Else
                lop2 = True
                Return ProcessDialogKey(keyData)
            End If
        Catch ex As Exception
            lop2 = True
            Return ProcessDialogKey(keyData)
        End Try
    End Function

    Sub firstcolumnenabled(ByRef currcolumn As Integer, ByRef currindex As Integer)
        Try
            Dim newindex As Integer = currindex
            For i As Integer = currcolumn + 1 To Columns.Count - 1
                If Not BarcodeIndex = i AndAlso Not Columns(i).ReadOnly AndAlso Columns(i).Visible And Not Rows(currindex).Cells(i).ReadOnly AndAlso Rows(currindex).Cells(i).Visible Then
                    currcolumn = i
                    currindex = newindex
                    Return
                End If
            Next
            currcolumn = -1
            currindex += 1
            firstcolumnenabled(currcolumn, currindex)
        Catch ex As Exception
            'currindex -= 1
            'firstcolumnenabled(currcolumn, currindex)
        End Try
    End Sub

    Private Sub MyGrid_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Try
            If Not BarcodeIndex = CurrentCell.ColumnIndex AndAlso e.KeyCode = Keys.Enter Then
                Dim rowindex As Integer = CurrentCell.RowIndex
                Dim columnindex As Integer = CurrentCell.ColumnIndex
                firstcolumnenabled(columnindex, rowindex)
                CurrentCell = Rows(rowindex).Cells(columnindex)
                e.Handled = True
            ElseIf e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.Delete Then
                If AllowUserToDeleteRows Then
                    Rows.Remove(CurrentRow)
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public IgnoreRecordNo As Boolean = False
    Private Sub MyGrid_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles Me.RowsAdded
        If IgnoreRecordNo Then Return
        Try
            Rows(e.RowIndex).HeaderCell.Value = (e.RowIndex + 1).ToString
            If Rows.GetLastRow(DataGridViewElementStates.None) > 0 Then
                Rows(Rows.GetLastRow(DataGridViewElementStates.None)).HeaderCell.Value = (Rows.GetLastRow(DataGridViewElementStates.None) + 1).ToString
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Me_EditingControlShowing(ByVal sender As Object, ByVal e As Forms.DataGridViewEditingControlShowingEventArgs)
        Try
            If e.Control.GetType Is GetType(Forms.DataGridViewComboBoxEditingControl) Then
                Dim cb As Forms.ComboBox = TryCast(e.Control, Forms.ComboBox)
                If cb IsNot Nothing Then
                    cb.DropDownStyle = Forms.ComboBoxStyle.DropDown
                    cb.AutoCompleteMode = Forms.AutoCompleteMode.SuggestAppend
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

End Class
