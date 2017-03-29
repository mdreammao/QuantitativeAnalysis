using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Option
{
    public struct OptionInfo
    {
        public string optionCode { get; set; }
        public string optionName { get; set; }
        public string executeType { get; set; }
        public double strike { get; set; }
        public string optionType { get; set; }
        public double contractMultiplier { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime modifiedDate { get; set; }
        public double strikeBeforeModified { get; set; }
    }
}
