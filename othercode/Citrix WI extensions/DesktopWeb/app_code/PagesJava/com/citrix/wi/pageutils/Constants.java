/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.metrics.PerformanceMetrics;
import com.citrix.wi.types.ApplicationView;
import com.citrix.wi.types.CompactApplicationView;
import com.citrix.wi.types.LayoutType;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.util.Platform;
import com.citrix.wing.types.AppAccessMethod;


public class Constants {

    public static final String PAGE_EXTENSION                     = Platform.isDotNet() ? ".aspx" : ".jsp";
    public static final String PRIVATE_FOLDER_PATH                = Platform.isDotNet() ? "~/app_data/" : "/WEB-INF/";

    // Override ICA file names
    public static final String ICA_FILE_DEFAULT                   = "default.ica";
    public static final String ICA_FILE_BANDWIDTH_HIGH            = "bandwidth_high.ica";
    public static final String ICA_FILE_BANDWIDTH_MEDIUM_HIGH     = "bandwidth_medium_high.ica";
    public static final String ICA_FILE_BANDWIDTH_MEDIUM          = "bandwidth_medium.ica";
    public static final String ICA_FILE_BANDWIDTH_LOW             = "bandwidth_low.ica";

    public static final int    MIN_WRAPPER_WIDTH                  = 780;
    public static final int    MAX_WRAPPER_WIDTH                  = 1000;
    public static final int    WRAPPER_BORDER_WIDTH               = 40;
    public static final int    CE_WINDOW_BORDER_WIDTH             = 20;

    public static final String SV_FRAME_SUFFIX                    = "FrameSuffix";
    public static final String ID_FRAME_MAIN                      = "CitrixMainFrame";
    public static final String ID_FRAME_TIMEOUT                   = "timeoutFrame";
    public static final String ID_FRAME_RECONNECT                 = "reconnectFrame";
    public static final String ID_FRAME_LAUNCH                    = "launchFrame";
    public static final String ID_FRAME_RETRY_POPULATOR           = "retryPopulatorFrame";
    public static final String ID_DIV_LAUNCH                      = "launchDiv";
    public static final String ID_DIV_RETRYPOPULATOR              = "retryPopulatorDiv";

