<%
// applistIconView.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="iconViewDiv">
<%
 for (int i = 0; i < viewControl.getNumFolders(); i++) {
      String folderPath = viewControl.folders[i].getDisplayPath() + viewControl.folders[i].getDisplayName();
%>
        <div class="iconView">
            <a id="folderLink_<%=i%>" class="iconLink folderLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CURRENT_FOLDER%>=<%=WebUtilities.escapeURL(folderPath)%>"
                title="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>"
                ><img alt="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>" height="<%=viewControl.appIconSize%>" src="<%=viewControl.folderIconPath%>" width="<%=viewControl.appIconSize%>"><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(viewControl.folders[i].getDisplayName()))%> </span></a>
        </div>
<%
}
%>
<%
 for (int i = 0; i < viewControl.getNumResources(); i++) {
      ResourceControl resource = viewControl.resources[i];
      String displayName = resource.getName(wiContext);
%>
        <div class="iconView">
            <a  id="<%=resource.getEncodedAppId()%>"
				<%=resource.launchHref%>
				class="iconLink"
				title="<%=WebUtilities.escapeHTML(displayName)%>"
             ><%=resource.iconImg%><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(displayName))%></span><img
																				id="spinner_<%=resource.getEncodedAppId()%>"
																				class="spinner"
																				width="11"
																				height="11"
																				src="<%=resource.getDelayedLaunchImgSrc(false)%>" alt=""
			></a>
        </div>
<%
}
%>
<%-- A spacer div here avoids a problem with Safari 3.1 not clearing the mouse-over highlight --%>
<div class="spacer"></div>
</div>