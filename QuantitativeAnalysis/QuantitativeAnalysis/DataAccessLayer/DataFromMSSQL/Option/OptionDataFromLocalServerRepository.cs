using QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Option;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Option
{
    class OptionDataFromLocalServerRepository : OptionDataFromMSSQLRepository
    {
        public override List<OptionTickFromMssql> readFromMSSQLDaily(string code, DateTime date, string connName, string SqlString)
        {
            string yyyyMM = date.ToString("yyyyMM");
            string yyyyMMdd = date.ToString("yyyyMMdd");
            string codeStr = code.Replace('.', '_');
            string connStr = MSSQLUtils.GetConnectionString(connName);
            DataTable dt = MSSQLUtils.GetTable(connStr, SqlString);
            return dt.AsEnumerable().Select(
                    row => new OptionTickFromMssql
                    {
                        code = Convert.ToString(row["code"]),
                        time = Kit.ToDateTime(row["tdate"], row["ttime"]),
                        tdate = Kit.ToInt(row["tdate"]),
                        ttime = Kit.ToInt(row["ttime"]),
                        lastPrice = Kit.ToDouble(row["lastPrice"]),
                        preClose = Kit.ToDouble(row["preClose"]),
                        preSettle = Kit.ToDouble(row["preSettle"]),
                        openInterest = Kit.ToDouble(row["OpenInterest"]),
                        volume = Kit.ToDouble(row["volume"]),
                        amount = Kit.ToDouble(row["amount"]),
                        ask1 = Kit.ToDouble(row["ask1"]),
                        ask2 = Kit.ToDouble(row["ask2"]),
                        ask3 = Kit.ToDouble(row["ask3"]),
                        ask4 = Kit.ToDouble(row["ask4"]),
                        ask5 = Kit.ToDouble(row["ask5"]),
                        askv1 = Kit.ToDouble(row["askv1"]),
                        askv2 = Kit.ToDouble(row["askv2"]),
                        askv3 = Kit.ToDouble(row["askv3"]),
                        askv4 = Kit.ToDouble(row["askv4"]),
                        askv5 = Kit.ToDouble(row["askv5"]),
                        bid1 = Kit.ToDouble(row["bid1"]),
                        bid2 = Kit.ToDouble(row["bid2"]),
                        bid3 = Kit.ToDouble(row["bid3"]),
                        bid4 = Kit.ToDouble(row["bid4"]),
                        bid5 = Kit.ToDouble(row["bid5"]),
                        bidv1 = Kit.ToDouble(row["bidv1"]),
                        bidv2 = Kit.ToDouble(row["bidv2"]),
                        bidv3 = Kit.ToDouble(row["bidv3"]),
                        bidv4 = Kit.ToDouble(row["bidv4"]),
                        bidv5 = Kit.ToDouble(row["bidv5"])
                    }).ToList();
        }
    }
}
