using NLog;
using QuantitativeAnalysis.ModelLayer.Stock.Tick;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Stock
{
    abstract class StockTickDataFromMSSQLRepository
    {
        Logger log = LogManager.GetCurrentClassLogger();
        public abstract List<StockTickFromMssql> readFromMSSQLDaily(string connName, string SqlString);
        
        public virtual StockTickFromMssql toEntityFromCsv(DataRow row)
        {
            return DataTableUtils.CreateItemFromRow<StockTickFromMssql>(row);
        }
    }
}
