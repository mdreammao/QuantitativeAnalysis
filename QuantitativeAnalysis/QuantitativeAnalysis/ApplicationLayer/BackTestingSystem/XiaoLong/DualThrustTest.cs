using Autofac;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ModelLayer.PositionModel;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.ServiceLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuantitativeAnalysis.DataAccessLayer.Futures;
using QuantitativeAnalysis.Utilities.Transaction.Minute.maoheng;
using QuantitativeAnalysis.ModelLayer.SignalModel;
using QuantitativeAnalysis.PresentationLayer;
using QuantitativeAnalysis.Utilities.DataApplication;

namespace BackTestingPlatform.Strategies.Futures.XiaoLong
{
    public class DualThrustTest
    {
        //回测参数设置
        private double initialCapital = 3000;
        private double slipPoint = 0.3;
        private DateTime startDate, endDate;
        //private double longLevel = 0.8, shortLevel = -0.8;
        private string underlying;
        private int frequency = 1;
        //private int numbers = 5;
        private double lossPercent = 0.005;
        private List<DateTime> tradeDays = new List<DateTime>();
        private Dictionary<DateTime, int> timeList = new Dictionary<DateTime, int>();
        private List<NetValue> netValue = new List<NetValue>();

        /// 策略的构造函数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="underlying"></param>
        /// <param name="frequency"></param>
        /// <param name="numbers"></param>
        /// <param name="longLevel"></param>FourParameterPairs
        /// <param name="shortLevel"></param>
        public DualThrustTest(int startDate, int endDate, string underlying)
        {
            this.startDate = Kit.ToDate(startDate);
            this.endDate = Kit.ToDate(endDate);
            this.underlying = underlying;
            this.tradeDays = DateUtils.GetTradeDays(startDate, endDate);
            if (underlying.IndexOf("RB") > -1) //螺纹钢手续费为每手万一，一手乘数为10
            {
                initialCapital = 3000;
                slipPoint = initialCapital * 0.0001;
            }
            else if (underlying.IndexOf("RU") > -1)//橡胶的手续费为万分之0.45,一手乘数为10
            {
                initialCapital = 30000;
                slipPoint = 2;//30000*0.000045=1.35约等于2
            }
            else if (underlying.IndexOf("A") > -1 && underlying.IndexOf("AU") < 0) //大豆的手续费为每手2块钱，一手乘数为10
            {
                initialCapital = 4000;
                slipPoint = 0.2;
            }
            else if (underlying.IndexOf("M") > -1) //豆粕的手续费为每手1.5块钱，一手乘数为10
            {
                initialCapital = 3000;
                slipPoint = 0.15;
            }
            else if (underlying.IndexOf("AU") > -1)
            {
                initialCapital = 300;
                slipPoint = 0.02;
            }
            else if (underlying.IndexOf("NI") > -1)
            {
                initialCapital = 12000;
                slipPoint = 1;
            }
            else if (underlying.IndexOf("IF") > -1)
            {
                initialCapital = 6000;
                slipPoint = 1 * 6000 / 10000;
            }
            compute();
        }

