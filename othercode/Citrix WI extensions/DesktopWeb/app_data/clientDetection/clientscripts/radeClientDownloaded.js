// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.

<% String downloadUrl = WizardUtil.getStreamingClientDownloadUrl(wizardContext); %>

function onLoadLayout() {
    startDownload();
}

function startDownload(){
<% if(!sClientInfo.isIE() && !Strings.isEmpty(downloadUrl)) { %>
    downloadClient('<%=downloadUrl%>');
<% } %>
}
