using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{

    public class Tick : Sequential, ICloneable
    {
        public string code { get; set; }
        public DateTime time { get; set; }
        public double lastPrice { get; set; }
        public Position[] ask { get; set; }
        public Position[] bid { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public override string ToString()
        {
            return String.Format("t={0},code={1},lastp={2}", time, code, lastPrice);
        }
    }

    public class TickFromMssql : Tick
    {
        public double high { get; set; }
        public double low { get; set; }

        public double volume { get; set; }
        public double amount { get; set; }
        public double preSettle { get; set; }

        public double preClose { get; set; }

        //包含交易日字段和自然日字段
        public int date { get; set; }
        public int ndate { get; set; }

        public int moment { get; set; }

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
