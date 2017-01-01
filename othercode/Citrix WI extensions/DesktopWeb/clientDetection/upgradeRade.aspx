<!--#include file="~/app_data/clientDetection/include/includeTop.aspxf" -->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        (new UpgradeRadeController(wizardContext)).perform();
        ViewModel viewModel = (ViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage) this.Master;
        master.PageTitle = viewModel.pageTitle;
        master.PageID = WizardConstants.PAGE_ID_UPGRADE_RADE;
        master.HorizonLayoutPage = true;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/clientscripts/upgradeRadePageClientScript.ascx" TagName="ClientScript" TagPrefix="wizard" %>
<%@ Register Src="~/app_data/clientDetection/include/upgradeRadePageView.ascx" TagName="PageView" TagPrefix="wizard" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ClientScriptPlaceHolder" runat="server">
    <wizard:ClientScript ID="PageClientScript" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ViewPlaceHolder" runat="server">
    <wizard:PageView ID="PageView" runat="server" />
</asp:Content>