
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;
using System.Data;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.ServiceLayer.Core;
using MathNet.Numerics;

namespace QuantitativeAnalysis.DataAccessLayer.Common
{
    public class TradeDayRepository : BasicDataRepository<DateTime>
    {

        public override DateTime toEntityFromCsv(DataRow row)
        {
            return Kit.ToDateTime(Kit.ToInt(row[0]), 0);
        }
        public override object[] toCsvRowValuesFromEntity(DateTime t)
        {
            return new object[] { Kit.ToInt_yyyyMMdd(t) };
        }
        public override DataColumn[] toCsvColumnsFromEntity(Type t)
        {
            return new DataColumn[] { new DataColumn("trade_days", typeof(int)) };
        }

        /// <summary>
        /// 从万德API获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<DateTime> readFromWind(DateTime startTime, DateTime endTime)
        {
            WindAPI wapi = Platforms.GetWindAPI();
            WindData wd = wapi.tdays(startTime, endTime, "");
            var wdd = (object[])wd.data;
            return wdd.Select(x => (DateTime)x).ToList();

        }

        protected override List<DateTime> readFromWind()
        {
            var endDate = DateTime.Now.AddYears(1);
            return readFromWind(Constants.TRADE_DAY_START, endDate);
        }
    }
}