    public static final String PAGE_LOGIN                         = "login" + PAGE_EXTENSION;
    public static final String PAGE_LOGIN_SETTINGS                = "loginSettings" + PAGE_EXTENSION;
    public static final String PAGE_LOGOUT                        = "logout" + PAGE_EXTENSION;
    public static final String PAGE_LOGGEDOUT                     = "loggedout" + PAGE_EXTENSION;
    public static final String PAGE_APPLIST                       = "default" + PAGE_EXTENSION;
    public static final String PAGE_LAUNCH                        = "launch" + (Platform.isDotNet() ? ".ica" : ".jsp");
    public static final String PAGE_STREAMING_LAUNCH              = "launch" + (Platform.isDotNet() ? ".rad" : ".jsp");
    public static final String PAGE_PREFERENCES                   = "preferences" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_PASSWD                 = "changepassword" + PAGE_EXTENSION;
    public static final String PAGE_PASSWORD_EXPIRY_WARN          = "passwordexpirywarn" + PAGE_EXTENSION;
    public static final String PAGE_APPEMBED                      = "appembed" + PAGE_EXTENSION;
    public static final String PAGE_DISCONNECT                    = "disconnect" + PAGE_EXTENSION;
    public static final String PAGE_RECONNECT                     = "reconnect" + PAGE_EXTENSION;
    public static final String PAGE_RECONNECT_UI                  = "reconnect_ui" + PAGE_EXTENSION;
    public static final String PAGE_CHALLENGE                     = "challenge" + PAGE_EXTENSION;
    public static final String PAGE_CERTIFICATE                   = "certificate" + PAGE_EXTENSION;
    public static final String PAGE_CERTIFICATE_ERROR             = "certificateError" + PAGE_EXTENSION;
    public static final String PAGE_PRE_CERTIFICATE               = "preCertificate" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_PIN_WARNING            = "change_pin_warning" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_PIN_SYSTEM             = "change_pin_system" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_PIN_USER               = "change_pin_user" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_PIN_EITHER             = "change_pin_either" + PAGE_EXTENSION;
    public static final String PAGE_NEXT_TOKENCODE                = "next_tokencode" + PAGE_EXTENSION;
    public static final String PAGE_ICON                          = "icons" + PAGE_EXTENSION;
    public static final String PAGE_HELP                          = "help" + PAGE_EXTENSION;
    public static final String PAGE_NOCOOKIES                     = "nocookies" + PAGE_EXTENSION;
    public static final String PAGE_DUMMY                         = "../html/dummy.html";
    public static final String PAGE_TIMEOUT_TRIGGER               = "timeoutTrigger" + PAGE_EXTENSION;;
    public static final String PAGE_DIRECT_LAUNCH                 = "directLaunch" + PAGE_EXTENSION;
    public static final String PAGE_LAUNCHER                      = "launcher" + PAGE_EXTENSION;
    public static final String PAGE_ACCOUNT_SS_ENTRY              = "account_ss_entry" + PAGE_EXTENSION;
    public static final String PAGE_ACCOUNT_SS_USER               = "account_ss_user" + PAGE_EXTENSION;
    public static final String PAGE_ACCOUNT_SS_QBA                = "account_ss_qba" + PAGE_EXTENSION;
    public static final String PAGE_ACCOUNT_SS_UNLOCK             = "account_ss_unlock" + PAGE_EXTENSION;
    public static final String PAGE_ACCOUNT_SS_RESET              = "account_ss_reset" + PAGE_EXTENSION;
    public static final String PAGE_APP_EMBED_RDP_JAVASCRIPT      = "appembedRDPJavaScript" + PAGE_EXTENSION;
    public static final String PAGE_APP_EMBED_ACTIVEX_JAVASCRIPT  = "appembedActiveXJavaScript" + PAGE_EXTENSION;
    public static final String PAGE_APP_EMBED_JICA_JAVASCRIPT     = "appembedJICAJavaScript" + PAGE_EXTENSION;
    public static final String PAGE_EXPLICIT                      = "explicit" + PAGE_EXTENSION;
    public static final String PAGE_MESSAGES                      = "messageScreen" + PAGE_EXTENSION;
    public static final String PAGE_VIEW_STYLE_SETTINGS           = "viewStyleSettings" + PAGE_EXTENSION;
    public static final String PAGE_CHANGE_MODE                   = "changeMode" + PAGE_EXTENSION;
    public static final String PAGE_PRE_LOGIN_MESSAGE             = "preLoginMessage" + PAGE_EXTENSION;
    public static final String PAGE_SILENT_DETECTION              = "silentDetection" + PAGE_EXTENSION;
    public static final String PAGE_STYLE                         = "style" + PAGE_EXTENSION;
    public static final String PAGE_JAVASCRIPT                    = "javascript" + PAGE_EXTENSION;
    public static final String PAGE_DELAY_LAUNCH_TIMER            = "delayLaunchTimer" + PAGE_EXTENSION;
    public static final String PAGE_RETRY_POPULATOR               = "retryPopulator" + PAGE_EXTENSION;
    public static final String PAGE_RETRY                         = "retry" + PAGE_EXTENSION;
    public static final String PAGE_SERVER_ERROR                  = "../html/serverError.html";
    public static final String PAGE_RESTART_DESKTOP               = "restartDesktop" + PAGE_EXTENSION;
    public static final String PAGE_CONFIRM_RESTART_DESKTOP       = "confirmRestartDesktop" + PAGE_EXTENSION;
    public static final String PAGE_AGE_LOGOUT                    = "agelogout" + PAGE_EXTENSION;
    public static final String PAGE_ASSIGN_DESKTOP                = "assignDesktop" + PAGE_EXTENSION;
    public static final String PAGE_SEARCH_RESULTS                = "searchResults" + PAGE_EXTENSION;

    //client (wizard) Detection integration pages
    public static final String PAGE_WIZARD_MASTER                 = "WIWizardLayout" + (Platform.isDotNet() ? ".master" : ".jsp");
    public static final String PAGE_WIZARD_START                  = "start" + PAGE_EXTENSION;
    public static final String PAGE_WIZARD_OUTPUT                 = "clientDetectionOutputs" + PAGE_EXTENSION;
    public static final String PAGE_WIZARD_PRE_INPUT              = "clientDetectionPreInputs" + PAGE_EXTENSION;
    public static final String PAGE_PASSWORD_CHALLENGE            = "password_challenge.aspx";

