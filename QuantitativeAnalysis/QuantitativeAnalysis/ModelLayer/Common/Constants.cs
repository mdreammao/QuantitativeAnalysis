using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    /// <summary>
    /// 存储全局变量
    /// </summary>
    public static class Constants
    {

        //常规分钟级别的时间序列
        public static TimeLine timelineOneMinute = new TimeLine(
                new TimeLineSection("09:30:00.000", "11:29:00.000", 60000),
                new TimeLineSection("13:00:00.000", "14:59:00.000", 60000)
                );

        //常规TICK级别的时间序列
        public static TimeLine timeline500ms = new TimeLine(
                new TimeLineSection("09:30:00.000", "11:30:00.000", 500),
                new TimeLineSection("13:00:00.000", "15:00:00.000", 500)
                );
        //A.DCE Tick级别的时间序列
        public static TimeLine timelineA_DCE500ms = new TimeLine(
                new TimeLineSection("09:00:00.000", "11:30:00.000", 500),
                new TimeLineSection("13:30:00.000", "15:00:00.000", 500),
                new TimeLineSection("21:00:00.000", "23:30:00.000", 500)
                );
        /// <summary>
        /// RB.SHF分钟级别的时间序列
        /// </summary>
        public static TimeLine timelineRB_SHFOneMinute = new TimeLine(
                new TimeLineSection("09:00:00.000", "11:29:00.000", 60000),
                new TimeLineSection("13:30:00.000", "14:59:00.000", 60000),
                new TimeLineSection("21:00:00.000", "22:59:00.000", 60000)
                );



        public static DateTime TRADE_DAY_START = new DateTime(2007, 1, 1, 0, 0, 0);
        public static DateTime TRADE_DAY_END = new DateTime(2016, 12, 31, 23, 59, 59);
    }
}
