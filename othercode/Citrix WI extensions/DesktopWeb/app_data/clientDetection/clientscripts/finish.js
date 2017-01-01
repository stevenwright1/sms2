function onLoadLayout(){
<%
    String reload = (String) wizardContext.getModel().getAttribute(WizardConstants.RELOADED);
    if (reload == WizardConstants.VAL_TRUE) {
%>
    <!-- Detect the ico state, set the appropriate cookie, then reload the page to repopulate the outputs with the new information -->
    detectAndSaveIcoStatus();
    <!-- We need to reload the page for IE anyway as we dont want to show IE Info bar prompts on this page -->
    <!-- We do this in onLoadLayout rather than in the page HTML to try and minimize "flashing" in the browser -->
    location.reload();
<%
    }
%>
}
