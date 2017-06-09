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
            if (list==null)
            {
                return null;
            }
            int last = 0;
            filterList.Add(list[0]);

            for (int i = 1; i < list.Count(); i++)
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
            return filterList;
        }
    }
}
