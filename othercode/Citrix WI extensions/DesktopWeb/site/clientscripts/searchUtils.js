<%
// searchUtils.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
// Clicking the search button will call into this function.
%>
function redirectToSearchResults() {
    var searchTextbox = document.getElementById('searchEntry');
    var searchText;

    if (searchTextbox.className == "searchInactive") {
        return;
    } else if (searchTextbox.hasFocus) {
        searchText = searchTextbox.value;
    } else {
        searchText = currentSearchQuery;
    }
    window.location.href = "<%=Constants.PAGE_SEARCH_RESULTS%>?CTX_SearchString=" + encodeURIComponent(searchText);
}

<%
// ===========================================================================================================
// Functions relating to the search box
// ===========================================================================================================
%>

<% // The text to show in the search input box when it is selected. %>
var currentSearchQuery = "<%=WebUtilities.escapeJavascript(searchBoxControl.query)%>";

<% // Determine whether the search tab is selected %>
function isSearchPageSelected()
{
    return <%=layoutControl.isSearchPage ? "true" : "false"%>;
}

<%
// The search text box starts out displaying the assistive text.
// On the search page, we should display the current query instead.
%>
function setupSearchBox()
{
    var searchTextbox = document.getElementById('searchEntry');
    if (searchTextbox) {
        if(isSearchPageSelected())
        {
            searchTextbox.focus();
            searchTextbox.value = currentSearchQuery;
            searchTextbox.select();
        }
    }
}

<%
// When the search text box gets focus, highlight the last query string
// and update the search field styling.
%>
function searchTextFocus(searchTextbox) {
    searchTextbox.hasFocus = true;
    searchTextbox.value = currentSearchQuery;
    searchTextbox.select();
    updateSearchStyle(true);
}

<%
// Update the search field styling depending on whether the field is active.
// When the search field is inactive and contains no text, the string "Search"
// is displayed using more subtle styling than when the field is active.
%>
function updateSearchStyle(isActive) {
    var searchField = document.getElementById("searchEntry");
    if (searchField) {
        if (isActive) {
            searchField.className = "";
         } else {
            // Only restore the default string "Search" when the input field is empty
            var currentValue = searchField.value;

            if (currentValue == "") {
                searchField.value = "<%=wiContext.getString("Search")%>";
                searchField.className = "searchInactive";
            }
        }
    }
}

