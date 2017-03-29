
using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    /// <summary>
    /// 时间序列相关的工具类
    /// </summary>
    public static class SequentialUtils
    {

        /// <summary>
        /// 在已按时间递增排序好的list中二分查找指定时间的元素下标。
        /// 若找到返回匹配元素的下标，若没有返回不小于指定时间的元素最大下标值的相反数.
        /// 该实现应该比包装原生List.BinarySearch要快些
        /// </summary>
        /// <typeparam name="T">元素类型，必须继承了Sequential</typeparam>
        /// <param name="ascList">数据列表，确保已经按时间递增排序好</param>
        /// <param name="targetTime">指定时间</param>
        /// <returns>若找到返回匹配元素的下标，若没有返回不小于指定时间的元素最小下标值的补</returns>
        public static int BinarySearch<T>(List<T> ascList, DateTime targetTime) where T : Sequential
        {
            int lo = 0;
            int hi = ascList.Count - 1;
            int mid;
            while (lo <= hi)
            {
                mid = lo + (hi - lo) / 2;
                var tt = ascList[mid].time;
                if (ascList[mid].time > targetTime)
                {
                    hi = mid - 1;
                }
                else if (ascList[mid].time < targetTime)
                {
                    lo = mid + 1;
                }
                else
                {
                    return mid;
                }
            }
            return ~lo;
        }

        /// <summary>
        /// 返回一个新的list，是原来的按时间递增有序list的子集,即：
        /// timeEnd >= 新list任何元素 >= timeStart
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="timeStart">起始时间，包含本身</param>
        /// <param name="timeEnd">结束时间，包含本身</param>
        /// <returns></returns>
        public static List<T> GetRange<T>(List<T> src, DateTime timeStart, DateTime timeEnd) where T : Sequential
        {
            int x1 = BinarySearch(src, timeStart);
            int x2 = BinarySearch(src, timeEnd);
            x1 = x1 < 0 ? ~x1 : x1;
            x2 = x2 < 0 ? ~x2 - 1 : x2;
            return src.GetRange(x1, x2 - x1 + 1);
        }

        /// <summary>
        /// 是否是按时间递增有序的
        /// </summary>
        /// <returns></returns>
        public static bool IsOrderedAsc<T>(IList<T> src) where T : Sequential
        {
            for (int i = 0; i < src.Count - 1; i++)
            {
                if (src[i].time > src[i + 1].time) return false;
            }
            return true;
        }

        /// <summary>
        /// 根据给定的时间序列作为采样点对原始Sequential列表重新筛选，并将结果列表里每项的time字段与timeline对齐
        /// 筛选后的结果列表数量和采样序点列timeline的数量一致。
        /// 
        /// </summary>
        /// <typeparam name="T">Sequential且ICloneable</typeparam>
        /// <param name="src">必须是按时间递增的序列</param>
        /// <param name="timeline">时间采样点序列</param>
        ///  <param name="date">对齐的目标日期</param>
        /// <returns></returns>
        public static List<T> ResampleAndAlign<T>(IList<T> src, TimeLine timeline, DateTime date) where T : Sequential, ICloneable, new()
        {
            date = date.Date;
            var res = Resample(src, timeline);
            var timelineInMillis = timeline.Millis;
            //重新校正时间，让res与timeline对齐
            for (int i = 0; i < timelineInMillis.Count; i++)
            {
                if (res[i] != null)
                {
                    res[i] = (T)res[i].Clone();
                }
                else
                {
                    res[i] = new T();
                }
                res[i].time = date.AddMilliseconds(timelineInMillis[i]);
            }
            return res.ToList();
        }

        /// <summary>
        /// 根据给定的时间序列作为采样点对原始Sequential列表重新筛选，但对结果列表每项的time字段不去和timeline对齐
        /// 筛选后的结果列表数量和采样序点列timeline的数量一致。
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">必须是按时间递增的序列</param>
        /// <param name="timeline">时间采样点序列</param>
        /// <returns></returns>
        public static List<T> Resample<T>(IList<T> src, TimeLine timeline) where T : Sequential
        {
            if (src == null || timeline == null)
                return null;
            var timelineInMillis = timeline.Millis;
            int n = timelineInMillis.Count;
            var res = new T[n];
            int i, j = 0;
            int sign = 1;
            //将src筛选,到res
            for (i = 0; i < n; i++)
            {
                for (; j < src.Count && (int)src[j].time.TimeOfDay.TotalMilliseconds <= timelineInMillis[i]; j++) ;
                //  if (j < src.Count && (int)src[j].time.TimeOfDay.TotalMilliseconds <= timelineInMillis[i]) j++;
                if (j > 0)
                {
                    if (sign == 1 && i != 0) // 前面有为null的值
                    {
                        for (int jj = 0; jj < i; jj++)
                            res[jj] = src[j - 1];
                        sign = 0;
                    }

                    res[i] = src[j - 1];

                }


            }

            return res.ToList();
        }

    }
}
