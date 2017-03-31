using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.Utilities.Common
{
    /// <summary>
    /// DataTable相关工具类
    /// </summary>
    public static class DataTableUtils
    {
        /// <summary>
        /// from http://stackoverflow.com/questions/18100783/how-to-convert-a-list-into-data-table
        /// see also:https://social.msdn.microsoft.com/Forums/vstudio/en-US/6ffcb247-77fb-40b4-bcba-08ba377ab9db/converting-a-list-to-datatable?forum=csharpgeneral
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> items)
        {
            return ToDataTable(items, toColumnsDefaultFunc, toRowValuesDefaultFunc);
        }
        public static DataTable ToDataTable<T>(IList<T> items, Func<Type, DataColumn[]> toColumnsFunc, Func<T, object[]> toRowValuesFunc)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //build columns
            var cols = toColumnsFunc(typeof(T));
            dataTable.Columns.AddRange(cols);

            //fill rows
            foreach (T item in items)
            {
                dataTable.Rows.Add(toRowValuesFunc(item));
            }

            return dataTable;
        }

        public static DataColumn[] toColumnsDefaultFunc(Type t)
        {
            //Setting column names as Property names
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray();
        }

        public static object[] toRowValuesDefaultFunc<T>(T t)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (t == null) return new object[props.Length];
            var values = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                //Type propType = props[i].PropertyType;
                values[i] = props[i].GetValue(t);
            }
            return values;
        }

        static bool IsIEnumerable(Type t)
        {
            return t.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            if (table == null) return null;
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<T> result = new List<T>(table.Rows.Count);
            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }
            return result;
        }

        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return CreateItemFromRow<T>(row, properties);
        }

        /// <summary>
        /// DataRow -> Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            object entity = new T();    // box the instance if T is struct
            Type type = typeof(T);
            foreach (var prop in properties)
            {
                object val = row[prop.Name];
                Type propType = prop.PropertyType;

                if (!propType.IsArray)
                {
                    prop.SetValue(entity, Kit.To(propType, val));
                }
            }
            return (T)entity;
        }


    }
}
