using System;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace EventMessenger
{
	/// <summary>
	/// Description of BasicCardSettings.
	/// </summary>
	public class SettingsReaderWriter
	{
		
		#region fields
		readonly string _settingsFileFileName = "settings.xml";
		readonly string _updateConfigFileFileName = "update.xml";
		readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
		readonly string _securityToken = "D68EF3A7-E787-4CC4-B020-878BA649B4CD";
		readonly string _updateURL = @"http://eventmessenger.hyperstack.de/update.xml";
		readonly int _updateInterval = 900;
		readonly string _payload = "update.zip";
		readonly string _baseUri = @"http://eventmessenger.hyperstack.de/download/";

		private string language;
		private bool hasDebugConsole;
		private bool autoCheckForUpdates;
		private bool autoLoadDB;
		private bool autoLoadOnStartup;
		
		private readonly string appDataPath;
		private readonly string startupPath;
		
		/// <summary>
		/// 
		/// </summary>
		public bool AutoCheckForUpdates
		{
			get { ReadSettings(); return autoCheckForUpdates; }
			set { autoCheckForUpdates = value; SaveSettings(); }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string DefaultLanguage
		{
			get	{ ReadSettings(); return language; }
			set { language = value; SaveSettings(); }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool HasDebugConsole
		{
			get { ReadSettings(); return hasDebugConsole;}
			set { hasDebugConsole = value; SaveSettings(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool AutoLoadDB
		{
			get { ReadSettings(); return autoLoadDB; }
			set { autoLoadDB = value; SaveSettings(); }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool AutoLoadOnStartup
		{
			get { ReadSettings(); return autoLoadOnStartup; }
			set { autoLoadOnStartup = value; SaveSettings(); }
		}
		#endregion
		
		#region constructors
		/// <summary>
		/// 
		/// </summary>
		public SettingsReaderWriter()
		{
			try{
				appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				
				appDataPath = Path.Combine(appDataPath, "EventMessenger");
				
				if(!Directory.Exists(appDataPath))
					Directory.CreateDirectory(appDataPath);
			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
			}
			
			
			if(true) //!File.Exists(Path.Combine(appDataPath, _updateConfigFileFileName))
			{
				try {
					XmlWriter writer = XmlWriter.Create(Path.Combine(appDataPath, _updateConfigFileFileName));
					writer.WriteStartDocument();
					writer.WriteStartElement("Manifest");
					writer.WriteAttributeString("version", string.Format("{0}.{1}.{2}",Version.Major,Version.Minor,Version.Build));
					
					writer.WriteEndElement();
					writer.Close();

					XmlDocument doc = new XmlDocument();
					doc.Load(Path.Combine(appDataPath, _updateConfigFileFileName));
					
					if (doc.SelectSingleNode("//CheckInterval") == null) {
						
						XmlElement CheckIntervalElem = doc.CreateElement("CheckInterval");
						XmlElement RemoteConfigUriElem = doc.CreateElement("RemoteConfigUri");
						XmlElement SecurityTokenElem = doc.CreateElement("SecurityToken");
						XmlElement BaseUriElem = doc.CreateElement("BaseUri");
						XmlElement PayLoadElem = doc.CreateElement("Payload");
						
						doc.DocumentElement.AppendChild(CheckIntervalElem);
						doc.DocumentElement.AppendChild(RemoteConfigUriElem);
						doc.DocumentElement.AppendChild(SecurityTokenElem);
						doc.DocumentElement.AppendChild(BaseUriElem);
						doc.DocumentElement.AppendChild(PayLoadElem);
						
						CheckIntervalElem.InnerText = _updateInterval.ToString();
						RemoteConfigUriElem.InnerText = _updateURL;
						SecurityTokenElem.InnerText = _securityToken;
						BaseUriElem.InnerText = _baseUri;
						PayLoadElem.InnerText = _payload;
						
						doc.Save(Path.Combine(appDataPath, _updateConfigFileFileName));
					}
					
					
				} catch (XmlException e) {
					LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
					Environment.Exit(0);
				}
			}
		}
		#endregion
		
		#region methods
		/// <summary>
		/// 
		/// </summary>
		public void SaveSettings() // create the basic settingsfile stucture
		{
			try {
				
				XmlDocument doc = new XmlDocument();
				doc.Load(Path.Combine(appDataPath,_settingsFileFileName));
				
				if (doc.SelectSingleNode("//defaultSettings") == null) {
					XmlElement defaultSettingsElem = doc.CreateElement("defaultSettings");
					XmlAttribute languageAttr = doc.CreateAttribute("defaultLanguage");
					XmlAttribute hasDebugConsoleAttr = doc.CreateAttribute("hasDebugConsole");
					XmlAttribute autoCheckForUpdatesAttr = doc.CreateAttribute("autoCheckForUpdates");
					XmlAttribute autoLoadDBAttr = doc.CreateAttribute("autoLoadDB");
					XmlAttribute autoLoadOnStartupAttr = doc.CreateAttribute("autoLoadOnStartup");
					
					languageAttr.Value = "english";
					autoCheckForUpdatesAttr.Value = "true";
					hasDebugConsoleAttr.Value = "false";
					autoLoadDBAttr.Value = "false";
					autoLoadOnStartupAttr.Value = "false";
					
					defaultSettingsElem.Attributes.Append(languageAttr);
					defaultSettingsElem.Attributes.Append(hasDebugConsoleAttr);
					defaultSettingsElem.Attributes.Append(autoCheckForUpdatesAttr);
					defaultSettingsElem.Attributes.Append(autoLoadDBAttr);
					defaultSettingsElem.Attributes.Append(autoLoadOnStartupAttr);
					
					doc.DocumentElement.AppendChild(defaultSettingsElem);
					doc.Save(Path.Combine(appDataPath,_settingsFileFileName));
				}
				
				else
				{
					try {
						XmlNode node = doc.SelectSingleNode("//defaultSettings");
						node.Attributes["hasDebugConsole"].Value = hasDebugConsole.ToString();
						node.Attributes["autoCheckForUpdates"].Value = autoCheckForUpdates.ToString();
						node.Attributes["defaultLanguage"].Value = language;
						node.Attributes["autoLoadDB"].Value = autoLoadDB.ToString();
						node.Attributes["autoLoadOnStartup"].Value = autoLoadOnStartup.ToString();
						
						doc.Save(Path.Combine(appDataPath,_settingsFileFileName));
						
					} catch (XmlException e) {
						LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
						Environment.Exit(0);
					}
				}
				
				ReadSettings();
				
			} catch (XmlException e) {
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				Environment.Exit(0);
			}
		}
		
		public void ReadSettings()
		{
			try{
				if (!File.Exists(Path.Combine(appDataPath,_settingsFileFileName))) {
					XmlWriter writer = XmlWriter.Create(Path.Combine(appDataPath,_settingsFileFileName));
					writer.WriteStartDocument();
					writer.WriteStartElement("EventMessengerSettings");
					
					writer.WriteEndElement();
					writer.Close();
					
					SaveSettings();
					
				}
				
				else if (File.Exists(Path.Combine(appDataPath,_settingsFileFileName))) {
					
					XmlDocument doc = new XmlDocument();
					doc.Load(Path.Combine(appDataPath,_settingsFileFileName));
					
					try {
						XmlNode node = doc.SelectSingleNode("//defaultSettings");
						
						language = node.Attributes["defaultLanguage"].Value;
						hasDebugConsole = Convert.ToBoolean(node.Attributes["hasDebugConsole"].Value);
						autoCheckForUpdates = Convert.ToBoolean(node.Attributes["autoCheckForUpdates"].Value);
						autoLoadDB = Convert.ToBoolean(node.Attributes["autoLoadDB"].Value);
						autoLoadOnStartup = Convert.ToBoolean(node.Attributes["autoLoadOnStartup"].Value);
						
					} catch (XmlException e) {
						LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
						Environment.Exit(0);
					}
				}
			}
			
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
			}

		}
		#endregion
	}
}
