using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{

    public class Tick : Sequential
    {
        public string code { get; set; }
        public DateTime time { get; set; }
        public double lastPrice { get; set; }
    }

    public class TickFromMssql : Tick
    {
        public double volume { get; set; }
        public double amount { get; set; }
       
        public double preClose { get; set; }
        
        //包含交易日字段和自然日字段
        public int tdate { get; set; }
        public int ttime { get; set; }
        public double ask1 { get; set; }
        public double askv1 { get; set; }
        public double ask2 { get; set; }
        public double askv2 { get; set; }
        public double ask3 { get; set; }
        public double askv3 { get; set; }
        public double ask4 { get; set; }
        public double askv4 { get; set; }
        public double ask5 { get; set; }
        public double askv5 { get; set; }
        public double bid1 { get; set; }
        public double bidv1 { get; set; }
        public double bid2 { get; set; }
        public double bidv2 { get; set; }
        public double bid3 { get; set; }
        public double bidv3 { get; set; }
        public double bid4 { get; set; }
        public double bidv4 { get; set; }
        public double bid5 { get; set; }
        public double bidv5 { get; set; }

    }
}
