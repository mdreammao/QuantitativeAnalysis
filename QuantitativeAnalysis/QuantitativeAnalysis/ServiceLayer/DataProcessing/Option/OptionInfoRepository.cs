using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Option;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Option;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Option;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionInfoRepository : BasicDataService<OptionInfo>
    {
        public override List<OptionInfo> readFromLocalCsv(string path)
        {
            return Platforms.container.Resolve<OptionInfoFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        public override List<OptionInfo> readFromWind()
        {
            return Platforms.container.Resolve<OptionInfoFromWindRepository>().readFromWind(null,new DateTime(),new DateTime(),null,null);
        }

        public override void saveToLocalCsvFile(IList<OptionInfo> data, string path, bool appendMode = false, string tag = null)
        {
            Platforms.container.Resolve<OptionInfoToLocalCSVRepository>().saveToLocalCsvFile(data,path,appendMode,tag);
        }
    }
}
