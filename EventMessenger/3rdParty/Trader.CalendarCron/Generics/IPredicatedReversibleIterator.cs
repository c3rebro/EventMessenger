using System;
using System.Collections.Generic;

namespace Trader.Generics
{
    /// <summary>
    /// The Reversible Extension for the for the Predicated Iterator Pattern
    /// The only Reason I have extracted this interface is to 
    /// demonstrate the requirements for the pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPredicatedReversibleIterator<T>
        : IPredicatedIterator<T>
    {

        IEnumerable<T> GetEnumeratorReverse(IEnumerable<T> enumerable, int count);
        IEnumerable<T> GetEnumeratorReverse(IEnumerable<T> enumerable);
        IEnumerable<T> GetEnumeratorReverse(int count);
        IEnumerable<T> GetEnumeratorReverse();

    }
}
