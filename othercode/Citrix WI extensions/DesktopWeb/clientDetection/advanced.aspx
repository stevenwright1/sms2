<!--#include file="~/app_data/clientDetection/include/includeTop.aspxf" -->

<script runat="server">
    void Page_Load(object sender, EventArgs e) {
        (new AdvancedPageController(wizardContext)).perform();

        AdvancedViewModel viewModel = (AdvancedViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage) this.Master;
        master.PageTitle = viewModel.pageTitle;
        master.TransientPage = viewModel.transientPage;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/clientscripts/advancedPageClientScript.ascx" TagName="ClientScript" TagPrefix="wizard" %>
<%@ Register Src="~/app_data/clientDetection/include/advancedPageView.ascx" TagName="PageView" TagPrefix="wizard" %>
<asp:Content ContentPlaceHolderID="ClientScriptPlaceHolder" runat="server">
    <wizard:ClientScript ID="PageClientScript" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="ViewPlaceHolder" runat="server">
    <wizard:PageView ID="PageView" runat="server" />
</asp:Content>