    // Query string names
    public static final String QSTR_EMBEDDED_RESOURCE_ERROR       = "CTX_EmbeddedResourceError";

    public static final String QSTR_PAGE_ACCESS_ONLY              = "PageAccessOnly";

    public static final String QSTR_MSG_KEY                       = "CTX_MessageKey";
    public static final String QSTR_MSG_TYPE                      = "CTX_MessageType";
    public static final String QSTR_MSG_ARGS                      = "CTX_MessageArgs";
    public static final String QSTR_LOG_EVENT_ID                  = "CTX_LogEventID";

    public static final String QSTR_CURRENT_FOLDER                = "CTX_CurrentFolder";
    public static final String QSTR_REFRESH                       = "CTX_Refresh";
    public static final String QSTR_TITLE                         = "Title";
    public static final String QSTR_LOGINID                       = "CTX_LoginId";
    public static final String QSTR_TIMEOUT                       = "CTX_Timeout";

    public static final String QSTR_SMC_LOGGED_OUT                = "CTX_SmartCardLoggedOut";
    public static final String QSTR_LOGINTYPE                     = "CTX_LoginType";

    public static final String QSTR_LAUNCH_UID                    = "CTX_UID";
    public static final String QSTR_LAUNCH_APP_FRIENDLY_NAME      = "CTX_AppFriendlyNameURLEncoded";
    public static final String QSTR_LAUNCH_APP_COMMAND_LINE       = "CTX_AppCommandLine";
    public static final String QSTR_LAUNCH_MIME_EXTENSION         = "CTX_MIMEExtension";
    public static final String QSTR_LAUNCH_WINDOW_WIDTH           = "CTX_WindowWidth";
    public static final String QSTR_LAUNCH_WINDOW_HEIGHT          = "CTX_WindowHeight";

    public static final String QSTR_HOSTID                        = "CTX_HostId";
    public static final String QSTR_HOSTID_TYPE                   = "CTX_HostIdType";
    public static final String QSTR_SESSIONID                     = "CTX_SessionId";

    public static final String QSTR_APPLICATION                   = "CTX_Application";
    public static final String QSTR_APP_FRIENDLY_NAME_URLENCODED  = "CTX_AppFriendlyNameURLENcoded";
    public static final String QSTR_APP_COMMAND_LINE              = "CTX_AppCommandLine";

    public static final String QSTR_METRIC_RECONNECT_ID           = "ReconnectId";
    public static final String QSTR_METRIC_LAUNCH_ID              = "LaunchId";

    public static final String QSTR_FROM_LOGGEDOUT_PAGE           = "CTX_FromLoggedoutPage";
    public static final String QSTR_LOCALE                        = "Locale";
    public static final String QSTR_CLIENT                        = "Client";
    public static final String QSTR_PLATFORM                      = "Platform";
    public static final String QSTR_CAPTION_LIST_INDEX            = "CaptionListIndex";
    public static final String QSTR_START_SELF_SERVICE            = "CTX_StartSelfService";
    public static final String QSTR_END_SELF_SERVICE              = "CTX_EndSelfService";

    public static final String QSTR_LAUNCH_METHOD                 = "CTX_LaunchMethod";

    // Query string parameter used for checking whether user came from account settings page
    public static final String QSTR_FROM_ACCOUNT_SETTINGS         = "FromAccountSettings";

    // Query String parameter for passing the user provided applist view style
    public static final String QSTR_CURRENT_VIEW_STYLE            = "CTX_CurrentViewStyle";
    // Query String parameter for passing the user provided layout (low or full graphics)
    public static final String QSTR_LAYOUT_TYPE                   = "LayoutType";
    // Query String parameter for passing the user provided search string pattern
    public static final String QSTR_SEARCH_STRING                 = "CTX_SearchString";
    // Query String parameter for passing the currently displayed tab name
    public static final String QSTR_CURRENT_TAB                   = "CTX_CurrentTab";
    // Query String parameter for passing the currently displayed search results page number
    public static final String QSTR_SEARCH_RESULTS_PAGE_NO        = "CTX_SearchPageNumber";
    // Query String parameter for hiding the hints area
    public static final String QSTR_CLOSE_HINTS_AREA              = "CTX_CloseHintsArea";
    // Query String parameter for switching to a tab for showing a specific resource type
    public static final String QSTR_SWITCH_TO_RESOURCE_VIEW = "CTX_SwitchToResourceView";

