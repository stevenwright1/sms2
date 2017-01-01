// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.

using com.citrix.wi.mvc;

namespace com.citrix.wi.pages.auth
{
    /// <summary>
    /// Controller for Certificate Error error page.
    /// </summary>
    public class CertificateError : AuthenticationErrorBase {

        public CertificateError(WIContext wiContext) : base(wiContext) {
        }

        override public System.String getBrowserPageTitleKey() {
            return "CertificateErrorTitle";
        }
    }
}
