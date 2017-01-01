<%
// login.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function clearForm(loginForm) {
    loginForm.<%=Constants.ID_USER%>.value = "";
    loginForm.<%=Constants.ID_PASSWORD%>.value = "";
    if (loginForm.<%=Constants.ID_DOMAIN%>) {
        if (loginForm.<%=Constants.ID_DOMAIN%>.type != "hidden") {
            loginForm.<%=Constants.ID_DOMAIN%>.value = "";
        }
    }
    if (loginForm.<%=Constants.ID_CONTEXT%> != null) {
        loginForm.<%=Constants.ID_CONTEXT%>.value = "";
    }
    setFocus(loginForm);
}

function setFocus(loginForm) {
    if (loginForm.<%=TwoFactorAuth.ID_TOKENCODE%> && (! loginForm.<%=TwoFactorAuth.ID_TOKENCODE%>.disabled)) {
        loginForm.<%=TwoFactorAuth.ID_TOKENCODE%>.focus();
    } else if (loginForm.<%=TwoFactorAuth.ID_PIN1%> && (! loginForm.<%=TwoFactorAuth.ID_PIN1%>.disabled)) {
        loginForm.<%=TwoFactorAuth.ID_PIN1%>.focus();
    } else if (loginForm.<%=Constants.ID_LOGIN_TYPE%> && (! loginForm.<%=Constants.ID_LOGIN_TYPE%>.disabled)) {
        if (loginForm.<%=Constants.ID_LOGIN_TYPE%>.value == "<%=WIAuthType.EXPLICIT%>") {
            setExplicitLoginFocus(loginForm);
        } else {
            if (loginForm.<%=Constants.ID_LOGIN_TYPE%>.options && (! loginForm.<%=Constants.ID_LOGIN_TYPE%>.options.disabled)) {
                if (loginForm.<%=Constants.ID_LOGIN_TYPE%>.options[loginForm.<%=Constants.ID_LOGIN_TYPE%>.selectedIndex].value  == "<%=WIAuthType.EXPLICIT%>") {
                    setExplicitLoginFocus(loginForm);
                } else {
                    var usrAgt = navigator.userAgent.toLowerCase();
                    var nav4 = ((usrAgt.indexOf('mozilla/4') != -1)
                                && (usrAgt.indexOf('msie') == -1)
                                && (usrAgt.indexOf('spoofer') == -1)
                                && (usrAgt.indexOf('compatible') == -1)
                                && (usrAgt.indexOf('opera') == -1)
                                && (usrAgt.indexOf('webtv') == -1));

                    if (!nav4) {
                        var buttonName;
                        if (isHighContrastEnabled() ) {
                            buttonName = "highContrast_LoginButton";
                        } else {
                            buttonName = "<%=Constants.ID_LOGIN_BTN%>";
                        }
                        if (!document.getElementById(buttonName).disabled) {
                            document.getElementById(buttonName).focus();
                        }
                    }
                }
            }
        }
    }
}

function setExplicitLoginFocus(loginForm) {
<%
if(viewControl.getFirstInvalidField()==null) {
%>

    if (loginForm.<%=Constants.ID_USER%>.value != "") {
        loginForm.<%=Constants.ID_PASSWORD%>.focus();
    } else if (!loginForm.<%=Constants.ID_USER%>.disabled) {
        loginForm.<%=Constants.ID_USER%>.focus();
    }

<%
} else {
// a previously submitted field contains invalid data; give
// that field the focus
%>

    loginForm.<%=viewControl.getFirstInvalidField()%>.focus();

<% } %>
}

function onLoadLayout() {
    <% // Properly display the login button if high contrast mode is enabled. %>
    maintainAccessibility("LoginButton");

    var frame = getTopFrame(window);
    if (frame != null) {
        frame.location.href = "<%=Constants.PAGE_LOGGEDOUT%>";
        return false;
    }

    var form = document.forms[0];
    if (form) {
        setFocus(form);
<% // ensure that the domain is greyed out if appropriate %>
        onUsernameTextEntry(form);
    }

    var accountSSlink = document.getElementById("<%=Constants.ID_ACCOUNTSS%>");

    if (accountSSlink != null) {
        showAccountSelfServiceIfEnabled(accountSSlink);
    }
    return;
}

function usernameFieldContainsDomain(f) {
    return (f.<%=Constants.ID_USER%>.value.indexOf("@") != -1) ||
           (f.<%=Constants.ID_USER%>.value.indexOf("\\") != -1);
}

