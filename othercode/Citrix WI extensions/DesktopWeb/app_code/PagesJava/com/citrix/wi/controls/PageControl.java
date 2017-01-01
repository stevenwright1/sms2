/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.HashMap;
import java.util.Map;
import java.util.ArrayList;

import com.citrix.wi.pageutils.Markup;

/**
 * Maintains presentation state for displayed pages.
 *
 * This is a base-class for all page controls.  For all but the simplest
 * pages, this class should be sub-classed to accommodate the needs of the
 * page.
 */
public class PageControl {
    private Map attributes = new HashMap();

    /**
     * Sets a custom attribute.
     *
     * Attributes should not be used as a matter of course for holding presentation
     * state.  Attributes are intended for use when customizing an existing site, as
     * a convenient way to pass additional information.
     *
     * @param name The name of the attribute, must be non-<code>null</code>.
     * @param value The value to store, may be <code>null</code>.
     */
    public void setAttribute( String name, Object value ) {
        if( name == null ) {
            throw new IllegalArgumentException( "Attribute name must be non-null" );
        }

        attributes.put( name, value );
    }

    /**
     * Gets a custom attribute.
     *
     * @param name The name of the attribute to retrieve.
     * @return The value of the attribute, or <code>name</code>
     * @see #setAttribute
     */
    public Object getAttribute( String name ) {
        return attributes.get( name );
    }

    /*
     * Methods to support inline errors.
     */

    // Contains all the form fields which contain invalid data
    private ArrayList invalidFields = new ArrayList(5);

    /**
     * Add the id of a field which contained invalid data.
     * @param field the field
     */
    public void addInvalidField(String field) {
        invalidFields.add(field);
    }

    /**
     * Get the class of the label for the field with the given id.
     * @param field the field
     * @return "invalidField" if this field contained invalid data, otherwise
     * ""
     */
    public String getLabelClass(String field) {
        return invalidFields.contains(field) ? Markup.invalidFieldLabelClass : "";
    }

    /**
     * Get the prefix of a label for the field with the given id.
     * @param field the field
     * @return "*" if this field contained invalid data, otherwise
     * ""
     */
    public String getLabelPrefix(String field) {
        return invalidFields.contains(field) ? Markup.invalidFieldLabelPrefix : "";
    }

    /**
     * Gets the id of the first invalid field.
     * @return the first invalid field, or <code>null</code> if all the fields are valid
     */
    public String getFirstInvalidField() {
        return invalidFields.size() > 0 ? (String)invalidFields.get(0) : null;
    }
}
