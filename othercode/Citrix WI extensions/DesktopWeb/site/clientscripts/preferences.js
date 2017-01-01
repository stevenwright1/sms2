// preferences.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.

function onLoadLayout() {
    var setForm = document.forms[0];
    if (setForm) {
        reconnect_clicked(setForm.chkReconnectAtLogin, setForm.slReconnectLogin);
        reconnect_clicked(setForm.chkReconnectButton, setForm.slReconnectButton);
    }
    
    click_standard_size(document.<%=Constants.ID_CITRIX_FORM%>);
    onChangeBandwidth(document.<%=Constants.ID_CITRIX_FORM%>);
    
    return;
}
