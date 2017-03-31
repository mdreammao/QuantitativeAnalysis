using System;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using NLog;
using System.Collections.Generic;
using Autofac;
using System.Configuration;
using System.IO;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common;

namespace QuantitativeAnalysis.PresentationLayer
{
    public partial class PLChart : Form
    {
        private ZedGraphControl zedG;

        //Image对象，保存图片使用
        private Image imageZed;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        //PL曲线输入字典
        private Dictionary<string, double[]> lineChart = new Dictionary<string, double[]>();
        private string[] date = { };
        private string formTitleString=String.Empty;
        private string XAxisTitleString = String.Empty;
        private string YAxisTitleString = String.Empty;

        //颜色模板列表
        List<ColorTemplate> colorList=new List<ColorTemplate>(); 


        public PLChart(Dictionary<string, double[]> line, string[] datePeriod,string formTitle="净值曲线",string XAxisTitle="时间",string YAxisTitle="净值")
        {
            InitializeComponent();
            lineChart = line;
            date = datePeriod;
            formTitleString = formTitle;
            XAxisTitleString = XAxisTitle;
            YAxisTitleString = YAxisTitle;

            //自定义配色方案，用于曲线的颜色设置（三个参数必须在0~255之间）
            colorList.Add(new ColorTemplate(249,108,216));//玫红色
            colorList.Add(new ColorTemplate(135,222,213));//深绿蓝色
            colorList.Add(new ColorTemplate(247,183,110));//橙色
            colorList.Add(new ColorTemplate(191,114,243));//紫色
            colorList.Add(new ColorTemplate(164, 233, 124));//浅绿色

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            //显示的属性设置，后期还要做美工处理**************
            zedG = new ZedGraphControl();
            SuspendLayout();

            // 图片属性设置 
            zedG.IsShowPointValues = false;
            zedG.Location = new Point(0, 0);
            zedG.Name = "zedG";
            zedG.PointValueFormat = "G";
            zedG.Size = new Size(1360, 764);
            zedG.TabIndex = 0;

            // Form属性设置
            AutoScaleBaseSize = new Size(10, 24);
            ClientSize = new Size(923, 538);
            Controls.Add(zedG);
            Name = "Form1";
            Text = "Form1";
            Load += new EventHandler(Form_Load);
            ResumeLayout(false);
        }

        /// <summary>
        /// 【原版】窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Load(object sender, EventArgs e)
        {
            MasterPane myPaneMaster = zedG.MasterPane;
            myPaneMaster.Title.Text = "NetWorth";
            myPaneMaster.Title.FontSpec.FontColor = Color.Black;

            GraphPane myPane = zedG.GraphPane;
            myPaneMaster.PaneList[0] = (myPane);

            //    //画一张的小图
            //GraphPane paneStats = new GraphPane(new Rectangle(10, 10, 10, 10), "Mes", " t ( h )", "Rate");
            //myPaneMaster.PaneList.Add(paneStats);

            //GraphPane paneStats = new GraphPane(new Rectangle(10, 10, 50, 50), "Mes", " t ( h )", "Rate");
            //myPaneMaster.PaneList[1]=(paneStats);

            LineItem[] myCurve = new LineItem[lineChart.Count];

            //建立indexD变量，索引myCurve变量
            int indexD = 0;

            #region 设置曲线颜色：随机模式【请勿删除！】
            //建立Random变量用于控制颜色变化
            //Random aa = new Random();
            //foreach (var variety in lineChart)
            //{
            //    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
            //        Color.FromArgb(aa.Next(1, 255), aa.Next(1, 255), aa.Next(1, 255)), SymbolType.None);
            //    myCurve[indexD].Symbol.Size = 8.0F;
            //    myCurve[indexD].Symbol.Fill = new Fill(Color.White);
            //    myCurve[indexD].Line.Width = 2.0F;
            //    ++indexD;
            //}
            #endregion

            #region 设置曲线颜色：自定义模式+随机模式
            //建立Random变量用于控制颜色变化
            Random aa = new Random();
            foreach (var variety in lineChart)
            {
                //如果当前索引值indexD小于用户自定义颜色模板的个数，则使用用户定义的颜色
                if (indexD <= colorList.Count - 1)
                {
                    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
                        Color.FromArgb(colorList[indexD].Red, colorList[indexD].Green, colorList[indexD].Blue), SymbolType.None);
                }
                //如果当前索引值indexD大于用户自定义颜色模板的个数，则随机生成颜色
                else
                {
                    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
                    Color.FromArgb(aa.Next(1, 255), aa.Next(1, 255), aa.Next(1, 255)), SymbolType.None);
                }
                
                myCurve[indexD].Symbol.Size = 8.0F;
                myCurve[indexD].Symbol.Fill = new Fill(Color.White);
                myCurve[indexD].Line.Width = 2.0F;
                ++indexD;
            }
            #endregion

            // Draw the X tics between the labels instead of at the labels
            //myPane.XAxis.IsTicsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = date;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            //设置X轴和Y轴的名称
            myPane.XAxis.Title.Text =XAxisTitleString;//X轴
            myPane.YAxis.Title.Text = YAxisTitleString;//Y轴

            //设置图的title
            myPane.Title.Text = formTitleString;

            

