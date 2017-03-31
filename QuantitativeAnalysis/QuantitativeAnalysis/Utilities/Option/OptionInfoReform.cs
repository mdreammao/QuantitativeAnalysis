
using QuantitativeAnalysis.ModelLayer.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Option
{
    public class OptionInfoReform
    {
        /// <summary>
        /// 重组期权列表，按code构造字典结构的期权info列表。
        /// </summary>
        /// <param name="optionInfoList">期权合约基本信息列表</param>
        /// <returns></returns>
        public static Dictionary<string, OptionInfo> ReformByCode(List<OptionInfo> optionInfoList)
        {
            Dictionary<string, OptionInfo> dic = new Dictionary<string, OptionInfo>();
            foreach (var item in optionInfoList)
            {
                dic.Add(item.optionCode, item);
            }
            return dic;
        }
    }
}
