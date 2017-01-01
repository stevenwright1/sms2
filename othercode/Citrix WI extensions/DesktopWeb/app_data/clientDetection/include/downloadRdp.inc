<% WizardViewModel viewModel = (WizardViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString("EnableRdpPageHeading")%></h2>
    <%=WizardUtil.getCentralHelpMarkup(wizardContext, WizardConstants.DOWNLOAD_RDP_CENTER_HELP_ID, "RdpNotAvailable", "RdpNotAvailableNoHelp")%>
<%
    String urlCancelLink = viewModel.showReturnToClientSelectionLink ? WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext)) :
        WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE));
%>
    <form id="form1" class="ButtonsPane" action="">
        <input id="cancel" class="Buttons" onclick="location.href='<%=urlCancelLink%>'"
          type="button" value="<%=wizardContext.getString("Cancel")%>">
    </form>
</div>

<div class="spacer"></div>
<%=WizardUtil.getSecurityMessageMarkup("RdpFurtherSecurityImplications", "RdpFurtherSecurityImplicationsObtainPermission", wizardContext, 1)%>
