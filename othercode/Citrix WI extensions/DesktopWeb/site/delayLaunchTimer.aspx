<%
// retry.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if (!(new com.citrix.wi.pages.site.DelayLaunchTimer(wiContext)).perform()) {
    return;
}
DelayLaunchTimerPageViewControl viewControl = (DelayLaunchTimerPageViewControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
String pageTitle = WIBrowserTitleBuilder.createTitle(wiContext, "RetryFrameTitle");
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <title><%= pageTitle %></title>
    <script type="text/javascript">
        <!--
        <!--#include file="clientscripts/euem.js"-->
        <!--#include file="clientscripts/retry.js" -->
        // -->
    </script>
</head>
<body onload="retry(<%=viewControl.getRetryTime()*1000 %>);">
</body>
</html>
