using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.DataApplication
{
    public class ResultPathUtil
    {
        //获得将从wind拉取的数据保存在本地CSV的本地路径
        public static string GetLocalPath(string fullPath, string tag, string dateStr, string type, string parameters, string performance)
        {
            return fullPath.Replace("{tag}", tag).Replace("{date}", dateStr).Replace("{parameters}", parameters).Replace("{type}", type).Replace("{performance}", performance);
        }

        //获得储存图片的本地路径
        public static string GetImageLocalPath(string fullPath, string tag, string dateStr, string underlying, string startDate, string endDate, string netProfit, string anualSharp, string MDD)
        {
            string resultPath = String.Empty;
            resultPath = fullPath.Replace("{tag}", tag).Replace("{date_image}", dateStr).Replace("{underlying}", underlying).Replace("{startDate}", startDate).
                Replace("{endDate}", endDate).Replace("{netProfit}", netProfit).Replace("{anualSharp}", anualSharp).Replace("{MDD}", MDD);
            return resultPath;
        }

    }
}
