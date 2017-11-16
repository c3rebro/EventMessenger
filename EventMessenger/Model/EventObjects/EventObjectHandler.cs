/*
 * Created by SharpDevelop.
 * Date: 19.02.2017
 * Time: 21:22
 * 
 */
using EventMessenger;
using EventMessenger.Model;
using EventMessenger.Model.ResponseObjects;

using GalaSoft.MvvmLight.Messaging;

using Hardcodet.Wpf.TaskbarNotification;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;

namespace EventMessenger.EventObjects
{
	/// <summary>
	/// Contains 'dynamic' events as well as manually created 'static' events and handles the list of Responses.
	/// </summary>
	/// <remarks>EventObjectHandler creates and destroys events that occur from outside (lsm software) through pipeserver
	/// and static "eventobjects" that are created manually through Editor. It compares if
	/// a static event fits a "live" event from pipeserver and fires each response object in the list of responses.
	/// </remarks>
	[XmlRootAttribute("EventHandler", IsNullable = false)]
	public class EventObjectHandler
	{
		#region fields and properties

		private EventObjectModel dynamicEventObject; // temporary event from pipeserver
		private readonly List<ResponseObjectModel> responseQueue; // temporary list of all responses to execute after pipe message
		
		private TaskbarIcon tb;
		private Version Version = Assembly.GetExecutingAssembly().GetName().Version;
		
		public string ManifestVersion { get; set; }

		/// <summary>
		/// offers the user-created 'static' events
		/// </summary>
		public ObservableCollection<EventObjectModel> StaticEvents
		{
			get { return listOfStaticEvents; }
			set { listOfStaticEvents = value; }
		} private ObservableCollection<EventObjectModel> listOfStaticEvents;
		
		#endregion

		#region constructors

		/// <summary>
		/// default constructor
		/// </summary>
		public EventObjectHandler()
		{
			ManifestVersion = string.Format("{0}.{1}.{2}",Version.Major,Version.Minor,Version.Build);
			
			responseQueue = new List<ResponseObjectModel>();
			listOfStaticEvents = new ObservableCollection<EventObjectModel>();
			
			tb = (TaskbarIcon) Application.Current.FindResource("TrayNotifyIcon");
		}

		#endregion
		
		#region methods

