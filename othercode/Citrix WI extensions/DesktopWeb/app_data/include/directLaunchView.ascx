<%
// directLaunchView.ascx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
DirectLaunchPageControl viewControl = (DirectLaunchPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");

if (viewControl.getShowDesktopUI() && !Include.isCompactLayout(wiContext)) {
%>
    <!--#include file="directLaunchDesktop.inc"-->
<% } else { %>
    <!--#include file="directLaunchApp.inc"-->
<% } %>
