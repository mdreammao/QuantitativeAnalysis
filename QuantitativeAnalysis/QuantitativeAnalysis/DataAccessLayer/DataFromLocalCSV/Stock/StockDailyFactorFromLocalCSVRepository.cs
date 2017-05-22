using NLog;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Common;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Stock
{
    public abstract class StockDailyFactorFromLocalCSVRepository<T> : DataFromLocalCSVRepository<T> where T : Factor, new()
    {

        public List<T> ReadFromLocalCSVForDays(string code, DateTime startDate, DateTime endDate)
        {
            List<T> list = new List<T>();
            var tradeDays = DateUtils.GetTradeDays(startDate, endDate);
            for (int i = 0; i < tradeDays.Count(); i++)
            {
                var today = tradeDays[i];
                var path = _buildCacheDataFilePath(code, today);
                var list0 = readFromLocalCSV(path);
                if (list0.Count!=0)
                {
                    list.AddRange(list0);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取对应保存路径
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="date">日期</param>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        private string _buildCacheDataFilePath(string code, DateTime date, string tag=null)
        {
            if (tag == null) tag = typeof(T).ToString();
            string[] tagArr = tag.Split('.');
            string tagLast = tagArr[tagArr.Length - 1];
            return FileUtils.GetCacheDataFilePath("CacheData.Path.StockFactor", new Dictionary<string, string>
            {
                ["{tag}"] = tagLast,
                ["{code}"] = code,
                ["{date}"] = date.ToString("yyyyMMdd")
            });
        }
    }
}