		/// <summary>
		/// Register to PipeServer enableing to receive messages from lsm
		/// </summary>
		public void Register()
		{
			Messenger.Default.Register<NotificationMessage<string>>(
				this, nm => {
					string[] cmdLineArgs = nm.Notification.Split(';');
					
					/* Processing the Message from LSM.Basic.Online
					 
					possible contents (copy of pipeserver):
					 
					int version = _ttoi(param[0]);
					int eventId = _ttoi(param[1]);
					CString date = param[2];
					CString wnAddr = param[3];
					CString serNumb = param[4];
					CString param1 = param[5];
					CString param2 = param[6];
					CString param3 = param[7];
					CString saName = param[8];
					CString lockName = param[9];
					CString transName = param[10];
	
					 */
					
					try
					{
						switch (nm.Content) {
							case "PipeServerMessage":
								switch (Enum.GetName(typeof(EventType), Convert.ToInt32(cmdLineArgs[1])))
								{
										
										
										
										
									case "UnlockingEvent":
										dynamicEventObject = new EventObjectModel(null,
										                                          Enum.GetName(typeof(EventType),
										                                                       Convert.ToInt32(cmdLineArgs[1])),
										                                          cmdLineArgs[8],
										                                          string.IsNullOrWhiteSpace(cmdLineArgs[10].Replace("/",string.Empty).Replace("\"",string.Empty)) ?
										                                          string.Format("G1 TID: {0}",cmdLineArgs[6]) :
										                                          cmdLineArgs[10].Remove(
										                                          	(cmdLineArgs[10].IndexOf('/') >= 0 ? cmdLineArgs[10].IndexOf('/') : 0),
										                                          	(cmdLineArgs[10].Length - (cmdLineArgs[10].IndexOf('/') >= 0 ? cmdLineArgs[10].IndexOf('/') : cmdLineArgs[10].Length))),
										                                          
										                                          cmdLineArgs[9].Remove(
										                                          	(cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : 0),
										                                          	(cmdLineArgs[9].Length - (cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : cmdLineArgs[9].Length))),
										                                          
										                                          Convert.ToInt32(cmdLineArgs[5]),
										                                          
										                                          string.IsNullOrWhiteSpace(cmdLineArgs[10].Replace("/",string.Empty).Replace("\"",string.Empty)) ?
										                                          string.Format("G1 TID: {0}",cmdLineArgs[6]) :
										                                          cmdLineArgs[10].Remove(0,cmdLineArgs[10].IndexOf('/') >= 0 ? cmdLineArgs[10].IndexOf('/') : 0),
										                                          cmdLineArgs[9].Remove(0,cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : 0),
										                                          
										                                          Enum.GetName(typeof(AccessType), Convert.ToInt32(cmdLineArgs[7])));
										

										
										EventComparer();
										break;
										
									case "InputEvent":
										
										string inputNum;
										string edgeType;
										
										if(cmdLineArgs[7] != "0")
										{
											inputNum = Enum.GetName(typeof(InputsByName),InputsByName.Input3);
											edgeType = Enum.GetName(typeof(InputEdgeTriggerType), Convert.ToInt32(cmdLineArgs[7]));
											
										}

										
										else if(cmdLineArgs[6] != "0")
										{
											inputNum = Enum.GetName(typeof(InputsByName),InputsByName.Input2);
											edgeType = Enum.GetName(typeof(InputEdgeTriggerType), Convert.ToInt32(cmdLineArgs[6]));
										}

										
										else
										{
											inputNum = Enum.GetName(typeof(InputsByName),InputsByName.Input1);
											edgeType = Enum.GetName(typeof(InputEdgeTriggerType), Convert.ToInt32(cmdLineArgs[5]));
										}

										
										
										dynamicEventObject = new EventObjectModel(null,
										                                          Enum.GetName(typeof(EventType), Convert.ToInt32(cmdLineArgs[1])),
										                                          cmdLineArgs[3],
										                                          inputNum,
										                                          edgeType);
										
										EventComparer();
										break;
										
									case "DoorHasBeenOpened":
									case "DoorStaysOpenTooLong":
									case "DoorHasBeenClosed":
									case "DoorHasBeenLocked":
									case "DoorHasBeenSecured":
									case "DoorHasBeenManipulated":
									case "DoorSensorError":
									case "DoorHasBeenClosedAfterTooLongOpened":
										
										dynamicEventObject = new EventObjectModel( null,
										                                          Enum.GetName(typeof(EventType), Convert.ToInt32(cmdLineArgs[1])),
										                                          cmdLineArgs[8],
										                                          cmdLineArgs[9].Remove(
										                                          	(cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : 0),
										                                          	(cmdLineArgs[9].Length - (cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : cmdLineArgs[9].Length))),
										                                          
										                                          Convert.ToInt32(cmdLineArgs[5]),
										                                          
										                                          cmdLineArgs[9].Remove(0,cmdLineArgs[9].IndexOf('/') >= 0 ? cmdLineArgs[9].IndexOf('/') : 0));
										
										EventComparer();
										
										break;

										
								}

								
								break;
								
							default:
								break;
						}
					}catch(Exception e)
					{
						LogWriter.CreateLogEntry(string.Format("Unable to Parse Message: {0}; {1}; {2}",DateTime.Now,e.Message, e.InnerException != null ? e.InnerException.Message : ""));
						return;
					}
					
				});
		}
		
