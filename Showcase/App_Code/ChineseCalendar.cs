using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Showcase.Util
{
    /// <summary>
    /// Summary description for CCalendar
    /// </summary>
    public class ChineseCalendar
    {
        public static ChineseLunisolarCalendar CHINESE_LS_CALENDAR = new ChineseLunisolarCalendar();
        public const int BEJING_TIME_ZONE = 8; //the different between Beijing time and UTC

        private static string[] tianGan = new string[10] {
            "\x7532", "\x4E59", "\x4E19", "\x4E01", "\x620A", "\x5DF1", "\x5E9A", "\x8F9B", "\x58EC", "\x7678"
        };

        private static string[] diZhi = new string[12] {
            "\x5b50", "\x4e11", "\x5bc5", "\x536f", "\x8fb0", "\x5df3", "\x5348", "\x672a", "\x7533", "\x9149", "\x620c", "\x4ea5"
        };

        private static string[] animals = new string[12] {
            "\x9F20", "\x725B", "\x864E", "\x5154", "\x9F99", "\x86C7", "\x9A6C", "\x7F8A", "\x7334", "\x9E21", "\x72D7", "\x732A"
        };

        private static string[] CHINESE_DAYS = new string[30] {
            "\x521D\x4E00", "\x521D\x4E8C", "\x521D\x4E09", "\x521D\x56DB", "\x521D\x4E94", "\x521D\x516D", "\x521D\x4E03", "\x521D\x516B", "\x521D\x4E5D", "\x521D\x5341",
            "\x5341\x4E00", "\x5341\x4E8C", "\x5341\x4E09", "\x5341\x56DB", "\x5341\x4E94", "\x5341\x516D", "\x5341\x4E03", "\x5341\x516B", "\x5341\x4E5D", "\x4E8C\x5341",
            "\x5EFF\x4E00", "\x5EFF\x4E8C", "\x5EFF\x4E09", "\x5EFF\x56DB", "\x5EFF\x4E94", "\x5EFF\x516D", "\x5EFF\x4E03", "\x5EFF\x516B", "\x5EFF\x4E5D", "\x4E09\x5341"
        };


        public ChineseCalendar()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// return Beijing time at the beginning of a day
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        private static DateTime GetBeijingTime(DateTime utcTime)
        {
            DateTime beijingTime = utcTime.AddHours(BEJING_TIME_ZONE);

            return new DateTime(beijingTime.Year, beijingTime.Month, beijingTime.Day);
        }

        /// <summary>
        /// Calculate Chinese New Year, see http://www.math.nus.edu.sg/aslaksen/calendar/chinese.shtml
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetChineseNewYear(int year)
        {
            //first give the December solstice time
            DateTime temp0 = new DateTime(year - 1, 12, 15);
            DateTime temp1 = Earth.GetEclipticLongitudeTime(temp0, 90);

            DateTime sui0 = GetBeijingTime(temp1); //this is the begig of sui

            temp0 = new DateTime(year, 12, 15);
            temp1 = Earth.GetEclipticLongitudeTime(temp0, 90);

            DateTime sui1 = GetBeijingTime(temp1); //this is the begin of next sui

            ArrayList monthes = new ArrayList(); //hold the start of each months during this sui

            //apply the rule #4, find the number of new Moon in a sui
            temp0 = MoonPhases.TruePhase(sui0, MPhase.New); //next new Moon time
            temp0 = GetBeijingTime(temp0);
            monthes.Add(temp0);
            int newYearMonth = 1; //for normal year, New Year would be the second monthes

            if (temp0 == sui0) //new moon at the same date as begining of sui, it is 11 month
            {
                newYearMonth = 2;
            }

            while (temp0 < sui1)
            {
                temp1 = MoonPhases.TruePhase(temp0.AddDays(28), MPhase.New); //next new Moon time
                temp1 = GetBeijingTime(temp1);
                if (temp1 < sui1)
                {
                    monthes.Add(temp1);
                    temp0 = temp1;
                }
                else
                    break;
            }

            if (monthes.Count == 13 && newYearMonth == 1) //leap sui
            {
                //apply rule #5, see if the month 11 or 12th is leap month;
                temp0 = Earth.GetEclipticLongitudeTime((DateTime)monthes[0], 105);
                temp0 = GetBeijingTime(temp0);
                if (temp0 > (DateTime)monthes[1]) //first monthes is leap month
                {
                    newYearMonth = 2;
                }
                else
                {
                    //apply rule #5, see if the month 11 or 12th is leap month;
                    temp0 = Earth.GetEclipticLongitudeTime((DateTime)monthes[1], 120);
                    temp0 = GetBeijingTime(temp0);
                    if (temp0 > (DateTime)monthes[2]) //second monthes is leap month
                        newYearMonth = 2;
                }

            }

            return (DateTime)monthes[newYearMonth];

        }

        /// <summary>
        /// Get Chinese name of the year that cycle every 60 years
        /// </summary>
        /// <returns></returns>
        public static string GetYear(int year)
        {
            int index = (year - 1984) % 60; //1984 is the first year of cycle
            if (index < 0)
                index += 60;

            int tianIndex = index % 10;
            int diIndex = index % 12; //dizhi

            return tianGan[tianIndex] + diZhi[diIndex];
        }

        /// <summary>
        /// Get the animal  symbol of Chinese year
        /// </summary>
        /// <param name="year">Year in Gregorian Calendar</param>
        /// <returns></returns>
        public static string GetYearSymbol(int year)
        {
            int index = (year - 1984) % 60; //1984 is the first year of cycle
            if (index < 0)
                index += 60;

            int diIndex = index % 12; //dizhi

            return animals[diIndex];
        }

        /// <summary>
        /// get the next jieqi on or after a specific date
        /// </summary>
        /// <param name="startDate">start date on Beijing Time</param>
        /// <returns></returns>
        public static CJieqi GetNextJieqi(DateTime startDate)
        {
            double d0 = DateTimeUtil.DateTimeToJulian(startDate.AddHours(-1 * BEJING_TIME_ZONE));
            double longitude0 = Earth.EclipticLongitude(d0);
            int nextJieqi = (int)Math.Floor(longitude0 / 15) + 1;
            int nextDegree = nextJieqi * 15;

            DateTime nextJieqiTime = Earth.GetEclipticLongitudeTime(startDate, nextDegree);

            return new CJieqi(nextJieqi % 24, nextJieqiTime.AddHours(BEJING_TIME_ZONE));
        }

        public static MonthData GetMonthData(int year, int month)
        {
            MonthData monthData = new MonthData();
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int chineseYear = CHINESE_LS_CALENDAR.GetYear(endDate);
            monthData.CY = startDate.ToString("yyyy MMM") + "(" + GetYear(chineseYear) + "  " + GetYearSymbol(chineseYear) + ")";





            return monthData;
        }

        public static String GetChineseDate(DateTime date)
        {
            int cday = CHINESE_LS_CALENDAR.GetDayOfMonth(date);
            return CHINESE_DAYS[cday - 1];
        }
    }

    public class CJieqi
    {
        /* Name of Chinese Jieqi in unicode */
        private static string[] jieqiNames = new string[24] {
        "\x7ACB\x6625", "\x96E8\x6C34", "\x60CA\x86F0", "\x6625\x5206", "\x6E05\x660E", "\x8C37\x96E8", //spring
        "\x7ACB\x590F", "\x5C0F\x6EE1", "\x8292\x79CD", "\x590F\x81F3", "\x5C0F\x6691", "\x5927\x6691", //summer
        "\x7ACB\x79CB", "\x5904\x6691", "\x767D\x9732", "\x79CB\x5206", "\x5BD2\x9732", "\x971C\x964D", //fall
        "\x7ACB\x51AC", "\x5C0F\x96EA", "\x5927\x96EA", "\x51AC\x81F3", "\x5C0F\x5BD2", "\x5927\x5BD2"  //winter
    };

        int jieqi;
        DateTime date; //the date time of Jieqi in Beijing time

        public CJieqi(int j, DateTime dt)
        {
            jieqi = j;
            date = dt;
        }

        public int Jieqi
        {
            get { return jieqi; }
            set { jieqi = value; }
        }

        public string Name
        {
            get { return jieqiNames[(jieqi + 15) % 24]; }
        }

        /// <summary>
        /// return the date time in Beijing Time
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
    }

    public class DateTimeUtil
    {
        /// <summary>
        /// the OA date start from midnight, 30 December 1899
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double DateTimeToJulian(DateTime dt)
        {
            return dt.ToOADate() + 2415018.5;
        }

        public static DateTime JulianToDateTime(double julianDate)
        {
            return DateTime.FromOADate(julianDate - 2415018.5);
        }
    }

    /// <summary>
    /// Calculate the earth longitude relate to sun
    /// code translate from AA+ v1.30 AAEarth.cpp from http://www.naughter.com/aa.html
    /// </summary>
    public class Earth
    {
        private static double[,] g_L0EarthCoefficients = new double[64, 3]
    {
        { 175347046, 0,         0 },
        { 3341656,   4.6692568, 6283.0758500 },
        { 34894,     4.62610,   12566.15170 },
        { 3497,      2.7441,    5753.3849 },
        { 3418,      2.8289,    3.5231 },
        { 3136,      3.6277,    77713.7715 },
        { 2676,      4.4181,    7860.4194 },
        { 2343,      6.1352,    3930.2097 },
        { 1324,      0.7425,    11506.7698 },
        { 1273,      2.0371,    529.6910 },
        { 1199,      1.1096,    1577.3435 },
        { 990,       5.233,     5884.927 },
        { 902,       2.045,     26.298 },
        { 857,       3.508,     398.149 },
        { 780,       1.179,     5223.694 },
        { 753,       2.533,     5507.553 },
        { 505,       4.583,     18849.228 },
        { 492,       4.205,     775.523 },
        { 357,       2.920,     0.067 },
        { 317,       5.849,     11790.629 },
        { 284,       1.899,     796.288 },
        { 271,       0.315,     10977.079 },
        { 243,       0.345,     5486.778 },
        { 206,       4.806,     2544.314 },
        { 205,       1.869,     5573.143 },
        { 202,       2.458,     6069.777 },
        { 156,       0.833,     213.299 },
        { 132,       3.411,     2942.463 },
        { 126,       1.083,     20.775 },
        { 115,       0.645,     0.980 },
        { 103,       0.636,     4694.003 },
        { 102,       0.976,     15720.839 },
        { 102,       4.267,     7.114 },
        { 99,        6.21,      2146.17 },
        { 98,        0.68,      155.42 },
        { 86,        5.98,      161000.69 },
        { 85,        1.30,      6275.96 },
        { 85,        3.67,      71430.70 },
        { 80,        1.81,      17260.15 },
        { 79,        3.04,      12036.46 },
        { 75,        1.76,      5088.63 },
        { 74,        3.50,      3154.69 },
        { 74,        4.68,      801.82 },
        { 70,        0.83,      9437.76 },
        { 62,        3.98,      8827.39 },
        { 61,        1.82,      7084.90 },
        { 57,        2.78,      6286.60 },
        { 56,        4.39,      14143.50 },
        { 56,        3.47,      6279.55 },
        { 52,        0.19,      12139.55 },
        { 52,        1.33,      1748.02 },
        { 51,        0.28,      5856.48 },
        { 49,        0.49,      1194.45 },
        { 41,        5.37,      8429.24 },
        { 41,        2.40,      19651.05 },
        { 39,        6.17,      10447.39 },
        { 37,        6.04,      10213.29 },
        { 37,        2.57,      1059.38 },
        { 36,        1.71,      2352.87 },
        { 36,        1.78,      6812.77 },
        { 33,        0.59,      17789.85 },
        { 30,        0.44,      83996.85 },
        { 30,        2.74,      1349.87 },
        { 25,        3.16,      4690.48 }
    };


        private static double[,] g_L1EarthCoefficients = new double[34, 3]
    {
        { 628331966747.0, 0,          0 },
        { 206059,         2.678235,   6283.075850 },
        { 4303,           2.6351,     12566.1517 },
        { 425,            1.590,      3.523 },
        { 119,            5.796,      26.298 },
        { 109,            2.966,      1577.344 },
        { 93,             2.59,       18849.23 },
        { 72,             1.14,       529.69 },
        { 68,             1.87,       398.15 },
        { 67,             4.41,       5507.55 },
        { 59,             2.89,       5223.69 },
        { 56,             2.17,       155.42 },
        { 45,             0.40,       796.30 },
        { 36,             0.47,       775.52 },
        { 29,             2.65,       7.11 },
        { 21,             5.43,       0.98 },
        { 19,             1.85,       5486.78 },
        { 19,             4.97,       213.30 },
        { 17,             2.99,       6275.96 },
        { 16,             0.03,       2544.31 },
        { 16,             1.43,       2146.17 },
        { 15,             1.21,       10977.08 },
        { 12,             2.83,       1748.02 },
        { 12,             3.26,       5088.63 },
        { 12,             5.27,       1194.45 },
        { 12,             2.08,       4694.00 },
        { 11,             0.77,       553.57 },
        { 10,             1.30,       6286.60 },
        { 10,             4.24,       1349.87 },
        { 9,              2.70,       242.73 },
        { 9,              5.64,       951.72 },
        { 8,              5.30,       2352.87 },
        { 6,              2.65,       9437.76 },
        { 6,              4.67,       4690.48 }
    };

        private static double[,] g_L2EarthCoefficients = new double[20, 3]
    {
        { 52919,  0,      0 },
        { 8720,   1.0721, 6283.0758 },
        { 309,    0.867,  12566.152 },
        { 27,     0.05,   3.52 },
        { 16,     5.19,   26.30 },
        { 16,     3.68,   155.42 },
        { 10,     0.76,   18849.23 },
        { 9,      2.06,   77713.77 },
        { 7,      0.83,   775.52 },
        { 5,      4.66,   1577.34 },
        { 4,      1.03,   7.11 },
        { 4,      3.44,   5573.14 },
        { 3,      5.14,   796.30 },
        { 3,      6.05,   5507.55 },
        { 3,      1.19,   242.73 },
        { 3,      6.12,   529.69 },
        { 3,      0.31,   398.15 },
        { 3,      2.28,   553.57 },
        { 2,      4.38,   5223.69 },
        { 2,      3.75,   0.98 }
    };

        private static double[,] g_L3EarthCoefficients = new double[7, 3]
    {
        { 289, 5.844, 6283.076 },
        { 35,  0,     0 },
        { 17,  5.49,  12566.15 },
        { 3,   5.20,  155.42 },
        { 1,   4.72,  3.52 },
        { 1,   5.30,  18849.23 },
        { 1,   5.97,  242.73 }
    };

        private static double[,] g_L4EarthCoefficients = new double[3, 3]
    {
        { 114, 3.142,  0 },
        { 8,   4.13,   6283.08 },
        { 1,   3.84,   12566.15 }
    };

        private static double[,] g_L5EarthCoefficients = new double[1, 3]
    {
        { 1, 3.14, 0 },
    };

        /// <summary>
        /// get earth ecliptic longitude with a specific Julian date
        /// </summary>
        /// <param name="JD"></param>
        /// <returns></returns>
        public static double EclipticLongitude(double JD)
        {
            double rho = (JD - 2451545) / 365250;
            double rhosquared = rho * rho;
            double rhocubed = rhosquared * rho;
            double rho4 = rhocubed * rho;
            double rho5 = rho4 * rho;

            //Calculate L0
            int nL0Coefficients = g_L0EarthCoefficients.Length / 3;
            double L0 = 0;
            int i;
            for (i = 0; i < nL0Coefficients; i++)
                L0 += g_L0EarthCoefficients[i, 0] * Math.Cos(g_L0EarthCoefficients[i, 1] + g_L0EarthCoefficients[i, 2] * rho);

            //Calculate L1
            int nL1Coefficients = g_L1EarthCoefficients.Length / 3;
            double L1 = 0;
            for (i = 0; i < nL1Coefficients; i++)
                L1 += g_L1EarthCoefficients[i, 0] * Math.Cos(g_L1EarthCoefficients[i, 1] + g_L1EarthCoefficients[i, 2] * rho);

            //Calculate L2
            int nL2Coefficients = g_L2EarthCoefficients.Length / 3;
            double L2 = 0;
            for (i = 0; i < nL2Coefficients; i++)
                L2 += g_L2EarthCoefficients[i, 0] * Math.Cos(g_L2EarthCoefficients[i, 1] + g_L2EarthCoefficients[i, 2] * rho);

            //Calculate L3
            int nL3Coefficients = g_L3EarthCoefficients.Length / 3;
            double L3 = 0;
            for (i = 0; i < nL3Coefficients; i++)
                L3 += g_L3EarthCoefficients[i, 0] * Math.Cos(g_L3EarthCoefficients[i, 1] + g_L3EarthCoefficients[i, 2] * rho);

            //Calculate L4
            int nL4Coefficients = g_L4EarthCoefficients.Length / 3;
            double L4 = 0;
            for (i = 0; i < nL4Coefficients; i++)
                L4 += g_L4EarthCoefficients[i, 0] * Math.Cos(g_L4EarthCoefficients[i, 1] + g_L4EarthCoefficients[i, 2] * rho);

            //Calculate L5
            int nL5Coefficients = g_L5EarthCoefficients.Length / 3;
            double L5 = 0;
            for (i = 0; i < nL5Coefficients; i++)
                L5 += g_L5EarthCoefficients[i, 0] * Math.Cos(g_L5EarthCoefficients[i, 1] + g_L5EarthCoefficients[i, 2] * rho);

            double value = (L0 + L1 * rho + L2 * rhosquared + L3 * rhocubed + L4 * rho4 + L5 * rho5) / 100000000;

            //convert results back to degrees
            value = CoordinateTransformation.MapTo0To360Range(CoordinateTransformation.RadiansToDegrees(value));
            return value;
        }


        /// <summary>
        /// get the date tme that the earth would be in a specific longitude after a certain date
        /// </summary>
        /// <param name="startTime">strtTime in UTC</param>
        /// <param name="degree">the degree (1-360) of longitude</param>
        /// <returns></returns>
        public static DateTime GetEclipticLongitudeTime(DateTime startDate, int degree)
        {
            double d0 = DateTimeUtil.DateTimeToJulian(startDate);
            double longitude0 = EclipticLongitude(d0);

            //estimate the next degree time
            double d1 = d0 + (degree - longitude0) * 365.25 / 360;

            double longitude1 = EclipticLongitude(d1);

            double diff = (degree == 360 && longitude1 > 0 && longitude1 < 15) ? longitude1 : longitude1 - degree;

            int count = 0;

            while (Math.Abs(diff) > 0.0001 && count < 10)
            {
                // AppConfig.LogDebug(String.Format("d1={0:#.####}, Longitude1={1:#.####}, Diff={2:#.####}, count={3}", d1, longitude1, diff, count));

                //estimate the next time, try to find the middle 
                d1 -= diff * 365.25 / 360;
                longitude1 = EclipticLongitude(d1);
                diff = (degree == 360 && longitude1 > 0 && longitude1 < 15) ? longitude1 : longitude1 - degree;
                count++;
            }

            return DateTimeUtil.JulianToDateTime(d1);
        }
    }

    public enum MPhase : int
    {
        New = 0,
        FirstQuater = 1,
        Full = 2,
        LastQuater = 3
    }

    /// <summary>
    ///  Calculate Monn phase un UTC time
    /// code translate from AA+ v1.30 AAMoonPhase.cpp from http://www.naughter.com/aa.html
    /// </summary>
    public class MoonPhases
    {

        const double MOON_PERIOD = 29.530588861;

        //Jan 6, 2002	6:14 PM UTC is a new moon time
        //check http://www.timeanddate.com/calendar/moonphases.html
        static DateTime NEW_MOON_TIME = new DateTime(2000, 1, 6, 14, 20, 38);

        /// <summary>
        /// Calculate the approximate mean time of various phases of the moon.
        /// This will give the approximate time that the specified phase
        /// will occur near the given date.   This is only accurate to within
        /// some number of hours.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static DateTime MeanPhase(DateTime startDate, MPhase phase)
        {
            double ph = 0.0;

            if (phase == MPhase.FirstQuater)
                ph = 0.25;
            else if (phase == MPhase.Full)
                ph = 0.50;
            else if (phase == MPhase.LastQuater)
                ph = 0.75;

            TimeSpan tp = startDate - NEW_MOON_TIME;

            double k = tp.TotalDays / MOON_PERIOD;

            double kint = Math.Floor(k);
            double kfra = k - kint;

            if (kfra > ph)
                k = kint + 1 + ph; //we already pass that phase, look for the next one
            else
                k = kint + ph;

            double T = k / 1236.85;
            double T2 = T * T;
            double T3 = T2 * T;
            double T4 = T3 * T;

            double dayPass = MOON_PERIOD * k + 0.00015437 * T2 - 0.000000150 * T3 + 0.00000000073 * T4;

            //return DateTimeUtil.JulianToDateTime(dayPass);

            return NEW_MOON_TIME.AddDays(dayPass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="phase">0, 1/3, 1/2 or 3/4</param>
        /// <returns></returns>
        public static DateTime TruePhase(DateTime startDate, MPhase phase)
        {
            //What will be the return value
            double JD = DateTimeUtil.DateTimeToJulian(MeanPhase(startDate, phase));

            double ph = 0.0;

            if (phase == MPhase.FirstQuater)
                ph = 0.25;
            else if (phase == MPhase.Full)
                ph = 0.50;
            else if (phase == MPhase.LastQuater)
                ph = 0.75;
            TimeSpan tp = startDate - NEW_MOON_TIME;
            double k = tp.TotalDays / MOON_PERIOD;

            double kint = Math.Floor(k);
            double kfra = k - kint;

            if (kfra > ph)
                k = kint + 1 + ph; //we already pass that phase, look for the next one
            else
                k = kint + ph;

            //convert from K to T
            double T = k / 1236.85;
            double T2 = T * T;
            double T3 = T2 * T;
            double T4 = T3 * T;

            double E = 1 - 0.002516 * T - 0.0000074 * T2;
            double E2 = E * E;

            double M = CoordinateTransformation.MapTo0To360Range(2.5534 + 29.10535670 * k - 0.0000014 * T2 - 0.00000011 * T3);
            M = CoordinateTransformation.DegreesToRadians(M);
            double Mdash = CoordinateTransformation.MapTo0To360Range(201.5643 + 385.81693528 * k + 0.0107582 * T2 + 0.00001238 * T3 - 0.000000058 * T4);
            Mdash = CoordinateTransformation.DegreesToRadians(Mdash);
            double F = CoordinateTransformation.MapTo0To360Range(160.7108 + 390.67050284 * k - 0.0016118 * T2 - 0.00000227 * T3 + 0.00000001 * T4);
            F = CoordinateTransformation.DegreesToRadians(F);
            double omega = CoordinateTransformation.MapTo0To360Range(124.7746 - 1.56375588 * k + 0.0020672 * T2 + 0.00000215 * T3);
            omega = CoordinateTransformation.DegreesToRadians(omega);
            double A1 = CoordinateTransformation.MapTo0To360Range(299.77 + 0.107408 * k - 0.009173 * T2);
            A1 = CoordinateTransformation.DegreesToRadians(A1);
            double A2 = CoordinateTransformation.MapTo0To360Range(251.88 + 0.016321 * k);
            A2 = CoordinateTransformation.DegreesToRadians(A2);
            double A3 = CoordinateTransformation.MapTo0To360Range(251.83 + 26.651886 * k);
            A3 = CoordinateTransformation.DegreesToRadians(A3);
            double A4 = CoordinateTransformation.MapTo0To360Range(349.42 + 36.412478 * k);
            A4 = CoordinateTransformation.DegreesToRadians(A4);
            double A5 = CoordinateTransformation.MapTo0To360Range(84.66 + 18.206239 * k);
            A5 = CoordinateTransformation.DegreesToRadians(A5);
            double A6 = CoordinateTransformation.MapTo0To360Range(141.74 + 53.303771 * k);
            A6 = CoordinateTransformation.DegreesToRadians(A6);
            double A7 = CoordinateTransformation.MapTo0To360Range(207.14 + 2.453732 * k);
            A7 = CoordinateTransformation.DegreesToRadians(A7);
            double A8 = CoordinateTransformation.MapTo0To360Range(154.84 + 7.306860 * k);
            A8 = CoordinateTransformation.DegreesToRadians(A8);
            double A9 = CoordinateTransformation.MapTo0To360Range(34.52 + 27.261239 * k);
            A9 = CoordinateTransformation.DegreesToRadians(A9);
            double A10 = CoordinateTransformation.MapTo0To360Range(207.19 + 0.121824 * k);
            A10 = CoordinateTransformation.DegreesToRadians(A10);
            double A11 = CoordinateTransformation.MapTo0To360Range(291.34 + 1.844379 * k);
            A11 = CoordinateTransformation.DegreesToRadians(A11);
            double A12 = CoordinateTransformation.MapTo0To360Range(161.72 + 24.198154 * k);
            A12 = CoordinateTransformation.DegreesToRadians(A12);
            double A13 = CoordinateTransformation.MapTo0To360Range(239.56 + 25.513099 * k);
            A13 = CoordinateTransformation.DegreesToRadians(A13);
            double A14 = CoordinateTransformation.MapTo0To360Range(331.55 + 3.592518 * k);
            A14 = CoordinateTransformation.DegreesToRadians(A14);

            //convert to radians
            if (phase == MPhase.New) //New Moon
            {
                double DeltaJD = -0.40720 * Math.Sin(Mdash) +
                      0.17241 * E * Math.Sin(M) +
                      0.01608 * Math.Sin(2 * Mdash) +
                      0.01039 * Math.Sin(2 * F) +
                      0.00739 * E * Math.Sin(Mdash - M) +
                      -0.00514 * E * Math.Sin(Mdash + M) +
                      0.00208 * E2 * Math.Sin(2 * M) +
                      -0.00111 * Math.Sin(Mdash - 2 * F) +
                      -0.00057 * Math.Sin(Mdash + 2 * F) +
                      0.00056 * E * Math.Sin(2 * Mdash + M) +
                      -0.00042 * Math.Sin(3 * Mdash) +
                      0.00042 * E * Math.Sin(M + 2 * F) +
                      0.00038 * E * Math.Sin(M - 2 * F) +
                      -0.00024 * E * Math.Sin(2 * Mdash - M) +
                      -0.00017 * Math.Sin(omega) +
                      -0.00007 * Math.Sin(Mdash + 2 * M) +
                      0.00004 * Math.Sin(2 * Mdash - 2 * F) +
                      0.00004 * Math.Sin(3 * M) +
                      0.00003 * Math.Sin(Mdash + M - 2 * F) +
                      0.00003 * Math.Sin(2 * Mdash + 2 * F) +
                      -0.00003 * Math.Sin(Mdash + M + 2 * F) +
                      0.00003 * Math.Sin(Mdash - M + 2 * F) +
                      -0.00002 * Math.Sin(Mdash - M - 2 * F) +
                      -0.00002 * Math.Sin(3 * Mdash + M) +
                      0.00002 * Math.Sin(4 * Mdash);
                JD += DeltaJD;
            }
            else if ((phase == MPhase.FirstQuater) || (phase == MPhase.LastQuater)) //First Quarter or Last Quarter
            {
                double DeltaJD = -0.62801 * Math.Sin(Mdash) +
                      0.17172 * E * Math.Sin(M) +
                      -0.01183 * E * Math.Sin(Mdash + M) +
                      0.00862 * Math.Sin(2 * Mdash) +
                      0.00804 * Math.Sin(2 * F) +
                      0.00454 * E * Math.Sin(Mdash - M) +
                      0.00204 * E2 * Math.Sin(2 * M) +
                      -0.00180 * Math.Sin(Mdash - 2 * F) +
                      -0.00070 * Math.Sin(Mdash + 2 * F) +
                      -0.00040 * Math.Sin(3 * Mdash) +
                      -0.00034 * E * Math.Sin(2 * Mdash - M) +
                      0.00032 * E * Math.Sin(M + 2 * F) +
                      0.00032 * E * Math.Sin(M - 2 * F) +
                      -0.00028 * E2 * Math.Sin(Mdash + 2 * M) +
                      0.00027 * E * Math.Sin(2 * Mdash + M) +
                      -0.00017 * Math.Sin(omega) +
                      -0.00005 * Math.Sin(Mdash - M - 2 * F) +
                      0.00004 * Math.Sin(2 * Mdash + 2 * F) +
                      -0.00004 * Math.Sin(Mdash + M + 2 * F) +
                      0.00004 * Math.Sin(Mdash - 2 * M) +
                      0.00003 * Math.Sin(Mdash + M - 2 * F) +
                      0.00003 * Math.Sin(3 * M) +
                      0.00002 * Math.Sin(2 * Mdash - 2 * F) +
                      0.00002 * Math.Sin(Mdash - M + 2 * F) +
                      -0.00002 * Math.Sin(3 * Mdash + M);
                JD += DeltaJD;

                double W = 0.00306 - 0.00038 * E * Math.Cos(M) + 0.00026 * Math.Cos(Mdash) - 0.00002 * Math.Cos(Mdash - M) + 0.00002 * Math.Cos(Mdash + M) + 0.00002 * Math.Cos(2 * F);
                if (phase == MPhase.FirstQuater) //First quarter
                    JD += W;
                else
                    JD -= W;
            }
            else if (phase == MPhase.Full) //Full Moon
            {
                double DeltaJD = -0.40614 * Math.Sin(Mdash) +
                      0.17302 * E * Math.Sin(M) +
                      0.01614 * Math.Sin(2 * Mdash) +
                      0.01043 * Math.Sin(2 * F) +
                      0.00734 * E * Math.Sin(Mdash - M) +
                      -0.00514 * E * Math.Sin(Mdash + M) +
                      0.00209 * E2 * Math.Sin(2 * M) +
                      -0.00111 * Math.Sin(Mdash - 2 * F) +
                      -0.00057 * Math.Sin(Mdash + 2 * F) +
                      0.00056 * E * Math.Sin(2 * Mdash + M) +
                      -0.00042 * Math.Sin(3 * Mdash) +
                      0.00042 * E * Math.Sin(M + 2 * F) +
                      0.00038 * E * Math.Sin(M - 2 * F) +
                      -0.00024 * E * Math.Sin(2 * Mdash - M) +
                      -0.00017 * Math.Sin(omega) +
                      -0.00007 * Math.Sin(Mdash + 2 * M) +
                      0.00004 * Math.Sin(2 * Mdash - 2 * F) +
                      0.00004 * Math.Sin(3 * M) +
                      0.00003 * Math.Sin(Mdash + M - 2 * F) +
                      0.00003 * Math.Sin(2 * Mdash + 2 * F) +
                      -0.00003 * Math.Sin(Mdash + M + 2 * F) +
                      0.00003 * Math.Sin(Mdash - M + 2 * F) +
                      -0.00002 * Math.Sin(Mdash - M - 2 * F) +
                      -0.00002 * Math.Sin(3 * Mdash + M) +
                      0.00002 * Math.Sin(4 * Mdash);
                JD += DeltaJD;
            }


            //Additional corrections for all phases
            double DeltaJD2 = 0.000325 * Math.Sin(A1) +
                  0.000165 * Math.Sin(A2) +
                  0.000164 * Math.Sin(A3) +
                  0.000126 * Math.Sin(A4) +
                  0.000110 * Math.Sin(A5) +
                  0.000062 * Math.Sin(A6) +
                  0.000060 * Math.Sin(A7) +
                  0.000056 * Math.Sin(A8) +
                  0.000047 * Math.Sin(A9) +
                  0.000042 * Math.Sin(A10) +
                  0.000040 * Math.Sin(A11) +
                  0.000037 * Math.Sin(A12) +
                  0.000035 * Math.Sin(A13) +
                  0.000023 * Math.Sin(A14);
            JD += DeltaJD2;

            return DateTimeUtil.JulianToDateTime(JD);
        }

    }

    /// <summary>
    /// 2D coordinate class
    /// </summary>
    public class D2Coordinate
    {
        double x;
        double y;

        public D2Coordinate()
        {

        }

        public D2Coordinate(double aX, double aY)
        {
            x = aX;
            y = aY;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

    }

    /// <summary>
    /// 3D coordinate class
    /// </summary>
    /*
    public class D3Coordinate
    {
        double x;
        double y;
        double z;

        public D3Coordinate()
        {

        }

        public D3Coordinate(double aX, double aY, double aZ)
        {
            x = aX;
            y = aY;
            z = aZ;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Z
        {
            get { return z; }
            set { z = value; }
        }
    }
    */

    public class CoordinateTransformation
    {
        public static D2Coordinate Equatorial2Ecliptic(double Alpha, double Delta, double Epsilon)
        {
            Alpha = HoursToRadians(Alpha);
            Delta = DegreesToRadians(Delta);
            Epsilon = DegreesToRadians(Epsilon);

            D2Coordinate Ecliptic = new D2Coordinate();
            Ecliptic.X = RadiansToDegrees(Math.Atan2(Math.Sin(Alpha) * Math.Cos(Epsilon) + Math.Tan(Delta) * Math.Sin(Epsilon), Math.Cos(Alpha)));
            if (Ecliptic.X < 0)
                Ecliptic.X += 360;
            Ecliptic.Y = RadiansToDegrees(Math.Asin(Math.Sin(Delta) * Math.Cos(Epsilon) - Math.Cos(Delta) * Math.Sin(Epsilon) * Math.Sin(Alpha)));

            return Ecliptic;
        }

        public static D2Coordinate Ecliptic2Equatorial(double Lambda, double Beta, double Epsilon)
        {
            Lambda = DegreesToRadians(Lambda);
            Beta = DegreesToRadians(Beta);
            Epsilon = DegreesToRadians(Epsilon);

            D2Coordinate Equatorial = new D2Coordinate();
            Equatorial.X = RadiansToHours(Math.Atan2(Math.Sin(Lambda) * Math.Cos(Epsilon) - Math.Tan(Beta) * Math.Sin(Epsilon), Math.Cos(Lambda)));
            if (Equatorial.X < 0)
                Equatorial.X += 24;
            Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(Beta) * Math.Cos(Epsilon) + Math.Cos(Beta) * Math.Sin(Epsilon) * Math.Sin(Lambda)));

            return Equatorial;
        }

        public static D2Coordinate Equatorial2Horizontal(double LocalHourAngle, double Delta, double Latitude)
        {
            LocalHourAngle = HoursToRadians(LocalHourAngle);
            Delta = DegreesToRadians(Delta);
            Latitude = DegreesToRadians(Latitude);

            D2Coordinate Horizontal = new D2Coordinate();
            Horizontal.X = RadiansToDegrees(Math.Atan2(Math.Sin(LocalHourAngle), Math.Cos(LocalHourAngle) * Math.Sin(Latitude) - Math.Tan(Delta) * Math.Cos(Latitude)));
            if (Horizontal.X < 0)
                Horizontal.X += 360;
            Horizontal.Y = RadiansToDegrees(Math.Asin(Math.Sin(Latitude) * Math.Sin(Delta) + Math.Cos(Latitude) * Math.Cos(Delta) * Math.Cos(LocalHourAngle)));

            return Horizontal;
        }

        public static D2Coordinate Horizontal2Equatorial(double Azimuth, double Altitude, double Latitude)
        {
            //Convert from degress to radians
            Azimuth = DegreesToRadians(Azimuth);
            Altitude = DegreesToRadians(Altitude);
            Latitude = DegreesToRadians(Latitude);

            D2Coordinate Equatorial = new D2Coordinate();
            Equatorial.X = RadiansToHours(Math.Atan2(Math.Sin(Azimuth), Math.Cos(Azimuth) * Math.Sin(Latitude) + Math.Tan(Altitude) * Math.Cos(Latitude)));
            if (Equatorial.X < 0)
                Equatorial.X += 24;
            Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(Latitude) * Math.Sin(Altitude) - Math.Cos(Latitude) * Math.Cos(Altitude) * Math.Cos(Azimuth)));

            return Equatorial;
        }

        public static D2Coordinate Equatorial2Galactic(double Alpha, double Delta)
        {
            Alpha = 192.25 - HoursToDegrees(Alpha);
            Alpha = DegreesToRadians(Alpha);
            Delta = DegreesToRadians(Delta);

            D2Coordinate Galactic = new D2Coordinate();
            Galactic.X = RadiansToDegrees(Math.Atan2(Math.Sin(Alpha), Math.Cos(Alpha) * Math.Sin(DegreesToRadians(27.4)) - Math.Tan(Delta) * Math.Cos(DegreesToRadians(27.4))));
            Galactic.X = 303 - Galactic.X;
            if (Galactic.X >= 360)
                Galactic.X -= 360;
            Galactic.Y = RadiansToDegrees(Math.Asin(Math.Sin(Delta) * Math.Sin(DegreesToRadians(27.4)) + Math.Cos(Delta) * Math.Cos(DegreesToRadians(27.4)) * Math.Cos(Alpha)));

            return Galactic;
        }

        public static D2Coordinate Galactic2Equatorial(double l, double b)
        {
            l -= 123;
            l = DegreesToRadians(l);
            b = DegreesToRadians(b);

            D2Coordinate Equatorial = new D2Coordinate();
            Equatorial.X = RadiansToDegrees(Math.Atan2(Math.Sin(l), Math.Cos(l) * Math.Sin(DegreesToRadians(27.4)) - Math.Tan(b) * Math.Cos(DegreesToRadians(27.4))));
            Equatorial.X += 12.25;
            if (Equatorial.X < 0)
                Equatorial.X += 360;
            Equatorial.X = DegreesToHours(Equatorial.X);
            Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(b) * Math.Sin(DegreesToRadians(27.4)) + Math.Cos(b) * Math.Cos(DegreesToRadians(27.4)) * Math.Cos(l)));

            return Equatorial;
        }

        public static double DegreesToRadians(double Degrees)
        {
            return Degrees * 0.017453292519943295769236907684886;
        }

        public static double RadiansToDegrees(double Radians)
        {
            return Radians * 57.295779513082320876798154814105;
        }

        public static double RadiansToHours(double Radians)
        {
            return Radians * 3.8197186342054880584532103209403;
        }

        public static double HoursToRadians(double Hours)
        {
            return Hours * 0.26179938779914943653855361527329;
        }

        static double HoursToDegrees(double Hours)
        {
            return Hours * 15;
        }

        public static double DegreesToHours(double Degrees)
        {
            return Degrees / 15;
        }

        static double PI()
        {
            return 3.1415926535897932384626433832795;
        }

        public static double MapTo0To360Range(double Degrees)
        {
            double Value = Degrees;

            //map it to the range 0 - 360
            while (Value < 0)
                Value += 360;
            while (Value > 360)
                Value -= 360;

            return Value;
        }

        public static double MapTo0To24Range(double HourAngle)
        {
            double Value = HourAngle;

            //map it to the range 0 - 24
            while (Value < 0)
                Value += 24;
            while (Value > 24)
                Value -= 24;

            return Value;
        }

        /// <summary>
        /// return decimal degree with the degree, minutes and seconds
        /// </summary>
        /// <param name="Degrees"></param>
        /// <param name="Minutes"></param>
        /// <param name="Seconds"></param>
        /// <param name="bPositive"></param>
        /// <returns></returns>
        public static double DMSToDegrees(double Degrees, double Minutes, double Seconds, bool bPositive)
        {
            if (bPositive)
                return Degrees + Minutes / 60 + Seconds / 3600;
            else
                return -Degrees - Minutes / 60 - Seconds / 3600;

        }

    }

    public class ChineseDate
    {
        public string JQ { get; set; } //JieQi
        public string CD { get; set; } //Chinese Day
        public int D { get; set; } //Day of month
        public int W { get; set; } //day of week
    }

    public class MonthData
    {
        public string CY { get; set; } //Chinese Year
        public string CM { get; set; } //Chinese Month
        public int M { get; set; }
        public int Y { get; set; }
        List<ChineseDate> CDates { get; set; }
    }
}