<%
// delayedLaunch.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

// Functions for handling delayed launches in JavaScript (used by retry page)
%>

// Updates the delayed resource UI for the Direct Launches.
// This method removes the progress bar and replaces the launching status string with a launch link
function updateDelayedDirectLaunchUI(desktopId, launchLink){
    var doc = getMainWindowDoc(desktopId);
    updateTitle(doc);
    removeProgressIndicator(doc, desktopId);
    updateLaunchLink(doc, desktopId, launchLink);
	updateDesktopIcon(desktopId, true);
	setRestartPaneVisible(desktopId, false);

	<% if (LaunchUtilities.browserBlocksLaunch(wiContext)) {
	%>
		setLaunchReadyIcon(window.top.document, desktopId);
	<%
	} else {
	%>
		setSpinnerVisible(desktopId, false);
	<%
	}
	%>
    var lightboxText = "<%=wiContext.getString("SwitchOff", WebUtilities.escapeHTML(viewControl.desktopDisplayName))%>";
    updateLightboxText(doc, lightboxText);

    // Update a variable in the main window to record that the retry operation is complete.
    window.parent.parent.ctxRetryInProgress = false;
}

// Updates the delayed resource UI for a desktop that cannot be auto-launched:
//   - displays an icon indicating the desktop is ready.
//   - displays a feedback message informing the user that the desktop is ready.
//   - if the lightbox is already showing for this desktop (when the delayed launch is finished), the lightbox text is updated.
function updateDelayedManualLaunchUI(desktopId) {
    var doc = getMainWindowDoc(desktopId);
    setLaunchReadyIcon(doc, desktopId);
	updateDesktopIcon(desktopId, true);
	setRestartPaneVisible(desktopId, false);

    var resourceReadyMessage = "<%=wiContext.getString("DelayedLaunchReady", WebUtilities.escapeHTML(viewControl.desktopDisplayName))%>";
    setFeedback(resourceReadyMessage, 'Info', doc);

    var lightboxMsg = "<%=wiContext.getString("SwitchOff", WebUtilities.escapeHTML(viewControl.desktopDisplayName))%>";
    updateDelayedLaunchHistory(doc, desktopId, lightboxMsg);
}

// Updates the desktop title string
function updateTitle(doc) {
    var titleNode = doc.getElementById('directLaunchTitle');
    if (titleNode != null) {
        titleNode.innerHTML = "<%=wiContext.getString("ReadyToConnect", WebUtilities.escapeHTML(viewControl.desktopDisplayName))%>";
    }
}

// Removes the progress bar
function removeProgressIndicator(doc, desktopId) {
    var progressNode = doc.getElementById('progress_' + desktopId);
    if (progressNode != null){
        var parentNode = progressNode.parentNode;
        parentNode.removeChild(progressNode);
    }
}

// This method replace launch status string with the launch link
function updateLaunchLink(doc, desktopId, launchLink){
    var statusNode = doc.getElementById("launchStatus_" + desktopId);
    if (statusNode != null){
        statusNode.innerHTML = launchLink;
    }
}


// Hides the connecting icon from the tab header.
function hideTabHeaderConnectingIcon(appId, appTabName) {
   var doc = getMainWindowDoc(appId);
   var tabHeaderTextNode = doc.getElementById(appTabName);
   if (tabHeaderTextNode != null) {
       var items = tabHeaderTextNode.getElementsByTagName("IMG");
       if (items != null && items.length == 1) {
           // There is at most 1 image element inside the list at present, which is the connecting icon.
           items[0].style.visibility ='hidden';
       }
   }
}
