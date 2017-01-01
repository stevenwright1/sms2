<% PopupHelpViewModel viewModel = (PopupHelpViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL); %>
<h1 id="ClientDetectOverallHeading"><%=wizardContext.getString("ClientDetectionDownload")%></h1>
<div class="MainDiv">
    <h2><%=wizardContext.getString("PopupHelpPageHeading")%></h2>
    <%=WizardUtil.getCentralHelpMarkup(wizardContext, WizardConstants.POPUP_CENTER_HELP_ID, viewModel.allowPopupsTextKey, "AllowPopupsNoHelp")%>
    <% if (!wizardContext.getClientInfo().isIE()) { %>
    <p><%=wizardContext.getString("AllowPopupClickButton")%></p>
    <form id="form1" class="ButtonsPane" action="">
      <input id="success" class="Buttons" onclick="javascript:location.reload()" type="button"
        title="<%=wizardContext.getString("PopupsAllowedButtonTooltip")%>" value="<%=wizardContext.getString("Continue")%>">
      <input id="cancel" class="Buttons" onclick="location.href='<%=viewModel.urlCancelLink%>'" type="button"
        value="<%=wizardContext.getString("Cancel")%>">
    </form>
    <% } %>
</div>
