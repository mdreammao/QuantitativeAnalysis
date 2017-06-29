using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
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
                //if (code == "000059.SZ" && k == 2049)
                //{
                //    var mycode = code;
                //    var mytime = timeList[k];
                //    var mypreClose = (double)Kit.DBNullToZero(dataList[k * fieldLen + 0]);
                //    Console.WriteLine(Kit.DBNullToZero(dataList[k * fieldLen + 1]).GetType());
                //    Console.WriteLine(Convert.ToDouble(dataList[k * fieldLen + 1]));
                //    var myopen = (double)Kit.DBNullToZero(dataList[k * fieldLen + 1]);
                //    var myhigh = (double)dataList[k * fieldLen + 2];
                //    var mylow = (double)dataList[k * fieldLen + 3];
                //    var myclose = (double)dataList[k * fieldLen + 4];
                //    var myvolume = (double)dataList[k * fieldLen + 5];
                //    var myamount = (double)dataList[k * fieldLen + 6];
                //    var mydealnum = dataList[k * fieldLen + 7] is DBNull ? 0 : (double)dataList[k * fieldLen + 7];
                //    var myupsAndDowns = (double)dataList[k * fieldLen + 8];
                //    var mypercentUpsAndDowns = (double)dataList[k * fieldLen + 9];
                //    var myswing = (double)dataList[k * fieldLen + 10];
                //    var myvwap = dataList[k * fieldLen + 11] is DBNull ? 0 : (double)dataList[k * fieldLen + 11];
                //    var myadjfactor = (double)dataList[k * fieldLen + 12];
                //    var myturn = (double)dataList[k * fieldLen + 13];
                //    var myfree_turn = (double)dataList[k * fieldLen + 14];
                //    var mytrade_status = dataList[k * fieldLen + 15] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 15]);
                //    var mysusp_reason = dataList[k * fieldLen + 16] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 16]);
                //    var mysusp_days = dataList[k * fieldLen + 17] is DBNull ? 0 : (int)dataList[k * fieldLen + 17];
                //    var mymaxUpOrDown = (int)dataList[k * fieldLen + 18];
                //}
                items.Add(new StockDailyMarket
                {
                    code = code,
                    time = timeList[k],
                    preClose = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 0])),
                    open = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 1])),
                    high = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 2])),
                    low = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 3])),
                    close = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 4])),
                    volume = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 5])),
                    amount = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 6])),
                    dealnum = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 7])),
                    upsAndDowns = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 8])),
                    percentUpsAndDowns = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 9])),
                    swing = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 10])),
                    vwap = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 11])),
                    adjfactor = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 12])),
                    turn = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 13])),
                    free_turn = Convert.ToDouble(Kit.DBNullToZero(dataList[k * fieldLen + 14])),
                    trade_status = dataList[k * fieldLen + 15] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 15]),
                    susp_reason = dataList[k * fieldLen + 16] is DBNull ? string.Empty : Convert.ToString(dataList[k * fieldLen + 16]),
                    susp_days= Convert.ToInt32(Kit.DBNullToZero(dataList[k * fieldLen + 17])),
                    maxUpOrDown = Convert.ToInt32(Kit.DBNullToZero(dataList[k * fieldLen + 18]))
                });
               // Console.Write("{0}  ", k);
            }
            return items;
        }
    }
}
