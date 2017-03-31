using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Futures
{
    public class FuturesDaily : KLine
    {

    }

    public class FuturesDailyWithInfo : FuturesDaily
    {
        public FuturesInfo basicInfo { get; set; }
    }
}
