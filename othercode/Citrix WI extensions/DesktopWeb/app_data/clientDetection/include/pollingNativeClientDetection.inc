<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
 <script type="text/javascript">
<!--
<%
  // let the browser cache this page for a defined length of time without 
  // asking if it has been updated to prevent flickering as page is reloaded
  // (use of 'private' should prevent proxies from caching the page
  //  which will only be correct for a given browser)
  web.setResponseCacheDuration(5);
  web.setResponseCacheControl(WebAbstraction.CACHE_CONTROL_PRIVATE);

  ClientInfo sClientInfo = (ClientInfo)web.getSessionAttribute(WizardConstants.SV_CLIENT_INFO);
%>

<% if ( (sClientInfo.osWin32() || sClientInfo.osWin64()) && sClientInfo.isIE() ) { %>

    // For Win32 IE, we get the version from the ActiveX ICO.
    function getClientDetails() {
        var details = {
              pollingSupported: true,
              // IE will do the polling from inside this page so parent page
              // doesn't need to keep reloading
              reloadRequired: false,
              detected: false,
              version: null
        };
        var obj = createIcaObj();
        if (obj != null) {
            details.detected = true;
            try {
                var temp = obj.ClientVersion;
                if (temp != null && temp != "") {
                    details.version = temp;
                }
            } catch (e) {}
        }
        return details;
    }

    // Create an ActiveX ICA Object.
    function createIcaObj() {
        var obj = null;
        try {
            obj = new ActiveXObject("Citrix.ICAClient");
        } catch (e) {}
        return obj;
    }

<% } else if (sClientInfo.isMozillaBased() || (sClientInfo.osMacOSX() && sClientInfo.isSafari())) { %>

    var pluginObjectId        = 'IcaObj';
    var pluginMimetype        = 'application/x-ica';
    var pluginParentElementId = 'insertionPoint';

    function hasPluginForMimetype(mimeType) {
        if (navigator && navigator.plugins) {
            if (navigator.plugins.refresh) {
                navigator.plugins.refresh(false);
            }

            var pluginCount = navigator.plugins.length;
            for (var i = 0; i < pluginCount; i++) {
                var plugIn = navigator.plugins[i];
                var mimeCount = plugIn.length;
                for (var m = 0; m < mimeCount; m++) {
                    if (plugIn[m].type == mimeType) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    function createEmbeddedObjectForMimetype(parentId, objectId, mimeType) {
        if (hasPluginForMimetype(mimeType)) {
            try {
                var objInsElement = document.getElementById(parentId);
                if (objInsElement) {
                    objInsElement.innerHTML
                      = '<object type="' + mimeType
                              + '" id="' + objectId
                            + '" name="' + objectId + '"></object>';
                }
            } catch (e) {}
        }
    }

    // For Win32 Firefox, we get the version from the Plug-in ICO.
    function getClientDetails() {
        createEmbeddedObjectForMimetype(pluginParentElementId, pluginObjectId, pluginMimetype);
        var details = {
              pollingSupported: true,
              reloadRequired: true,
              detected: false,
              version: null
        };
        if (hasPluginForMimetype(pluginMimetype)) {
            details.detected = true;
            try {
                var elem = document.getElementById(pluginObjectId);
                var version = elem && elem.GetPropValue && elem.GetPropValue('ClientVersion');
                if (version != null && version != "") {
                    details.version = version;
                }
            } catch (e) {}
        }
        return details;
    }

<% } else { %>

    // Other browsers and OSs just return default details object

    function getClientDetails() {
        return {
              pollingSupported: false,
              reloadRequired: false,
              detected: false,
              version: null
        };
    }

<% } %>

    function doDetection() {

        // Call the client detection method
        // to get the object describing client
        // detection results
        var details = getClientDetails();

        // Call the parent's callback if defined
        if (parent && parent.clientDetailsCallback) {
            // callback returns true if detection has succeeded
            // so we don't need to keep polling
            if (parent.clientDetailsCallback(details)) {
                return;
            }
        }
<% if ( (sClientInfo.osWin32() || sClientInfo.osWin64()) && sClientInfo.isIE() ) { %>
        // trigger next detection poll
        setTimeout('doDetection();', <%= WizardConstants.CLIENT_DETECTION_POLLING_PERIOD %>);
<% } %>
    }


// -->
 </script>
</head>
<body onLoad="setTimeout('doDetection();', 100);">
  <p>Empty page (js used for detecting client presence and version)</p>
  <div id="insertionPoint"></div>
</body>
</html>
