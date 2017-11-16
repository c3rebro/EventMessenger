/*
 * Created by SharpDevelop.
 * Date: 22.02.2017
 * Time: 23:38
 * 
 */
using System;

namespace EventMessenger
{
	/// <summary>
	/// 
	/// </summary>
	public enum KMTronicRelayContactBehavior
	{
		RelayContactIsTimed = 0,
		RelayContactIsStaticOn = 1,
		RelayContactIsStaticOff = 2,
		RelayContactIsToggle = 3
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum KMTronicDeviceType
	{
		W2CR = 0,
		W8CR = 1
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum TelegramUserStatus
	{
		UserIsAdmin,
		UserIsRegistered,
		UserIsNotRegistered,
		UserIsBanned
	}
	/// <summary>
	/// 
	/// </summary>
	public struct TelegramUser
	{
		public TelegramUser(TelegramUserStatus _status, long _chatID)
		{
			UserStatus = _status;
			chatID = _chatID;
		}
		
		public TelegramUserStatus UserStatus;
		public long chatID;
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum ScheduleRepeatType
	{
		ScheduleRepeatNone = 0,
		ScheduleRepeatDaily = 1,
		ScheduleRepeatWeekly = 2,
		ScheduleRepeatEverySecondWeek = 3,
		ScheduleRepeatEveryThirdWeek = 4,
		ScheduleRepeatMonthly = 5
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum AccessType
	{
		AccessGranted = 0,
		AccessDenied = 1,
		TriggerBoth = 2
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum GeneralResponseTypes
	{
		DeviceKNX = 1,
		DeviceKMTronic = 2,
		MessageEMAIL = 3,
		MessageTELEGRAM = 4
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum GeneralEventTypes
	{
		InputEvent = 1,
		DoorMonitoringEvent = 2,
		UnlockingEvent = 3
	}

	/// <summary>
	/// 
	/// </summary>
	public enum InputsByName
	{
		Input1 = 1,
		Input2 = 2,
		Input3 = 3
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum InputEdgeTriggerType
	{
		RisingEdge = 1,
		FallingEdge = 2,
		BothEdges = 3
	}
	
	/// <summary>
	/// 
	/// </summary>
	public enum DoorMonEventType
	{
		DoorHasBeenOpened = 2,
		DoorStaysOpenTooLong = 3,
		DoorHasBeenClosed = 4,
		DoorHasBeenLocked = 5,
		DoorHasBeenSecured = 6,
		DoorHasBeenManipulated = 7,
		DoorSensorError = 8,
		DoorHasBeenClosedAfterTooLongOpened = 10
	}
	
	/// <summary>
	/// Lists all possible Events that may occur
	/// </summary>
	public enum EventType
	{
		/// <summary> An Input Change Event: LockNode or Router</summary>
		InputEvent = 1,
		/// <summary> Door Monitoring Event: Door has been Opened</summary>
		DoorHasBeenOpened = 2,
		/// <summary> Door Monitoring Event: Door stays open too Long</summary>
		DoorStaysOpenTooLong = 3,
		/// <summary> Door Monitoring Event: Door has been Closed</summary>
		DoorHasBeenClosed = 4,
		/// <summary> Door Monitoring Event: Door has been Locked</summary>
		DoorHasBeenLocked = 5,
		/// <summary> Door Monitoring Event: Door has been Secured</summary>
		DoorHasBeenSecured = 6,
		/// <summary> Door Monitoring Event: Manipulating Alarm</summary>
		DoorHasBeenManipulated = 7,
		/// <summary> Door Monitoring Event: Sensor Error</summary>
		DoorSensorError = 8,
		/// <summary> Unlocking Event: Transponder has been used</summary>
		UnlockingEvent = 9,
		/// <summary> Door Monitoring Event: Door has been closed after too long open</summary>
		DoorHasBeenClosedAfterTooLongOpened = 10
	}
}
