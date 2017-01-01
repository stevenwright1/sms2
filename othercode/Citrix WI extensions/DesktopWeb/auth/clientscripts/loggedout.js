// loggedout.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.

function onLoadLayout() {
    var frame = getTopFrame(window);
    if (frame != null) {
        frame.location.href = window.location.href;
        return false;
    }

    return;
}
