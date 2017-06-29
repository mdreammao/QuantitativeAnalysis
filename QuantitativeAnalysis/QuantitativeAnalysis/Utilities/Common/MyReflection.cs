using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    public static class MyReflection
    {
        public static List<string> getPropertyName(Type t)
        {
            List<string> name = new List<string>();
            foreach (var item in t.GetProperties())
            {
                name.Add(item.Name);
            }
            return name;
        }
    }
}
