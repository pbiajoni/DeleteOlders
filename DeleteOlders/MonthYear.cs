﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeleteOlders
{
    public class MonthYear
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public MonthYear()
        {

        }

        public MonthYear(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public override string ToString()
        {
            return this.Month + "/" + this.Year;
        }
    }
}
