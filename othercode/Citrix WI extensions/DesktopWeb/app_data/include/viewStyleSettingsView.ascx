<%
// viewStyleSettingsView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
ViewStyleSettingsPageControl viewControl = (ViewStyleSettingsPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<!--#include file="viewStyleSettings.inc"-->