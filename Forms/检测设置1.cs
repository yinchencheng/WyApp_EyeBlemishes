﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.ObjectModel;
using WY_App.Utility;
using OpenCvSharp.XImgProc;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using HalconDotNet;
using System.Runtime.CompilerServices;
using Sunny.UI;
using OpenCvSharp.Flann;
using System.Numerics;
using OpenCvSharp.Internal.Vectors;
using Sunny.UI.Win32;
using System.Security.Cryptography;
using static WY_App.Utility.Parameter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Common;
using SevenZip.Compression.LZ;

namespace WY_App
{
    public partial class 检测设置1 : Form
    {
        public static int Round_Edge = 0;
        System.Drawing.Point downPoint;
        public static Cricle pointReaultCricle = new Cricle();
        public 检测设置1()
        {
            InitializeComponent();
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);//设置窗体的规格 
            hWindowControl1.HalconWindow.DispObj(MainForm.hImage[0]);
        }
        
        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new System.Drawing.Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new System.Drawing.Point(e.X, e.Y);
        }


        private void btn_Close_System_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_加载检测图片_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();

            if (openfile.ShowDialog() == DialogResult.OK && (openfile.FileName != "")) 
            {
                //picture0.ImageLocation = openfile.FileName;
                //MatImage = Cv2.ImRead(openfile.FileName);
                Halcon.ImgDisplay(0,openfile.FileName, hWindowControl1.HalconWindow,ref MainForm.hImage[0]);;
            }
            openfile.Dispose();
        }


        private void btn_SaveParams_Click(object sender, EventArgs e)
        {
            XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, Parameter.commministion.productName + "/Specifications.xml");
        }

        private void 检测设置_Load(object sender, EventArgs e)
        {
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);//设置窗体的规格 
            hWindowControl1.HalconWindow.DispObj(MainForm.hImage[0]);
            num_AreaHigh2.Value = Parameter.specifications.圆形检测[0].AreaHigh;
            num_AreaLow2.Value = Parameter.specifications.圆形检测[0].AreaLow;
            num_ThresholdHigh2.Value = Parameter.specifications.圆形检测[0].ThresholdHigh;
            num_ThresholdLow2.Value = Parameter.specifications.圆形检测[0].ThresholdLow;
            num_PixelResolution.Value = Parameter.specifications.圆形检测[0].PixelResolution;
            chk_SaveOrigalImage.Checked = Parameter.specifications.SaveOrigalImage;
            chk_SaveDefeatImage.Checked = Parameter.specifications.SaveDefeatImage;        
        }

        private void num_PixelResolution_ValueChanged(object sender, double value)
        {
            Parameter.specifications.圆形检测[0].PixelResolution = num_PixelResolution.Value;
        }

        private void chk_SaveDefeatImage_CheckedChanged(object sender, EventArgs e)
        {
            Parameter.specifications.SaveDefeatImage = chk_SaveDefeatImage.Checked;
        }

        private void chk_SaveOrigalImage_CheckedChanged(object sender, EventArgs e)
        {
            Parameter.specifications.SaveOrigalImage = chk_SaveOrigalImage.Checked;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            HOperatorSet.DispObj(MainForm.hImage[0], hWindowControl1.HalconWindow);
            try
            {
                Halcon.DetectionCricleHObject(0,hWindowControl1.HalconWindow, MainForm.hImage[0], Parameter.specifications.圆形检测[0], Parameter.specifications.圆形检测[0].Row, Parameter.specifications.圆形检测[0].Colum, 50, ref pointReaultCricle);

            }
            catch 
            { 
            
            }
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数           
            time.Text = milliseconds.ToString();
        }

        private void btn_Minimizid_System_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

       

        private void uiButton1_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawCriclaAOI(hWindowControl1.HalconWindow, MainForm.hImage[0], ref Parameter.specifications.圆形检测[0]);
        }

        private void uiButton13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            HOperatorSet.DispObj(MainForm.hImage[0], hWindowControl1.HalconWindow);
            bool DetectionResult = true;
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);
            Detection(hWindowControl1.HalconWindow, MainForm.hImage[0],ref DetectionResult);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数           
            time.Text = milliseconds.ToString();
            

        }
        public static void Detection(HWindow hWindow, HObject hImage, ref bool DetectionResult)
        {
            try
            {
                pointReaultCricle = Parameter.specifications.圆形检测[0];
                Halcon.DetectionCricleHObject(0, hWindow, hImage, Parameter.specifications.圆形检测[0], Parameter.specifications.圆形检测[0].Row, Parameter.specifications.圆形检测[0].Colum, 30, ref pointReaultCricle);
                HOperatorSet.SetTposition(hWindow, pointReaultCricle.Row + pointReaultCricle.Radius, pointReaultCricle.Colum);
                HOperatorSet.WriteString(hWindow, "半径:" + pointReaultCricle.Radius.ToString("0.0000"));
                Halcon.DetectionCricle0(0, hWindow, hImage, pointReaultCricle, ref DetectionResult);
            }
            catch (Exception ex)
            {
                MainForm.AlarmList.Add("检测1Error;" + ex.Message);
                DetectionResult = false;
            }           
        }

        private void uiDoubleUpDown4_ValueChanged(object sender, double value)
        {
            Parameter.specifications.圆形检测[0].ThresholdLow = value;
        }

        private void uiDoubleUpDown3_ValueChanged(object sender, double value)
        {
            Parameter.specifications.圆形检测[0].ThresholdHigh = value;
        }

        private void uiDoubleUpDown2_ValueChanged(object sender, double value)
        {
            Parameter.specifications.圆形检测[0].AreaLow = value;
        }

        private void uiDoubleUpDown1_ValueChanged(object sender, double value)
        {
            Parameter.specifications.圆形检测[0] .AreaHigh= value;
        }

        private void MeachRate_ValueChanged(object sender, double value)
        {

        }
    }
}
