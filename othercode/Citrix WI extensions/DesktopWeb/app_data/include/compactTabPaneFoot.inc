<%
// compactTabPaneFoot.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="spacer"></div>

<div id="searchPane">
<%
if ((viewControl.lowGraphicsParentFolderName != null) || viewControl.showLowGraphicsChangeView || viewControl.showRefresh) {
%>
  <ul>
<%
}

if (viewControl.showRefresh) {
%>
    <li>
      <a id="refreshLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_REFRESH%>=On"><%= wiContext.getString("Refresh") %></a>
    </li>
<%
}
if (viewControl.showLowGraphicsChangeView) {
%>
    <li>
      <a id="changeViewLink"
        href="<%=Constants.PAGE_VIEW_STYLE_SETTINGS%>"
        title="<%=wiContext.getString("TipChangeView")%>"
      ><%=wiContext.getString("ChangeView")%></a>
    </li>
<%
}
if ((viewControl.lowGraphicsParentFolderName != null) || viewControl.showLowGraphicsChangeView || viewControl.showRefresh) {
%>
    </ul>
<%
}
if (searchBoxControl.show) {
// For low graphics, keep the search area nice and simple
%>
     <form action='<%=Constants.PAGE_SEARCH_RESULTS%>' >
       <div class='lowGraphicsSearchArea'> <% // This div keeps HTML validation happy %>
        <input
            type="text"
            maxlength="60"
            value="<%=WebUtilities.escapeHTML(searchBoxControl.query)%>"
            name="<%=Constants.QSTR_SEARCH_STRING%>"
            id="lowGraphicsSearchEntry"
         >
        <input
            type="submit"
            value="<%=wiContext.getString("Search")%>"
            title="<%=wiContext.getString("SearchTooltip")%>"
            id="lowGraphicsSearchButton"
        >
      </div>
     </form>
<%
}
%>
</div>