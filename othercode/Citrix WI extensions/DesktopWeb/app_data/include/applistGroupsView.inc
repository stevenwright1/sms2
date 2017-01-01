<%
// applistGroupsView.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="groupViewDiv">
<%
 for (int g = 0; g < viewControl.getNumResourceGroups(); g++) {
 %>
 <div class="groupHeader">
    <div class="groupHeaderRule"></div>
    <div class="groupHeaderText">
        <%=viewControl.resourceGroups[g].title%> (<%=viewControl.resourceGroups[g].getNumResources()%>)
    </div>
</div>
<%
 for (int r = 0; r < viewControl.resourceGroups[g].getNumResources(); r++) {
      ResourceControl resource = viewControl.resourceGroups[g].contents[r];
      String displayName = resource.getName(wiContext);
%>
        <div class="groupView">
            <a	id="<%=resource.getEncodedAppId()%>"
				<%=resource.launchHref%>
				class="iconLink"
				title="<%=WebUtilities.escapeHTML(displayName)%>"
                ><%=resource.iconImg%><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(displayName))%> </span><img
																						id="spinner_<%=resource.getEncodedAppId()%>"
																						class="spinner"
																						width="11"
																						height="11"
																						src="<%=resource.getDelayedLaunchImgSrc(false)%>" alt=""
			></a>
        </div>
<%
} // end resource for
%>
<%
}   // end resource group for
%>
<% if ( viewControl.getNumFolders() > 0 ){ %>
<div class="groupHeader">
    <div class="groupHeaderRule"></div>
    <div class="groupHeaderText">
        <%=wiContext.getString("Folders")%> (<%=viewControl.getNumFolders()%>)
    </div>
</div>
<%
 for (int i = 0; i < viewControl.getNumFolders(); i++) {
      String folderPath = viewControl.folders[i].getDisplayPath() + viewControl.folders[i].getDisplayName();
%>
        <div class="groupView">
            <a id="folderLink_<%=i%>" class="iconLink folderLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CURRENT_FOLDER%>=<%=WebUtilities.escapeURL(folderPath)%>"
                title="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>"
                ><img alt="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>" height="<%=viewControl.appIconSize%>" src="<%=viewControl.folderIconPath%>" width="<%=viewControl.appIconSize%>"><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(viewControl.folders[i].getDisplayName()))%></span></a>
        </div>
<%
}   // end for
%>
<%
}   // end if folders
%>
<%-- A spacer div here avoids a problem with Safari 3.1 not clearing the mouse-over highlight --%>
<div class="spacer"></div>
</div>