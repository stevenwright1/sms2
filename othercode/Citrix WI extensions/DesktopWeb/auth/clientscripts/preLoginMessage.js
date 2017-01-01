<%
// preLoginMessage.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    // Focus on the yes button so clicking enter presses the button
    var citrixForm = document.forms[0]; // only one form per page
    var button = citrixForm.elements['<%=Constants.VAL_YES%>']; // name of the button is yes
    button.focus();
}
