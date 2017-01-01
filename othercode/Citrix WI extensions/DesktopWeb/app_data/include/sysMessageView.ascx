<%
// sysMessageView.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
SysMessageControl sysMessageControl = (SysMessageControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_SYSMESSAGE);
%>

<!--#include file="sysMessage.inc"-->
