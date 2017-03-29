using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    /// <summary>
    /// 有时间序列特征的
    /// </summary>
    public interface Sequential
    {
        DateTime time { get; set; }
    }
}