<%
// desktopRestart.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
// Note, this page always sends a client-side redirect and has no UI of its own.
if (!(new com.citrix.wi.pages.site.RestartDesktop(wiContext)).perform()) {
    return;
}
%>
