/*
 * Created by SharpDevelop.
 * Date: 20.03.2017
 * Time: 20:31
 * 
 */
using System;

namespace EventMessenger.Model
{
	/// <summary>
	/// Description of DoorMonitoringModel.
	/// </summary>
	public class DoorMonitoringModel
	{
		/// <summary>
		/// default constructor
		/// </summary>
		public DoorMonitoringModel()
		{
		}

		public DoorMonitoringModel(DoorMonitoringModel doorMonitoringModel)
		{
			DoorMonitoringEventType = doorMonitoringModel.DoorMonitoringEventType;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string DoorMonitoringEventType
		{
			get { return doorMonitoringEventType;}
			set { doorMonitoringEventType = value;}
		}
		private string doorMonitoringEventType;
	}
}