function isExplicitLoginType(f) {
    return (f.<%=Constants.ID_LOGIN_TYPE%>.value == "<%=WIAuthType.EXPLICIT%>");
}

function setDomainState(f) {
    <%
    if (! (viewControl.getRestrictDomains() && viewControl.getNumLoginDomains() == 0)) {
    %>
        var explicit = isExplicitLoginType(f);

        setDisabled(f.<%=Constants.ID_DOMAIN%>, ! explicit || usernameFieldContainsDomain(f));
    <%
        if (! wiContext.getClientInfo().osWinCE()) {
    %>
        setDisabled(document.getElementById("lblDomain"), ! explicit || usernameFieldContainsDomain(f));
    <%
        }
    %>
    <%
    }
    %>
}

function onChangeLoginType(f) {
     var explicit = isExplicitLoginType(f);

     setDisabled(f.<%=Constants.ID_USER%>, ! explicit);
     setDisabled(f.<%=Constants.ID_PASSWORD%>, ! explicit);
     setDisabled(f.<%=Constants.ID_ACCOUNTSS%>, !explicit);
     setDisabled(f.<%=Constants.ID_CONTEXT%>, ! explicit);
     setDisabled(f.<%=Constants.ID_TREE%>, ! explicit);
     setDisabled(f.<%=Constants.ID_PASSCODE%>, ! explicit);

<%
if (! wiContext.getClientInfo().osWinCE()) {
%>
     setDisabled(document.getElementById("lblCredentials"), ! explicit);
     setDisabled(document.getElementById("lblUserName"), ! explicit);
     setDisabled(document.getElementById("lblPasswd"), ! explicit);
     setDisabled(document.getElementById("lblContext"), ! explicit);
     setDisabled(document.getElementById("lblTree"), ! explicit);
     setDisabled(document.getElementById("lblPasscode"), ! explicit);
<%
}
%>

     setDomainState(f);
}

function onUsernameTextEntry(f) {
    setDomainState(f);
}

function showAccountSelfServiceIfEnabled(accountSSlink) {
    if (getItemFromCookie("<%=Constants.COOKIE_CLIENT_CONN_SECURE%>") == "true") {
        accountSSlink.style.display="";
    } else  {
        accountSSlink.style.display="none";
    }
}

function addCssClass(item, c) {
    var names = item.className.split(' ');
    for (var i in names) {
        if (names[i] == c) return;
    }
    item.className += ' ' + c;
}
function removeCssClass(item, c) {
    var names = item.className.split(' ');
    var newNames = '';
    for (var i in names) {
        if (names[i] != c) {
            newNames += ' ' + names[i];
        }
    }
    item.className = newNames;
}

function setDisabled(item, disabled) {
    if (item) {
        if (item.tagName == 'INPUT' || item.tagName == 'SELECT') {
            if (disabled) {
                addCssClass(item, 'loginEntriesDisabled');
            } else {
                removeCssClass(item, 'loginEntriesDisabled');
            }
        }
        item.disabled = disabled;
    }
}


// Disable all links in the document
function disableLinks() {
    if (document.getElementsByTagName) {
        var allAnchors = document.getElementsByTagName("a");
        for(i =0; i<allAnchors.length; i++) {
            allAnchors[i].onclick = function () { return false; };
        }
    }
}

// This variable is used to stop double form submits
// does not need to be reset as the page is refreshed
// when the login form is sent
var isSubmitted = false;

function submitForm() {
    if (!isSubmitted) {
        isSubmitted = true;
        disableLinks();
        document.forms[0].submit();
    }
}

function setup_login_submit_keys()
{
    // the form uses a link/image instead of a submit button, so it needs scripting to submit when the enter key is pressed on fields

    var submitIfEnter = function(e)  {
        var keynum;
        if(window.event) { // IE
            keynum = window.event.keyCode;
        }
        else if(e.which) { // Other browser
            keynum = e.which;
        }
        if (keynum == 13) { // enter key
            submitForm();
            return false;
        }
    }

    var inputs = document.forms[0].getElementsByTagName("input");
    for(var i=0;i<inputs.length;i++) {
        inputs[i].onkeypress = submitIfEnter;
    }

    var selects = document.forms[0].getElementsByTagName("select");
    for(var i=0;i<selects.length;i++) {
        selects[i].onkeypress = submitIfEnter;
    }
}

var isSecure = (location.protocol.toLowerCase() == 'https:');
setItemInCookie("<%=Constants.COOKIE_CLIENT_CONN_SECURE%>", isSecure);
