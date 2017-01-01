<%
// feedback.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
if (feedbackControl.isFeedbackSet()) {
%>
    <div id="feedbackArea">
        <p class="<%=feedbackControl.getFeedbackCssClass()%>">
            <%=feedbackControl.getFeedback().getMessageString(wiContext)%>
        </p>
    </div>
<%
} else {
%>
    <div id="feedbackArea" class="noFeedback"><!-- --></div>
<%
}
%>
