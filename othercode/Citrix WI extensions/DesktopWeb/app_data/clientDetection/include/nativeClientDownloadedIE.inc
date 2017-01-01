<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <% if (viewModel.upgrade) { %>
    <h2><%= wizardContext.getString("NativeClientUpgradeHeading")%></h2>
    <p id="ClientInstallDescription"><%= wizardContext.getString("UpgradeInProgress")%></p>
    <% } else { %>
    <h2><%= wizardContext.getString("EnableActiveXPageHeading")%></h2>
    <p id="ClientInstallDescription"><%= wizardContext.getString("EnableActiveX")%></p>
    <% } %>
    <% if (viewModel.showUseJavaLink) { %>
    <p id="JavaClientLink"><%=wizardContext.getString("JavaClientLinkInstalling", "'" + WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext)) + "'")%></p>
    <% } %>
</div>

<ul id="wizardUsefulLinks">
    <li>
<%=WizardUtil.getSecurityMessageMarkup(null, "FurtherSecurityImplicationsFileDownload", wizardContext)%>
    </li>
<% if (viewModel.showSkipLink) { %>
    <li class="linkSeparator">
      <a id="skipWizardLink" href="<%=viewModel.skipWizardLink%>" title="<%=wizardContext.getString(viewModel.skipWizardTooltipKey)%>"><%=wizardContext.getString(viewModel.skipWizardTextKey)%></a>
    </li>
<% } %>

<% if (viewModel.showReturnToClientSelectionLink) { %>
    <li class="linkSeparator">
      <a id="returnToClientSelectionLink" href="<%=WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext))%> "
         title="<%= wizardContext.getString("TryOtherMethods")%>"
           ><%=wizardContext.getString("ReturnToClientSelection")%></a>
    </li>
<% } %>
</ul>

<iframe class="HiddenControl" id="pollingDetectionFrame" name="pollingDetectionFrame" src="<%= WizardConstants.PAGE_NATIVE_POLLING %>"></iframe>
<form><input id="success" onclick="<%= viewModel.autoSuccessPage %>" type="hidden" title="" value=""></form>
