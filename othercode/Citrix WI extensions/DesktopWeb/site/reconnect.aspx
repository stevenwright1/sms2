<%
// reconnect.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%@ Import Namespace="com.citrix.wi.pages.site" %>
<%@ Import Namespace="com.citrix.wi.controls" %>
<%@ Import Namespace="com.citrix.wi.pageutils" %>
<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
String pageTitle = WIBrowserTitleBuilder.createTitle(wiContext, "ReconnectFrameTitle");
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN" "http://www.w3.org/TR/html4/frameset.dtd">
<html>
<head>
<title><%=pageTitle%></title>
<!--#include file="~/app_data/include/cachedJavaScript.inc"-->
<script type="text/javascript">
  <!--
  <!--#include file="clientscripts/euem.js"-->
  <!--#include file="clientscripts/launch.js"-->
  // -->
</script>
<%
new Reconnect(wiContext).perform();
ReconnectViewControl viewControl = (ReconnectViewControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");

if (viewControl.getRedirectUrl() == null) {
%>
<%= viewControl.getBufferedJavascript() %>
<%} else { %>
<script type="text/javascript">
<!--
// An error occurred so redirect the main frame to display an appropriate message
redirectToMainFrame('<%=viewControl.getRedirectUrl()%>');
// -->
</script>
<%} %>
</head>
<% if (viewControl.getRedirectUrl() == null) { %>
<%= viewControl.getBufferedFrameset() %>
<% } %>
</html>
