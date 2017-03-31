
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ModelLayer.Common
{
    /// <summary>
    /// 股票盘口价格的格式，不定长度的N挡盘口价量
    /// </summary>
    public struct Position
    {
        public double price, volume;
        public Position(double price, double volume)
        {
            this.price = price;
            this.volume = volume;
        }

        public override string ToString()
        {
            return String.Format("p={0} v={1}", this.price, this.volume);
        }

        public static Position[] build(DataRow row, int size, string priceColName, string volumeColName)
        {
            return null;    //TODO
        }

        /// <summary>
        /// 获取5档盘口价格
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Position[] buildAsk5(DataRow row)
        {
            Position[] res = new Position[5];
            res[0] = new Position(Convert.ToDouble(row["S1"]), Convert.ToDouble(row["SV1"]));
            res[1] = new Position(Convert.ToDouble(row["S2"]), Convert.ToDouble(row["SV2"]));
            res[2] = new Position(Convert.ToDouble(row["S3"]), Convert.ToDouble(row["SV3"]));
            res[3] = new Position(Convert.ToDouble(row["S4"]), Convert.ToDouble(row["SV4"]));
            res[4] = new Position(Convert.ToDouble(row["S5"]), Convert.ToDouble(row["SV5"]));
            return res;
        }
        public static Position[] buildBid5(DataRow row)
        {
            Position[] res = new Position[5];
            res[0] = new Position(Convert.ToDouble(row["B1"]), Convert.ToDouble(row["BV1"]));
            res[1] = new Position(Convert.ToDouble(row["B2"]), Convert.ToDouble(row["BV2"]));
            res[2] = new Position(Convert.ToDouble(row["B3"]), Convert.ToDouble(row["BV3"]));
            res[3] = new Position(Convert.ToDouble(row["B4"]), Convert.ToDouble(row["BV4"]));
            res[4] = new Position(Convert.ToDouble(row["B5"]), Convert.ToDouble(row["BV5"]));
            return res;
        }
        /// <summary>
        /// 获取一档盘口价格
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Position[] buildAsk(DataRow row)
        {
            Position[] res = new Position[5];
            res[0] = new Position(Convert.ToDouble(row["S1"]), Convert.ToDouble(row["SV1"]));
            return res;
        }
        public static Position[] buildBid(DataRow row)
        {
            Position[] res = new Position[5];
            res[0] = new Position(Convert.ToDouble(row["B1"]), Convert.ToDouble(row["BV1"]));
            return res;
        }
    }
}
