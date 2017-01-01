/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

/**
 * Utility class for retrieving XHTML markup strings
 */
public class Markup {

    /**
     * Returns the selected attribute in XHTML
     */
    public static final String selectedStr (boolean val) {
        return val ? "selected=\"selected\"" : "";
    }

    /**
     * Returns the disabled attribute in XHTML
     */
    public static final String disabledStr (boolean val) {
        return val ? "disabled=\"disabled\"" : "";
    }

    /**
     * Returns the checked attribute in XHTML
     */
    public static final String checkedStr (boolean val) {
        return val ? "checked=\"checked\"" : "";
    }

    /**
     * Style for labels whose fields containing invalid data.
     */
    public static final String invalidFieldLabelClass = "invalidField";

    /**
     * Prefix for labels whose fields containing invalid data.
     */
    public static final String invalidFieldLabelPrefix = "*";
}
