<!--#include file="~/app_data/clientDetection/clientscripts/include.aspxf" -->

<%
// Only in interactive mode if there are no remote clients and the wizard is not in silent mode
bool isInteractive = inputs.getRemoteClients().isEmpty() && (inputs.getMode() != Mode.SILENT);

// Decide whether the page needs to be reloaded to remove the IE info bar
bool needToReloadPage = !isInteractive && sClientInfo.isIE();

string reloadAttr = (String) wizardContext.getModel().getAttribute(WizardConstants.RELOADED);
bool pageHasBeenReloaded = (reloadAttr == WizardConstants.VAL_TRUE);
%>

function onLoadLayout(){
    detectRade();
}
<!--#include file="~/app_data/clientDetection/clientscripts/cookies.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/radeClientDetection.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/processRadeDetection.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/commonDetection.js" -->

<%
if (needToReloadPage && !pageHasBeenReloaded) {
    // Set this attribute so that the next visit to the page will not cause
    // RADE detection to take place
    wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_TRUE);
} else {
    wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_FALSE);
}
%>