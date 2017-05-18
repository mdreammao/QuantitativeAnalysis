using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromWind.stock
{
    public class StockDailyMarketFromWindRepository : DataFromWindRepository<StockDailyMarket>
    {
        public override List<StockDailyMarket> readFromWind(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            if (Caches.WindConnection==false && Caches.WindConnectionTry==true)
            {
                return null;
            }
            WindAPI w = Platforms.GetWindAPI();
            WindData wd = w.wsd(code, "pre_close,open,high,low,close,volume,amt,dealnum," +
                "chg,pct_chg,swing,vwap,adjfactor,turn,free_turn,trade_status,susp_reason,maxupordown", startDate, endDate, "");
            int len = wd.timeList.Length;
            int fieldLen = wd.fieldList.Length;

            var items = new List<StockDailyMarket>(len * fieldLen);
            if (wd.data is double[])
            {
                double[] dataList = (double[])wd.data;
                DateTime[] timeList = wd.timeList;
                for (int k = 0; k < len; k++)
                {
                    items.Add(new StockDailyMarket
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
