<%
AdvancedViewModel viewModel = (AdvancedViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
if (viewModel.reloadPage ) {
  // don't show anything
  return;
}
%>

<div class="MainDiv AdvancedMode">
<h2><%= wizardContext.getString("ClientSelection") %></h2>
<p><%=wizardContext.getString("ClientSettingsDescription")%></p>
</div>
<div class="spacer"></div>

<form id="form1" action="<%=WizardConstants.PAGE_FINISH%>"  method="get">
<input type="hidden" name="<%=WizardConstants.QSTR_TOKENNAME%>" value="<%=WizardUtil.getCsrfToken(wizardContext.getWebAbstraction())%>">

<h6 class="settingsHeader"><%=wizardContext.getString("Client")%></h6>
<dl>
  <dt>
    <p><%=wizardContext.getString("ClientsDescription")%></p>
  </dt>
  <dd>
    <table class="clientStatus" cellspacing="0">
        <% if (viewModel.showAvailableTable) { %>
        <tr class="headingRow" id="Available">
            <th><img src="<%=ClientInfoUtilities.getImageName(wizardContext.getClientInfo(), "media/TickRound.png")%>"  alt=""></th>
            <th colspan="2"><%=wizardContext.getString("Available")%></th>
        </tr>
        <%=AdvancedPageController.getAvailableClientRows(wizardContext, viewModel)%>
        <% } %>
        <% if (viewModel.showNotAvailableTable) { %>
        <tr class="headingRow" id="NotAvailable" >
            <th><img src="<%=ClientInfoUtilities.getImageName(wizardContext.getClientInfo(), "media/WarningRound.png")%>"  alt=""></th>
            <th colspan="2"><%=wizardContext.getString("NotCurrentlyAvailable")%></th>
        </tr>
        <%=AdvancedPageController.getNotAvailableClientRows(wizardContext, viewModel)%>
        <% } %>
    </table>

    <a id="Help_Client" class="inlineHelpLink" href="#"><img src="media/help.gif" alt=""></a>
    <div id="Popup_Help_Client" class="wiPopup"><%=AdvancedPageController.getClientInlineHelpMarkup(wizardContext, viewModel)%></div>
    <div class="spacer"></div>

    <% if (viewModel.showAsterisk) { %>
            <p class="AsteriskText">*<%= wizardContext.getString("ModifyExplained")%></p>
    <% } %>

    <p id="ClientDropDownLabel"><label for="<%=WizardConstants.ID_DEFAULT_CLIENT%>"><%= wizardContext.getString("DefaultClientDescription") %></label></p>
    <p class="ClientDropDown"><select name="<%=WizardConstants.ID_DEFAULT_CLIENT%>" id="<%=WizardConstants.ID_DEFAULT_CLIENT%>" <%//onchange="clientSelected()"%>>
        <option id="Auto" value="<%=WizardConstants.AUTO%>" <%if (viewModel.showAutoAsSelected) {%>selected="selected"<%}%>>
            <%= wizardContext.getString(WizardConstants.AUTO)%>
        </option>
<%
Iterator clientsForDropDown = viewModel.clients.iterator();
while(clientsForDropDown.hasNext()){
  ClientType clientType = (ClientType) clientsForDropDown.next();
%>
         <option id="<%=clientType.getName()%>" value="<%=clientType%>" <%if (AdvancedPageController.isClientSelected(clientType, wizardContext)) { %>selected="selected"<% } %>>
            <%= wizardContext.getString(clientType.getName())%>
         </option>
<%
}
%>
    </select>
    </p>
    <a id="Help_ClientDropDown" class="inlineHelpLink" href="#"><img src="media/help.gif" alt=""></a>
    <div id="Popup_Help_ClientDropDown" class="wiPopup"><%=wizardContext.getString("ClientDropDownHelp")%></div>
    <div class="spacer"></div>
  </dd>
</dl>
<div id="advancedPageButtons">
  <div>
    <div class="ButtonsPane">
      <input class="Apply_Cancel_OK" type="button" value="<%=wizardContext.getString("Save")%>"
          title="<%=wizardContext.getString("AdvancedOkButtonTooltip")%>" onclick="form.submit()">
      <input class="Apply_Cancel_OK" type="button" value="<%=wizardContext.getString("Cancel")%>"
          title="<%=wizardContext.getString("AdvancedCancelButtonTooltip")%>" onclick="form.submitMode.value='<%=WizardConstants.VAL_CANCEL%>';form.submit()">
      <input type="hidden" name="<%=WizardConstants.ID_SUBMIT_MODE%>" value="<%=WizardConstants.VAL_OK%>">
    </div>
  </div>
</div>

</form>