        public void compute()
        {
            //初始化头寸信息
            SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions = new SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>>();
            //初始化Account信息
            BasicAccount myAccount = new BasicAccount(initialAssets: initialCapital, totalAssets: initialCapital, freeCash: initialCapital);
            //记录历史账户信息
            List<BasicAccount> accountHistory = new List<BasicAccount>();
            //基准衡量
            List<double> benchmark = new List<double>();
            //持仓量
            double positionVolume = 0;
            //最大收入值
            double maxIncome = 0;


            //螺纹钢的日线数据准备，从回测期开始之前10个交易开始取
            int number = 10;
            double k1 = 0.2;
            double k2 = -0.1;
            List<FuturesDaily> dailyData = new List<FuturesDaily>();
            dailyData = Platforms.container.Resolve<FuturesDailyRepository>().fetchFromLocalCsvOrWindAndSave(underlying, startDate, endDate);
            List<double> highList = new List<double>();
            List<double> closeList = new List<double>();
            List<double> lowList = new List<double>();
            List<double> buyLineList = new List<double>();
            List<double> sellLineList = new List<double>();
            List<double> rangeList = new List<double>();

            for (int i = 0; i < dailyData.Count; i++)
            {
                if (highList.Count == number)
                {
                    highList.RemoveAt(0);
                    closeList.RemoveAt(0);
                    lowList.RemoveAt(0);
                }
                highList.Add(dailyData[i].high);
                closeList.Add(dailyData[i].close);
                lowList.Add(dailyData[i].low);

                //N日High的最高价HH
                double hightestHigh = highList.Max();
                //N日Close的最高价HC
                double hightestClose = closeList.Max();
                //N日Close的最低价LC
                double lowestClose = closeList.Min();
                //N日Low的最低价LL
                double open = dailyData[i].open;
                double lowestLow = lowList.Min();
                double range = Math.Max(hightestHigh - lowestClose, hightestClose - lowestLow);
                double buyLine = open + k1 * range;
                double sellLine = open + k2 * range;

                rangeList.Add(range);
                buyLineList.Add(buyLine);
                sellLineList.Add(sellLine);
            }

            //第一层循环：所有交易日循环一遍...
            for (int i = 0; i < tradeDays.Count(); i++)
            {
                if (i == 513)
                {
                    double buyLine1 = buyLineList[i];
                    double sellLine1 = sellLineList[i];
                }
                DateTime today = tradeDays[i];
                //从wind或本地CSV获取相应交易日的数据list
                //这里不能直接去调用FreqTransferUtils.minuteToNMinutes(data, frequency),因为data.count可能为0
                var dataOnlyToday = getData(today, underlying);//一个交易日里有多条分钟线数据

                //将获取的数据，储存为KLine格式
                Dictionary<string, List<KLine>> dataToday = new Dictionary<string, List<KLine>>();
                dataToday.Add(underlying, dataOnlyToday.Cast<KLine>().ToList());

                //第二层循环：只循环某当天的数据（开始的索引值为前一天数据的List.count）
                for (int j = 15; j < dataOnlyToday.Count() - 30; j++)
                {
                    DateTime now = dataOnlyToday[j].time;

                    # region 追踪止损判断 触发止损平仓

                    //追踪止损判断 触发止损平仓
                    if (positionVolume != 0) //头寸量不为0，额外要做的操作 
                    {
                        //计算开盘价和头寸当前价的差价
                        double incomeNow = individualIncome(positions.Last().Value[underlying], dataOnlyToday[j].open);
                        //若当前收入大于最大收入值，则更新最大收入值
                        if (incomeNow > maxIncome)
                        {
                            maxIncome = incomeNow;
                        }

                        //若盈利回吐大于5个点 或者 最大收入大于45，则进行平仓
                        else if ((maxIncome - incomeNow) > lossPercent * Math.Abs(dataOnlyToday[j].open) || incomeNow < -lossPercent * Math.Abs(dataOnlyToday[j].open)) //从最高点跌下来3%，就止损
                        {
                            positionVolume = 0;
                            Console.WriteLine("追踪止损！平仓价格: {0}", dataOnlyToday[j].open);
                            MinuteCloseAllWithBar.CloseAllPosition(dataToday, ref positions, ref myAccount, now, j, slipPoint);
                            maxIncome = 0;
                        }
                    }

                    #endregion

                    var price = dataOnlyToday[j].open;
                    //价格向上突破上轨
                    if (price > buyLineList[i])
                    {
                        //如果持有空仓，则先平仓
                        if (positionVolume == -1)
                        {
                            positionVolume = 0;
                            Console.WriteLine("追踪止损！平仓价格: {0}", dataOnlyToday[j].open);
                            MinuteCloseAllWithBar.CloseAllPosition(dataToday, ref positions, ref myAccount, now, j, slipPoint);
                        }
                        //如果没有仓位，则直接多仓
                        if (positionVolume == 0)
                        {
                            double volume = 1;
                            //长头寸信号
                            MinuteSignal longSignal = new MinuteSignal() { code = underlying, volume = volume, time = now, tradingVarieties = "futures", price = dataOnlyToday[j].open, minuteIndex = j };
                            //signal保存长头寸longSignal信号
                            Dictionary<string, MinuteSignal> signal = new Dictionary<string, MinuteSignal>();
                            signal.Add(underlying, longSignal);
                            MinuteTransactionWithBar.ComputePosition(signal, dataToday, ref positions, ref myAccount, slipPoint: slipPoint, now: now, nowIndex: longSignal.minuteIndex);
                            Console.WriteLine("做多期货！多头开仓价格: {0}", dataOnlyToday[j].open);
                            //头寸量叠加
                            positionVolume += volume;
                        }
                    }

                    //价格向下突破下轨
                    if (price < sellLineList[i])
                    {
                        //如果持有多仓，则先平仓
                        if (positionVolume == 1)
                        {
                            positionVolume = 0;
                            Console.WriteLine("追踪止损！平仓价格: {0}", dataOnlyToday[j].open);
                            MinuteCloseAllWithBar.CloseAllPosition(dataToday, ref positions, ref myAccount, now, j, slipPoint);
                        }
                        //如果没有仓位，则直接空仓
                        if (positionVolume == 0)
                        {
                            double volume = -1;
                            MinuteSignal shortSignal = new MinuteSignal() { code = underlying, volume = volume, time = now, tradingVarieties = "futures", price = dataOnlyToday[j].open, minuteIndex = j };
                            Console.WriteLine("做空期货！空头开仓价格: {0}", dataOnlyToday[j].open);
                            Dictionary<string, MinuteSignal> signal = new Dictionary<string, MinuteSignal>();
                            signal.Add(underlying, shortSignal);
                            positionVolume += volume;
                            //分钟级交易
                            MinuteTransactionWithBar.ComputePosition(signal, dataToday, ref positions, ref myAccount, slipPoint: slipPoint, now: now, nowIndex: shortSignal.minuteIndex);
                        }
                    }
                }
                int closeIndex = dataOnlyToday.Count() - 1;

                if (positionVolume != 0)
                {
                    positionVolume = 0;
                    MinuteCloseAllWithBar.CloseAllPosition(dataToday, ref positions, ref myAccount, dataOnlyToday[closeIndex].time, closeIndex, slipPoint);
                    Console.WriteLine("{2}   每日收盘前强制平仓，平仓价格:{0},账户价值:{1}", dataOnlyToday[closeIndex].open, myAccount.totalAssets, today);
                }

                if (dataOnlyToday.Count > 0)
                {
                    //更新当日属性信息
                    AccountOperator.Minute.maoheng.AccountUpdatingWithMinuteBar.computeAccount(ref myAccount, positions, dataOnlyToday.Last().time, dataOnlyToday.Count() - 1, dataToday);

                    //记录历史仓位信息
                    accountHistory.Add(new BasicAccount(myAccount.time, myAccount.totalAssets, myAccount.freeCash, myAccount.positionValue, myAccount.margin, myAccount.initialAssets));
                    benchmark.Add(dataOnlyToday.Last().close);
                    if (netValue.Count() == 0)
                    {
                        netValue.Add(new NetValue { time = today, netvalueReturn = 0, benchmarkReturn = 0, netvalue = myAccount.totalAssets, benchmark = dataOnlyToday.Last().close });
                    }
                    else
                    {
                        var netValueLast = netValue.Last();
                        netValue.Add(new NetValue { time = today, netvalueReturn = myAccount.totalAssets / netValueLast.netvalue - 1, benchmarkReturn = dataOnlyToday.Last().close / netValueLast.benchmark - 1, netvalue = myAccount.totalAssets, benchmark = dataOnlyToday.Last().close });
                    }
                }
            }

            ChartStatistics chart = new ChartStatistics();
            chart.showChart(accountHistory, positions, benchmark, underlying, initialCapital, netValue, startDate, endDate, frequency);

            ////策略绩效统计及输出
            //PerformanceStatisics myStgStats = new PerformanceStatisics();
            ////TODO:了解该函数中计算出了那些评价标准
            //myStgStats = PerformanceStatisicsUtils.compute(accountHistory, positions, benchmark.ToArray());
            ////画图
            //Dictionary<string, double[]> line = new Dictionary<string, double[]>();
            //double[] netWorth = accountHistory.Select(a => a.totalAssets / initialCapital).ToArray();
            //line.Add("NetWorth", netWorth);
            //string recordName = underlying.Replace(".", "_") + "_DH_" /*+ "numbers_" + numbers.ToString()*/ + "_frequency_" + frequency.ToString() + "_k2_" + k2.ToString();
            ////记录净值数据
            //RecordUtil.recordToCsv(accountHistory, GetType().FullName, "account", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            //RecordUtil.recordToCsv(netValue, GetType().FullName, "netvalue", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            ////记录持仓变化
            //var positionStatus = OptionRecordUtil.Transfer(positions);
            //RecordUtil.recordToCsv(positionStatus, GetType().FullName, "positions", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            ////记录统计指标
            //var performanceList = new List<PerformanceStatisics>();
            //performanceList.Add(myStgStats);
            //RecordUtil.recordToCsv(performanceList, GetType().FullName, "performance", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            ////统计指标在console 上输出
            //Console.WriteLine("--------Strategy Performance Statistics--------\n");
            //Console.WriteLine(" netProfit:{0,5:F4} \n totalReturn:{1,-5:F4} \n anualReturn:{2,-5:F4} \n anualSharpe :{3,-5:F4} \n winningRate:{4,-5:F4} \n PnLRatio:{5,-5:F4} \n maxDrawDown:{6,-5:F4} \n maxProfitRatio:{7,-5:F4} \n informationRatio:{8,-5:F4} \n alpha:{9,-5:F4} \n beta:{10,-5:F4} \n averageHoldingRate:{11,-5:F4} \n", myStgStats.netProfit, myStgStats.totalReturn, myStgStats.anualReturn, myStgStats.anualSharpe, myStgStats.winningRate, myStgStats.PnLRatio, myStgStats.maxDrawDown, myStgStats.maxProfitRatio, myStgStats.informationRatio, myStgStats.alpha, myStgStats.beta, myStgStats.averageHoldingRate);
            //Console.WriteLine("-----------------------------------------------\n");
            ////benchmark净值
            //List<double> netWorthOfBenchmark = benchmark.Select(x => x / benchmark[0]).ToList();
            //line.Add("Base", netWorthOfBenchmark.ToArray());
            //string[] datestr = accountHistory.Select(a => a.time.ToString("yyyyMMdd")).ToArray();

            ////绘制图形的标题
            //string formTitle = this.startDate.ToShortDateString() + "--" + this.endDate.ToShortDateString() + "  " + this.underlying + " 净值曲线"
            //    + "\r\n" + "\r\n" + "净利润：" + myStgStats.netProfit + "  " + "夏普率：" + myStgStats.anualSharpe + "  " + "最大回撤：" + myStgStats.maxDrawDown
            //    + "\r\n" + "\r\n" + "参数包含: frequency，numbers，lossPercent，K1，K2";
            ////生成图像
            //PLChart plc = new PLChart(line, datestr, formTitle: formTitle);
            ////运行图像
            //Application.Run(plc);
            ////保存图像
            //plc.SaveZed(GetType().FullName, this.underlying, this.startDate, this.endDate, myStgStats.netProfit.ToString(), myStgStats.anualSharpe.ToString(), myStgStats.maxDrawDown.ToString());
            ////Application.Run(new PLChart(line, datestr));
          
        }

