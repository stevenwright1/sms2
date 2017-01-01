/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 10/07/2014
 * Time: 17:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using ManyConsole;

namespace AuthGateway.Util
{
	abstract class UtilCommands : ConsoleCommand
	{
		protected void help()
		{
			var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			Console.WriteLine(string.Format("Usage: {0} <command> [options]", assemblyName));
			Console.WriteLine();
			Console.WriteLine("Config:");
			Options.WriteOptionDescriptions(Console.Out);
		}
	}
}
