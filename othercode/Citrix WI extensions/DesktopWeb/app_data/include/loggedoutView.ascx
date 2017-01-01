<%
// loggedoutView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Register TagPrefix="wi" TagName="Feedback" Src="feedbackView.ascx" %>

<%
LoggedoutPageControl viewControl = (LoggedoutPageControl)Context.Items["viewControl"];
%>

<!--#include file="../serverscripts/include.aspxf"-->

<% if (!Include.isCompactLayout(wiContext)) { %>
  <wi:Feedback runat="server" />
<% } %>

<!--#include file="loggedout.inc"-->
