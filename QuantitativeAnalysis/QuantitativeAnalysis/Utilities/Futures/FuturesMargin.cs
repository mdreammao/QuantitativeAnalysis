using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Futures
{
    public class FuturesMargin
    {
        /// <summary>
        /// 期货开仓保证金的计算
        /// </summary>
        /// <param name="underlyingOpenPrice">标的开仓价格</param>
        /// <param name="marginRatio">保证金比例</param>
        /// <param name="multiplier">乘数</param>
        /// <returns></returns>
        public static double ComputeOpenMargin(double underlyingOpenPrice, double marginRatio, double multiplier)
        {
            return underlyingOpenPrice * multiplier * marginRatio;
        }

        /// <summary>
        /// 计算实时保证金计算带来的现金流
        /// </summary>
        /// <param name="underlyingPrice">标的价格</param>
        /// <param name="marginRatio">保证金比例</param>
        /// <param name="multiplier">乘数</param>
        /// <param name="longOrShort">多空方向</param>
        /// <param name="lastMargin">上次计算时的保证金</param>
        /// <param name="lastUnderlyingPrice">上次计算时标的价格</param>
        /// <returns></returns>
        public static double ComputeCashFlow(double underlyingPrice, double marginRatio, double multiplier, double longOrShort, double lastMargin, double lastUnderlyingPrice)
        {
            double margin = ComputeOpenMargin(underlyingPrice, marginRatio, multiplier);//实时保证金
            return (lastMargin + longOrShort * (underlyingPrice - lastUnderlyingPrice) * multiplier) - margin; //浮动盈亏带来的现金流
        }

    }
}
