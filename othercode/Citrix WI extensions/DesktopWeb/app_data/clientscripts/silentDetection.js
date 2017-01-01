<%
// silentDetection.js
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

// JavaScript code for performing client detection.
// Everything should be considered private except for the ClientDetection object.
%>

<%
// Simple object encapsulating information about the client OS and browser.
// It avoids excessive intermingling of client and server script.
ClientInfo ci = wiContext.getClientInfo();
%>
var ClientInfo = {
    isIE: function() {return <%=ci.isIE() ? "true" : "false"%>;},
    isFirefox: function() {return <%=ci.isFirefox() ? "true" : "false"%>;},
    isNetscape: function() {return <%=ci.isNetscape() ? "true" : "false"%>;},
    isSafari: function() {return <%=ci.isSafari() ? "true" : "false"%>;},
    isWindows: function() {return <%=ci.osWin32() || ci.osWin64() ? "true" : "false"%>;},
    isWinCE: function() {return <%=ci.osWinCE() ? "true" : "false"%>;},
    isMacOSX: function() {return <%=ci.osMacOSX() ? "true" : "false"%>;},
    isSymbian: function() {return <%=ci.osSymbian() ? "true" : "false"%>;}
};

var DetectionUtils = {
    _popupsEnabled: null,
    popupsEnabled: function() {
        if (DetectionUtils._popupsEnabled != null) {
            return DetectionUtils._popupsEnabled;
        }
        try {
            var popup = window.open("","testwindow","height=1,width=1,scrollbars=no,status=no,resizable=no,toolbar=no");
            if (popup != null) {
                popup.close();
                DetectionUtils._popupsEnabled = true;
                return true;
            }
        } catch(e) {}
        DetectionUtils._popupsEnabled = false;
        return false;
    }
};

<%
// Helper object that uses an ActiveX control or browser plug-in to determine if a particular client
// is installed and to gather information about it.
%>
var _CitrixClient = {
    init: function(mimeType) {
        if (ClientInfo.isNetscape() || ClientInfo.isFirefox() || (ClientInfo.isMacOSX() && ClientInfo.isSafari())) {
            if (_CitrixClient.isInstalled(null, mimeType)) {
                var hiddenDiv = document.getElementById("hiddenDiv");
                hiddenDiv.innerHTML = "<embed type='" + mimeType + "' hidden='true' name='IcaObj' id='IcaObj'></embed>";
            }
        }
    },
    isInstalled: function(activeX, mimeType) {
        if (ClientInfo.isWinCE() || ClientInfo.isSymbian()) {
            return true; // We can't tell or install it if we could
        }
        if (ClientInfo.isIE() && ClientInfo.isWindows()) {
            try {
                var obj = new ActiveXObject(activeX);
                if (obj) {
                    return true;
                }
            } catch (e) {}
            return false;
        }
        if (ClientInfo.isNetscape() || ClientInfo.isFirefox() || (ClientInfo.isMacOSX() && ClientInfo.isSafari())) {
            var found = false;
            navigator.plugins.refresh(false);
            for (var idx = 0; (!found) && (idx < navigator.plugins.length); idx++) {
                var plugIn = navigator.plugins[idx];
                var mimeCount = plugIn.length;
                for (mimeIdx = 0; (!found) && (mimeIdx < mimeCount); mimeIdx++) {
                    if (plugIn[mimeIdx].type == mimeType) {
                        found = true;
                    }
                }
            }
            return found;
        }
        return false;
    },
    isCorrectZone: function(activeX) {
        if (ClientInfo.isIE() && ClientInfo.isWindows()) {
            try {
                var obj = new ActiveXObject(activeX);
                if (obj != null) {
                    obj.Launch = true;
                    var propertyChanged = obj.Launch;
                    if (propertyChanged) {
                        return true;
                    }
                 }
            } catch (e) {
            }
            return false;
        }
        return true; // Zone doesn't matter on non-IE browsers
    },
    getVersion: function(activeX) {
        // Only call if native is detected
        if (ClientInfo.isIE() && ClientInfo.isWindows()) {
            try {
                var obj = new ActiveXObject(activeX);
                if (obj != null) {
                    var version = obj.ClientVersion;
                    if (version != null && version != "") {
                        return version;
                    }
                }
            } catch (e) {
            }
            return null;
        }
        if (ClientInfo.isNetscape() || ClientInfo.isFirefox() || (ClientInfo.isMacOSX() && ClientInfo.isSafari())) {
            try {
                var version = document.IcaObj.GetPropValue("ClientVersion");
                if (version != null && version != "") {
                    return version;
                }
            } catch (e) {}
            return null;
        }
        return null; // No version found
    },
    isPassThrough: function(activeX) {
        if (ClientInfo.isWindows()) {
            // Pass-through mode is only relevant for Windows OSes
            if (ClientInfo.isIE()) {
                // Use ActiveX for IE
                try {
                    var obj = new ActiveXObject(activeX);
                    return obj.IsPassThrough();
                } catch (e) {}
                return null;
            } else if (ClientInfo.isNetscape() || ClientInfo.isFirefox()) {
                // Use the browser plug-in
                try {
                    return document.IcaObj.IsPassThrough();
                } catch (e) {}
                return null;
            } else {
                // For other Windows browsers, assume pass-through to disable workspace control
                return true;
            }
        } else {
            // Non-Windows OS - this will never be pass-through
            return false;
        }
    }
};