            // Fill the axis area with a gradient
            //myPane.AxisFill = new Fill(Color.White,
            //Color.FromArgb(255, 255, 166), 90F);
            // Fill the pane area with a solid color
            //myPane.PaneFill = new Fill(Color.FromArgb(250, 250, 255));

            //绩效指标图统计

            zedG.AxisChange();
            imageZed = zedG.GetImage();
        }

        /// <summary>
        /// 【新版】窗体加载
        /// </summary>
        public void LoadForm()
        {
            MasterPane myPaneMaster = zedG.MasterPane;
            myPaneMaster.Title.Text = "NetWorth";
            myPaneMaster.Title.FontSpec.FontColor = Color.Black;

            GraphPane myPane = zedG.GraphPane;
            myPaneMaster.PaneList[0] = (myPane);

            //    //画一张的小图
            //    GraphPane paneStats = new GraphPane(new Rectangle(10, 10, 10, 10), "Mes", " t ( h )", "Rate");
            //    myPaneMaster.PaneList.Add(paneStats);

            //GraphPane paneStats = new GraphPane(new Rectangle(10, 10, 50, 50), "Mes", " t ( h )", "Rate");
            //myPaneMaster.PaneList.Add(paneStats);

            LineItem[] myCurve = new LineItem[lineChart.Count];

            //建立indexD变量，索引myCurve变量
            int indexD = 0;

            #region 设置曲线颜色：随机模式【请勿删除！】
            //建立Random变量用于控制颜色变化
            //Random aa = new Random();
            //foreach (var variety in lineChart)
            //{
            //    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
            //        Color.FromArgb(aa.Next(1, 255), aa.Next(1, 255), aa.Next(1, 255)), SymbolType.None);
            //    myCurve[indexD].Symbol.Size = 8.0F;
            //    myCurve[indexD].Symbol.Fill = new Fill(Color.White);
            //    myCurve[indexD].Line.Width = 2.0F;
            //    ++indexD;
            //}
            #endregion

            #region 设置曲线颜色：自定义模式+随机模式
            //建立Random变量用于控制颜色变化
            Random aa = new Random();
            foreach (var variety in lineChart)
            {
                //如果当前索引值indexD小于用户自定义颜色模板的个数，则使用用户定义的颜色
                if (indexD <= colorList.Count - 1)
                {
                    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
                        Color.FromArgb(colorList[indexD].Red, colorList[indexD].Green, colorList[indexD].Blue), SymbolType.None);
                }
                //如果当前索引值indexD大于用户自定义颜色模板的个数，则随机生成颜色
                else
                {
                    myCurve[indexD] = myPane.AddCurve(variety.Key, null, lineChart[variety.Key],
                    Color.FromArgb(aa.Next(1, 255), aa.Next(1, 255), aa.Next(1, 255)), SymbolType.None);
                }

                myCurve[indexD].Symbol.Size = 8.0F;
                myCurve[indexD].Symbol.Fill = new Fill(Color.White);
                myCurve[indexD].Line.Width = 2.0F;
                ++indexD;
            }
            #endregion

            // Draw the X tics between the labels instead of at the labels
            //myPane.XAxis.IsTicsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = date;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            //设置X轴和Y轴的名称
            myPane.XAxis.Title.Text = XAxisTitleString;//X轴
            myPane.YAxis.Title.Text = YAxisTitleString;//Y轴

            //设置图的title
            myPane.Title.Text = formTitleString;



            // Fill the axis area with a gradient
            //myPane.AxisFill = new Fill(Color.White,
            //Color.FromArgb(255, 255, 166), 90F);
            // Fill the pane area with a solid color
            //myPane.PaneFill = new Fill(Color.FromArgb(250, 250, 255));

            //绩效指标图统计

            zedG.AxisChange();
            imageZed = zedG.GetImage();
        }

        //备份：之前版本的图片保存
        //public void SaveZed(string path)
        //{
        //    imageZed.Save(path);
        //}

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="underlying"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="netProfit"></param>
        /// <param name="anualSharp"></param>
        /// <param name="MDD"></param>
        public void SaveZed(string tag,string underlying,DateTime start,DateTime end,string netProfit,string anualSharp,string MDD)
        {
            //从配置文件读出来的“基本路径”
            var fullPath= ConfigurationManager.AppSettings["CacheData.ResultPath"] + ConfigurationManager.AppSettings["CacheData.ImagePath"];
            
            //程序运行时间（作为文件夹的名称）
            var todayDate= Kit.ToInt_yyyyMMdd(DateTime.Now).ToString()+"_image";
            
            var startDate= Kit.ToInt_yyyyMMdd(start).ToString();//开始时间
            var endDate= Kit.ToInt_yyyyMMdd(end).ToString();//结束时间

            //得到真正的本地保存路径
            fullPath = ResultPathUtil.GetImageLocalPath(fullPath, tag, todayDate, underlying, startDate, endDate, netProfit,
                anualSharp, MDD);

            //若文件路径不存在则生成该文件夹
            var dirPath = Path.GetDirectoryName(fullPath);
            if (dirPath != "" && !Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            imageZed.Save(fullPath);
            //imageZed.Save(path); 
        }
    }

    /// <summary>
    /// 颜色模板
    /// </summary>
    public class ColorTemplate
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public ColorTemplate(int red,int green,int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}
