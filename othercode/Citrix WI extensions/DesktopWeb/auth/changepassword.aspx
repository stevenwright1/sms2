<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// changepassword.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">

    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/app_data/clientscripts/changepasswordClientScript.ascx";
        layout.PageView = "changepasswordView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.ChangePassword(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />