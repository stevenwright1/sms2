// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.

using System;
using System.IO;

using com.citrix.wing;
using com.citrix.wing.util;

/**
 * Class to allow use of System.IO.FileSystemWatcher whilst failing silently if required privileges are not present.
 */
public class FileSystemWatcherWrapper {

    public static FileSystemWatcher Create(SimpleDelegate sd, ErrorEventHandler errorHandler, string path, string filter, bool subdirs, StaticEnvironmentAdaptor staticEnvAdaptor, ResourceBundleFactory bundleFactory) {
        // Only attempt to create a watcher for paths that exist
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            return null;
        }

        FileSystemWatcher fileWatcher = null;
        try {
            fileWatcher = CreateInternal(sd, errorHandler, path, filter, subdirs);
        } catch (System.Security.SecurityException) {
            // fail silently
        } catch (System.ArgumentException) {
            // fail silently. This exception can be thrown, for example, if path in CreateInternal does not exist.
        } catch {
            // fail silently. Some other unknown error prevented the file watcher from being created.
        }
        if (fileWatcher == null)
        {
            staticEnvAdaptor.getDiagnosticLogger().log(
                MessageType.ERROR,
                new LocalizableString(bundleFactory, "ErrorCreatingFileWatcher", path));
        }
        return fileWatcher;
    }

    // LinkDemands will cause SecurityExceptions when the method is JITed which won't happen until it is called
    private static FileSystemWatcher CreateInternal(SimpleDelegate sd, ErrorEventHandler errorHandler, string path, string filter, bool subdirs) {
        SimpleDelegateWrapper sdw = new SimpleDelegateWrapper(sd);
        FileSystemEventHandler fseh = new FileSystemEventHandler(sdw.callDelegate);
        RenamedEventHandler reh = new RenamedEventHandler(sdw.callRenameDelegate);

        FileSystemWatcher fsw = new FileSystemWatcher();
        fsw.Path = path;
        fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        fsw.Filter = filter;
        fsw.IncludeSubdirectories = subdirs;
        fsw.Changed += fseh;
        fsw.Created += fseh;
        fsw.Deleted += fseh;
        fsw.Renamed += reh;
        fsw.Error += errorHandler;
        fsw.EnableRaisingEvents = true;
        return fsw;
    }

}
