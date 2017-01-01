<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// default.aspx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
void Page_Load(object sender, System.EventArgs e) {
    layout.PageClientScript = "~/site/clientscripts/applistClientScript.ascx";
    layout.PageView = "applistView.ascx";
}
</script>

<%
if (!(new com.citrix.wi.pages.site.AppList(wiContext).perform())) return;
%>
<wi:Layout id="layout" runat="server" />
