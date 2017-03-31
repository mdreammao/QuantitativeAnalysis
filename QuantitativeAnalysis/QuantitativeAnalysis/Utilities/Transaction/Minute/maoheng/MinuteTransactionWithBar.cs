
using BackTestingPlatform.AccountOperator.Minute.maoheng;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.PositionModel;
using QuantitativeAnalysis.ModelLayer.SignalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Transaction.Minute.maoheng
{
    /// <summary>
    /// 根据分钟K线图来判断成交。具体的，如果开多仓，开仓价格必须大于等于K线中的最低价格，如果开空仓，开仓价格必须小于等于K线中的最高价格。
    /// 计算account的时候，按品种分别讨论，分成股票，期权和期货三类来讨论。
    /// </summary>
    public class MinuteTransactionWithBar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal">交易信号（正向/反向）</param>
        /// <param name="data">KLine格式的交易数据</param>
        /// <param name="positions">头寸信息</param>
        /// <param name="myAccount">账户信息</param>
        /// <param name="now">交易日的时间信息</param>
        /// <param name="nowIndex">当前索引值（不知道什么意思）</param>
        /// <param name="slipPoint">滑点</param>
        /// <returns></returns>
        public static Dictionary<string, ExecutionReport> ComputePosition(Dictionary<string, MinuteSignal> signal, Dictionary<string, List<KLine>> data, ref SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions, ref BasicAccount myAccount, DateTime now, int nowIndex, double slipPoint = 0.00)
        {
            //初始化记录成交回报的变量
            Dictionary<string, ExecutionReport> tradingFeedback = new Dictionary<string, ExecutionReport>();
            //初始化上一次头寸记录时间
            DateTime lastTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            //如果signal无信号，无法成交，直接返回空的成交回报。
            if (signal == null || signal.Count == 0)
            {
                return tradingFeedback;
            }
            if (positions.Count != 0)
            {
                lastTime = positions.Keys.Last();
            }
            //新建头寸变量，作为接受新仓位的容器
            Dictionary<string, PositionsWithDetail> positionShot = new Dictionary<string, PositionsWithDetail>();
            //如果持仓最后状态时间大于signal信号的时间，无成交，直接返回空的成交回报。
            if (lastTime > now)
            {
                return tradingFeedback;
            }
            //如果两者时间相等，则把总仓位变化数组positions中的最后一项，添加进新仓位的容器positionShot中
            if (now == lastTime)
            {
                positionShot = positions[positions.Keys.Last()];
            }
            //如果交易信号时间在最后一次持仓变化时间点之后，则需要重新把最后持仓的仓位变化信息手工copy一份；
            //然后添加进新仓位的容器positionShot中。
            else if (positions.Count > 0)//如果持仓大于0
            {
                foreach (var item in positions[positions.Keys.Last()])//循环 持仓最后状态时间的持仓数据
                {
                    //这里必须手动copy一份，不可以直接传引用。因为最后持仓变化节点的仓位信息是不应该被改变的；
                    //如果直接传引用，交易信号时间点仓位变化会同时改变最后持仓变化节点的仓位信息。
                    PositionsWithDetail position0 = new PositionsWithDetail().myClone(item.Value);//复制一份新的
                    positionShot.Add(position0.code, position0);
                }
            }
            //获取前一步的头寸信息，如果没有寸头就设为null
            Dictionary<string, PositionsWithDetail> positionLast = (positions.Count == 0 ? null : positions[positions.Keys.Last()]);

            //对信号进行遍历
            foreach (var signal0 in signal.Values)
            {
                //整理成交信号，剔除不合理信号。
                //①信号触发时间必须在positionLast的记录时间之后，在当前时间now之前。
                //②信号必须有合理的交易数量。
                //③信号必须有对应的数据。
                if (signal0.time != now) //【？？？】不是说必须要在当前时间now之前么
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.status = "交易时间错误，无效信号";
                    tradingFeedback.Add(signal0.code, report0);
                    continue;
                }
                if (signal0.volume == 0 || signal0.price == 0)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.status = "交易数据错误，无效信号";
                    tradingFeedback.Add(signal0.code, report0);
                    continue;
                }
                if (data.ContainsKey(signal0.code) == false)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.status = "无法找到行情数据，无效信号";
                    tradingFeedback.Add(signal0.code, report0);
                    continue;
                }

                //根据K线来判断成交数量，有可能完全成交，也有可能部分成交
                //开多头时，如果价格大于最低价，完全成交，否者不成交
                //开空头时，如果价格小于最高价，完全成交，否者不成交

                //找出对应的K线
                KLine KLineData = data[signal0.code][nowIndex];
                //确定滑点
                double slip = Math.Max(slipPoint, signal0.bidAskSpread);

                //开多头时，如果价格大于最低价，完全成交，否者不成交
                if (signal0.volume > 0 && signal0.price >= KLineData.low)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.volume = signal0.volume;
                    report0.price = signal0.price + slip;
                    report0.status = "完全成交";
                    tradingFeedback.Add(signal0.code, report0);
                }
                if (signal0.volume > 0 && signal0.price < KLineData.low)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.price = signal0.price + slip;
                    report0.status = "无法成交";
                    tradingFeedback.Add(signal0.code, report0);
                    continue;
                }

                //开空头时，如果价格小于最高价，完全成交，否者不成交
                if (signal0.volume < 0 && signal0.price <= KLineData.high)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.price = signal0.price - slip;
                    report0.volume = signal0.volume;
                    report0.status = "完全成交";
                    tradingFeedback.Add(signal0.code, report0);
                }
                if (signal0.volume < 0 && signal0.price > KLineData.high)
                {
                    ExecutionReport report0 = new ExecutionReport();
                    report0.code = signal0.code;
                    report0.time = signal0.time;
                    report0.price = signal0.price - slip;
                    report0.status = "无法成交";
                    tradingFeedback.Add(signal0.code, report0);
                    continue;
                }

                ///【解释：以下部分 position，positionShot和position0的关系】
                /// position：是传入的参数，
                ///

                //接下来处理能够成交的signal0，信号下单的时间只能是lastTime或者now。
                PositionsWithDetail position0 = new PositionsWithDetail();
                //查询当前持仓数量
                double nowHoldingVolume;
                //当前证券已有持仓
                if (positionShot.Count > 0 && positionShot.ContainsKey(signal0.code))
                {
                    //将当前证券持仓情况赋值给临时持仓变量
                    position0 = positionShot[signal0.code];
                    nowHoldingVolume = position0.volume;
                }
                else //若历史无持仓
                {
                    //当前信号证券代码
                    position0.code = signal0.code;
                    //多空头寸初始化
                    position0.LongPosition = new PositionDetail();
                    position0.ShortPosition = new PositionDetail();
                    position0.record = new List<TransactionRecord>();
                    nowHoldingVolume = 0;
                    positionShot.Add(position0.code, position0);
                }
                //持仓和开仓方向一致
                if (nowHoldingVolume * signal0.volume >= 0)
                {
                    //开多仓
                    if (signal0.volume > 0)
                    {
                        //重新计算仓位和价格
                        double volume = signal0.volume + position0.LongPosition.volume;
                        double cost = (signal0.price + slip) * signal0.volume + position0.LongPosition.volume * position0.LongPosition.averagePrice;
                        double averagePrice = cost / volume;
                        position0.LongPosition = new PositionDetail { volume = volume, totalCost = cost, averagePrice = averagePrice };
                    }
                    else //开空仓
                    {
                        //重新计算仓位和价格
                        double volume = signal0.volume + position0.ShortPosition.volume;
                        double cost = (signal0.price - slip) * signal0.volume + position0.ShortPosition.volume * position0.ShortPosition.averagePrice;
                        double averagePrice = cost / volume;
                        position0.ShortPosition = new PositionDetail { volume = volume, totalCost = cost, averagePrice = averagePrice };
                    }
                }
                else //持仓和开仓方向不一致
                {
                    if (nowHoldingVolume > 0) //原先持有多头头寸，现开空仓
                    {
                        //计算总头寸，分类讨论
                        double volume = signal0.volume + position0.LongPosition.volume;
                        if (volume > 0)
                        {
                            position0.LongPosition.volume = volume;
                            position0.LongPosition.totalCost = position0.LongPosition.volume * position0.LongPosition.averagePrice;
                        }
                        else if (volume < 0)
                        {
                            position0.ShortPosition.volume = volume;
                            position0.ShortPosition.totalCost = volume * (signal0.price - slip);
                            position0.ShortPosition.averagePrice = (signal0.price - slip);
                            position0.LongPosition = new PositionDetail();
                        }
                        else
                        {
                            position0.LongPosition = new PositionDetail();
                        }
                    }
                    else //原先持有空头头寸，现开多仓
                    {
                        //计算总头寸，分类讨论
                        double volume = signal0.volume + position0.ShortPosition.volume;
                        if (volume < 0)
                        {
                            position0.ShortPosition.volume = volume;
                            position0.ShortPosition.totalCost = position0.ShortPosition.volume * position0.ShortPosition.averagePrice;
                        }
                        else if (volume > 0)
                        {
                            position0.LongPosition.volume = volume;
                            position0.LongPosition.averagePrice = (signal0.price + slip);
                            position0.LongPosition.totalCost = (signal0.price - slip) * volume;
                            position0.ShortPosition = new PositionDetail();
                        }
                        else
                        {
                            position0.ShortPosition = new PositionDetail();
                        }
                    }
                }
                //更新其他信息
                position0.record.Add(new TransactionRecord
                {
                    time = now,
                    volume = signal0.volume,
                    price = signal0.price + slip * (signal0.volume > 0 ? 1 : -1),
                    code = position0.code
                });
                position0.totalCashFlow = position0.totalCashFlow - (signal0.price + (signal0.volume > 0 ? 1 : -1) * slip) * signal0.volume;
                position0.transactionCost = position0.transactionCost + Math.Abs(slip * signal0.volume);
                position0.volume = position0.volume + signal0.volume;
                position0.time = now;
                position0.code = signal0.code;
                position0.currentPrice = KLineData.close;
                position0.tradingVarieties = signal0.tradingVarieties;
                //总资产=权益现值+现金盈亏
                position0.totalAmt = position0.totalCashFlow + position0.volume * position0.currentPrice;
            }
            if (now > lastTime)
            {
                positions.Add(now, positionShot);
            }
            //更新持仓的头寸信息
            if (positions.Count != 0)
                AccountUpdatingWithMinuteBar.computeAccount(ref myAccount, positions, now, nowIndex, data);
            return tradingFeedback;
        }
    }
}