    // Query string values for resource types. (Used when navigating to a folder from search results).
    public static final String QSTR_RESOURCE_APP                    = "APP";
    public static final String QSTR_RESOURCE_CONTENT                = "CONTENT";
    public static final String QSTR_RESOURCE_DESKTOP                = "DESKTOP";

    public static final String QSTR_RETRY_APPLICATION               = "RetryApplication";
    public static final String QSTR_RETRY_IN_PROGRESS               = "RetryInProgress";

    // Query string for problems with
    public static final String QSTR_INVALID_USERNAME                = "InvalidUsername";
    public static final String QSTR_INVALID_DOMAIN                  = "InvalidDomain";
    public static final String QSTR_INVALID_CONTEXT                 = "InvalidContext";

    //Wizard QueryStrings
    // Use when invoking the wizard from a download caption (to a install remote or streaming client)
    public static final String QSTR_CLIENT_TYPE                   = "CLIENT_TYPE";
    // Use when invoking the wizard from the settings page
    public static final String QSTR_SETTINGS                      = "SETTINGS";
    // Use when invoking the wizard from the toolbar
    public static final String QSTR_WIZARD_TOOLBAR                = "TOOLBAR";
    // Use when invoking the wizard from a download caption (to upgrade or change zone)
    public static final String QSTR_DETECT_CURRENT                = "DETECTCURRENT";
    // Show zone detection
    public static final String QSTR_SHOW_ZONE                     = "SHOWZONE";
    // Show client upgrade
    public static final String QSTR_SHOW_UPGRADE                  = "SHOWUPGRADE";
    // Use when invoking the wizard from a client summary table link
    public static final String VAL_CLIENT                         = "Client";
    // Use when forcing the java client
    public static final String QSTR_FORCE_ICA_JAVA                = "FORCE_ICA_JAVA";
    // Use when forcing the native client
    public static final String QSTR_FORCE_ICA_LOCAL               = "FORCE_ICA_LOCAL";

    // Cookie names

    // Cookie used for information provided by client (set by JavaScript)
    public static final String COOKIE_CLIENT_INFO                 = "WIClientInfo";

    public static final String COOKIE_NDS_CONTEXT                 = "CTX_Context";
    public static final String COOKIE_ICA_CLIENT_VERSION          = "icaClientVersion";
    public static final String COOKIE_RDP_CLASS_ID                = "rdpClassId";
    public static final String COOKIE_ICA_SCREEN_RESOLUTION       = "icaScreenResolution";
    public static final String COOKIE_WRAPPER_WIDTH               = "wrapperWidth";
    public static final String COOKIE_CLIENT_CONN_SECURE          = "clientConnSecure";
    public static final String COOKIE_TEST_COOKIE                 = "Cookies_On";
    public static final String COOKIE_TEST_COOKIE_VALUE           = "true";
    public static final String COOKIE_REMOTE_CLIENT_DETECTED      = "remoteClientDetected";
    public static final String COOKIE_STREAMING_CLIENT_DETECTED   = "streamingClientDetected";
    public static final String COOKIE_ALTERNATE_RESULT            = "alternateResult";
    public static final String COOKIE_ICO_STATUS                  = "icoStatus";
    public static final String COOKIE_WING_SESSION_NAME           = "WINGSession";
    public static final String COOKIE_DEVICE_NAME                 = "WINGDevice";
    public static final String COOKIE_TREE_VIEW_CURRENT_FOLDER    = "treeViewCurrentFolder";

    // ASP only cookies
    public static final String COOKIE_SMC_LOGGED_OUT              = "CTXSMCLoggedOut";
    public static final String COOKIE_ALLOW_AUTO_LOGIN            = "CTXAllowSilent";
    public static final String COOKIE_AGE_LOGGED_OUT              = "CitrixAGELoggedOut";

    public static final String COOKIE_DEVICE_ID                   = "CTX_DeviceId";

    public static final String USER_COOKIE_NAME                   = "WIUser";

    // Session variable names
    public static final String SV_RECONNECT_AT_LOGIN              = "CTX_ReconnectAtLogin";

