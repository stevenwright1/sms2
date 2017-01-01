<%
// environmentDetection.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>


/*
 * Detect information about the client environment
 */
function setClientAndEnvironmentCookies(url) {
    if (isCookieEnabled()) {
        detectEnvironment();
        var newloc = new String(((url == null) || (url == "")) ? location : url);
        location.replace(newloc);
    } else {
        location.replace("<%=Constants.PAGE_NOCOOKIES%>");
    }
}
//===============================
// Internal functions
//===============================

// Check whether the browser accepts cookies by writing a cookie, reading
// it back and checking if it is the same.
function isCookieEnabled() {
    var testName = "<%=Constants.COOKIE_TEST_COOKIE%>";
    var testValue = "<%=Constants.COOKIE_TEST_COOKIE_VALUE%>";
    setItemInCookie(testName, testValue);
    var value = getItemFromCookie(testName);
    return ((value != null) && (value == testValue));
}

// Detect information about the client environment
function detectEnvironment() {
    // Get screen resolution for embedded apps as a percentage of screen size
    var ScreenWidth, ScreenHeight, strResolution;
    ScreenWidth = window.screen.width;
    ScreenHeight = window.screen.height;
    strResolution = ScreenWidth + 'x' + ScreenHeight;
    setItemInCookie("<%=Constants.COOKIE_ICA_SCREEN_RESOLUTION%>", strResolution);

    // Duplicate of inline code from login.js. This value is used server-side at the
    // same point screen resolution (above) is used, and this may be earlier than
    // login.js.
    var isSecure = (location.protocol.toLowerCase() == 'https:');
    setItemInCookie("<%=Constants.COOKIE_CLIENT_CONN_SECURE%>", isSecure);
}
