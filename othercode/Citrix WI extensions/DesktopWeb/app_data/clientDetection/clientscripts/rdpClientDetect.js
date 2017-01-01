<%
// rdpClientDetect.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function detectRdp() {
    try {
        // Only try to detect RDP 6.0 and higher
        rdpClient = new ActiveXObject("MsRdp.MsRdp.5");
        setWizardCookieItem('<%=WizardConstants.RDP_CLIENT_TYPE %>','<%=WizardConstants.MsRdp%>');
        return "4EB89FF4-7F78-4A0F-8B8D-2BF02E94E4B2";
    } catch (e) {
        //Ignore exception as this is thrown when the client cannot be created
    }
    return "";
}

function checkRdpZone(rdpClientClassID){

    if(rdpClientClassID!=""){
        document.write("<object classid=clsid:" + rdpClientClassID + " id='rdpobj' height='0' width='0' border='0'></object>");
    }

    if(document.rdpobj!=null){
        if (document.rdpobj.SecuredSettingsEnabled){
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

function checkForRdp(){
    var rdpClientClassId = detectRdp();
    if (rdpClientClassId!=""){
        setWizardCookieItem('<%=WizardConstants.RDP %>','<%=WizardConstants.VAL_TRUE%>');
        setWizardCookieItem('<%=WizardConstants.COOKIE_RDPCLASSID%>',rdpClientClassId);
        var correctZone = checkRdpZone(rdpClientClassId);
        if (correctZone){
            setWizardCookieItem('<%=WizardConstants.CORRECT_ZONE_RDP %>','<%=WizardConstants.VAL_TRUE%>');
            var bpopup = popupBlocked();
            if (bpopup && '<%=inputs.getMode() != Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>'){
                location.href="<%=WizardConstants.PAGE_POPUP_HELP %>";
            } else if (bpopup){
                location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
            } else {
                setWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED%>','<%=WizardConstants.VAL_TRUE%>');
                location.href="<%=model.getNextStepWithCsrf(wizardContext, true)%>";
            }
        } else if (!correctZone && '<%=inputs.getMode() != Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>'){
            location.href = '<%=WizardConstants.PAGE_CHANGE_ZONE_HELP %>';
        } else {
            location.href = '<%=model.getNextStepWithCsrf(wizardContext) %>';
        }
    }
    else {
        if ('<%=inputs.getMode() != Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>') {
            location.href="<%=WizardConstants.PAGE_DOWNLOAD_RDP %>";
        } else {
            location.href = '<%=model.getNextStepWithCsrf(wizardContext) %>';
        }
    }
}
