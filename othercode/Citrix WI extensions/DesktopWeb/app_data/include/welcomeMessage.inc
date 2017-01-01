<%
// welcomeMessage.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
if (welcomeControl.getShowBody()) {
%>
    <div id="welcomeMessage">
        <p class="welcomeText"><%=welcomeControl.getBody()%></p>
    </div>
<%
} else {
%>
    <div id="welcomeMessage"><!-- --></div>
<%
}
%>
