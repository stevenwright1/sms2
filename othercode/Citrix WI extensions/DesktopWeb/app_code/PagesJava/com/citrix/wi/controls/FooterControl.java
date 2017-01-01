/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.util.ClientInfoUtilities;

/**
 * Maintains presentation state for the footer control.
 */
public class FooterControl {
    private String footerText = "";
    private static final String FOOTER_LINK_APPS = "http://www.citrix.com";
    private static final String FOOTER_LINK_DESKTOPS = "http://www.citrix.com/xendesktop";
    private static final String HDX_LINK = "http://hdx.citrix.com";

    /**
     * Gets the footer text
     */
    public String getFooterText() {
        return footerText;
    }

    /**
     * Sets the footer text
     * @param value the footer text
     */
    public void setFooterText(String value) {
        footerText = value;
    }

    /**
     * Gets the relative path to the footer logo image.
     *
     * @return the footer image path
     */
    public String getFooterImg(WIContext wiContext, LayoutControl layoutControl) {
        String img = (layoutControl.isLoggedOutPage) ? "CitrixLogoDarkLoggedOff.png" : "CitrixWatermark.png";
        return ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/" + img);
    }

    /**
     * Gets the relative path to the footer HDX image.
     *
     * @return the footer HDX image path
     */
    public String getHdxImg(WIContext wiContext, LayoutControl layoutControl) {
        String img = (layoutControl.isLoggedOutPage) ? "HDXLoggedOff.png" : "HDX.png";
        return ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/" + img);
    }

    /**
     * Gets the alt text to use for the footer logo image.
     *
     * @return the footer logo image alt text
     */
    public String getFooterImgAltText(WIContext wiContext) {
        return wiContext.getString((Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) ? "FooterImgAltTextDesktops" : "FooterImgAltTextApps");
    }

    /**
     * Gets the alt text to use for the footer HDX image.
     *
     * @return the footer HDX image alt text
     */
    public String getHdxImgAltText(WIContext wiContext) {
        return wiContext.getString("HdxImgAltText");
    }

    /**
     * Gets the URL for the footer logo link.
     *
     * @return the footer logo link
     */
    public String getFooterImgLink(WIContext wiContext) {
        return (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) ? FOOTER_LINK_DESKTOPS : FOOTER_LINK_APPS;
    }

    /**
     * Gets the URL for the footer HDX link.
     *
     * @return the footer HDX link
     */
    public String getHdxImgLink(WIContext wiContext) {
        return HDX_LINK;
    }

}
