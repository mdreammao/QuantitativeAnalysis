using Autofac;
using QuantitativeAnalysis.DataAccessLayer.Common;
using QuantitativeAnalysis.ServiceLayer.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.Common
{
    class TradeDaysService
    {
        TradeDayRepository tradeDayRepository = Platforms.container.Resolve<TradeDayRepository>();

        public List<DateTime> fetchFromLocalCsvOrWindAndSaveAndCache(int localCsvExpiration = 180, bool appendMode = false, String tag = "TradeDays")
        {
            return tradeDayRepository.fetchFromLocalCsvOrWindAndSaveAndCache(localCsvExpiration, appendMode, tag);
        }

    }
}
