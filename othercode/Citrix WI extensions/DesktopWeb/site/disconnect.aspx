<%
// disconnect.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<%
(new com.citrix.wi.pages.site.Disconnect(wiContext)).perform();
%>
</head>
<body></body>
</html>
