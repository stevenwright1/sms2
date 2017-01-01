<%
// sessionSettings.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function click_standard_size(setForm) {
    var elWindowSize = setForm.<%=Constants.ID_OPTION_WINDOW_SIZE%>;
    if (!elWindowSize) {
        return;
    }
    var customDisabled  = (elWindowSize.value != "<%=AppDisplaySizePreference.CUSTOM.getType()%>");
    var percentDisabled = (elWindowSize.value != "<%=AppDisplaySizePreference.PERCENT.getType()%>");

    setForm.<%=Constants.ID_TEXT_DESIRED_HRES%>.disabled = customDisabled;
    setForm.<%=Constants.ID_TEXT_DESIRED_VRES%>.disabled = customDisabled;
    setForm.<%=Constants.ID_TEXT_SCREEN_PERCENT%>.disabled = percentDisabled;
    
<%
if (! wiContext.getClientInfo().osWinCE()) {
%>
    document.getElementById("<%=Constants.ID_LABEL_WINSIZE_CUSTOM%>").disabled = customDisabled;
    document.getElementById("<%=Constants.ID_SPAN_WINSIZE_CUSTOM%>").disabled = customDisabled;
    document.getElementById("<%=Constants.ID_LABEL_WINSIZE_PERCENT%>").disabled = percentDisabled;
    document.getElementById("<%=Constants.ID_SPAN_WINSIZE_PERCENT%>").disabled = percentDisabled;
<%
}
%>

}

function onChangeBandwidth(f) {
    var bwElement = f.<%=Constants.ID_OPTION_BANDWIDTH%>;
    if (!bwElement) {
        return;
    }

    var bw = bwElement.value;
    if (bw == "<%=Utils.toString(BandwidthProfilePreference.CUSTOM)%>") {
        // No bandwidth selected, enable all html elements in the performance section.
        setColorElemEnabled(f, true);
        setAudioElemEnabled(f, true);
        setPrinterElemEnabled(f, true);
    } else {
        var colorSelected = 0;
        var audioSelected = 0;
        var printerChecked = false;
        if (bw == "<%=Utils.toString(BandwidthProfilePreference.HIGH)%>") {
            colorSelected = 2;
            audioSelected = 4;
            printerChecked = true;
        } else if (bw == "<%=Utils.toString(BandwidthProfilePreference.MEDIUM_HIGH)%>") {
            colorSelected = 1;
            audioSelected = 1;
            printerChecked = true;
        } else if (bw == "<%=Utils.toString(BandwidthProfilePreference.MEDIUM)%>") {
            colorSelected = 1;
            audioSelected = 1;
            printerChecked = false;
        } else if (bw == "<%=Utils.toString(BandwidthProfilePreference.LOW)%>") {
            colorSelected = 1;
            audioSelected = 1;
            printerChecked = false;
        }

        if (f.<%=Constants.ID_OPTION_WINDOW_COLOR%>) {
            f.<%=Constants.ID_OPTION_WINDOW_COLOR%>.options[colorSelected].selected = true;
            setColorElemEnabled(f, false);
        }

        if (f.<%=Constants.ID_OPTION_AUDIO%>) {
            f.<%=Constants.ID_OPTION_AUDIO%>.options[audioSelected].selected = true;
            setAudioElemEnabled(f, false);
        }

        if (f.<%=Constants.ID_CHECK_PRINTER%>) {
            f.<%=Constants.ID_CHECK_PRINTER%>.checked = printerChecked;
            setPrinterElemEnabled(f, false);
        }
    }
}

function setColorElemEnabled(f, v) {
    if (!f.<%=Constants.ID_OPTION_WINDOW_COLOR%>) {
        return;
    }
    f.<%=Constants.ID_OPTION_WINDOW_COLOR%>.disabled = !v;
    
<% if (! wiContext.getClientInfo().osWinCE()) { %>
        document.getElementById("lblWinColor").disabled = !v;
<% } %>
}

function setAudioElemEnabled(f, v) {
    if (!f.<%=Constants.ID_OPTION_AUDIO%>) {
        return;
    }
    f.<%=Constants.ID_OPTION_AUDIO%>.disabled = !v;
    
<% if (! wiContext.getClientInfo().osWinCE()) { %>
        document.getElementById("lblAudio").disabled = !v;
<% } %>
}

function setPrinterElemEnabled(f, v) {
    if (!f.<%=Constants.ID_CHECK_PRINTER%>) {
        return;
    }
    f.<%=Constants.ID_CHECK_PRINTER%>.disabled = !v;
    
<% if (! wiContext.getClientInfo().osWinCE()) { %>
        document.getElementById("lblPrinter").disabled = !v;
<% } %>
}
