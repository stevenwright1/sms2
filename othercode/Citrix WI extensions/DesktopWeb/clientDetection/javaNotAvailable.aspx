<!--#include file="~/app_data/clientDetection/include/includeTop.aspxf" -->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        (new JavaNotAvailableController(wizardContext)).perform();
        ViewModel viewModel = (ViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage) this.Master;
        master.PageTitle = viewModel.pageTitle;
        master.PageID = WizardConstants.PAGE_ID_JAVA_NOT_AVAILABLE;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/include/javaNotAvailablePageView.ascx" TagName="PageView" TagPrefix="wizard" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ViewPlaceHolder" runat="server">
    <wizard:PageView ID="PageView" runat="server" />
</asp:Content>