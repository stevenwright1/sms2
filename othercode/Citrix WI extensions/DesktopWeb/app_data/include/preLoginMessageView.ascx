<%
// preLoginMessageView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
PreLoginMessagePageControl viewControl = (PreLoginMessagePageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<!--#include file="preLoginMessage.inc"-->