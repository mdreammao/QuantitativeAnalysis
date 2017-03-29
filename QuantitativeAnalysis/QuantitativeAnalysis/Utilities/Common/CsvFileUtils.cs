using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    public static class CsvFileUtils
    {

        static Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        ///  DataTable -> CSV.
        ///  如果是appendMode，会覆盖旧文件全部内容，否则在旧文件尾部新增内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filePath"></param>
        public static void WriteToCsvFile(string filePath, DataTable dt, bool appendMode = false)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            if (!appendMode)    //如果是appendMode则不生成存储列名的首行
                sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray
                    .Select(toReadableString).Select(toDoubleQuotedString);
                sb.AppendLine(string.Join(",", fields));
            }

            var dirPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            //若文件路径不存在则生成该文件夹
            if (dirPath != "" && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (appendMode)
                File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
            else
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        static string toReadableString(object cell)
        {
            if (cell is IList)
            {
                return ((IList)cell).Cast<object>()
                    .Aggregate((i, j) => String.Concat(i, ";", j))
                    .ToString();
            }
            if (cell is DateTime)
            {
                return ((DateTime)cell).ToString("yyyy/MM/dd HH:mm:ss fff");
            }
            return cell.ToString();
        }

        static string toDoubleQuotedString(string src)
        {
            return string.Concat("\"", src.Replace("\"", "\"\""), "\"");
        }

        static string toNonDoubleQuotedString(string src)
        {
            int len = src.Length;
            if (src[0] == '\"' && src[len - 1] == '\"')
                return src.Substring(1, len - 2);
            else
                return src;
        }


        /// <summary>
        /// http://stackoverflow.com/a/27705485
        /// CSV -> DataTable 简单的csv读取处理，不支持含逗号的内容。
        /// 若文件不存在则返回null
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="firstRowAsHeader">csv文件第一行是否作为header</param>
        /// <returns></returns>
        public static DataTable ReadFromCsvFile(string filePath, bool firstRowAsHeader = true, string columns = "")
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        if (!sr.EndOfStream && firstRowAsHeader)
                        {
                            string[] headers = sr.ReadLine().Split(',');
                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }
                        }

                        if (!sr.EndOfStream && firstRowAsHeader == false && columns != "")
                        {
                            string[] headers = columns.Split(',');
                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }
                        }

                        while (!sr.EndOfStream)
                        {
                            string[] rows = sr.ReadLine().Split(',').Select(toNonDoubleQuotedString).ToArray();
                            dt.Rows.Add(rows);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return null;
            }
            return dt;
        }



        /// <summary>
        /// 将values转换为csv文件的一行，包含一些默认的类型转换，例如：
        /// toCsvFileLine("a",2.1,DateTime1)=="\"a\",\"2.1\",\"20160804093200\"";
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [Obsolete]
        public static string toCsvFileLine(params object[] values)
        {
            if (values == null) return "";
            var res = new StringBuilder();
            foreach (var val in values)
            {
                var s = "";
                if (val is DateTime)
                    s = ((DateTime)val).ToString("yyyyMMddHHmmss");
                s = val.ToString();
                res.Append(s);
            }
            return res.ToString();
        }
    }



}
