/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.authentication.web.AuthUtilities;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wing.util.WebUtilities;

/**
 * Support for a session token to help protected against Cross Site Request Forgery (CSRF) attacks.
 */
public class SessionToken {

    /**
     * Name of token within session.
     */
    private static final String sessionTokenName = "sessionToken";

    /**
     * Name used for token within forms.
     */
    public static final String ID_FORM = "SESSION_TOKEN";

    /**
     * Named used for GET query strings.
     */
    public static final String QSTR_TOKENNAME = "CTX_Token";

    /**
     * Get (or create) the session token.
     * @param wiContext The context.
     * @return The session token.
     */
    public static String get(WIContext wiContext)
    {
        WebAbstraction web = wiContext.getWebAbstraction();
        String sessionToken = get(web);
        return sessionToken;
    }

    /**
     * Get (or create) the session token.
     * @param web The web abstraction.
     * @return The session token.
     */
    public static String get(WebAbstraction web) {
        // Recall any existing token.
        String sessionToken = (String)web.getSessionAttribute(sessionTokenName);

        // If no existing token...
        if (sessionToken == null) {
            // ...generate and remember new token.
            sessionToken = AuthUtilities.createRandomString();
            web.setSessionAttribute(sessionTokenName, sessionToken);
        }

        return sessionToken;
    }

    /**
     * Check for form token, if present and correct return true, if absent or
     * incorrect redirect to error page and return false.
     * @param wiContext The context.
     * @return true if processing should continue.
     */
    public static boolean guard(WIContext wiContext) {
        boolean result = verifyFormCsrfSafe(wiContext);

        if (!result) {
            csrfDetectedRedirect(wiContext.getWebAbstraction());
        }

        return result;
    }

    /**
     * Perform a redirect when an attack is detected.
     */
    private static void csrfDetectedRedirect(WebAbstraction web) {
        web.clientRedirectToContextUrl("/html/stateError.html");
    }

    /**
     * Get session token from request query string.
     * @param web The web abstraction.
     * @return The token, if present, or null if not.
     */
    private static String getCsrfQueryToken(WebAbstraction web) {
        String token = web.getQueryStringParameter(QSTR_TOKENNAME); 
        // The CSRF token is comprised of hex digits so escaping it should be a no-op.
        // If the escaped token differs, it implies the token has been tampered with.
        if (token != null && token.equals(WebUtilities.escapeHTML(token))) {
            return token;
        } else {
            return null;
        }
    }

    /**
     * Check that request carries correct token.
     */
    public static boolean checkQueryValid(WebAbstraction web) {
        String query = web.getQueryStringParameter(QSTR_TOKENNAME);
        String token = get(web);

        boolean valid = token != null && query != null &&
            token.compareToIgnoreCase(query) == 0;

        if (!valid) {
            csrfDetectedRedirect(web);
        }

        return valid;
    }

    /**
     * Create a token to add to a request query string.
     * @param wiContext The context.
     * @return The query string token, like "&name=value".
     */
    public static String makeCsrfQueryToken(WIContext wiContext) {
        String query;

        // When persistent URLs are enabled we do not protection, so...
        if (wiContext.getConfiguration().getEnablePassthroughURLs()) {
            // ...don't make a token.
            query = "";
        }
        else {
            // Make a token.
            String token = SessionToken.get(wiContext);
            query = "&" + QSTR_TOKENNAME + "=" + token;
        }

        return query;
    }

    /**
     * Create a new query string token from one in request, if present.
     * @param wiContext The context.
     * @return Query string fragment, "&token=value" if incoming token
     * present, zero length string if not.
     */
    public static String copyCsrfQueryToken(WIContext wiContext) {
        String query;

        String token = getCsrfQueryToken(wiContext.getWebAbstraction());
        if (token == null) {
            query = "";
        }
        else {
            query = "&" + QSTR_TOKENNAME + "=" + token;
        }

        return query;
    }

    /**
     * Check whether query string token matches expected token.
     * @param wiContext The context.
     * @return true iff session token matches expectations, or is not required.
     */
    public static boolean verifyCsrfSafe(WIContext wiContext) {
        boolean safe;

        // With passthrough...
        if (wiContext.getConfiguration().getEnablePassthroughURLs()) {
            // ...we have not protection and there is no check for safety.
            safe = true;
        }
        else {
            // Otherwise, things must seriously match.
            WebAbstraction web = wiContext.getWebAbstraction();
            String sessionToken = get(web);
            String queryToken = getCsrfQueryToken(web);
            safe = sessionToken != null && queryToken != null &&
                sessionToken.equalsIgnoreCase(queryToken);
        }

        return safe;
    }

    /**
     * Check whether POSTed form data string token matches expected token.
     * @param wiContext The context.
     * @return true iff session token matches expectations, or request is not a POST.
     */
    public static boolean verifyFormCsrfSafe(WIContext wiContext) {
        boolean result;

        WebAbstraction web = wiContext.getWebAbstraction();

        // Only interested in POST requests.
        if (web.isPostRequest())
        {
            // Check token on form and token in session are same.
            String formToken = web.getFormParameter(ID_FORM);
            String sessionToken = get(web);

            if (formToken != null &&
                formToken.equalsIgnoreCase(sessionToken)) {
                result = true;
            }
            else {
                result = false;
            }
        }
        else {
            // Not really interested in GETs, etc.
            result = true;
        }

        return result;

    }

}
