<%
// removeDelayedUI.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

// Functions for removing the delayed launch UI
%>
// This array keeps track of all the delayed launches which are currently in progress.
var delayedLaunchDesktops = [];
// Variable keeping track of any desktops which shows a lightbox, so that we can update the
// lightbox text once the restart is finished.
var desktopShowingLightbox = "";

// Gets the main frame document from inside the RetryFrame
// otherwise returns the current window's document
function getMainWindowDoc(desktopId) {
    if (window.name == desktopId){
        return window.parent.parent.document;
    } else {
        return window.document;
    }
}

// Updates the delayed resource UI for a desktop that can be auto-launched:
//   - hides the spinner icon.
//   - clears any feedback message that is currently displayed.
//   - if the lightbox is already showing for this desktop (when the delayed launch is finished), the lightbox text is updated.
function updateDelayedAutoLaunchUI(desktopId, lightboxMsg) {
    var doc = getMainWindowDoc(desktopId);
    if (doc != null) {
        // Hide the launch spinner.
        setSpinnerVisible(desktopId, false);
        updateDesktopIcon(desktopId, true);
		setRestartPaneVisible(desktopId, false);

        // Clear any feedback message, most likely the one notifying the user that their desktop is being started.
        clearFeedback(doc);

        // Update the delayed launch history
        updateDelayedLaunchHistory(doc, desktopId, lightboxMsg);
    }
}

function updateDelayedLaunchHistory(doc, desktopId, lightboxMsg) {
    // Check if a delayed launch is already happening for this particular desktopId.
    var desktopIndex = indexOfElement(window.parent.parent.delayedLaunchDesktops, desktopId);
    if (desktopIndex != -1) {
       // remove the desktopId from the array
       window.parent.parent.delayedLaunchDesktops.splice(desktopIndex, 1);
       // update the lightbox text if its already showing.
       if (window.parent.parent.desktopShowingLightbox == desktopId) {
           updateLightboxText(doc, lightboxMsg);
           window.parent.parent.desktopShowingLightbox = "";
       }
    }
}


function updateLightboxText(doc, lightboxMsg) {
    var elt = doc.getElementById('lightboxMessageTop');
    if (elt != null) {
        elt.innerHTML = lightboxMsg;
        elt.style.display = 'block';
    }
    elt = doc.getElementById('lightboxMessageBottom');
    if (elt != null) {
        elt.innerHTML = "<%=wiContext.getString("UnsavedWorkLost")%>";
        elt.style.display = 'block';
    }
}
