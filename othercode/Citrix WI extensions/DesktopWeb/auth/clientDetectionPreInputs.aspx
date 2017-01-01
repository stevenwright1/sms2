<%
// clientDetectionPreInputs.aspx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if ((new com.citrix.wi.pages.site_auth.clientdetection.PreInputs(wiContext)).perform())
{
    VariablesForPostPageControl viewControl = (VariablesForPostPageControl)Context.Items["viewControl"];
%>
<!--#include file="~/app_data/include/clientDetectionVariablesForPost.inc"-->
<% } %>