    // Session variable storing the current folder in the applist view
    public static final String SV_CURRENT_FOLDER = "CTX_CurrentFolder";
    // Session variable storing the last provided search string pattern
    public static final String SV_SEARCH_QUERY = "CTX_SearchQuery";
    // Session variable storing the search results for the last search string pattern
    public static final String SV_CURRENT_SEARCH_RESULTS = "CTX_CurrentSearchResults";
    // Session variable storing the current page number of the search results page
    public static final String SV_CURRENT_PAGE_NUMBER = "CTX_CurrentPageNumber";
    // Session variable storing the tab name which is currently being displayed
    public static final String SV_CURRENT_TAB = "CTX_CurrentTab";
    // Session variable storing the tab name which was previously displayed
    public static final String SV_PREVIOUS_TAB = "CTX_PreviousTab";
    // Session variable storing the order in which non-resource tabs are displayed
    public static final String SV_TAB_ORDER = "CTX_AppTabOrder";

    // Session variable storing the launch Info
    public static final String SV_APP_LAUNCH_INFO = "AppLaunchInfo";

    //Session variable to store the last feedback message received by ICO/RCO
    public static final String SV_LAUNCH_FILE_FEEDBACK_MSG        = "LaunchFileErrorKey";

    // Session variable to store browser preferred language
    public static final String SV_BROWSER_LOCALE                  = "BrowserLocale";

    // Session variable to store the index to the list of WebPNs that points to
    // the currently in-use WebPN.
    public static final String SV_WEB_PN                          = "WebPN";

    //Variable for storing the error keys
    public static final String CONST_ERROR_MESSAGE                = "ErrorMessage";
    public static final String CONST_RESOURCE_ERROR               = "ResourceError";

    // Variable for closing the search tab
    public static final String CONST_CLOSE_SEARCH_TAB             = "CloseSearchTab";
    // Variable storing close hints area
    public static final String CONST_CLOSE_HINTS_AREA             = "CloseHintsArea";

    // Index to the list of WebPNs that indicates the user's primary WebPN.
    public static final int    INDEX_PRIMARY_WEB_PN               = 0;

    // Index to the list of PNA Services that indicates the user's primary PNA
    // Service.
    public static final int    INDEX_PRIMARY_PNA_SERVICE          = 0;

    // Index to the list of WebPNs that indicates the user's first disaster
    // recovery WebPN.
    public static final int    INDEX_FIRST_RECOVERY_WEB_PN        = 1;

    // Index to the list of PNA Services that indicates the user's first
    // disaster recovery PNA Service.
    public static final int    INDEX_FIRST_RECOVERY_PNA_SERVICE   = 1;

    // Tab names
    public static final String TAB_NAME_ALL_RESOURCES             = "AllResources";
    public static final String TAB_NAME_DESKTOPS                  = "Desktops";
    public static final String TAB_NAME_APPS                      = "Applications";
    public static final String TAB_NAME_CONTENT                   = "Content";

    // Localizable String Keys
    public static final String STR_LAUNCHING_TAB_TITLE            = "LaunchingTabTitle";
    public static final String STR_LAUNCH_CONTROL_TAB_TITLE       = "LaunchControlTabTitle";

    // Request/Context variable names
    public static final String AUTHSTATE_VARIABLE_NAME            = "CTX_AUTHENTICATION_STATE";

    // Server variable names
    public static final String SRV_URL                            = "URL";
    public static final String SRV_HTTPS                          = "HTTPS";
    public static final String SRV_SERVER_NAME                    = "SERVER_NAME";
    public static final String SRV_SERVER_PORT                    = "SERVER_PORT";
    public static final String SRV_AUTH_USER                      = "AUTH_USER";

    // AGE parameter names
    public static final String AGE_USERNAME                       = "username";
    public static final String AGE_PASSWORD                       = "password";
    public static final String AGE_DOMAIN                         = "domain";
    public static final String AGE_SESSION_ID                     = "AGESessionId";
    public static final String AGE_ACCESS_TOKEN                   = "access_token";
    public static final String AGE_PROTOCOL_REVISION              = "ProtocolRevision";

    // Return codes
    public static final int    RC_ENABLED                         = 10;
    public static final int    RC_DISABLED                        = 11;
    public static final int    RC_JAVACLIENT_ONLY                 = 12;

    // Values of buttons
    public static final String VAL_OK                             = "ok";
    public static final String VAL_CANCEL                         = "cancel";
    public static final String VAL_SAVE                           = "save";
    public static final String VAL_CONFIRM                        = "confirm";

