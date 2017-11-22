using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace Trader.Calendar
{
    /// <summary>
    /// Experiments in design.
    /// 
    /// Simulating Unix 'Cron' Syntax (confession - never used unix cron)
    /// 
    /// Interesting use of Iteration
    /// Basically we modify the return of GetEnumerator to reflect whether we are enumerating years,hours etc
    ///              i.e. state dependent iteration return
    /// 
    /// Various Time functions return Cron
    ///          This allows chaining calls
    /// <code>new Cron().Hours(1,3).Hours(5,10)</code>
    /// 
    /// Various private Time functions return IEnumerable&lt;DateTime&gt;
    /// 
    /// GetEnumerator returns result of Time functions.GetEnumerator 
    ///                     eg return Hours().GetEnumerator
    /// 
    /// 
    /// One foreach call on Cron.Years(start,end).Month(start,end)....
    /// 
    /// 
    /// Results in chaining of the call through a series of Cron
    ///           Note that Cron implemments IEnumerable&lt;DateTime&gt; to terminate the chain
    /// 
    /// 
    /// Note that we can iterate in reverse at any level simply by having start > end
    /// 
    /// 
    /// All pretty much self contained.
    /// Less Flexible than Schedule but potentially more efficient and elegant syntax
    ///  Also has less tendency to error in over running time series because cannot set up bad Chaining
    ///      Less Flexible because only one Predicate can be included and is tacked on in the final Where call
    ///                 Can change design to allow multiple predicates -------DONE   so is now very flexible
    /// 
    /// Tag On A Where Predicate at the anywhere in The Chain Maintaining efficiency
    ///                Cron.Years(start,end).Where(predicate).Month(start,end)....Where(predicate)
    /// 
    /// Predicates can be preformed and reused inline
    /// <code>
    ///  Predicate<DateTime> predmodall= new Predicate&lt;DateTime&gt;(delegate(DateTime d) { return d.Day % 2 == 0; });
    /// foreach (DateTime date in Cron.Years(2007).Month(1).Day(1, 4).Where(predmodall).Day(6, 7).Where(predmodall).Day(8, 10).Where(predmodall))
    /// {
    ///         Blah.Helo
    /// 
    /// }
    /// </code>
    /// </summary>
    public class Cron
        : IEnumerable<DateTime>
    {
        #region static
        public static Cron Years(int year)
        {
            return new Cron().Year(year, year);
        }
        public static Cron Years(int start, int end)
        {
            return new Cron().Year(start, end);
        }
        #endregion
        private enum State
        {
            None, Years, Months, Days, Hours, Minutes, Seconds
        }
        #region Attributes
        private Cron _parent;
        private DateTime _current;
        private int _delta;
        private int _ending;
        private int _starting;
        private State _state;
        private Predicate<DateTime> _predicate;
        private Action<DateTime> _action;
        #endregion
        #region the real constructors / Chaining the call via parent rel's
        public Cron Year(int year)
        {
            return Year(year, year);
        }
        public Cron Year(int start, int end)
        {
            if (start < 1 || start > DateTime.MaxValue.Year)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > DateTime.MaxValue.Year)
            {
                throw new ArgumentException("end");
            }
            if (_state > State.Years)
                throw new InvalidOperationException("Invalid Call Chain Syntax - Years");
            Cron cron = Create(start, end, State.Years);
            cron._current = new DateTime(start, 1, 1);
            cron._parent = this._state == State.None ? null : this;
            return cron;
        }
        public Cron Month(int month)
        {
            return Month(month, month);
        }
        public Cron Month(int start, int end)
        {
            if (start < 1 || start > 12)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > 12)
            {
                throw new ArgumentException("end");
            }
            if (_state > State.Months)
                throw new InvalidOperationException("Invalid Call Chain Syntax - Months");
            return Create(start, end, State.Months);
        }
        public Cron Day(int day)
        {
            return Day(day, day);
        }
        public Cron Day(int start, int end)
        {
            if (start < 1 || start > 31)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > 31)
            {
                throw new ArgumentException("end");
            }
            int maxday = _current.AddMonths(1).AddDays(-1).Day;
            start = Math.Min(start, maxday);
            end = Math.Min(end, maxday);
            if (_state > State.Days)
                throw new InvalidOperationException("Invalid Call Chain Syntax - Days");
            return Create(start, end, State.Days);
        }
        public Cron Hour(int hour)
        {
            return Hour(hour, hour);
        }
        public Cron Hour(int start, int end)
        {
            if (start < 1 || start > 24)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > 24)
            {
                throw new ArgumentException("end");
            }
            if (_state > State.Hours)
                throw new InvalidOperationException("Invalid Call Chain Syntax - Hours");
            return Create(start, end, State.Hours);
        }
        public Cron Minute(int start, int end)
        {
            if (start < 1 || start > 59)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > 59)
            {
                throw new ArgumentException("end");
            }
            if (_state > State.Minutes)
                throw new InvalidOperationException("Invalid Call Chain Syntax - Minutes");
            return Create(start, end, State.Minutes);
        }

        public Cron Second(int start, int end)
        {
            if (start < 1 || start > 59)
            {
                throw new ArgumentException("start");
            }
            if (end < 1 || end > 59)
            {
                throw new ArgumentException("end");
            }
            return Create(start, end, State.Seconds);
        }
        private Cron Create(int start, int end, State state)
        {
            Cron cron = new Cron();
            cron._delta = Math.Sign(end - start);
            cron._ending = end;
            cron._starting = start;
            cron._parent = this;
            cron._state = state;
            return cron;
        }
        #endregion
        #region Private IEnumerable<DateTime>
        private bool PassOrFailAndAction()
        {
            bool pass = false;
            if (_predicate == null || _predicate(_current))
            {
                pass = true;
                if (_action != null)
                    _action(_current);
            }
            return pass;

        }
        private IEnumerable<DateTime> Years()
        {
            //really just a sibling
            if (_parent != null)
            {
                foreach (DateTime date in _parent.Years())
                {
                    yield return date;
                }
            }
            if (_delta == 0)
            {
                if (PassOrFailAndAction())
                {
                    yield return _current;
                }
            }
            else
            {
                // //zero the yr
                _current = _current.AddYears(-_current.Year + _starting);
                do
                {

                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                    if (_ending == _current.Year)
                        break;
                    _current = _current.AddYears(_delta);
                }
                while (true);
            }
        }
        private IEnumerable<DateTime> Months()
        {
            if (_parent._state == State.Months)
            {
                foreach (DateTime date in _parent)
                {
                    yield return date;
                }
            }
            Cron pyr = _parent;
            while (pyr._state == State.Months)
            {
                pyr = pyr._parent;
            }
            foreach (DateTime date in pyr.Years())
            {
                //zero the month
                _current = date.AddMonths(-date.Month + _starting);
                if (_delta == 0)
                {
                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                }
                else
                {
                    do
                    {
                        if (PassOrFailAndAction())
                        {
                            yield return _current;
                        }
                        if (_current.Month == _ending)
                            break;
                        _current = _current.AddMonths(_delta);
                    }
                    while (true);
                }
            }
        }
        private IEnumerable<DateTime> Days()
        {
            if (_parent._state == State.Days)
            {
                foreach (DateTime date in _parent)
                {
                    yield return date;
                }
            }
            Cron pyr = _parent;
            while (pyr._state == State.Days)
            {
                pyr = pyr._parent;
            }
            foreach (DateTime date in pyr.Months())
            {
                //zero the day
                DateTime ndate =  date.AddDays(_starting - date.Day);
                if (ndate.Month != date.Month)
                    break;
                _current = ndate;
                if (_delta == 0)
                {
                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                }
                else
                {
                    do
                    {
                        if (PassOrFailAndAction())
                        {
                            yield return _current;
                        }
                        if (_current.Day == _ending)
                            break;
                        DateTime nextdate = _current.AddDays(_delta);
                        if (nextdate.Month != _current.Month)
                            break;
                        _current = nextdate;
                    }
                    while (true);
                }
            }
        }
        private IEnumerable<DateTime> Hours()
        {
            if (_parent._state == State.Hours)
            {
                foreach (DateTime date in _parent)
                {
                    yield return date;
                }
            }
            Cron pyr = _parent;
            while (pyr._state == State.Hours)
            {
                pyr = pyr._parent;
            }
            foreach (DateTime date in pyr.Days())
            {
                //zero the hour
                _current = date.AddHours(-date.Hour + _starting);
                if (_delta == 0)
                {
                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                }
                else
                {
                    do
                    {
                        if (PassOrFailAndAction())
                        {
                            yield return _current;
                        }
                        if (_current.Hour == _ending)
                            break;
                        _current = _current.AddHours(_delta);
                    }
                    while (true);
                }
            }
        }
        private IEnumerable<DateTime> Minutes()
        {
            if (_parent._state == State.Minutes)
            {
                foreach (DateTime date in _parent)
                {
                    yield return date;
                }
            }
            Cron pyr = _parent;
            while (pyr._state == State.Minutes)
            {
                pyr = pyr._parent;
            }
            foreach (DateTime date in pyr.Hours())
            {
                //zero the minute
                _current = date.AddMinutes(-date.Minute + _starting);
                if (_delta == 0)
                {
                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                }
                else
                {
                    do
                    {
                        if (PassOrFailAndAction())
                        {
                            yield return _current;
                        }
                        if (_current.Minute == _ending)
                            break;
                        _current = _current.AddMinutes(_delta);
                    }
                    while (true);
                }
            }
        }
        private IEnumerable<DateTime> Seconds()
        {
            if (_parent._state == State.Seconds)
            {
                foreach (DateTime date in _parent)
                {
                    yield return date;
                }
            }
            Cron pyr = _parent;
            while (pyr._state == State.Seconds)
            {
                pyr = pyr._parent;
            }
            foreach (DateTime date in pyr.Minutes())
            {
                //zero the second
                _current = date.AddSeconds(-date.Second + _starting);
                if (_delta == 0)
                {
                    if (PassOrFailAndAction())
                    {
                        yield return _current;
                    }
                }
                else
                {
                    do
                    {
                        if (PassOrFailAndAction())
                        {
                            yield return _current;
                        }
                        if (_current.Second == _ending)
                            break;
                        _current = _current.AddSeconds(_delta);
                    }
                    while (true);
                }
            }
        }
        #endregion
        #region IEnumerable<DateTime> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// Return value based on state
        /// Instead of creating multiple classes implementing IEnumerable , Functions on this class return IEnumerable
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DateTime> GetEnumerator()
        {
            Debug.Assert(_state != State.None, "Inconsistent State");
            IEnumerator<DateTime> ie = null;
            switch (_state)
            {
                case State.Years:
                    ie = Years().GetEnumerator();
                    break;
                case State.Months:
                    ie = Months().GetEnumerator();
                    break;
                case State.Days:
                    ie = Days().GetEnumerator();
                    break;
                case State.Hours:
                    ie = Hours().GetEnumerator();
                    break;
                case State.Minutes:
                    ie = Minutes().GetEnumerator();
                    break;
            }

            return ie;
        }

        public Cron Where(Predicate<DateTime> predicate)
        {
            //Cannot chain predicates (delegates) which have a return function
            //       only last predicate in chain provides the bool test value
            // this._predicate += predicate; Does Not work
            if (_predicate == null)
            {
                _predicate = predicate;
            }
            else
            {
                //create a new combined predicate
                Predicate<DateTime> orig = _predicate;
                _predicate = null;
                _predicate = new Predicate<DateTime>(delegate(DateTime date) { return orig(date) && predicate(date); });

            }


            return this;
        }
        public Cron Action(Action<DateTime> action)
        {
            // combine delegates without hesitation
            // all actions will be called
            this._action +=  action;
            return this;

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
