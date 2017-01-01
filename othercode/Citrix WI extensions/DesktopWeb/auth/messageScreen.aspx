<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// messageScreen.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/messagesClientScript.ascx";
        layout.PageView = "messageScreenView.ascx";
    }
</script>

<%
  new com.citrix.wi.pages.auth.PreLoginMessageScreen(wiContext).perform();
%>
<wi:Layout id="layout" runat="server" />
