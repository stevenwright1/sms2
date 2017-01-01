package com.citrix.wi.mvc;

/**
 * Summary description for ActionState.
 */
public class ActionState {
    private boolean showForm;
    private StatusMessage statusMessage;

    public ActionState(boolean showForm, StatusMessage statusMessage) {
        this.showForm = showForm;
        this.statusMessage = statusMessage;
    }

    public ActionState(boolean showForm) {
        this(showForm, null);
    }

    public ActionState(StatusMessage statusMessage) {
        this(false, statusMessage);
    }

    public boolean getShowForm() {
        return showForm;
    }

    public StatusMessage getStatusMessage() {
        return statusMessage;
    }
}
