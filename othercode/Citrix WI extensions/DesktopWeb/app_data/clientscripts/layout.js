<%
// layout.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
// Adjusts the side margins and the position of the footer area based on the browser window.
// If the page contains a lightbox, the lightbox position is recalculated and the overlay mask resized.
%>
function updateLayout() {
<%
if(!Include.isCompactLayout(wiContext)) {
%>
     if (document.getElementById) {
        var viewportSize = getFrameViewportSize();
        positionFooter(viewportSize[1]);

        if (document.getElementById('lightbox')) {
            configureLightbox();
        }

        <%
        // Hacks for various implementation bugs in IE...
        if (wiContext.getClientInfo().isIE()) {
            int ieVersion = wiContext.getClientInfo().getBrowserVersionMajor();

            // IE 6 does not do min width, so fix it up
            if (ieVersion < 7) {
            %>
//                setOverallWrapperSize(viewportSize[0]);
            <%
            }
            %>
        
            <%
            // IE doesn't reposition tab content correctly
            // if feedback message is longer than one line.
            // It needs to have style re-applied.
            %>
            var el = document.getElementById('selectedTabContent');
            if (el != null)
            {
//                el.style.position = "static";
//                el.style.position = "relative";
            }

        <%  
        } // wiContext.getClientInfo().isIE()
        %>
    }
 <%
} // !isCompactLayout
 %>
}

<%
// Increases the bottom padding of the bottomPane div so we push the footer to the bottom of the screen
%>
function positionFooter(viewportHeight) {
     if (viewportHeight > 0) {
        var totalHeight = 0;

        var contentElement = document.getElementById("pageContent");
        if (contentElement) {
            totalHeight += contentElement.offsetHeight;
        }

        var footerElement = document.getElementById("footer");
        var footerHeight = 0;
        if (footerElement) {
            footerHeight = footerElement.offsetHeight;
            totalHeight += footerHeight;
        }

        var space = viewportHeight - totalHeight;

        if (space > 0) {
            var heightFillerElement = document.getElementById("heightFiller");
            if (heightFillerElement) {
                heightFillerElement.style.height = space + 'px';
            }
        }
    } <% // viewportHeight > 0 %>
}

<%
// Used to implement min-width in IE 6 and below
%>
function setOverallWrapperSize(viewportWidth) {
    // get width of wrapper, but leave room for borders either side
    var wrapperWidth = viewportWidth;// - <%=Constants.WRAPPER_BORDER_WIDTH%>;

    // ensure it is within limits
    if (wrapperWidth < <%=Constants.MIN_WRAPPER_WIDTH%>) {
        wrapperWidth = <%=Constants.MIN_WRAPPER_WIDTH%>;
    }
//    if (wrapperWidth > <%=Constants.MAX_WRAPPER_WIDTH%>) {
//        wrapperWidth = <%=Constants.MAX_WRAPPER_WIDTH%>;
//    }

    // save as the best guess for next time
    setItemInCookie("<%=Constants.COOKIE_WRAPPER_WIDTH%>", wrapperWidth);

    var overallWrapper = document.getElementById("overallWrapper");
    if (overallWrapper) {
        overallWrapper.style.width = wrapperWidth + 'px';
    }
}

window.onresize = updateLayout;
