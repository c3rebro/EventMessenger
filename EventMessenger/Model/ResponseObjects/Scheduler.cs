/*
 * Created by SharpDevelop.
 * Date: 08/31/2017
 * Time: 22:45
 * 
 */
using Expl.Itinerary;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of Period.
	/// </summary>
	public class Scheduler
	{
		
		/// <summary>
		/// 
		/// </summary>
		public Scheduler()
		{
			Schedule = new ObservableCollection<Period>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<Period> Schedule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Exist(Period periodToCompare)
		{
			if(Schedule != null && Schedule.Count > 0)
			{
				foreach(Period period in Schedule)
				{
					switch(periodToCompare.RepeatType)
					{
						case ScheduleRepeatType.ScheduleRepeatDaily:

							ISchedule schedCurPer = new IntervalSchedule(new TimeSpan(1,0,0,0), period.End - period.Begin, period.Begin);
							ISchedule schedPerToComp = new IntervalSchedule(new TimeSpan(1,0,0,0), periodToCompare.End - periodToCompare.Begin, periodToCompare.Begin);
							
							ISchedule overlap = new BoolNonIntersectionSchedule(schedCurPer, schedPerToComp);
							
							
							IEnumerable<TimedEvent> eventsCurPer = schedCurPer.GetRange(period.Begin, period.End);
							IEnumerable<TimedEvent> eventsPerToComp = overlap.GetRange(periodToCompare.Begin, periodToCompare.End);
							
							if(eventsCurPer.Any())
								return true;

							break;
							

						case ScheduleRepeatType.ScheduleRepeatWeekly:
							
							ISchedule sched1 = new IntervalSchedule(new TimeSpan(7,0,0,0), period.End - period.Begin, period.Begin);
							

							IEnumerable<TimedEvent> events1 = sched1.GetRange(DateTime.Now.Add(new TimeSpan(-7,0,0,0)), DateTime.Now.Add(new TimeSpan(7,0,0,0)));
							
							foreach(TimedEvent e in events1)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							

						case ScheduleRepeatType.ScheduleRepeatEverySecondWeek:
							
							ISchedule sched2 = new IntervalSchedule(new TimeSpan(14,0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events2 = sched2.GetRange(DateTime.Now.Add(new TimeSpan(-14,0,0,0)), DateTime.Now.Add(new TimeSpan(14,0,0,0)));
							
							foreach(TimedEvent e in events2)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						case ScheduleRepeatType.ScheduleRepeatEveryThirdWeek:
							
							ISchedule sched3 = new IntervalSchedule(new TimeSpan(21,0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events3 = sched3.GetRange(DateTime.Now.Add(new TimeSpan(-21,0,0,0)), DateTime.Now.Add(new TimeSpan(21,0,0,0)));
							
							foreach(TimedEvent e in events3)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						case ScheduleRepeatType.ScheduleRepeatMonthly:
							
							ISchedule sched4 = new IntervalSchedule(new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events4 = sched4.GetRange(DateTime.Now.Add(new TimeSpan(-31,0,0,0)), DateTime.Now.Add(new TimeSpan(31,0,0,0)));
							
							foreach(TimedEvent e in events4)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						default:
							if(DateTime.Now >= period.Begin && DateTime.Now <= period.End)
								return true;
							break;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsOnTime(Period periodToCompare = null, bool compareWithCurrentDateTime = true)
		{
			if(Schedule != null && Schedule.Count > 0)
			{
				foreach(Period period in Schedule)
				{
					switch(period.RepeatType)
					{
						case ScheduleRepeatType.ScheduleRepeatDaily:
							
							ISchedule sched0 = new IntervalSchedule(new TimeSpan(1,0,0,0), period.End - period.Begin, period.Begin);
							IEnumerable<TimedEvent> events0 = sched0.GetRange(DateTime.Now.Add(new TimeSpan(-1,0,0,0)), DateTime.Now.Add(new TimeSpan(1,0,0,0)));
							
							foreach(TimedEvent e in events0)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						case ScheduleRepeatType.ScheduleRepeatWeekly:
							
							ISchedule sched1 = new IntervalSchedule(new TimeSpan(7,0,0,0), period.End - period.Begin, period.Begin);
							

							IEnumerable<TimedEvent> events1 = sched1.GetRange(DateTime.Now.Add(new TimeSpan(-7,0,0,0)), DateTime.Now.Add(new TimeSpan(7,0,0,0)));
							
							foreach(TimedEvent e in events1)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
							
						case ScheduleRepeatType.ScheduleRepeatEverySecondWeek:
							
							ISchedule sched2 = new IntervalSchedule(new TimeSpan(14,0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events2 = sched2.GetRange(DateTime.Now.Add(new TimeSpan(-14,0,0,0)), DateTime.Now.Add(new TimeSpan(14,0,0,0)));
							
							foreach(TimedEvent e in events2)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						case ScheduleRepeatType.ScheduleRepeatEveryThirdWeek:
							
							ISchedule sched3 = new IntervalSchedule(new TimeSpan(21,0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events3 = sched3.GetRange(DateTime.Now.Add(new TimeSpan(-21,0,0,0)), DateTime.Now.Add(new TimeSpan(21,0,0,0)));
							
							foreach(TimedEvent e in events3)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						case ScheduleRepeatType.ScheduleRepeatMonthly:
							
							ISchedule sched4 = new IntervalSchedule(new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),0,0,0), period.End - period.Begin, period.Begin);
							
							IEnumerable<TimedEvent> events4 = sched4.GetRange(DateTime.Now.Add(new TimeSpan(-31,0,0,0)), DateTime.Now.Add(new TimeSpan(31,0,0,0)));
							
							foreach(TimedEvent e in events4)
							{
								if(DateTime.Now >= e.StartTime && DateTime.Now <= e.EndTime)
									return true;
							}

							break;
							
						default:
							if(DateTime.Now >= period.Begin && DateTime.Now <= period.End)
								return true;
							break;
					}
				}
			}
			
			return false;
		}
	}
}
