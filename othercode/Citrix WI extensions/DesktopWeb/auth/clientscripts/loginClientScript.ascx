<%
// loginClientScript.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<script runat="server">
  LoginPageControl viewControl = null;
</script>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
viewControl = (LoginPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<!--#include file="~/app_data/clientscripts/cookies.js"-->

<!--#include file="login.js"-->
