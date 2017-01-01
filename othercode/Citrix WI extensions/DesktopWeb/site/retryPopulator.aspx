<%
// retryPopulator.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if (!(new com.citrix.wi.pages.site.RetryPopulator(wiContext)).perform()) {
    return;
}
RetryPopulatorControl viewControl = (RetryPopulatorControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");

String pageTitle = WIBrowserTitleBuilder.createTitle(wiContext, "RetryFrameTitle");
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <title><%=pageTitle%></title>
    <script type="text/javascript">
        <!--
        <% if (viewControl.redirectUrl != null) { %>
                <%= viewControl.redirectUrl %>
        <% } %>
        // -->
    </script>
</head>
<body>
    <%=viewControl.getRetryIframesHtml()%>
</body>
</html>
