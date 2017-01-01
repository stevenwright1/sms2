using System;
using System.Collections.Generic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;

namespace SMSService
{
	public class ServerLogic
	{
		public String _strPost;

		private SystemConfiguration sc;

		public ActionerInstance Actioner { get; private set; }

		private Dictionary<string, Type> modulesTypesByModuleName;
		private Dictionary<string, CloudSMSModuleConfig> modulesConfigsByModuleName;

		private static String _startUpPath = String.Empty;

		#region Class Constructors
		public ServerLogic(SystemConfiguration sc)
		{
			this.sc = sc;
		}
		#endregion

		public void Init()
		{
			Logger.Instance.WriteToLog("ServerLogic.Init", LogLevel.Debug);
			this.Stop();
			Logger.Instance.WriteToLog("ServerLogic.Init End", LogLevel.Debug);
			this.modulesTypesByModuleName = new Dictionary<string, Type>();
			this.modulesConfigsByModuleName = new Dictionary<string, CloudSMSModuleConfig>();

			Logger.Instance.WriteToLog("Populating Actioner", LogLevel.Info);
			this.Actioner = new ActionerInstance();
			this.Actioner.Add<GetModules>(new ActionerInstance.CommandAction(this.GetModules));
			this.Actioner.Add<SendSms>(new ActionerInstance.CommandAction(this.SendSMSMessage));

			try
			{
				var firstModule = sc.CloudSMSConfiguration.CloudSMSModules[0];
				var module = Get(firstModule.ModuleName);
				module.SendSMSMessage(new SendSms() { 
					Authenticated = true, Code = "Code", 
					Destination = "Destination", Message = "Message", 
					ModuleName = firstModule.ModuleName });
			}
			catch
#if DEBUG
 (Exception ex)
#endif
			{
#if DEBUG
				Logger.Instance.WriteToLog(string.Format("ServerLogic.Init FirstModule ERROR: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.Init FirstModule STACK: {0}", ex.StackTrace), LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
#endif
            }
		}

		public void Stop()
		{
			this.modulesTypesByModuleName = new Dictionary<string, Type>();
			this.modulesConfigsByModuleName = new Dictionary<string, CloudSMSModuleConfig>();
			if (this.Actioner != null)
				this.Actioner.Clear();
		}

		public RetBase GetModules(CommandBase cmd)
		{
			var command = (GetModules)cmd;
			var ret = new GetModulesRet();

			try
			{
				ret.Modules = new List<string>();
#if DEBUG
				Logger.Instance.WriteToLog(string.Format("ServerLogic.GetModules Count: {0}", sc.CloudSMSConfiguration.CloudSMSModules.Count), LogLevel.Error);
#endif
				foreach (var mc in this.sc.CloudSMSConfiguration.CloudSMSModules)
					ret.Modules.Add(mc.ModuleName);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("ServerLogic.GetModules ERROR: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("ServerLogic.GetModules STACK: {0}", ex.StackTrace), LogLevel.Debug);
				ret.Error = "Error getting modules list.";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}

			return ret;
		}

		public RetBase SendSMSMessage(CommandBase cmd)
		{
			var command = (SendSms)cmd;
			return this.Get(command.ModuleName).SendSMSMessage(command);
		}

		public IModule Get(string moduleName)
		{
			var moduleConfig = GetModuleConfigByModuleName(moduleName);

			if (!this.modulesTypesByModuleName.ContainsKey(moduleConfig.ModuleName))
				this.modulesTypesByModuleName.Add(moduleConfig.ModuleName, null);

			if (this.modulesTypesByModuleName[moduleConfig.ModuleName] == null)
			{
				var type = GetTypeByModuleName(moduleConfig.ModuleName);
				this.modulesTypesByModuleName[moduleConfig.ModuleName] = type;
			}
			var moduleObject = (IModule)Activator.CreateInstance(this.modulesTypesByModuleName[moduleConfig.ModuleName]);
			moduleObject.SetConfiguration(sc);
			moduleObject.SetModuleConfig(moduleConfig);

			return moduleObject;
		}

		private Type GetTypeByModuleName(string moduleName)
		{
			var moduleConfig = GetModuleConfigByModuleName(moduleName);
			var moduleClassName = moduleConfig.TypeName;
			if (!moduleClassName.StartsWith("SMSService.Modules."))
				moduleClassName = "SMSService.Modules." + moduleClassName;

			var type = Type.GetType(moduleClassName);
			if (type == null)
				throw new Exception(string.Format("Class '{0}' not found.", moduleName));
			return type;
		}

		private CloudSMSModuleConfig GetModuleConfigByModuleName(string moduleName)
		{
			if (sc.CloudSMSConfiguration.CloudSMSModules.Count == 0)
				throw new Exception(string.Format("Empty CloudSMSModules list, '{0}' not found.", moduleName));

			if (string.IsNullOrEmpty(moduleName))
				moduleName = sc.CloudSMSConfiguration.CloudSMSModules[0].ModuleName;

			if (this.modulesConfigsByModuleName.ContainsKey(moduleName))
				return this.modulesConfigsByModuleName[moduleName];

			foreach (var mc in sc.CloudSMSConfiguration.CloudSMSModules)
			{
				if (mc.ModuleName == moduleName)
				{
					this.modulesConfigsByModuleName.Add(moduleName, mc);
					break;
				}
			}

			if (!this.modulesConfigsByModuleName.ContainsKey(moduleName))
			{
				if (sc.CloudSMSConfiguration.CloudSMSModules.Count > 0)
				{
					Logger.Instance.WriteToLog(string.Format("Configuration for moduleName '{0}' not found, using first one available.", moduleName), LogLevel.Debug);
					return sc.CloudSMSConfiguration.CloudSMSModules[0];
				}
				throw new Exception(string.Format("Configuration for moduleName '{0}' not found.", moduleName));
			}

			return this.modulesConfigsByModuleName[moduleName];
		}
	}
}
