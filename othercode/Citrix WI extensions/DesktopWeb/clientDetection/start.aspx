<%@ Page MasterPageFile="~/app_data/clientDetection/include/wizardLayout.master" Language="C#"%>
<%@ MasterType TypeName="com.citrix.wi.ui.BaseMasterPage" %>

<!--#include file="~/app_data/clientDetection/include/include.aspxf" -->

<script runat="server">
    WizardContext wizardContext;

    void Page_PreInit(object sender, System.EventArgs e) {
        wizardContext = new WizardContext(AspWebAbstraction.getInstance(Context));
        (new InitializeWizard()).perform(wizardContext);


        WebAbstraction abstraction = wizardContext.getWebAbstraction();
        if (abstraction.isNewSession())
        {
            WizardUtil.handleSessionExpired(abstraction);
            Response.End();
        }
        try {
            if (wizardContext.getInputs().getMasterPage() != null){
                this.MasterPageFile = wizardContext.getInputs().getMasterPage();
            }
        } catch (Exception) {
            wizardContext.getInputs().setMasterPage(null);
            this.MasterPageFile = "~/app_data/clientDetection/include/wizardLayout.master";
        }
    }
    void Page_Load(object sender, System.EventArgs e)
    {
        (new StartPageController(wizardContext)).perform();
        ViewModel viewModel = (ViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
        BaseMasterPage master = (BaseMasterPage)this.Master;
        master.PageTitle = viewModel.pageTitle;
        master.PageID = WizardConstants.PAGE_ID_START;
    }
</script>

<%@ Register Src="~/app_data/clientDetection/clientscripts/startPageClientScript.ascx" TagName="ClientScript" TagPrefix="wizard" %>
<%@ Register Src="~/app_data/clientDetection/include/startPageView.ascx" TagName="PageView" TagPrefix="wizard" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ClientScriptPlaceHolder" runat="server">
    <wizard:ClientScript ID="PageClientScript" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ViewPlaceHolder" runat="server">
    <wizard:PageView ID="PageView" runat="server" />
</asp:Content>
