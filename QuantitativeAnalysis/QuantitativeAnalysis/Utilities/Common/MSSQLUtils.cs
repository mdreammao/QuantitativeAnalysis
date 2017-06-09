using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace QuantitativeAnalysis.Utilities.Common
{
    public class MSSQLUtils
    {
        static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 新建数据库函数。
        /// </summary>
        /// <param name="dataBaseName">需新建的数据库名称</param>
        /// <param name="connectString">连接字符串</param>
        public static void CreateDataBase(string dataBaseName, string connectString,string commandText=null)
        {
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();//打开数据库  
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =commandText==null? "CREATE DATABASE " + dataBaseName + " ON PRIMARY (NAME = '" + dataBaseName + "', FILENAME = 'D:\\HFDB\\" + dataBaseName + ".dbf',SIZE = 1024MB,MaxSize = 512000MB,FileGrowth = 1024MB) LOG ON (NAME = '" + dataBaseName + "Log',FileName = 'D:\\HFDB\\" + dataBaseName + ".ldf',Size = 20MB,MaxSize = 1024MB,FileGrowth = 10MB)":commandText;
                try
                {
                    cmd.ExecuteReader();
                }
                catch (Exception myerror)
                {
                    System.Console.WriteLine(myerror.Message);
                }
            }
        }

        /// <summary>
        /// 新建数据表。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="connectString">连接字符串</param>
        public static void CreateTable(string tableName, string connectString,string commmandText=null)
        {
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();//打开数据库  
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =commmandText==null? "CREATE TABLE [dbo].[" + tableName + "]([code] [char](11) NOT NULL,[tdate] [int] NOT NULL," +
                    "[ttime] [int] NOT NULL,[lastPrice] [decimal](12,3) NULL,[ask1] [decimal](12,3) NULL,[ask2] [decimal](12,3) NULL," +
                    "[ask3] [decimal](12,3) NULL,[ask4] [decimal](12,3) NULL,[ask5] [decimal](12,3) NULL,[bid1] [decimal](12,3) NULL," +
                    "[bid2] [decimal](12,3) NULL,[bid3] [decimal](12,3) NULL,[bid4] [decimal](12,3) NULL,[bid5] [decimal](12,3) NULL," +
                    "[askv1] [decimal](10, 0) NULL,[askv2] [decimal](10, 0) NULL,[askv3] [decimal](10, 0) NULL,[askv4] [decimal](10, 0) NULL," +
                    "[askv5] [decimal](10, 0) NULL,[bidv1] [decimal](10, 0) NULL,[bidv2] [decimal](10, 0) NULL,[bidv3] [decimal](10, 0) NULL," +
                    "[bidv4] [decimal](10, 0) NULL,[bidv5] [decimal](10, 0) NULL,[volume] [decimal](20, 0) NULL,[amount] [decimal](20, 3) NULL," +
                    "[openInterest] [decimal](20, 0) NULL,[preClose] [decimal](12,3) NULL,[preSettle] [decimal](12,3) NULL,CONSTRAINT[PK_" + tableName + "] " +
                    "PRIMARY KEY NONCLUSTERED([code] ASC,[tdate] ASC,[ttime] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON [PRIMARY] CREATE CLUSTERED " +
                    "INDEX[IX_" + tableName + "_TDATE] ON[dbo].[" + tableName + "]([tdate] ASC,[ttime] ASC)WITH(PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]":commmandText;
                try
                {
                    cmd.ExecuteReader();
                }
                catch (Exception myerror)
                {
                    System.Console.WriteLine(myerror.Message);
                }
            }
        }

        /// <summary>
        /// 获取app.config里配置的ConnectionString
        /// 如果以“server=”开头，则原样返回
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string connectionName)
        {
            if (connectionName.StartsWith("server="))
                return connectionName;
            return ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }
        
        /// <summary>
        /// 根据app.config里配置的连接名称打开连接
        /// </summary>
        /// <param name="connName">app.config里配置的连接名称</param>
        /// <returns>打开的连接</returns>
        public static SqlConnection openConnectionByConfigName(string connName)
        {
            string connStr = GetConnectionString(connName);
            return new SqlConnection(connStr);
        }

        /// <summary>
        /// 通过一次性的连接，执行查询，返回DataTable对象
        /// </summary>
        /// <param name="connStr">sql连接字符串，代表一个数据源</param>
        /// <param name="sql">sql语句</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTable(string connStr, string sql)
        {
            return GetTable(connStr, sql, null);
        }

        /// <summary>
        /// 通过一次性的连接，执行查询，返回DataTable对象
        /// </summary>
        /// <param name="connStr">sql连接字符串，代表一个数据源</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTable(string connStr, string sql, SqlParameter[] paramArr)
        {
            return GetTable(connStr, sql, paramArr, CommandType.Text);
        }

        /// <summary>
        /// 通过一次性的连接，执行查询，返回DataTable对象
        /// </summary>
        /// <param name="connStr">sql连接字符串，代表一个数据源</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <param name="cmdType">Command类型</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTable(string connStr, string sql, SqlParameter[] paramArr, CommandType cmdType)
        {
            return GetTable(connStr, sql, paramArr, cmdType, 0);
        }

        /// <summary>
        /// 通过一次性的连接，执行查询，返回DataTable对象
        /// </summary>
        /// <param name="connStr">sql连接字符串，代表一个数据源</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <param name="cmdTimeout"></param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTable(string connStr, string sql, SqlParameter[] paramArr, int cmdTimeout)
        {
            return GetTable(connStr, sql, paramArr, CommandType.Text, cmdTimeout);
        }

        /// <summary>
        /// 通过一次性的连接，执行查询，返回DataTable对象
        /// </summary>
        /// <param name="connStr">sql连接字符串，代表一个数据源</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <param name="cmdType">Command类型</param>
        /// <param name="cmdTimeout"></param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTable(string connStr, string sql, SqlParameter[] paramArr, CommandType cmdType, int cmdTimeout)
        {
            DataTable dt = new DataTable();
            log.Debug("Executing SQL Query: conn={0},SQL=[\n{1}\n]", connStr, sql);
            if (paramArr != null)
                log.Debug("params={0}", paramArr.ToString());
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.SelectCommand.CommandType = cmdType;
                    if (cmdTimeout > 0) da.SelectCommand.CommandTimeout = cmdTimeout;
                    if (paramArr != null)
                    {
                        da.SelectCommand.Parameters.AddRange(paramArr);
                    }
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return dt;
        }

        public static DataTable GetTable(SqlConnection conn, string sql, SqlParameter[] paramArr = null, CommandType commandType = CommandType.Text)
        {
            DataTable dt = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.CommandType = commandType;
            if (paramArr != null)
            {
                da.SelectCommand.Parameters.AddRange(paramArr);
            }
            da.Fill(dt);

            return dt;
        }

        public static DataSet GetDataSet(string connStr, string sql)
        {
            return GetDataSet(connStr, sql, null);
        }

        public static DataSet GetDataSet(string connStr, string sql, SqlParameter[] paramArr)
        {
            return GetDataSet(connStr, sql, paramArr, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="connStr">sql连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <param name="commandType">Command类型</param>
        /// <returns>DataSet对象</returns>
        public static DataSet GetDataSet(string connStr, string sql, SqlParameter[] paramArr, CommandType commandType)
        {
            DataSet dt = new DataSet();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.CommandType = commandType;
                if (paramArr != null)
                {
                    da.SelectCommand.Parameters.AddRange(paramArr);
                }
                da.Fill(dt);
            }
            return dt;
        }

        public static int ExcuteProc(string connStr, string procName)
        {
            return ExcuteSQL(connStr, procName, null, CommandType.StoredProcedure);
        }

        public static int ExcuteProc(string connStr, string procName, SqlParameter[] paras)
        {
            return ExcuteSQL(connStr, procName, paras, CommandType.StoredProcedure);
        }

        public static int ExcuteSQL(string connStr, string sql)
        {
            return ExcuteSQL(connStr, sql, null);
        }

        public static int ExcuteSQL(string connStr, string sql, SqlParameter[] paras)
        {
            return ExcuteSQL(connStr, sql, paras, CommandType.Text);
        }

        /// <summary>
        /// 执行非查询存储过程和SQL语句
        /// 增、删、改
        /// </summary>
        /// <param name="connStr">sql连接字符串</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <param name="cmdType">Command类型</param>
        /// <returns>返回影响行数</returns>
        public static int ExcuteSQL(string connStr, string sql, SqlParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = cmdType;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                i = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return i;

        }

        public static int ExcuteScalarSQL(string connStr, string sql)
        {
            return ExcuteScalarSQL(connStr, sql, null);
        }

        public static int ExcuteScalarSQL(string connStr, string sql, SqlParameter[] paras)
        {
            return ExcuteScalarSQL(connStr, sql, paras, CommandType.Text);
        }

        public static int ExcuteScalarProc(string connStr, string sql, SqlParameter[] paras)
        {
            return ExcuteScalarSQL(connStr, sql, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行SQL语句，返回第一行，第一列
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <param name="cmdType"></param>
        /// <returns>返回影响行数</returns>
        public static int ExcuteScalarSQL(string connStr, string sql, SqlParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = cmdType;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                i = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
            return i;

        }

        /// <summary>
        /// 调用不带参数的存储过程获取单个值
        /// </summary>
        /// <param name="ProcName"></param>
        /// <returns></returns>
        public static object GetObjectByProc(string connStr, string ProcName)
        {
            return GetObjectByProc(connStr, ProcName, null);
        }

        /// <summary>
        /// 调用带参数的存储过程获取单个值
        /// </summary>
        /// <param name="ProcName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static object GetObjectByProc(string connStr, string ProcName, SqlParameter[] paras)
        {
            return GetObject(connStr, ProcName, paras, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 根据sql语句获取单个值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object GetObject(string connStr, string sql)
        {
            return GetObject(connStr, sql, null);
        }

        /// <summary>
        /// 根据sql语句 和 参数数组获取单个值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static object GetObject(string connStr, string sql, SqlParameter[] paras)
        {
            return GetObject(connStr, sql, paras, CommandType.Text);
        }

        /// <summary>
        /// 执行SQL语句，返回首行首列
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>返回的首行首列</returns>
        public static object GetObject(string connStr, string sql, SqlParameter[] paras, CommandType cmdtype)
        {
            object o = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = cmdtype;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);

                }

                conn.Open();
                o = cmd.ExecuteScalar();
                conn.Close();
            }
            return o;

        }

        /// <summary>
        /// 调用不带参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderByProc(string connStr, string procName)
        {
            return GetReaderByProc(connStr, procName, null);
        }

        /// <summary>
        /// 调用带有参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderByProc(string connStr, string procName, SqlParameter[] paras)
        {
            return GetReader(connStr, procName, paras, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 根据sql语句返回DataReader对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReader(string connStr, string sql)
        {
            return GetReader(connStr, sql, null);
        }

        /// <summary>
        /// 根据sql语句和参数返回DataReader对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReader(string connStr, string sql, SqlParameter[] paras)
        {
            return GetReader(connStr, sql, paras, CommandType.Text);
        }

        /// <summary>
        /// 查询SQL语句获取DataReader
        /// </summary>
        /// <param name="sql">查询的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>查询到的DataReader（关闭该对象的时候，自动关闭连接）</returns>
        public static SqlDataReader GetReader(string connStr, string sql, SqlParameter[] paras, CommandType cmdtype)
        {
            SqlDataReader sqldr = null;
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = cmdtype;
            if (paras != null)
            {
                cmd.Parameters.AddRange(paras);
            }
            conn.Open();
            //CommandBehavior.CloseConnection的作用是如果关联的DataReader对象关闭，则连接自动关闭
            sqldr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return sqldr;
        }

        /// <summary>
        /// 往数据库中批量插入数据
        /// </summary>
        /// <param name="sourceDt">数据源表</param>
        /// <param name="targetTable">服务器上目标表</param>
        public static void BulkToDB(string connStr, DataTable sourceDt, string targetTable)
        {
            SqlConnection conn = new SqlConnection(connStr);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);   //用其它源的数据有效批量加载sql server表中
            bulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称
            bulkCopy.BatchSize = sourceDt.Rows.Count;   //每一批次中的行数
            bulkCopy.ColumnMappings.Add("ttime", "ttime");
            bulkCopy.ColumnMappings.Add("tdate", "tdate");
            bulkCopy.ColumnMappings.Add("code", "code");
            try
            {
                conn.Open();
                if (sourceDt != null && sourceDt.Rows.Count != 0)
                    bulkCopy.WriteToServer(sourceDt);   //将提供的数据源中的所有行复制到目标表中
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }

        }

    }
}
