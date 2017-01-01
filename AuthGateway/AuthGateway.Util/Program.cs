/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 10/07/2014
 * Time: 17:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using AuthGateway.Shared;
using ManyConsole;

namespace AuthGateway.Util
{
	class Program
	{
		public static void Main(string[] args)
		{
			var commands = GetCommands();
			ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
		}

		static IEnumerable<ConsoleCommand> GetCommands()
		{
			return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
		}
	}

	class Util
	{
		public Util(string configFile)
		{
			var sc = new SystemConfiguration();
			sc.LoadSettingsFromFile(configFile, false);
		}
	}






}