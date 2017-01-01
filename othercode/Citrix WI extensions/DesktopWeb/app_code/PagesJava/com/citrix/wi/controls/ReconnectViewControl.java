package com.citrix.wi.controls;

public class ReconnectViewControl {

    private String bufferedJavascript = "";
    private String bufferedFrameset = "";
    private String redirectUrl = null;

    public String getBufferedFrameset() {
        return bufferedFrameset;
    }
    public void setBufferedFrameset(String bufferedFrameset) {
        this.bufferedFrameset = bufferedFrameset;
    }
    public String getBufferedJavascript() {
        return bufferedJavascript;
    }
    public void setBufferedJavascript(String bufferedJavascript) {
        this.bufferedJavascript = bufferedJavascript;
    }
    public String getRedirectUrl() {
        return redirectUrl;
    }
    public void setRedirectUrl(String url) {
        redirectUrl = url;
    }
}
