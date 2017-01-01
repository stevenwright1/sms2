<% UpgradeViewModel viewModel = (UpgradeViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
  <h2><%=wizardContext.getString(viewModel.pageHeadingKey)%></h2>
  <%=WizardUtil.getDownloadButtonMarkup(true, wizardContext)%>
</div>
<ul id="wizardUsefulLinks">
  <li>
<%=WizardUtil.getSecurityMessageMarkup(null, "FurtherSecurityImplicationsFileDownload", wizardContext)%>
  </li>
<% if (viewModel.showUpgradeLaterLink) { %>
  <li class="linkSeparator">
    <a id="skipWizardLink" href="<%=WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext, true))%>"
       title="<%=wizardContext.getString("UpgradeLaterButtonTooltip")%>"
         ><%=wizardContext.getString("UpgradeLater")%></a>
  </li>
<% } %>
</ul>
