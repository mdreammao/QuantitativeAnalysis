using NLog;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common
{
    public abstract class DataToLocalCSVRepository<T>
    {
        Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 将数据以csv文件的形式保存到CacheData文件夹下的预定路径
        /// </summary>
        /// <param name="data">要保存的数据</param>
        /// <param name="path">读写文件路径</param>
        /// <param name="appendMode">是否为追加的文件尾部模式，否则是覆盖模式</param>
        public virtual void saveToLocalCsv(string path, IList<T> data, bool appendMode = false)
        {
            if (data == null)
            {
                log.Error("没有任何内容可以保存到csv！");
                return;
            }
            var dt = DataTableUtils.ToDataTable(data);
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
