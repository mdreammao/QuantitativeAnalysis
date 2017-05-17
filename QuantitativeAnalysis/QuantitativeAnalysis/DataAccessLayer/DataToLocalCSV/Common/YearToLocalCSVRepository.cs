using QuantitativeAnalysis.ModelLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common
{
    public abstract class YearToLocalCSVRepository<T> : DataToLocalCSVRepository<T> where T : Sequential, new()
    {
         
    }
}
