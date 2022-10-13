<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UC_page_title.ascx.vb" Inherits="UC_page_title" %>
<table cellspacing="1" cellpadding="1" width="95%" align="center" border="0">
	<tr>
		<td align="left" class="PageStyle" style="white-space:nowrap="nowrap";height:8%">
		    第<asp:label id="lbl_Page" Height="16" Text="" CssClass="PageContent" runat="Server"></asp:label>頁 
			/ 共<asp:label id="lbl_PageCount" Height="16" Text="" CssClass="PageContent" runat="Server"></asp:label>頁&nbsp; 
			共<asp:label id="lbl_RowMessage" Height="16" Text="" CssClass="PageContent" runat="Server"></asp:label>筆
		</td>
		<td align="right" style="height:8%" class="PageContent">
			<asp:linkbutton id="lkb_First" accessKey="Q" TabIndex="-1" Height="16" Text="第一頁" CssClass="PageContent"
				runat="Server" CommandName="First">第一頁</asp:linkbutton>
			<span class="PageContent"></span>
			<asp:linkbutton id="lkb_Previous" accessKey="Q" TabIndex="-1" Height="16" Text="上一頁" CssClass="PageContent"
				runat="Server" CommandName="Previous">上一頁</asp:linkbutton>
			<span class="PageContent"></span>
			<asp:linkbutton id="lkb_Next" accessKey="Q" TabIndex="-1" Height="16" Text="下一頁" CssClass="PageContent"
				runat="Server" CommandName="Next">下一頁</asp:linkbutton>
			<span class="PageContent"></span>
			<asp:linkbutton id="lkb_Last" accessKey="Q" TabIndex="-1" Height="16" Text="最末頁" CssClass="PageContent"
				runat="Server" CommandName="Last">最末頁</asp:linkbutton>
			<span class="PageContent">至 </span>
			<asp:textbox id="int_GoPage" tabIndex="-1" CssClass="inputNKeyin" runat="Server"
				MaxLength="4" Width="40px"></asp:textbox><span class="PageContent">頁</span>
			<asp:linkbutton id="lkb_GoPage" accessKey="Q" Height="16" Text="Go" CssClass="PageContent" runat="Server"
				CommandName="GoPage" tabIndex="-1">Go</asp:linkbutton>
			<span class="PageContent">每頁顯示筆數 </span>
			<asp:textbox id="PageSize" tabIndex="-1" CssClass="inputNKeyin" runat="Server" Width ="40px"></asp:textbox>
			<asp:linkbutton id="lkb_ReloadPage" accessKey="Q" Height="16" Text="Go" CssClass="PageContent" runat="Server"
				CommandName="ReloadPage" tabIndex="-1">設定</asp:linkbutton>			
		</td>
	</tr>
</table>