<%
// loginView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Register TagPrefix="wi" TagName="Feedback" Src="feedbackView.ascx" %>

<!--#include file="../serverscripts/include.aspxf"-->

<%
LoginPageControl viewControl = (LoginPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<!--#include file="loginMainForm.inc"-->
<tr><td id="feedbackCell" colspan="2">
<% if (!Include.isCompactLayout(wiContext)) { %>
  <wi:Feedback runat="server" />
<% } %>
</td></tr>
<!--#include file="loginMainFormFoot.inc"-->
