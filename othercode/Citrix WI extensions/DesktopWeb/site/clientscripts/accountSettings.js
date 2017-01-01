<%
// accountSettings.js
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
// Updates the state of the reconnect dropdown box
%>
function reconnect_clicked(checkNode, optionNode) {
    if (checkNode && optionNode) {
        optionNode.disabled = !checkNode.checked;
    }
}