<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// login.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import Namespace="com.citrix.wi.clientdetect" %>
<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/loginClientScript.ascx";
        layout.PageView = "loginView.ascx";
    }
</script>

<% if(new com.citrix.wi.pages.auth.LoginASP(wiContext).perform()) { %>
<wi:Layout id="layout" runat="server" />
<% } %>
