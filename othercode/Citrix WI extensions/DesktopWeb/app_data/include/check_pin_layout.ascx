<%@ Register TagPrefix="wi" TagName="Header" Src="headerView.ascx" %>
<%@ Register TagPrefix="wi" TagName="Nav" Src="navView.ascx" %>
<%@ Register TagPrefix="wi" TagName="Feedback" Src="feedbackView.ascx" %>
<%@ Register TagPrefix="wi" TagName="ScreenTitle" Src="screenTitleView.ascx" %>
<%@ Register TagPrefix="wi" TagName="WelcomeMessage" Src="welcomeMessageView.ascx" %>
<%@ Register TagPrefix="wi" TagName="SysMessage" Src="sysMessageView.ascx" %>
<%@ Register TagPrefix="wi" TagName="Footer" Src="footerView.ascx" %>
<%@ Register TagPrefix="wi" TagName="CompactOptions" Src="compactOptionsView.ascx" %>

<%
// layout.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<script runat="server">
    public string PageClientScript = "";
    public string PageView = "";

    void Page_Load(object sender, System.EventArgs e) {
        if (PageClientScript != null && PageClientScript != "") {
            ClientScriptPlaceHolder.Controls.Add(LoadControl(PageClientScript));
        }

        if (PageView != null && PageView != "") {
            ViewPlaceHolder.Controls.Add( LoadControl(PageView) );
            PreferencesViewPlaceHolder.Controls.Add( LoadControl(PageView) );
            LoginViewPlaceHolder.Controls.Add( LoadControl(PageView) );
            AppListViewPlaceHolder.Controls.Add( LoadControl(PageView) );
            CompactViewPlaceHolder.Controls.Add(LoadControl(PageView));
        }
    }
</script>

<%
LayoutControl layoutControl = (LayoutControl)wiContext.getWebAbstraction().getRequestContextAttribute("layoutControl");
WelcomeControl welcomeControl = (WelcomeControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_WELCOME);
// We need the feedback control for feedback.js
FeedbackControl feedbackControl = (FeedbackControl)wiContext.getWebAbstraction().getRequestContextAttribute("feedbackControl");
// We need the navControl for manipulating the low graphics logout button.
NavControl navControl = (NavControl)wiContext.getWebAbstraction().getRequestContextAttribute("navControl");

// Set the form to point back at the current page
string FormAction = layoutControl.formAction;
if (Strings.equalsIgnoreCase(FormAction, Constants.FORM_POSTBACK)) {
    FormAction = WebUtilities.escapeHTML(Request.Url.Segments[Request.Url.Segments.Length - 1]);
}
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
  <meta name="ROBOTS" content="NOINDEX, NOFOLLOW, NOARCHIVE">
  <meta http-equiv="Content-Style-Type" content="text/css">
  <meta http-equiv="Content-Script-Type" content="text/javascript">
  <title>SMS2 Secure Two Factor Authentication</title>
  <link rel="SHORTCUT ICON" href="../media/<%=Images.getFavoritesIcon(wiContext)%>" type="image/vnd.microsoft.icon">
  <link rel="stylesheet" type="text/css"  media="handheld,all" href="<%=UIUtils.getStyleSheetURL(wiContext)%>" >
  <!--#include file="~/app_data/include/cachedJavaScript.inc"-->
  <script type="text/javascript">
      <!--
      <%
      // we want to do some launch or reconnecting during the onload event, usually nothing
      AutoLaunchJavaScriptPageControl autoLaunchJavaScriptControl = (AutoLaunchJavaScriptPageControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_AUTO_LAUNCH_JAVASCRIPT);
      %>
      function doAutoLaunching() {
        <%=autoLaunchJavaScriptControl.reconnectJavaScript%>
        <%=autoLaunchJavaScriptControl.autoLaunchJavaScript%>
      }
      <asp:PlaceHolder id="ClientScriptPlaceHolder" runat="server" />
      // -->
  </script>

  <%
  // See CPR 183027
  if (layoutControl.isLoginPage && !Include.isCompactLayout(wiContext) && !layoutControl.embeddedLayout) {
  %>
  <!--#include file="loginStyle.inc" -->
  <%
  }
  %>
</head>

<%
String onLoadBehaviour = "putInTopFrame();";

if(Include.isLoggedIn(wiContext.getWebAbstraction())) {
    onLoadBehaviour += "resetSessionTimeout();";
}

if(DelayedLaunchUtilities.isRetryFrameRequired(wiContext)) {
    onLoadBehaviour += "addDesktopRetryFrame('"+WebUtilities.encodeAmpersands(DelayedLaunchUtilities.getRetryFramePage(wiContext))+"');";
}

if(Include.isCompactLayout(wiContext)) {
    onLoadBehaviour += "doAutoLaunching();onLoadLayout();";
} else {
    onLoadBehaviour += "doAutoLaunching();onLoadLayout();setup_popup_behaviour();updateLayout();";
    if(layoutControl.isLoginPage && FormAction != null) {
        onLoadBehaviour += "setup_login_submit_keys();";
    }
    if(layoutControl.hasLightbox) {
        onLoadBehaviour += "configureLightbox();";
    }
}

