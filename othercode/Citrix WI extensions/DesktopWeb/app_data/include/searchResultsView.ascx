<%
// searchResultsView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
SearchResultsControl searchControl = (SearchResultsControl)Context.Items["searchControl"];
%>

<% if (Include.isCompactLayout(wiContext)) { %>
<!--#include file="compactSearchResults.inc"-->
<% } else { %>
<!--#include file="searchResults.inc"-->
<% } %>