    public static final String ICA_CONN_CENTER_HRES               = "330";
    public static final String ICA_CONN_CENTER_VRES               = "142";

    public static final String DEFAULT_ICA_WINDOW_HRES            = "640";
    public static final String DEFAULT_ICA_WINDOW_VRES            = "480";

    // Default window size for applications
    public static final int    MAX_ICA_WINDOW_HRES                = 32768;
    public static final int    MAX_ICA_WINDOW_VRES                = 32768;

    public static final String VAL_RADIUS                         = "RADIUS";
    public static final String VAL_USER                           = "USER";
    public static final String VAL_SYSTEM                         = "SYSTEM";

    // Values of change password options
    public static final String VAL_NEVER                          = "Never";
    public static final String VAL_ALWAYS                         = "Always";
    public static final String VAL_EXPIRED_ONLY                   = "Expired-Only";

    // Values of reconnect at login options
    public static final String VAL_DISCONNECTED                   = "Disconnected";
    public static final String VAL_DISCONNECTED_ACTIVE            = "DisconnectedAndActive";

    public static final String VAL_AUTO                           = "Auto";
    public static final String VAL_ON                             = "On";
    public static final String VAL_OFF                            = "Off";
    public static final String VAL_NONE                           = "None";

    public static final String VAL_SEAMLESS                       = "seamless";

    public static final String VAL_TRUE                           = "True";
    public static final String VAL_FALSE                          = "False";

    public static final String VAL_ICO_NOT_PRESENT                = "0";
    public static final String VAL_ICO_OLD                        = "1";
    public static final String VAL_ICO_IS_PASS_THROUGH            = "2";
    public static final String VAL_ICO_NOT_PASS_THROUGH           = "3";

    public static final String VAL_CLIENT_TYPE_LOCAL_ICA          = MPSClientType.LOCAL_ICA.toString();
    public static final String VAL_CLIENT_TYPE_JAVA               = MPSClientType.JAVA.toString();
    public static final String VAL_CLIENT_TYPE_EMBEDDED_RDP       = MPSClientType.EMBEDDED_RDP.toString();
    public static final String VAL_CLIENT_TYPE_AUTO               = "AutoClient";

    public static final String VAL_ACCESS_METHOD_STREAMING        = AppAccessMethod.STREAMING.toString();
    public static final String VAL_ACCESS_METHOD_REMOTE           = AppAccessMethod.REMOTE.toString();

    public static final String VAL_LAYOUT_TYPE_AUTO               = LayoutType.AUTO.toString();
    public static final String VAL_LAYOUT_TYPE_NORMAL             = LayoutType.NORMAL.toString();
    public static final String VAL_LAYOUT_TYPE_COMPACT            = LayoutType.COMPACT.toString();

    public static final String ID_CITRIX_FORM                     = "CitrixForm";
    public static final String ID_SUBMIT_MODE                     = "submitMode";
    public static final String MODE_SUBMIT                        = "submit";
    public static final String MODE_APPLY                         = "apply";

    // Low graphics list view style
    public static final String VAL_VIEW_STYLE_LIST                = CompactApplicationView.LIST.toString();
    // Low graphics icons view style
    public static final String VAL_VIEW_STYLE_ICONS               = CompactApplicationView.ICONS.toString();

    // ================================================
    // Constants used by the login page.
    // ================================================
    public static final String LOGIN_ENTRY_MAX_LENGTH             = "256";
    public static final String PASSCODE_ENTRY_MAX_LENGTH          = "256";
    public static final int PASSWORD_ENTRY_MAX_LENGTH             = 127;

    public static final String ID_LOGIN_TYPE                      = "LoginType";
    public static final String ID_USER                            = "user";
    public static final String ID_PASSWORD                        = "password";
    public static final String ID_ACCOUNTSS                       = "accountSS";
    public static final String ID_DOMAIN                          = "domain";
    public static final String ID_ASS_ANSWER                      = "lblAnswer";
    public static final String ID_LOGIN_BTN                       = "btnLogin";

    public static final String ID_CLIENT_TYPE                     = "ClientType";
    public static final String ID_BANDWIDTH                       = "bandwidth";

