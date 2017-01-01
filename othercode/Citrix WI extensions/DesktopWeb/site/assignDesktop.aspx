<%
// assignDesktop.aspx
// Copyright (c) 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
(new com.citrix.wi.pages.site.AssignDesktop(wiContext)).perform();

AssignDesktopControl viewControl = (AssignDesktopControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

{
  redirectUrl: "<%= viewControl.redirectUrl %>",
  markup: "<%= viewControl.markup %>",
  autoLaunch: <%= viewControl.autoLaunch ? "true" : "false" %>,
  feedbackMessage: "<%= viewControl.feedbackMessage %>"
}
