<%
// changeMode.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
(new com.citrix.wi.pages.site_auth.ChangeMode(wiContext)).perform();
return; // this page has no UI
%>
