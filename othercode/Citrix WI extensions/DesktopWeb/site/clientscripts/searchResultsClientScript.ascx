<%
// applistClientScript.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
SearchBoxControl searchBoxControl = (SearchBoxControl)Context.Items["searchBoxControl"];
LayoutControl layoutControl = (LayoutControl)Context.Items["layoutControl"];
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<% if (!Include.isCompactLayout(wiContext)) { %>
<!--#include file="search.js"-->
<!--#include file="searchUtils.js"-->
<% } %>
