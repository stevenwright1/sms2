/*
 * PlatformSpecificUtils.java
 * Copyright (c) 2006 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.mvc;

import com.citrix.authenticators.IPasswordCachingSecurIDAuthenticator;
import com.citrix.authenticators.ISafewordAuthenticator;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.wi.accountselfservice.ContextFactory;
import com.citrix.wi.authservice.ASClient;
import com.citrix.wi.authservice.ASCommunicationException;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.StaticEnvironmentAdaptor;
import java.io.IOException;

/**
 * Platform-specific utility methods.
 */
public interface PlatformSpecificUtils {
    /**
     * Checks for Session fixation and redirects to the appropriate page if found.
     * @param wiContext
     * @return true if session fixation found
     * @throws IOException
     */
    public boolean checkForSessionFixation(WIContext wiContext) throws IOException;

    /**
     * Expires a Web Interface authentication cookie.
     *
     * @param wiContext the Web Interface context
     * @param authCookieName the name of the authentication cookie
     */
    public void expireAuthCookie(WIContext wiContext, String authCookieName);

    /**
     * Creates a Web Interface authentication cookie and adds it to the response.
     *
     * @param wiContext the Web Interface context
     * @param authCookieName the name of the authentication cookie
     */
    public String addAuthCookie(WIContext wiContext, String authCookieName);

    /**
     * Gets a factory for creating Account Self-Service Context objects (used
     * for communication with Password Manager).
     * 
     * @return a ContextFactory object
     */
    public ContextFactory getAccountSSContextFactory();

    /**
     * Returns an object that can communicate with the given Access Gateway
     * Authentication Service endpoint URL.
     *
     * @param url The URL string of the endpoint to contact
     * @param envAdaptor The static environment adaptor
     * @return an <code>ASClient</code>
     * @throws ASCommunicationException if the URL was not a valid endpoint
     */
    public ASClient getASClient(String url, StaticEnvironmentAdaptor envAdaptor) throws ASCommunicationException;

    /**
     * Gets a new instance of an authenticator object for use with Safeword
     * two-factor authentication.
     * 
     * @return a new instance of <code>ISafewordAuthenticator</code>
     */
    public ISafewordAuthenticator getNewSafewordAuthenticator();

    /**
     * Gets a new instance of an authenticator object for use with SecurID
     * two-factor authentication.
     * 
     * @return a new instance of <code>ISecurIDAuthenticator</code>
     */
    public ISecurIDAuthenticator getNewSecurIDAuthenticator();

    /**
     * Gets a new instance of an authenticator object that supports password
     * caching, for use with SecurID two-factor authentication.
     * 
     * @return a new instance of <code>IPasswordCachingSecurIDAuthenticator</code>
     */
    public IPasswordCachingSecurIDAuthenticator getNewPasswordCachingSecurIDAuthenticator();

    /**
     * Logs the user out of the configured federation system (e.g. AD FS) as
     * well as the Web Interface.
     * 
     * @param wiContext the Web Interface context
     * @return <code>null</code> if the operation was a success; otherwise, a <code>StatusMessage</code>
     * object describing the error.
     */
    public StatusMessage doFederatedLogout(WIContext wiContext);

    /**
     * Stores a logout ticket for Access Gateway in the application state.
     * 
     * The logout ticket is stored in the application state along with a
     * reference to the web server session that is associated with it.
     * 
     * @param wiContext the Web Interface context
     * @param logoutTicket the logout ticket to store
     */
    public void storeAGLogoutTicket(WIContext wiContext, String logoutTicket);

    /**
     * Abandons a Web Interface session.
     * 
     * This is used to synchronize the termination of an Access Gateway
     * session with a Web Interface session. The logout ticket parameter is
     * provided by the Access Gateway and matches the ticket that was generated
     * during the single sign-on process with Web Interface.
     * 
     * @param wiContext the Web Interface context
     * @param logoutTicket the logout ticket associated with the session to abandon
     */
    public void abandonSession(WIContext wiContext, String logoutTicket);
}
