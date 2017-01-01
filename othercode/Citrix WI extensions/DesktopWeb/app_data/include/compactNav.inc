<%
// compactNav.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
    TabSet tabSet = navControl.getTabs();
    if (tabSet != null) {
        Tab[] tabs = tabSet.getTabs();
        if (tabSet.getNumTabs() > 1 ) {
%>
    <div id="tabHeader">
        <select onChange='lgChangeTab(this);'>
        <% for (int i = 0; i < tabSet.getNumTabs(); i++) {
            Tab tab = tabs[i];
            String value = tab.targetPage() + "?" + Constants.QSTR_CURRENT_TAB + "=" + tab.getId();
        %>
            <option value="<%=value%>" <%=Markup.selectedStr(tabSet.isSelected(tab))%> >
                <%=tab.getTitle()%>
            </option>
        <% } %>
        </select>
    </div>
<%
        }
    }
%>
