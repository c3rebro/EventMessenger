/*
 * Created by SharpDevelop.
 * Date: 04.10.2017
 * Time: 21:09
 * 
 */
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace arc
{
	class Program
	{
		private static readonly string assemblyPath = Process.GetCurrentProcess().MainModule.FileName;
		
		public static void Main(string[] args)
		{
			bool setValue = (from x in args select x.Contains("set")).Any();
			
			DirectoryInfo rootPath = new DirectoryInfo(assemblyPath);
			string exePath = Path.Combine(rootPath.Parent.FullName, "EventMessenger.exe");
			bool exeExist = File.Exists(exePath);
			
			// Create a subkey named Test9999 under HKEY_CURRENT_USER.
			// The path to the key where Windows looks for startup applications
			RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			if(exeExist)
			{
				try{
					// Check to see the current state (running at startup or not)
					if (setValue && rkApp.GetValue("EventMessenger") == null)
					{
						// Add the value in the registry so that the application runs at startup
						rkApp.SetValue("EventMessenger", exePath + " runSilent");
					}
					else
					{
						// Remove the value from the registry so that the application doesn't start
						rkApp.DeleteValue("EventMessenger", false);
					}
				}
				catch(Exception e)
				{
					return;
				}

			}
		}
	}
}