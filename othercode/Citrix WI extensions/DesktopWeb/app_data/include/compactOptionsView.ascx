<%
// compactOptionsView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
NavControl navControl = (NavControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_NAV);
MessagesControl messagesControl = (MessagesControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_MESSAGES);
%>

<!--#include file="compactOptions.inc"-->
