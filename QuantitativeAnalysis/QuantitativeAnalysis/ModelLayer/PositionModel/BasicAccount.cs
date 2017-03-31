using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.PositionModel
{
    public class BasicAccount
    {
        public DateTime time { get; set; }
        public double totalAssets { get; set; }
        public double freeCash { get; set; }
        public double positionValue { get; set; }
        public double margin { get; set; }
        public double initialAssets { get; set; }
        public BasicAccount(DateTime time = new DateTime(), double totalAssets = 0, double freeCash = 0, double positionValue = 0, double margin = 0, double initialAssets = 0)
        {
            this.time = time;
            this.totalAssets = totalAssets;
            this.freeCash = freeCash;
            this.positionValue = positionValue;
            this.margin = margin;
            this.initialAssets = initialAssets;
        }
    }
}
