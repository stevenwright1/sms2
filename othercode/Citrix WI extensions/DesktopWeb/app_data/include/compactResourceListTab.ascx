<%
// compactResourceListTab.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
ApplistPageControl viewControl = (ApplistPageControl)Context.Items["viewControl"];
%>

<%
if (viewControl.messageKey != null) {
// ********** Show a message rather than the applications **********
%>
<!-- #include file="applistEmptyView.inc" -->
<%
} else {
%>
<!-- #include file="compactResourceListTabTools.inc" -->
<%
if (viewControl.currentCompactViewStyle == CompactApplicationView.ICONS) {
%>
<!--#include file="compactApplistIconView.inc"-->
<%
} else if (viewControl.currentCompactViewStyle == CompactApplicationView.DESKTOPS) {
%>
<!--#include file="compactDesktopListView.inc"-->
<%
} else { // CompactApplicationView.LIST as default
%>
<!--#include file="applistListView.inc"-->
<%
}
%>
<%
}
%>