<%
// securid.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import namespace="com.citrix.authenticators" %>
<%@ Import namespace="com.citrix.authentication.tokens" %>


<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<% new com.citrix.wi.pages.auth.twofactor.SecurID(wiContext).perform(); %>
