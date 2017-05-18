using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    public class Factor : Sequential
    {
        public string code { get; set;}
        public DateTime time { get; set; }
    }
}
