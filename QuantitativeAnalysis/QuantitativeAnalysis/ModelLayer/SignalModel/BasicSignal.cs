using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.SignalModel
{
    public class BasicSignal
    {
        public DateTime time { get; set; }
        public string code { get; set; }
        public double volume { get; set; }
        public double price { get; set; }
        //交易品种
        public string tradingVarieties { get; set; }
        //买卖价差
        public double bidAskSpread { get; set; }
    }


}
