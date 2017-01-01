<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// preferences.aspx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
void Page_Load(object sender, System.EventArgs e) {
  layout.PageClientScript = "~/site/clientscripts/preferencesClientScript.ascx";
  layout.PageView = "preferencesView.ascx";
}
</script>

<%
if (!(new com.citrix.wi.pages.site.Preferences(wiContext)).perform()) return;
%>

<wi:Layout id="layout" runat="server" />