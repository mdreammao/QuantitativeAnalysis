using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.Core
{
    public class MySettings
    {
        public static void CommonSettings()
        {
            Dictionary<string, double> slipRatio = new Dictionary<string, double>();
            Dictionary<string, double> slipPoint = new Dictionary<string, double>();
            slipRatio.Add("RB.SHF", 0.0001);
            slipPoint.Add("RU.SHF", 2);
            Caches.put("slipRation", slipRatio);
            Caches.put("slipPoint", slipPoint);
        }
    }
}
