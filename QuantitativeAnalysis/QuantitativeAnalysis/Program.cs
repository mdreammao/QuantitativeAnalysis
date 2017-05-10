using Autofac;
using BackTestingPlatform.Strategies.Futures.XiaoLong;
using QuantitativeAnalysis.ServiceLayer.Core;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {

            Platforms.Initialize(); //初始化
            // LocalRunner runner = new LocalRunner();
            // runner.run();
            (new LocalRunner()).run();
            Platforms.ShutDown();   //关闭
            //test();

        }

        /*static void test() {
            int number = 5;
            List<double> list = new List<double>();
            list.Add(2);
            list.Add(3);
            list.Add(1);
            list.Add(5);
            list.Add(4);

            //if (list.Count == 5) {
            //list.RemoveAt(0);
            //list.Insert(number-1, 6);
            //}
            for (int i = 0; i < 5; i++) {
                Console.WriteLine(list[i]);
            }
            //Console.WriteLine(list[1]);
            //Console.WriteLine(list[2]);
        }*/
    }


    class LocalRunner
    {
        public void run()
        {
            //double[,] A = new double[2, 2] { { 3, -1 }, { 1, -3 } };
            //double[] b = new double[2] { 1, 1 };
            //var x = MatrixInverse.getInverse(A, b);
            //readFromDataYes.getData("D:\\BTP\\OtherSource\\kline_dce_i.csv", "kline_dce_i.csv");
            DateTime now = DateTime.Now;
            //var data = Platforms.container.Resolve<FuturesMinuteRepository>().fetchFromLocalCsvOrWindAndSave("RB.SHF", Kit.ToDate(20161001));
            //EfficiencyRatio test = new EfficiencyRatio(20140101, 20161209, "RB.SHF", frequency:5,numbers:6,longLevel:0.75,shortLevel:-0.75);
            //StraddleWithHedge2 test = new StraddleWithHedge2(20150210, 20161206, 20);
            //var paras = new EfficiencyRatioWithParametersChoice(20150101, 20170218, "RB.SHF", 60, 20);
            //var test = new EfficiencyRatioWithSpecifiedParametres(20150101, 20170218, "RB.SHF", paras.parameters);
            var test = new DualThrustTest(20150101, 20170509, "RB.SHF");
            DateTime now2 = DateTime.Now;
            Console.WriteLine(now2 - now);
        }
    }
}
