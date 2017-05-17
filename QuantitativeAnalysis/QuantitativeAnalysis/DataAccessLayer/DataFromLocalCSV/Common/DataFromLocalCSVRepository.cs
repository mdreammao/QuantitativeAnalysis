using NLog;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Common
{
    public abstract class DataFromLocalCSVRepository<T> where T : new()
    {
        Logger log = LogManager.GetCurrentClassLogger();
        //public abstract List<T> readFromLocalCSV(String path);
        public virtual List<T> readFromLocalCSV(string path)
        {
            if (path == null || !File.Exists(path))
            {
                log.Debug("未找到文件{0}！", path);
                return null;
            }
            DataTable dt = CsvFileUtils.ReadFromCsvFile(path);
            if (dt == null) return null;
            return dt.AsEnumerable().Select(toEntityFromCsv).ToList();
        }
        public virtual T toEntityFromCsv(DataRow row)
        {
            return DataTableUtils.CreateItemFromRow<T>(row);
        }
    }
}
