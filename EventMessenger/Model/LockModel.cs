/*
 * Created by SharpDevelop.
 * Date: 03/01/2017
 * Time: 23:58
 * 
 */
using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace EventMessenger.Model
{
	/// <summary>
	/// Description of LockModel.
	/// </summary>
	[XmlRootAttribute("LockModel", IsNullable = false)]
	public class LockModel
	{
		/// <summary>
		/// 
		/// </summary>
		public LockModel()
		{
			LockName = "";
			LockPHI = "";
			DoorMonitoringTrigger = new ObservableCollection<string>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockPHI"></param>
		/// <param name="lockName"></param>
		public LockModel(string lockPHI, string lockName)
		{
			LockPHI = lockPHI;
			LockName = lockName;
			DoorMonitoringTrigger = new ObservableCollection<string>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockPHI"></param>
		/// <param name="lockName"></param>
		/// <param name="doorMonitoringTrigger"></param>
		public LockModel(string lockPHI, string lockName, string doorMonitoringTrigger)
		{
			LockPHI = lockPHI;
			LockName = lockName;
			DoorMonitoringTrigger = new ObservableCollection<string>();
			if(!DoorMonitoringTrigger.Contains(doorMonitoringTrigger))
				DoorMonitoringTrigger.Add(doorMonitoringTrigger);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string LockPHI { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string LockName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<string> DoorMonitoringTrigger { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string LockNames
		{
			get {return string.Format("Lock Name: {0}; Lock PHI: {1}", LockName, LockPHI);}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object MakeDeepCopy()
		{
			return this.MemberwiseClone();
		}
	}
}
