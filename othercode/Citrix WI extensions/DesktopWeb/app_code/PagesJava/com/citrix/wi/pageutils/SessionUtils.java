/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.net.UnknownHostException;
import java.util.List;
import java.util.Map;
import java.util.Set;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthMethod;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.AGAuthenticationMethod;
import com.citrix.wi.types.IcaAuthType;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.AppAttributeKey;
import com.citrix.wi.util.RoamingUserManager;
import com.citrix.wing.AccessPrefs;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.DeviceInfo;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.types.AccessConditions;
import com.citrix.wing.util.MPSClientNames;
import com.citrix.wing.webpn.FarmBinding;
import com.citrix.wing.webpn.MPSFarm;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.webpn.WebPN;
import com.citrix.wing.webpn.WebPNBuilder;
import com.citrix.wi.controls.DesktopGroupsController;

/**
 * This file contains the logic to initialize and maintain session state. This
 * involves: - Detecting & initializing new sessions. - Populating page-scope
 * variables containing session information.
 */
public class SessionUtils {
    /**
     * Gets the User Context for a request / response pair
     * <P>
     * The User Context is the class used by the scripts to access & launch
     * resources on behalf of the user.
     * </P>
     * <P>
     * This method will attempt to recover the user context from the session
     * state, if that's not possible it assumes this is a new session and
     * re-initializes the user context.
     * </P>
     * <P>
     * Pages must only call this method once. In order to persist any changes to
     * the user context, the page must call sessReturnUserContext before any
     * action is performed that will cause the response to be sent to the
     * browser.
     * </P>
     *
     * @param wiContext WIContext.
     * @return A UserContext instance.
     */
    public static UserContext checkOutUserContext(WIContext wiContext) throws UnknownHostException {
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();

        WebAbstraction web = wiContext.getWebAbstraction();
        WebPNBuilder builder = (WebPNBuilder)web.getApplicationAttribute(AppAttributeKey.WEBPN_BUILDER);

        WebPN webPN = DisasterRecoveryUtils.getWebPN(wiContext);

        // Try to recover the user context. Failing that, create a new one.
        UserContext userCtxt = builder.recoverUserContext(webPN, envAdaptor);
        if (userCtxt == null) {
            userCtxt = createNewUserContext(wiContext);

            // If the UserContext could not be created or recovered, then
            // something's gone seriously wrong with the session state and we
            // should return the null value to cause an exception higher up the
            // stack.

        } else {
            // Always update the access prefs in case the configuration
            // or cookies have changed (and it's a cheap operation)
            wiContext.getUserPreferences().updateAccessPrefs(userCtxt.getAccessPrefs());
        }

        return userCtxt;
    }

    /**
     * Commits the UserContext to persistent storage. Returning the UserContext
     * causes the information in it to be persisted, possibly to the response
     * cookies.
     *
     * @param userContext The UserContext instance to persist.
     */
    public static void returnUserContext(UserContext userContext) {
        UserEnvironmentAdaptor adaptor = userContext.getEnvironmentAdaptor();

        userContext.close();
        adaptor.commitState();
        adaptor.destroy();
    }

