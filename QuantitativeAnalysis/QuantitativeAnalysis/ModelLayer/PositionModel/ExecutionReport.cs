using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.PositionModel
{
    public class ExecutionReport
    {
        public DateTime time { get; set; }
        public double volume { get; set; }
        public double price { get; set; }
        public string code { get; set; }
        //记录成交状态
        public string status { get; set; }
    }
}
