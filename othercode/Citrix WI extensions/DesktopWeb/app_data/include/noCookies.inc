<%
String pageTitle = WIBrowserTitleBuilder.createTitle(wiContext, "BrowserTitleNoCookies");
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
  <meta name="ROBOTS" content="NOINDEX, NOFOLLOW, NOARCHIVE">
  <title><%=pageTitle%></title>
  <link rel="SHORTCUT ICON" href="../media/<%=Images.getFavoritesIcon(wiContext)%>" type="image/vnd.microsoft.icon">
  <link rel="stylesheet" type="text/css" href="../html/styles/basicStyle.css">
</head>
<body dir="<%=wiContext.getString( "TextDirection" )%>">
  <div id="overallWrapper">
    <div id="pageContent">
	  <div id="headerWrapper">
        <div id="header">
          <img src="../media/CitrixLogoHeader.gif" alt="[CompanyLogo]">
        </div>
	    <div class="spacer"><!-- --></div>
	  </div>
      <div class="mainPane">
        <div id="welcome">
          <h4 id="welcomeTitle"><%=wiContext.getString("NoCookiesTitle")%></h4>
        </div>
        <div id="commonBox">
          <p><%=wiContext.getString("NoCookiesDesc")%></p>
          <p><%=wiContext.getString("NoCookiesAction")%></p>
        </div>
      </div>
    </div>
  </div>
</body>
</html>
