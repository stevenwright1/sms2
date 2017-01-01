// the preCertificate page uses login screen layout, thus
// must have those functions defined to prevent js errors
// if any of those functions is already defined, the implementation
// will be overriden

function doAutoLaunching() {}
function setup_popup_behaviour() {}
function setup_login_submit_keys() {}

var req = newAjaxRequest();
var nextAuthPage = "<%=Constants.PAGE_CERTIFICATE%>";


// If user uses smartcard with IE and presses cancel on PIN prompt,
// IE shows "page cannot be found" error.
// To work around it, the certificate authorization page is first accessed
// within AJAX call. If there is no response from the server, that means the authentication
// error has occurred - redirect to the certificate error page to handle it.
function goToNextPage()
{
    // Request is completed.
    // Note that response to the original is ignored - we only look if it returned anything.
    if (req.readyState == 4)
    {
        // If the server did not send a response it means that the smartcard authentication has failed
        // with a "page cannot be found" error.
        if (req.getAllResponseHeaders().length == 0)
        {
            document.location = "<%=Constants.PAGE_CERTIFICATE_ERROR%>";
        } else
        {
            // if there was a response from the server redirect browser to it
            // this should happen always but when cancelling PIN prompt
            document.location = nextAuthPage;
        }
    }
}

function showEnterPINMessage()
{
    var msgTitle = document.getElementById('welcomeTitle');
    var msgBody  = document.getElementById('PINMessage');

    msgTitle.innerHTML = '<%=WebUtilities.escapeJavascript(wiContext.getString("PINRequired"))%>';
    msgBody.innerHTML = '<%=WebUtilities.escapeJavascript(wiContext.getString("EnterPINMessage"))%>';
}

function onLoadLayout()
{
    showEnterPINMessage();

    // create XML request to next authentication page
    if (req != null)
    {
        req.onreadystatechange = goToNextPage;
        req.open("GET", nextAuthPage+"?<%=Constants.QSTR_PAGE_ACCESS_ONLY%>=<%=Constants.VAL_YES%>", true);
        req.send(null);
    }
    else
    {
        document.location = nextAuthPage;
    }
}
