<%
// fullApplist.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout()
{
    maintainAccessibility("SearchButton", true);
    setupTreeView();
    setupSearchBox();
    setFocusIfViewChanged();
}

function setFocusIfViewChanged()
{
    if (window.location.href.indexOf("<%=Constants.QSTR_CURRENT_VIEW_STYLE%>=") != -1)
    {
        var el = document.getElementById("viewButton");
        if (el != null)
        {
            el.focus();
        }
    }
}

<%
  // The current tree view folder - we need this when you flip from tree view
  // to another view style to ensure the current folder is displayed.
%>
var treeViewCurrentFolder = "<%=WebUtilities.escapeJavascript(viewControl.treeViewInitialFolder)%>";

<% // Called when the full graphics view style dropdown is changed %>
function changeView(viewName)
{
    var newpage = "<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CURRENT_VIEW_STYLE%>=" + viewName;
    if (isTreeView()) {
        newpage += "&<%=Constants.QSTR_CURRENT_FOLDER%>=" + encodeURIComponent(getTreeViewCurrentFolder());
        <% // Clear the treeviewCurrentFolder cookie as we are leaving the tree view now  %>
        setItemInCookie("<%=Constants.COOKIE_TREE_VIEW_CURRENT_FOLDER%>", null);
    }
    location.href = newpage;
}

<%
  // If we're in tree view, expand the current folder
%>
function setupTreeView()
{
    if (isTreeView()) {
        <% // We're in tree view mode %>
        retrieveImage(document.getElementById('treeView'));
        expandTreeViewInitialFolder();
    }
}

<% // Expands/collapses folder nodes of the tree view %>
function toggleTreeNode(node)
{
    var nodeClass = node.className;
    if(nodeClass == "folderClose"){
        openFolder(node);
        setTreeViewCurrentFolder(node, false);
    } else {
        closeFolder(node);
        setTreeViewCurrentFolder(node, true);
    }
    updateLayout(); <% // in firefox we have to call this manually %>
}

<% // Opens the given folder node of the tree view %>
function openFolder(node) {
    node.className = "folderOpen";
    node.nextSibling.style.display='block';
    retrieveImage(node.nextSibling);
    changeSrcOfChildImgNodes(node, "../media/FolderOpenArrowDown.gif");
}

<% // Closes the given folder node of the tree view %>
function closeFolder(node) {
    node.className = "folderClose";
    node.nextSibling.style.display='none';
    changeSrcOfChildImgNodes(node, "../media/FolderClosedArrow.gif");
}

<%
// Used to change the picture on the mouseover event
// to the one with the glowing triangle
%>
function updateMouseoverTreeNodePicture(node) {
    var nodeClass = node.className;
    if(nodeClass == "folderClose"){
        changeSrcOfChildImgNodes(node, "../media/FolderClosedArrowHover.gif");
    } else {
        changeSrcOfChildImgNodes(node, "../media/FolderOpenArrowDownHover.gif");
    }
}

<%
// Used to change the picture back on the mouseout event
%>
function updateMouseoutTreeNodePicture(node) {
    var nodeClass = node.className;
    if(nodeClass == "folderClose"){
        changeSrcOfChildImgNodes(node, "../media/FolderClosedArrow.gif");
    } else {
        changeSrcOfChildImgNodes(node, "../media/FolderOpenArrowDown.gif");
    }
}

<%
// Change all img tags that are children of node
// to have the given url as their src
%>
function changeSrcOfChildImgNodes(node, url) {
    for (var j = 0; j < node.childNodes.length; j++) {
          var child = node.childNodes[j];
          // if it is an image tag, put the closed image in
          if (child.nodeName == "IMG") {
              child.src = url;
              break;
          }
     }
}

<%
// ===========================================================================================================
// Helper functions
// ===========================================================================================================
%>

<%
// Sets the src attribute for the child resources of the selected folder
// so that the browser can fetch the icon while showing the expanded node.
// The values for the src attribute were initially stored in the name attribute to prevent the extra load when we first display the tree
%>
function retrieveImage(selectedFolder)
{
    for (var i = 0; i <selectedFolder.childNodes.length; i++) {
        var item = selectedFolder.childNodes[i];
        // Iterate through all leaf (i.e., non-folder) <li> nodes and patch up the src attribute in all descendant images.
        if (item.nodeName == "LI" && item.className != "folder") { //found a list item here
            var images = item.getElementsByTagName("IMG");
            for (var j = 0; j < images.length; j++) {
                if (images[j].name != null && images[j].className != "spinner") {
                    images[j].src = images[j].name;
                    break;
                }
            }
        }
    }
}

