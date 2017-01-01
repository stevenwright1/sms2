<%
// feedback.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

var alertID = null, alerted = false, updateAlertID = null, updateAlert = null;

/* public */
function setSessionTimeout() {
<%
if (wiContext.getClientInfo().osPocketPC()) {
%>
    alertID = window.setTimeout("timeoutExpired()", <%=feedbackControl.getWarningTimeoutMins() + feedbackControl.getAdvanceWarningMins() * 60000%>);
<%
} else {
%>
    alertID = window.setTimeout("startTimeoutAlert()", <%=feedbackControl.getWarningTimeoutMins()*60000%>);
<%
}
%>
}

/* private */
function startTimeoutAlert() {
    updateAlert = <%=feedbackControl.getAdvanceWarningMins()%>;

    timeoutAlert();

    // update warning message every minute
    updateAlertID = window.setInterval("timeoutAlert()", <%=feedbackControl.getAlertIntervalMins()*60000%>);
}

/* private */
function timeoutAlert() {
    alertID = null;
    alerted = true;

    var feedback = document.getElementById("feedbackArea");

    if (updateAlert > 0) {
        addAlertMsg(feedback);
        alertPopup();
        updateAlert -= <%=feedbackControl.getAlertIntervalMins()%>;
    }

    // timeout expired
    else {
        timeoutExpired();
    }
}

/* private */
function addAlertMsg(feedback) {
<%
if (feedbackControl.getShowTimeoutWarning()) {
%>
    if (feedback != null) {
        removeAlertMsg(feedback);
        var minsLeft = Math.round(updateAlert);
        var msg;
        if(minsLeft==1) {
            msg = "<%=wiContext.getString("TimeoutAlertSingular")%>";
        } else {
            msg = "<%=wiContext.getString("TimeoutAlert", "REPLACETIME")%>";
            msg = msg.replace("REPLACETIME", minsLeft + "");
        }

        msg += " <%=wiContext.getString("TimeoutAlertMoreTime", "'" + Constants.PAGE_TIMEOUT_TRIGGER + "' target='" + Constants.ID_FRAME_TIMEOUT + "FRAMESUFFIX" + "' onClick='resetSessionTimeout()' ")%>";
        msg = msg.replace("FRAMESUFFIX", getFrameSuffix() + "");
        feedback.innerHTML = "<p class=\"feedbackStyleWarning\">" + msg + "</p>";
        feedback.className="";

        <%
        // We change the size of the page, so we need to up update the layout.
        // We only call the updateLayout function in full mode as compact mode does not
        // have to recalculate dimensions for the page and refresh the layout.
        // The updateLayout function from layout.js is not included in compact mode.
        %>
        <% if(!Include.isCompactLayout(wiContext)) { %>
        updateLayout();
        <% } %>
    }
<%
}
%>
}

/* private */
function removeAlertMsg(feedback) {
    clearFeedback();
}

/* private */
function alertPopup(){
    if (parent.windowHandles != null) {
        var windows = parent.windowHandles;
        for (var i in windows) {
            var winRef;

            // Get the pop-up window handle from the parent
            winRef = windows[i];

            if (winRef && winRef.open && !winRef.closed) {
                var feedback = winRef.document.getElementById("feedbackArea");
                addAlertMsg(feedback);
            }
        }
    }
}

/* private */
function timeoutExpired() {

    if(updateAlertID != null) {
        window.clearInterval(updateAlertID);
    }

    // close popup windows
    if (parent.windowHandles != null) {
            var windows = parent.windowHandles;
            for (var i in windows) {
               var winRef;
               // Get the pop-up window handle from the parent
               winRef = windows[i];
               if (winRef && winRef.open && !winRef.closed) {
                   winRef.close();
               }
            }
            parent.windowHandles = null;
        }

    // Redirect the main WI page to logout.aspx under the current page's folder.
    var path = getLogoutPage() +
        "?<%=Constants.QSTR_TIMEOUT%>=<%=Constants.VAL_ON%>&<%=SessionToken.QSTR_TOKENNAME%>=" + getSessionToken();

    this.location.href = path;
}

// This is overwritten in the wizard master page
// so it goes to the correct logout page
/* public */
function getLogoutPage() {
    return "<%=Constants.PAGE_LOGOUT%>";
}

function shouldUseOpener(w) {
    var useOpener = false;

    try {
        // If a window and its opener have different origins (different host, port or protocol URL components)
        // Firefox throws a permission denied error when attempting to access properties on the opener.
        // This situation can arise if a customer creates an HTML page in one domain containing a link
        // to a WI site in a different domain.
        if (w.opener && w.opener.resetSessionTimeout) {
            useOpener = true;
        }
    } catch (e) {}

    return useOpener;
}

/* private */
function clearSessionTimeout() {
    if (shouldUseOpener(window)) {
        window.opener.resetSessionTimeout();
    }
    else {
        if (alerted) {
            var feedback = document.getElementById("feedbackArea");
            removeAlertMsg(feedback);

            // check if there is any popup
            if (parent.windowHandles != null) {
                var windows = parent.windowHandles;
                for (var i in windows) {
                    var winRef;

                    // Get the pop-up window handle from the parent
                    winRef = windows[i];

                    if (winRef && winRef.open && !winRef.closed) {
                        var feedback = winRef.document.getElementById("feedbackArea");
                        removeAlertMsg(feedback);
                    }
                }
            }
            alerted = false;
        }

        if (alertID != null) {
            window.clearTimeout(alertID);
        }

        if (updateAlertID != null) {
            window.clearInterval(updateAlertID);
        }
    }
}

/* public */
function resetSessionTimeout() {
    if (shouldUseOpener(window)) {
        window.opener.resetSessionTimeout();
    }
    else {
        clearSessionTimeout();
        setSessionTimeout();
    }
}

/* public */
function setFeedback(msg, severity, doc) {
    var feedback;

    if (doc) {
        feedback = doc.getElementById("feedbackArea");
    } else {
        feedback = document.getElementById("feedbackArea");
    }

    if (feedback) {
        if (msg) {
            feedback.innerHTML = "<p class=\"feedbackStyle" + severity + "\">" + msg + "</p>";
            feedback.className="";
        } else {
            feedback.innerHTML="";
            <%
            // On IE the empty div may take up screen space even with no content, so it is hidden.
            %>
            feedback.className="noFeedback";
        }

        <%
        // The insertion/removal of a feedback message will typically resize the page,
        // so updateLayout() needs to be called when in full graphics mode.
        %>
        <% if(!Include.isCompactLayout(wiContext)) { %>
        var main = findMainFrame(window);
        if (main) {
            main.updateLayout();
        }
        <% } %>
    }
}

<%
// Clear the feedback area.
%>

function clearFeedback(doc) {
    setFeedback(null, null, doc);
}
