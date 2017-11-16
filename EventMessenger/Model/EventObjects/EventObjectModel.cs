/*
 * Created by SharpDevelop.
 * Date: 23.02.2017
 * Time: 00:21
 * 
 */
using GalaSoft.MvvmLight;

using EventMessenger.Model;
using EventMessenger.Model.ResponseObjects;

using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Collections.ObjectModel;


namespace EventMessenger.EventObjects
{
	/// <summary>
	/// Description of EventObjectModel.
	/// </summary>
	[XmlRootAttribute("EventObjectModel", IsNullable = false)]
	public class EventObjectModel : ViewModelBase
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public EventObjectModel()
		{
			responseCollection = new ObservableCollection<ResponseObjectModel>();
			IsDynamicEvent = false;
			
			//selectedResponse = responseCollection[0];
			
			EventObjectID = new Random().Next();
		}

		/// <summary>
		/// Constructor for Unlocking Event Model
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="eventType"></param>
		/// <param name="lockingPlanName"></param>
		/// <param name="transponderName"></param>
		/// <param name="lockName"></param>
		/// <param name="sID"></param>
		/// <param name="transponderPHI"></param>
		/// <param name="lockPHI"></param>
		public EventObjectModel(string eventName, string eventType, string lockingPlanName, string transponderName, string lockName, int sID, string transponderPHI, string lockPHI, string accessType)
		{
			Regex removeNonAlphaNumeric = new Regex("[^a-zA-Z0-9äöü -]");
			
			responseCollection = new ObservableCollection<ResponseObjectModel>();
			//responseList = new ObservableCollection<string>(new String[] {"N/A"});
			TransponderModel = new TransponderModel(removeNonAlphaNumeric.Replace(transponderName,string.Empty).Trim(),
			                                        removeNonAlphaNumeric.Replace(transponderPHI, string.Empty).Trim(),
			                                        new LockModel(
			                                        	removeNonAlphaNumeric.Replace(lockPHI, string.Empty).Trim(),
			                                        	removeNonAlphaNumeric.Replace(lockName, string.Empty).Trim()
			                                        ));
			
			TransponderModel.SelectedTransponderTriggerType = accessType;
			//selectedResponse = responseList[0];
			
			EventName = eventName; EventType = eventType;
			
			_eventType=eventType;
			_eventName=eventName;
			
			switch(eventType)
			{
				case "UnlockingEvent":
					HasConfiguration = false;
					IsDynamicEvent = true;
					break;
			}
		}
		
		/// <summary>
		/// Constructor for DoorMonitoring Event Model
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="eventType"></param>
		/// <param name="lockingPlanName"></param>
		/// <param name="lockName"></param>
		/// <param name="sID"></param>
		/// <param name="lockPHI"></param>
		public EventObjectModel(string eventName, string eventType, string lockingPlanName, string lockName, int sID, string lockPHI)
		{
			Regex removeNonAlphaNumeric = new Regex("[^a-zA-Z0-9äöü -]");
			
			responseCollection = new ObservableCollection<ResponseObjectModel>();
			//responseList = new ObservableCollection<string>(new String[] {"N/A"});
			TransponderModel = new TransponderModel("$dont care",
			                                        "$dont care",
			                                        new LockModel(
			                                        	removeNonAlphaNumeric.Replace(lockPHI, string.Empty).Trim(),
			                                        	removeNonAlphaNumeric.Replace(lockName, string.Empty).Trim(),
			                                        	eventType
			                                        ));
			
			//selectedResponse = responseList[0];
			
			EventName = eventName; EventType = eventType;
			
			_eventType=eventType;
			_eventName=eventName;
			
			switch(eventType)
			{
				case "UnlockingEvent":
					HasConfiguration = false;
					IsDynamicEvent = true;
					break;
					
				case "InputEvent":
					HasConfiguration = false;
					IsDynamicEvent = true;
					break;
					
				case "DoorHasBeenOpened":
				case "DoorStaysOpenTooLong":
				case "DoorHasBeenClosed":
				case "DoorHasBeenLocked":
				case "DoorHasBeenSecured":
				case "DoorHasBeenManipulated":
				case "DoorSensorError":
				case "DoorHasBeenClosedAfterTooLongOpened":
					HasConfiguration = false;
					IsDynamicEvent = true;
					break;
			}
		}
		
		/// <summary>
		/// Constructor to create Input Event Model
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="eventType"></param>
		/// <param name="addr"></param>
		/// <param name="inputNumber"></param>
		/// <param name="edgeType"></param>
		public EventObjectModel(string eventName, string eventType, string addr, string inputNumber, string edgeType)
		{
			InputModel = new InputModel(inputNumber, edgeType, addr);
			EventName = eventName;
			EventType = eventType;
		}

