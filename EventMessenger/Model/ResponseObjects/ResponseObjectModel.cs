/*
 * Created by SharpDevelop.
 * Date: 03/13/2017
 * Time: 13:33
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of ResponseObjectModel.
	/// </summary>
	[XmlRootAttribute("ResponseObjectModel", IsNullable = false)]
	[XmlInclude(typeof(TelegramMessageObjectModel))]
	[XmlInclude(typeof(EMailResponseObjectModel))]
	[XmlInclude(typeof(KNXInterface))]
	[XmlInclude(typeof(KMTronicModel))]
	public class ResponseObjectModel
	{
		/// <summary>
		/// 
		/// </summary>
		public ResponseObjectModel()
		{
			ResponseObjectID = new Random().Next();
			Scheduler = new Scheduler();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="emailObj"></param>
		public ResponseObjectModel(EMailResponseObjectModel emailObj)
		{
			ResponseObject = emailObj;
			Scheduler = new Scheduler();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="kmTronicObj"></param>
		public ResponseObjectModel(KMTronicModel kmTronicObj)
		{
			ResponseObject = kmTronicObj;
			Scheduler = new Scheduler();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="telegramObj"></param>
		public ResponseObjectModel(TelegramMessageObjectModel telegramObj)
		{
			ResponseObject = telegramObj;
			Scheduler = new Scheduler();
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public int ResponseObjectID	{ get; set;	}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public bool HasConfiguration { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public bool HasSchedule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string ResponseName	{ get; set;	}
		
		/// <summary>
		/// 
		/// </summary>
		public string ResponseDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string ResponseType	{ get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public object ResponseObject {	get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public Scheduler Scheduler { get; set; }
		
		/// <summary>
		/// Provides a Method to Compare Instantiated EventObjectModels with Different ones from a static List
		/// </summary>
		/// <param name="obj">The object to check for equality</param>
		/// <returns>true if 'this' and obj are the same. false otherwise</returns>
		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is ResponseObjectModel))
				return false;

			ResponseObjectModel tempEventObject = (obj as ResponseObjectModel);
			
			if(tempEventObject.ResponseDescription == ResponseDescription
			   && tempEventObject.ResponseName == ResponseName
			   && tempEventObject.ResponseObjectID == ResponseObjectID
			   && tempEventObject.ResponseObject.GetType().Name == ResponseObject.GetType().Name)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Generates a hash code to identify each event. nessassary to check for equality
		/// </summary>
		/// <returns>Integer Hash of relevant properties</returns>
		public override int GetHashCode()
		{
			return ResponseObjectID.GetHashCode();
		}
	}
}
