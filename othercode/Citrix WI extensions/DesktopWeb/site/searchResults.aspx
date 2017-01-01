<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// searchResults.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/site/clientscripts/searchResultsClientScript.ascx";
        layout.PageView = "searchResultsView.ascx";
    }
</script>

<%
  new com.citrix.wi.pages.site.SearchResults(wiContext).perform();
%>
<wi:Layout id="layout" runat="server" />
