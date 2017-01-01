<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// directlaunch.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
void Page_Load(object sender, System.EventArgs e) {
    layout.PageClientScript = "~/site/clientscripts/directLaunchClientScript.ascx";
    layout.PageView = "directLaunchView.ascx";
}
</script>

<%
if (!(new com.citrix.wi.pages.site.DirectLaunch(wiContext).perform())) return;
%>
<wi:Layout id="layout" runat="server" />