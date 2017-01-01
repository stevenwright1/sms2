<%
// compactSearchResults.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<h4 id="lgApplistHeading"><%=wiContext.getString("SearchResultsFor", WebUtilities.escapeHTML(searchControl.searchQuery))%></h4>
<%
if(searchControl.getNumResources()==0) {
%>
    <p id="noSearchResults"><%= wiContext.getString("NoSearchResults", WebUtilities.escapeHTML(searchControl.searchQuery)) %></p>
<%
} else {
%>
    <dl id="searchResultsView">
<%
for (int i = 0; i < searchControl.getNumResources(); i++) {
    ResourceControl resource = searchControl.resources[i];
%>
        <dt>
            <a  id="<%=resource.getEncodedAppId()%>"
                <%=resource.launchHref%>
                class="iconLink"
                title="<%=WebUtilities.escapeHTML(resource.getName(wiContext))%>"
              ><img id="spinner_<%=resource.getEncodedAppId()%>"
                    class="spinner"
                    width="11"
                    height="11"
                    src="<%=resource.getDelayedLaunchImgSrc(false)%>"
                    alt=""
               ><%=resource.iconImg%><span><%=WebUtilities.escapeHTML(resource.getName(wiContext))%></span></a>
        </dt>
        <dd>
            <a id="searchFolderLink_<%=i%>" class="iconLink" href="<%=resource.pathUrl%>"
                title="<%=wiContext.getString("SearchResultPath", WebUtilities.escapeHTML(resource.pathText))%>"
             ><img src="<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/FolderClosed16.png")%>" alt="<%=wiContext.getString("TipFolder")%>"><span><%=WebUtilities.escapeHTML(resource.pathText)%></span></a>
        </dd>
<%
   }
%>
    </dl>
<%
}
%>
<div class="searchNextPrevious">
<% if (searchControl.showPreviousLink) { %>
    <a id="previousLink"
       href="<%=Constants.PAGE_SEARCH_RESULTS%>?<%=Constants.QSTR_SEARCH_RESULTS_PAGE_NO%>=<%=(searchControl.currentPageNumber - 1)%>"
       title='<%=WebUtilities.escapeHTML(wiContext.getString("PreviousPage"))%>'
       ><%=WebUtilities.escapeHTML(wiContext.getString("LGPreviousPage"))%></a>
    &nbsp;
<% } %>
<% if (searchControl.showNextLink) { %>
    <a id="nextLink"
       href="<%=Constants.PAGE_SEARCH_RESULTS%>?<%=Constants.QSTR_SEARCH_RESULTS_PAGE_NO%>=<%=(searchControl.currentPageNumber + 1)%>"
       title='<%=WebUtilities.escapeHTML(wiContext.getString("NextPage"))%>'
       ><%=WebUtilities.escapeHTML(wiContext.getString("LGNextPage"))%></a>
<% } %>
</div>

<div id="searchPane">
  <form action='<%=Constants.PAGE_SEARCH_RESULTS%>' >
    <div class='lowGraphicsSearchArea'> <% // This div keeps HTML validation happy %>
     <input
	 type="text"
	 maxlength="60"
	 value="<%=WebUtilities.escapeHTML(searchControl.searchQuery)%>"
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
</div>
