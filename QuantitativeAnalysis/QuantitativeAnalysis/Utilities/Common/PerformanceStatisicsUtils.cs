
using QuantitativeAnalysis.ModelLayer.PositionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearRegression;
using QuantitativeAnalysis.ModelLayer.Common;

namespace QuantitativeAnalysis.Utilities.Common
{
    public static class PerformanceStatisicsUtils
    {
        /// <summary>
        ///  计算策略的各项性能指标，目前胜率等和交易次数相关的指标只针对非仓位管理型的策略
        /// 该性能统计默认为是对日频采样的统计，若为其他频率需调整barsOfYear
        /// 所有指标保留四位小数
        /// 若未穿入benchmark
        /// </summary>
        /// <param name="accountHistory"></param>
        /// <param name="positions"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static PerformanceStatisics compute(List<BasicAccount> accountHistory, SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions, double[] benchmark = null, double riskFreeRate = 0.00)
        {
            //若没有输入benchmark，构建默认的benchmark（全为1）3
            int sign0 = 0;
            if (benchmark == null)
            {
                benchmark = new double[accountHistory.Count];
                for (; sign0 < accountHistory.Count; sign0++) benchmark[sign0] = 1;
            }
            //benchmark与净值数据不匹配时跳出
            if (benchmark.Length != accountHistory.Count)
            {
                Console.WriteLine("The length of benchmark is not consistent with accountHistory!");
                return null;
            }
            PerformanceStatisics performanceStats = new PerformanceStatisics();

            //无风险收益率(年化)        
            double barsOfYear = 252;
            //account长度，account记录周期数
            int lengthOfAccount = accountHistory.Count;
            //初始资产
            double intialAssets = accountHistory[0].totalAssets;
            //净值
            double[] netWorth = accountHistory.Select(a => a.totalAssets / intialAssets).ToArray();
            //收益率与超额收益率，比净值数少一
            double[] returnArray = new double[netWorth.Length - 1];//收益率
            double[] returnArrayOfBenchmark = new double[netWorth.Length - 1];//基准收益率
            double[] benchmarkExcessReturn = new double[returnArray.Length];//基准收益率 - 无风险收益率
            double[] excessReturnToBenchmark = new double[returnArray.Length];//收益率 - 基准收益率
            double[] excessReturnToRf = new double[returnArray.Length];//收益率 - 无风险收益率
            double[] timeIndexList = new double[netWorth.Length];//时间标签tick
            for (int i = 0; i < returnArray.Length; i++)
            {
                returnArray[i] = (netWorth[i + 1] - netWorth[i]) / netWorth[i];
                returnArrayOfBenchmark[i] = (benchmark[i + 1] - benchmark[i]) / benchmark[i];
                excessReturnToRf[i] = returnArray[i] - riskFreeRate / barsOfYear;
                benchmarkExcessReturn[i] = returnArrayOfBenchmark[i] - riskFreeRate / barsOfYear;
                excessReturnToBenchmark[i] = returnArray[i] - returnArrayOfBenchmark[i];
                timeIndexList[i] = i;
            }
            timeIndexList[timeIndexList.Length - 1] = timeIndexList.Length - 1;
            //交易次数
            double numOfTrades = 0;
            //成功交易次数
            double numOfSuccess = 0;
            //失败交易次数
            double numOfFailure = 0;
            //累计盈利
            double cumProfit = 0;
            //累计亏损
            double cumLoss = 0;

            //交易统计
            foreach (var date in positions.Keys)
            {
                foreach (var variety in positions[date].Keys)
                {
                    //交易笔数累计（一组相邻的反向交易为一笔交易）
                    numOfTrades += positions[date][variety].record.Count / 2;
                    //成功交易笔数累计
                    //  List<TransactionRecord> lastestRecord = new List<TransactionRecord>(positions[date][variety].record[positions[date][variety].record.Count -1])
                    for (int rec = 1; rec < positions[date][variety].record.Count; rec += 2)
                    {
                        var nowRec = positions[date][variety].record[rec];
                        var lastRec = positions[date][variety].record[rec - 1];
                        //若当前为平多，则平多价格大于开多价格，成功数+1；
                        //若当前为平空，则平空价格小于于开空价格，成功数+1
                        if ((nowRec.volume < 0 && nowRec.price > lastRec.price) || (nowRec.volume > 0 && nowRec.price < lastRec.price))
                        {
                            //成功计数
                            numOfSuccess++;
                            //收益累加
                            cumProfit += nowRec.volume < 0 ? (nowRec.price - lastRec.price) * Math.Abs(nowRec.volume) : (-nowRec.price + lastRec.price) * Math.Abs(nowRec.volume);
                        }
                        else
                        {
                            //亏损累加
                            cumLoss += nowRec.volume < 0 ? (nowRec.price - lastRec.price) * Math.Abs(nowRec.volume) : (-nowRec.price + lastRec.price) * Math.Abs(nowRec.volume);
                        }
                    }
                }
            }
            numOfFailure = numOfTrades - numOfSuccess;

            // netProfit
            performanceStats.netProfit = Math.Round((accountHistory[lengthOfAccount - 1].totalAssets - intialAssets), 4);

            //perNetProfit
            performanceStats.perNetProfit = Math.Round((performanceStats.netProfit / numOfTrades), 4);

            //totalReturn
            performanceStats.totalReturn = Math.Round((performanceStats.netProfit / intialAssets), 4);

            //anualReturn
            double daysOfBackTesting = accountHistory.Count;
            performanceStats.anualReturn = Math.Round((performanceStats.totalReturn / (daysOfBackTesting / barsOfYear)), 4);

            //anualSharpe
            performanceStats.anualSharpe = Math.Round(((returnArray.Average() - riskFreeRate / barsOfYear) / Statistics.StandardDeviation(returnArray) * Math.Sqrt(252)), 4);

            //winningRate
            performanceStats.winningRate = Math.Round((numOfSuccess / numOfTrades), 4);

            //PnLRatio
            performanceStats.PnLRatio = Math.Round((cumProfit / Math.Abs(cumLoss)), 4);

            //maxDrawDown
            performanceStats.maxDrawDown = Math.Round(computeMaxDrawDown(netWorth.ToList()), 4);

            //maxProfitRate
            performanceStats.maxProfitRatio = Math.Round(computeMaxProfitRate(netWorth.ToList()), 4);

            //profitMDDRatio
            performanceStats.profitMDDRatio = Math.Round((performanceStats.totalReturn / performanceStats.maxDrawDown), 4);

            //informationRatio

            performanceStats.informationRatio = Math.Round((excessReturnToBenchmark.Average() / Statistics.StandardDeviation(excessReturnToBenchmark) * Math.Sqrt(barsOfYear)), 4);

            //alpha
            var regstats = SimpleRegression.Fit(benchmarkExcessReturn, excessReturnToRf);
            performanceStats.alpha = Math.Round(regstats.Item1, 4);

            //beta
            performanceStats.beta = Math.Round(regstats.Item2, 4);

            //rSquare
            performanceStats.rSquare = Math.Round(Math.Pow(Correlation.Pearson(timeIndexList, netWorth), 2), 4);

            //averageHoldingRate 
            double barsOfHolding = 0;
            double[] positionRate = new double[accountHistory.Count];
            int sign = 0;
            foreach (var accout in accountHistory)
            {
                if (accout.positionValue != 0) barsOfHolding++;
                positionRate[sign] = accout.positionValue / accout.totalAssets;
                sign++;
            }

            performanceStats.averageHoldingRate = Math.Round((barsOfHolding / accountHistory.Count), 4);

            //averagePositionRate
            performanceStats.averagePositionRate = Math.Round(positionRate.Average(), 4);

            return performanceStats;
        }

