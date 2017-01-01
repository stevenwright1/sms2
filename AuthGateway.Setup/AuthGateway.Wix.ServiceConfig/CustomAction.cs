using System;
using System.Collections.Generic;
using Microsoft.Deployment.WindowsInstaller;

namespace AuthGateway.Wix.ServiceConfig
{
	public class CustomActions
	{
		private static CustomActionsHandler cah = new CustomActionsHandler();

		[CustomAction]
		public static ActionResult WrightCCS_CA_RemoveOldResidual(Session session)
		{
			return cah.WrightCCS_CA_RemoveOldResidual(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_Before_InstallFinalize_Rollback(Session session)
		{
			return cah.WrightCCS_CA_Before_InstallFinalize_Rollback(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_Before_InstallFinalize(Session session)
		{
			return cah.WrightCCS_CA_Before_InstallFinalize(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_After_InstallFiles_Rollback(Session session)
		{
			return cah.WrightCCS_CA_After_InstallFiles_Rollback(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_After_InstallFiles(Session session)
		{
			return cah.WrightCCS_CA_After_InstallFiles(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_ConfigureAuthEngine(Session session)
		{
			return cah.WrightCCS_CA_ConfigureAuthEngine(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_ConfigureCloudSMS(Session session)
		{
			return cah.WrightCCS_CA_ConfigureCloudSMS(new CASessionAdapter(session));
		}

        [CustomAction]
        public static ActionResult WrightCCS_CA_ConfigureMutualAuthImages(Session session)
        {
            return cah.WrightCCS_CA_ConfigureMutualAuthImages(new CASessionAdapter(session));
        }

		[CustomAction]
		public static ActionResult WrightCCS_CA_ConfigureOATHCalc(Session session)
		{
			return cah.WrightCCS_CA_ConfigureOATHCalc(new CASessionAdapter(session));
		}

		[CustomAction]
		public static ActionResult WrightCCS_CA_ConfigureClients(Session session)
		{
			return cah.WrightCCS_CA_ConfigureClients(new CASessionAdapter(session));
		}

        [CustomAction]
        public static ActionResult WrightCCS_CA_ConfigureCommonProperties(Session session)
        {
            return cah.WrightCCS_CA_ConfigureCommonProperties(new CASessionAdapter(session));
        }

		[CustomAction]
		public static ActionResult WrightCSS_CA_CheckIAS(Session session)
		{
			return cah.WrightCSS_CA_CheckIAS(new CASessionAdapter(session));
		}
	}
}
