using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.OptionUtils_50ETF
{
    static class OptionUtils_50ETF
    {

        static double standardContractMultiplier = 10000;
        /// <summary>
        /// 根据合约代码，返回合约信息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static OptionInfo getOptionByCode(List<OptionInfo> list, string code)
        {
            foreach (var item in list)
            {
                if (item.optionCode == code)
                {
                    return item;
                }
            }
            return new OptionInfo();
        }


        /// <summary>
        /// 根据给定的条件，查找对应期权的合约代码
        /// </summary>
        /// <param name="list">期权合约列表</param>
        /// <param name="endDate">到期时间</param>
        /// <param name="type">认购还是认沽</param>
        /// <param name="strike">行权价格</param>
        /// <returns>满足条件的期权合约列表</returns>
        public static List<OptionInfo> getSpecifiedOption(List<OptionInfo> list, DateTime endDate, string type, double strike)
        {
            return list.FindAll(delegate (OptionInfo info)
            {
                if (info.optionType == type && info.strike == strike && info.endDate == endDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// 将期权合约列表按到期时间升序排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<DateTime> getEndDateListByAscending(List<OptionInfo> list)
        {
            List<DateTime> durationList = new List<DateTime>();
            foreach (var item in list)
            {
                if (durationList.Contains(item.endDate) == false)
                {
                    durationList.Add(item.endDate);
                }
            }
            return durationList.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 按期权合约列表按行权价升序排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<double> getStrikeListByAscending(List<OptionInfo> list)
        {
            List<double> durationList = new List<double>();
            foreach (var item in list)
            {
                if (durationList.Contains(item.strike) == false)
                {
                    durationList.Add(item.strike);
                }
            }
            return durationList.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 将上市的期权合约到期时间按升序排序输出
        /// </summary>
        /// <param name="list">期权合约列表</param>
        /// <param name="today">今日日期</param>
        /// <returns>到日期列表(当月，下月，季月，下季月)</returns>
        public static List<double> getDurationStructure(List<OptionInfo> list, DateTime today)
        {
            List<double> durationList = new List<double>();
            foreach (var item in list)
            {
                if (item.startDate <= today && item.endDate >= today)
                {
                    double duration = DateUtils.GetSpanOfTradeDays(today, item.endDate);
                    if (durationList.Contains(duration) == false && duration >= 0)
                    {
                        durationList.Add(duration);
                    }
                }
            }
            return durationList.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 给定期权合约对应的IH合约代码
        /// </summary>
        /// <param name="info">期权合约信息</param>
        /// <param name="date">当日日期</param>
        /// <returns>IH合约代码，如果不存在对用的IH合约返回null</returns>
        public static string getCorrespondingIHCode(OptionInfo info, int date)
        {

            DateTime today = Kit.ToDate(date);
            if (info.endDate < today || date < 20150416)
            {
                return null;
            }
            if (Kit.ToInt_yyyyMMdd(info.endDate) <= 20150430 && Kit.ToInt_yyyyMMdd(info.endDate) >= 20150401)
            {
                return "IH1505.CFE";
            }
            DateTime IHExpirationDate = DateUtils.NextOrCurrentTradeDay(DateUtils.GetThirdFridayOfMonth(info.endDate));

            if (today <= IHExpirationDate)
            {
                return "IH" + IHExpirationDate.ToString("yyMM") + ".CFE";
            }
            else
            {
                return "IH" + IHExpirationDate.AddMonths(1).ToString("yyMM") + ".CFE";
            }

        }

        /// <summary>
        /// 按期权是认购还是认沽来筛选合约列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByOptionType(List<OptionInfo> list, string type)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (item.optionType == type)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }
        /// <summary>
        /// 按期权行权价来筛选合约列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="strikeLower"></param>
        /// <param name="strikeUpper"></param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByStrike(List<OptionInfo> list, double strikeLower, double strikeUpper)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (item.strike >= strikeLower && item.strike <= strikeUpper)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }

        /// <summary>
        /// 给定行权价，找到对应的期权合约信息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="strike"></param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByStrike(List<OptionInfo> list, double strike)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (item.strike == strike)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }

        /// <summary>
        /// 根据时间段来筛选上市的期权合约列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="firstDay">开始时间</param>
        /// <param name="lastDay">结束时间</param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByDate(List<OptionInfo> list, int firstDay, int lastDay)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (Convert.ToInt32(item.startDate.ToString("yyyyMMdd")) <= lastDay && Convert.ToInt32(item.endDate.ToString("yyyyMMdd")) >= firstDay)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }

        /// <summary>
        /// 根据日期来筛选上市的期权合约列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByDate(List<OptionInfo> list, int date)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (Convert.ToInt32(item.startDate.ToString("yyyyMMdd")) <= date && Convert.ToInt32(item.endDate.ToString("yyyyMMdd")) >= date)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }

        /// <summary>
        /// 根据到期时间来筛选期权合约
        /// </summary>
        /// <param name="list"></param>
        /// <param name="today"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static List<OptionInfo> getOptionListByDuration(List<OptionInfo> list, DateTime today, double duration)
        {
            return list.FindAll(delegate (OptionInfo item)
            {
                if (DateUtils.GetSpanOfTradeDays(today, item.endDate) == duration)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            );
        }


        /// <summary>
        /// 获取当日上市的合约信息
        /// </summary>
        /// <param name="list">期权合约列表</param>
        /// <param name="today">当日日期</param>
        /// <returns>期权合约列表</returns>
        public static List<OptionInfo> getUnmodifiedOptionInfoList(List<OptionInfo> list, DateTime today)
        {
            List<OptionInfo> listUnmodified = new List<OptionInfo>();
            foreach (var option in list)
            {
                var item = option;
                if (item.startDate <= today && item.endDate >= today)
                {
                    if (item.modifiedDate > today)
                    {
                        item.strike = item.strikeBeforeModified;
                        item.contractMultiplier = standardContractMultiplier;
                    }
                    listUnmodified.Add(item);
                }
            }
            return listUnmodified;
        }


        /// <summary>
        /// 根据给定的期权，得到他对应的期权。看涨期权给出对应的看跌期权，看跌去期权给出其对应的看涨期权。
        /// </summary>
        /// <param name="list">期权备选列表</param>
        /// <param name="optionSelected">给定的期权</param>
        /// <returns></returns>
        public static OptionInfo getCallByPutOrPutByCall(List<OptionInfo> list, OptionInfo optionSelected)
        {
            OptionInfo optionChosen = new OptionInfo();
            foreach (var option in list)
            {
                if (option.endDate == optionSelected.endDate && option.strike == optionSelected.strike && option.contractMultiplier == optionSelected.contractMultiplier && option.optionType != optionSelected.optionType)
                {
                    optionChosen = option;
                    break;
                }

            }
            return optionChosen;
        }
    }
}
