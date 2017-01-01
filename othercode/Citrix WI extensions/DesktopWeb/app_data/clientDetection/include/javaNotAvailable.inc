<% JavaNotAvailableViewModel viewModel = (JavaNotAvailableViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString("JavaNotAvailablePageHeading")%></h2>
    <%=WizardUtil.getCentralHelpMarkup(wizardContext, WizardConstants.JAVA_NOT_AVAILABLE_CENTER_HELP_ID, "JavaNotAvailable", "JavaNotAvailable")%>
    <form id="form1" class="ButtonsPane" action="">
      <input id="Continue" class="Buttons" type="button" value="<%=wizardContext.getString("Continue")%>"
        title="<%=wizardContext.getString("ClickIfJavaInstalled")%>" onclick="location.href='<%=WebUtilities.escapeHTML(wizardContext.getModel().getCurrentStep())%>'">
      <input id="Cancel" class="Buttons" type="button" value="<%=wizardContext.getString("Cancel")%>"
        onclick="location.href='<%=viewModel.urlCancelLink%>'">
    </form>
</div>
