<%
// nav.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
if (navControl.getShowNavBar()) {
%>
    <div class="spacer"></div>

    <div id="navBarWrapper">
        <div id="navBar">
            <ul>
                <%
                TabSet tabSet = navControl.getTabs();
                Tab[] tabs = tabSet.getTabs();
                for (int i = 0; i < tabSet.getNumTabs(); i++) {
                    Tab tab = tabs[i];
                %>
                    <%=TabUtils.generateTabMarkup(wiContext, tabSet, tab)%>
                <% } %>
            </ul>
        <div class="spacer"></div>
      </div>
    </div>
<% } %>
