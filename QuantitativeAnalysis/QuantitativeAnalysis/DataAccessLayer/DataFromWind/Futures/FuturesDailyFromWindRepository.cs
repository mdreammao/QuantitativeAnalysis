using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromWind.Futures
{
    public class FuturesDailyFromWindRepository : DataFromWindRepository<FuturesDaily>
    {
        public override List<FuturesDaily> readFromWind(string code, DateTime dateStart, DateTime dateEnd, string tag = null, IDictionary<string, object> options = null)
        {
            WindAPI w = Platforms.GetWindAPI();
            WindData wd = w.wsd(code, "open,high,low,close,volume,amt", dateStart, dateEnd, "Fill=Previous");
            int len = wd.timeList.Length;
            int fieldLen = wd.fieldList.Length;

            var items = new List<FuturesDaily>(len * fieldLen);
            if (wd.data is double[])
            {
                double[] dataList = (double[])wd.data;
                DateTime[] timeList = wd.timeList;
                for (int k = 0; k < len; k++)
                {
                    items.Add(new FuturesDaily
                    {
                        time = timeList[k],
                        open = (double)dataList[k * fieldLen + 0],
                        high = (double)dataList[k * fieldLen + 1],
                        low = (double)dataList[k * fieldLen + 2],
                        close = (double)dataList[k * fieldLen + 3],
                        volume = (double)dataList[k * fieldLen + 4],
                        amount = (double)dataList[k * fieldLen + 5]
                    });
                }
            }
            return items;
        }
    }
}
