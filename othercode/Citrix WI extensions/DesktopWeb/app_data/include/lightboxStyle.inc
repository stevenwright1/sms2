<%
// lightboxStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

#lightboxMask {
    display: none;
    position: fixed;
    top: 0px;
    left: 0px;
    background-color: black;
<% if(wiContext.getClientInfo().isIE()) { %>
    filter: alpha(opacity=60);
<% } else { %>
    -moz-opacity: 0.6;
    opacity:.60;
<% } %>
}

<% // Override for IE6 as it does not support position:fixed  %>
* html #lightboxMask {
    position: absolute;
}

#lightbox {
    display: none;
    position: fixed;
    width: 480px;
    left: 50%;
    margin-left: -240px;
    z-index:1;
    overflow: visible;
}

<% // Override for IE6 as it does not support position:fixed  %>
* html #lightbox {
    position: absolute;
}

#lightboxText {
	margin-left: 63px;
}

#closeButton {
	float: right;
	padding-top: 2px;
}

#lightboxTitle {
	float: left;
	padding-top: 1px;
}

#lightboxHeading
{
    text-align: left;
    padding: 0px;
    margin: 15px 0px 10px 0px;
    font-size: 120%;
    font-weight: bold;
}

#lightbox img#logo
{
    margin: 15px 8px 0px 0px;
    float: left;
}

#lightbox p
{
    text-align: left;
    margin-right: 12px;
}

#lightboxMessageBottom
{
    margin-top: 8px;
}

.lightboxButtonPane
{
    margin: 0 2px 8px;
    float: right;
    overflow: hidden;
}

#lightbox td.lightBoxMid {
    width: 400px;
}

table.lightBox {
    cursor: default;
}

td.lightBoxLeft,
td.lightBoxRight {
  width:11px;
}

td.lightBoxTop {
  height: 34px;
}

td.lightBoxFooter {
  height: 54px;
}

td.lightBoxMid {
  background-color: white;
  padding: 7px 10px 35px;
}

td.lightBoxTopLeft {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxTopLeft.png")%>) no-repeat bottom right;
}

td.lightBoxTopMid {
  font-weight: bold;
  text-align: left;
  font-size: 120%;
  padding: 2px 2px 0 10px;
  width: auto;
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxTopBorder.png")%>) repeat-x bottom center;
}

td.lightBoxTopRight {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxTopRight.png")%>) no-repeat bottom left;
}

td.lightBoxMidLeft {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxLeftBorder.png")%>) repeat-y center left;
}

td.lightBoxMidRight {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxRightBorder.png")%>) repeat-y center right;
}

td.lightBoxFooterLeft {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxBottomLeft.png")%>) no-repeat top left;
}

td.lightBoxFooterMid {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxBottomBorder.png")%>) repeat-x top center;
}

td.lightBoxFooterRight {
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxBottomRight.png")%>) no-repeat top right;
}