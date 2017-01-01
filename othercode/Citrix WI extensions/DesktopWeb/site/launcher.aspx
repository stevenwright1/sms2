<%
// launcher.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if (!(new com.citrix.wi.pages.site.Launcher(wiContext)).perform()) {
    return;
}
LauncherControl viewControl = (LauncherControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <title><%= viewControl.decodedTitle %></title>
    <link rel="SHORTCUT ICON" href="<%= viewControl.faviconLink %>" type="image/png">
    <!--#include file="~/app_data/include/cachedJavaScript.inc"-->
    <script type="text/javascript">
        <!--
        <!--#include file="clientscripts/launch.js"-->
        <!--#include file="clientscripts/launcher.js"-->
        // -->
    </script>
</head>
<%
//setTimeout(..) tricks Firefox, Safari and Netscape to download the favicon before this page redirects to a different page.
%>
<body onload="setTimeout('checkFrameName();',0);">
</body>
</html>
