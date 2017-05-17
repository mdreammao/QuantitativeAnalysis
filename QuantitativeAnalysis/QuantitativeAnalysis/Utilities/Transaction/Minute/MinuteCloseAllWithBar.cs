

using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.PositionModel;
using QuantitativeAnalysis.ModelLayer.SignalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.AccountOperator.Minute
{
    public class MinuteCloseAllWithBar
    {
        /// <summary>
        /// 平仓（所有的）
        /// </summary>
        /// <param name="data">KLine格式的交易数据</param>
        /// <param name="positions">头寸信息</param>
        /// <param name="myAccount">账户信息</param>
        /// <param name="now">交易日的时间信息</param>
        /// <param name="nowIndex">当前索引值（不知道什么意思）</param>
        /// <param name="slipPoint">滑点</param>
        /// <returns></returns>
        public static Dictionary<string, ExecutionReport> CloseAllPosition(Dictionary<string, List<KLine>> data, ref SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions, ref BasicAccount myAccount, DateTime now, int nowIndex, double slipPoint = 0.00)
        {
            //初始化记录成交回报的变量
            Dictionary<string, ExecutionReport> tradingFeedback = new Dictionary<string, ExecutionReport>();
            //初始化平仓信号
            Dictionary<string, MinuteSignal> signal = new Dictionary<string, MinuteSignal>();
            //查询当前持仓情况
            Dictionary<string, PositionsWithDetail> positionShot = new Dictionary<string, PositionsWithDetail>();
            //获取最新持仓情况
            Dictionary<string, PositionsWithDetail> positionLast = (positions.Count == 0 ? null : positions[positions.Keys.Last()]);
            //检查最新持仓，若无持仓，直接返回
            if (positionLast == null)
            {
                return tradingFeedback;
            }
            else
            {
                positionShot = new Dictionary<string, PositionsWithDetail>(positionLast);
            }
            //生成清仓信号
            foreach (var position0 in positionShot.Values)
            {
                //若当前品种持仓量为0（仅用于记录历史持仓）
                if (position0.volume == 0)
                    continue;

                int index = nowIndex;

                //对所有的持仓，生成现价等量反向的交易信号(cuixun 也就是平仓信号)
                MinuteSignal nowSignal = new MinuteSignal()
                {
                    code = position0.code,
                    volume = -position0.volume,
                    time = now,
                    tradingVarieties = position0.tradingVarieties,
                    price = data[position0.code][index].open,
                    minuteIndex = index
                };
                signal.Add(nowSignal.code, nowSignal);
            }
            return MinuteTransactionWithBar.ComputePosition(signal, data, ref positions, ref myAccount, slipPoint: slipPoint, now: now, nowIndex: nowIndex);
        }
    }
}
