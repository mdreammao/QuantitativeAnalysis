
using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.DataApplication
{

    public static class KLineDataUtils
    {
        /// <summary>
        /// //查缺补漏函数：如果某天数据为NAN，那么复制上一天的数据
        /// 特殊情况是：如果第一条数据没有，则复制（最近的）下一条的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orignalList"></param>
        /// <returns></returns>
        public static List<T> leakFilling<T>(List<T> orignalList) where T : KLine, new()
        {
            for (int i = 0; i < orignalList.Count(); i++)
            {
                var KLine0 = orignalList[i];

                #region 处理最开始第1个时间点没有数据的情况（此时要从后面的复制到前面）
                if (i == 0 && double.IsNaN(KLine0.close) == true)
                {
                    for (int j = 1; j < orignalList.Count(); j++)
                    {
                        if (double.IsNaN(orignalList[j].close) != true)
                        {
                            orignalList[0] = orignalList[j];
                        }
                    }
                    continue;
                }
                #endregion
                if (double.IsNaN(KLine0.close) == true)
                {
                    orignalList[i] = orignalList[i - 1];
                }
            }
            return orignalList;
        }
    }
}