String bodyClass = "";
if (layoutControl.isLoginPage) {
    bodyClass = "class=\"horizonPage\"";
} else if (layoutControl.isLoggedOutPage) {
    bodyClass = "class=\"horizonPage loggedOut\"";
}
%>

<body <%=bodyClass%> onLoad="<%=onLoadBehaviour%>" dir="<%=wiContext.getString( "TextDirection" )%>">

<%
if (FormAction != null) {
%>
  <form method="post" action="<%=FormAction%>" name="<%=Constants.ID_CITRIX_FORM%>" autocomplete="off">
<input type="hidden" name="<%=SessionToken.ID_FORM%>" value="<%=SessionToken.get(wiContext)%>" >
<%
}
%>

<%
if (Include.isCompactLayout(wiContext) || layoutControl.embeddedLayout) {
%>

<%
   // ---------------------------------------------------------------------------------
   // Start low graphics and embedded layout
   // ---------------------------------------------------------------------------------
%>

<% if (!layoutControl.embeddedLayout) { %>
    <wi:Header runat="server" />
<% } %>
    <div class="mainPane">
      <wi:ScreenTitle runat="server" />
      <div class="actionPane">
        <wi:Nav runat="server"/>
        <wi:Feedback runat="server" />
        <asp:PlaceHolder id="CompactViewPlaceHolder" runat="server" />
      </div>
<% if (!layoutControl.isLoginPage || layoutControl.layoutMode == LayoutMode.ADVANCED) { %>
      <wi:CompactOptions runat="server" />
<% } %>
      <div id="logoutArea">
<% if (FormAction == null) { %>
        <form action="">
<% }
   if (layoutControl.hasCancelButton) { %>
          <input id="backButton" class="Back" type="button" value="<%=wiContext.getString("Cancel")%>"
                 onclick="location.href='<%=Include.isLoggedIn(wiContext.getWebAbstraction()) ? Include.getHomePage(wiContext) : Constants.PAGE_LOGIN%>'">
<% }
   if (navControl.getShowLogout()) { %>
          <input id="logoutButton" type="button" onclick="location.replace('<%=Constants.PAGE_LOGOUT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>')"
                 value="<%= wiContext.getString("LogOff") %>" title="<%= wiContext.getString(navControl.getLogoutToolTip()) %>">
<% }
   if (FormAction == null) { %>
        </form>
<% } %>
      </div> <% // End logoutArea %>
    </div> <% // End mainPane %>
<%
   // ---------------------------------------------------------------------------------
   // End low graphics layout
   // ---------------------------------------------------------------------------------
%>

<%
} else {
%>

<%
   // ---------------------------------------------------------------------------------
   // Start full graphics layout
   // ---------------------------------------------------------------------------------

String wrapperClass = layoutControl.showBackgroundGradient ? " class='gradientBackground' " : "";

%>
  <div id="overallWrapper" <%=wrapperClass%>>
<%
// Add the content DIV based on the page we are rendering
// Content DIV contains all the main controls except the heightFiller and footer
if (layoutControl.isLoginPage || layoutControl.isLoggedOutPage) {
    String brandingImg;
    if (layoutControl.isLoggedOutPage) {
        brandingImg = (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) ? "CitrixXenDesktopLoggedoff.png" : "CitrixXenAppLoggedoff.png";
    } else {
        brandingImg = (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) ? "CitrixXenDesktop.png" : "CitrixXenApp.png";
    }
    brandingImg = ClientInfoUtilities.getImageName(wiContext.getClientInfo(), brandingImg);
%>
            <div id="pageContent">
<%
if (layoutControl.layoutMode == LayoutMode.ADVANCED && !layoutControl.isLoggedOutPage) {
%>
            <wi:Header runat="server" />
<%
}
%>
            <div id="horizonTop"><img src="../media/<%=brandingImg%>" alt=""></div>
            <div class="mainPane">
         <table class="carbonBox" cellpadding="0" cellspacing="0">
         <tr><td class="carbonBoxBottom">
            <table class="glowBox dynamicGlowBoxMargin" cellpadding="0" cellspacing="0" align="center">
              <tr>
                <td class="glowBoxTop glowBoxLeft glowBoxTopLeft"></td>
                <td class="glowBoxTop glowBoxTopMid"><div class="leftGradient"><div class="rightGradient"><div class="centerGradient"></div></div></div></td>
                <td class="glowBoxTop glowBoxRight glowBoxTopRight"></td>
              </tr>
              <tr>
                <td class="glowBoxLeft glowBoxMidLeft"></td>
                <td class="glowBoxMid loginTableMidWidth">

                  <wi:ScreenTitle runat="server" />
                  <wi:WelcomeMessage runat="server" />
                  <div class="spacer"></div>

                  <div class="actionPane">
                    <asp:PlaceHolder id="LoginViewPlaceHolder" runat="server" />
                    <div class="spacer"></div>
                  </div> <% // End actionPane %>

                </td>
                <td class="glowBoxRight glowBoxMidRight"></td>
              </tr>
              <tr>
                <td class="glowBoxFooter glowBoxLeft glowBoxFooterLeft"></td>
                <td class="glowBoxFooter glowBoxFooterMid"></td>
                <td class="glowBoxFooter glowBoxRight glowBoxFooterRight"></td>
              </tr>
              <tr>
                <td class="glowBoxLeft"></td>
                <td>
                  <wi:SysMessage runat="server" />
                </td>
                <td class="glowBoxRight"></td>
              </tr>
            </table>
          </td></tr>
        </table>
        <h3 id="horizonTagline"><%=wiContext.getString("HorizonTagline")%></h3>
      </div><% // End mainPane %>
    </div><% // End pageContent %>
<%
} else if (layoutControl.isAppListPage) {


String pageContentClass = layoutControl.showApplistBox ? "" : " class='hideBox' ";
%>
      <div id="pageContent" <%=pageContentClass%>>
        <wi:Header runat="server" />
        <wi:Nav runat="server"/>
        <div class="spacer"></div>

  <div class="mainPane" id="appMainPane">
    <div id="mainTopPane">
        <wi:Feedback runat="server" />
        <wi:WelcomeMessage runat="server" />
    </div> <% // End mainTopPane %>
    <asp:PlaceHolder id="AppListViewPlaceHolder" runat="server" />
    <div class="spacer"></div>
  </div> <% // End mainPane %>
  <wi:SysMessage runat="server" />
  </div> <% // End content %>
<%
// content DIV for non-login/applications pages
} else {

String pageContentClass = layoutControl.showApplistBox ? "" : " class='hideBox' ";
%>
              <div id="pageContent" <%=pageContentClass%>>
                <wi:Header runat="server" />
                <wi:Nav runat="server"/>
                <div class="spacer"></div>

            <div class="mainPane" id="commonMainPane">
              <wi:Feedback runat="server" />
<% if (welcomeControl.getShowTitle()) { %>
              <wi:ScreenTitle runat="server" />
<% }

   if (layoutControl.isPreferencesPage) {
%>
              <div id="preferences">
                <asp:PlaceHolder id="PreferencesViewPlaceHolder" runat="server" />
                <div class="spacer"></div>
              </div>
<% } else { %>
              <div id="commonBox">
                <wi:WelcomeMessage runat="server" />

                <div id="actionPane">
                  <asp:PlaceHolder id="ViewPlaceHolder" runat="server" />
                  <div class="spacer"></div>
                </div><% // End actionPane %>
              </div> <% // End commonBox %>
<% } 

   if (FormAction == null && layoutControl.hasCancelButton) { %>
              <form action="">
                <div class="customButton">
                  <a id="backButton" class="leftDoor"
                         onclick="location.href='<%=Include.isLoggedIn(wiContext.getWebAbstraction()) ? Include.getHomePage(wiContext) : Constants.PAGE_LOGIN%>'"
                         ><span class="rightDoor"><%=wiContext.getString("Cancel")%></span></a>
                </div>
              </form>
<% } %>
            </div> <% // End mainPane %>
            <wi:SysMessage runat="server" />
          </div> <% // End content %>
<%
} // end non-login pages
%>
          <div id="heightFiller"><!-- --></div>
          <wi:Footer runat="server" />

  </div> <% // End overallWrapper %>
<%
   // ---------------------------------------------------------------------------------
   // End full graphics layout
   // ---------------------------------------------------------------------------------
%>

<%
}
%>

