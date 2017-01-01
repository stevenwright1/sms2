<%
// util.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

<%
// Expands/collapses the more security content
%>
function toggleMoreSecurity() {
    var securityContent = document.getElementById("MoreSecurityContent");
    if (securityContent) {

        var securityContentClass = securityContent.className;
        if (securityContentClass == "ShowMoreSecurity") {
            securityContent.className = "HideMoreSecurity";
        } else if (securityContentClass == "HideMoreSecurity") {
            securityContent.className = "ShowMoreSecurity";
        }
    }
    safeUpdateLayout();
}

function displayDiv(name){
    document.getElementById(name).style.display = "";
}

function alreadyInstalled(url){
    setWizardCookieItem('<%=WizardConstants.USER_PREFERRED %>','<%=WizardConstants.NATIVE%>');
    location.href=url;
}

<%
// Expands/Collapses a section based on the current content class
%>
function toggleSection(divId, linkId) {
    var divElement = document.getElementById(divId);
    var linkElement = document.getElementById(linkId);

    if (divElement && linkElement) {
        var divClass =  divElement.className;
        if (divClass == "SectionClose") {
            divElement.className = "SectionOpen";
        } else if (divClass == "SectionOpen") {
            divElement.className = "SectionClose";
        }
    }
    safeUpdateLayout();
}

<%
// The function called "updateLayout" is a callback to function that may or may not be implemented
// in the code decorating the client detection wizard.  It is called when the on screen display is
// expected to have changed size. i.e. When one of the information sections above have been expanded
// or collapsed. Currently it is only implemented fully for full graphics mode WI.
%>
function safeUpdateLayout() {
    if ((typeof updateLayout) == "function") {
        updateLayout();
    }
}

<%
// This checks if the user has clicked the checkbox
// Only if they have clicked the box is the startDownload function called
%>
function downloadButtonClicked() {
    if (!document.getElementById) {
        return; // haven't got the correct support on this browser
    }

    // must check the check box first
    var checkBox = document.getElementById("chkLegalStatement");
    if (!checkBox || checkBox.checked) {
        // if it is checked, or there is no check box
        // do the required work
        startDownload();
    } else {
        // if there is a check box, and it is not checked
        // do not execute the link, and show the warning instead
        showDownloadWarningText();
        var downloadWarningTextLink = document.getElementById("downloadWarningTextLink");
        if(downloadWarningTextLink) {
            // Focus the link, so that it will be read by a screen reader
            downloadWarningTextLink.focus();
        }
    }
}

<%
// There are the URLs for the download button
%>
var disabledURL = "<%=WizardUtil.getDownloadButtonURL(false, false, wizardContext)%>";
var enabledURL = "<%=WizardUtil.getDownloadButtonURL(true, false, wizardContext)%>";
var rolloverURL = "<%=WizardUtil.getDownloadButtonURL(true, true, wizardContext)%>";

<%
// This changes the button colour to match whether the check box is checked or not
// This is called whever the check box is changed
%>
function updateDownloadButton() {
    if (!document.getElementById) {
        return false; // haven't got the correct support on this browser
    }

    var checkBox = document.getElementById("chkLegalStatement");
    var downloadButtonImg = document.getElementById("downloadButtonImg");

    if (!downloadButtonImg) {
        return false; // the correct elements don't exist
    }

    if (!checkBox || checkBox.checked) {
        downloadButtonImg.src = enabledURL;
    } else {
        // only show this if there is a check box
        // and it is checked
        downloadButtonImg.src = disabledURL;
    }

    // always hide the warning text
    // whether mouse out or clicking the box
    hidePopup("downloadWarningText");
}

<%
// This changes the button to glow if they
// have accepted the licence, and warn them otherwise
%>
function mouseOverDownloadButton() {
    if (!document.getElementById) {
        return false; // haven't got the correct support on this browser
    }

    var checkBox = document.getElementById("chkLegalStatement");
    var downloadButtonImg = document.getElementById("downloadButtonImg");

    if (!downloadButtonImg) {
        return false; // the correct elements don't exist
    }

    // we want to rollover if there is no check box,
    // or if it exists and is checked
    if (!checkBox || checkBox.checked) {
        downloadButtonImg.src = rolloverURL;
    } else {
        showDownloadWarningText();
    }
}

<%
// This returns the button to its proper state
%>
function mouseOutDownloadButton() {
    updateDownloadButton();
}

