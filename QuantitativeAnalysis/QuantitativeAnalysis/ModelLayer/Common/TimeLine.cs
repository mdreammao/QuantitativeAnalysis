using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{


    /// <summary>
    /// 时间线
    /// </summary>
    public class TimeLine
    {
        TimeLineSection[] sections;
        public IList<int> Millis { get; }  //毫秒序列

        /// <summary>
        /// 用若干时间段拼接构造的时间线
        /// </summary>
        public TimeLine(params TimeLineSection[] sections)
        {
            this.sections = sections;
            Millis = new List<int>();
            int i, k = -1;
            for (i = 0; i < sections.Count(); i++)
            {
                var sect = sections[i];
                if (k == sect.start) k += sect.interval;
                for (k = sect.start; k <= sect.end; k += sect.interval)
                {
                    Millis.Add(k);
                }
            }

        }

        /// <summary>
        /// 用一个时间段构造的时间线
        /// </summary>
        public TimeLine(TimeLineSection section) : this(new TimeLineSection[1] { section })
        {

        }

        /// <summary>
        /// 直接用具体的时间序列（当天毫秒数）构造的时间线
        /// </summary>
        public TimeLine(IList<int> millis)
        {
            this.Millis = millis;
        }
        /// <summary>
        /// 直接用具体的时间序列（DateTime）构造的时间线
        /// </summary>
        public TimeLine(IList<DateTime> times)
        {
            Millis = times.Select(t => (int)t.TimeOfDay.TotalMilliseconds).ToList();
        }

    }

    /// <summary>
    /// 均匀的时间线段，表示均匀分布的采样点序列。主要参数由起始时间，结束时间，间隔毫秒数构成
    /// </summary>
    public struct TimeLineSection
    {
        public int start, end, interval; //millisecond values

        public TimeLineSection(int millisStart, int millisEnd, int millisInterval)
        {
            start = millisStart;
            end = millisEnd;
            interval = millisInterval;
        }

        public TimeLineSection(string timeStart, string timeEnd, int millisInterval)
        {
            start = ToIntOfMillis(timeStart);
            end = ToIntOfMillis(timeEnd);
            interval = millisInterval;
        }

        public TimeLineSection(DateTime timeStart, DateTime timeEnd, int millisInterval)
        {
            start = (int)timeStart.TimeOfDay.TotalMilliseconds;
            end = (int)timeEnd.TimeOfDay.TotalMilliseconds;
            interval = millisInterval;
        }
        public List<int> ToListOfMillis()
        {
            var res = new List<int>((end - start) / interval + 1);
            for (int k = start; k <= end; k += interval)
            {
                res.Add(k);
            }
            return res;
        }

        // "09:30:15.400" -> 1945815400
        static int ToIntOfMillis(string src)
        {
            DateTime dt = DateTime.ParseExact(src, "HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            return (int)dt.TimeOfDay.TotalMilliseconds;
        }

    }

    public static class TimeLines
    {

    }
}
