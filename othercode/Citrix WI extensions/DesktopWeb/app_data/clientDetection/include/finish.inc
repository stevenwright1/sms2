<%
FinishViewModel viewModel = (FinishViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
if (!viewModel.reloadingPage) {
%>
<form name="wizardOutputForm" action="<%=viewModel.redirectURL%>" method="post">
    <input type="hidden" name="<%=WizardConstants.FORM_CSRF_TOKEN%>" value="<%=WizardUtil.getCsrfToken(wizardContext.getWebAbstraction())%>">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_REMOTE_CLIENT%>" value="<%=viewModel.remoteClientResult%>">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_STREAMING_CLIENT%>" value="<%=viewModel.streamingClientResult%>">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_ICO_STATUS%>" value="<%=viewModel.icoStatusResult%>">
    <% if (viewModel.showAlternateResult ) { %>
    <input type="hidden" name="<%=WizardConstants.ALTERNATE_RESULT%>" value="<%=viewModel.alternateResult%>">
    <% } %>
    <% if (viewModel.showRDPClassID) { %>
    <input type="hidden" name="<%=WizardConstants.PARAMETER_RDP_CLIENT_CLASS_ID%>" value="<%=viewModel.RDPClassID%>">
    <% } %>
</form>
<script type="text/javascript">
    <!--
    document.wizardOutputForm.submit();
    // -->
</script>
<%
}
%>
