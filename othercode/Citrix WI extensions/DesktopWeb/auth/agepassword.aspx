<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// agepassword.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/agepasswordClientScript.ascx";
        layout.PageView = "agepasswordView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.age.Password(wiContext)).perform()) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
