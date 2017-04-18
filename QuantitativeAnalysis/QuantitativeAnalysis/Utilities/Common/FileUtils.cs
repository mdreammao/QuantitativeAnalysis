using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    public static class FileUtils
    {
        /// <summary>
        /// 获取该app的根路径，目前没找到比较完美的办法
        /// </summary>
        /// <returns></returns>
        public static string GetAppRootPath()
        {
            if (_appRootPath != null) return _appRootPath;
            //首次调用该方法时会计算_appRootPath
            _appRootPath = System.Environment.CurrentDirectory;
            if (_appRootPath.EndsWith("bin\\Debug")) _appRootPath = _appRootPath.Substring(0, _appRootPath.Length - 10);
            return _appRootPath;
        }
        private static string _appRootPath = null;


        public static string GetCacheDataDirPath()
        {
            return ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["CacheData.RootPath"];
        }
        /// <summary>
        /// 根据key获取路径配置，列出所有匹配的文件路径，按文件名倒序排列
        /// </summary>
        /// <param name="appKey">app.config中的key</param>
        /// <returns></returns>
        public static List<string> GetCacheDataFilePaths(string appKey)
        {
            var path = FileUtils.GetCacheDataFilePath(appKey);
            var dirPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            var fileName_ = fileName.Replace("{0}", "*").Replace("{date}", "*").Replace("{type}", "*").Replace("{code}", "*").Replace("{name}", "*").Replace("{tag}", "*").Replace("{time}", "*");
            return Directory.EnumerateFiles(dirPath, fileName_)
                .OrderByDescending(fn => fn).ToList();
        }

        public static string GetCacheDataFilePathThatLatest(string appKey)
        {
            var list = GetCacheDataFilePaths(appKey);
            return (list != null && list.Count > 0) ? list[0] : null;
        }

        /// <summary>
        /// 根据给定的key和app.config生成CacheData文件路径,包含当前日期后缀
        /// </summary>
        /// <param name="key">例如"CacheData.Path.OptionInfo"</param>
        /// <returns>例如TradeDays_20160803.txt</returns>
        public static string GetCacheDataFilePath(string key, DateTime timestamp)
        {
            return ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["CacheData.RootPath"]
                + ConfigurationManager.AppSettings[key].Replace("{0}", timestamp.ToString("yyyyMMdd"));
        }

        public static string GetCacheDataFilePath(string key)
        {
            return
                ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["CacheData.RootPath"]
               + ConfigurationManager.AppSettings[key];
        }

        public static string GetCacheDataFilePath(string appKey, Dictionary<string, string> paramsMap)
        {
            var path = FileUtils.GetCacheDataFilePath(appKey);
            foreach (var key in paramsMap.Keys)
            {
                path = path.Replace(key, paramsMap[key]);
            }
            return path;

        }

        [Obsolete]
        public static string GetCacheDataFilePath(string appKey, string tag, string date = "*")
        {
            return GetCacheDataFilePath(appKey, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{date}"] = date
            });
        }
        [Obsolete]
        public static string GetCacheDataFilePath(string appKey, string tag, string code, string date)
        {
            return GetCacheDataFilePath(appKey, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{code}"] = code,
                ["{date}"] = date
            });
        }

        public static DateTime GetCacheDataFileTimestamp(string filePath)
        {
            int x1 = filePath.LastIndexOf('_');
            int x2 = filePath.LastIndexOf('.');
            string timeStr = filePath.Substring(x1 + 1, x2 - x1 - 1);
            return Kit.ToDate(timeStr);
        }

        public static void DeleteOldCacheDataFile(string appKey)
        {
            var filePaths = FileUtils.GetCacheDataFilePaths(appKey);
            foreach (var fpath in filePaths)
            {
                File.Delete(fpath);
            }

        }


        /// <summary>
        /// 计算出CacheDataFile中的指定文件的时间戳和今天所差的天数，
        /// 返回值=今天-该文件的时间戳，如果没有找到文件则返回MaxValue
        /// </summary>      
        /// <returns></returns>
        public static int GetCacheDataFileDaysPastTillToday(string filePath)
        {
            if (filePath != null)
            {
                var timestamp = FileUtils.GetCacheDataFileTimestamp(filePath);
                return (DateTime.Now - timestamp).Days;
            }
            else
            {
                return int.MaxValue;
            }
        }

    }


}
