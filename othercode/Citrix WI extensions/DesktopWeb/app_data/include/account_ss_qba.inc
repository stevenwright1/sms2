<%
// account_ss_qba.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="accountSelfServicePane">
  <h5>
    <%=wiContext.getString("Question")%>
    <%=viewControl.getQuestionNumber()%>
    <%=wiContext.getString("Of")%>
    <%=viewControl.getTotalQuestionCount()%>
  </h5>
  <p class="questionValue">
    <%=WebUtilities.escapeHTML(viewControl.getQuestionText())%>
  </p>
  <ul id="accountSSForm">
    <li>
      <label id='lblAnswer' class='<%=viewControl.getLabelClass(Constants.ID_ASS_ANSWER)%>' for='<%=AccountSelfService.ID_ANSWER%>'>
        <%=viewControl.getLabelPrefix(Constants.ID_ASS_ANSWER)%><%=wiContext.getString("AnswerLabel")%>
      </label>
    <input type='<%=viewControl.getAnswerFieldType()%>' name='<%=AccountSelfService.ID_ANSWER%>' id='<%=AccountSelfService.ID_ANSWER%>'>
    </li>
<% if (viewControl.getShowConfirmationField()) { %>
    <li>
      <label id='lblConfirmAnswer' for='<%=AccountSelfService.ID_CONFIRM_ANSWER%>'>
        <%=wiContext.getString("ConfirmAnswerLabel")%>
      </label>
    <input type='<%=viewControl.getAnswerFieldType()%>' name='<%=AccountSelfService.ID_CONFIRM_ANSWER%>' id='<%=AccountSelfService.ID_CONFIRM_ANSWER%>'>
    </li>
<% } // end show confirmation field %>
  </ul>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type='submit' name='btnContinue' value='<%=wiContext.getString(viewControl.continueButtonLabelKey)%>'
        title='<%=wiContext.getString("OKTooltip")%>' onclick='this.disabled=true;form.submit();' class='Cancel_OK'>
    <input type='button' name='btnCancel' value='<%=wiContext.getString("Cancel")%>'
      title='<%=wiContext.getString("CancelTooltip")%>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();" class='Cancel_OK'>
  </div>
</div>