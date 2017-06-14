using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
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
        public override List<OptionInfo> readFromWind(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            return readFromWindOnlyByDate(code.ToUpper(), startDate, endDate);
        }

        public List<OptionInfo> read50ETFOptionFromWind(string tag = null, IDictionary<string, object> options = null)
        {

            return readFromWindOnly50ETFOption("510050.SH", "sse");
        }

        protected List<OptionInfo> readFromWindOnlyByDate(string underlying,DateTime startDate,DateTime endDate)
        {
            if (Caches.WindConnection == false)
            {
                return null;
            }
            underlying = underlying.ToUpper();
            var tradeDays = DateUtils.GetTradeDays(startDate, endDate);
            WindAPI wapi = Platforms.GetWindAPI();
            List<OptionInfo> items = new List<OptionInfo>();
            Dictionary<string, OptionInfo> list = new Dictionary<string, OptionInfo>();
            foreach (var date in tradeDays)
            {
                string dateStr = date.ToString("yyyy-MM-dd");
                WindData wd = wapi.wset("optionchain", "date=" + dateStr + ";us_code=" + underlying + ";option_var=全部;call_put=全部");
                object[] dm = (object[])wd.data;
                int len = wd.codeList.Length;
                int fieldLen = wd.fieldList.Length;
                for (int k = 0; k < len; k++)
                {
                    OptionInfo myInfo = new OptionInfo
                    {
                        optionCode = (string)dm[k * fieldLen + 2],
                        optionName = (string)dm[k * fieldLen + 4],
                        executeType = (string)dm[k * fieldLen + 5],
                        strike = (double)dm[k * fieldLen + 6],
                        contractMultiplier = (double)dm[k * fieldLen + 13],
                        optionType = (string)dm[k * fieldLen + 8],
                        startDate = (DateTime)dm[k * fieldLen + 9],
                        endDate = (DateTime)dm[k * fieldLen + 10]
                    };
                    if (list.ContainsKey(myInfo.optionName)==false)
                    {
                        list.Add(myInfo.optionName, myInfo);
                        items.Add(myInfo);
                    }
                }
            }
            return items;
        }

        protected List<OptionInfo> readFromWindOnly50ETFOption(string underlying = "510050.SH", string market = "sse")
        {
            DateTime timeOf50ETFDividend2016 = new DateTime(2016, 11, 29);//2016年50ETF分红时间
            double standardContractMultiplier = 10000;
            string marketStr = "";
            if (market == "sse")
            {
                marketStr = ".SH";
            }
            if (Caches.WindConnection == false)
            {
                return null;
            }
            WindAPI wapi = Platforms.GetWindAPI();
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
