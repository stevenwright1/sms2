<% DownloadViewModel viewModel = (DownloadViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString(viewModel.pageHeadingKey)%></h2>
    <% if (viewModel.customCaption != null) { %>
    <p class="CaptionInfo"><%=viewModel.customCaption%></p>
    <% } %>
    <%=WizardUtil.getDownloadButtonMarkup(!viewModel.showLicenceAgreement, wizardContext)%>
    <% if (viewModel.showLicenceAgreement) { %>
    <form id="form1" action="">
      <p><input type="checkbox" id ="chkLegalStatement" name="chkLegalStatement" value="off" onclick="updateDownloadButton()">
      <label id="lblLegalStatement" for="chkLegalStatement"><%=wizardContext.getString("CheckToAcceptLicense")%><a id="License" class="inlineHelpLink" href=""><%=wizardContext.getString(viewModel.licenseAgreementKey)%></a><%=wizardContext.getString("FullStop")%></label></p>
      <div id="Popup_License" class="wiPopup"><%=WizardUtil.getClientLicenseAgreement(wizardContext)%></div>
    </form>
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
      <a id="skipWizardLink" href="<%=WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE))%>"
         title="<%=wizardContext.getString(viewModel.skipWizardTooltipKey)%>"
           ><%=wizardContext.getString(viewModel.skipWizardTextKey)%></a>
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