<%
// This function expands the tree to the current folder.
%>
function expandTreeViewInitialFolder()
{
    var currentFolderValue = getTreeViewCurrentFolder();
    var rootNode = document.getElementById("treeView");
    if ((currentFolderValue != null) && (currentFolderValue != "")) {
        expandTreeView(rootNode, currentFolderValue);
    }
}

<%
// This function expands the tree recursively.
%>
function expandTreeView(ulElement, path)
{
    if (path.indexOf("\\") == 0) { // remove the leading "\" from the path
        path = path.substring(1);
    }

    var value;
    if (path.indexOf("\\") > 0) { // gets the folder name
        value = path.substring(0, path.indexOf("\\"));
    } else {
        value = path;
    }

    if (value == "") { // just return if there are no folder components left
        return;
    }

    for (var i = 0; i <ulElement.childNodes.length; i++) {
        var item = ulElement.childNodes[i];
        if (item.nodeName == "LI") { // found a list item here
            for (var j = 0; j <item.childNodes.length; j++) {
                 var liChildNode = item.childNodes[j];
                 // If the list item is followed by an <a> tag and a <ul> tag
                 // then we know that the first child of the list item is the folder name.
                 if (liChildNode.nodeName == "A" && (liChildNode.nextSibling != null && liChildNode.nextSibling.nodeName == "UL")) {
                     // Check if the folder name matches
                     if (findFolderName(liChildNode) == value) {
                         openFolder(liChildNode);
                         path = path.substring(path.indexOf("\\"));
                         expandTreeView(liChildNode.nextSibling, path);
                     }
                 }
             }
        }
    }
}

function setTreeViewCurrentFolder(node, isCollapsed)
{
      var currentFolder = findFolderName(node);
      var currentNode = node.parentNode.parentNode.parentNode;
      while(currentNode.nodeName == "LI" ) {
         for (var j = 0; j <currentNode.childNodes.length; j++) {
              var liChildNode = currentNode.childNodes[j];
              // If the list item is followed by an <a> tag and a <ul> tag
              // then we know that the first child of the list item is the folder name.
              if (liChildNode.nodeName == "A" && (liChildNode.nextSibling != null && liChildNode.nextSibling.nodeName == "UL")) {
                  currentFolder = findFolderName(liChildNode)+"\\"+currentFolder;
              }
         }
         currentNode = currentNode.parentNode.parentNode;
      }

      if (currentFolder != "" && isCollapsed) {
          <% // collapsed folder - current folder is now the parent %>
          var index = currentFolder.lastIndexOf("\\");
          if(index == -1) {
            currentFolder = "\\";
          } else {
            currentFolder = currentFolder.substring(0, index);
          }
      }
      setItemInCookie("<%=Constants.COOKIE_TREE_VIEW_CURRENT_FOLDER%>", currentFolder);
}

<%
// For the given node that represents a folder
// this function returns the name of that folder
%>
function findFolderName(node) {
    var currentFolder = null;
    for (var j = 0; j < node.childNodes.length; j++) {
      var child = node.childNodes[j];
      // if it a text node (i.e. not the image node)
      // it should be the name of the folder
      if (child.nodeType == 3) {
          currentFolder = child.nodeValue;
          break;
      }
    }
    return currentFolder;
}

<% // Determine if we're in tree view mode %>
function isTreeView()
{
    return document.getElementById('treeView') != null;
}

<%
// This function returns the currently selected folder
%>
function getTreeViewCurrentFolder() {
   <% // First check and see if there is any cookie element present. %>
    var currentFolderValue = getItemFromCookie("<%=Constants.COOKIE_TREE_VIEW_CURRENT_FOLDER%>");
    if (currentFolderValue == null || currentFolderValue == "") {
        currentFolderValue = "<%=WebUtilities.escapeJavascript(viewControl.treeViewInitialFolder)%>";
    }
    return currentFolderValue;
}