    /**
     * Initialize the UserContext for a new session. This method is called when
     * it is determined that a new session is being created.
     *
     * @param wiContext WIContext.
     * @param userCtxt The user context to be initialized.
     * @param accessToken The identity / credentials used to authenticate to WI.
     * @param accessConditions Null on J2EE
     * @param useRoaming Whether or not the roaming user functionality is
     * enabled for this UserContext.
     */
    private static void initializeNewSession(WIContext wiContext, UserContext userCtxt, AccessToken accessToken,
                    AccessConditions accessConditions, boolean useRoaming) throws UnknownHostException {

        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // Modify the code below to perform any per-session initialization.
        // Typical activities that can be done are:
        // * Extracting the actual credentials to use for accessing resources
        // from a credential store,
        // * Only choosing to use a subset of the configured farms.
        // * Using an alternative strategy for generating the client name.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

        // Here we determine the set of farms that will be used for this user,
        // and the credentials that should be used for each farm. WI's default
        // behavior is to use the user's authentication credentials to access
        // all of the farms.
        WebPN webPN = userCtxt.getWebPN();
        MPSFarm[] availableFarms = webPN.getMPSFarms();
        Set activeFarms = userCtxt.getActiveFarms();

        SIDBasedToken sidToken = null;
        if (accessToken instanceof SIDBasedToken) {
            sidToken = (SIDBasedToken)accessToken;
        }

        // When the Roaming User feature is enabled, determine the user's "home farms" by comparing the
        // user's group SIDs against the list of SIDs configured for each of the available farms.
        if (useRoaming && sidToken != null) {
            RoamingUserManager rum = (RoamingUserManager)wiContext.getStaticEnvironmentAdaptor().getApplicationAttribute(AppAttributeKey.ROAMING_USER_MANAGER);
            Set homeFarms = rum.getHomeFarmBindings(sidToken);
            activeFarms.addAll(homeFarms);
        } else {
            // Roaming User is not enabled or the token is not a SID-based token - just bind all the available farms
            for (int i = 0; i < availableFarms.length; ++i) {
                activeFarms.add(new FarmBinding(availableFarms[i].getName(), accessToken));
            }
        }

        if (accessConditions != null) {
            userCtxt.setAccessConditions(accessConditions);
        }

        // Set the access prefs for this session.
        //
        // The access prefs are first restricted by the configuration to ensure
        // there is no attempt by the user to bypass admin control.
        AccessPrefs accessPrefs = userCtxt.getAccessPrefs();

        wiContext.getUserPreferences().updateAccessPrefs(accessPrefs);

        IcaAuthType authToMPS = null;
        WIAuthType logonType = Authentication.getLogonType(wiContext.getUserEnvironmentAdaptor());
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        AuthPoint authPoint = authConfig.getAuthPoint();
        Boolean smartCardAuth = Boolean.FALSE;

        if ((logonType != null) && (authPoint instanceof WIAuthPoint)) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authPoint;
            AuthMethod authMethod = wiAuthPoint.getAuthMethod(logonType);

            if (authMethod != null) {
                authToMPS = authMethod.getAuthToICAServer();

                smartCardAuth = new Boolean(authToMPS == IcaAuthType.SMARTCARD
                                || authToMPS == IcaAuthType.SMARTCARD_PASSTHROUGH);
            }
        } else if (authPoint instanceof AGAuthPoint) {
            AGAuthPoint agAuthPoint = (AGAuthPoint)authPoint;
            AGAuthenticationMethod authMethod = agAuthPoint.getAuthenticationMethod();

            if (authMethod != null) {
                smartCardAuth = new Boolean(authMethod == AGAuthenticationMethod.SMART_CARD);
            }
        }

        accessPrefs.setSmartCardAuth(smartCardAuth);

        accessPrefs.setPassthroughAuth(new Boolean(authToMPS == IcaAuthType.PASSTHROUGH
                        || authToMPS == IcaAuthType.SMARTCARD_PASSTHROUGH
                        || authToMPS == IcaAuthType.KERBEROS_PASSTHROUGH));

        accessPrefs.setSSPIAuth(new Boolean(authToMPS == IcaAuthType.KERBEROS_PASSTHROUGH));

        // Set the device info for this session.
        //
        // deviceState is the content of the persistent cookie held at the browser.
        // deviceInfo is the info about the device held in the current session.

        DeviceInfo deviceInfo = userCtxt.getClientDevice();
        deviceInfo.setDetectedAddress(Include.getClientAddress(wiContext));

        String deviceId = null;
        String clientName = null;

        // if AGE provides client name, use it for both client name and device id
        String ageClientName = AGEUtilities.getAGEClientName(wiContext);
        if (ageClientName != null) {
            deviceId = ageClientName;
            clientName = ageClientName;
        } else {
            deviceId = retrieveDeviceId(userCtxt);
            clientName = getClientName(wiContext, accessToken, deviceId);
        }

        deviceInfo.setDeviceId(deviceId);
        deviceInfo.setClientName(clientName);


        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // The supplied address can be set here. You might need this if you have
        // a trusted reverse-proxy (or similar) that puts the real client in a
        // HTTP header. If you do this, remember to also call
        // setTrustClientSuppliedAddresses(true) on the 'global' part of the
        // core configuration (in startup.jsp). For example:
        //   deviceInfo.setSuppliedAddress( ... );
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

