<div class="commonInnerBox">
<% if (viewControl.retryInProgress) { %>
  <p class="secondParagraph"><%=wiContext.getString("SwitchOffWhileStarting", viewControl.desktopDisplayName)%></p>
<% } else { %>
  <p class="secondParagraph"><%=wiContext.getString("SwitchOff", viewControl.desktopDisplayName)%></p>
<% } %>
  <p><%=wiContext.getString("UnsavedWorkLost")%></p>
  <div class="buttonsPane">
    <input type="hidden" name="<%=Constants.ID_SUBMIT_MODE%>" value="<%=Constants.VAL_OK%>">
    <input type="hidden" name="<%=Constants.ID_APPLICATION%>" value="<%=viewControl.appId%>">
    <input type="hidden" name="<%=Constants.ID_RETRY_IN_PROGRESS%>" value="<%=viewControl.retryInProgress%>">

    <input type="submit" value="<%=wiContext.getString("OK")%>"
        id="submit1" name="submit1" class="Apply_Cancel_OK">
    <input type="button" value="<%=wiContext.getString("Cancel")%>"
        onclick="form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
        id="reset1" name="reset1" class="Apply_Cancel_OK">
  </div>
  <div class="spacer"></div>
</div>
