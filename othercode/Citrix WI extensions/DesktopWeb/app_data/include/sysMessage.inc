<%
// sysMessage.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="sysMessage">
<%
String sysMessageText = sysMessageControl.getMessage();
if (!Strings.isEmpty(sysMessageText)) {
%>
    <p><%=sysMessageText%></p>
<%
} else {
%>
     <!-- -->
 <%
 }
 %>
</div>