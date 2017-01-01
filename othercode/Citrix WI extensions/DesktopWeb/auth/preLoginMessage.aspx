<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// preLoginMessage.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/preLoginMessageClientScript.ascx";
        layout.PageView = "preLoginMessageView.ascx";
    }
</script>

<%
  if(new com.citrix.wi.pages.auth.PreLoginMessage(wiContext).perform()) {
%>
<wi:Layout id="layout" runat="server" />
<%
  }
%>
