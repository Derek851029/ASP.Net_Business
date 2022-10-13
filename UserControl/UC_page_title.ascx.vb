Imports System.Text.RegularExpressions

''' <summary>
''' 翻頁用戶控件
''' </summary>
''' <remarks></remarks>
''' <history>
''' 	[Gavin]	2008/4/1	Created
''' </history>
Partial Class UC_page_title
    Inherits System.Web.UI.UserControl

    Public CurrentPage As Int64 = 0
    Public NumberOfPages, AllPage, TotCount As Int64
    Public TotalRowCount As Int64 = 0
    Public _intPageSize As Integer = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings("PageSize"))
    Public Event pagerEvent(ByVal s As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' 引發事件
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Public Sub OnpagerEvent(ByVal e As EventArgs)
        RaiseEvent pagerEvent(Me, EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' 當前頁數
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Public Property Status() As Integer
        Get
            Return ViewState("CurrPage")
        End Get
        Set(ByVal Value As Integer)
            ViewState("CurrPage") = Value.ToString()
        End Set
    End Property

    ''' <summary>
    ''' 總頁數
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Public Property MaxPage() As Int64
        Set(ByVal Value As Int64)
            ViewState("AllPage") = Value
            AllPage = Value
            lbl_PageCount.Text = Value.ToString

            Reset_UControl()
        End Set
        Get
            Return Convert.ToInt64(ViewState("AllPage").ToString())
        End Get
    End Property

    ''' <summary>
    ''' 數據總筆數
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Public Property setCount() As Int64
        Get
            Return Convert.ToInt64(ViewState("TotCount").ToString())
        End Get
        Set(ByVal Value As Int64)
            ViewState("TotCount") = Value
            TotCount = Value
            lbl_RowMessage.Text = Value.ToString()
        End Set
    End Property

    ''' <summary>
    ''' GridView綁定後,調用方法
    ''' 給當前頁數,數據總筆數,總頁數賦值
    ''' </summary>
    ''' <param name="int_grd_Count"></param>
    ''' <param name="int_grd_PageIndex"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Public Sub get_AllacatePage(ByVal int_grd_Count As Int64, ByVal int_grd_PageIndex As Int64)
        Status = int_grd_PageIndex
        setCount = int_grd_Count

        If int_grd_Count = 0 Then
            MaxPage = 1
            Me.int_GoPage.Text = ""
            Me.int_GoPage.Enabled = False
        Else
            Me.int_GoPage.Enabled = True
            If (int_grd_Count Mod _intPageSize > 0) Then
                MaxPage = Convert.ToInt16(int_grd_Count \ _intPageSize) + 1
            Else
                MaxPage = Convert.ToInt16(int_grd_Count \ _intPageSize)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 用戶控件載入時,
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If (Not Me.IsPostBack) Then
            ViewState("AllPage") = AllPage
            ViewState("CurrPage") = CurrentPage
            lbl_PageCount.Text = ViewState("AllPage").ToString()
            lbl_RowMessage.Text = TotCount.ToString()
            _intPageSize = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings("PageSize"))
            ' ViewState("PageSize") = _intPageSize
            PageSize.Text = _intPageSize
            ViewState("TotCount") = TotCount.ToString()
        Else
            lbl_PageCount.Text = ViewState("AllPage").ToString()
            AllPage = Convert.ToInt64(ViewState("AllPage").ToString())
            CurrentPage = Convert.ToInt64(ViewState("CurrPage").ToString())
            lbl_RowMessage.Text = ViewState("TotCount").ToString()

            _intPageSize = PageSize.Text

            'PageSize.Text = _intPageSize
        End If


        Reset_UControl()


    End Sub

    ''' <summary>
    ''' linkButton--第一頁
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub lkb_First_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_First.Click

        CurrentPage = 0
        ViewState("CurrPage") = CurrentPage
        Reset_UControl()

        OnpagerEvent(EventArgs.Empty)

    End Sub

    ''' <summary>
    ''' linkButton--上一頁
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub lkb_Previous_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_Previous.Click

        CurrentPage = CurrentPage - 1
        ViewState("CurrPage") = CurrentPage
        Reset_UControl()

        OnpagerEvent(EventArgs.Empty)


    End Sub

    ''' <summary>
    ''' linkButton--下一頁
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub lkb_Next_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_Next.Click

        CurrentPage = CurrentPage + 1
        ViewState("CurrPage") = CurrentPage
        Reset_UControl()

        OnpagerEvent(EventArgs.Empty)

    End Sub

    ''' <summary>
    ''' linkButton--最末頁
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub lkb_Last_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_Last.Click

        CurrentPage = AllPage - 1
        ViewState("CurrPage") = CurrentPage
        Reset_UControl()

        OnpagerEvent(EventArgs.Empty)


    End Sub

    ''' <summary>
    ''' 根據指定頁數跳轉
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub lkb_GoPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_GoPage.Click
        If CheckNumber() Then

            CurrentPage = Convert.ToInt64(int_GoPage.Text.ToString.Trim) - 1
            If (CurrentPage = -1) Then
                CurrentPage = 0
            ElseIf (CurrentPage >= AllPage) Then
                CurrentPage = AllPage - 1
            End If
            ViewState("CurrPage") = CurrentPage
            Reset_UControl()

            OnpagerEvent(EventArgs.Empty)


        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, Guid.NewGuid.ToString, "<script type='text/javascript'>alert('您輸入的頁碼不正確!');</script>")
        End If
    End Sub

    ''' <summary>
    ''' 指定分頁筆數
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' </history>
    Private Sub lkb_ReloadPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lkb_ReloadPage.Click
        If PageSizeCheckNumber() Then

            Reset_UControl()

            OnpagerEvent(EventArgs.Empty)
        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, Guid.NewGuid.ToString, "<script type='text/javascript'>alert('您輸入的每頁顯示筆數不正確!');</script>")
        End If

    End Sub

    ''' <summary>
    ''' 驗證是否未數字
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Function CheckNumber() As Boolean
        Dim objRegEx As Regex = New Regex("^[0-9]*[1-9][0-9]*$")

        If objRegEx.IsMatch(int_GoPage.Text.ToString.Trim) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function PageSizeCheckNumber() As Boolean
        Dim objRegEx As Regex = New Regex("^[0-9]*[1-9][0-9]*$")

        If PageSize.Text = "" Then
            _intPageSize = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings("PageSize"))
            PageSize.Text = _intPageSize
            'ViewState("PageSize") = _intPageSize
        End If
        If objRegEx.IsMatch(PageSize.Text.ToString.Trim) Then
            _intPageSize = PageSize.Text
            CurrentPage = 0
            'ViewState("PageSize") = _intPageSize
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' 顯示當前頁數
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub Show_Pager_txtNum()
        lbl_Page.Text = (ViewState("CurrPage") + 1).ToString()
    End Sub

    ''' <summary>
    ''' 第一頁時,[第一頁]與[上一頁]不可用;
    ''' 最末頁時,[最末頁]與[下一頁]不可用.
    ''' 只有一頁時,全部不可用
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Gavin]	2008/4/1	Created
    ''' </history>
    Private Sub Reset_UControl()
        If ViewState("CurrPage") > Convert.ToInt64(ViewState("AllPage").ToString()) - 1 Then
            ViewState("CurrPage") = 0
        End If

        Show_Pager_txtNum()

        If ViewState("CurrPage") = 0 Then                                                               '第一頁時
            Me.lkb_First.Enabled = False
            Me.lkb_Previous.Enabled = False
            Me.lkb_Next.Enabled = True
            Me.lkb_Last.Enabled = True
        ElseIf ViewState("CurrPage") = Convert.ToInt64(ViewState("AllPage").ToString()) - 1 Then        '最末頁時
            Me.lkb_First.Enabled = True
            Me.lkb_Previous.Enabled = True
            Me.lkb_Next.Enabled = False
            Me.lkb_Last.Enabled = False
        Else
            Me.lkb_First.Enabled = True
            Me.lkb_Previous.Enabled = True
            Me.lkb_Next.Enabled = True
            Me.lkb_Last.Enabled = True
        End If

        If Convert.ToInt64(ViewState("AllPage").ToString()) = 1 Then                                    '只有一頁時
            Me.lkb_First.Enabled = False
            Me.lkb_Previous.Enabled = False
            Me.lkb_Next.Enabled = False
            Me.lkb_Last.Enabled = False
        End If
    End Sub
End Class
