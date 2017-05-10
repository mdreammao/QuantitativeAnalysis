
using NLog;
using QuantitativeAnalysis.ServiceLayer.Core;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.Common
{
    public abstract class BasicDataRepository<T> where T : new()
    {
        const string PATH_KEY = "CacheData.Path.Basic";
        static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 由DataTable中的行向实体类的转换函数的默认实现。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual T toEntityFromCsv(DataRow row)
        {
            return DataTableUtils.CreateItemFromRow<T>(row);
        }

        public virtual DataColumn[] toCsvColumnsFromEntity(Type t)
        {
            return DataTableUtils.toColumnsDefaultFunc(t);
        }

        public virtual object[] toCsvRowValuesFromEntity(T t)
        {
            return DataTableUtils.toRowValuesDefaultFunc<T>(t);
        }

        /// <summary>
        ///  尝试从本地csv文件获取数据,可能会抛出异常
        /// </summary>
        /// <returns></returns>
        protected List<T> readFromLocalCsv(string path)
        {
            DataTable dt = CsvFileUtils.ReadFromCsvFile(path);
            if (dt == null) return null;
            //T tmp2 = toEntityFromCsv(dt.Rows[0]);
            //List<T> tmp= dt.AsEnumerable().Select(toEntityFromCsv).ToList();
            return dt.AsEnumerable().Select(toEntityFromCsv).ToList();
        }

        /// <summary>
        ///  尝试从Wind获取数据,可能会抛出异常
        /// </summary>
        /// <returns></returns>
        protected abstract List<T> readFromWind();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appendMode">是否为append模式，否则为new模式</param>
        /// <param name="localCsvExpiration">CacheData中本地csv文件的保鲜期（天数）</param>
        /// <param name="tag"></param>
        public List<T> fetchFromLocalCsvOrWindAndSaveAndCache(int localCsvExpiration, bool appendMode = false, String tag = null)
        {

            if (tag == null) tag = typeof(T).Name;
            List<T> data = null;
            var filePathPattern = _buildCacheDataFilePath(tag, "*");
            var todayFilePath = _buildCacheDataFilePath(tag, DateTime.Now.ToString("yyyyMMdd"));
            var dirPath = Path.GetDirectoryName(filePathPattern);
            var fileNamePattern = Path.GetFileName(filePathPattern);
            var allFilePaths = Directory.EnumerateFiles(dirPath, fileNamePattern)
                .OrderByDescending(fn => fn).ToList();

            var lastestFilePath = (allFilePaths == null || allFilePaths.Count == 0) ? null : allFilePaths[0];
            var daysdiff = FileUtils.GetCacheDataFileDaysPastTillToday(lastestFilePath);
            if (daysdiff > localCsvExpiration)
            {   //CacheData太旧，需要远程更新，然后保存到本地CacheData目录
                var txt = (daysdiff == int.MaxValue) ? "不存在" : "已过期" + daysdiff + "天";
                log.Info("本地csv文件{0}，尝试Wind读取新数据...", txt);
                try
                {
                    data = readFromWind();
                }
                catch (Exception e)
                {
                    log.Error(e, "从Wind读取数据失败！");

                }

                log.Info("正在保存新数据到本地...");
                try
                {
                    if (lastestFilePath == null)
                    {   //新增                        
                        saveToLocalCsvFile(data, todayFilePath, appendMode, tag);
                        log.Debug("文件{0}已保存.", todayFilePath);
                    }
                    else
                    {   //修改
                        saveToLocalCsvFile(data, lastestFilePath, appendMode, tag);
                        //重命名为最新日期
                        File.Move(lastestFilePath, todayFilePath);
                        log.Debug("文件重命名为{0}", todayFilePath);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

            }
            else
            {   //CacheData不是太旧，直接读取
                log.Info("正在从本地csv文件{0}读取数据... ", lastestFilePath);
                try
                {

                    data = readFromLocalCsv(lastestFilePath);
                }
                catch (Exception e)
                {
                    log.Error(e, "从本地csv文件读取数据失败！");
                }

            }
            if (data != null)
            {
                //加载到内存缓存
                Caches.put(tag, data);
                log.Info("已将{0}加载到内存缓存.", tag);
                log.Info("获取{0}数据列表成功.共{1}行.", tag, data.Count);
            }
            else
            {
                log.Warn("没有任何内容可以缓存！");
            }

            return data;
        }

        public List<T> fetchFromLocalCsvOnly(int localCsvExpiration, bool appendMode = false, String tag = null)
        {
            if (tag == null) tag = typeof(T).Name;
            List<T> data = null;
            var filePathPattern = _buildCacheDataFilePath(tag, "*");
            var todayFilePath = _buildCacheDataFilePath(tag, DateTime.Now.ToString("yyyyMMdd"));
            var dirPath = Path.GetDirectoryName(filePathPattern);
            var fileNamePattern = Path.GetFileName(filePathPattern);
            var allFilePaths = Directory.EnumerateFiles(dirPath, fileNamePattern)
                .OrderByDescending(fn => fn).ToList();

            var lastestFilePath = (allFilePaths == null || allFilePaths.Count == 0) ? null : allFilePaths[0];
            var daysdiff = FileUtils.GetCacheDataFileDaysPastTillToday(lastestFilePath);
            //连接不上万德只能从本地CSV读取
            log.Info("正在从本地csv文件{0}读取数据... ", lastestFilePath);
            try
            {

                data = readFromLocalCsv(lastestFilePath);
            }
            catch (Exception e)
            {
                log.Error(e, "从本地csv文件读取数据失败！");
            }



            if (data != null)
            {
                //加载到内存缓存
                Caches.put(tag, data);
                log.Info("已将{0}加载到内存缓存.", tag);
                log.Info("获取{0}数据列表成功.共{1}行.", tag, data.Count);
            }
            else
            {
                log.Warn("没有任何内容可以缓存！");
            }

            return data;
        }



        /// <summary>
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

        private static string _buildCacheDataFilePath(string tag, string date)
        {
            if (tag == null) tag = typeof(T).ToString();
            return FileUtils.GetCacheDataFilePath(PATH_KEY, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{date}"] = date
            });
        }


    }
}