var ICA = {
    _mimeType: "application/x-ica",
    _activeX: "Citrix.ICAClient",
    init: function() {
        _CitrixClient.init(ICA._mimeType);
    },
    isInstalled: function() {
        return _CitrixClient.isInstalled(ICA._activeX, ICA._mimeType);
    },
    isCorrectZone: function() {
        return _CitrixClient.isCorrectZone(ICA._activeX);
    },
    getVersion: function() {
        return _CitrixClient.getVersion(ICA._activeX);
    },
    isPassThrough: function() {
        return _CitrixClient.isPassThrough(ICA._activeX);
    },
    getDetails: function() {
        var result = {
            available: null,
            detected: null,
            version: null,
            correctZone: null,
            passthrough: null
        };
        ICA.init();
        result.detected = ICA.isInstalled();
        result.available = result.detected;
        if (result.available) {
            result.correctZone = ICA.isCorrectZone();
            result.version = ICA.getVersion();
            result.passthrough = ICA.isPassThrough();
        }
        return result;
    }
};

var Streaming = {
    _mimeType: "application/x-ctxrade",
    _activeX: "Rco.RadeClient",
    init: function() {
        _CitrixClient.init(Streaming._mimeType);
    },
    isInstalled: function() {
        return _CitrixClient.isInstalled(Streaming._activeX, Streaming._mimeType);
    },
    isCorrectZone: function() {
        return _CitrixClient.isCorrectZone(Streaming._activeX);
    },
    getVersion: function() {
        var version = _CitrixClient.getVersion(Streaming._activeX);
        if (version != null && version.indexOf("4.5.") == 0) {
            version = "1.0.0.0";
        }
        return version;
    },
    getDetails: function() {
        var result = {
            available: null,
            detected: null,
            correctZone: null,
            version: null
        };
        Streaming.init();
        result.detected = Streaming.isInstalled();
        result.correctZone = Streaming.isCorrectZone();
        result.available = result.detected && result.correctZone;
        if (result.available) {
            result.version = Streaming.getVersion();
        }
        return result;
    }
};

var Java = {
    init: function() {
        if (ClientInfo.isIE()) {
            document.body.id = 'oClientCaps';
            document.body.style.behavior = 'url(#default#clientCaps)';
        }
    },
    isInstalled: function() {
        if (ClientInfo.isIE()) {
            try {
                if (oClientCaps) {
                    var installed = oClientCaps.isComponentInstalled("{08B0E5C0-4FCB-11CF-AAA5-00401C608500}", "ComponentID");
                    return installed && oClientCaps.javaEnabled;
                }
            } catch (e) {}
            return false;
        }
        if (ClientInfo.isNetscape() || ClientInfo.isFirefox()) {
            if (navigator.plugins) {
               navigator.plugins.refresh(false);
               // Check to find a java plugin
               for (var i=0; i < navigator.plugins.length; i++) {
                    var plugin = navigator.plugins[i];
                    if (plugin && plugin.name.toLowerCase().indexOf('java') != -1) {
                        // Check if this java plugin has a mime type for applets
                        for (var j=0; j < plugin.length; j++) {
                            var mimetype = plugin[j];
                            if (mimetype && mimetype.type && mimetype.type.toLowerCase().indexOf('java-applet') != -1) {
                                return true; // We have java applet support
                            }
                        }
                    }
                }
            }
        }
        try {
            if (navigator && navigator.javaEnabled) {
                return navigator.javaEnabled();
            }
        } catch(e) {}
        return false;
    },
    getDetails: function() {
        var result = {
            available: null,
            javaInstalled: null,
            popupsEnabled: null
        };
        Java.init();
        result.javaInstalled = Java.isInstalled();
        result.popupsEnabled = DetectionUtils.popupsEnabled();
        result.available = result.javaInstalled && result.popupsEnabled;
        return result;
    }
};

