﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Expl.Auxiliary {
   /// <summary>
   /// Wraps IEnumerator to provide single element lookahead.
   /// </summary>
   /// <typeparam name="T">Enumerated type.</typeparam>
   public class SpyEnumerator<T> : IEnumerator<T> {
      IEnumerator<T> _Iterator;
      T _Current;
      T _Peek;
      bool _HasData = false;
      bool _HasMore = false;

      public SpyEnumerator(IEnumerator<T> Iterator) {
         _Iterator = Iterator;
         if (_HasMore = _Iterator.MoveNext()) _Peek = _Iterator.Current;
      }

      /// <summary>
      /// Get whether enumeration has more data after current position.
      /// </summary>
      public bool HasMore { get { return _HasMore; } }

      /// <summary>
      /// Peek at next element after current position.
      /// This property does not change current position.
      /// </summary>
      public T Peek {
         get {
            if (_HasMore) return _Peek;
            else throw new InvalidOperationException();
         }
      }

      #region IEnumerable<T> Members

      public T Current {
         get {
            if (_HasData) return _Current;
            else throw new InvalidOperationException();
         }
      }

      public bool MoveNext() {
         if (_HasMore) {
            _Current = _Peek;
            _HasData = true;
            if (_HasMore = _Iterator.MoveNext()) _Peek = _Iterator.Current;
         }
         else {
            _HasData = false;
         }
         return _HasData;
      }

      public void Reset() {
         _Iterator.Reset();
         _HasMore = _Iterator.MoveNext();
         if (_HasMore) _Peek = _Iterator.Current;
      }

      #endregion

      #region IEnumerable Members

      object IEnumerator.Current { get { return _Current; } }

      #endregion

      #region IDisposable Members

      public void Dispose() {
         _Iterator = null;
      }

      #endregion
   }

   /// <summary>
   /// Useful extensions to SpyEnumerator.
   /// </summary>
   public static class SpyEnumerableExtensions {
      /// <summary>
      /// Wrap GetEnumerator() with SpyEnumerator.
      /// </summary>
      /// <typeparam name="T">Enumerated type.</typeparam>
      /// <param name="Iterator">IEnumerable object.</param>
      /// <returns>SpyEnumerator object.</returns>
      public static SpyEnumerator<T> GetSpyEnumerator<T>(this IEnumerable<T> enumerable) {
         return new SpyEnumerator<T>(enumerable.GetEnumerator());
      }
   }
}
