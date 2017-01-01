<%
// compactTabPane.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%@ Import Namespace="com.citrix.wi.pages.site" %>
<%@ Register TagPrefix="wi" TagName="compactResourceListTab" Src="compactResourceListTab.ascx" %>

<!--#include file="../serverscripts/include.aspxf"-->

<%
ApplistPageControl viewControl = (ApplistPageControl)Context.Items["viewControl"];
NavControl navControl = (NavControl)Context.Items["navControl"];
// The search box is part of the applist HTML for low graphics mode
SearchBoxControl searchBoxControl = (SearchBoxControl)Context.Items["searchBoxControl"];
%>

<wi:compactResourceListTab runat="server" />

<!--#include file="compactTabPaneFoot.inc"-->
