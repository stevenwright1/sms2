<%
// account_ss_entryView.ascx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
AccountSSEntryPageControl viewControl = (AccountSSEntryPageControl)Context.Items["viewControl"];
%>

<%
string VAL_ACCOUNT_UNLOCK = AccountTask.ACCOUNT_UNLOCK.ToString();
string VAL_PASSWORD_RESET = AccountTask.PASSWORD_RESET.ToString();
%>

<!--#include file="account_ss_entry.inc"-->