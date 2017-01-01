<%
// fullUtils.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
// Returns the position of an element in pixels relative to the top left
// of our frame.
%>
function getElementPosition(elt)
{
  var positionX = 0;
  var positionY = 0;

  while (elt != null)
  {
    positionX += elt.offsetLeft;
    positionY += elt.offsetTop;
    elt = elt.offsetParent;
  }

  return [positionX, positionY];
}

<%
// Returns the viewport size (of our frame) in pixels
%>
function getFrameViewportSize()
{
  var sizeX = 0;
  var sizeY = 0;

  if (typeof window.innerWidth != 'undefined')
  {
      sizeX = window.innerWidth;
      sizeY = window.innerHeight;
  }
  else if (typeof document.documentElement != 'undefined'
      && typeof document.documentElement.clientWidth != 'undefined'
      && document.documentElement.clientWidth != 0)
  {
      sizeX = document.documentElement.clientWidth;
      sizeY = document.documentElement.clientHeight;
  }
  else
  {
      sizeX = document.getElementsByTagName('body')[0].clientWidth;
      sizeY = document.getElementsByTagName('body')[0].clientHeight;
  }

  return [sizeX, sizeY];
}

<%
/**
 * private
 */

// Returns the scolling position (of our frame) in pixels
%>
function getFrameScrollingPosition()
{
  var scrollX = 0;
  var scrollY = 0;

  if (typeof window.pageYOffset != 'undefined')
  {
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
  }

  else if (typeof document.documentElement.scrollTop != 'undefined'
      && (document.documentElement.scrollTop > 0 ||
      document.documentElement.scrollLeft > 0))
  {
      scrollX = document.documentElement.scrollLeft;
      scrollY = document.documentElement.scrollTop;
  }

  else if (typeof document.body.scrollTop != 'undefined')
  {
      scrollX = document.body.scrollLeft;
      scrollY = document.body.scrollTop;
  }

  return [scrollX, scrollY];
}

<%
// This function changes the close image for the hints area on mouse hover and mouse out events.
// The isMouseHover parameter is true for a mouse hover event and is false for a mouse out event.
%>
function changeCloseImage(currentNode, isMouseHover) {
    for (var j = 0; j <currentNode.childNodes.length; j++) {
        var imgChildNode = currentNode.childNodes[j];
        if (imgChildNode.nodeName == "IMG") {
            if (isMouseHover) {
               imgChildNode.origSrc = imgChildNode.src;
               imgChildNode.src="../media/ActiveClose.gif";
            } else {
               imgChildNode.src= imgChildNode.origSrc;
            }
        }
    }
}
