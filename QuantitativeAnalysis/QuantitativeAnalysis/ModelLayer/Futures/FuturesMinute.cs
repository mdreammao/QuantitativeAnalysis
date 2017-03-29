
using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Futures
{
    public class FuturesMinute : KLine
    {

    }

    public class FuturesMinuteWithInfo : FuturesMinute
    {
        public FuturesInfo basicInfo { get; set; }
    }
}
