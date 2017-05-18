using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market
{
    public class StockDailyMarket : Factor
    {
        public double preClose { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double amount { get; set; }
        public double dealnum { get; set; }
        public double upsAndDowns { get; set; }
        public double percentUpsAndDowns { get; set; }
        public double swing { get; set; }
        public double vwap { get; set; }
        public double adjfactor { get; set; }
        public double turn { get; set; }
        public double free_turn { get; set; }
        public string trade_status { get; set; }
        public string susp_reason { get; set; }
        public double maxUpOrDown { get; set; }
    }
}
