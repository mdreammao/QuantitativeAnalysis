using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Option;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.PositionModel;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuantitativeAnalysis.PresentationLayer
{
    class ChartStatistics
    {
        public void showChart(List<BasicAccount> accountHistory, SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions,
            List<double> benchmark, string underlying, double initialCapital, List<NetValue> netValue, DateTime startDate, DateTime endDate, int frequency) {
            //策略绩效统计及输出
            PerformanceStatisics myStgStats = new PerformanceStatisics();
            //TODO:了解该函数中计算出了那些评价标准
            myStgStats = PerformanceStatisicsUtils.compute(accountHistory, positions, benchmark.ToArray());
            //画图
            Dictionary<string, double[]> line = new Dictionary<string, double[]>();
            double[] netWorth = accountHistory.Select(a => a.totalAssets / initialCapital).ToArray();
            line.Add("NetWorth", netWorth);
            string recordName = underlying.Replace(".", "_") + "_DH_" /*+ "numbers_" + numbers.ToString()*/ + "_frequency_" + frequency.ToString();
            //记录净值数据
            RecordUtil.recordToCsv(accountHistory, GetType().FullName, "account", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            RecordUtil.recordToCsv(netValue, GetType().FullName, "netvalue", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            //记录持仓变化
            var positionStatus = OptionRecordUtility_50ETF.Transfer(positions);
            RecordUtil.recordToCsv(positionStatus, GetType().FullName, "positions", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            //记录统计指标
            var performanceList = new List<PerformanceStatisics>();
            performanceList.Add(myStgStats);
            RecordUtil.recordToCsv(performanceList, GetType().FullName, "performance", parameters: recordName, performance: myStgStats.anualSharpe.ToString("N").Replace(".", "_"));
            //统计指标在console 上输出
            Console.WriteLine("--------Strategy Performance Statistics--------\n");
            Console.WriteLine(" netProfit:{0,5:F4} \n totalReturn:{1,-5:F4} \n anualReturn:{2,-5:F4} \n anualSharpe :{3,-5:F4} \n winningRate:{4,-5:F4} \n PnLRatio:{5,-5:F4} \n maxDrawDown:{6,-5:F4} \n maxProfitRatio:{7,-5:F4} \n informationRatio:{8,-5:F4} \n alpha:{9,-5:F4} \n beta:{10,-5:F4} \n averageHoldingRate:{11,-5:F4} \n", myStgStats.netProfit, myStgStats.totalReturn, myStgStats.anualReturn, myStgStats.anualSharpe, myStgStats.winningRate, myStgStats.PnLRatio, myStgStats.maxDrawDown, myStgStats.maxProfitRatio, myStgStats.informationRatio, myStgStats.alpha, myStgStats.beta, myStgStats.averageHoldingRate);
            Console.WriteLine("-----------------------------------------------\n");
            //benchmark净值
            List<double> netWorthOfBenchmark = benchmark.Select(x => x / benchmark[0]).ToList();
            line.Add("Base", netWorthOfBenchmark.ToArray());
            string[] datestr = accountHistory.Select(a => a.time.ToString("yyyyMMdd")).ToArray();

            //绘制图形的标题
            string formTitle = startDate.ToShortDateString() + "--" + endDate.ToShortDateString() + "  " + underlying + " 净值曲线"
                + "\r\n" + "\r\n" + "净利润：" + myStgStats.netProfit + "  " + "夏普率：" + myStgStats.anualSharpe + "  " + "最大回撤：" + myStgStats.maxDrawDown
                + "\r\n" + "\r\n" + "参数包含: frequency，numbers，lossPercent，K1，K2";
            //生成图像
            PLChart plc = new PLChart(line, datestr, formTitle: formTitle);
            //运行图像
            Application.Run(plc);
            //保存图像
            plc.SaveZed(GetType().FullName, underlying, startDate, endDate, myStgStats.netProfit.ToString(), myStgStats.anualSharpe.ToString(), myStgStats.maxDrawDown.ToString());
            //Application.Run(new PLChart(line, datestr));
        }
    }
}
