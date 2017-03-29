using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    public class KLine : Sequential
    {
        public DateTime time { get; set; } //自然日时间
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double amount { get; set; }
        public double openInterest { get; set; }//持仓量
        public DateTime tradeday { get; set; } //归属的交易日
        public override string ToString()
        {
            return String.Format("KLine{time={0},open={1},hi={2},lo={3},close={4},vol={5},amt={6}}",
                time, open, high, low, close, volume, amount);
        }
    }
}
