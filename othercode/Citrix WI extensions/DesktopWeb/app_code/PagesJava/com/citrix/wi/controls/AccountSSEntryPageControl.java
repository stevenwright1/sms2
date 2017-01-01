/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.accountselfservice.AccountTask;

import java.util.HashSet;

/**
 * Maintains presentation state for the Account Self Service Entry Page.
 */
public class AccountSSEntryPageControl extends PageControl {
    private HashSet allowedTasks = new HashSet();

    /**
     * Tests whether a user is allowed to perform the given task using account
     * self service.
     * @param task a task as an <code>AccountTask</code> object
     * @return <code>true</code> if the task is permitted, else <code>false</code>
     */
    public boolean isTaskAllowed( AccountTask task ) {
        return allowedTasks.contains(task);
    }

    /**
     * Adds the given task to the set of allowed account self service tasks.
     * @param task a task as an <code>AccountTask</code> object
     */
    public void setTaskAllowed( AccountTask task ) {
        allowedTasks.add( task );
    }

    /**
     * Gets the number of different tasks that a user may perform using
     * account self service.
     * @return the number of tasks
     */
    public int getAllowedTasksCount() {
        return allowedTasks.size();
    }
}
