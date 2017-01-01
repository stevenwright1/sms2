<%
// processRadeDetection.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function detectRade(){
    // First determine whether this is the first or second visit to the page
    var pageHasBeenReloaded = ('<%=pageHasBeenReloaded%>' == '<%=WizardConstants.VAL_TRUE%>');

    if (pageHasBeenReloaded) {
        // The page has been reloaded (to remove the IE infobar) and this is therefore the
        // second visit.
        // The page should not perform any more detection and should just redirect to the next
        // step of the wizard.
        location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
        return;
    }

    // If execution has reached this point then this is only the first visit to this page
    // and detection can proceed as normal.

    var detected = detectRadeClient();
    if (detected){
        if (!isRadeClientUpToDateClient()){
            setWizardCookieItem('<%=WizardConstants.RADE%>','<%=WizardConstants.UPGRADEABLE%>');
        } else {
            setWizardCookieItem('<%=WizardConstants.RADE%>','<%=WizardConstants.VAL_TRUE%>');
        }
        if (isCorrectRadeZone()){
            setWizardCookieItem('<%=WizardConstants.CORRECT_ZONE_RADE%>','<%=WizardConstants.VAL_TRUE%>');
        }
    }

    // Rade client detection is always performed silently except when this is the only
    // client to be detected. So for a dual mode site, when the wizard is run in AUTO mode,
    // the wizard will offer download help for the REMOTE client rather than the STREAMING client, even
    // though the streaming client is detected first.
    var interactive = '<%=isInteractive%>' == '<%=WizardConstants.VAL_TRUE%>';
    if (!interactive){
        // Non-interactive mode
        if('<%=needToReloadPage%>' == '<%=WizardConstants.VAL_TRUE%>'){
            // In order to remove the info bar caused by instantiating the RCO, we need to
            // reload this page.
            location.reload();
        } else {
            // No need to reload so proceed directly to the next step
            location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
        }
    } else {
        if (detected){
            if (!isRadeClientUpToDateClient()
                 && '<%=inputs.getShowUpgradePage()%>' == '<%=WizardConstants.VAL_TRUE%>'
                 && getWizardCookieItem('<%=WizardConstants.COOKIE_UPGRADE_LATER%>') != '<%=WizardConstants.VAL_TRUE%>'){
                location.href = '<%=WizardConstants.PAGE_UPGRADE_RADE %>';
                return;
            }
            if (!isCorrectRadeZone()){
                location.href = '<%=WizardConstants.PAGE_CHANGE_ZONE_HELP %>';
            } else {
                location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
            }
        } else {
            location.href='<%=WizardConstants.PAGE_DOWNLOAD_RADE %>';
        }
    }
}