﻿using System;

namespace Expl.Auxiliary {
   /// <summary>
   /// Calendar math functions.
   /// </summary>
   public static class Calendar {
      /// <summary>
      /// Number of calendar days in each month for non-leap year.
      /// </summary>
      private static readonly int[] monthDaysLookup = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

      /// <summary>
      /// Get number of days in a given month.
      /// </summary>
      /// <param name="year">Year.</param>
      /// <param name="month">Month 1-12.</param>
      /// <returns>Number of days</returns>
      public static int GetMonthDays(int year, int month) {
         if (month == 2) return IsLeapYear(year) ? 29 : 28;
         else return monthDaysLookup[month - 1];
      }

      /// <summary>
      /// Get whether a given year is a leap year.
      /// </summary>
      /// <param name="year">Year.</param>
      /// <returns>True if leap year</returns>
      public static bool IsLeapYear(int year) {
         return (year % 4 == 0) &&
            (
               (year % 100 != 0) || (year % 400 == 0)
            );
      }
   }
}
