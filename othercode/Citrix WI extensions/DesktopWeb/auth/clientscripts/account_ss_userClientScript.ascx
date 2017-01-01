<%
// account_ss_userClientScript.ascx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<script runat="server">
  LoginPageControl viewControl = null;
</script>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
viewControl = (LoginPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
AccountSelfService.populate(viewControl);
%>


<!--#include file="account_ss_user.js"-->
