using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    /// <summary>
    /// 策略的性能统计指标
    /// </summary>
    public class PerformanceStatisics
    {
        public double netProfit { get; set; }//净利润
        public double perNetProfit { get; set; }//平均每笔交易净利润
        public double totalReturn { get; set; }//总收益率
        public double anualReturn { get; set; }//年化收益率
        public double anualSharpe { get; set; }//年化夏普率
        public double winningRate { get; set; }//胜率
        public double PnLRatio { get; set; }//盈亏比
        public double maxDrawDown { get; set; }//最大回撤率        
        public double maxProfitRatio { get; set; }//最大升水率
        public double profitMDDRatio { get; set; }//收益回撤比
        public double informationRatio { get; set; }//信息比率
        public double alpha { get; set; }//Jensen' alpha
        public double beta { get; set; }//beta系数，CAPM模型
        public double rSquare { get; set; }//R平方，线性拟合优度
        public double averageHoldingRate { get; set; }//平均持仓比例
        public double averagePositionRate { get; set; }//平均仓位

    }
}
