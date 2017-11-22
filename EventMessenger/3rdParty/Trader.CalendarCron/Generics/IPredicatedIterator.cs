using System;
using System.Collections.Generic;
namespace Trader.Generics
{
    /// <summary>
    /// Base Interface for the Predicated Iterator Pattern.
    /// The only Reason I have extracted this interface is to 
    /// demonstrate the requirements for the pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPredicatedIterator<T> 
        : IEnumerable<T>
    {
        T CurrentStart
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <param name="count">The max count.</param>
        /// <returns></returns>
        IEnumerable<T> GetEnumerator(int count);
        /// <summary>
        /// Gets the enumerator over the enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        IEnumerable<T> GetEnumerator(IEnumerable<T> enumerable);
        /// <summary>
        /// Gets the enumerator over the enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="count">The max count.</param>
        /// <returns></returns>
        IEnumerable<T> GetEnumerator(IEnumerable<T> enumerable, int count);
        Predicate<T> Predicate { get; set; }
    }
}
