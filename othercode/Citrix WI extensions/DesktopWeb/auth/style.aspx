<%
// style.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
(new com.citrix.wi.pages.site_auth.Style(wiContext)).perform();
StyleControl viewControl = (StyleControl)Context.Items["viewControl"];
if (viewControl.showLowGrahicsStyle()) {
// Embedded layout is just low graphics with a few tweaks
%>
  <!--#include file="~/app_data/include/lowStyle.inc"-->
<%
  if (viewControl.showEmbeddedStyle()) {
%>
  <!--#include file="~/app_data/include/ageEmbedStyle.inc"-->
<%
  }
} if (viewControl.showFullStyle()) {
%>
  <!--#include file="~/app_data/include/style.inc"-->
<%
}
%>
