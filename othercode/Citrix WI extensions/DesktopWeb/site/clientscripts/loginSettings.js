// loginSettings.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.

<%
// Updates the state of the reconnect dropdown box
%>
function reconnect_clicked(checkNode, optionNode) {
    if (checkNode && optionNode) {
        optionNode.disabled = !checkNode.checked;
    }
}

function onLoadLayout() {
    var setForm = document.forms[0];
    if (setForm) {
        reconnect_clicked(setForm.chkReconnectAtLogin, setForm.slReconnectLogin);
        reconnect_clicked(setForm.chkReconnectButton, setForm.slReconnectButton);
    }
    
    return;
}

