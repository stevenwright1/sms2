<%
// retry.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if (!(new com.citrix.wi.pages.site.Retry(wiContext)).perform()) {
    return;
}
RetryPageViewControl viewControl = (RetryPageViewControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");

String pageTitle = WIBrowserTitleBuilder.createTitle(wiContext, "RetryFrameTitle");
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <title><%=pageTitle%></title>
    <!--#include file="~/app_data/include/cachedJavaScript.inc"-->
    <script type="text/javascript">
        <!--
        <!--#include file="clientscripts/delayedLaunch.js"-->

        <%=viewControl.retrySuccessfulTag %>
        <% if (viewControl.redirectUrl != null && viewControl.redirectMainWindow){ %>
                redirectToMainFrame("<%= viewControl.redirectUrl %>");
        <% } else if (viewControl.redirectUrl != null && !viewControl.redirectMainWindow) { %>
                window.location.replace("<%=viewControl.redirectUrl %>");
        <% } %>
        // -->
    </script>
</head>
<body>
</body>
</html>
