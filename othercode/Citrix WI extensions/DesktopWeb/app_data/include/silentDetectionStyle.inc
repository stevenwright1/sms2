<%
// silentDetectionStyle.inc
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

body {
  background: white;
  font-family: Segoe UI, Tahoma,Verdana, Helvetica, Arial, sans-serif;
  font-size: 70%;
  text-align: center;
  margin: 0;
}

<% if (Include.isCompactLayout(wiContext)) { %>

#mainPane h3 {
  font-size: 180%;
  font-weight: normal;
  margin: 0;
  padding: 50px 0 0;
}

#mainPane p {
  font-size: 120%;
  letter-spacing: 2px;
  margin: 0;
  padding: 0 0 20px;
}

<% } else { %>

body {
  background: #566169 url("../media/HorizonBgBottom.png") repeat-x left 325px;
}

#horizonTop {
  height: 325px;
  background: #FDFDFD url("../media/HorizonBgTop.png") no-repeat top left;
}

#mainPane {
  position: relative;
  top: -120px;
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Devices.png")%>) no-repeat center 30px;
  color: white;
}

#mainPane h3 {
  font-size: 160%;
  font-weight: normal;
  margin: 38px 0;
}

#mainPane img {
  margin-bottom: 36px;
}

#actionPane {
  width: 23em;
}

td.glowBoxLeft {
  width:20px;
}
td.glowBoxRight {
  width:20px;
}
td.glowBoxTop {
  height:20px;
}
td.glowBoxFooter {
  height:51px;
}
td.glowBoxMid {
  background: #73808C;
  padding: 7px 30px;
}

td.glowBoxTopLeft {
  background-position:bottom right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopLeftBorderGlow.png")%>);
}

td.glowBoxTopMid .leftGradient {
  background-position:top left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopLeftGradient.png")%>);
  height: 20px;
}

td.glowBoxTopMid .rightGradient {
  background-position:top right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopRightGradient.png")%>);
  height: 20px;
  margin-left: 96px;
}

td.glowBoxTopMid .centerGradient {
  background-position:top left;
  background-repeat:repeat-x;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopMidBorderGlow.png")%>);
  height: 20px;
  margin-right: 96px;
}

td.glowBoxTopRight {
  background-position:bottom left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopRightBorderGlow.png")%>);
}

td.glowBoxMidLeft {
  background-position:center left;
  background-repeat:repeat-y;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneCenterLeftBorderGlow.png")%>);
}

td.glowBoxMidRight {
  background-position:center right;
  background-repeat:repeat-y;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneCenterRightBorderGlow.png")%>);
}

td.glowBoxFooterLeft {
  background-position:top left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterLeftBorderGlow.png")%>);
}

td.glowBoxFooterMid {
  background-position:top center;
  background-repeat:repeat-x;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterMidBorderGlow.png")%>);
}

td.glowBoxFooterRight {
  background-position:top right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterRightBorderGlow.png")%>);
}

<% } %>
