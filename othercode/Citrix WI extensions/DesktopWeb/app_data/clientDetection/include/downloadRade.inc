<% DownloadViewModel viewModel = (DownloadViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString(viewModel.pageHeadingKey)%></h2>
    <%=WizardUtil.getDownloadButtonMarkup(true, wizardContext)%>
</div>

<ul id="wizardUsefulLinks">
    <li>
<%=WizardUtil.getSecurityMessageMarkup(null, "FurtherSecurityImplicationsFileDownload", wizardContext)%>
    </li>
<% if (viewModel.showSkipLink) { %>
    <li class="linkSeparator">
      <a id="skipWizardLink" href="<%=WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE))%>"
         title="<%=wizardContext.getString(viewModel.skipWizardTooltipKey)%>"
           ><%=wizardContext.getString(viewModel.skipWizardTextKey)%></a>
    </li>
<% } %>
</ul>