<%
   // ---------------------------------------------------------------------------------
   // The hidden elements for launch, reconnect, timeout and delayed launches
   // ---------------------------------------------------------------------------------
   String _frameSuffix = Include.getFrameSuffix(wiContext.getUserEnvironmentAdaptor());
   if (!layoutControl.isLoginPage) {
%>
    <div class="HiddenDiv" id="<%=Constants.ID_DIV_LAUNCH%>"><!-- --></div>
    <iframe width="0" height="0" class="HiddenIframe" id="<%=Constants.ID_FRAME_RECONNECT + _frameSuffix%>" name="<%=Constants.ID_FRAME_RECONNECT + _frameSuffix%>"title="<%=wiContext.getString("ReconnectFrameTitle")%>" src="<%=Constants.PAGE_DUMMY%>"></iframe>
    <div class="HiddenDiv" id="<%=Constants.ID_DIV_RETRYPOPULATOR%>"><!-- the retry iframe is added here by the addDesktopRetryFrame script if there are delayed desktop launches in progress --></div>
<%
} // only the timeout frame is needed on the login page
%>
    <iframe width="0" height="0" class="HiddenIframe" id="<%=Constants.ID_FRAME_TIMEOUT + _frameSuffix%>" name="<%=Constants.ID_FRAME_TIMEOUT + _frameSuffix%>" title="<%=wiContext.getString("TimeoutFrameTitle")%>" src="<%=Constants.PAGE_DUMMY%>"></iframe>
<%
if (FormAction != null) {
%>
  </form>
<%
}
%>

<% if (layoutControl.hasLightbox) { %>
<!--#include file="lightbox.inc" -->
<% } %>

</body>
</html>
