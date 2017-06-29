using NLog;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DateToMSSQL.Common
{
    public abstract class DataToMSSQLRepository
    {
        Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 将数据存储到SQLSERVER的函数
        /// </summary>
        /// <param name="targetServer">目标服务器</param>
        /// <param name="dataBase">数据库</param>
        /// <param name="tableName">表</param>
        /// <param name="data">数据</param>
        /// <param name="pair">datatable和数据库表结构的配对</param>
        public virtual void saveToSQLServer(string targetServer,string dataBase,string tableName,DataTable data,Dictionary<string,string> pair=null)
        {
            string connStr = MSSQLUtils.GetConnectionString(targetServer) + "database=" + dataBase + ";";
            MSSQLUtils.dataBulkToMSSQL(connStr, data, tableName,pair);
        }

    }
}
