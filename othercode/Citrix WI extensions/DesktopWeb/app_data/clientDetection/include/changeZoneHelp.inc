<% ChangeZoneViewModel viewModel = (ChangeZoneViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString("ChangeZoneHelpPageHeading")%></h2>
    <div id="trustedZoneText"><%=wizardContext.getString(viewModel.trustedZoneTextKey, viewModel.siteURL)%></div>
    <div class="SiteAddr"><%=viewModel.siteURL%></div>
    <%=WizardUtil.getCentralHelpMarkup(wizardContext, WizardConstants.CHANGE_ZONE_CENTER_HELP_ID, "FollowStepsToAddSite", "FollowStepsToAddSiteNoHelp")%>
    <p><%=wizardContext.getString("ConfirmSiteAdded")%></p>
    <form id="form1" class="ButtonsPane" action="">
        <input id="success" class="Buttons" onclick="location.href='<%=viewModel.urlSiteAddedLink%>'" type="button"
          title="<%=wizardContext.getString("SiteAddedToZoneTooltip")%>" value="<%=wizardContext.getString("Continue")%>">
        <input id="cancel" class="Buttons" onclick="location.href='<%=viewModel.urlCancelLink%>'"
          type="button" value="<%=wizardContext.getString("Cancel")%>">
    </form>
</div>
<div class="spacer"></div>
<%=WizardUtil.getSecurityMessageMarkup("SecurityImplicationsZoneChangeHeading", viewModel.securityMessageKey, wizardContext, 2)%>
