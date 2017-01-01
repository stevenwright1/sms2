/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controls;


/**
 * Maintains the state for the style sheet page.
 */
public class StyleControl {

    public static class Style {
        public static final Style FULL     = new Style("full");
        public static final Style LOW      = new Style("low");
        public static final Style EMBEDDED = new Style("embedded");

        private String name;

        private Style(String name) {
            this.name = name;
        }

        public String toString() {
            return name;
        }
    }

    public Style currentStyle = Style.FULL;

    public boolean showFullStyle() {
        return currentStyle == Style.FULL;
    }

    public boolean showLowGrahicsStyle() {
        return currentStyle == Style.LOW || currentStyle == Style.EMBEDDED;
    }

    public boolean showEmbeddedStyle() {
        return currentStyle == Style.EMBEDDED;
    }
}