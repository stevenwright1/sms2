// passwordexpirywarn.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    self.focus();
    var f = document.forms[0];
    f.submit1.focus();
}
