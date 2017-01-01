<%
// applistListView.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<ul id="listView">
<%
 for (int i = 0; i < viewControl.getNumFolders(); i++) {
      String folderPath = viewControl.folders[i].getDisplayPath() + viewControl.folders[i].getDisplayName();
%>
        <li>
            <a id="folderLink_<%=i%>" class="iconLink folderLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CURRENT_FOLDER%>=<%=WebUtilities.escapeURL(folderPath)%>"
                title="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>"
            ><img alt="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>" height="<%=viewControl.appIconSize%>" src="<%=viewControl.folderIconPath%>" width="<%=viewControl.appIconSize%>"><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(viewControl.folders[i].getDisplayName()))%></span></a>
        </li>
<%
  }
  for (int i = 0; i < viewControl.getNumResources(); i++) {
       ResourceControl resource = viewControl.resources[i];
       String displayName = resource.getName(wiContext);
%>
        <li>
            <a	id="<%=resource.getEncodedAppId()%>"
				<%=resource.launchHref%>
				class="iconLink"
				title="<%=WebUtilities.escapeHTML(displayName)%>"
              ><img id="spinner_<%=resource.getEncodedAppId()%>"
					class="spinner"
					width="11"
					height="11"
					src="<%=resource.getDelayedLaunchImgSrc(false)%>"
					alt="" 
				><%=resource.iconImg%><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(displayName))%></span></a>
        </li>
<%
 }
%>
<div class="spacer"></div>
</ul>