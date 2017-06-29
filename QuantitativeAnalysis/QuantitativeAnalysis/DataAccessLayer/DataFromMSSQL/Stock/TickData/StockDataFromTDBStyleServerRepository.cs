using QuantitativeAnalysis.ModelLayer.Stock.Tick;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Stock
{
    class StockDataFromTDBStyleServerRepository : StockTickDataFromMSSQLRepository
    {

        private int timeModified(int time)
        {
            if (time < 2400000)
            {
                time *= 1000;
            }
            return time;
        }

        public override List<StockTickFromMssql> readFromMSSQLDaily(string connName, string SqlString)
        {
            string connStr = MSSQLUtils.GetConnectionString(connName);
            DataTable dt = MSSQLUtils.GetTable(connStr, SqlString);
            return dt.AsEnumerable().Select(
                row => new StockTickFromMssql
                {
                    code = Convert.ToString(row["stkcd"]),
                    time = Kit.ToDateTime(Kit.ToInt(row["tdate"]), timeModified(Kit.ToInt(row["ttime"]))),
                    tdate = Kit.ToInt(row["tdate"]),
                    ttime = timeModified(Kit.ToInt(row["ttime"])),
                    lastPrice = Math.Max(0, Kit.ToDouble(row["cp"])),
                    preClose = Math.Max(0, Kit.ToDouble(row["PRECLOSE"])),
                    volume = Math.Max(0, Kit.ToDouble(row["ts"])),
                    amount = Math.Max(0, Kit.ToDouble(row["tt"])),
                    ask1 = Math.Max(0, Kit.ToDouble(row["S1"])),
                    ask2 = Math.Max(0, Kit.ToDouble(row["S2"])),
                    ask3 = Math.Max(0, Kit.ToDouble(row["S3"])),
                    ask4 = Math.Max(0, Kit.ToDouble(row["S4"])),
                    ask5 = Math.Max(0, Kit.ToDouble(row["S5"])),
                    askv1 = Math.Max(0, Kit.ToDouble(row["SV1"])),
                    askv2 = Math.Max(0, Kit.ToDouble(row["SV2"])),
                    askv3 = Math.Max(0, Kit.ToDouble(row["SV3"])),
                    askv4 = Math.Max(0, Kit.ToDouble(row["SV4"])),
                    askv5 = Math.Max(0, Kit.ToDouble(row["SV5"])),
                    bid1 = Math.Max(0, Kit.ToDouble(row["B1"])),
                    bid2 = Math.Max(0, Kit.ToDouble(row["B2"])),
                    bid3 = Math.Max(0, Kit.ToDouble(row["B3"])),
                    bid4 = Math.Max(0, Kit.ToDouble(row["B4"])),
                    bid5 = Math.Max(0, Kit.ToDouble(row["B5"])),
                    bidv1 = Math.Max(0, Kit.ToDouble(row["BV1"])),
                    bidv2 = Math.Max(0, Kit.ToDouble(row["BV2"])),
                    bidv3 = Math.Max(0, Kit.ToDouble(row["BV3"])),
                    bidv4 = Math.Max(0, Kit.ToDouble(row["BV4"])),
                    bidv5 = Math.Max(0, Kit.ToDouble(row["BV5"]))
                }).ToList();
        }
    }
}
