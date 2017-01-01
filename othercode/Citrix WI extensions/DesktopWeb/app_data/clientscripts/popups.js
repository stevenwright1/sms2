<%
// popups.js
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
// ---------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------
// FUNCTIONS FOR USING POPUP HELP, MENUS ETC
// ---------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------
%>

<%
// ---------------------------------------------------------------------------------------------------------------
// HELPER FUNCTIONS
// ---------------------------------------------------------------------------------------------------------------
%>
<%
// If no delay is specified, this default delay will be used when showing a delayed popup.
%>
function getDefaultPopupShowDelay()
{
    return 500;
}

<%
// If no delay is specified, this default delay will be used when hiding a delayed popup.
%>
function getDefaultPopupHideDelay()
{
    return 200;
}

<%
// Creates an id for a popup based on the id of the element it is associated with.
//
// associatedId specifies the id of the HTML element the popup will be associated with.
%>
function getPopupId(associatedId)
{
    return "Popup_" + associatedId;
}

<%
//
// Displays a popup near to its associated element.
//
// associatedId - associated element id
%>
function show_popup_helper(associatedId)
{
    clearPopupTimer(associatedId);

    var popupId = getPopupId(associatedId);
    var popup = document.getElementById(popupId);
    var associated = document.getElementById(associatedId);

    <%
    // It could be that by the time we got round to actually displaying the popup,
    // it is no longer wanted (eg the user moved their mouse out), so we check for this here.
    %>
    if(!isPopupWanted(associatedId))
    {
        return;
    }

    <% // Calculate the desired position of our popup %>
    var desiredPosition = popup.savedCursorPosition;
    var onScreenPosition;

    if(desiredPosition)
    {
        onScreenPosition = shuffle(popup, desiredPosition, 0, 10, 25);
    }
    else
    {
        <% // No data on the cursor position - position the popup just below or above the parent element %>
        desiredPosition = getElementPosition(associated);
        <% if (wiContext.getClientInfo().isIE()) {
        // these are only to make the drop down menu in the nav bar look perfect
        // as this clause is currenly only used for the preferences drop down%>
          desiredPosition = [desiredPosition[0]-1, desiredPosition[1]];
        <% } else if (wiContext.getClientInfo().isFirefox()) {%>
//        desiredPosition = [desiredPosition[0], desiredPosition[1]+1];
        <% } %>
        if (isMatchedAttribute(popup, "class", "rightAligned")) {
            onScreenPosition = shuffle(popup, desiredPosition, associated.offsetWidth, 0, 0);
        } else {
            onScreenPosition = shuffle(popup, desiredPosition, 0, 0, associated.offsetHeight);
        }
    }

    popup.style.left = onScreenPosition[0] + 'px';
    popup.style.top = onScreenPosition[1] + 'px';

    <% // Fix IE glitches %>
    <% if(wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() < 7)) { %>
        createIframeLayer(popup);
    <% } %>
}

<%
// This margin is set quite large because the view port size calculation doesn't
// work properly on firefox (reports the scrollbars as part of the viewport)
%>
shuffle.margin = 25;

<%
// Shuffles the position of a popup to try and make it entirely visible within the
// viewport.
//
// popup - the popup element to shuffle
// originalPosition - the position of the cursor OR the position of the element triggering the popup
// offsetRight - if we decide to place the popup right of the given position, this is the offset to use
// offsetAbove - if we decide to place the tooltip above the given position, this is the offset to use
// offsetBelow - if we decide to place the tooltip below the given position, this is the offset to use
%>
function shuffle(popup, originalPosition, offsetRight, offsetAbove, offsetBelow)
{
    // Make a copy of the originalPosition
    var position = [originalPosition[0], originalPosition[1]];

    viewPortSize = getFrameViewportSize();
    scollingPos = getFrameScrollingPosition();

    <% // too far right? %>
    var rightmost = position[0] + offsetRight + popup.offsetWidth;
    var rightmostVisible = scollingPos[0] + viewPortSize[0] - shuffle.margin;
    if( rightmost > rightmostVisible )
    {
        position[0] = rightmostVisible - popup.offsetWidth;
    }

    <% // too far left? %>
    var leftmost = position[0] + offsetRight;
    var leftmostVisible = scollingPos[0] + shuffle.margin;
    if (leftmost < leftmostVisible) {
        position[0] = leftmostVisible;
    } else {
        position[0] = leftmost;
    }

    <% // See if placing the popup below the cursor works %>
    var bottommost = position[1] + popup.offsetHeight + offsetBelow;
    var bottommostVisible = scollingPos[1] + viewPortSize[1] - shuffle.margin;
    if( bottommost > bottommostVisible )
    {
        <%
        // The bottom of the popup would not be visible. See if we can place it such that
        // the bottom of the pop up is offsetAbove above the cursor.
        %>
        var topmost = position[1] - popup.offsetHeight - offsetAbove;
        var topmostVisible = scollingPos[1] + shuffle.margin;
        if( topmost >= topmostVisible )
        {
            <% // The popup can sit between the top of the visible screen and the cursor.
               // It is bottom aligned to the cursor with a little offset so it looks okay. %>
            position[1] -= (popup.offsetHeight + offsetAbove);
        }
        else {
            <% // We cannot place the pop up above the cursor by the required distance as
               // the top of the pop up would be above the visible area.
               // There is no ideal placement situation here so we'll simply place the top
               // of the pop up at the top of the screen. %>
            position[1] = topmostVisible;
        }
    }
    else {
        <% // The pop up can sit between the cursor position and the bottom of
           // screen okay. %>
        position[1] += offsetBelow;
    }

    return position;
}

