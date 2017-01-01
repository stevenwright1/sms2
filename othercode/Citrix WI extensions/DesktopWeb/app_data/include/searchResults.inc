<%
// searchResults.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
%>
<%
if( searchControl.getNumResources() == 0) {
%>
<p class="applistMessageStyleInfo">
  <%= wiContext.getString("NoSearchResults", WebUtilities.escapeHTML(searchControl.searchQuery)) %>
</p>
<% } else {
%>
<p class="secondParagraph"><%=wiContext.getString("SearchResultsCount", WebUtilities.escapeHTML(searchControl.searchQuery), searchControl.nTotal)%></p>
<div id="tabularView">
  <table>
    <tr>
      <th>
        <%= wiContext.getString("AppName") %>
      </th>
      <th class="noLeftBorder">
        <%= wiContext.getString("SearchResultsPath") %>
      </th>
    </tr>
    <%
    for (int i = 0; i < searchControl.getNumResources(); i++) {
        ResourceControl resource = searchControl.resources[i];
        String displayName = resource.getName(wiContext);
    %>
    <tr class="row<%=i%2%>">
      <td class="leftColumn">
        <a  id="<%=resource.getEncodedAppId()%>"
            <%=resource.launchHref%>
            class="iconLink"
            title="<%=WebUtilities.escapeHTML(displayName)%>"
           ><img
                id="spinner_<%=resource.getEncodedAppId()%>"
                class="spinner"
                width="11"
                height="11"
                src="<%=resource.getDelayedLaunchImgSrc(false)%>" alt=""
            ><%=resource.iconImg%><span><%=WebUtilities.escapeHTML(displayName)%></span></a>
      </td>
      <td class="descriptionColumn">
        <a id="searchFolderLink_<%=i%>" href="<%=resource.pathUrl%>" onclick="setItemInCookie('<%=Constants.COOKIE_TREE_VIEW_CURRENT_FOLDER%>', null);" class="folderIconLink" title="<%=WebUtilities.escapeHTML(resource.pathText)%>"
           ><img alt="<%=WebUtilities.escapeHTML(resource.pathText)%>" height="16" width="16" src="../media/FolderClosed16.gif"><span><%=WebUtilities.escapeHTML(resource.pathText)%></span></a>
      </td>
    </tr>
    <%
    }
    %>
  </table>
</div>
    <% if (searchControl.showNextLink || searchControl.showPreviousLink) { %>
<div>
        <% if (searchControl.showNextLink) { %>
  <a id="nextLink" href="<%=Constants.PAGE_SEARCH_RESULTS%>?<%=Constants.QSTR_SEARCH_RESULTS_PAGE_NO%>=<%=(searchControl.currentPageNumber + 1)%>"
     title='<%=WebUtilities.escapeHTML(wiContext.getString("NextPage"))%>' ><%=WebUtilities.escapeHTML(wiContext.getString("NextPage"))%></a>
        <%
        } if (searchControl.showPreviousLink) { %>
  <a id="previousLink" href="<%=Constants.PAGE_SEARCH_RESULTS%>?<%=Constants.QSTR_SEARCH_RESULTS_PAGE_NO%>=<%=(searchControl.currentPageNumber - 1)%>"
     title='<%=WebUtilities.escapeHTML(wiContext.getString("PreviousPage"))%>' ><%=WebUtilities.escapeHTML(wiContext.getString("PreviousPage"))%></a>
        <% } %>
  <label id="currentResultsNo">
    <%=WebUtilities.escapeHTML(wiContext.getString("CurrentSearchResultsNo",searchControl.nStartCurrentSearchResults, searchControl.nEndCurrentSearchResults))%>
  </label>
</div>
  <% } %>
<% } %>
