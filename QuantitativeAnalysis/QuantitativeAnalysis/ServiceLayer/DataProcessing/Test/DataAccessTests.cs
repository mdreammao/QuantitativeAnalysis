using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Futures;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Test
{
    class DataAccessTests
    {
        public void testFromLocalCSVRepo()
        {
            var repo = Platforms.container.Resolve<FuturesDailyFromLocalCSVRepository>();
        }
    }
}
