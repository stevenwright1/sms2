// euem.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.

// Adds the current client UTC time to the URL that is passed in,
// then sets the specified window location to be the new URL.
function addCurrentTimeToWindowLocation(win, url, tParamName) {
    addTimeToWindowLocation(win, url, tParamName, getTimeInMillis());
}

// Adds the specified time to the URL that is passed in, then
// sets the specified window location to be the new URL.
function addTimeToWindowLocation(win, url, tParamName, time) {
    if (win != null) {
       win.location = addTimeToURL(url, tParamName, time);
   }
}

// Adds the page's client UTC load time to the href
// of the specified HTML anchor.
//
// This is the time when the Javascript is first executed
// in the browser.
function addPageLoadTimeToHref(anchor, url, tParamName) {
    addTimeToHref(anchor, url, tParamName, pageLoadTime);
    return;
}

// Adds the current client UTC time to the href
// of a given HTML anchor.
//
// This is primarily used to add click times to URLs
// during local client launches.
function addCurrentTimeToHref(anchor, url, tParamName) {
    addTimeToHref(anchor, url, tParamName, getTimeInMillis());
    return;
}

// Adds a specific time to the href of a given
// HTML anchor.
function addTimeToHref(anchor, url, tParamName, time) {
    anchor.href = addTimeToURL(url, tParamName, time);
    return;
}

// Adds the current page's client UTC load time to the
// specified URL.
//
// This is the time when the Javascript is first executed
// in the browser.
function addPageLoadTimeToURL(currentUrl, tParamName) {
    return addTimeToURL(currentUrl, tParamName, pageLoadTime);
}

// Adds the current client UTC time to the URL.
//
// This is primarily used to add click times to
// URLs during embedded launches.
function addCurrentTimeToURL(currentUrl, tParamName) {
    return addTimeToURL(currentUrl, tParamName, getTimeInMillis());
}

// Adds a specific time to a URL's query string
// under the given parameter name.
//
// This method will first remove all references
// to the given parameter (no matter how many
// there are).
function addTimeToURL(url, tParamName, time) {

    var separator = "?";

    if (url != null && tParamName != null && tParamName != "" && time != -1) {
        // Strip out any existing timings under the param name
        // in the query string
        while(url.indexOf("?") != -1 && url.indexOf(tParamName) != -1
                && url.indexOf(tParamName) > url.indexOf("?")) {
            var tParamStart = url.indexOf(tParamName);
            var tParamEnd = (url.indexOf("&", tParamStart) != -1) ? url.indexOf("&", tParamStart) : url.length;

            // Timing is not the last query string value
            if(tParamEnd != url.length) {
                // Strip off trailing "&";
                tParamEnd = tParamEnd + 1;
                // Timing is the only query string value
                if (tParamStart == url.indexOf("?") + 1) {
                    // Strip off leading "?"
                    tParamStart = tParamStart - 1;
                }
                // Timing is the last query string value
            } else {
                // Strip off leading "?" or "&"
                tParamStart = tParamStart - 1;
            }

            url = url.substring(0, tParamStart) + url.substring(tParamEnd, url.length);
        }

        // Append new timing to clean query string
        if(url.indexOf("?") != -1) {
            separator = "&";
        }
        url += separator + tParamName + "=" + time;
    }// else error
    return url;
}

// Get the time the current page loaded.
//
// This is the client side time when the current
// page started executing.
function getPageLoadTime() {
    return pageLoadTime;
}

// Gets the current client UTC time.
function getTimeInMillis() {
    return (new Date()).valueOf();
}

var pageLoadTime = getTimeInMillis();
