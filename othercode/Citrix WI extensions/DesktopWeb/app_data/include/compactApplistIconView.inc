<%
// compactApplistIconView.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
// We use a series of divs rather than a list because some devices (eg HP iPaq)
// insist on rendering list items vertically.
%>
<div id="iconsView">
<%
  int iconNumber = 0;
  // Populate the folders
  for (int i = 0; i < viewControl.getNumFolders(); i++) {
    String folderPath = viewControl.folders[i].getDisplayPath() + viewControl.folders[i].getDisplayName();
%>
  <div class="col1">
    <p>
      <a id="folderLink_<%=i%>" class="iconLink expandedLink folderLink" href="<%=Constants.PAGE_APPLIST%>?CTX_CurrentFolder=<%=WebUtilities.escapeURL(folderPath)%>"
        title="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>"
        ><img alt="<%=WebUtilities.escapeHTML(viewControl.folders[i].getDisplayName())%>" src="<%=viewControl.folderIconPath%>"><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(viewControl.folders[i].getDisplayName()))%></span></a>
    </p>
  </div>
<%
    iconNumber++;
} // end getNumFolders
%>
<%
  // Populate resources
  for (int i = 0; i < viewControl.getNumResources(); i++) {
    ResourceControl resource = viewControl.resources[i];
%>
  <div class="col1">
    <p>
      <a id="<%=resource.getEncodedAppId()%>"
		 <%=resource.launchHref%>
		 class="iconLink expandedLink"
		 title="<%=WebUtilities.escapeHTML(resource.getName(wiContext))%>"
      ><%=resource.iconImg%><br><span><%=WebUtilities.escapeHTML(viewControl.getTruncatedName(resource.getName(wiContext)))%></span><br><img
																								id="spinner_<%=resource.getEncodedAppId()%>"
																								class="spinner"
																								width="11"
																								height="11"
																								src="<%=resource.getDelayedLaunchImgSrc(false)%>" alt=""
	 ></a>
    </p>
  </div>
<%
    iconNumber++;
} // end for getNumResources()
%>
</div>