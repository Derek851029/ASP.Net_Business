
Partial Class TreeView_UC_tvMenu
    Inherits System.Web.UI.UserControl

    ''' TREE_id=節點代碼
    ''' TREE_NAME=節點顯示名稱
    ''' IMAGE_URL=節點圖形檔名
    ''' NAVIGATE_URL=節點巡覽檔名
    ''' PARENT_id=父節點名稱

    Public WriteOnly Property setTreeNode() As Data.DataTable
        Set(ByVal value As Data.DataTable)
            Me.ViewState("TREE_NODE") = value
        End Set
    End Property

    Dim strNavigatePath As String = "~/"

    Public Sub initMenu()
        If Me.ViewState("TREE_NODE") Is Nothing Then Exit Sub
        Dim dv As New Data.DataView
        dv.Table = Me.ViewState("TREE_NODE")
        Dim drowparent As Data.DataRow()

        drowparent = dv.Table.Select("Level_id=2")
        For Each dr As Data.DataRow In drowparent
            Menu.Items.Add(New MenuItem("&nbsp&nbsp" & dr("TREE_NAME").ToString & "&nbsp&nbsp", dr("TREE_ID").ToString))
            For Each dr1 As Data.DataRow In dv.Table.Select("Level_id=3 and PARENT_ID='" & dr("TREE_ID").ToString.Trim & "' ")
                Dim men1 As MenuItem = New MenuItem("&nbsp&nbsp" & dr1("TREE_NAME").ToString, dr1("TREE_ID").ToString, "", strNavigatePath & dr1("NAVIGATE_URL").ToString)
                Menu.FindItem(dr1("PARENT_ID").ToString.Trim).ChildItems.Add(men1)
            Next
        Next
    End Sub
End Class
