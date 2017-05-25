using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Stock;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.stock;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Stock;
using QuantitativeAnalysis.ModelLayer.Stock.BasicInfo;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    public class StockBasicInfoService : BasicDataService<StockBasicInfo>
    {
        public override List<StockBasicInfo> readFromLocalCsv(string path)
        {
            return Platforms.container.Resolve<StockBasicInfoFromLocalCsvRepository>().readFromLocalCSV(path);
        }

        public override List<StockBasicInfo> readFromWind(string code, DateTime startDate, DateTime endDate)
        {
            return Platforms.container.Resolve<StockBasicInfoFromWindRepository>().readFromWind(DateTime.Today);
        }

        public override void saveToLocalCsvFile(IList<StockBasicInfo> data, string path, bool appendMode = false, string tag = null)
        {
            Platforms.container.Resolve<StockBasicInfoToLocalCSVRepository>().saveToLocalCsv(path,data);
        }
    }
}
