using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Option;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Option;
using QuantitativeAnalysisQuantitativeAnalysis.Utilities.DataApplication;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantitativeAnalysis.Utilities.Common;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionInfoOf50ETFService : BasicDataService<OptionInfo>
    {
        protected override List<OptionInfo> readFromLocalCsv(string path)
        {
            return Platforms.container.Resolve<OptionInfoFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        protected override List<OptionInfo> readFromWind(string code,DateTime startDate,DateTime endDate)
        {
            return Platforms.container.Resolve<OptionInfoFromWindRepository>().read50ETFOptionFromWind(null,null);
        }

        protected override void saveToLocalCsvFile(IList<OptionInfo> data, string path, bool appendMode = false, string tag = null)
        {
            Platforms.container.Resolve<OptionInfoToLocalCSVRepository>().saveToLocalCsv(path,data,appendMode);
        }

        protected override void saveToSQLServer(IList<OptionInfo> data, string serviceName, string dataBaseName, string tableName, string sqlStr)
        {
            //第一步，检查目标数据库是否存在
            //第二步，检查目标表是否存在
            //第三步，从目标表中获取数据，更新新的数据
            throw new NotImplementedException();
        }
    }
}