    // The following ids are not used in JSP site but needed for client
    // scripts shared with ASPX site
    public static final String ID_CONTEXT                         = "context";
    public static final String ID_TREE                            = "tree";
    public static final String ID_PASSCODE                        = "passcode";

    // Form element ids for presentation settings page.
    public static final String ID_CHECK_REMEMBER_FOLDER           = "RememberFolder";
    public static final String ID_CHECK_SILENT_AUTHENTICATION     = "SilentAuthentication";
    public static final String ID_OPTION_LAYOUT_TYPE              = "slLayoutType";
    public static final String ID_OPTION_LANGUAGE                 = "slLanguage";
    public static final String ID_CHECK_SHOW_SEARCH               = "ShowSearch";
    public static final String ID_CHECK_SHOW_HINTS                = "ShowHints";

    // Form element ids for client settings page.
    public static final String ID_DIV_REMOTE_CLIENTS              = "divRemoteClients";
    public static final String ID_OPTION_CLIENT_TYPE              = "slClientType";

    public static final String ID_DIV_JICA_PACKAGE                = "divJICAPackage";
    public static final String ID_CHECK_JICA_AUDIO                = "chkJICAAudio";
    public static final String ID_CHECK_JICA_CDM                  = "chkJICACDM";
    public static final String ID_CHECK_JICA_CLIPBOARD            = "chkJICAClipboard";
    public static final String ID_CHECK_JICA_CONFIGUI             = "chkJICAConfigUI";
    public static final String ID_CHECK_JICA_PRINTER              = "chkJICAPrinter";
    public static final String ID_CHECK_JICA_ZERO                 = "chkJICAZero";

    public static final String ID_LABEL_JICA_AUDIO                = "lblJICAAudio";
    public static final String ID_LABEL_JICA_CDM                  = "lblJICACDM";
    public static final String ID_LABEL_JICA_CLIPBOARD            = "lblJICAClipboard";
    public static final String ID_LABEL_JICA_CONFIGUI             = "lblJICAConfigUI";
    public static final String ID_LABEL_JICA_PRINTER              = "lblJICAPrinter";
    public static final String ID_LABEL_JICA_ZERO                 = "lblJICAZero";
    public static final String ID_LABEL_JICA_PACKAGE              = "lblJICAPackage";

    // Form element ids for session settings page.
    public static final String ID_OPTION_WINDOW_SIZE              = "slWindowSize";
    public static final String ID_LABEL_WINSIZE_CUSTOM            = "lblWindowSizeCustom";
    public static final String ID_SPAN_WINSIZE_CUSTOM             = "spWindowSizeCustom";
    public static final String ID_TEXT_DESIRED_HRES               = "txtDesiredHRES";
    public static final String ID_TEXT_DESIRED_VRES               = "txtDesiredVRES";
    public static final String ID_LABEL_WINSIZE_PERCENT           = "lblWindowSizePercent";
    public static final String ID_SPAN_WINSIZE_PERCENT            = "spCustomWindowSizePercent";
    public static final String ID_TEXT_SCREEN_PERCENT             = "txtScreenPercent";
    public static final String ID_OPTION_KEY_PASSTHROUGH          = "slKeyPassthrough";
    public static final String ID_LABEL_KEY_PASSTHROUGH           = "lblKeyPassthrough";
    public static final String ID_LEGEND_KEY_PASSTHROUGH          = "lgdKeyPassthrough";
    public static final String ID_OPTION_WINDOW_COLOR             = "slWindowColor";
    public static final String ID_OPTION_AUDIO                    = "slAudio";
    public static final String ID_OPTION_BANDWIDTH                = "slBandwidth";
    public static final String ID_CHECK_PRINTER                   = "chkPrinter";
    public static final String ID_CHECK_SPECIALFOLDERREDIRECTION  = "chkSpecialFolderRedirection";
    public static final String ID_CHECK_VIRTUAL_COM_PORT          = "chkVirtualCOMPort";

    // Form element ids for workspace control settings
    public static final String ID_LABEL_RECONNECT_LOGIN           = "lblReconnectAtLogin";
    public static final String ID_CHECK_RECONNECT_LOGIN           = "chkReconnectAtLogin";
    public static final String ID_OPTION_RECONNECT_LOGIN          = "slReconnectLogin";
    public static final String ID_LABEL_RECONNECT_BUTTON          = "lblReconnectButton";
    public static final String ID_CHECK_RECONNECT_BUTTON          = "chkReconnectButton";
    public static final String ID_OPTION_RECONNECT_BUTTON         = "slReconnectButton";
    public static final String ID_LABEL_LOGOFF_ACTION             = "lblLogoffAction";
    public static final String ID_LABEL_LOGOFF                    = "lblLogoff";
    public static final String ID_CHECK_LOGOFF                    = "chkLogoff";