<%
// Hides the popup associated with the given element.
//
// associatedId - the id of the element the popup is associated with
%>
function hide_popup_help(associatedId)
{
    clearPopupTimer(associatedId); // this function can be a timer callback, so clear any timer

    var popupId = getPopupId(associatedId);
    var popup = document.getElementById(popupId);

    if( popup != null )
    {
        removeIframeLayer(popup);
        popup.style.left = '-999px';
        popup.style.top = '-999px';
        popup.savedCursorPosition = null;
    }
}

<%
// Some browsers, such as IE5.5, always show input fields on top of all the other
// elements.
//
// So for example, any input elements on the page would
// appear in front of a popup, regardless of the z-ordering. This is not what
// we want.
//
// This function fixes this problem, by displaying an iFrame exactly behind the
// popup. The iFrame magically obscures the underlying page.
%>
function createIframeLayer(popup)
{
    <%
    // We may have already created an iFrame layer for this popup;
    // this can happen eg if the popup is being display because we moused
    // over the associated element, then we moved the mouse over the popup itself.
    // In such a circumstance, the popup would never have been hidden, and thus
    // the corresponding iFrame layer wouldn't have been destroyed.
    %>
    var layer = popup.iframeLayer;

    if(layer==null)
    {
        <% // create one %>
        layer = document.createElement('iframe');
        layer.className = "hiddenFrameLayer"; <% // this style makes it display correctly %>
        layer.tabIndex = '-1';
        layer.src = 'javascript:false;';
        popup.parentNode.appendChild(layer);

        <% // squirrel it away %>
        popup.iframeLayer = layer;
    }

    <% // position it exactly behind the popup %>
    layer.style.left = popup.offsetLeft + 'px';
    layer.style.top = popup.offsetTop + 'px';
    layer.style.width = popup.offsetWidth + 'px';
    layer.style.height = popup.offsetHeight + 'px';
}

<%
// See createIframeLayer.
%>
function removeIframeLayer(popup)
{
    var layer = popup.iframeLayer;

    if(layer != null )
    {
        layer.parentNode.removeChild(layer);
        popup.iframeLayer = null;
    }
}

<%
// The functions:
//
// setPopupWanted, isPopupWanted
//
// are used to mark an element to indicate whether we want to be showing its associated popup.
// This is useful for when there is a delay between the action triggering the popup
// (eg a mouseover) and the showing of the popup (such as could happen if the popup
// is showed after a delay, or if it takes a while to download the popup content using
// AJAX).
//
%>

<%
// This code could be simplified eg only need one callback for showing/hiding
// a popup because we set this "popupWanted" variable.
//
%>

function setPopupWanted(associatedId, timer, wanted)
{
    associated = document.getElementById(associatedId);
    associated.popupWanted = wanted;
    <% // Kill any existing timer %>
    clearPopupTimer(associatedId);

    <% // Store the new timer, if any %>
    associated.popupTimer = timer;
}

function isPopupWanted(associatedId)
{
    associated = document.getElementById(associatedId);
    return associated.popupWanted;
}

