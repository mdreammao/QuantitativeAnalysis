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
            if (Caches.WindConnection == false && Caches.WindConnectionTry == true)
            {
                return null;
            }
            WindAPI w = Platforms.GetWindAPI();
            WindData wd = w.wsd(code, "pre_close,open,high,low,close,volume,amt,dealnum,chg,pct_chg,swing,vwap,adjfactor,turn,free_turn,trade_status,susp_reason,susp_days,maxupordown", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), "");
            int len = wd.timeList.Length;
            int fieldLen = wd.fieldList.Length;
            var items = new List<StockDailyMarket>(len * fieldLen);
            DateTime[] timeList = wd.timeList;
            object[] dataList = (object[])wd.data;
            for (int k = 0; k < len; k++)
            {
                items.Add(new StockDailyMarket
                {
                    code = code,
                    time = timeList[k],
                    preClose = (double)dataList[k * fieldLen + 0],
                    open = (double)dataList[k * fieldLen + 1],
                    high = (double)dataList[k * fieldLen + 2],
                    low = (double)dataList[k * fieldLen + 3],
                    close = (double)dataList[k * fieldLen + 4],
                    volume = (double)dataList[k * fieldLen + 5],
                    amount = (double)dataList[k * fieldLen + 6],
                    dealnum = dataList[k * fieldLen + 7] is DBNull ? 0 : (double)dataList[k * fieldLen + 7],
                    upsAndDowns = (double)dataList[k * fieldLen + 8],
                    percentUpsAndDowns = (double)dataList[k * fieldLen + 9],
                    swing = (double)dataList[k * fieldLen + 10],
                    vwap = dataList[k * fieldLen + 11] is DBNull ? 0 : (double)dataList[k * fieldLen +11],
                    adjfactor = (double)dataList[k * fieldLen + 12],
                    turn = (double)dataList[k * fieldLen + 13],
                    free_turn = (double)dataList[k * fieldLen + 14],
                    trade_status = dataList[k * fieldLen + 15] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 15]),
                    susp_reason = dataList[k * fieldLen + 16] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 16]),
                    susp_days= dataList[k * fieldLen + 17] is DBNull ? 0 : (int)dataList[k * fieldLen + 17],
                    maxUpOrDown = (int)dataList[k * fieldLen + 18]
                });
            }
            return items;
        }
    }
}
