// Copyright (c) 2004 - 2010 Citrix Systems, Inc. All Rights Reserved.

using com.citrix.wi.mvc;

namespace com.citrix.wi.pages.auth
{
    /// <summary>
    /// Controller for Access Denied error page.
    /// </summary>
    public class AccessDeniedError : AuthenticationErrorBase
    {
        public AccessDeniedError(WIContext wiContext) : base(wiContext) {
        }

        override public System.String getBrowserPageTitleKey() {
            return "AccessDeniedErrorTitle";
        }
    }
}
