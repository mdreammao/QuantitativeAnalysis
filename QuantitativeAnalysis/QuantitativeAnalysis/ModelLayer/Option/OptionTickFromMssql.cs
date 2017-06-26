using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Option
{
    public class OptionTickFromMssql : TickFromMssql
    {
        public double openInterest { get; set; }
        public double preSettle { get; set; }
    }
}
