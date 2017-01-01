<%
// cookies.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
/**
 * Puts an item of data into the Web Interface client information cookie.
 *
 * The name and value of the item are escaped.
 *
 * @param name the name of the item
 * @param value the value of the item
 */
%>
function setItemInCookie(name, value) {
    if (value == null) {
        value = "";
    }
    if ((name == null) || (name == "")) {
        return;
    }

    var newCookie = "";
    var oldCookie = getCookie("<%=Constants.COOKIE_CLIENT_INFO%>");
    if (oldCookie != "") {
        var cookieItems = oldCookie.split("<%=Cookies.NVPAIR_DIVIDER%>");
        for (i=0; i < cookieItems.length; i++) {
            // The name of the item will be escaped so we need to make sure
            // that we search for the escaped version.
            if (cookieItems[i].indexOf(escape(name) + "<%=Cookies.NAME_VALUE_DIVIDER%>") != 0) {
                newCookie += cookieItems[i] + "<%=Cookies.NVPAIR_DIVIDER%>";
            }
        }
    }

    newCookie += escape(name) + "<%=Cookies.NAME_VALUE_DIVIDER%>" + escape(value);
    storeCookie("<%=Constants.COOKIE_CLIENT_INFO%>", newCookie);
}

<%
/**
 * Gets an item of data from the Web Interface client information cookie.
 *
 * The value of the item is returned unescaped.
 *
 * @param name the name of the item
 * @return the value of the item
 */
%>
function getItemFromCookie(name) {
    return unescape(getValueFromString(escape(name), getCookie("<%=Constants.COOKIE_CLIENT_INFO%>"), "<%=Cookies.NAME_VALUE_DIVIDER%>", "<%=Cookies.NVPAIR_DIVIDER%>"));
}

<%
/**
 * Stores a cookie on the client.
 *
 * The caller is responsible for encoding the cookie contents as appropriate.
 * Any non-empty value is enclosed in quotes to comply with RFCs 2109 & 2965.
 * For pages retrieved via https, the "secure" cookie attribute is added.
 *
 * @param name the name of the cookie
 * @param value the value of the cookie
 */
%>
function storeCookie(name, value) {
    if (value) { // non-null, non-empty
        value = "\"" + value + "\"";
    } else {
        value = "";
    }

    if (window.location.protocol.toLowerCase() == "https:") {
        value += "; secure";
    }

    var cookie = name + "=" + value;
<%  // The cookie path appears to cause problems for the way NetScaler proxies cookies.
    // We do not use path with AG / NetScaler to improve compatibility with them.
    if(!AGEUtilities.isAGEIntegrationEnabled(wiContext.getConfiguration())) {
%>
    cookie = cookie + "; path=<%=wiContext.getWebAbstraction().getAbsoluteRequestDirectory()%>";
<% } %>
    document.cookie = cookie;
}

<%
/**
 * Gets a cookie from the client.
 *
 * The caller is responsible for decoding the cookie contents as appropriate.
 * This function will strip off enclosing quotes from the cookie value if found.
 *
 * @param name the name of the cookie
 * @return the value of the cookie
 */
%>
function getCookie(name) {
    var cookie = getValueFromString(name, document.cookie, "=", ";");
    if ( (cookie.charAt(0) == "\"") && (cookie.charAt(cookie.length-1) == "\"") ) {
        cookie = cookie.substring(1, cookie.length-1);
    }
    return cookie;
}

function getValueFromString(name, str, sep1, sep2) {
    var result = "";

    if (str != null) {
        var itemStart = str.indexOf(name + sep1);
        if (itemStart != -1) {
            var valueStart = itemStart + name.length + 1;
            var valueEnd = str.indexOf(sep2, valueStart);
            if (valueEnd == -1) {
                valueEnd = str.length;
            }
            result = str.substring(valueStart, valueEnd);
        }
    }

    return result;
}
