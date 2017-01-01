/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.util.Platform;

/**
 * A class that represents the features enabled on the platform the code is
 * running on
 */
public class Features {
    private Features() {
    }

    public static boolean isSecurIDSupported() {
        return Platform.isDotNet();
    }

    public static boolean isNativeSecurIDSafeWordSupported() {
        return Platform.isDotNet();
    }

    public static boolean isNDSSupported() {
        return Platform.isDotNet();
    }

    public static boolean isSSOSupported() {
        return Platform.isDotNet();
    }

}