<%
// account_ss_userView.ascx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
LoginPageControl viewControl = (LoginPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>
<div class="outerContainerDiv">
<!--#include file="loginMainForm.inc"-->
<!--#include file="loginMainFormFoot.inc"-->
</div>
<!--#include file="account_ss_buttons.inc"-->
