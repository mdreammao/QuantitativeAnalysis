
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

using System.Text;
using System.Threading.Tasks;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Option;
using QuantitativeAnalysis.ModelLayer.PositionModel;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.Utilities.Futures;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Option;

namespace QuantitativeAnalysis.Utilities.AccountOperator.Minute
{
    public class AccountUpdatingWithMinuteBar
    {
        //初始化log组件
        static Logger log = LogManager.GetCurrentClassLogger();
        static Dictionary<string, OptionInfo> optionInfoList = OptionInfoReform.ReformByCode(Platforms.container.Resolve<OptionInfoService>().fetchFromLocalCsvOrWindAndSaveAndCache(localCsvExpiration:0,tag:"50ETFOptionInfo"));
        public static void computeAccount(ref BasicAccount myAccount, SortedDictionary<DateTime, Dictionary<string, PositionsWithDetail>> positions, DateTime now, int nowIndex, Dictionary<string, List<KLine>> data)
        {
            myAccount.time = now;
            //若position为null，直接跳过
            if (positions.Count == 0)
            {
                return;
            }
            //提取初始资产
            double initialCapital = myAccount.initialAssets;
            Dictionary<string, PositionsWithDetail> nowPosition = new Dictionary<string, PositionsWithDetail>();
            nowPosition = positions[positions.Keys.Last()];
            //初始化保证金，可用现金
            double totalMargin = 0;
            double totalCashFlow = 0;
            double totalPositionValue = 0;
            double totalAssets = 0;
            //当前时间对应data中timeList 的序号
            int index = nowIndex;
            if (index < 0)
            {
                log.Warn("Signal时间出错，请查验");
                return;
            }
            foreach (var item in nowPosition)
            {
                PositionsWithDetail position0 = item.Value;
                if (position0.volume != 0)
                {
                    double price = data[position0.code][index].close;
                    totalPositionValue += price * position0.volume;
                }
                if (position0.volume < 0) //计算保证金
                {
                    if (position0.tradingVarieties == "option") //按每分钟收盘价来近似期权的保证金
                    {
                        totalMargin += (OptionMargin.ComputeMaintenanceMargin(data["510050.SH"][index].close, data[position0.code][index].close, optionInfoList[position0.code].strike, optionInfoList[position0.code].optionType, Math.Abs(position0.volume)));
                    }
                    else if (position0.tradingVarieties == "stock") //股票卖空按照一半保证金计算
                    {
                        totalMargin += 0.5 * data[position0.code][index].close * Math.Abs(position0.volume);
                    }
                    else if (position0.tradingVarieties == "futures")
                    {
                        totalMargin += FuturesMargin.ComputeOpenMargin(data[position0.code][index].close, 0.4, position0.volume);
                    }
                }
                totalCashFlow += position0.totalCashFlow;
            }
            myAccount.totalAssets = initialCapital + totalCashFlow + totalPositionValue;
            myAccount.freeCash = initialCapital + totalCashFlow - totalMargin;
            myAccount.margin = totalMargin;
            myAccount.positionValue = totalPositionValue;

        }
    }
}
