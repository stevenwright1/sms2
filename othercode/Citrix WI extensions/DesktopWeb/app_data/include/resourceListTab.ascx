<%
// resourceListTab.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
ApplistPageControl viewControl = (ApplistPageControl)Context.Items["viewControl"];
%>

<!--#include file="resourceListTabTools.inc"-->

<%
if (viewControl.messageKey != null) {
// ********** Show a message rather than the applications **********
%>
<!-- #include file="applistEmptyView.inc" -->
<%
} else if (viewControl.currentViewStyle == ApplicationView.DESKTOPS) {
%>
<!--#include file="desktopListView.inc"-->
<%
} else if (viewControl.currentViewStyle == ApplicationView.LIST) {
%>
<!--#include file="applistListView.inc"-->
<%
} else if (viewControl.currentViewStyle == ApplicationView.DETAILS) {
%>
<!--#include file="applistDetailView.inc"-->
<%
} else if (viewControl.currentViewStyle == ApplicationView.TREE) {
%>
<!--#include file="applistTreeView.inc"-->
<%
} else if (viewControl.currentViewStyle == ApplicationView.GROUPS) {
%>
<!--#include file="appListGroupsView.inc"-->
<%
} else { // ApplicationView.ICON as default
%>
<!--#include file="appListIconView.inc"-->
<%
}
%>

