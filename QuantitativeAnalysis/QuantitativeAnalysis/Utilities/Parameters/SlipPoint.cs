using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Parameters
{
    public class SlipPoint
    {
        public static double getSlipRatio(string code)
        {
            double slipRatio = 0;
            string[] codeStr = code.ToUpper().Split('.');
            var slipList =(Dictionary<string, double>) Caches.get("slipRatio");
            switch (codeStr[codeStr.Length-1])
            {
                case "SH":
                    slipRatio = slipList["SH"];
                    break;
                case "SZ":
                    slipRatio = slipList["SZ"];
                    break;
                case "CFE":
                    slipRatio = slipList["CFE"];
                    break;
                case "SHF":
                    slipRatio = slipList["SHF"];
                    break;
                case "CZC":
                    slipRatio = slipList["CZC"];
                    break;
                case "DCE":
                    slipRatio = slipList["DCE"];
                    break;
                default:
                    slipRatio = slipList["default"];
                    break;
            }
            switch (codeStr[0])
            {
                case "RU":
                    slipRatio = slipList["RU.SHF"];
                    break;
                case "A":
                    slipRatio = slipList["A.DCE"];
                    break;
                default:
                    break;
            }
            return slipRatio;
        }
    }
}
