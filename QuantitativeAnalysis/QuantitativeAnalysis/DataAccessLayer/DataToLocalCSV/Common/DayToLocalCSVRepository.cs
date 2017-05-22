using NLog;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common
{
    public abstract class DayToLocalCSVRepository<T> : DataToLocalCSVRepository<T> where T : Sequential, new()
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 将数据以csv文件的形式保存到CacheData文件夹下的预定路径。
        /// 默认不可以保存今天的数据，因为可能数据不全。
        /// </summary>
        /// <param name="data">要保存的数据</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <param name="appendMode">是否为追加的文件尾部模式，否则是覆盖模式</param>\
        /// <param name="canSaveToday">是否可以保存今天的数据，默认不可以</param>
        public virtual void saveToLocalCsv(IList<T> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            if (!canSaveToday && date.Date >= DateTime.Now.Date)
            {
                log.Debug("今天的{0}数据不保存，请改天保存。", Kit.ToShortName(tag));
            }
            var path = _buildCacheDataFilePath(code, date, tag);
            saveToLocalCsv(path, data, appendMode);
        }

        private static string _buildCacheDataFilePath(string code, DateTime date, string tag)
        {
            if (tag == null) tag = typeof(T).ToString();
            return FileUtils.GetCacheDataFilePath("CacheData.Path.SequentialByDay", new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{code}"] = code,
                ["{date}"] = date.ToString("yyyyMMdd")
            });
        }
    }
}
