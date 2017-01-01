// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.

using System;
using System.IO;
using System.Threading;

// the simplest delegate possible - no args and void return type
public delegate void SimpleDelegate();

/**
 * Class to allow SimpleDelegates to be used in place of more complex ones.
 * Currently allows a SimpleDelegate to pretend to be a FileSystemEventHandler.
 * Required as ConfigurationProvider cannot reference FileSystemEventArgs.
 */
public class SimpleDelegateWrapper {

    private SimpleDelegate sd;
    private Timer waiting = null;

    public SimpleDelegateWrapper(SimpleDelegate sd) {
        this.sd = sd;
    }

    /**
     * FileSystemEventHandler delegate.
     * Delay first call by 500ms as FileSystemWatcher generates multiple events even
     * for a simple save. Other calls are ignored until first call has completed.
     */
    public void callDelegate(object sender, FileSystemEventArgs args) {
        startWaiting();
    }

    /**
     * RenamedEventHandler delegate.
     */
    public void callRenameDelegate(object sender, RenamedEventArgs args) {
        startWaiting();
    }

    private void startWaiting() {
        lock (this) {
            if (waiting == null) {
                waiting = new Timer(callSimpleDelegate, null, 500, 0);
            }
        }
    }

    private void callSimpleDelegate(object obj) {
        lock (this) {
            try {
                sd();
            }
            finally {
                waiting = null;
            }
        }
    }
}
