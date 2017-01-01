<%
// commonDetection.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function popupBlocked(){
    var res = false;
    var popupAllowed = getWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED%>');
    popupAllowed = popupAllowed == '<%=WizardConstants.VAL_TRUE%>';
    if (popupAllowed){
        res = !popupAllowed;
    } else {
        var ptest = window.open('popupTest.html','tmp',"height=1,width=1,status=no,toolbar=no,menubar=no,location=no, top=1600,left=1600");
        if(ptest!=null){
            try{
              ptest.close(); //close window
              res = false;
            } catch(e) {
              //some popup blockers cause this close attempt to fail since the window wasn't actually opened
              res=true;
            }
        }else{
          //you have a blocker installed
          res = true;
        }
    }
    return res;
}

// Compare two comma- or dot-separated version numbers.
// Return true if haveVersion >= wantVersion
function isUpToDateVersion(haveVersion, wantVersion) {
    var haveComponents = getVersionComponents(haveVersion);
    var wantComponents = getVersionComponents(wantVersion);

    for(var ix=0; ix<wantComponents.length; ix++) {

        var have = ( ix<haveComponents.length ) ? parseInt(haveComponents[ix]) : 0;
        var want = parseInt(wantComponents[ix]);

        if( have > want ) {
            return true;
        }
        if( have < want ) {
            return false;
        }
    }

    return true;
}

// Split a version number into its components.
// This function can accept a comma or dot as a separator.
function getVersionComponents(version) {
    if(version.indexOf(".") == -1) {
        return version.split(",");
    } else {
        return version.split(".");
    }
}