<%
// Show the licence warning
%>
function showDownloadWarningText() {
    if (!document.getElementById) {
        return false; // haven't got the correct support on this browser
    }

    var downloadButtonImg = document.getElementById("downloadButtonImg");
    var downloadWarningText = document.getElementById("downloadWarningText");

    if (!downloadButtonImg || !downloadWarningText) {
        return false; // the correct elements don't exist
    }

    var buttonW = downloadButtonImg.offsetWidth;
    var buttonH = downloadButtonImg.offsetHeight;
    var tooltipW = downloadWarningText.offsetWidth;
    var tooltipH = downloadWarningText.offsetHeight;

    var buttonPosition = getElementPosition(downloadButtonImg);
    var horizonMainPane = document.getElementById("horizonMainPane");
    var x, y;
    // For horizon pages the tooltip is positioned above the button; for other pages to the right of it.
    if (horizonMainPane) {
        var horizonMainPanePosition = getElementPosition(horizonMainPane);

        x = buttonPosition[0] + ((buttonW - tooltipW) / 2);
        y = buttonPosition[1] - tooltipH;
        // Reduce the y coordinate since the tooltip is displayed relative to the horizon main pane (it uses position:relative).
        y = y - horizonMainPanePosition[1];
    } else {
        x = buttonW;
        y = buttonPosition[1] + ((buttonH - tooltipH) / 2);
    }

    // show the warning box
    applyShowPopupStyle(downloadWarningText, x, y);
}

<%
// Function called when the user "clicks" (or presses enter) on the warning text
%>
function warningTextClicked() {
    if (!document.getElementById) {
        return false; // haven't got the correct support on this browser
    }
    updateDownloadButton(); // this will hide the warning text.

    // Return focus to the download button
    if (isHighContrastEnabled()) {
        document.getElementById("highContrast_Download").focus();
    } else {
        document.getElementById("Download").focus();
    }
}

function upgradeLater() {
    setWizardCookieItem('<%=WizardConstants.COOKIE_UPGRADE_LATER%>','<%=WizardConstants.VAL_TRUE%>');
    location.href='<%=wizardContext.getModel().getCurrentStep()%>';
}

<%
// Hide a popup
%>
function hidePopup(id) {
    if (!document.getElementById) {
        return false; // havn't got the correct support on this browser
    }

    var popup = document.getElementById(id);

    if (!popup) {
        return false; // the correct elements don't exsist
    }

    popup.style.top = "-999px";
    popup.style.left = "-999px";
    popup.style.visibility = 'hidden'; // stop screenreader reading it
}

<%
// Show the given popup in the given location
%>
function applyShowPopupStyle(popup, x, y) {
    // make sure it stays on the screen
    var position = shuffle(popup, [x, y], 0, 0, 0);
    popup.style.left = position[0] + "px";
    popup.style.top = position[1] + "px";
    popup.style.visibility = 'visible'; // allow screenreader to read it
}

<%
// Gets the x y co-ordinates of an arbitary element
%>
function getElementPosition(elt) {
  var positionX = 0;
  var positionY = 0;

  while (elt != null)
  {
    positionX += elt.offsetLeft;
    positionY += elt.offsetTop;
    elt = elt.offsetParent;
  }

  return [positionX, positionY];
}

<%
// Returns the viewport size (of our frame) in pixels
%>
function getFrameViewportSize()
{
  var sizeX = 0;
  var sizeY = 0;

  if (typeof window.innerWidth != 'undefined')
  {
      sizeX = window.innerWidth;
      sizeY = window.innerHeight;
  }
  else if (typeof document.documentElement != 'undefined'
      && typeof document.documentElement.clientWidth != 'undefined'
      && document.documentElement.clientWidth != 0)
  {
      sizeX = document.documentElement.clientWidth;
      sizeY = document.documentElement.clientHeight;
  }
  else
  {
      sizeX = document.getElementsByTagName('body')[0].clientWidth;
      sizeY = document.getElementsByTagName('body')[0].clientHeight;
  }

  return [sizeX, sizeY];
}

<%
/**
 * private
 */

// Returns the scolling position (of our frame) in pixels
%>
function getFrameScrollingPosition()
{
  var scrollX = 0;
  var scrollY = 0;

  if (typeof window.pageYOffset != 'undefined')
  {
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
  }

  else if (typeof document.documentElement.scrollTop != 'undefined'
      && (document.documentElement.scrollTop > 0 ||
      document.documentElement.scrollLeft > 0))
  {
      scrollX = document.documentElement.scrollLeft;
      scrollY = document.documentElement.scrollTop;
  }

  else if (typeof document.body.scrollTop != 'undefined')
  {
      scrollX = document.body.scrollLeft;
      scrollY = document.body.scrollTop;
  }

  return [scrollX, scrollY];
}
