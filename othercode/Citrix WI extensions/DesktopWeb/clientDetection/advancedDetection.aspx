<!--#include file="~/app_data/clientDetection/include/includeTop.aspxf" -->

<script runat="server">
    void Page_Load(object sender, EventArgs e) {
        (new AdvancedDetectionController(wizardContext)).perform();

        ViewModel viewModel = (ViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage) this.Master;
        master.TransientPage = viewModel.transientPage;
        master.PageTitle = viewModel.pageTitle;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/clientscripts/advancedDetectionPageClientScript.ascx" TagName="ClientScript" TagPrefix="wizard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ClientScriptPlaceHolder" runat="server">
    <wizard:ClientScript ID="AdvancedDetectionPageClientScript" runat="server" />
</asp:Content>