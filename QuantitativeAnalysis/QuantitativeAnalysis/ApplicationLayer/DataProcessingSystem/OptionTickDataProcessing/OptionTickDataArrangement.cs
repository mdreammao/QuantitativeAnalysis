using Autofac;
using NLog;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ModelLayer.Stock.Tick;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Option;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.Utilities.DataApplication;
using QuantitativeAnalysis.Utilities.OptionUtils_50ETF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ApplicationLayer.DataProcessingSystem.OptionTickDataProcessing
{
    public class OptionTickDataArrangement
    {
        private DateTime startDate, endDate;
        private List<DateTime> tradeDays = new List<DateTime>();
        private List<OptionInfo> optionInfoList = new List<OptionInfo>();
        private string targetServer;
        private string dataBase;
        static Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        public OptionTickDataArrangement(int startDate,int endDate,string targetServer="local",string dataBase="TickData_50ETFOption")
        {
            this.startDate = Kit.ToDate(startDate);
            this.endDate = Kit.ToDate(endDate);
            this.tradeDays = DateUtils.GetTradeDays(startDate, endDate);
            this.optionInfoList = (List<OptionInfo>)Caches.get("OptionInfo_510050.SH");
            this.targetServer = targetServer;
            this.dataBase = dataBase;
            //save50ETFOptionData();
            saveStockData("510050.SH");
        }

        private void save50ETFOptionData()
        {
            //给定model和数据库表的配对
            Dictionary<string, string> pair = new Dictionary<string, string>();
            var propertyList = MyReflection.getPropertyName((new OptionTickFromMssql()).GetType());
            foreach (var name in propertyList)
            {
                if (name != "time")
                {
                    pair.Add(name, name);
                }
            }
            //逐日进行检查
            foreach (var date in tradeDays)
            {
                //记录50ETF期权数据
                var list = OptionUtils_50ETF.getOptionListByDate(optionInfoList,Kit.ToInt(date));
                //逐合约进行检查
                foreach (var option in list)
                {
                    //先检查目标数据库中存在的数据
                    string tableName = "MarketData_"+option.optionCode.Replace('.', '_');
                    int numbers = checkTargetDataTable(date, tableName);
                    if (numbers <= 1000)
                    {
                        log.Warn("date:{0} code:{1} numbers:{2}.", date.ToString("yyyyMMdd"), option.optionCode, numbers);
                    }
                    if (numbers == 0)
                    {
                        Platforms.container.Resolve<OptionTickDataDailyStoringService>().fetchDataFromSQLandModifiedandSaveToSQL(option.optionCode, date, "corp170", targetServer, dataBase, tableName, pair);
                    }
                }
                


                Console.WriteLine("Date:{0} Complete!", date.ToString("yyyyMMdd"));
            }
        }

        private void saveStockData(string code)
        {
            //给定model和数据库表的配对
            Dictionary<string, string> pair = new Dictionary<string, string>();
            var propertyList = MyReflection.getPropertyName((new StockTickFromMssql()).GetType());
            foreach (var name in propertyList)
            {
                if (name!="time")
                {
                    pair.Add(name, name);
                }
            }
            //逐日进行检查
            foreach (var date in tradeDays)
            {
                //先检查目标数据库中存在的数据
                string tableName = "MarketData_" + code.Replace('.', '_');
                int numbers = checkTargetDataTable(date, tableName);
                if (numbers<=1000)
                {
                    log.Warn("date:{0} code:{1} numbers:{2}.", date.ToString("yyyyMMdd"), code, numbers);
                }
                if (numbers==0)
                {
                    Platforms.container.Resolve<StockTickDataDailyStoringService>().fetchDataFromSQLandModifiedandSaveToSQL(code, date, "corp217", targetServer, dataBase, tableName, pair);
                }
                Console.WriteLine("Date:{0} Complete!", date.ToString("yyyyMMdd"));
            }
        }
        private int checkTargetDataTable(DateTime date,string tableName)
        {

            int number = 0;
            string connStr = MSSQLUtils.GetConnectionString(targetServer);
            try
            {
                //数据库不存在
                if (MSSQLUtils.CheckDataBaseExist(dataBase, connStr) == false)
                {
                    MSSQLUtils.CreateDataBase(connStr, getCreateDataBaseString(dataBase));
                    string connTableStr = connStr + "database=" + dataBase + ";";
                    MSSQLUtils.CreateTable(connTableStr, getCreateTableString(tableName));
                }
                //表不存在
                else if (MSSQLUtils.CheckExist(dataBase, tableName, connStr) == false)
                {
                    string connTableStr = connStr + "database=" + dataBase + ";";
                    MSSQLUtils.CreateTable(connTableStr, getCreateTableString(tableName));
                }
                //表存在判断数据是否存在
                else
                {
                    string connTableStr = connStr + "database=" + dataBase + ";";
                    string sqlStr = string.Format(@"select count(*) from {0} where tdate={1}", tableName, date.ToString("yyyyMMdd"));
                    number = MSSQLUtils.getNumbers(tableName, connTableStr, sqlStr);
                }
            }
            catch (Exception e)
            {

                log.Error(e, "无法从数据库中读取数据！");
            }
            finally
            {
                
            }
            return number;
        }

        private string getCreateDataBaseString(string dataBaseName)
        {
            return "CREATE DATABASE " + dataBaseName + " ON PRIMARY (NAME = '" + dataBaseName + "', FILENAME = 'D:\\HFDB\\" + dataBaseName + ".dbf',SIZE = 1024MB,MaxSize = 512000MB,FileGrowth = 1024MB) LOG ON (NAME = '" + dataBaseName + "Log',FileName = 'D:\\HFDB\\" + dataBaseName + ".ldf',Size = 20MB,MaxSize = 1024MB,FileGrowth = 10MB)";
        }

        private string getCreateTableString(string tableName)
        {
            return "CREATE TABLE [dbo].[" + tableName + "]([code] [char](11) NOT NULL,[tdate] [int] NOT NULL," +
                    "[ttime] [int] NOT NULL,[lastPrice] [decimal](12,4) NULL,[ask1] [decimal](12,4) NULL,[ask2] [decimal](12,4) NULL," +
                    "[ask3] [decimal](12,4) NULL,[ask4] [decimal](12,4) NULL,[ask5] [decimal](12,4) NULL,[bid1] [decimal](12,4) NULL," +
                    "[bid2] [decimal](12,4) NULL,[bid3] [decimal](12,4) NULL,[bid4] [decimal](12,4) NULL,[bid5] [decimal](12,4) NULL," +
                    "[askv1] [decimal](10, 0) NULL,[askv2] [decimal](10, 0) NULL,[askv3] [decimal](10, 0) NULL,[askv4] [decimal](10, 0) NULL," +
                    "[askv5] [decimal](10, 0) NULL,[bidv1] [decimal](10, 0) NULL,[bidv2] [decimal](10, 0) NULL,[bidv3] [decimal](10, 0) NULL," +
                    "[bidv4] [decimal](10, 0) NULL,[bidv5] [decimal](10, 0) NULL,[volume] [decimal](20, 0) NULL,[amount] [decimal](20, 3) NULL," +
                    "[openInterest] [decimal](20, 0) NULL,[preClose] [decimal](12,4) NULL,[preSettle] [decimal](12,4) NULL,CONSTRAINT[PK_" + tableName + "] " +
                    "PRIMARY KEY NONCLUSTERED([code] ASC,[tdate] ASC,[ttime] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON [PRIMARY] CREATE CLUSTERED " +
                    "INDEX[IX_" + tableName + "_TDATE] ON[dbo].[" + tableName + "]([tdate] ASC,[ttime] ASC)WITH(PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]";
        }
    }
}
