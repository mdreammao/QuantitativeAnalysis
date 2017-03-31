using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.PositionModel
{
    public class PositionsWithDetail : BasicPositions
    {
        public PositionDetail LongPosition { get; set; }
        //    public PositionDetail closeLong { get; set; }
        public PositionDetail ShortPosition { get; set; }
        //    public PositionDetail clsoeShort { get; set; }
        public PositionsWithDetail myClone(PositionsWithDetail target)
        {
            LongPosition = new PositionDetail(target.LongPosition.volume, target.LongPosition.totalCost, target.LongPosition.averagePrice);
            ShortPosition = new PositionDetail(target.ShortPosition.volume, target.ShortPosition.totalCost, target.ShortPosition.averagePrice);
            record = target.record;
            time = target.time;
            code = target.code;
            currentPrice = target.currentPrice;
            totalAmt = target.totalAmt;
            totalCashFlow = target.totalCashFlow;
            tradingVarieties = target.tradingVarieties;
            transactionCost = target.transactionCost;
            volume = target.volume;
            return this;
        }
    }
}
