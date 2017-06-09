using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Futures;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Futures;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Futures;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Futures
{
    class FuturesDailyDataService : SequentialByYearService<FuturesDaily>
    {
        public override List<FuturesDaily> readFromLocalCSVOnly(String path)
        {
            return Platforms.container.Resolve<FuturesDailyFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        public override List<FuturesDaily> readFromWindOnly(string code, DateTime date1, DateTime date2, string tag = null, IDictionary<string, object> options = null)
        {
            return Platforms.container.Resolve<FuturesDailyFromWindRepository>().readFromWind(code, date1, date2, tag, options);
        }

        public override void saveToLocalCSV(string path, IList<FuturesDaily> data, bool appendMode = false)
        {
            Platforms.container.Resolve<FuturesDailyToLocalCSVRepository>().saveToLocalCsv(path, data, appendMode);
        }

        public override List<FuturesDaily> readFromMSSQLOnly(string code, DateTime dateStart, DateTime dateEnd, string tag = null, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }
    }
}
