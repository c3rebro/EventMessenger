using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace Trader.Calendar
{
    public class SchedulePoints
        : IEnumerable<DateTime>
    {
        TimeSpan[] _spans;
        DateTime _startdate;
        public DateTime StartDate
        {
            get { return _startdate; }
            set { _startdate = value; }
        }
        public SchedulePoints(params TimeSpan[] spans)
        {
            _spans = spans;
        }
        public TimeSpan[] Spans
        {
            get { return _spans; }
            set { _spans = value; }
        }
        #region ISchedule Members
        public IEnumerator<DateTime> GetEnumerator()
        {
            foreach (TimeSpan span in _spans)
            {
                DateTime spandate = _startdate.Add(span);
                yield return spandate;
            }
        }
        public IEnumerable<DateTime> Enumerate(IEnumerable<DateTime> enumerable)
        {

            if (enumerable != null)
            {
                foreach (DateTime date in enumerable)
                {
                    foreach (TimeSpan span in _spans)
                    {
                        DateTime spandate = date.Add(span);
                        yield return spandate;
                    }
                }
            }
        }
        #endregion
        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
