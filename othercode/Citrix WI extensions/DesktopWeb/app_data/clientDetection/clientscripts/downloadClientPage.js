<%
// downloadClientPage.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

<% DownloadViewModel viewModel = (DownloadViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>

function onLoadLayout() {
    <% // Properly display the download button if high contrast mode is enabled. %>
    maintainAccessibility("DownloadButton");
}

function startDownload() {
<%
// If there is no file download, go to the client download site
// If we have a file, IE download it now, others download on
// the downloaded page
if (Strings.isEmpty(viewModel.downloadUrl)) { %>
    openClientDownloadSite()
<% } else if(sClientInfo.isIE()) { %>
    downloadClient('<%=viewModel.downloadUrl%>');
<% } %>
    location.href = '<%=viewModel.downloadedUrl%>';
}
