/*
 * Created by SharpDevelop.
 * Date: 02/28/2017
 * Time: 23:21
 * 
 */
using System;
using System.Collections.Generic;

namespace EventMessenger.EventObjects
{
	/// <summary>
	/// Contains Event related settings
	/// </summary>
	public class EventObjectConfiguration
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		
		public EventObjectConfiguration()
		{
		}
		
	}
}
