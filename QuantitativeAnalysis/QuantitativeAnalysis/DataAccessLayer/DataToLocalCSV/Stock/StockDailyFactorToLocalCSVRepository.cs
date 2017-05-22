using NLog;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Stock
{
    public abstract class StockDailyFactorToLocalCSVRepository<T> : DataToLocalCSVRepository<T> where T : Factor, new()
    {
        Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 将数据以csv文件的形式保存到CacheData文件夹下的预定路径
        /// </summary>
        /// <param name="data">要保存的数据</param>
        /// <param name="appendMode">是否为追加的文件尾部模式，否则是覆盖模式</param>
        public virtual void saveToLocalCsv(IList<T> data, bool appendMode = false)
        {
            if (data == null)
            {
                log.Error("没有任何内容可以保存到csv！");
                return;
            }
            foreach (var item in data)
            {
                var dt = DataTableUtils.ToDataTableOneRowOnly(item);
                string path = _buildCacheDataFilePath(item.code, item.time, null);
               // if (File.Exists(path)==false)
                {
                    try
                    {
                        var s = (File.Exists(path)) ? "覆盖" : "新增";
                        CsvFileUtils.WriteToCsvFile(path, dt, appendMode);
                        log.Debug("文件已{0}：{1}. 共{2}行数据.", s, path, data.Count);
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "保存到本地csv文件失败！({0})", path);
                    }
                }
                
            }
        }


        /// <summary>
        /// 获取对应保存路径
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="date">日期</param>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        private static string _buildCacheDataFilePath(string code, DateTime date, string tag)
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