        /// <summary>
        /// 计算单独头寸的收入
        /// </summary>
        /// <param name="position">传入的头寸</param>
        /// <param name="price">传入的价格</param>
        /// <returns></returns>
        private double individualIncome(PositionsWithDetail position, double price)
        {
            double income = 0;
            if (position.LongPosition.volume != 0)
            {
                return (price - position.LongPosition.averagePrice);
            }
            if (position.ShortPosition.volume != 0)
            {
                return (position.ShortPosition.averagePrice - price);
            }
            return income;
        }

        /// <summary>
        /// 从wind或本地CSV获取当天数据
        /// </summary>
        /// <param name="today">今天的日期</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        private List<FuturesMinute> getData(DateTime today, string code)
        {
            List<FuturesMinute> orignalList = Platforms.container.Resolve<FuturesMinuteRepository>().fetchFromLocalCsvOrWindAndSave(code, today);
            List<FuturesMinute> data = KLineDataUtils.leakFilling(orignalList);

            //从本地csv 或者 wind获取数据，从wind拿到额数据会保存在本地
            //List<FuturesMinute> data = KLineDataUtils.leakFilling(Platforms.container.Resolve<FuturesMinuteRepository>().fetchFromLocalCsvOrWindAndSave(code, today));

            #region 20170118更新
            //下面这行需要注释掉，因为可能data的count为0
            var dataModified = FreqTransferUtils.minuteToNMinutes(data, frequency);
            #endregion

            return dataModified;
        }
    }
}
