using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    /// <summary>
    /// 常用且较为通用的工具类，大多数是极其常用的类型转换。
    /// 包括一些类型安全的转换，包容null值，溢出等异常情况。
    /// </summary>
    public static class Kit
    {

        /// <summary>
        /// 转换到DateTime类型，遇到非法转换则返回DateTime.MinValue
        /// 用例：
        /// ToDateTime(20160805,93000)
        /// ToDateTime(20160805,93000000)
        /// </summary>
        /// <param name="tdate"></param>
        /// <param name="ttime"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(int tdate, int ttime, bool considerMillis = true)
        {
            int y = tdate / 10000, m = (tdate % 10000) / 100, d = tdate % 100;
            int millisecond = 0;
            if (y < 1 || m == 0 || d == 0)
                return DateTime.MinValue;
            if (considerMillis && ttime > 240000)
            {
                millisecond = ttime % 1000;
                ttime = ttime / 1000;   //ttime可能包含了毫秒值
            }
            if (ttime < 0 || ttime > 240000)
                return DateTime.MinValue;
            return new DateTime(y, m, d,
                 ttime / 10000, (ttime % 10000) / 100, ttime % 100, millisecond);
        }
        public static DateTime ToDateTime(string tdate, string ttime)
        {
            int d = 0, t = 0;
            bool ok1 = int.TryParse(tdate, out d);
            bool ok2 = int.TryParse(ttime, out t);
            if (ok1 && ok2)
            {
                return ToDateTime(d, t);

            }
            return _tryParseToDateTime(tdate + ttime, "yyyy/MM/dd HH:mm:ss fff");
        }

        public static DateTime ToDateTime(object tdate, object ttime)
        {
            return ToDateTime(tdate.ToString(), ttime.ToString());
        }
        /// <summary>
        /// 转换到DateTime类型，遇到非法转换则返回DateTime.MinValue，用例：
        /// ToDateTime(20160805)
        /// ToDateTime(20160805093000)
        /// ToDateTime(20160805093000000)
        /// </summary>
        /// <param name="arg">类似20160805093000</param>
        /// <returns></returns>
        public static DateTime ToDateTime(long arg)
        {
            if (arg >= 10000000000000000) arg = arg / 1000; //可能包含了毫秒值
            if (arg < 100000000) arg = arg * 1000000; //可能未包含hhmmss
            return ToDateTime((int)(arg / 1000000), (int)(arg % 1000000));
        }

        /// <summary>
        /// 转换到DateTime类型,用例：
        /// ToDateTime(20160805093000)
        /// </summary>
        /// <param name="arg">类似20160805093000</param>
        /// <returns></returns>
        public static DateTime ToDateTime(int arg)
        {
            return ToDateTime(arg / 1000000, arg % 1000000, false);
        }

        /// <summary>
        /// 转换到DateTime类型,用例：
        /// ToDateTime(20160805093000)
        /// </summary>
        /// <param name="arg">类似yyyyMMddhhmmss</param>
        /// <returns></returns>
        public static DateTime ToDateTime(string arg)
        {
            long x = 0;
            bool ok = long.TryParse(arg, out x);
            if (ok)
            {
                return ToDateTime(x);
            }
            return _tryParseToDateTime(arg, new string[] { "yyyy/MM/dd HH:mm:ss fff", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" });
        }

        public static DateTime ToDateTime(object arg)
        {
            if (arg == null)
                return DateTime.MinValue;
            if (arg.GetType() == typeof(string))
                return ToDateTime((string)arg);

            if (arg.GetType() == typeof(decimal))
                return ToDateTime(Convert.ToInt64(arg));
            if (arg.GetType() == typeof(int))
                return ToDateTime(Convert.ToInt64(arg));
            if (arg.GetType() == typeof(long))
                return ToDateTime((long)arg);

            return DateTime.MinValue;
        }

        /// <summary>
        /// 转换到DateTime类型,不含hhmmss,用例：
        /// ToDateTime(20160805),相当于ToDateTime(20160805000000)
        /// </summary>
        /// <param name="arg">类似yyyyMMdd</param>
        /// <returns></returns>
        public static DateTime ToDate(object arg)
        {
            if (arg == null)
                return DateTime.MinValue;
            if (arg.GetType() == typeof(string))
                return ToDate((string)arg);
            if (arg.GetType() == typeof(int))
                return ToDateTime((int)arg, 0);
            return ToDateTime(arg).Date;
        }

        public static DateTime ToDate(string arg)
        {
            long d = 0;

            bool ok = long.TryParse(arg, out d);
            if (ok)
            {
                return ToDateTime(d, 0);
            }
            return _tryParseToDateTime(arg, new string[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss fff" }).Date;
        }


        private static DateTime _tryParseToDateTime(string arg, string pattern = "yyyy/MM/dd HH:mm:ss fff")
        {
            return _tryParseToDateTime(arg, new string[] { pattern });
        }
        private static DateTime _tryParseToDateTime(string arg, string[] patterns)
        {
            var res = DateTime.MinValue;
            DateTime.TryParseExact(arg, patterns, CultureInfo.InvariantCulture, DateTimeStyles.None, out res);
            return res;
        }

        /// <summary>
        /// 返回值类似20160805093000
        /// </summary>
        /// <param name="t">时间(DateTime)</param>
        /// <returns></returns>
        public static int ToInt_yyyyMMddHHmmss(DateTime t)
        {
            return (t.Year * 10000 + t.Month * 100 + t.Day) * 1000000
            + t.Hour * 10000 + t.Minute * 100 + t.Second;
        }

        /// <summary>
        /// 返回值类似93000000
        /// </summary>
        /// <param name="t">时间(DateTime)</param>
        /// <returns></returns>
        public static int ToInt_HHmmssfff(DateTime t)
        {
            return t.Hour * 10000000 + t.Minute * 100000 + t.Second * 1000 + t.Millisecond;
        }


        /// <summary>
        /// 返回值类似2016080509
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int ToInt_yyyyMMdd(DateTime t)
        {
            return (t.Year * 10000 + t.Month * 100 + t.Day);
        }

        /// <summary>
        /// 安全的类型转换。
        /// 相较于Convert.ToInt32(object)更为安全。
        /// 若发生溢出，null值等无法转换的情形，返回0
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static int ToInt(object arg)
        {
            if (arg == null)
                return 0;
            if (arg.GetType() == typeof(decimal))
                return Convert.ToInt32((decimal)arg);
            if (arg.GetType() == typeof(int))
                return (int)arg;
            if (arg.GetType() == typeof(double))
                return Convert.ToInt32((double)arg);
            if (arg.GetType() == typeof(long))
                return Convert.ToInt32((long)arg);
            if (arg.GetType() == typeof(string))
            {
                int r = 0;
                int.TryParse((string)arg, out r);
                return r;
            }
            return 0;
        }

        /// <summary>
        /// 安全的类型转换。
        /// 相较于Convert.ToInt64(object)更为安全。
        /// 若发生溢出，null值等无法转换的情形，返回0
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static long ToLong(object arg)
        {
            if (arg == null)
                return 0;
            if (arg.GetType() == typeof(decimal))
                return Convert.ToInt64((decimal)arg);
            if (arg.GetType() == typeof(int))
                return Convert.ToInt64((int)arg); ;
            if (arg.GetType() == typeof(double))
                return Convert.ToInt64((double)arg); ;
            if (arg.GetType() == typeof(long))
                return (long)arg;
            if (arg.GetType() == typeof(string))
            {
                long r = 0;
                long.TryParse((string)arg, out r);
                return r;
            }
            return 0;
        }

        /// <summary>
        /// 安全的类型转换。
        /// 相较于Convert.ToDouble(object)更为安全。
        /// 若发生溢出，null值等无法转换的情形，返回0
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static double ToDouble(object arg)
        {
            if (arg == null)
                return 0;
            if (arg.GetType() == typeof(decimal))
                return Convert.ToDouble((decimal)arg);
            if (arg.GetType() == typeof(int))
                return Convert.ToDouble((int)arg);
            if (arg.GetType() == typeof(double))
                return (double)arg;
            if (arg.GetType() == typeof(long))
                return Convert.ToDouble((long)arg);
            if (arg.GetType() == typeof(string))
            {
                double r = 0;
                double.TryParse((string)arg, out r);
                return r;
            }
            return 0;
        }

        /// <summary>
        /// 万能转换,更简洁的表达
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static object To<T>(object arg)
        {
            return To(typeof(T), arg);
        }

        /// <summary>
        /// 万能转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static object To(Type type, object arg)
        {
            if (arg == null)
                return null;
            if (type == typeof(DateTime))
                return Kit.ToDateTime(arg);
            if (type == typeof(int))
                return Kit.ToInt(arg);
            if (type == typeof(string))
                return arg.ToString();
            if (type == typeof(double))
                return Kit.ToDouble(arg);
            if (type == typeof(long))
                return Kit.ToLong(arg);
            return arg;
        }

        public static string ToShortName(string text)
        {
            if (text == null) return null;
            int x = text.LastIndexOf('.');
            return text.Substring(x + 1);
        }

    }
}
