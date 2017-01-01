<%
// launch.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout()
{
}

<%
// try ... catch is not supported by Pocket IE and we currently
// do not support embedded launch for Pocket PC anyway.

if (! wiContext.getClientInfo().osPocketPC()) {
%>
function appEmbed(mylink, width, height)
{
    var scrollable = "no";
    if (width > window.screen.availWidth) {
        scrollable = "yes";
        width = window.screen.availWidth - 10;
    }
    if (height > window.screen.availHeight) {
        scrollable = "yes";
        height = window.screen.availHeight - 30;
    }
    var win = window.open(mylink, '_blank', 'width=' + width + ',height=' + height + ',scrollbars=' + scrollable + ',status=no,resizable=no,toolbar=no');
    if (win != null) {
        try {
            win.moveTo(window.screen.availLeft, window.screen.availTop);
        } catch (e) {
            // Ignore exception caused by this page being in a different zone to the opened window. (RDP Trusted Sites issue).
        }
    }
    return win;
}
<%
}
%>
