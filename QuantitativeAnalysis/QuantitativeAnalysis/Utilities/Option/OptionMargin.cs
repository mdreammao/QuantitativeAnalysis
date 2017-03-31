using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Option
{
    /// <summary>
    /// 计算期权保证金的类。
    /// </summary>
    /// 认购期权义务仓开仓保证金＝[合约前结算价+Max（12%×合约标的前收盘价-认购期权虚值，7%×合约标的前收盘价）]×合约单位
    /// 认沽期权义务仓开仓保证金＝Min[合约前结算价+Max（12%×合约标的前收盘价-认沽期权虚值，7%×行权价格），行权价格] ×合约单位 
    /// 认购期权义务仓维持保证金＝[合约结算价+Max（12%×合约标的收盘价-认购期权虚值，7%×合约标的收盘价）]×合约单位
    /// 认沽期权义务仓维持保证金＝Min[合约结算价 +Max（12%×合约标的收盘价-认沽期权虚值，7%×行权价格），行权价格]×合约单位
    /// 认购期权虚值=max（行权价-合约标的收盘价，0）
    /// 认沽期权虚值=max（合约标的收盘价-行权价，0）
    public class OptionMargin
    {
        public static double ComputeOpenMargin(double underlyingPreClose, double optionPreSettlePrice, double strike, string type, double contractMultiplier, double underlyingPrice)
        {
            double margin = 0;
            if (type == "认购")
            {
                double invalue = Math.Max(strike - underlyingPrice, 0);
                /// 认购期权义务仓开仓保证金＝[合约前结算价+Max（12%×合约标的前收盘价-认购期权虚值，7%×合约标的前收盘价）]×合约单位
                margin = (optionPreSettlePrice + Math.Max(0.12 * underlyingPreClose - invalue, 0.07 * underlyingPreClose)) * contractMultiplier;

            }
            else if (type == "认沽")
            {
                double invalue = Math.Max(underlyingPrice - strike, 0);
                /// 认沽期权义务仓开仓保证金＝Min[合约前结算价+Max（12%×合约标的前收盘价-认沽期权虚值，7%×行权价格），行权价格] ×合约单位 
                margin = Math.Min(optionPreSettlePrice + Math.Max(0.12 * underlyingPreClose - invalue, 0.07 * strike), strike) * contractMultiplier;
            }
            return margin;
        }
        public static double ComputeMaintenanceMargin(double underlyingClose, double optionSettlePrice, double strike, string type, double contractMultiplier)
        {
            double margin = 0;
            if (type == "认购")
            {
                double invalue = Math.Max(strike - underlyingClose, 0);
                /// 认购期权义务仓维持保证金＝[合约结算价+Max（12%×合约标的收盘价-认购期权虚值，7%×合约标的收盘价）]×合约单位
                margin = (optionSettlePrice + Math.Max(0.12 * underlyingClose - invalue, 0.07 * underlyingClose)) * contractMultiplier;
            }
            else if (type == "认沽")
            {
                double invalue = Math.Max(underlyingClose - strike, 0);
                /// 认沽期权义务仓维持保证金＝Min[合约结算价 +Max（12%×合标的收盘价-认沽期权虚值，7%×行权价格），行权价格]×合约单位
                margin = Math.Min(optionSettlePrice + Math.Max(0.12 * underlyingClose - invalue, 0.07 * strike), strike) * contractMultiplier;
            }
            return margin;
        }
    }
}
