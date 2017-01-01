<% RadeDownloadedViewModel viewModel = (RadeDownloadedViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString("RadeClientDownloadedHeading")%></h2>
    <p id="ClientInstallDescription"><%=wizardContext.getString("RestartAfterRadeInstallation")%></p>
    <form id="form1" class="ButtonsPane" action="">
      <input id="success" class="Buttons" onclick="<%=viewModel.nextPage%>" type="button"
        title="<%=wizardContext.getString("ContinueRadeTooltip")%>" value="<%= wizardContext.getString("Continue")%>">
    </form>
</div>
<ul id="wizardUsefulLinks">
    <li>
<%=WizardUtil.getSecurityMessageMarkup(null, "FurtherSecurityImplicationsFileDownload", wizardContext)%>
    </li>
    <li class="linkSeparator">
      <a id="skipWizardLink" href="<%=WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext, true))%>"
         title="<%=wizardContext.getString(viewModel.skipWizardTooltipKey)%>"
           ><%=wizardContext.getString(viewModel.skipWizardTextKey)%></a>
    </li>
</ul>