		/// <summary>
		/// This method compares a 'dynamic' event object with all 'static' events in the user created list and fires the accociated responses.
		/// </summary>
		private void EventComparer()
		{
			try{
				
				using (var resMan = new ResourceLoader())
				{
					//perform check for each created 'static' event
					foreach(EventObjectModel staticEventObject in listOfStaticEvents)
					{
						#region check 'static' <=> 'dynamic' event filter
						if(staticEventObject.IsEnabled)
						{
							//TODO: consider using enums instead
							if(staticEventObject.EventType == "UnlockingEvent")
							{
								if(dynamicEventObject.EventType == staticEventObject.EventType)
								{
									//overwrite 'dynamic' event fields with fields from 'static' events if $dontcare flag is set
									var TransponderNameCopy = staticEventObject.TransponderModel.TransponderName == "$dont care" ? staticEventObject.TransponderModel.TransponderName : dynamicEventObject.TransponderModel.TransponderName;
									var TransponderPHICopy = staticEventObject.TransponderModel.TransponderPHI == "$dont care" ? staticEventObject.TransponderModel.TransponderPHI : dynamicEventObject.TransponderModel.TransponderPHI;
									
									foreach(LockModel lm in staticEventObject.TransponderModel.LockModels)
									{
										var LockNameCopy = lm.LockName == "$dont care" ? lm.LockName : dynamicEventObject.TransponderModel.LockModels[0].LockName;
										var LockPHICopy = lm.LockPHI =="$dont care" ? lm.LockPHI : dynamicEventObject.TransponderModel.LockModels[0].LockPHI;
										
										var tempDynamicEventObject = new EventObjectModel(null,
										                                                  "UnlockingEvent",
										                                                  null,
										                                                  TransponderNameCopy,
										                                                  LockNameCopy,
										                                                  0,
										                                                  TransponderPHICopy,
										                                                  LockPHICopy,
										                                                  
										                                                  
										                                                  staticEventObject.TransponderModel.SelectedTransponderTriggerType == "TriggerBoth" ?
										                                                  "TriggerBoth" :
										                                                  dynamicEventObject.TransponderModel.SelectedTransponderTriggerType);
										
										if(staticEventObject.Equals(tempDynamicEventObject))
										{
											staticEventObject.LastSeenDataGridRowContent = DateTime.Now.ToString();
											
											if(tb != null)
												tb.ShowBalloonTip(resMan.getResource("taskBarIconBalloonHeaderNewUnlockingEvent"), resMan.getResource("taskBarIconBalloonMessageNewUnlockingEvent"), BalloonIcon.Info);
											
											foreach(ResponseObjectModel rom in staticEventObject.ResponseCollection){
												responseQueue.Add(rom);
											}
										}
									}
								}
							}
							
							else if(staticEventObject.EventType == "InputEvent")
							{
								if(dynamicEventObject.EventType == staticEventObject.EventType)
								{
									if(staticEventObject.Equals(dynamicEventObject))
									{
										staticEventObject.LastSeenDataGridRowContent = DateTime.Now.ToString();
										
										if(tb != null)
											tb.ShowBalloonTip(resMan.getResource("taskBarIconBalloonHeaderNewInputEvent"), resMan.getResource("taskBarIconBalloonMessageNewInputEvent"), BalloonIcon.Info);
										
										foreach(ResponseObjectModel rom in staticEventObject.ResponseCollection){
											responseQueue.Add(rom);
										}
									}
								}

							}
							
							else if(staticEventObject.EventType == "DoorMonitoringEvent")
							{
								if(Enum.IsDefined(typeof(DoorMonEventType),dynamicEventObject.EventType))
								{
									foreach(LockModel lm in staticEventObject.TransponderModel.LockModels)
									{
										var LockNameCopy = lm.LockName == "$dont care" ? lm.LockName : dynamicEventObject.TransponderModel.LockModels[0].LockName;
										var LockPHICopy = lm.LockPHI =="$dont care" ? lm.LockPHI : dynamicEventObject.TransponderModel.LockModels[0].LockPHI;
										
										var tempDynamicEventObject = new EventObjectModel(null,dynamicEventObject.TransponderModel.LockModels[0].DoorMonitoringTrigger[0],null,LockNameCopy,0,LockPHICopy);
										
										if(staticEventObject.Equals(tempDynamicEventObject))
										{
											staticEventObject.LastSeenDataGridRowContent = DateTime.Now.ToString();
											
											if(tb != null)
												tb.ShowBalloonTip(resMan.getResource("taskBarIconBalloonHeaderNewDoorMonitoringEvent"), resMan.getResource("taskBarIconBalloonMessageNewDoorMonitoringEvent"), BalloonIcon.Info);
											
											foreach(ResponseObjectModel rom in staticEventObject.ResponseCollection){
												responseQueue.Add(rom);
											}
										}
									}
								}

							}
						}
						#endregion
						
						#region fire responses

						if(responseQueue.Count > 0)
						{
							bool canExecute = true;
							
							foreach(ResponseObjectModel response in responseQueue)
							{
								if(response.HasSchedule)
								{
									if(response.Scheduler != null && response.Scheduler.Schedule.Count > 0)
									{
										if(response.Scheduler.IsOnTime())
										{
											canExecute = true;
										}
										else
											canExecute = false;
									}
									
									else
										canExecute = false;
								}
								
								if(response.ResponseObject as TelegramMessageObjectModel != null && canExecute && response.IsEnabled && (response.ResponseObject as TelegramMessageObjectModel).Bot2Use.IsReceiving)
								{
									if(string.IsNullOrEmpty
									   ((response.ResponseObject as TelegramMessageObjectModel).MessageToSend))
									{
										switch(staticEventObject.EventType)
										{
											case "UnlockingEvent":
												(response.ResponseObject as TelegramMessageObjectModel)
													.SendTextMessageToChat(
														string.Format(resMan.getResource("defaultMessageTelegramMessageUnlockingEventMessage"),
														              dynamicEventObject.TransponderModel.TransponderName,
														              dynamicEventObject.TransponderModel.TransponderPHI,
														              dynamicEventObject.TransponderModel.LockModels[0].LockName,
														              dynamicEventObject.TransponderModel.LockModels[0].LockPHI,
														              DateTime.Now, resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.TransponderModel.SelectedTransponderTriggerType))),
														(response.ResponseObject as TelegramMessageObjectModel).Bot2Use);
												break;
												
											case "DoorMonitoringEvent":

												(response.ResponseObject as TelegramMessageObjectModel)
													.SendTextMessageToChat(
														string.Format(resMan.getResource("defaultMessageTelegramMessageDoorMonitoringMessage"),
														              dynamicEventObject.TransponderModel.LockModels[0].LockName,
														              dynamicEventObject.TransponderModel.LockModels[0].LockPHI,
														              DateTime.Now, resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.EventType))),
														(response.ResponseObject as TelegramMessageObjectModel).Bot2Use);
												break;
												
											case "InputEvent":

												(response.ResponseObject as TelegramMessageObjectModel)
													.SendTextMessageToChat(
														string.Format(resMan.getResource("defaultMessageTelegramMessageInputEventMessage"),
														              resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.InputModel.InputName)),
														              resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.InputModel.InputTriggerType)),
														              dynamicEventObject.InputModel.Address,
														              DateTime.Now, resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.EventType))),
														(response.ResponseObject as TelegramMessageObjectModel).Bot2Use);
												break;
										}
									}
									else
									{
										string message = (response.ResponseObject as TelegramMessageObjectModel).MessageToSend;
										
										if(message.IndexOf("$lockname",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$lockname",dynamicEventObject.TransponderModel.LockModels[0].LockName);
										if(message.IndexOf("$lockphi",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$lockphi",dynamicEventObject.TransponderModel.LockModels[0].LockPHI);
										if(message.IndexOf("$transpondername",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$transpondername",dynamicEventObject.TransponderModel.TransponderName);
										if(message.IndexOf("$transponderphi",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$transponderphi",dynamicEventObject.TransponderModel.TransponderPHI);
										if(message.IndexOf("$datetime",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$datetime",DateTime.Now.ToString());
									}
									
								}
								
								else if(response.ResponseObject as EMailResponseObjectModel != null && canExecute && response.IsEnabled)
								{
									
									if(string.IsNullOrEmpty((response.ResponseObject as EMailResponseObjectModel).MessageBody))
									{
										switch(staticEventObject.EventType)
										{
											case "UnlockingEvent":
												(response.ResponseObject as EMailResponseObjectModel).MessageBody = string.Format(
													resMan.getResource("defaultMessageTelegramMessageUnlockingEventMessage"),
													dynamicEventObject.TransponderModel.TransponderName,
													dynamicEventObject.TransponderModel.TransponderPHI,
													dynamicEventObject.TransponderModel.LockModels[0].LockName,
													dynamicEventObject.TransponderModel.LockModels[0].LockPHI,
													DateTime.Now, resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.TransponderModel.SelectedTransponderTriggerType)));
												break;
												
											case "DoorMonitoringEvent":

												(response.ResponseObject as EMailResponseObjectModel).MessageBody =	string.Format(
													resMan.getResource("defaultMessageTelegramMessageDoorMonitoringMessage"),
													dynamicEventObject.TransponderModel.LockModels[0].LockName,
													dynamicEventObject.TransponderModel.LockModels[0].LockPHI,
													DateTime.Now, resMan.getResource(string.Format("ENUM.{0}",dynamicEventObject.EventType)));
												break;
										}
										
										
										(response.ResponseObject as EMailResponseObjectModel).SendMessage();
										(response.ResponseObject as EMailResponseObjectModel).MessageBody = "";
									}
									else
									{
										string message = (response.ResponseObject as EMailResponseObjectModel).MessageBody;
										
										if(message.IndexOf("$lockname",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$lockname",dynamicEventObject.TransponderModel.LockModels[0].LockName);
										if(message.IndexOf("$lockphi",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$lockphi",dynamicEventObject.TransponderModel.LockModels[0].LockPHI);
										if(message.IndexOf("$transpondername",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$transpondername",dynamicEventObject.TransponderModel.TransponderName);
										if(message.IndexOf("$transponderphi",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$transponderphi",dynamicEventObject.TransponderModel.TransponderPHI);
										if(message.IndexOf("$datetime",StringComparison.CurrentCultureIgnoreCase) != -1)
											message = message.Replace("$datetime",DateTime.Now.ToString());
										
										(response.ResponseObject as EMailResponseObjectModel).MessageBody = message;
										
										(response.ResponseObject as EMailResponseObjectModel).SendMessage();
										
										(response.ResponseObject as EMailResponseObjectModel).MessageBody = null;
									}
								}
								
								else if(response.ResponseObject as KNXInterface != null && canExecute && response.IsEnabled)
								{
									
								}
								
								else if(response.ResponseObject as KMTronicModel != null && canExecute && response.IsEnabled)
								{
									(response.ResponseObject as KMTronicModel).ProcessRelays();
								}
							}
						}
						
						#endregion
						
						responseQueue.Clear();
					}
					responseQueue.Clear();
				}
				
				dynamicEventObject = null;
				
			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("Unable to Parse Message: {0}; {1}; {2}",DateTime.Now,e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				return;
			}
		}
		
		#endregion
	}
}