        /// <summary>
        /// 计算最大回撤率
        /// </summary>
        /// <param name="price"></param>传入double数组的价格序列
        /// <returns></returns>
        public static double computeMaxDrawDown(List<double> price)
        {
            double maxDrawDown = 0;
            double[] MDDArray = new double[price.Count];

            for (int i = 0; i < price.Count; i++)
            {
                double tempMax = price.GetRange(0, i + 1).Max();//获取从0到i的最大值
                if (tempMax == price[i])//如果当前值price[i]就是这段时期的max，则MDDArray[i]设为0
                    MDDArray[i] = 0;
                else//如果不是max，就计算和这段时期max的回撤比例
                    MDDArray[i] = Math.Abs((price[i] - tempMax) / tempMax);
            }
            maxDrawDown = MDDArray.Max();//最后MDDArray数组中，最大的数组即为最大回撤比率
            return maxDrawDown;
        }

        /// <summary>
        /// 计算最大净值升水率
        /// </summary>
        /// <param name="price"></param>传入double数组的价格序列
        /// <returns></returns>
        public static double computeMaxProfitRate(List<double> price)
        {
            double maxProfitRate = 0;
            double[] MPRArray = new double[price.Count];

            for (int i = 0; i < price.Count; i++)
            {
                double tempMin = price.GetRange(0, i + 1).Min();
                if (tempMin == price[i])
                    MPRArray[i] = 0;
                else
                    MPRArray[i] = Math.Abs((price[i] - tempMin) / tempMin);
            }
            maxProfitRate = MPRArray.Max();
            return maxProfitRate;
        }
    }
}
