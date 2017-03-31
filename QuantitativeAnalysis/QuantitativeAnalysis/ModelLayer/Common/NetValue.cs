using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    /// <summary>
    /// 记录净值曲线的数据结构，为了方便的作图
    /// </summary>
    public class NetValue
    {
        public DateTime time { get; set; }
        public double netvalueReturn { get; set; }
        public double benchmarkReturn { get; set; }
        public double netvalue { get; set; }
        public double benchmark { get; set; }
    }
}
