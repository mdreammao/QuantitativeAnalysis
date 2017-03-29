using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.PositionModel
{
    public class PositionDetail
    {
        public double volume { get; set; }
        public double totalCost { get; set; }
        public double averagePrice { get; set; }
        public PositionDetail(double volume = 0, double totalCost = 0, double averagePrice = 0)
        {
            this.volume = volume;
            this.totalCost = totalCost;
            this.averagePrice = averagePrice;
        }
    }
}
