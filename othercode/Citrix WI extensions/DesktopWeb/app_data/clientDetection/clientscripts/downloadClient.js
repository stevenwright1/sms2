<%
// downloadClient.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

<%
// This performs the download of the client
%>
function downloadClient(downloadUrl) {
    <% if (sClientInfo.isIE()) { %>
        // Download the msi file using a popup in IE to workaround the info bar problem
        window.open(downloadUrl, 'Download', 'toolbar=0,status=0,scrollbars=no,resizable=0,width=1,height=1,top=0,left=0');
    <% } else if (sClientInfo.isSafari()) { %>
        // In Safari need to use a timeout to workaround a problem with downloading
        // and redirecting at the same time
        setTimeout("window.location='" + downloadUrl + "'", 100);
    <% } else { %>
        location.href = downloadUrl;
    <% } %>
}

<%
// Open a window and it should point to the
// configured client download site
%>
function openClientDownloadSite() {
    window.open('<%=WizardUtil.getClientDeploymentConfiguration(wizardContext).getClientDefaultUrl()%>');
}

<%
// A function to download the client straight away
// used by the problem downloading links
%>
function downloadClientNow(downloadUrl) {
    if (!downloadUrl) {
        openClientDownloadSite();
    } else {
        downloadClient(downloadUrl);
    }
}