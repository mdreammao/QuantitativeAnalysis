using QuantitativeAnalysis.ModelLayer.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.DataApplication
{
    public static class OptionTickDataUtils
    {
        public static List<OptionTickFromMssql> filteringTickData(List<OptionTickFromMssql> list)
        {
            List<OptionTickFromMssql> filterList = new List<OptionTickFromMssql>();
            if (list==null || list.Count==0)
            {
                return null;
            }
            int last = 0;
            filterList.Add(list[0]);
            for (int i = 1; i < list.Count(); i++)
            {
                if (list[i].lastPrice==0 && list[i].ask1==1 && list[i].bid1==0)//删除成交价及盘口价格为0的数据
                {
                    continue;
                }
                if (list[i].ttime<150000000)
                {
                    if (list[i].code == filterList[last].code && list[i].ttime == filterList[last].ttime && list[i].tdate == filterList[last].tdate)
                    {
                        if ((i == list.Count() - 1) || (list[i + 1].tdate > list[i].tdate) || (list[i + 1].ttime > list[i].ttime + 500))
                        {

                            OptionTickFromMssql tickData = list[i];
                            tickData.ttime += 500;
                            filterList.Add(tickData);
                            last++;
                        }
                    }
                    else
                    {
                        filterList.Add(list[i]);
                        last++;
                    }
                }
                else
                {
                    if (filterList[last].ttime>=150000000)
                    {
                        OptionTickFromMssql tickData = list[i];
                        filterList[last] = tickData;
                        filterList[last].ttime = 150000000;
                    }
                    else
                    {
                        OptionTickFromMssql tickData = list[i];
                        tickData.ttime = 150000000;
                        filterList.Add(tickData);
                        last++;
                    }
                }
                
            }
            return filterList;
        }
    }
}