function clearPopupTimer(associatedId)
{
    associated = document.getElementById(associatedId);
    if(associated.popupTimer != null)
    {
        window.clearTimeout(associated.popupTimer);
        associated.popupTimer = null;
    }
}

function setShowingPopup(associatedId, showing) {
    associated = document.getElementById(associatedId);
    associated.showingPopup = showing;
}

function isShowingPopup(associatedId) {
    associated = document.getElementById(associatedId);
    return associated.showingPopup;
}

<%
// ---------------------------------------------------------------------------------------------------------------
// HELPER FUNCTIONS TO SET UP BEHAVIOUR
// ---------------------------------------------------------------------------------------------------------------
%>

<%
// Sets up the behaviour for an inline help element.
%>
function setup_inline_help(eltId)
{
    var associated = document.getElementById(eltId);

    associated.hasPopup = true;

    associated.onmouseover=function()
    {
        wi_popup_show_delayed(eltId);
    }

    associated.onmouseout=function()
    {
        wi_popup_hide_delayed(eltId);
    }

    attachEventHandler(associated, "mousemove", record_cursor_position, false);

    associated.onclick=function()
    {
        wi_popup_show(eltId);

        return false; <% // no further click processing %>
    }

    associated.onblur=function()    <% // ensure popup is hidden when using keyboard navigation %>
    {
        wi_popup_hide_delayed(eltId);
    }

    var popup = document.getElementById(getPopupId(eltId));
    if(popup) {
        <% // Keep the popup visible if the mouse moves from the help link to over the popup.
           // Without this, and on a small screen, the popup can appear over the help link and cause itself to disappear %>
        popup.onmouseover = popup.onclick = function() { setPopupWanted(eltId, null, true); }
        popup.onmouseout = function() { wi_popup_hide_delayed(eltId); }

        <% // For pages using the horizon layout, the "mainPane" div uses position:relative. To prevent popups on these pages
           // from being positioned relative to the mainPane and being contained (and clipped) by the main pane, they are
           // relocated to within the "horizonTop" element, which is outside the "mainPane" div. %>
        var horizonTop = document.getElementById('horizonTop');
        if (horizonTop) {
            horizonTop.appendChild(popup);
        }
    }
}

<%
// Sets up the behaviour for a dropdown menu element.
%>
function setup_drop_down_menu(eltId)
{
    var associated = document.getElementById(eltId);
    var popupId = getPopupId(eltId);
    var popup = document.getElementById(popupId);

    associated.hasPopup = true;

    // reduce the delay to 100ms from the default of 500ms
    var delay = 100;
    popup.onmouseover=function()
    {
        wi_popup_show_delayed(eltId, delay);
    }

    popup.onmouseout=function()
    {
        wi_popup_hide_delayed(eltId, delay);
    }

    associated.onmouseover=function()
    {
        wi_popup_show_delayed(eltId, delay);
    }

    associated.onmouseout=function()
    {
        wi_popup_hide_delayed(eltId, delay);
    }

    var show = function() { wi_popup_show(eltId); };
    var hide = function() { wi_popup_hide(eltId); };

    // ensure keyboard navigation opens the menu
    var popupLinks = popup.getElementsByTagName("a");
    for(var i=0;i<popupLinks.length;i++) {
        popupLinks[i].onfocus = show;
        popupLinks[i].onblur = hide;
    };
}

function setup_custom_menu(eltId)
{
    var associated = document.getElementById(eltId);
    var popupId = getPopupId(eltId);
    var popup = document.getElementById(popupId);

    associated.hasPopup = true;

    associated.onclick = function(e) { 
        associated.popupWanted ? wi_popup_hide(eltId) : setShowingPopup(eltId, true); wi_popup_show(eltId);
        return false;
    };

    // attach an event handler to the document to dismiss the menu
    attachEventHandler(document, 'click', function() { wi_popup_doc_handler(eltId); }, false);

    // ensure keyboard navigation opens the menu
    var popupLinks = popup.getElementsByTagName("a");
    for(var i = 0; i < popupLinks.length; i++) {
        popupLinks[i].onfocus = function() { wi_popup_show(eltId); };
        popupLinks[i].onblur = function() { wi_popup_hide(eltId); };
    };
}

<%
// Saves the current mouse position. This is used to position the popup when it
// is shown.
%>
function record_cursor_position(event)
{
    var target = getEventTarget(event);

    <%
    // The event's target may be a subelement of the element we
    // attached the callback to, so search the ancestors.
    %>
    while(target)
    {
        if(target.hasPopup)
        {
            break;
        }

        target = target.parentNode;
    }

    if(target)
    {
        var popup = document.getElementById(getPopupId(target.id));
        popup.savedCursorPosition = getFrameCursorPosition(event);
    }

    return true;
}

