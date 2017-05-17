using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.MyCore
{
    public class MySettings
    {
        //进行大量的参数配置，对应common设置
        public static void CommonSettings()
        {
            Dictionary<string, double> slipRatio = setCommonSlipRatio();
            Caches.put("slipRatio", slipRatio);
        }

        //手续费的设置
        private static Dictionary<string,double> setCommonSlipRatio()
        {
            Dictionary<string, double> slipRatio = new Dictionary<string, double>();
            slipRatio.Add("default", 0.001);
            slipRatio.Add("SH", 0.001);
            slipRatio.Add("SZ", 0.001);
            slipRatio.Add("CFE", 0.0001);
            slipRatio.Add("SHF", 0.0001);
            slipRatio.Add("CZC", 0.0001);
            slipRatio.Add("DCE", 0.00005);
            slipRatio.Add("RU.SHF", 0.0001);
            slipRatio.Add("A.DCE", 0.000045);
            return slipRatio; 
        }
            
    }
}
