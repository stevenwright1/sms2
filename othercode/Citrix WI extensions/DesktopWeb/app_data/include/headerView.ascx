<%
// headerView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
HeaderControl headerControl = (HeaderControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_HEADER);
Header.populate(wiContext, headerControl);

SearchBoxControl searchBoxControl = (SearchBoxControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_SEARCH_BOX);
MessagesControl messagesControl = (MessagesControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_MESSAGES);

NavControl navControl = (NavControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_NAV);
LayoutControl layoutControl = (LayoutControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_LAYOUT);
%>

<!--#include file="header.inc"-->
