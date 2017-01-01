<%
// resourceListTabTools.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="displayTools"> <% // Breadcrumb trail, change view and refresh pane %>
<% if(viewControl.currentViewStyle!=ApplicationView.TREE || viewControl.showRefresh || viewControl.allowedViewStyles.size() > 1) { %>
<table id="displayToolsTable"><tr>
  <td id="displayToolsMainCell">
<% if(viewControl.currentViewStyle!=ApplicationView.TREE && viewControl.breadCrumbTrail!=null) { %>
    <%=viewControl.breadCrumbTrail%>
<% } %>
  </td>
  <td id="displayToolsViewCell">
<% if (viewControl.showChangeView && viewControl.allowedViewStyles.size() > 1) { %>
    <div class="selectButton customButton">
	  <a id="viewButton" class="leftDoor CustomMenu" href="#"><span class="rightDoor"><%=wiContext.getString("SelectView")%><img src="../media/DownArrow.png" alt=""></span></a>
    </div>
    <div id="Popup_viewButton" class="wiPopup dropDownMenu">
      <% if (viewControl.allowedViewStyles.contains(ApplicationView.ICONS)) { %>
          <a href="javascript:changeView('Icons')"><%= wiContext.getString("Icons") %></a>
      <% }
         if (viewControl.allowedViewStyles.contains(ApplicationView.DETAILS)) { %>
          <a href="javascript:changeView('Details')"><%= wiContext.getString("Details") %></a>
      <% }
         if (viewControl.allowedViewStyles.contains(ApplicationView.LIST)) { %>
          <a href="javascript:changeView('List')"><%= wiContext.getString("List") %></a>
      <% }
         if (viewControl.allowedViewStyles.contains(ApplicationView.TREE)) { %>
          <a href="javascript:changeView('Tree')"><%= wiContext.getString("Tree") %></a>
      <% }
         if (viewControl.allowedViewStyles.contains(ApplicationView.GROUPS)) { %>
          <a href="javascript:changeView('Groups')"><%= wiContext.getString("Groups") %></a>
      <% } %>
    </div>
<% } else { %>
<%-- Hidden button to keep the breadcrumb area the same height whether or not the "Select view" and "Refresh" buttons are shown --%>
    <div class="customButton hiddenElement">
	  <a id="hiddenButton" class="leftDoor" href="#"><span class="rightDoor"></span></a>
    </div>
<% } %>
  </td>
<% if (viewControl.showRefresh) { %>
  <td id="displayToolsRefreshCell">
    <a id="refreshLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_REFRESH%>=On" title="<%=wiContext.getString("TipRefreshButton")%>">&nbsp;</a>
  </td>
<% } %>
</tr></table>
<% } %>
</div>
