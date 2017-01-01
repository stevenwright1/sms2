<%
AdvancedViewModel viewModel = (AdvancedViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);

if (viewModel.reloadPage) {
  // Reload the page for IE as we dont want to show IE Info bar prompts on this page
  // We do this in onLoadLayout rather than in the page HTML to try and minimize "flashing" in the browser

%>
function onLoadLayout() {
    location.reload();
}
<%

} else {

%>
function onLoadLayout() {
}
<%

} // end of if(viewModel.reload)

%>
