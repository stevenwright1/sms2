<%
// applistClientScript.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
ApplistPageControl viewControl = (ApplistPageControl)Context.Items["viewControl"];
SearchBoxControl searchBoxControl = (SearchBoxControl)Context.Items["searchBoxControl"];
NavControl navControl = (NavControl)Context.Items["navControl"];
LayoutControl layoutControl = (LayoutControl)Context.Items["layoutControl"];
%>


<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<% if (!Include.isCompactLayout(wiContext)) { %>
<!--#include file="searchUtils.js"-->
<!--#include file="fullApplist.js"-->
<!--#include file="~/app_data/clientscripts/cookies.js"-->
<% } %>