		#region public properties
		
		/// <summary>
		/// Auto Property 'TransponderModel'
		/// usage:
		/// 1. injecting nessassary 'dont care' flags into live events to make 'equals' work
		/// </summary>
		public TransponderModel TransponderModel { get; set; }
		
		/// <summary>
		/// Auto Property 'InputModel'
		/// usage:
		/// 1. injecting nessassary 'dont care' flags into live events to make 'equals' work
		/// </summary>
		public InputModel InputModel { get; set; }
		
		#endregion
		
		#region Event Related Properties
		
		/// <summary>
		/// 
		/// </summary>
		public int EventObjectID
		{
			get; set;
		}
		
		private bool _hasConfiguration;
		public bool HasConfiguration
		{
			get { return _hasConfiguration; }
			set { _hasConfiguration = value; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsDynamicEvent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsSelected
		{
			get { return isSelected; }
			set {
				isSelected = value;
				RaisePropertyChanged("IsSelected");
			}
		} private bool isSelected;
		
		private bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set { _isEnabled = value;
				RaisePropertyChanged("IsEnabled");
			}
		}
		
		private string _eventName;
		public string EventName
		{
			get { return _eventName;}
			set { _eventName = value;
				RaisePropertyChanged("EventName");
			}
		}

		private string _eventDescription;
		public string EventDescription
		{
			get { return _eventDescription;}
			set { _eventDescription = value;
				RaisePropertyChanged("EventDescription");
			}
		}
		
		private string _eventType;
		public string EventType
		{
			get { return _eventType; }
			set { _eventType = value;
				RaisePropertyChanged("EventType");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<ResponseObjectModel> ResponseCollection
		{
			get { return responseCollection; }
			set { responseCollection = value;
				RaisePropertyChanged("SelectedResponse");
				RaisePropertyChanged("ResponseCollection");
			}
		} private ObservableCollection<ResponseObjectModel> responseCollection;

		#endregion
		
		#region eventmodelbased datasource for datagrid cells
		
		/// <summary>
		/// 
		/// </summary>
		public string LastSeenDataGridRowContent {
			get { return lastSeen; }
			set { lastSeen = value; RaisePropertyChanged("LastSeenDataGridRowContent"); }
		} private string lastSeen;

		#endregion
		
		#region eyualitycomparer member implementations
		
		/// <summary>
		/// Provides a Method to Compare Instantiated EventObjectModels with Different ones from a static List
		/// </summary>
		/// <param name="obj">The object to check for equality</param>
		/// <returns>true if 'this' and obj are the same. false otherwise</returns>
		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is EventObjectModel))
				return false;
			
			EventObjectModel tempEventObject = (obj as EventObjectModel);
			
			switch(tempEventObject.EventType)
			{
				case "UnlockingEvent":
					
					if ((obj as EventObjectModel).TransponderModel != null && (this.IsDynamicEvent || tempEventObject.IsDynamicEvent))
					{
						if((this.TransponderModel.TransponderName != null) && this.TransponderModel.TransponderName == "$dont care" && this.TransponderModel.TransponderPHI == "$dont care" && this.TransponderModel.SelectedTransponderTriggerType == "TriggerBoth")
						{
							foreach(LockModel lm in this.TransponderModel.LockModels)
							{
								if((lm.LockName != null) && lm.LockName == "$dont care" && lm.LockPHI == "$dont care")
									return true;
								else
								{
									/*
									 * Make sure x.Equals(y) == y.Equals(x)
									 */
									if((tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ').Contains(lm.LockName.ToLower().Replace('"',' '))
									    || lm.LockName.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ')))
									   
									   &&
									   
									   ((tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' ').Contains(lm.LockPHI.ToLower().Replace('"',' ')))
									    || lm.LockPHI.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' '))))
										
										return true;
								}
							}
							return true;
						}
						else if((this.TransponderModel.TransponderName != null) && this.TransponderModel.TransponderName != "$dont care" || this.TransponderModel.TransponderPHI != "$dont care" || this.TransponderModel.SelectedTransponderTriggerType != "TriggerBoth")
						{
							/*
							 * Make sure x.Equals(y) == y.Equals(x)
							 */
							if(((tempEventObject.TransponderModel.TransponderName.ToLower().Replace('"',' ').Contains(this.TransponderModel.TransponderName.ToLower().Replace('"',' ')))
							    || (this.TransponderModel.TransponderName.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.TransponderName.ToLower().Replace('"',' '))))
							   
							   &&
							   
							   ((tempEventObject.TransponderModel.TransponderPHI.ToLower().Replace('"',' ').Contains(this.TransponderModel.TransponderPHI.ToLower().Replace('"',' ')))
							    || this.TransponderModel.TransponderPHI.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.TransponderPHI.ToLower().Replace('"',' ')))
							   
							   && tempEventObject.TransponderModel.SelectedTransponderTriggerType == this.TransponderModel.SelectedTransponderTriggerType)
							{
								foreach(LockModel lm in this.TransponderModel.LockModels)
								{
									if((lm.LockName != null) && lm.LockName == "$dont care" && lm.LockPHI == "$dont care")
										return true;
									else
									{
										/*
										 * Make sure x.Equals(y) == y.Equals(x)
										 */
										if((tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ').Contains(lm.LockName.ToLower().Replace('"',' '))
										    || lm.LockName.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ')))
										   
										   &&
										   
										   ((tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' ').Contains(lm.LockPHI.ToLower().Replace('"',' ')))
										    || lm.LockPHI.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' '))))
											
											return true;
									}
								}
							}
							
							return false;
						}
						else
							return false;
					}
					else
					{
						
						if(EventName == tempEventObject.EventName && EventDescription == tempEventObject.EventDescription && EventType == tempEventObject.EventType)
							return true;
						return false;
					}

				case "InputEvent":
					if(
						InputModel != null && tempEventObject != null &&
						InputModel.Address == tempEventObject.InputModel.Address.Replace("ffff",string.Empty) &&
					   InputModel.InputName == tempEventObject.InputModel.InputName &&
					   
					   (InputModel.InputTriggerType == tempEventObject.InputModel.InputTriggerType
					    || InputModel.InputTriggerType == Enum.GetName(typeof(InputEdgeTriggerType), InputEdgeTriggerType.BothEdges)))
						return true;
					else
						return false;
					
				case "DoorMonitoringEvent":
				case "DoorHasBeenOpened":
				case "DoorStaysOpenTooLong":
				case "DoorHasBeenClosed":
				case "DoorHasBeenLocked":
				case "DoorHasBeenSecured":
				case "DoorHasBeenManipulated":
				case "DoorSensorError":
				case "DoorHasBeenClosedAfterTooLongOpened":
					
					if ((obj as EventObjectModel).TransponderModel != null && (this.IsDynamicEvent || tempEventObject.IsDynamicEvent))
					{
						if((this.TransponderModel != null && this.TransponderModel.LockModels != null) )
						{
							foreach(LockModel lm in this.TransponderModel.LockModels)
							{
								if(lm.LockName == "$dont care" && lm.LockPHI == "$dont care")
								{
									if(tempEventObject.IsDynamicEvent)
									{
										if(lm.DoorMonitoringTrigger.Contains(tempEventObject.TransponderModel.LockModels[0].DoorMonitoringTrigger[0]))
											return true;
									}
								}
								else
								{
									/*
									 * Make sure x.Equals(y) == y.Equals(x)
									 */
									if(((tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ').Contains(lm.LockName.ToLower().Replace('"',' ')) && lm.DoorMonitoringTrigger.Contains(tempEventObject.TransponderModel.LockModels[0].DoorMonitoringTrigger[0]))
									    || (lm.LockName.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockName.ToLower().Replace('"',' ')) && lm.DoorMonitoringTrigger.Contains(tempEventObject.TransponderModel.LockModels[0].DoorMonitoringTrigger[0])))
									   
									   &&
									   
									   ((tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' ').Contains(lm.LockPHI.ToLower().Replace('"',' ')))
									    || lm.LockPHI.ToLower().Replace('"',' ').Contains(tempEventObject.TransponderModel.LockModels[0].LockPHI.ToLower().Replace('"',' '))))
										
										return true;
									return false;
								}
							}
							
						}

						else
							return false;
					}
					else
					{
						
						if(EventName == tempEventObject.EventName && EventDescription == tempEventObject.EventDescription && EventType == tempEventObject.EventType)
							return true;
						return false;
					}
					
					break;
					
				default:
					
					break;
			}
			return false;
		}

		/// <summary>
		/// Generates a hash code to identify each event. nessassary to check for equality
		/// </summary>
		/// <returns>Integer Hash of relevant properties</returns>
		public override int GetHashCode()
		{
			return EventObjectID.GetHashCode();
		}
		
		#endregion
	}
	

}
