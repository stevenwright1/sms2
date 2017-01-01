<%
// nativeClientDownloaded.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

<%
String downloadUrl = WizardUtil.getClientDownloadUrl(wizardContext);
%>

function onLoadLayout(){
    startDownload();
}

function startDownload(){
<% if(!sClientInfo.isIE() && !Strings.isEmpty(downloadUrl)) { %>
    downloadClient('<%=downloadUrl%>');
<% } %>
    pollForClient();
}

var blockPollingForClient = false;
function pollForClient() {
  // Don't keep polling if we've determined that the platform doesn't support it
  if (blockPollingForClient) { return; }
  // trigger a reload of the pollingClientDetectionPage
  frames['pollingDetectionFrame'].location = '<%= WizardConstants.PAGE_NATIVE_POLLING %>';
  // set next poll time
  setTimeout("pollForClient();", <%= WizardConstants.CLIENT_DETECTION_POLLING_PERIOD %>);
}

// Function which embedded javascript (in iframe) can use to inform this
// page of client detection results.
function clientDetailsCallback(clientDetails) {

  // stop if we don't have a valid clientDetails object
  if (!clientDetails) { return false; }

  // If we have determined that browser doesn't support polling
  // then stop polling loop and exit
  if (!clientDetails.pollingSupported) {
    blockPollingForClient = true;
    return false;
  }

  // If polling is supported, but doesn't require
  // this page to reload the iframe then stop the polling
  // loop in this page, but don't exit as this method will
  // be invoked from the iframe
  if (!clientDetails.reloadRequired) {
    blockPollingForClient = true;
  }

  // If we detect an up-to-date client then success

  // In the following commented line, the `success' variable evaluates to `true' in IE9 beta even when
  // `alert(clientDetails.detected)' indicates one of the booleans is false:
  // var success = clientDetails.detected && isRequiredClientVersion(clientDetails.version);
  //
  // Reversing the order of the booleans and also explictly checking for `success == true'
  // (rather than just `success') seems to work around the problem. It appears this issue may be related
  // to the fact that this function is called from an iframe:
  // http://blog.j15r.com/2010/09/ugly-bug-in-ie9-beta.html
  var success = isRequiredClientVersion(clientDetails.version) && clientDetails.detected;
  if (success == true) {
    // click the success button
    var successButton = document.getElementById('success');
    successButton && successButton.click();
  }
  return success;
}

function isRequiredClientVersion(detectedVersion) {
    var serverVersion = '<%= WizardUtil.getServerClientVersion(wizardContext) %>';
    var upgrading = getWizardCookieItem('<%=WizardConstants.NATIVE%>') == '<%=WizardConstants.UPGRADEABLE%>';

    if (!serverVersion || !upgrading) { // No requirement for a particular version
        return true;
    } else if (!detectedVersion){ // No version detected
        return false;
    }

    return isUpToDateVersion(detectedVersion, serverVersion);    // see commonDetection.js
}

