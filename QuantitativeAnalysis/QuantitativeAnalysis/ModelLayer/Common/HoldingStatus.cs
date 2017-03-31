using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    public class HoldingStatus
    {
        public DateTime time { get; set; }
        public string code { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public string remarks { get; set; }
        public HoldingStatus(DateTime time, string code, double price, double volume, string remarks = "")
        {
            this.time = time;
            this.code = code;
            this.price = price;
            this.volume = volume;
            this.remarks = remarks;
        }
    }
}
