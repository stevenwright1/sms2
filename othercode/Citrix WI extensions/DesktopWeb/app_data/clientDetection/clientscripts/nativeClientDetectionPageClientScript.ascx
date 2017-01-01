<!--#include file="~/app_data/clientDetection/clientscripts/include.aspxf" -->

<%
    // These two query string parameters are supplied by links from the advanced mode client status table; they should
    // not both be set at once
    bool bAdvancedModeUpgradeNow = WizardConstants.VAL_TRUE == wizardContext.getWebAbstraction().getQueryStringParameter(WizardConstants.UPGRADE_NOW);
    bool bAdvancedModeShowZonePageNow = WizardConstants.VAL_TRUE == wizardContext.getWebAbstraction().getQueryStringParameter(WizardConstants.SHOW_ZONE_PAGE_ONLY);

    // only show zone page if explicitly passed showZonePageOnly in the query string i.e. from advanced page
    // or when not in advanced mode and ShowZonePage is set to On in inputs.
    bool bShowZonePage = bAdvancedModeShowZonePageNow
        || (inputs.getShowZonePage() && inputs.getMode() != Mode.ADVANCED);

    // Only show the upgrade page if we are in advanced mode and the upgrade link has been clicked or
    // we are in auto mode and showing the upgrade page is allowed
    bool bShowUpgradePage = bAdvancedModeUpgradeNow
           || (inputs.getShowUpgradePage() && inputs.getMode() != Mode.ADVANCED);

    model.setAttribute(WizardConstants.SHOW_ZONE_PAGE_ONLY, bShowZonePage ? WizardConstants.VAL_TRUE : WizardConstants.VAL_FALSE);
%>

function onLoadLayout(){
    <% if (bAdvancedModeUpgradeNow) { %>
        setWizardCookieItem('<%=WizardConstants.COOKIE_UPGRADE_LATER%>','<%=WizardConstants.VAL_FALSE%>');
    <% } %>
    <% if(!bShowUpgradePage) { %>
        setWizardCookieItem('<%=WizardConstants.COOKIE_UPGRADE_LATER%>','<%=WizardConstants.VAL_TRUE%>');
    <% } %>
    detectNative();
}

<!--#include file="~/app_data/clientDetection/clientscripts/cookies.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/commonDetection.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/nativeClientDetect.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/javaClientDetect.js" -->