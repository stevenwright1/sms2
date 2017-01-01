<%
// clientDetectionOutputs.aspx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%@ Page Language="C#" %>
<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
// The following always returns a redirect to another page so that the POST request data
// and token is not cached.
(new com.citrix.wi.pages.site_auth.clientdetection.Outputs(wiContext)).perform();
%>
