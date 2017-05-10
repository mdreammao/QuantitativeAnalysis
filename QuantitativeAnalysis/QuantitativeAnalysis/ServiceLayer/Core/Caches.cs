using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.Core
{
    /// <summary>
    /// 作用于全局的变量缓存池
    /// </summary>
    public class Caches
    {
        static IDictionary<string, object> data = new Dictionary<string, object>();

        public static void put(string key, object val)
        {
            data[key] = val;
        }

        public static object get(string key)
        {
            return data[key];
        }

        public static T get<T>(string key)
        {
            return (T)data[key];
        }

        public static IDictionary<string, object> getAll()
        {
            return data;
        }

        //------ 以下是一些常用的标记 ---------
        public static bool WindConnection=false;
        public static bool MSSQLConnection = false;

        //------ 以下是一些常用的变量get函数 ---------
        public static List<DateTime> getTradeDays()
        {
            return get<List<DateTime>>("TradeDays");
        }


    }
}
