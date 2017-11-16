/*
 * Created by SharpDevelop.
 * Date: 09/27/2017
 * Time: 21:59
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Timers;
using Microsoft.Win32;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of RelayContact.
	/// </summary>
	public class RelayContact
	{
		private System.Timers.Timer t;
		
		private string HTTP_Req = null;
		private WebRequest myWebRequest;
		private WebResponse myWebResponse;
		
		private string hostName;
		private int port;
		private string username;
		private string password;
		
		public RelayContact()
		{
			t = new Timer();
			//t.Interval = Time;
			t.Elapsed += Check;
			t.AutoReset = false;
			//t.Start();
		}
		
		public bool State { get; set; }
		
		public KMTronicRelayContactBehavior Behavior { get; set; }
		
		public int RelayNumber { get; set; }
		
		public int Time
		{
			get { return time; }
			set {
				time = value;
			}
		} private int time;

		public async void SetRelay(string _hostName, int _portNumber, string _userName, string _password)
		{
			hostName = _hostName;
			port = _portNumber;
			username = _userName;
			password = _password;
			
			try
			{
				HTTP_Req = "http://" + _hostName + ":" + _portNumber.ToString() + ConvertIntToSetRelay(RelayNumber); //relays.cgi?relay=5
				myWebRequest = WebRequest.Create(HTTP_Req);
				myWebRequest.Credentials = new NetworkCredential(_userName, _password);
				myWebRequest.Method = "POST";
				myWebRequest.ContentType = "";
				myWebRequest.ContentLength = 0;
				myWebResponse = await myWebRequest.GetResponseAsync();

				
				State = true;
				myWebResponse.Dispose();
				myWebRequest = null;
				
				if(Behavior == KMTronicRelayContactBehavior.RelayContactIsTimed)
				{
					
					t.Interval = Time * 1000;
					t.Start();
				}
				
				return;
			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				State = false;
				return;
			}
		}
		
		public async void ResetRelay(string _hostName, int _portNumber, string _userName, string _password)
		{
			try
			{
				HTTP_Req = "http://" + _hostName + ":" + _portNumber.ToString() + ConvertIntToResetRelay(RelayNumber); //relays.cgi?relay=5
				myWebRequest = WebRequest.Create(HTTP_Req);
				myWebRequest.Credentials = new NetworkCredential(_userName, _password);
				myWebRequest.Method = "POST";
				myWebRequest.ContentType = "";
				myWebRequest.ContentLength = 0;
				myWebResponse = await myWebRequest.GetResponseAsync();
				
				State = false;
				t.Stop();
				myWebRequest = null;
				myWebResponse.Dispose();
				return;
				

			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				State = true;
				return;
			}
		}
		
		private string ConvertIntToSetRelay(int relay)
		{
			return string.Format("/FF{0:00}01",relay);
		}
		
		private string ConvertIntToResetRelay(int relay)
		{
			return string.Format("/FF{0:00}00",relay);
		}
		
		private void Check(object sender, ElapsedEventArgs e)
		{
			if(State && Behavior == KMTronicRelayContactBehavior.RelayContactIsTimed)
				ResetRelay(hostName, port, username, password);
			else
				t.Stop();
			
			return;
		}
	}
}
