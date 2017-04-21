using NLog;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
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
        public virtual DataColumn[] toCsvColumnsFromEntity(Type t)
        {
            return DataTableUtils.toColumnsDefaultFunc(t);
        }

        public virtual object[] toCsvRowValuesFromEntity(T t)
        {
            return DataTableUtils.toRowValuesDefaultFunc<T>(t);
        }

        /// 将数据以csv文件的形式保存到CacheData文件夹下的预定路径
        /// </summary>
        public void saveToLocalCsvFile(IList<T> data, string path, bool appendMode = false, string tag = null)
        {
            if (tag == null) tag = typeof(T).Name;
            if (data == null || data.Count == 0)
            {
                log.Warn("没有任何内容可以保存到csv！");
                return;
            }
            var dt = DataTableUtils.ToDataTable(data, toCsvColumnsFromEntity, toCsvRowValuesFromEntity);
            try
            {
                var s = (File.Exists(path)) ? "覆盖" : "新增";
                CsvFileUtils.WriteToCsvFile(path, dt);
                log.Info("文件已{0}：{1} ", s, path);
            }
            catch (Exception e)
            {
                log.Error(e, "保存到本地csv文件失败！");

            }

        }
    }
}
