<%
// retry.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

// Sets the timer to retry the launch after given number of milliseconds
// At timeout this calls the retryLaunch method call
function retry(delayHintInMilliseconds){
    setTimeout("retryLaunch()",delayHintInMilliseconds);
}

// This method redirects the window to retry page
function retryLaunch(){
    // now retry the launch and also add the current time to the url for euem
    window.location.replace(addCurrentTimeToURL('<%= viewControl.getLaunchUrl() %>','<%=Constants.QSTR_METRIC_LAUNCH_ID %>'));
}
