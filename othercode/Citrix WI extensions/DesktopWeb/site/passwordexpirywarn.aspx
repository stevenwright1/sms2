<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// passwordexpirywarn.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%@ Import namespace="com.citrix.authenticators" %>

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/site/clientscripts/passwordexpirywarnClientScript.ascx";
        layout.PageView = "passwordExpiryWarnView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.site.PasswordExpiryWarn(wiContext).perform())) return;
%>

<wi:Layout id="layout" runat="server" />