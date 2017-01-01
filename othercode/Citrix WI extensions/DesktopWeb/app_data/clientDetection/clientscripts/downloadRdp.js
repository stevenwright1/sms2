<%
// downloadRdp.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>
function onLoadLayout(){
    var rdpClientClassId = detectRdp();
    if (rdpClientClassId!="") {
        location.href='<%=model.getCurrentStep()%>';
    }
}
