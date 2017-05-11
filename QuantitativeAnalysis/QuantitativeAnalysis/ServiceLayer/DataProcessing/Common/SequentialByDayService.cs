
using Autofac;
using NLog;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Common;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Common
{
    /// <summary>
    /// 按每天存取时间序列数据的Repository
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SequentialByDayService<T>  where T : Sequential, new()
    {
        const string PATH_KEY = "CacheData.Path.SequentialByDay";
        static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  尝试从Wind
        ///  数据,可能会抛出异常
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        //protected abstract List<T> readFromWind(string code, DateTime date);
        public abstract List<T> getWindResult(string code, DateTime dateStart, DateTime dateEnd, string tag = null, IDictionary<string, object> options = null);

        /// <summary>
        /// 尝试从默认MSSQL源获取数据,可能会抛出异常
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected abstract List<T> readFromDefaultMssql(string code, DateTime date);

        /// <summary>
        ///  尝试从本地csv文件获取数据,可能会抛出异常
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public abstract List<T> getLocalCSVResult(string code, DateTime date, string tag = null);

        public abstract void saveToLocalCSV(IList<T> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false);
        
        /// <summary>
        /// 尝试从本地csv文件，Wind获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromLocalCsv(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);

            //尝试从csv获取
            log.Debug("尝试从csv获取{0}...", code);
            try
            {
                result = getLocalCSVResult(code, date, tag);
            }
            catch (Exception e)
            {
                log.Error(e, "尝试从csv获取失败！");
                //debug 输出失败信息
                Console.WriteLine("尝试从本地csv失败！品种{0},时间{1}", code, date.ToShortDateString());
            }

            logInfo(code, date, tag, result);
            return result;
        }

        /// <summary>
        /// 尝试从Wind获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromWind(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);
            
            //尝试从Wind获取
            log.Debug("尝试从Wind获取{0}...", code);
            try
            {
                result = getWindResult(code, date, new DateTime(), null, null);
            }
            catch (Exception e)
            {
                log.Error(e, "尝试从Wind获取失败！");
                //debug 输出失败信息
                Console.WriteLine("尝试从本地csv失败！品种{0},时间{1}", code, date.ToShortDateString());
            }

            logInfo(code, date, tag, result);
            return result;
        }

        /// <summary>
        /// 尝试从默认MSSQL源获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromMssql(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);

            try
            {
                //尝试从默认MSSQL源获取
                log.Debug("尝试从默认MSSQL源获取{0}...", code);
                result = readFromDefaultMssql(code, date);
            }
            catch (Exception e)
            {
                log.Error(e, "尝试从默认MSSQL源获取失败！");
            }

            logInfo(code, date, tag, result);
            return result;
        }

        /// <summary>
        /// 先后尝试从本地csv文件，Wind获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromLocalCsvOrWind(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;

            result = fetchFromLocalCsv(code, date, tag);

            if (result == null)
            {
                result = fetchFromWind(code, date, tag);
            }

            return result;
        }

        /// <summary>
        /// 先后尝试从本地csv文件，Wind获取数据。若无本地csv，则保存到CacheData文件夹。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromLocalCsvOrWindAndSave(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            bool csvHasData = false;

            result = fetchFromLocalCsv(code, date, tag);
            if (result != null) csvHasData = true;
            
            if (result == null)
            {
                result = fetchFromWind(code, date, tag);
            }
            if (!csvHasData && result != null && result.Count() > 0)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result, code, date, tag);
            }
            return result;
        }
        /// <summary>
        /// 先后尝试从本地csv文件，默认MSSQL数据库源获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromLocalCsvOrMssql(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;

            result = fetchFromLocalCsv(code, date, tag);

            if (result == null)
            {
                result = fetchFromMssql(code, date, tag);
            }

            return result;
        }

        /// <summary>
        /// 先后尝试从本地csv文件，默认MSSQL数据库源获取数据。若无本地csv，则保存到CacheData文件夹。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromLocalCsvOrMssqlAndSave(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            bool csvHasData = false;

            result = fetchFromLocalCsv(code, date, tag);
            if (result != null) csvHasData = true;
            
            if (result == null)
            {
                result = fetchFromMssql(code, date, tag);
            }

            if (!csvHasData && result != null && result.Count() > 0)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result, code, date, tag);
            }

            return result;
        }

        /// <summary>
        /// 尝试Wind获取数据。然后将数据覆盖保存到CacheData文件夹
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        public List<T> fetchFromWindAndSave(string code, DateTime date, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;

            result = fetchFromWind(code, date, tag);

            if (result != null && result.Count() > 0)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result, code, date, tag);
            }

            return result;
        }

        /// <summary>
        /// 多种途径获取/储存数据（根据参数选择方式）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <param name="tag"></param>
        /// <param name="tryCsv"></param>
        /// <param name="tryWind"></param>
        /// <param name="tryMssql0"></param>
        /// <param name="saveToCsv"></param>
        /// <returns></returns>

        private void logInfo(string code, DateTime date, string tag, List<T> result)
        {
            if (result != null && result.Count() > 0)
            {
                log.Info("获取{3}数据{0}(date={1})成功.共{2}行.", Kit.ToShortName(tag), date, result.Count, code);
            }
            else
            {
                log.Info("获取{2}数据{0}(date={1})失败.无有效数据.", Kit.ToShortName(tag), date, code);
            }
        }

        /*private List<T> fetch0(string code, DateTime date, string tag, bool tryCsv, bool tryWind, bool tryMssql0, bool saveToCsv)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            bool csvHasData = false;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);
            if (tryCsv)
            {
                //尝试从csv获取
                log.Debug("尝试从csv获取{0}...", code);
                try
                {
                    //result = getLocalCsvResult(code, date, tag);
                    result = getLocalCSVResult(code, date, tag);
                }
                catch (Exception e)
                {
                    log.Error(e, "尝试从csv获取失败！");
                    //debug 输出失败信息
                    Console.WriteLine("尝试从本地csv失败！品种{0},时间{1}", code, date.ToShortDateString());
                }
                if (result != null) csvHasData = true;
            }
            if (result == null && tryWind)
            {
                //尝试从Wind获取
                log.Debug("尝试从Wind获取{0}...", code);
                try
                {
                    result = getWindResult(code, date,new DateTime(),null,null);
                }
                catch (Exception e)
                {
                    log.Error(e, "尝试从Wind获取失败！");
                    //debug 输出失败信息
                    Console.WriteLine("尝试从本地csv失败！品种{0},时间{1}", code, date.ToShortDateString());
                }
            }
            if (result == null && tryMssql0)
            {
                try
                {
                    //尝试从默认MSSQL源获取
                    log.Debug("尝试从默认MSSQL源获取{0}...", code);
                    result = readFromDefaultMssql(code, date);
                }
                catch (Exception e)
                {
                    log.Error(e, "尝试从默认MSSQL源获取失败！");
                }

            }
            if (!csvHasData && result != null && result.Count() > 0 && saveToCsv)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result, code, date, tag);
            }
            if (result != null && result.Count() > 0)
            {
                log.Info("获取{3}数据{0}(date={1})成功.共{2}行.", Kit.ToShortName(tag), date, result.Count, code);
            }
            else
            {
                log.Info("获取{2}数据{0}(date={1})失败.无有效数据.", Kit.ToShortName(tag), date, code);
            }
            return result;
        }*/

    }
}
