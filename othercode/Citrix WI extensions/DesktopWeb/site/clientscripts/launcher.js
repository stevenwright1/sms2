<%
// launcher.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

// Check if we are in the main frame in WI (i.e. not hidden frames)
// If so, redirect to a hidden one so that we can still see the WI default page
<%
String noParentWindowRedirectUrl = (viewControl.isPassthroughEnabled)? Include.getHomePage(wiContext) : viewControl.passthroughErrorUrl;
String strIsRedirectUrlNull = (viewControl.redirectUrl == null)? "y": "n";
%>
function checkFrameName() {
    if ("<%=strIsRedirectUrlNull%>" == "y")
    {
        if (findMainFrame(window) == null) {
            redirectToMainFrame("<%= noParentWindowRedirectUrl%>")
        } else {
          <%= viewControl.launchTag %>
        }
    } else
    {
        <% /* redirect the main frame to the given url (i.e. an error has occured) */ %>
        redirectToMainFrame("<%= viewControl.redirectUrl %>");
    }
}