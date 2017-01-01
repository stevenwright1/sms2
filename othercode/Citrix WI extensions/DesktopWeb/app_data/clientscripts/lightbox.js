<%
// lightbox.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function configureLightbox() {
    var mask = document.getElementById('lightboxMask');
    var viewportSize = getFrameViewportSize();

    // The lightbox overlay mask is resized to cover the viewport and the entire
    // document (whichever is larger). This prevents the underlying browser contents
    // from being revealed when scrolling in IE6 (which does not support 'position:fixed').
    var maskWidth = Math.max(viewportSize[0], document.body.clientWidth);
    var maskHeight = Math.max(viewportSize[1], document.body.clientHeight);

    mask.style.width = maskWidth + 'px';
    mask.style.height = maskHeight + 'px';

    // Calculate the position for the lightbox itself.
    var dialogTop = (viewportSize[1] / 6);

    // IE6 uses 'position:absolute', so the scroll position needs to be added on.
<% if (wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() < 7)) { %>
    var scrollPosition = getFrameScrollingPosition();
    dialogTop += scrollPosition[1];
<% } %>

    var lightbox = document.getElementById('lightbox');
    lightbox.style.top = parseInt(dialogTop) + 'px';
}

function showLightbox(func) {
    document.getElementById('lightboxMask').style.display = 'block';
    document.getElementById('lightbox').style.display = 'block';
    lightboxCommitFunction = func;

<% if (wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() > 6)) { %>
    // Show and hide the buttons to work around a rendering bug in IE7 where the button labels are incorrectly
    // positioned outside the button boundary. This problem occurs after the lightbox has been shown and is
    // then shown again with a different message that alters the lightbox height.
    document.getElementById('okButtonPane').style.display = 'none';
    document.getElementById('okButtonPane').style.display = 'block';

    document.getElementById('cancelButtonPane').style.display = 'none';
    document.getElementById('cancelButtonPane').style.display = 'block';
<% } %>
}

function displayLightbox(anchor) {
    showLightbox(function() {redirectToMainFrame(anchor.href);});
}

// Dynamically pass in the top/bottom messages, and desktopId to the light box.
// If we get a valid desktopId in this method, we know that a delayed launch is already
// happening for that particular desktopId.
function showLightboxWithMessage(anchor, title, message, desktopId) {
    setLightboxTitle(title);
    setLightboxMessage(message);
    
    // This desktopId will get set only if the user clicks on the restart button while a
    // delayed launch is already happening, when the applist page was rendered.
    if (desktopId != null) {
        window.parent.parent.desktopShowingLightbox = desktopId;
        var desktopIndex = -1;
        if (window.parent.parent.delayedLaunchDesktops != null && window.parent.parent.delayedLaunchDesktops != ""){
           // Check if the delayed launch is still happening and if not update the lightbox text.
           desktopIndex = indexOfElement(window.parent.parent.delayedLaunchDesktops, desktopId);
        }
    }

    displayLightbox(anchor);
}

function hideLightbox(okPressed) {
    document.getElementById('lightboxMask').style.display = 'none';
    document.getElementById('lightbox').style.display = 'none';

    if (okPressed && (typeof lightboxCommitFunction == "function")) {
        lightboxCommitFunction();
    }
}

function handleLightboxKeys(e) {
    var keyCode = window.event ? window.event.keyCode : e.keyCode;

    if (keyCode == 27) { // Escape
        hideLightbox();
    }
}

function setLightboxString(id, message) {
    var elt = document.getElementById(id);

    if (elt != null) {
        elt.innerHTML = message;
        if (!message) {
            elt.style.display = 'none';
        } else {
            elt.style.display = 'block';
        }
    }
}

function setLightboxTitle(title) {
    setLightboxString('lightboxHeading', title);
}

function setLightboxMessage(message) {
    setLightboxString('lightboxMessage', message);
}

document.onkeypress = handleLightboxKeys;
