<%
// displaySettings.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function applyLanguage(settingsForm) {
    settingsForm.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.MODE_APPLY%>';
    settingsForm.submit();
}