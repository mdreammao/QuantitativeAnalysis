using QuantitativeAnalysis.DataAccessLayer.Common;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common
{
    public abstract class DataFromWindRepository<T>
    {
        /// <summary>
        /// 从wind获得数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="tag">标记</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public abstract List<T> readFromWind(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null);

    }
}