<%
// Locates all the elements with a given class, and sets up their behaviour.
//
// elementClass - class of the elements to locate
// behaviour - function which applies the behaviour
%>
function setup_behaviour_helper(elementClass, behaviour)
{
    var pageContent = document.getElementById("pageContent");
    if (pageContent)
    {
        <% // Optimize - our behaviours are only ever applied to links and list items %>
        apply_behaviour(elementClass, behaviour, pageContent.getElementsByTagName("a"));
        apply_behaviour(elementClass, behaviour, pageContent.getElementsByTagName("li"));
    }
}

function apply_behaviour(elementClass, behaviour, elements)
{
        for (var i = 0; i < elements.length; i++)
        {
            if(isMatchedAttribute(elements[i], "class", elementClass))
            {
                behaviour(elements[i].id);
            }
        }
}

<%
// ---------------------------------------------------------------------------------------------------------------
// "PUBLIC" FUNCTIONS
// ---------------------------------------------------------------------------------------------------------------
%>

<%
// Show a popup.
//
// associatedId - the id of the element the popup is associated with
%>
function wi_popup_show(associatedId)
{
    wi_popup_show_delayed(associatedId, 0);
}

<%
// Show a popup after a delay
//
// associatedId - the id of the element the popup is associated with
// delay - delay in millis before the popup is shown
%>
function wi_popup_show_delayed(associatedId, delay)
{
    if(delay==null)
    {
        delay = getDefaultPopupShowDelay();
    }

    if( delay > 0 )
    {
        setPopupWanted(
            associatedId,
            window.setTimeout("show_popup_helper('" + associatedId + "');", delay),
            true
            );
    }
    else
    {
        setPopupWanted(associatedId, null, true);
        show_popup_helper(associatedId);
    }
}

<%
// Hides the popup.
//
// associatedId - the id of the element the popup is associated with
%>
function wi_popup_hide(associatedId)
{
    wi_popup_hide_delayed(associatedId, 0);
}

<%
// Hides the popup.
//
// If the hide is delayed and another event is triggered which would cause the
// popup to be displayed before the hide happens, then the hide is cancelled.
//
// This could be useful, for example, if you want the popup to stay displayed
// when the mouse is over it, even if the mouse leaves the popup for a shortwhile.
//
// associatedId - the id of the element the popup is associated with
// delay - the hide operation is delayed by the specified millisecond delay
%>
function wi_popup_hide_delayed(associatedId, delay)
{
    if(delay==null)
    {
        delay = getDefaultPopupHideDelay();
    }

    if( delay > 0 )
    {
        setPopupWanted(
            associatedId,
            window.setTimeout("hide_popup_help('" + associatedId + "');", delay),
            false
            );
    }
    else
    {
        setPopupWanted(associatedId, null, false);
        hide_popup_help(associatedId);
    }
}

<%
// Handles the click event on the document object, to ensure that click-to-open popups are dismissed appropriately.
%>
function wi_popup_doc_handler(associatedId) {
  // The onclick handler for a button menu displays the corresponding popup menu. However, after that function runs,
  // this onclick handler registered on the document (to hide the popup) also runs, due to event propagation.
  // Therefore, when intially showing the menu, the showingPopup field is set on the button element to indicate that
  // its menu should not be hidden at this point, to prevent it from disappearing as soon as it is shown.
  if (!isShowingPopup(associatedId)) {
    wi_popup_hide(associatedId);
  } else {
    setShowingPopup(associatedId, false);
  }
}

<%
// Goes throught the page, attach events to appropriate elements.
%>
function setup_popup_behaviour()
{
<% if(!Include.isCompactLayout(wiContext)) { %>
    setup_behaviour_helper("inlineHelpLink", setup_inline_help); <% // inline help %>
    setup_behaviour_helper("DropDownMenu", setup_drop_down_menu); <% // Workspace control dropdown menu %>
    setup_behaviour_helper("CustomMenu", setup_custom_menu); <% // Select view options button %>
<% } %>
}

function wizard_setup_popup_behaviour()
{
    setup_behaviour_helper("inlineHelpLink", setup_inline_help); <% // inline help %>
}