var RDP = {
    _classID: "4EB89FF4-7F78-4A0F-8B8D-2BF02E94E4B2", // RDP 6.0
    getClassID: function() {
        try {
            rdpClient = new ActiveXObject("MsRdp.MsRdp.5");
            if (rdpClient != null) {
                return RDP._classID;
            }
        } catch (e) {}
        return null;
    },
    isCorrectZone: function() {
        var hiddenDiv = document.getElementById("hiddenDiv");
        hiddenDiv.innerHTML = "<object classid=clsid:" + RDP._classID + " id='rdpobj' height='0' width='0' border='0'></object>";
        if (document.rdpobj != null) {
            if (document.rdpobj.SecuredSettingsEnabled) {
                return true;
            }
        }
        return false;
    },
    getDetails: function() {
        var result = {
            available: null,
            classID: null,
            correctZone: null
        };
        result.classID = RDP.getClassID();
        result.correctZone = RDP.isCorrectZone();
        result.available = result.classID && result.correctZone;
        return result;
    }
};

<%
// Detects clients based on the parameters passed to the "run" function.
%>
var SilentDetection = {
    run: function (remoteClientToDetect, useJavaFallback, detectStreamingClient, serverRemoteClientVersion, serverStreamingClientVersion) {
        var client;
        var correctZone = true;
        var canUpgradeRemote = false;
        var result = {
            remoteClient: "",
            streamingClient: "",
            icoStatus: "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>",
            rdpClassId: null
        }

        // Always use ICO to determine whether this is a pass-through session
        client = ICA.getDetails();
        if (client.available && (client.passthrough != null)) {
            result.icoStatus = client.passthrough ? "<%=WizardConstants.VAL_ICO_IS_PASS_THROUGH%>" : "<%=WizardConstants.VAL_ICO_NOT_PASS_THROUGH%>";
        }

        switch (remoteClientToDetect) {
            case "<%=WizardConstants.NATIVE%>":
                if (client.available) {
                    result.remoteClient = "<%=WizardConstants.NATIVE%>";
                    correctZone = client.correctZone;
                    if (client.version) {
                        canUpgradeRemote = !isUpToDateVersion(client.version, serverRemoteClientVersion);
                    }
                } else if (useJavaFallback) {
                    client = Java.getDetails();
                    if (client.available) {
                        result.remoteClient = "<%=WizardConstants.JAVA%>";
                    }
                }
                break;
            case "<%=WizardConstants.JAVA%>":
                client = Java.getDetails();
                if (client.available) {
                    result.remoteClient = "<%=WizardConstants.JAVA%>";
                }
                break;
            case "<%=WizardConstants.RDP%>":
                client = RDP.getDetails();
                if (client.available) {
                    result.remoteClient = "<%=WizardConstants.RDP%>";
                    correctZone = client.correctZone;
                    result.rdpClassId = client.classID;
                }
                break;
        }

        if (result.remoteClient) {
            // There is no opportunity for the user to "force" a client during silent detection
            result.remoteClient += "=<%=WizardConstants.AUTO%>";

            if (canUpgradeRemote) {
                result.remoteClient += ",<%=WizardConstants.UPGRADEABLE%>";
            }
            if (!correctZone) {
                result.remoteClient += ",<%=WizardConstants.INCORRECT_ZONE%>";
            }
        }

        if (detectStreamingClient) {
            client = Streaming.getDetails();
            if (client.available) {
                result.streamingClient = "<%=WizardConstants.RADE%>=<%=WizardConstants.AUTO%>";
                if (client.version && !isUpToDateVersion(client.version, serverStreamingClientVersion)) {
                    result.streamingClient += ",<%=WizardConstants.UPGRADEABLE%>";
                }
            }
        }

        return result;
    }
};
