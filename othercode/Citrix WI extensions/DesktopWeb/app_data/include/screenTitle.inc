<%
// screenTitle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
if (welcomeControl.getShowTitle() || welcomeControl.getShowSecondTitle()) {
%>
    <div id="welcome">
        <%
        if (welcomeControl.getShowTitle()) {
        %>
            <h4 id="welcomeTitle" class="<%=welcomeControl.getTitleClass()%>">
                  <%=welcomeControl.getTitle()%>
            </h4>  <%// Keep the </h4> on a separate line to the content otherwise Opera interprets the CR before the </span> as a space and messes up the background image alignment %>
        <% } %>
        
        <%
        if (welcomeControl.getShowSecondTitle()) {
        %>
          <h5><%=welcomeControl.getSecondTitle()%></h5>
        <% } %>
    </div>
<%
} else {
%>
    <div id="welcome"><!-- --></div>
<% } %>