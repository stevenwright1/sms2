// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace com.citrix.wi.ui
{
    public class BaseMasterPage : MasterPage
    {
        private String pageTitle;
        private String pageID;
        private bool transientPage = false;
        private bool horizonLayoutPage = false;

        public String PageTitle
        {
            get { return pageTitle; }
            set { pageTitle = value; }
        }

        public String PageID
        {
            get { return pageID; }
            set { pageID = value; }
        }

        // If the page is only transient (like some of the detection pages)
        // modify the master page UI to make it look reasonable.
        public bool TransientPage
        {
            get { return transientPage; }
            set { transientPage = value; }
        }

        public bool HorizonLayoutPage
        {
            get { return horizonLayoutPage; }
            set { horizonLayoutPage = value; }
        }
    }
}
