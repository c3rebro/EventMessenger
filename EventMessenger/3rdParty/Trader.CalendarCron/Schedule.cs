using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Trader.Generics;
namespace Trader.Calendar
{
    /// <summary>
    /// Trader.Calendar Project Outline
    /// May 11th 2007
    /// 
    /// Dev Lead Anton
    /// Unit tester Derek
    /// 
    /// Development Method as always Agile Incremental Unit test Oriented 
    ///                                   (Not 'Unit test first' - too inflexible)
    /// 
    /// 
    /// Aim: Use Flexible Predicated Iterator pattern to iterate date series
    /// Why: Linq Not in use and not flexible enough, nor is easy to re-use
    ///      Linq Not easily optimisable unless complicated nesting (too difficult)
    ///      Technological Challenge
    ///      Test Flexibility of Overloading GetEnumerator 
    ///      Develop reuseable pattern ? Split out Interface def's
    ///      Incorporate Holidays into Trading schedule, Exchange Time Clocks
    ///      Attach Notifiable events to Prices on charts
    ///      Investigate Database sourcing/caching for other derivative pattern
    ///      Extend Predicate Methods Onto Classes to maximise reuse
    /// 
    ///      
    /// 
    ///    Function:                                                                             Status
    /// 1. Forward and reverse iteration of dates [CurrentStart to EndDate]                    - Complete
    /// 2. Iterate by year,month,week,day,hour,minute and second.
    ///                          (Simple optimisations over date only)                         - Complete
    /// 3. Filtering of output === Predicated iteration operations                             - Complete
    /// 4. Limit output to a 'Count' specified in Overide of GetEnumerator                     - Complete
    /// 5. Chaining of iteration via calling chain syntax (obviates need for nested loops)     - Complete
    /// 6. Ability to retrieve CurrentStart during chaining enueration call                    - Complete
    /// 7. Reverse iteration of dates via overloads on GetEnumerator 
    ///                            Another Pattern / Split off into seperate interface         - Complete
    /// 8. Develop predicate 'Library'                                                         - Not feasable/worthwhile
    /// 9. Test Under Orcas                                                                    - Done
    /// 10. Add Linq, Net 3.0 optimisation / extensions / or convert to 3.0                    - evaluating
    /// 11. Investigate other uses for pattern                                                 - 
    /// 
    /// 
    /// Test Status
    /// 1. Basic                                                                                  - Passed/Complete
    /// 2. Chaining                                                                               - Passed/Complete
    /// 3. BAD Parameters Tests to check for need for Exception Handling                          - Passed/Complete (zero output by design)
    /// 4. Bad Chaining                                                                           - Output Error (subtle logic error when bad call
    ///                                                                                              chain : No way to detect) 
    /// 
    /// Pattern Extensions
    /// Schedule Points - see SchedulePoints Class                                             -Unit Testing
    /// Holiday Schedule - see HolidaySchedule Class                                           -First Attempt
    /// 
    /// 
    /// ** Uploaded a version to CP
    /// http://www.codeproject.com/useritems/Flexible_Time_Schedule.asp
    /// 
    /// ** Check Cron - original inspiration
    /// http://www.codeproject.com/useritems/IdaligoTime.asp
    /// 
    /// 
    /// </summary>
    public class Schedule
        : IPredicatedReversibleIterator<DateTime>
    {

        #region Attributes
        DateTime _currentStart;
        DateTime _endDate;
        ScheduleWalkThrough _walk;
        Predicate<DateTime> _predicate;
        #endregion
        #region CTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// Start and EndDate are irrelevent when chaining.
        /// </summary>
        /// <param name="walk">The walk.</param>
        public Schedule(ScheduleWalkThrough walk)
        {
            _walk = walk;
            _currentStart = _endDate = DateTime.MinValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="step">The walk.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        public Schedule(ScheduleStep step, DateTime startDate, DateTime endDate)
        {
            _currentStart = startDate;
            _endDate = endDate;
            // ScheduleStep is converted to ScheduleWalkTo 
            // Done just to maintain clarity when setting up constructor
            // otherwise caller will have confusion when using this constructor regarding end dates
            // which are ignored 
            switch (step)
            {
                case ScheduleStep.Years:
                    _walk = ScheduleWalkThrough.Years;
                    break;
                case ScheduleStep.Months:
                    _walk = ScheduleWalkThrough.MonthsInYear;
                    break;
                case ScheduleStep.Weeks:
                    _walk = ScheduleWalkThrough.WeeksInYear;
                    break;
                case ScheduleStep.Days:
                    _walk = ScheduleWalkThrough.DaysInYear;
                    break;
                case ScheduleStep.Hours:
                    _walk = ScheduleWalkThrough.HoursInDay;
                    break;
                case ScheduleStep.Minutes:
                    _walk = ScheduleWalkThrough.MinutesInHour;
                    break;
                case ScheduleStep.Seconds:
                    _walk = ScheduleWalkThrough.SecondsInMinute;
                    break;
            }
        }
        #endregion
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public ScheduleWalkThrough Walk
        {
            get { return _walk; }
            set { _walk = value; }
        }
        /// <summary>
        /// Gets or sets the predicate which acts as a filter on walked DateTime's.
        /// Can be null;
        /// </summary>
        /// <value>The predicate.</value>
        public Predicate<DateTime> Predicate
        {
            get { return _predicate; }
            set { _predicate = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CurrentStart
        {
            get { return _currentStart; }
            set { _currentStart = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        #endregion
        #region IEnumerable<DateTime> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// this allows 
        /// foreach (DateTime in schedule)
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime date = _currentStart;
            while (date <= _endDate)
            {
                if (_predicate != null)
                {
                    if (!_predicate(date))
                    {
                        date = ApplyStep(date, 1);
                        continue;
                    }
                }

                yield return date;
                date = ApplyStep(date, 1);
            }
        }
        /// <summary>
        /// Gets the enumerator reverse.
        /// Allows
        /// foreach (DateTime in schedule.GetEnumeratorReverse())
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumeratorReverse()
        {
            DateTime date = _endDate;
            while (date >= _currentStart)
            {
                if (_predicate != null)
                {
                    if (!_predicate(date))
                    {
                        date = ApplyStep(date, -1);
                        continue;
                    }
                }

                yield return date;
                date = ApplyStep(date, -1);
            }
        }
        /// <summary>
        /// Gets the enumerator.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumerator(1))
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumerator(int count)
        {
            DateTime date = _currentStart;
            int tc = count;
            while (date <= _endDate && tc > 0)
            {
                if (_predicate != null)
                {
                    if (!_predicate(date))
                    {
                        date = ApplyStep(date, 1);
                        continue;
                    }
                }
                tc--;
                yield return date;
                date = ApplyStep(date, 1);
            }
        }
        /// <summary>
        /// Gets the enumerator reverse.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumeratorReverse(1))
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumeratorReverse(int count)
        {
            DateTime date = _endDate;
            int tc = count;
            while (date >= _currentStart && tc > 0)
            {
                if (_predicate != null)
                {
                    if (!_predicate(date))
                    {
                        date = ApplyStep(date, -1);
                        continue;
                    }
                }
                tc--;
                yield return date;
                date = ApplyStep(date, -1);
            }
        }
        /// <summary>
        /// Gets the enumerator.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumerator(schedule ))
        /// foreach (DateTime in schedule.GetEnumerator(schedule.GetEnumeratorReverse() ))
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumerator(IEnumerable<DateTime> enumerable)
        {
            foreach (DateTime date in enumerable)
            {
                _currentStart = date;
                _endDate = GetSubScheduleEndDate(date);
                foreach (DateTime subdate in this)
                {
                    yield return subdate;
                }
            }
        }
        /// <summary>
        /// Gets the enumerator reverse.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumeratorReverse(schedule ))
        /// foreach (DateTime in schedule.GetEnumeratorReverse(schedule.GetEnumeratorReverse() ))
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumeratorReverse(IEnumerable<DateTime> enumerable)
        {
            foreach (DateTime date in enumerable)
            {
                _currentStart = date;
                _endDate = GetSubScheduleEndDate(date);
                foreach (DateTime subdate in this.GetEnumeratorReverse())
                {
                    yield return subdate;
                }
            }
        }
        /// <summary>
        /// Gets the enumerator.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumerator(schedule,count ))
        /// foreach (DateTime in schedule.GetEnumerator(schedule.GetEnumeratorReverse(),count ))
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumerator(IEnumerable<DateTime> enumerable, int count)
        {
            foreach (DateTime date in enumerable)
            {
                _currentStart = date;
                _endDate = GetSubScheduleEndDate(date);
                foreach (DateTime subdate in this.GetEnumerator(count))
                {
                    yield return subdate;
                }
            }
        }
        /// <summary>
        /// Gets the enumerator reverse.
        /// Allows syntax
        /// foreach (DateTime in schedule.GetEnumeratorReverse(schedule,count ))
        /// foreach (DateTime in schedule.GetEnumeratorReverse(schedule.GetEnumeratorReverse(),count ))
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetEnumeratorReverse(IEnumerable<DateTime> enumerable, int count)
        {
            foreach (DateTime date in enumerable)
            {
                _currentStart = date;
                _endDate = GetSubScheduleEndDate(date);
                foreach (DateTime subdate in this.GetEnumeratorReverse(count))
                {
                    yield return subdate;
                }
            }
        }
        /// <summary>
        /// Applies the step.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="delta">The delta.(1 or -1)</param>
        /// <returns></returns>
        private DateTime ApplyStep(DateTime date, int delta)
        {
            DateTime ndate = date;
            switch (_walk)
            {
                case ScheduleWalkThrough.Years:
                    ndate = ndate.AddYears(delta);
                    break;
                case ScheduleWalkThrough.MonthsInYear:
                    ndate = ndate.AddMonths(delta);
                    break;
                case ScheduleWalkThrough.DaysInMonth:
                case ScheduleWalkThrough.DaysInWeek:
                case ScheduleWalkThrough.DaysInYear:
                    ndate = ndate.AddDays(delta);
                    break;
                case ScheduleWalkThrough.MinutesInHour:
                    ndate = ndate.AddMinutes(delta);
                    break;
                case ScheduleWalkThrough.HoursInDay:
                    ndate = ndate.AddHours(delta);
                    break;
                case ScheduleWalkThrough.SecondsInMinute:
                    ndate = ndate.AddSeconds(delta);
                    break;
                case ScheduleWalkThrough.WeeksInMonth:
                    ndate = ndate.AddDays(7 * delta);
                    break;
                case ScheduleWalkThrough.WeeksInYear:
                    ndate = ndate.AddDays(7 * delta);
                    break;
            }
            return ndate;
        }
        /// <summary>
        /// Gets the sub schedule end date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private DateTime GetSubScheduleEndDate(DateTime date)
        {
            DateTime ndate = date;
            switch (_walk)
            {
                case ScheduleWalkThrough.Years:
                    Debug.Assert(false, "Should not get here");
                    break;
                case ScheduleWalkThrough.MonthsInYear:
                    ndate = ndate.AddYears(1).AddMonths(-1);
                    break;
                case ScheduleWalkThrough.WeeksInYear:
                    ndate = ndate.AddYears(1).AddDays(-7);
                    break;
                case ScheduleWalkThrough.DaysInMonth:
                    ndate = ndate.AddMonths(1).AddDays(-1);
                    break;
                case ScheduleWalkThrough.WeeksInMonth:
                    ndate = ndate.AddMonths(1).AddDays(-1);
                    break;
                case ScheduleWalkThrough.MinutesInHour:
                    ndate = ndate.AddMinutes(59);
                    break;
                case ScheduleWalkThrough.HoursInDay:
                    ndate = ndate.AddHours(23);
                    break;
                case ScheduleWalkThrough.SecondsInMinute:
                    ndate = ndate.AddSeconds(59);
                    break;
                case ScheduleWalkThrough.DaysInWeek:
                    ndate = ndate.AddDays(6);
                    break;
                case ScheduleWalkThrough.DaysInYear:
                    ndate = ndate.AddYears(1).AddDays(-1);
                    break;
            }
            return ndate;
        }
        #endregion
        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        /// Determines whether is week day.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is week day] ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWeekDay(DateTime date)
        {
            return ((date.DayOfWeek != DayOfWeek.Saturday) && (date.DayOfWeek != DayOfWeek.Sunday));
        }
        public static DateTime OnOrNextDayOfWeek(DateTime date, DayOfWeek day)
        {
            while (date.DayOfWeek != day)
                date = date.AddDays(1);
            return date;
        }
        /// <summary>
        /// Returns unchanged if is business day
        /// else subsequent Monday
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><see cref="T:System.DateTime"/></returns>
        public static DateTime OnOrNextBusinessDay(DateTime date)
        {
            DateTime d = date;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    d = date.AddDays(1);
                    break;
                case System.DayOfWeek.Saturday:
                    d = date.AddDays(2);
                    break;
            }
            return d;
        }
        /// <summary>
        /// Returns unchanged if is business day
        /// else previous  Friday
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static DateTime OnOrPreviousBusinessDay(DateTime date)
        {
            DateTime d = date;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    d = date.AddDays(-2);
                    break;
                case System.DayOfWeek.Saturday:
                    d = date.AddDays(-1);
                    break;
            }
            return d;
        }
        /// <summary>
        /// Next Business Day (not Sat or Sun)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><see cref="T:System.DateTime"/></returns>
        public static DateTime NextBusinessDay(DateTime date)
        {
            DateTime d = date;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                case System.DayOfWeek.Monday:
                case System.DayOfWeek.Tuesday:
                case System.DayOfWeek.Wednesday:
                case System.DayOfWeek.Thursday:
                    d = date.AddDays(1);
                    break;
                case System.DayOfWeek.Friday:
                    d = date.AddDays(3);
                    break;
                case System.DayOfWeek.Saturday:
                    d = date.AddDays(2);
                    break;
            }
            return d;
        }
        /// <summary>
        /// Previous Trade Day (not Sat or Sun)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><see cref="T:System.DateTime"/></returns>
        public static DateTime PreviousBusinessDay(DateTime date)
        {
            DateTime d = date;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    d = date.AddDays(-2);
                    break;
                case System.DayOfWeek.Monday:
                    d = date.AddDays(-3);
                    break;
                case System.DayOfWeek.Tuesday:
                case System.DayOfWeek.Wednesday:
                case System.DayOfWeek.Thursday:
                case System.DayOfWeek.Friday:
                case System.DayOfWeek.Saturday:
                    d = date.AddDays(-1);
                    break;
            }
            return d;
        }
    }
}
