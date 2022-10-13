<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UC_tvMenu.ascx.vb" Inherits="TreeView_UC_tvMenu" %>
<style type="text/css">

    .menu_Main {
        height: 25px;
        font-size: 18px;
        text-decoration: none;
    }

    body {
        font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
        font-size: 14px;
    }

    .menu_StaticMenuItemStyle { /*MENU框*/
        line-height: 35px;
        height: 35px;
        color: #fff;
        font-size: 18px;
        border-radius: 6px;
        background-image: -webkit-linear-gradient(top, #337ab7 0%, #265a88 100%);
        background-image: -o-linear-gradient(top, #337ab7 0%, #265a88 100%);
        background-image: -webkit-gradient(linear, left top, left bottom, from(#337ab7), to(#265a88));
        background-image: linear-gradient(to bottom, #337ab7 0%, #265a88 100%);
        filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff337ab7', endColorstr='#ff265a88', GradientType=0);
        filter: progid:DXImageTransform.Microsoft.gradient(enabled = false);
        background-repeat: repeat-x;
        border-color: #245580;
        text-decoration: none;
        /*HorizontalPadding="5px" VerticalPadding="2px"*/
    }

    .menu_StaticHoverStyle { /*MENU框被選擇時*/
        color: #FFF;
        font-size: 18px;
        text-decoration: none;
    }

    .menu_DynamicMenuStyle {
        font-size: 20px;
        border-right: #B1B266 thin solid;
        border-top: #B1B266 thin solid;
        border-left: #B1B266 thin solid;
        border-bottom: #B1B266 thin solid;
        text-decoration: none;
    }

    .dropdown-menu {
        line-height: 30px;
        text-decoration: none;
    }

    .menu_DynamicHoverStyle {
        background-color: #e8e8e8;
        background-image: -webkit-linear-gradient(top, #f5f5f5 0%, #e8e8e8 100%);
        background-image: -o-linear-gradient(top, #f5f5f5 0%, #e8e8e8 100%);
        background-image: -webkit-gradient(linear, left top, left bottom, from(#f5f5f5), to(#e8e8e8));
        background-image: linear-gradient(to bottom, #f5f5f5 0%, #e8e8e8 100%);
        filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#fff5f5f5', endColorstr='#ffe8e8e8', GradientType=0);
        background-repeat: repeat-x;
        text-decoration: none;
    }
</style>
<asp:Menu ID="Menu" runat="server" Orientation="Horizontal" StaticEnableDefaultPopOutImage="False" Style="margin: 0px 15px">
    <StaticMenuItemStyle CssClass="menu_StaticMenuItemStyle" />
    <StaticHoverStyle CssClass="menu_StaticHoverStyle" />
    <StaticSelectedStyle CssClass="menu_StaticSelectedStyle" />
    <DynamicMenuStyle CssClass="dropdown-menu" />
    <DynamicSelectedStyle CssClass="menu_DynamicSelectedStyle" />
    <DynamicHoverStyle CssClass="menu_DynamicHoverStyle" />
</asp:Menu>
