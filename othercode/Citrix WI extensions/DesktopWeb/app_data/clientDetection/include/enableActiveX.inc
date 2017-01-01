<% WizardViewModel viewModel = (WizardViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div id="MainDiv" class="MainDiv">
  <h2><%=wizardContext.getString("EnableActiveXPageHeading")%></h2>
  <p id="ClientInstallDescription"><%=wizardContext.getString("EnableActiveX")%></p>
</div>

<ul id="wizardUsefulLinks">
    <li>
<%=WizardUtil.getSecurityMessageMarkup(null, "FurtherSecurityImplicationsObtainPermission", wizardContext)%>
    </li>
<% if (viewModel.showSkipLink) { 
    String skipWizardTextKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkText" : "SkipToLoginText";
    String skipWizardTooltipKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkTooltip" : "SkipToLoginTooltip";
%>
    <li class="linkSeparator">
      <a id="skipWizardLink" href="<%=WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE))%>"
        title="<%=wizardContext.getString(skipWizardTooltipKey)%>"
          ><%=wizardContext.getString(skipWizardTextKey)%></a>
    </li>
<% } %>

<% if (viewModel.showReturnToClientSelectionLink) { %>
    <li class="linkSeparator">
      <a id="returnToClientSelectionLink" href="<%=WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext))%>"
         title="<%= wizardContext.getString("TryOtherMethodsActiveXProblem")%>"
           ><%=wizardContext.getString("ReturnToClientSelection")%></a>
    </li>
<% } %>
</ul>
