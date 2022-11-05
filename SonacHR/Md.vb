Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Drawing

Module Md
    Public LastVersion As Integer = 1
    Public MyProjectType As ProjectType = ProjectType.Sonac

    'Install-Package NavigationPane

    Public AllowPreviousYearsForNonManager = True
    Public StopProfiler As Boolean = False
    Public ShowShifts As Boolean = False
    Public RptFromToday As Boolean = False
    Public Demo As Boolean = False


    Public con As New SqlConnection
    Public SqlConStrBuilder As New SqlClient.SqlConnectionStringBuilder
    Public FourceExit As Boolean = False
    Public HasLeft As Boolean = False

    Public UserName, ArName, LevelId, Password, CompanyName, CompanyTel, Nurse As String
    Public Manager, Receptionist As Boolean
    Public DefaultStore, DefaultSave, DefaultBank As Integer
    Public EnName As String = "Please, Login", Currentpage As String = ""


    Public dtLevelsMenuitems As DataTable
    Public CurrentDate As DateTime
    Public CurrentShiftId As Integer = 0
    Public CurrentShiftName As String = ""
    Public Cashier As String = "0"
    Public UdlName As String = "" '"Connect"
    Public IsLogedIn As Boolean = False

    Public BarcodePrinter As String = ""
    Public PonePrinter As String = ""

    Public DictionaryCurrent As New ResourceDictionary()
    Public MyDictionaries As New ListBox

    Enum ProjectType
        PCs
        Sonac
    End Enum

    
End Module
