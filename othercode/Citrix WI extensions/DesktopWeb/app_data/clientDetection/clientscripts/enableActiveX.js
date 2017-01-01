<%
// enableActiveX.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

<% if (model.getCurrentStep() == WizardConstants.PAGE_RADE) { %>
        function onLoadLayout(){
            var detected = detectRadeClient();
            if (detected){
                setWizardCookieItem('<%=WizardConstants.RADE%>','<%=WizardConstants.VAL_TRUE%>');
                if (isCorrectRadeZone()){
                    setWizardCookieItem('<%=WizardConstants.CORRECT_ZONE_RADE %>','<%=WizardConstants.VAL_TRUE%>');
                    location.href='<%=model.getNextStepWithCsrf(wizardContext, true)%>';
                } else {
                    location.href='<%=WizardConstants.PAGE_CHANGE_ZONE_HELP %>';
                }
            } else {
                displayDiv("MainDiv");
            }
        }
<% } else { %>
    function onLoadLayout(){
        var detected = detectNativeClient();
        if (detected){
            setWizardCookieItem('<%=WizardConstants.NATIVE%>','<%=WizardConstants.VAL_TRUE%>');
                if (isCorrectZone()){
                    location.href='<%=model.getNextStepWithCsrf(wizardContext, true)%>';
                } else {
                    setWizardCookieItem('<%=WizardConstants.INCORRECT_ZONE_NATIVE %>','<%=WizardConstants.VAL_TRUE%>');
                    if ('<%= wizardContext.getInputs().getShowZonePage() %>' == '<%=WizardConstants.VAL_TRUE %>'
                           && '<%=model.getAttribute(WizardConstants.SHOW_ZONE_PAGE_ONLY)%>' == '<%=WizardConstants.VAL_TRUE %>'){
                        location.href='<%=WizardConstants.PAGE_CHANGE_ZONE_HELP %>';
                    } else {
                        location.href='<%=model.getNextStepWithCsrf(wizardContext, true)%>';
                    }
                }
        } else {
            displayDiv("MainDiv");
        }
    }
<% } %>
