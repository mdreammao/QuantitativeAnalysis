using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Stock.BasicInfo
{
    public class StockBasicInfo
    {
        public DateTime listDate { get; set; }
        public DateTime delistDate { get; set; }
        public string name { get; set; }
    }
}