    public static final String ID_RADIO_COMPACT_VIEW_STYLE        = "radioCompactViewStyle";

    // Form element ids for the restart desktop confirmation page
    public static final String ID_APPLICATION                     = "application";
    public static final String ID_RETRY_IN_PROGRESS               = "retryInProgress";

    // Constants for icon sizes
    public static final int    ICON_SIZE_16                       = 16;
    public static final int    ICON_SIZE_24                       = 24;
    public static final int    ICON_SIZE_32                       = 32;
    public static final int    ICON_SIZE_48                       = 48;
    public static final String ICON_SIZE_SMALL                    = "small";
    public static final String ICON_SIZE_NORMAL                   = "normal";

    // Screen icons
    public static final String MEDIA_LOCATION                     = "../media/";

    // Metric names
    public static final String METRIC_START_SCD                   = PerformanceMetrics.START_KEY_PREFIX + "SCD";

    public static final String METRIC_RECD                        = "RECD";
    public static final String METRIC_START_RECD                  = PerformanceMetrics.START_KEY_PREFIX + METRIC_RECD;
    public static final String METRIC_END_RECD                    = PerformanceMetrics.END_KEY_PREFIX + METRIC_RECD;

    public static final String METRIC_REWD                        = "REWD";
    public static final String METRIC_START_REWD                  = PerformanceMetrics.START_KEY_PREFIX + METRIC_REWD;
    public static final String METRIC_END_REWD                    = PerformanceMetrics.END_KEY_PREFIX + METRIC_REWD;

    public static final String METRIC_START_IFDCD                 = PerformanceMetrics.START_KEY_PREFIX + "IFDCD";

    public static final String METRIC_LPWD                        = "LPWD";
    public static final String METRIC_START_LPWD                  = PerformanceMetrics.START_KEY_PREFIX + METRIC_LPWD;
    public static final String METRIC_END_LPWD                    = PerformanceMetrics.END_KEY_PREFIX + METRIC_LPWD;

    public static final String METRIC_NRD                         = "NRWD";
    public static final String METRIC_TRD                         = "TRWD";

    // Launch method strings
    public static final String LAUNCH_METHOD_STREAMING            = "streaming";

    // Keyboard navigation on login page
    public static final String TAB_INDEX_NAVCONTROL               = "1";
    public static final String TAB_INDEX_FORM                     = "2";

    // Values of buttons
    public static final String VAL_YES                            = "yes";
    public static final String VAL_NO                             = "no";

    public static final String FORM_POSTBACK                      = "postback";

    // Values for the different change view options
    public static final String VAL_ICON_VIEW                        = ApplicationView.ICONS.toString();;
    public static final String VAL_DETAILS_VIEW                     = ApplicationView.DETAILS.toString();
    public static final String VAL_LIST_VIEW                        = ApplicationView.LIST.toString();
    public static final String VAL_TREE_VIEW                        = ApplicationView.TREE.toString();
    public static final String VAL_GROUPS_VIEW                      = ApplicationView.GROUPS.toString();

    // Types of embedded resource failures
    public static final String VAL_GENERAL                          = "General";
    public static final String VAL_ICO                              = "ICO";
    public static final String VAL_RCO                              = "RCO";

    // Common controls
    public static final String CTRL_FOOTER = "footerControl";
    public static final String CTRL_HEADER = "headerControl";
    public static final String CTRL_LAYOUT = "layoutControl";
    public static final String CTRL_FEEDBACK = "feedbackControl";
    public static final String CTRL_MESSAGES = "messagesControl";
    public static final String CTRL_NAV = "navControl";
    public static final String CTRL_SYSMESSAGE = "sysMessageControl";
    public static final String CTRL_WELCOME = "welcomeControl";
    public static final String CTRL_SEARCH_BOX = "searchBoxControl";
    public static final String CTRL_AUTO_LAUNCH_JAVASCRIPT = "autoLaunchJavaScritpControl";
}
