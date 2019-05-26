using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeleteOlders
{
    public static class Extensions
    {
        public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }
    }
}
