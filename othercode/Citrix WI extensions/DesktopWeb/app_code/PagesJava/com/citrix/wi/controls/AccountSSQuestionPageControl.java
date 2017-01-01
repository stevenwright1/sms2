/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the Account Self Service Question Page.
 */
public class AccountSSQuestionPageControl extends PageControl {
    private int currentQuestionNumber;
    private int questionCount;
    private String currentQuestionText;
    private boolean maskAnswerFields = false;
    private boolean showConfirmationField = false;

    public String continueButtonLabelKey = null;

    /**
     * Gets the number of the current question.
     * @return the number of the current question
     */
    public int getQuestionNumber() {
        return currentQuestionNumber;
    }

    /**
     * Sets the number of the current question.
     * @param num the number of the current question
     */
    public void setQuestionNumber( int num ) {
        currentQuestionNumber = num;
    }

    /**
     * Gets the total question count.
     * @return the total question count
     */
    public int getTotalQuestionCount() {
        return questionCount;
    }

    /**
     * Sets the total question count.
     * @param num the total question count
     */
    public void setTotalQuestionCount( int num ) {
        questionCount = num;
    }

    /**
     * Gets the text of the current question.
     * @return the text of the current question as a string
     */
    public String getQuestionText() {
        return currentQuestionText;
    }

    /**
     * Sets the text of the current question.
     * @param text the text of the current question
     */
    public void setQuestionText( String text ) {
        currentQuestionText = text;
    }

    /**
     * Gets whether answer fields should be masked.
     * @return <code>true</code> if the fields should be masked, else <code>false</code>
     */
    public boolean getMaskAnswerFields() {
        return maskAnswerFields;
    }

    /**
     * Sets whether answer fields should be masked.
     * @param <code>true</code> if the fields should be masked, else <code>false</code>
     */
    public void setMaskAnswerFields(boolean m) {
        maskAnswerFields = m;
    }

    /**
     * Gets whether to display a second confirmation field.
     * @return <code>true</code> if the confirmation field should be displayed, else <code>false</code>
     */
    public boolean getShowConfirmationField() {
        return showConfirmationField;
    }

    /**
     * Sets whether to display a second confirmation field.
     * @param <code>true</code> if the confirmation field should be displayed, else <code>false</code>
     */
    public void setShowConfirmationField(boolean c) {
        showConfirmationField = c;
    }

    /**
     * Gets the type of the input fields used for answers.
     * 
     * @return "text" for clear text input field, or "password" for masked input fields
     */
    public String getAnswerFieldType() {
        if (getMaskAnswerFields()) {
            return PASSWORD_FIELD;
        } else {
            return TEXT_FIELD;
        }
    }

    private String PASSWORD_FIELD = "password";
    private String TEXT_FIELD = "text";
}
