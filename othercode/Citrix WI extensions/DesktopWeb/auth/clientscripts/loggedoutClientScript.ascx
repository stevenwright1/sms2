<%
// loggedoutClientScript.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
  LoggedoutPageControl viewControl = null;
</script>
<%
viewControl = (LoggedoutPageControl)Context.Items["viewControl"];
%>



<!--#include file="loggedout.js"-->
