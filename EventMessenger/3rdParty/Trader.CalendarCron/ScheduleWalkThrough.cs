using System;
using System.Collections.Generic;
using System.Text;

namespace Trader.Calendar
{
    public enum ScheduleWalkThrough
    {
        /// <summary>
        /// Will Step Through years
        /// </summary>
        Years,
        /// <summary>
        /// Can be chained on year iterator
        /// Will step through 12 months
        /// </summary>
        MonthsInYear,
        /// <summary>
        /// Can Be Chained on month iterator
        /// Will step through the number of days of the walked month.
        /// Be careful when start day is not the first of the month
        /// Days will then extend into the subsequent month
        /// </summary>
        DaysInMonth,
        /// <summary>
        /// Can be chained on Hour iterator
        /// Steps through 60 minutes
        /// </summary>
        MinutesInHour,
        /// <summary>
        /// Can be chained on Days or Week
        /// Steps through 24 Hours
        /// </summary>
        HoursInDay,
        /// <summary>
        /// Can be chained on Minutes
        /// steps through 60 seconds
        /// </summary>
        SecondsInMinute,
        /// <summary>
        /// Can Be Chained on month iterator
        /// Will step through the weeks of the walked month.
        /// Be careful when start day is not the first of the month
        /// Weeks will then extend into the subsequent month.
        /// </summary>
        WeeksInMonth,
        /// <summary>
        /// Can Be Chained on week iterator
        /// Will step through 7 days.
        /// Be careful when start day is not the first of the month
        /// Weeks will then extend into the subsequent month.
        /// </summary>
        DaysInWeek,
        /// <summary>
        /// Can Be Chained on year iterator
        /// Will step through weeks of year.
        /// Be careful when start day is not the first day of the year
        /// Weeks will then extend into the subsequent year.
        /// </summary>
        WeeksInYear,
        /// <summary>
        /// Can Be Chained on year iterator
        /// Will step through days of year.
        /// Be careful when start day is not the first day of the year
        /// Days will then extend into the subsequent year.
        /// </summary>
        DaysInYear
    }
}
