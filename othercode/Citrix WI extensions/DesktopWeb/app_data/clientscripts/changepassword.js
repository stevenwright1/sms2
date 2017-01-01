// changepassword.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    self.focus();
    var f = document.forms[0];
    if (f.password){
        f.password.focus();
    }else if (f.submit1){
        f.submit1.focus();
    }
}
