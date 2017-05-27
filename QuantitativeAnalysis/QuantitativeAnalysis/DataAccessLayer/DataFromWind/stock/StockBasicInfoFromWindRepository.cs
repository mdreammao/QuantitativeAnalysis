using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Stock.BasicInfo;
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
    public class StockBasicInfoFromWindRepository : DataFromWindRepository<StockBasicInfo>
    {
        

        public List<StockBasicInfo> readFromWind(DateTime date,string tag=null, List<string> existCode=null,List<StockBasicInfo> preList=null,IDictionary<string, object> options = null)
        {
            if (Caches.WindConnection == false && Caches.WindConnectionTry == true)
            {
                return null;
            }
            date = DateUtils.PreviousOrCurrentTradeDay(date);//获取最近的交易日
            WindAPI w = Platforms.GetWindAPI();
            WindData  delist= w.wset("sectorconstituent", "date="+date.ToString("yyyy-MM-dd")+";sectorid=a001010m00000000");
            WindData list = w.wset("sectorconstituent", "date=" + date.ToString("yyyy-MM-dd") + ";sectorid=a001010100000000");
            List<string> codeList = new List<string>();
            int len = delist.codeList.Length;
            int fieldLen = delist.fieldList.Length;
            object[] dataList = (object[])delist.data;
            for (int k = 0; k < len; k++)
            {
                var code = dataList[k * fieldLen + 1].ToString();
                if ((existCode!=null && existCode.Contains(code)==true) || existCode==null)
                {
                    codeList.Add(code);
                }
            }
            len = list.codeList.Length;
            fieldLen = list.fieldList.Length;
            dataList = (object[])list.data;
            for (int k = 0; k < len; k++)
            {
                var code = dataList[k * fieldLen + 1].ToString();
                if ((existCode!=null && existCode.Contains(code)==false) || existCode==null)
                {
                    codeList.Add(code);
                }
            }
            codeList.Sort();
            List<StockBasicInfo> items = new List<StockBasicInfo>();
            WindData wd = new WindData();
            foreach (var code in codeList)
            {
                wd = w.wsd(code, "sec_name,ipo_date,delist_date", date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd"), "");
                dataList = (object[])wd.data;
                items.Add(new StockBasicInfo
                {
                    code=code,
                    name = dataList[0].ToString(),
                    listDate = (DateTime)dataList[1],
                    delistDate = dataList[2] is DBNull ? new DateTime(2099, 12, 31) : (DateTime)dataList[2]
                });
            }
            if (preList!=null)
            {
                foreach (var stock in preList)
                {
                    if (items.Find(x => x.code == stock.code) == null)
                    {
                        items.Add(stock);
                    }
                }
            }
            return items.OrderBy(x=>x.code).ToList();
        }

        public override List<StockBasicInfo> readFromWind(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }
    }
}
