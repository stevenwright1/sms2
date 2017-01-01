<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// account_ss_user.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/account_ss_userClientScript.ascx";
        layout.PageView = "account_ss_userView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.AccountSSUser(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />