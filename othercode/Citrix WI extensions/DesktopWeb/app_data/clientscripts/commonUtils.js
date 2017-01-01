// commonUtils.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.

function numbersonly(myfield, e) {
    var key;
    var keychar;

    if (window.event) {
        key = window.event.keyCode;
    } else if (e) {
        key = e.which;
    } else {
        return true;
    }
    keychar = String.fromCharCode(key);

    // control keys
    if ((key == null) || (key == 0) || (key == 8) ||
            (key == 9) || (key == 13) || (key == 27) ) {
        return true;
    }

    // numbers
    if ((("0123456789").indexOf(keychar) > -1)) {
        return true;
    }

    return false;
}



function putInTopFrame() {
    <%
    if (!wiContext.getConfiguration().getAllowDisplayInFrames()
        && !AGEUtilities.isAGEEmbeddedOrIndirectMode(wiContext)) {
    %>
    if (top != self) {
        top.location = self.location;
        alert("<%=wiContext.getString("LaunchInFramesNotAllowed")%>");
    }
    <% } %>
}

function clearFormData(){
    var submittedForm = document.<%=Constants.ID_CITRIX_FORM%>;
    for(i = 0; i < submittedForm.elements.length;i++){
         if (submittedForm.elements[i].name != "<%=SessionToken.ID_FORM%>"){
              submittedForm.elements[i].value = "";
         }
    }
}


<%
/**
 * private
 */

// Determine if the given window is the WI main frame
%>
function isMainFrame(w) {
    var d = null;

    try {
        d = w.document;
    } catch (e) {
        // Caught a JavaScript exception. This may happen when attempting to
        // access a document object which has different origins to the current
        // document, e.g. resides on a different server.
    }

    if (d != null) {
    <%
        // Hack to get around lack of getElementById() support on Windows Mobile 6.0
        // This method doesn't work on IE8.
        if (wiContext.getClientInfo().osPocketPC()) {
    %>
        return (d.<%=Constants.ID_DIV_LAUNCH%> != null);
    <% } else { %>
        return (d.getElementById('<%=Constants.ID_DIV_LAUNCH%>') != null);
    <% } %>
    } else {
        return false;
    }
}


<%
/**
 * private
 */

// Returns a reference to the WI main frame, or null if this could not be found
%>
function findMainFrame(w) {
    if (isMainFrame(w)) {
        return w;
    }

    if (w.parent == w) {
        // This is the top frame - nowhere else to look
        return null;
    } else {
        // Search the parent of this frame
        return findMainFrame(w.parent);
    }
}

<%
/**
 * private
 */

// Returns a reference the visible parent frame if this is a hidden frame
%>
function getTopFrame(w) {
    if (w != w.parent) {
        var doc = w.parent.document;
        var iframes = doc.getElementsByTagName('iframe');
        for (var i = 0; i < iframes.length; i++) {
            if (iframes[i].name.indexOf('<%=Constants.ID_FRAME_TIMEOUT%>') == 0) {
                return w.parent;
            }
        }
    }

    return null;
}

<%
/**
 * private
 */

// Gets whether the given window is a popup window
%>
function isPopupWindow(w) {
    var isPopup = false;

    try {
        // If a window and its opener have different origins (different host, port or protocol URL components)
        // Firefox throws a permission denied error when attempting to access properties on the opener.
        // This situation can arise if a customer creates an HTML page in one domain containing a link
        // to a WI site in a different domain.
        isPopup = (w.opener && w.opener!=null && !w.opener.closed && w.opener.parent != null);
    } catch (e) {}

    return isPopup;
}

<%
// Client side redirect to the WI main frame
%>
function redirectToMainFrame(href) {
    // See if this frame, or one of its parents, is the main frame
    var mainFrame = findMainFrame(window);

    // If this is a pop-up window, search the hierarchy of its opener
    var isPopup = isPopupWindow(window);
    if ((mainFrame == null) && isPopup) {
        mainFrame = findMainFrame(window.opener);
    }

    // Fall back to the current frame as we have nowhere else to go
    if (mainFrame == null) {
        mainFrame = window;
    }

    // Use location.replace to prevent the current page from appearing in
    // the browser history and being accessible via the browser's Back button.
    mainFrame.location.replace(href);

    // If this window is a pop-up and we did not redirect it, close it
    var redirectedCurrentWindow = (mainFrame == window);
    if (isPopup && !redirectedCurrentWindow) {
        window.close();
    }
}

<%
// Determine whether the element has an attribute with the given value.
%>
function isMatchedAttribute(element, attribute, attributeValue)
{
    if (attribute == "class")
    {
      var pattern = new RegExp("(^| )" + attributeValue + "( |$)");

      return pattern.test(element.className);
    }
    else if (attribute == "for")
    {
      if (element.getAttribute("htmlFor") || element.getAttribute("for"))
      {
        return element.htmlFor == attributeValue;
      }
    }
    else
    {
        return element.getAttribute(attribute) == attributeValue;
    }
}

<%
// Extracts the cursor position from an event.
%>
function getFrameCursorPosition(event)
{
    if (typeof event == "undefined")
    {
        event = window.event;
    }

    var scrollingPosition = getFrameScrollingPosition();
    var cursorX = 0;
    var cursorY = 0;

    if (typeof event.pageX != "undefined" && typeof event.x != "undefined")
    {
        cursorX = event.pageX;
        cursorY = event.pageY;
    }
    else
    {
        cursorX = event.clientX + scrollingPosition[0];
        cursorY = event.clientY + scrollingPosition[1];
    }

    return [cursorX, cursorY];
}

<%
// Extracts the target element from an event.
%>
function getEventTarget(event)
{
  var targetElement = null;

  if (typeof event.target != "undefined")
  {
    targetElement = event.target;
  }
  else
  {
    targetElement = event.srcElement;
  }

  return targetElement;
}

<%
// Attaches an event handler.
%>
function attachEventHandler(target, eventType, functionRef, capture)
{
  if (typeof target.addEventListener != "undefined")
  {
    target.addEventListener(eventType, functionRef, capture);
  }
  else if (typeof target.attachEvent != "undefined")
  {
    target.attachEvent("on" + eventType, functionRef);
  }
  else
  {
    eventType = "on" + eventType;

    if (typeof target[eventType] == "function")
    {
      var oldHandler = target[eventType];

      target[eventType] = function()
      {
        oldHandler();

        return functionRef();
      }
    }
    else
    {
      target[eventType] = functionRef;
    }
  }

  return true;
}

<% // Stops an event from being propagated back up through ancestor elements. %>
function stopEventPropagation(e)
{
    e = e || event;/* get IE event (not passed) */
    e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
}

<%
//Dynamically create an iframe and append it to the specified div (or any element).
//If the clearDiv parameter is true, the function deletes the existing contents of
//the div before adding the frame.
%>
function addFrameToDiv(divId, frameIdPrefix, frameSrc, frameTitle, clearDiv) {
    var container = document.getElementById(divId);

    if (container) {
        // Remove existing child elements if required
        while (clearDiv && container.hasChildNodes()) {
            container.removeChild(container.lastChild);
        }

        var iframe = document.createElement('IFRAME');

        iframe.id = frameIdPrefix + getFrameSuffix();
//        iframe.width = '0';
//        iframe.height = '0';
        iframe.name = frameIdPrefix + getFrameSuffix();
        iframe.src = frameSrc;
        iframe.title = frameTitle;
        container.appendChild(iframe);
    }
}

function newAjaxRequest() {
    try { return new XMLHttpRequest(); } catch(e) {}
    try { return new ActiveXObject("Msxml2.XMLHTTP"); } catch (e) {}
    try { return new ActiveXObject("Microsoft.XMLHTTP"); } catch (e) {}
    return null;
}

function assignDesktop(elt) {
    var req = newAjaxRequest();
    var href = elt.href;
    var ajaxUrl = href.replace('<%= Constants.PAGE_LAUNCHER %>', '<%= Constants.PAGE_ASSIGN_DESKTOP %>');

    if (req != null) {
        // Show the spinner icon and disable the app link to prevent the user from initiating
        // multiple simultaneous assignment requests for the same desktop. The link is not re-enabled
        // since the markup for the desktop is replaced (assignment succeeded) or the page is redirected
        // (assignment failed).
        disableAppLink(elt);

        setSpinnerVisible(elt.id, true);

        req.onreadystatechange = function() { handleDesktopAssignmentResponse(elt, req) };
        req.open("GET", ajaxUrl, true);
        req.send(null);
    } else {
        // If ajax is not available just launch the desktop without updating the UI.
        // (this situation would arise when using IE6 with ActiveX disabled.)
        // In this case the assignment is performed by the broker.
        // When there are multiple assign-on-first-use desktops within a group, the user
        // will need to press refresh (or log off/on) to see the updated desktop name and
        // assign additional desktops.
        addCurrentTimeToHref(elt, elt.href);
        launch(elt);
    }
}

function handleDesktopAssignmentResponse(elt, req)
{
    if (req.readyState != 4 || req.status != 200)  {
        return;
    }

    var json = eval('(' + req.responseText + ')');

    // A redirect URL is returned in the ajax response if the desktop assignment failed
    if (json.redirectUrl) {
        location.href = json.redirectUrl;
        return;
    }

    if (json.feedbackMessage) {
        setFeedback(json.feedbackMessage, 'Info');
    }

    var target = document.getElementById("desktop_" + elt.id)
    if (target) {
        var markup = unescapeHTML(json.markup);
        target.innerHTML = markup;

        // The original elt has now been replaced in the DOM (with updated markup for the element's desktop group).
        // The following line locates the corresponding replaced element, so that the launch() function can operate on
        // the 'live' element (e.g., to make it inactive) rather than the element that has been removed from the DOM.
        elt = document.getElementById(elt.id);

        <% if(!Include.isCompactLayout(wiContext)) { %>
            // Update the layout since the page size may have changed. This is only needed for full graphics mode.
            updateLayout();
        <% } %>
    }

    if (json.autoLaunch) {
        addCurrentTimeToHref(elt, elt.href);
        launch(elt);
    }
}

<% // This does the reverse operation to WebUtilities.escapeHTML() %>
function unescapeHTML(markup) {
    markup = markup.replace(/&lt;/g, "<");
    markup = markup.replace(/&gt;/g, ">");
    markup = markup.replace(/&quot;/g, "\"");
    markup = markup.replace(/&#39;/g, "\'");
    markup = markup.replace(/&#37;/g, "%");
    markup = markup.replace(/&#59;/g, ";");
    markup = markup.replace(/&#40;/g, "(");
    markup = markup.replace(/&#41;/g, ")");
    markup = markup.replace(/&amp;/g, "&");
    markup = markup.replace(/&#43;/g, "+");

    return markup;
}

<%
// private
//
// The onclick handler for when an app link is inactive. It returns false to prevent the
// href being followed.
%>
function nolaunch() { return false; }

<%
// private
//
// Replace the element's onclick handler and change its class name so that it can
// be styled differently (mouse pointer changed).
%>
function disableAppLink(elt)
{
    elt.classNameOrig = elt.className;
    elt.className = elt.className + " iconLinkLaunching";

    elt.onclickOrig = elt.onclick;
    elt.onclick = nolaunch;        
}

<%
// private
//
// Restore the element's original onclick handler and class name.
%>
function enableAppLink(elt)
{
    elt.className = elt.classNameOrig;
    elt.onclick = elt.onclickOrig;        
}


function showDesktopLaunchingUI(elt) {
    var launchTimeout = <%=wiContext.getConfiguration().getMultiLaunchTimeout()%> * 1000;

    if (launchTimeout > 0) {
        disableAppLink(elt);

        setSpinnerVisible(elt.id, true);
        setRestartPaneVisible(elt.id, true);
    }
    
    // Called before adding the frame to the div as otherwise it
    // breaks on Windows Mobile 6.1 (spinner never disappears).
    setTimeout(function() {
                   setRestartPaneVisible(elt.id, false);
                   setSpinnerVisible(elt.id, false);
                   updateDesktopIcon(elt.id, true);
                   enableAppLink(elt);
               }, launchTimeout);
}


<%
// Trigger a launch by dynamically creating an iframe and populating it with the supplied url.
// The launch is done in this way instead of using a static iframe to avoid inadvertent launches
// when the user refreshes the browser window or clicks the browser's Back button.
%>
function launch(elt) {
    showDesktopLaunchingUI(elt);
    autolaunch(elt.href);
}

function autolaunch(url) {
    addFrameToDiv('<%=Constants.ID_DIV_LAUNCH%>',
                  '<%=Constants.ID_FRAME_LAUNCH%>',
                  url,
                  '<%=wiContext.getString("LaunchFrameTitle")%>',
                  true);
}

<%
// Shows or hides the launch spinner.
%>
function setSpinnerVisible(appId, show) {
    var eltApp = window.top.document.getElementById("spinner_" + appId);
    if (eltApp) {
        eltApp.src = (show) ? "../media/LaunchSpinner.gif" : "../media/Transparent16.gif";
    }

    // For desktops tab, show different spinner image.
    var eltDesktop = window.top.document.getElementById("desktopSpinner_" + appId);
    if (eltDesktop) {
        var spinnerClass = (show) ? "delayedImageSpinner" : "delayedImageNone";
        eltDesktop.className = spinnerClass;
        eltDesktop.classNameOnBlur = spinnerClass;
    }
}

<%
//Adds the retry frame to the page for delayed desktop launches.
//The frame is added with a timeout callback in order to work around
//caching/reload issues in Firefox 3. (If the frame is embedded
//directly in the page FF3.0.5 doesn't load its contents correctly
//when the user uses the browser back/forward buttons. This leads to
//some desktops launching repeatedly and others not launching at all.)
%>
function addDesktopRetryFrame(frameSrc) {
    setTimeout(function(){
        addFrameToDiv('<%=Constants.ID_DIV_RETRYPOPULATOR%>',
                      '<%=Constants.ID_FRAME_RETRY_POPULATOR%>',
                      frameSrc,
                      '<%=wiContext.getString("RetryFrameTitle")%>',
                      false);
    },1);
}



<%
// Array.indexOf() is not implemented in IE 7 and hence this implementation.
%>
function indexOfElement(arrayToSearch, elementName) {
    for( var i =0; i<arrayToSearch.length; i++) {
        if(arrayToSearch[i] == elementName) {
           return i;
        }
    }
    return -1;
}


<%
// Some of the buttons we display are links with background images,
// which dont display correctly if high contrast mode is enabled.
// In such cases we revert back to the normal html buttons for proper displaying.
%>
function maintainAccessibility(buttonId, displayInline) {
  if (isHighContrastEnabled()) {
      displayHighContrastButton(buttonId, displayInline);
  }
}

<%
// Returns whether the high contrast mode is enabled or not.
// Set a background image on a div and try to see whether we
// can retrieve it dynamically.
// If the high contrast mode is enabled, it always returns 'none'.
%>
function isHighContrastEnabled() {
    var testDiv = document.createElement("div");
    testDiv.style.background = "url(../media/Error24.gif)";
    testDiv.style.display="none";
    document.body.appendChild(testDiv);
    // test for high contrast
    var backgroundImage = null;
    if (window.getComputedStyle) {
        var cStyle = getComputedStyle(testDiv, "");
        backgroundImage = cStyle.getPropertyValue("background-image");
    } else {
        backgroundImage = testDiv.currentStyle.backgroundImage;
    }
    if (backgroundImage != null && backgroundImage == "none" ) {
        return true;
    }
    return false;
}

<%
// Displays the normal html button if high contrast mode is enabled.
%>
function displayHighContrastButton(buttonId, displayInline) {
   var graphicButton = document.getElementById("graphic_"+buttonId);
   if (graphicButton != null) {
       graphicButton.style.display='none';
   }
   var highContrastButton = document.getElementById("highContrast_"+buttonId);
   if (highContrastButton != null) {
      if (displayInline) {
          // Currently search button needs to be displayed inline for better alignment.
          highContrastButton.style.display='inline';
      } else {
          highContrastButton.style.display='block';
      }
   }
}

<% // Change "tab" in low graphics layout (the UI is actually a dropdown list) %>
function lgChangeTab(dropdown)
{
    location.href = dropdown.options[dropdown.selectedIndex].value;
}

function setRestartPaneVisible(desktopId, isVisible) {
    var container = document;
    var restartElt = container.getElementById("restart_" + desktopId);

    if (!restartElt) {
        container = window.top.document;
        restartElt = container.getElementById("restart_" + desktopId);
    }

    if (restartElt && restartElt.className != "restartLinkNotRestartable") {
        restartElt.className  = isVisible ? "restartLinkAlwaysShow" : "restartLinkShowOnFocus";
    }

    
}


function updateDesktopIcon(desktopId, isActive) {
    var screenElt = document.getElementById("screen_" + desktopId);
    if (!screenElt) {
        screenElt = window.top.document.getElementById("screen_" + desktopId);
    }

    if (screenElt) {
        screenElt.className = isActive ? "desktopScreen activeDesktop" : "desktopScreen";
    }
}

function updateDelayedLaunchImage(desktopId, active) {
    var desktopIconElt = document.getElementById('desktopSpinner_' + desktopId);
    if (!desktopIconElt) {
        return;
    }

    if (active) {
        desktopIconElt.classNameOnBlur = desktopIconElt.className;
        
        if (desktopIconElt.className == "delayedImageNone") {
            setLaunchReadyIconRollOver(document, desktopId);
        }
    } else {
        desktopIconElt.className = desktopIconElt.classNameOnBlur;
    }
}

function updateDirectLaunchDisplay(elt, active) {
    var desktopId = null;
    if (elt.id.indexOf('screen_') == 0) {
        desktopId = elt.id.substring(7);
        updateDelayedLaunchImage(desktopId, active);
    }
}

function updateDesktopDisplay(elt, active) {
    elt.className = (active) ? "desktopResource desktopFocus" : "desktopResource";
    
    var desktopId = null;
    if (elt.id.indexOf('desktop_') == 0) {
        desktopId = elt.id.substring(8);
        updateDelayedLaunchImage(desktopId, active);
    }
}

function setLaunchReadyIconRollOver(doc, desktopId) {
    var spinnerNodeDesktopsTab = doc.getElementById('desktopSpinner_' + desktopId);
    if (spinnerNodeDesktopsTab) {
        spinnerNodeDesktopsTab.className = "delayedImagePlay";
    }
}

// Changes the launch spinner into an icon indicating the resource is ready to be manually launched
function setLaunchReadyIcon(doc, desktopId) {
    var spinnerNode = doc.getElementById('spinner_' + desktopId);
    if (spinnerNode) {
        spinnerNode.src = "../media/LaunchReady.gif";
    }
    
    var spinnerNodeDesktopsTab = doc.getElementById('desktopSpinner_' + desktopId);
    if (spinnerNodeDesktopsTab) {
        spinnerNodeDesktopsTab.className = "delayedImagePlay";
        spinnerNodeDesktopsTab.classNameOnBlur = "delayedImagePlay";
    }
}

function showRestart(id, isActive) {
    var el = document.getElementById("restartConfirmation_" + id);
    el.style.visibility = isActive ? "visible" : "hidden";
}