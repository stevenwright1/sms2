<%
// tabPane.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%@ Import Namespace="com.citrix.wi.pages.site" %>
<%@ Register TagPrefix="wi" TagName="resourceListTab" Src="resourceListTab.ascx" %>

<!--#include file="../serverscripts/include.aspxf"-->

<%
ApplistPageControl viewControl = (ApplistPageControl)Context.Items["viewControl"];
NavControl navControl = (NavControl)Context.Items["navControl"];
%>

<!--#include file="tabPaneHead.inc"-->

<wi:resourceListTab runat="server" />

<!--#include file="tabPaneFoot.inc"-->
