using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromWind.Option
{
    public class OptionInfoFromWindRepository : DataFromWindRepository<OptionInfo>
    {
        public override List<OptionInfo> readFromWind(string code, DateTime dateStart, DateTime dateEnd, string tag = null, IDictionary<string, object> options = null)
        {
            return readFromWind("510050.SH", "sse");
        }

        protected List<OptionInfo> readFromWind(string underlying = "510050.SH", string market = "sse")
        {
            DateTime timeOf50ETFDividend2016 = new DateTime(2016, 11, 29);//2016年50ETF分红时间
            double standardContractMultiplier = 10000;
            string marketStr = "";
            if (market == "sse")
            {
                marketStr = ".SH";
            }
            WindAPI wapi = Platforms.GetWindAPI();
            if (Caches.WindConnection==false)
            {
                return null;
            }
            WindData wd = wapi.wset("optioncontractbasicinfo", "exchange=" + market + ";windcode=" + underlying + ";status=all");
            int len = wd.codeList.Length;
            int fieldLen = wd.fieldList.Length;
            List<OptionInfo> items = new List<OptionInfo>(len);
            object[] dm = (object[])wd.data;
            for (int k = 0; k < len; k++)
            {
                items.Add(new OptionInfo
                {
                    optionCode = (string)dm[k * fieldLen + 0] + marketStr,
                    optionName = (string)dm[k * fieldLen + 1],
                    executeType = (string)dm[k * fieldLen + 5],
                    strike = (double)dm[k * fieldLen + 6],
                    contractMultiplier = (double)dm[k * fieldLen + 7],
                    optionType = (string)dm[k * fieldLen + 4],
                    startDate = (DateTime)dm[k * fieldLen + 9],
                    endDate = (DateTime)dm[k * fieldLen + 10]
                });
            }
            for (int i = 0; i < items.Count(); i++)
            {
                var item = items[i];
                if (item.startDate < timeOf50ETFDividend2016 && item.endDate >= timeOf50ETFDividend2016)
                {
                    item.modifiedDate = timeOf50ETFDividend2016;
                    item.strikeBeforeModified = Math.Round(item.strike * item.contractMultiplier / standardContractMultiplier, 2);
                }
                else
                {
                    item.strikeBeforeModified = item.strike;
                }
                items[i] = item;
            }
            return items;
        }
    }
}
