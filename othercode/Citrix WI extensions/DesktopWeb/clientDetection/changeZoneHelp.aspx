<!--#include file="~/app_data/clientDetection/include/includeTop.aspxf" -->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        (new ChangeZoneController(wizardContext)).perform();
        ChangeZoneViewModel viewModel = (ChangeZoneViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage) this.Master;
        master.PageTitle = viewModel.pageTitle;
        master.PageID = WizardConstants.PAGE_ID_CHANGE_ZONE_HELP;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/include/changeZoneHelpPageView.ascx" TagName="PageView" TagPrefix="wizard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ViewPlaceHolder" runat="server">
    <wizard:PageView ID="ChangeZoneHelpPageView" runat="server" />
</asp:Content>