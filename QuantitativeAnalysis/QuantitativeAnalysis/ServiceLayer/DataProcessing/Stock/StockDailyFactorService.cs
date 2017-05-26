using NLog;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.Utilities.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    public abstract class StockDailyFactorService<T> where T : Factor, new() 
    {
        /// <summary>
        /// 保存日志记录的变量
        /// </summary>
        static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   仅从wind读取数据 数据,可能会抛出异常
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="tag">标签</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        protected abstract List<T> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null);

        /// <summary>
        ///  尝试从本地csv文件获取数据,可能会抛出异常
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        protected abstract List<T> readFromLocalCSVOnly(string code, DateTime startDate, DateTime endDate, string tag = null);

        /// <summary>
        /// 将从其他数据源读到的数据保存到本地CSV文件
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="code">代码</param>
        /// <param name="appendMode">添加模式</param>
        /// <param name="canSaveToday">是否需保存今日数据</param>
        protected abstract void saveToLocalCSV(IList<T> data, string tag = null, bool appendMode = false, bool canSaveToday = false);

        /// <summary>
        /// 尝试从Wind获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        virtual public List<T> fetchFromWind(string code, DateTime startDate, DateTime endDate,string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);
            if (Caches.WindConnection == false && Caches.WindConnectionTry==true)
            {
                log.Error("无法连接Wind,无法从Wind获取失败！");
                return result;
            }
            //尝试从Wind获取
            //根据股票的上市退市日期来调整获取数据的日期
            startDate = startDate > StockBasicInfoUtils.getStockListDate(code) ? startDate : StockBasicInfoUtils.getStockListDate(code);
            endDate = endDate > StockBasicInfoUtils.getStockDelistDate(code) ? StockBasicInfoUtils.getStockDelistDate(code) : endDate;
            log.Debug("尝试从Wind获取{0}...", code);
            try
            {
                result = readFromWindOnly(code, startDate,endDate, null, null);
            }
            catch (Exception e)
            {
                log.Error(e, "尝试从Wind获取失败！");
                //debug 输出失败信息
                Console.WriteLine("尝试从wind读取数据失败！品种{0},时间{1}至{2}", code, startDate.ToShortDateString(),endDate.ToShortDateString());
            }
            logInfo(code, startDate,endDate,tag, result);
            return result;
        }

        /// <summary>
        /// 尝试从本地csv文件，获取数据。
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        virtual public List<T> fetchFromLocalCsv(string code, DateTime startDate, DateTime endDate,string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            log.Debug("正在获取{0}数据列表{1}...", Kit.ToShortName(tag), code);
            //尝试从csv获取
            log.Debug("尝试从csv获取{0}...", code);
            try
            {
                result = readFromLocalCSVOnly(code, startDate,endDate,tag);
            }
            catch (Exception e)
            {
                log.Error(e, "尝试从csv获取失败！");
                //debug 输出失败信息
                Console.WriteLine("尝试从本地csv失败！品种{0},时间{1}至{2}", code, startDate.ToShortDateString(),endDate.ToShortDateString());
            }
            logInfo(code, startDate,endDate, tag, result);
            return result;
        }

        /// <summary>
        /// 尝试Wind获取数据。然后将数据覆盖保存到CacheData文件夹
        /// </summary>
        /// <param name="code">代码，如股票代码，期权代码</param>
        /// <param name="date">指定的日期</param>
        /// <param name="tag">读写文件路径前缀，若为空默认为类名</param>
        /// <returns></returns>
        virtual public List<T> fetchFromWindAndSaveToLocalCSV(string code, DateTime startDate,DateTime endDate, string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;

            result = fetchFromWind(code,startDate,endDate);

            if (result != null && result.Count() > 0)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result);
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
        virtual public List<T> fetchFromLocalCsvOrWindAndSave(string code, DateTime startDate,DateTime endDate,string tag = null)
        {
            if (tag == null) tag = typeof(T).ToString();
            List<T> result = null;
            bool csvHasData = false;
            result = fetchFromLocalCsv(code, startDate,endDate,tag);
            if (result != null) csvHasData = true;
            if (result == null && Caches.WindConnection == false && Caches.WindConnectionTry==true)
            {
                log.Error("本地无CSV数据并且wind无法连接，故无法获得数据！");
                return result;
            }

            if (result == null) //数据不完整，必须去万德获取数据
            {
                result = fetchFromWind(code, startDate, endDate);
            }
            if (!csvHasData && result != null && result.Count() > 0)
            {
                //如果数据不是从csv获取的，可保存至本地，存为csv文件
                log.Debug("正在保存到本地csv文件...");
                saveToLocalCSV(result);
            }
            return result;
        }

        /// <summary>
        /// 生成获取数据日志文件的函数
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="date">日期</param>
        /// <param name="tag">标签</param>
        /// <param name="result">数据</param>
        private void logInfo(string code, DateTime startDate, DateTime endDate,string tag, List<T> result)
        {
            if (result != null && result.Count() > 0)
            {
                log.Info("获取{3}数据{0}至{4}(date={1})成功.共{2}行.", Kit.ToShortName(tag), startDate.ToShortDateString(), result.Count, code,endDate.ToShortDateString());
            }
            else
            {
                log.Info("获取{2}数据{0}至{4}(date={1})失败.无有效数据.", Kit.ToShortName(tag), startDate.ToShortDateString(), code,endDate.ToShortDateString());
            }
        }
    }
}
