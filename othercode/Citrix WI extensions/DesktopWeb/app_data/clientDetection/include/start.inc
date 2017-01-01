<% String error = wizardContext.getModel().getError();%>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2 id="StartHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h2>
    <%
    if (error != null) {
        String errorStr = wizardContext.getString(error);
    %>
        <img src="../media/Error24.gif" align="left" alt="">
        <span class="Error"><%=wizardContext.getString(WizardConstants.ERROR)%></span>
    <%
    } else {
    %>
        <% if (!WizardUtil.isSupportedBrowser(wizardContext.getClientInfo().getBrowser())) { %>
        <p class="feedbackStyleInfo"><%=wizardContext.getString("UnsupportedBrowser")%></p>
        <% } %>
        <p><%=wizardContext.getString("ClientDetectionInfo")%></p>
        <p><%=wizardContext.getString("StartButtonDescription")%></p>
        <div class="ButtonsDiv">
        <form id="form1" action="">
            <input id="Continue" class="Buttons" onclick="location.href='<%=WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext))%>'" type="button"
                title="<%=wizardContext.getString("ContinueButtonTooltip")%>" value="<%=wizardContext.getString("DetectClients")%>">
<% if (error == null) { %>
            <input id="Cancel" class="Buttons" onclick="location.href='<%=WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE))%>'"
                type="button" value="<%=wizardContext.getString("Cancel")%>">
<% } %>
        </form>
        </div>
    <% } %>
</div>
