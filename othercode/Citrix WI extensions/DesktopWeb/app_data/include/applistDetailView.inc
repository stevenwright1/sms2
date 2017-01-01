<%
// applistDetailView.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="tabularView">
  <table>
    <tr>
      <th>
        <%= wiContext.getString("AppName") %>
      </th>
      <th class="noLeftBorder">
        <%= wiContext.getString("AppDescription") %>
      </th>
    </tr>
    <%
    for (int i = 0; i < viewControl.getNumFolders(); i++) {
		String escapedFolderDisplayName = WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName());
        String folderPath = viewControl.folders[i].getDisplayPath() + viewControl.folders[i].getDisplayName();
    %>
    <tr class="row<%=i%2%>">
      <td class="leftColumn">
        <a class="iconLink folderLink" id="folderLink_<%=i%>" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CURRENT_FOLDER%>=<%=WebUtilities.escapeURL(folderPath)%>"
           title="<%=escapedFolderDisplayName%>"
        ><img alt="<%=escapedFolderDisplayName%>" height="<%=viewControl.appIconSize%>" src="<%=viewControl.folderIconPath%>" width="<%=viewControl.appIconSize%>"><span><%=escapedFolderDisplayName%></span></a>
      </td>
      <td class="descriptionColumn">
      <% //Adding &nbsp; to make IE7 happy as it wont draw borders for empty cells %>
      <%=Strings.isEmpty(viewControl.folders[i].getDescription()) ? "&nbsp;" : WebUtilities.escapeHTML(viewControl.folders[i].getDescription())%>
      </td>
    </tr>
    <%
    }
    for (int i = 0; i < viewControl.getNumResources(); i++) {
        ResourceControl resource = viewControl.resources[i];
        String escapedDisplayName = WebUtilities.escapeHTML(resource.getName(wiContext));
    %>
    <tr class="row<%=(viewControl.getNumFolders() + i)%2%>">
      <td class="leftColumn">
        <a	
			id="<%=resource.getEncodedAppId()%>"
			<%=resource.launchHref%>
			class="iconLink"
			title="<%=escapedDisplayName%>"
        ><img
			id="spinner_<%=resource.getEncodedAppId()%>"
			class="spinner"
			width="11"
			height="11"
			src="<%=resource.getDelayedLaunchImgSrc(false)%>"
			alt=""
		><%=resource.iconImg%><span><%=escapedDisplayName%></span></a>
      </td>
      <% if (resource.startInProgress) { %>
           <script type="text/javascript">
             delayedLaunchDesktops.push('<%=resource.getEncodedAppId()%>');
           </script>
      <% } %>
      <td class="descriptionColumn">
      <% //Adding &nbsp; to make IE7 happy as it wont draw borders for empty cells %>
      <%=Strings.isEmpty(resource.publishedDescription) ? "&nbsp;" : WebUtilities.escapeHTML(resource.getDescription(wiContext))%>
      </td>
    </tr>
    <%
      }
    %>
  </table>
</div>
