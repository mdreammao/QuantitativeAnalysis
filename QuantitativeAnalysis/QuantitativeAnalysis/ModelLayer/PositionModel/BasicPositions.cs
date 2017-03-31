using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.PositionModel
{
    public class BasicPositions
    {
        public string code { get; set; }
        public DateTime time { get; set; }
        public double volume { get; set; }
        public double currentPrice { get; set; }
        public List<TransactionRecord> record { get; set; }
        public double transactionCost { get; set; }
        public double totalCashFlow { get; set; }
        //实时总权益
        public double totalAmt { get; set; }
        //交易品种
        public string tradingVarieties { get; set; }
    }
}
