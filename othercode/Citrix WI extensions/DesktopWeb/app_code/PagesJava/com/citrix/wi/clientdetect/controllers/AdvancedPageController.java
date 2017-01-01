/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;
import java.io.StringWriter;
import java.util.Iterator;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.AdvancedViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the advancedPage
 */
public class AdvancedPageController extends Controller {

    private AdvancedViewModel viewModel = new AdvancedViewModel();

    public AdvancedPageController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        // may need to get rid of the infobar by reloading
        String reloaded = (String)wizardContext.getModel().getAttribute(WizardConstants.RELOADED);
        if ((reloaded != WizardConstants.VAL_TRUE) && wizardContext.getClientInfo().isIE()) {
            wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_TRUE);
            // do not render the page if it is reloading
            viewModel.reloadPage = true;
            viewModel.transientPage = true;
            // set the page title
            viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "AdvancedPageTitle");
            return false;
        } else {
            wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_FALSE);
        }

        // update the client result
        String cookies = (String)wizardContext.getWebAbstraction().getRequestHeader("Cookie");
        wizardContext.getModel().updateClientsResult(cookies);

        // clear the show zone page attribute if exists
        wizardContext.getModel().setAttribute(WizardConstants.SHOW_ZONE_PAGE_ONLY, null);

        // decide how the page should appear
        setupViewModel();

        return true;
    }

    private void setupViewModel() {
        // get the list of clients to display
        viewModel.clients = wizardContext.getInputs().getRemoteClients();

        // change the UI based on the selected client
        Client preferredClient = wizardContext.getInputs().getPreferredClient();
        viewModel.showAutoAsSelected = preferredClient.isAutoDetected();

        // show the asterisk if one of the clients
        // has to show the zone link
        viewModel.showAsterisk = false;
        viewModel.showAvailableTable = false;
        viewModel.showNotAvailableTable = false;
        Iterator it = viewModel.clients.iterator();
        while (it.hasNext()) {
            ClientType clientType = (ClientType)it.next();
            viewModel.showAsterisk = viewModel.showAsterisk || showZoneLink(clientType, wizardContext);
            if (wizardContext.getModel().isClientAvailable(clientType)) {
                viewModel.showAvailableTable = true;
            } else {
                viewModel.showNotAvailableTable = true;
            }
        }

        // set the page title
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "AdvancedPageTitle");
    }

    /**
     * If true the client is selected in the drop down, if false the it should
     * not be selected
     *
     * @param clientType
     * @param wizardContext
     * @return
     */
    public static boolean isClientSelected(ClientType clientType, WizardContext wizardContext) {
        Client preferredClient = wizardContext.getInputs().getPreferredClient();
        boolean show = preferredClient.getClientType().equals(clientType) && !preferredClient.isAutoDetected();
        return show;
    }

    /**
     * This generates the markup for the inline help popup that
     * describes all the clients.
     *
     * @param wizardContext
     * @param viewModel
     * @return the markup
     */
    public static String getClientInlineHelpMarkup(WizardContext wizardContext, AdvancedViewModel viewModel) {
        StringWriter writer = new StringWriter();
        Iterator clientsForTable = viewModel.clients.iterator();
        for (int i=0; clientsForTable.hasNext(); i++) {
            ClientType client = (ClientType) clientsForTable.next();
            writer.write("<p><strong>");
            writer.write(wizardContext.getString(client.getName()));
            writer.write("</strong></p>");
            writer.write("<p>");
            writer.write(wizardContext.getString(client.getName() + "ClientDescription"));
            writer.write("</p>");
        }
        return writer.toString();
    }

    /**
     * Gets the rows of available clients for the table
     *
     * @return the markup
     */
    public static String getAvailableClientRows(WizardContext wizardContext, AdvancedViewModel viewModel) {
        String table = "";
        Iterator clientsForTable = viewModel.clients.iterator();
        for (int i=0; clientsForTable.hasNext(); i++) {
            ClientType client = (ClientType) clientsForTable.next();
            if(wizardContext.getModel().isClientAvailable(client)) {
                table += getClientTableRow(wizardContext, client);
            }
        }
        return table;
    }

    /**
     * Gets the rows of non-available clients for the table
     *
     * @return the markup
     */
    public static String getNotAvailableClientRows(WizardContext wizardContext, AdvancedViewModel viewModel) {
        String table = "";
        Iterator clientsForTable = viewModel.clients.iterator();
        for (int i=0; clientsForTable.hasNext(); i++) {
            ClientType client = (ClientType) clientsForTable.next();
            if(!wizardContext.getModel().isClientAvailable(client)) {
                table += getClientTableRow(wizardContext, client);
            }
        }
        return table;
    }

    /**
     * Get the markup for a client row for the table
     * In each row there are two cells.
     * The first is the client name, the section has links
     * to help fix the client.
     * The links are either Deploy or Upgrade and/or Modify
     */
    private static String getClientTableRow(WizardContext wizardContext, ClientType client) {
        StringWriter writer = new StringWriter();
        writer.write("<tr><td></td><td class=\"clientColumn\">");
        writer.write(wizardContext.getString(client.getName()));
        writer.write("</td><td class=\"linkColumn\">");

        if (AdvancedPageController.showEnableLink(client, wizardContext)) {
            writer.write("<a id=\"clientLink_");
            writer.write(client.getName());
            writer.write("\" href=\"");
            writer.write(client.getPage());
            writer.write("\" title=\"");
            writer.write(wizardContext.getString("EnableExplained"));
            writer.write("\">");
            writer.write(wizardContext.getString("Enable"));
            writer.write("</a>");
        } else {
            if (AdvancedPageController.showUpgradeLink(client, wizardContext)) {
                writer.write("<a id=\"upgradeClientLink_");
                writer.write(client.getName());
                writer.write("\" href=\"");
                writer.write(WizardUtil.getUrlWithQueryStr(client.getPage(), WizardConstants.UPGRADE_NOW, WizardConstants.VAL_TRUE));
                writer.write("\" title=\"");
                writer.write(wizardContext.getString("UpgradeExplained"));
                writer.write("\">");
                writer.write(wizardContext.getString("Upgrade"));
                writer.write("</a>");
            }
            if (AdvancedPageController.showZoneLink(client, wizardContext)) {
                if(AdvancedPageController.showUpgradeLink(client, wizardContext)) {
                    writer.write("|");
                }
                writer.write("<a id=\"modifyClientLink_");
                writer.write(client.getName());
                writer.write("\" href=\"");
                writer.write(WizardUtil.getUrlWithQueryStr(WizardConstants.PAGE_NATIVE, WizardConstants.SHOW_ZONE_PAGE_ONLY, WizardConstants.VAL_TRUE));
                writer.write("\" title=\"");
                writer.write(wizardContext.getString("ModifyExplained"));
                writer.write("\">");
                writer.write(wizardContext.getString("Modify"));
                writer.write("<span class=\"AsteriskText\">*</span></a>");
            }
        }
        writer.write("</td></tr>");

        return writer.toString();
    }

    /**
     * For the given client, dectides if the enable link should be shown
     *
     * @param clientType
     * @param wizardContext
     * @return
     */
    private static boolean showEnableLink(ClientType clientType, WizardContext wizardContext) {
        return !wizardContext.getModel().isClientAvailable(clientType);
    }

    /**
     * If true the upgrade link should be shown
     *
     * @param clientType
     * @param wizardContext
     * @return
     */
    private static boolean showUpgradeLink(ClientType clientType, WizardContext wizardContext) {
        return !showEnableLink(clientType, wizardContext) && wizardContext.getModel().isClientUpgradeable(clientType)
                        && wizardContext.getInputs().getShowUpgradePage();
    }

    /**
     * If true the zone link should be shown
     *
     * @param clientType
     * @param wizardContext
     * @return
     */
    private static boolean showZoneLink(ClientType clientType, WizardContext wizardContext) {
        return ClientType.NATIVE.equals(clientType) && !showEnableLink(clientType, wizardContext)
                        && !wizardContext.getModel().isCorrectZoneForNative()
                        && wizardContext.getInputs().getShowZonePage();
    }
}
