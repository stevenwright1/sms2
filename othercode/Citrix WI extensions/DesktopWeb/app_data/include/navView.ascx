<%
// navView.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
  NavControl navControl = (NavControl)Context.Items["navControl"];
  MessagesControl messagesControl = (MessagesControl)Context.Items["messagesControl"];
%>

<% if (Include.isCompactLayout(wiContext)) { %>
<!--#include file="compactNav.inc"-->
<% } else { %>
<!--#include file="nav.inc"-->
<% } %>
