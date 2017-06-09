using NLog;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Common
{
    public abstract class DataFromMSSQLRepository<T> where T:TickFromMssql,new ()
    {
        Logger log = LogManager.GetCurrentClassLogger();
        public virtual List<T> readFromMSSQLDaily(string code, DateTime date,string connName,string SqlString)
        {
            string yyyyMM = date.ToString("yyyyMM");
            string yyyyMMdd = date.ToString("yyyyMMdd");
            string codeStr = code.Replace('.', '_');
            string connStr = MSSQLUtils.GetConnectionString(connName);
            DataTable dt = MSSQLUtils.GetTable(connStr, SqlString);
            return dt.AsEnumerable().Select(
                row => new T
                {
                    code = Convert.ToString(row["stkcd"]),
                    time = Kit.ToDateTime(row["tdate"], row["ttime"]),
                    tdate = Kit.ToInt(row["tdate"]),
                    ttime = Kit.ToInt(row["ttime"]),
                    lastPrice = Kit.ToDouble(row["cp"]),
                    preClose = Kit.ToDouble(row["PRECLOSE"]),
                    preSettle = Kit.ToDouble(row["PrevSettle"]),
                    openInterest=Kit.ToDouble(row["OpenInterest"]),
                    volume = Kit.ToDouble(row["ts"]),
                    amount = Kit.ToDouble(row["tt"]),
                    ask1 = Kit.ToDouble(row["S1"]),
                    ask2 = Kit.ToDouble(row["S2"]),
                    ask3 = Kit.ToDouble(row["S3"]),
                    ask4 = Kit.ToDouble(row["S4"]),
                    ask5 = Kit.ToDouble(row["S5"]),
                    askv1 = Kit.ToDouble(row["SV1"]),
                    askv2 = Kit.ToDouble(row["SV2"]),
                    askv3 = Kit.ToDouble(row["SV3"]),
                    askv4 = Kit.ToDouble(row["SV4"]),
                    askv5 = Kit.ToDouble(row["SV5"]),
                    bid1 = Kit.ToDouble(row["B1"]),
                    bid2 = Kit.ToDouble(row["B2"]),
                    bid3 = Kit.ToDouble(row["B3"]),
                    bid4 = Kit.ToDouble(row["B4"]),
                    bid5 = Kit.ToDouble(row["B5"]),
                    bidv1 = Kit.ToDouble(row["BV1"]),
                    bidv2 = Kit.ToDouble(row["BV2"]),
                    bidv3 = Kit.ToDouble(row["BV3"]),
                    bidv4 = Kit.ToDouble(row["BV4"]),
                    bidv5 = Kit.ToDouble(row["BV5"])
                }).ToList();
        }

        public virtual T toEntityFromCsv(DataRow row)
        {
            return DataTableUtils.CreateItemFromRow<T>(row);
        }
    }
}
