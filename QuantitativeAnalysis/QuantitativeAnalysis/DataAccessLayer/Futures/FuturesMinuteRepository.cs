
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.DataAccessLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;
using QuantitativeAnalysis.ServiceLayer.Core;

namespace QuantitativeAnalysis.DataAccessLayer.Futures
{
    public class FuturesMinuteRepository : SequentialByDayRepository<FuturesMinute>
    {
        protected override List<FuturesMinute> readFromDefaultMssql(string code, DateTime date)
        {
            throw new NotImplementedException();
        }

        protected override List<FuturesMinute> readFromWind(string code, DateTime date)
        {
            List<FuturesMinute> items = new List<FuturesMinute>();
            string[] str = code.Split('.');

            //CFE中国期货交易所
            if (str[1] == "CFE")//CFE中国期货交易所
            {
                DateTime modifiedDate1 = new DateTime(2015, 12, 31);
                if (date <= modifiedDate1)
                {
                    return readByParameters(code, date, "periodstart=09:15:00;periodend=15:15:00");
                }
                else
                {
                    return readByParameters(code, date, "periodstart=09:30:00;periodend=15:00:00");
                }

            }

            //DCE大连商品交易所
            if (str[1] == "DCE") //DCE大连商品交易所 目前所有品种交易时间最晚时间为2点半
            {
                DateTime modifiedDate1 = new DateTime(2015, 5, 8);
                if (date <= modifiedDate1)
                {
                    var nightData1 = readByParameters(code, date, "periodstart=21:00:00;periodend=23:59:59");
                    var nightData2 = readByParameters(code, date, "periodstart=00:00:00;periodend=2:30:00");
                    var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                    nightData1.AddRange(nightData2);
                    nightData1.AddRange(dayData);
                    return nightData1;
                }
                else
                {
                    var nightData = readByParameters(code, date, "periodstart=21:00:00;periodend=23:30:00");
                    var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                    nightData.AddRange(dayData);
                    return nightData;
                }
            }

            //CZC郑州商品交易所
            if (str[1] == "CZC")
            {
                var nightData = readByParameters(code, date, "periodstart=21:00:00;periodend=23:30:00");
                var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                nightData.AddRange(dayData);
                return nightData;
            }

            //SHF上海期货交易所
            if (str[0].IndexOf("RB") > -1 && str[1] == "SHF")//SHF上海期货交易所
            {
                DateTime modifiedDate1 = new DateTime(2014, 12, 26);
                DateTime modifiedDate2 = new DateTime(2016, 5, 3);
                if (date <= modifiedDate1)
                {
                    return readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                }
                else if (date <= modifiedDate2)
                {
                    var nightData1 = readByParameters(code, date, "periodstart=21:00:00;periodend=23:59:59");
                    var nightData2 = readByParameters(code, date, "periodstart=00:00:00;periodend=1:00:00");
                    var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                    nightData1.AddRange(nightData2);
                    nightData1.AddRange(dayData);
                    return nightData1;
                }
                else
                {
                    var nightData = readByParameters(code, date, "periodstart=21:00:00;periodend=23:00:00");
                    var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                    nightData.AddRange(dayData);
                    return nightData;
                }

            }
            if (str[0].IndexOf("AU") > -1 && str[1] == "SHF")
            {
                var nightData1 = readByParameters(code, date, "periodstart=21:00:00;periodend=23:59:59");
                var nightData2 = readByParameters(code, date, "periodstart=00:00:00;periodend=2:29:59");
                var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                nightData1.AddRange(nightData2);
                nightData1.AddRange(dayData);
                return nightData1;
            }
            if (str[0].IndexOf("NI") > -1 && str[1] == "SHF")
            {
                var nightData1 = readByParameters(code, date, "periodstart=21:00:00;periodend=23:59:59");
                var nightData2 = readByParameters(code, date, "periodstart=00:00:00;periodend=1:00:00");
                var dayData = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
                nightData1.AddRange(nightData2);
                nightData1.AddRange(dayData);
                return nightData1;
            }
            items = readByParameters(code, date, "periodstart=09:00:00;periodend=15:00:00");
            return items;
        }

        private List<FuturesMinute> readByParameters(string code, DateTime date, string paramters)
        {
            WindAPI w = Platforms.GetWindAPI();
            DateTime date2 = new DateTime(date.Year, date.Month, date.Day, 15, 0, 0);
            DateTime date1 = DateUtils.PreviousTradeDay(date).AddHours(17);
            //获取日盘数据
            WindData wd = w.wsi(code, "open,high,low,close,volume,amt,oi", date1, date2, paramters);
            int len = wd.timeList.Length;
            int fieldLen = wd.fieldList.Length;
            var items = new List<FuturesMinute>(len);
            if (wd.data is double[])
            {
                double[] dataList = (double[])wd.data;
                DateTime[] timeList = wd.timeList;
                for (int k = 0; k < len; k++)
                {
                    items.Add(new FuturesMinute
                    {
                        tradeday = date,
                        time = timeList[k],
                        open = (double)dataList[k * fieldLen + 0],
                        high = (double)dataList[k * fieldLen + 1],
                        low = (double)dataList[k * fieldLen + 2],
                        close = (double)dataList[k * fieldLen + 3],
                        volume = (double)dataList[k * fieldLen + 4],
                        amount = (double)dataList[k * fieldLen + 5],
                        openInterest = (double)dataList[k * fieldLen + 6]
                    });
                }
            }

            //【原版】如果该时间段第1个时间点的close为NAN，则放弃该时间段的所有数据
            //if (items.Count>0 && double.IsNaN(items[0].close)==true)
            //{
            //    return new List<FuturesMinute>();
            //}

            //判断该时间段前25条数据是否含有真正的数据(至少一条数据)
            List<FuturesMinute> tempItem = items.GetRange(0, 25);
            bool haveData = items.Any(x => double.IsNaN(x.close) != true);

            //【新版1】如果该时间段前5个时间点的close为NAN，则放弃该时间段的所有数据
            //if (items.Count > 0 && double.IsNaN(items[0].close) && double.IsNaN(items[1].close) && 
            //    double.IsNaN(items[2].close) && double.IsNaN(items[3].close) && double.IsNaN(items[4].close))
            //{
            //    return new List<FuturesMinute>();
            //}

            //【新版2】如果该时间段前20个时间点的close为NAN，则放弃该时间段的所有数据
            if (items.Count > 0 && haveData == false)
            {
                return new List<FuturesMinute>();
            }


            return items;
        }
    }
}
    