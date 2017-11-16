/*
 * Created by SharpDevelop.
 * Date: 27.09.2017
 * Time: 21:28
 * 
 */
using EventMessenger.Model.ResponseObjects;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using System.Web;
using System.Net;
using System.IO;

using System.Xml.Serialization;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of KMTronicModel.
	/// </summary>
	[XmlRootAttribute("KMTronicModel", IsNullable = false)]
	public class KMTronicModel
	{
		public KMTronicDeviceType DeviceType;
		public ObservableCollection<RelayContact> RelayContactCollection;
		
		public string UserName { get; set; }
		public string Password { get; set; }
		
		public string HostName { get; set; }
		public int PortNumber { get; set; }
		
		public ResponseObjectModel ResponseObjectModel;
		
		/// <summary>
		/// 
		/// </summary>
		public KMTronicModel()
		{
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		public KMTronicModel(KMTronicDeviceType _device, string _hostName = "localhost", int _portNumber = 80, string _userName = "admin", string _password = "admin")
		{
			UserName = _userName;
			Password = _password;
			
			HostName = _hostName;
			PortNumber = _portNumber;
			
			DeviceType = _device;
			
			RelayContactCollection = new ObservableCollection<RelayContact>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void ProcessRelays()
		{
			foreach(RelayContact rel in RelayContactCollection)
			{
				switch(rel.Behavior)
				{
					case KMTronicRelayContactBehavior.RelayContactIsStaticOn:
						rel.SetRelay(HostName, PortNumber, UserName, Password);
						break;
						
					case KMTronicRelayContactBehavior.RelayContactIsStaticOff:
						rel.ResetRelay(HostName, PortNumber, UserName, Password);
						break;
						
					case KMTronicRelayContactBehavior.RelayContactIsTimed:
						rel.Behavior = KMTronicRelayContactBehavior.RelayContactIsTimed;
						rel.SetRelay(HostName, PortNumber, UserName, Password);
						break;
						
					case KMTronicRelayContactBehavior.RelayContactIsToggle:
						if(rel.State)
							rel.ResetRelay(HostName, PortNumber, UserName, Password);
						else
							rel.SetRelay(HostName, PortNumber, UserName, Password);
						break;
				}
			}
		}
	}
}