        // Override the default screen size if we've been able to detect the actual size
        int[] screenSize = UIUtils.getCookieScreenRes(wiContext);
        int screenHRES = screenSize[0];
        int screenVRES = screenSize[1];
        if ((screenHRES > 0) && (screenVRES > 0)) {
            deviceInfo.setDisplayWidth(screenHRES);
            deviceInfo.setDisplayHeight(screenVRES);
        }
    }

    /**
     * Retrieves the deviceId from the cookie. If there is no cookie stored, generate the deviceId and store it.
     * 
     * @param userCtxt the user context
     * @return the retrieved deviceId
     */
    private static String retrieveDeviceId(UserContext userCtxt) {
        Map deviceState = userCtxt.getEnvironmentAdaptor().getDeviceState();
        String deviceId = (String)deviceState.get(Constants.COOKIE_DEVICE_ID);

        if (!MPSClientNames.isValidUniqueName(deviceId) || DeviceInfo.DEFAULT_NAME.equals(deviceId)) {
            deviceId = MPSClientNames.generateUnique();
            // Remember for future sessions
            deviceState.put(Constants.COOKIE_DEVICE_ID, deviceId);
        }

        return deviceId;
    }

    /**
     * Gets the client name, which is generated from access token, when workspace control is off, or
     * equal to deviceId in all other cases.
     * 
     * @param wiContext the context
     * @param accessToken The identity / credentials used to authenticate to WI
     * @param deviceId the unique device identifier
     * @return the client name
     */
    private static String getClientName(WIContext wiContext,AccessToken accessToken, String deviceId) {
        // Use the old client name format when Workspace Control is disabled for compatibility.
        // (MPS/U and MF1.8 need the old format to do roaming reconnection).
        String clientName = wiContext.getConfiguration().getWorkspaceControlConfiguration().getEnabled() ?
                                                             deviceId :
                                                             MPSClientNames.fromAccessToken(accessToken); 
        return clientName;
    }

    /**
     * Creates a new UserContext object and stores it in the session.
     * The created UserContext will support home farms and disaster recovery if these features are enabled.
     * @param wiContext the context
     * @return <code>true</code> if the UserContext was successfully created, false otherwise
     * @throws UnknownHostException If the UserContext's state could not be initialised.
     */
    public static UserContext createNewUserContext(WIContext wiContext) throws UnknownHostException {
        List webPNs = DisasterRecoveryUtils.getWebPNs(wiContext);
        int webPnIndex = webPNs.indexOf(DisasterRecoveryUtils.getWebPN(wiContext));

        for (int i = webPnIndex; i < webPNs.size(); i++) {
            WebPN webPN = (WebPN)webPNs.get(i);
            boolean roamingUserConfigured = wiContext.getConfiguration().isRoamingUserEnabled();
            boolean useRoaming = roamingUserConfigured && (i == Constants.INDEX_PRIMARY_WEB_PN);
            try {
                UserContext userContext = createNewUserContext(wiContext, webPN,
                                useRoaming);
                if (userContext != null) {
                    DisasterRecoveryUtils.setWebPN(wiContext, webPN);
                    return userContext;
                }
            } catch (AccessTokenException e) {
                // If credentials have become invalid, fail the creation
                break;
            } catch (ResourceUnavailableException e) {
                // If the WebPN is not suitable for use, fail the creation
                break;
            }
        }

        return null;
    }

    private static UserContext createNewUserContext(WIContext wiContext, WebPN webPN, boolean useRoaming) throws UnknownHostException, ResourceUnavailableException, AccessTokenException {
        WebAbstraction web = wiContext.getWebAbstraction();
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        WebPNBuilder builder = (WebPNBuilder)web.getApplicationAttribute(AppAttributeKey.WEBPN_BUILDER);
        UserContext userCtxt = builder.createInitialUserContext(webPN, envAdaptor);

        // Get the access token (identity / credentials) from the authentication
        // layer. This is the identity the user used to authenticate to the web
        // app
        AccessToken authToken = Authentication.getPrimaryAccessToken(web);

        // This is a new user, perform any one-off initialization
        AccessConditions accessConditions = AGEUtilities.getAGEAccessConditions(wiContext);
        initializeNewSession(wiContext, userCtxt, authToken, accessConditions, useRoaming);

        if (!testUserContext(wiContext, userCtxt)) {
            // UserContext is no good after all, so return without saving it to
            // the session state.
            return null;
        }

        // The test passed so the UserContext is usable.
        //
        // Ensure that important information about the UserContext is
        // persisted to the session state before returning the result.
        userCtxt.close();
        return userCtxt;
    }

    private static boolean testUserContext(WIContext wiContext, UserContext userContext) throws AccessTokenException, ResourceUnavailableException {
        try {
            // Test the WebPN by enumerating the user's available resources.
            userContext.findFolderContent("\\");
            DesktopGroupsController.createAndStoreInSession(wiContext, userContext);
            return true;
        } catch (ResourceUnavailableException e) {
            if ((e.getCause() instanceof SourceUnavailableException)) {
                return false;
            }

            throw e;
        }
    }
}
