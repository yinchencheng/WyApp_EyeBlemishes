using HalconDotNet;
using SevenZip.Compression.LZ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WY_App;
using static WY_App.Utility.Parameter;

namespace WY_App.Utility
{
    public class Halcon
    {
        public static HTuple[] hv_Width = new HTuple[4];
        public static HTuple[] hv_Height = new HTuple[4];
        public static HTuple[] hv_AcqHandle = new HTuple[4];
        public static bool[] CamConnect = new bool[3] { false ,false,false};
        public static bool initalCamera(string CamID, ref HTuple hv_AcqHandle)
        {
            try
            {
                //获取相机句柄               
                HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive",-1, "default", -1, "false", "default", CamID, 0, -1, out hv_AcqHandle);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "SingleFrame");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + CamID+"相机链接失败" + ex.Message);
                主窗体.AlarmList.Add(System.DateTime.Now.ToString() + CamID+ "相机链接失败" + ex.Message);
                return false;
            }

        }
       
        public Halcon()
        {

                Thread th = new Thread(ini_Cam);
                th.IsBackground = true;
                th.Start();
            
        }

        private void ini_Cam()
        {
            while (true)
            {
                Thread.Sleep(5000);
                while (!CamConnect[0])
                {
                    Thread.Sleep(5000);
                    if (!CamConnect[0])
                    {
                        CamConnect[0] = initalCamera("CAM0", ref hv_AcqHandle[0]);              
                    }
                    else
                    {

                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机1链接成功");
                        主窗体.AlarmList.Add(System.DateTime.Now.ToString() + "相机1链接成功");
                    }
                }              
                while (!CamConnect[1])
                {
                    Thread.Sleep(5000);
                    if (!CamConnect[1])
                    {
                        CamConnect[1] = initalCamera("CAM1", ref hv_AcqHandle[1]);
                    }
                    else
                    {
                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机2链接成功");
                        主窗体.AlarmList.Add(System.DateTime.Now.ToString() + "相机2链接成功");
                    }
                }
                while (!CamConnect[2])
                {
                    Thread.Sleep(5000);
                    if (!CamConnect[2])
                    {
                        CamConnect[2] = initalCamera("CAM2", ref hv_AcqHandle[2]);
                    }
                    else
                    {

                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机3链接成功");
                        主窗体.AlarmList.Add(System.DateTime.Now.ToString() + "相机3链接成功");
                    }
                }
            }
        }

        public static bool GrabImage(HTuple hv_AcqHandle, out HObject ho_Image)
        {
            try
            {
                HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);
                return true;
            }
            catch
            {
                ho_Image=null;
                return false;
            }
        }

        public static bool GrabImageLive(HWindow hWindow, HTuple hv_AcqHandle, out HObject ho_Image)
        {
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
            while (true)
            {
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                hWindow.DispObj(ho_Image);
            }    
        }
        public static bool StopImage(HTuple hv_AcqHandle)
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);
            return true;
        }

        public static bool ImgDisplay(int i, string imgPath, HTuple Hwindow ,ref HObject hImage)
        {
            HOperatorSet.GenEmptyObj(out hImage);
            HOperatorSet.SetPart(Hwindow, 0, 0, -1, -1);//设置窗体的规格
            HOperatorSet.ReadImage(out hImage, imgPath);//读取图片存入到HalconImage           
            HOperatorSet.GetImageSize(hImage, out hv_Width[i], out hv_Height[i]);//获取图片大小规格
            HOperatorSet.DispObj(hImage, Hwindow);//显示图片
            return true;
        }
        //        //-----------------------------------------------------------------------------
        public static void CloseFramegrabber(HTuple hv_AcqHandle)
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);
        }
        public static void TriggerModeOff(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
        }
        public static void TriggerModeOn(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
        }
        public static void SetFramegrabberParam(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "gain", Parameter.cameraParam.Gain[0]);
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", Parameter.cameraParam.Shutter[0]);

        }
        public static void GrabImageAsync(HTuple hv_AcqHandle, out HObject himage)
        {
            HOperatorSet.GrabImageAsync(out himage, hv_AcqHandle, -1);
        }
        public static void GrabImageStart(HTuple hv_AcqHandle)
        {
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
        }

        public static void DetectionDrawAOI(HWindow hWindow, out HObject hv_Region)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.DrawRegion(out hv_Region, hWindow);
            HOperatorSet.DispObj(hv_Region, hWindow);
        }

        public static void DetectionDrawCriclaAOI(HWindow hWindow, HObject hImage, ref Parameter.Cricle cricle)
        {
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.SetColor(hWindow, "green");
            HObject DrawCircle;
            HTuple Row;
            HTuple Column;
            HTuple Area;

            hWindow.DrawCircle(out cricle.Row, out cricle.Colum, out cricle.Radius);
            HOperatorSet.GenCircle(out DrawCircle, cricle.Row, cricle.Colum, cricle.Radius);
            HOperatorSet.DispObj(DrawCircle, hWindow);
            HOperatorSet.AreaCenter(DrawCircle, out Area, out Row, out Column);
            HOperatorSet.SetTposition(hWindow, Row, Column);
            HOperatorSet.WriteString(hWindow, "Area:" + Area * cricle.PixelResolution);

        }
        public static bool DetectionCricleHObject(int i, HWindow hWindow, HObject hImage, Parameter.Cricle cricle, HTuple PointRow, HTuple PointColum, HTuple length, ref Parameter.Cricle cricle1)
        {

            HObject ho_Contours, ho_Cross, ho_Contour;
            HObject ho_Circle;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_shapeParam = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Parameter = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            //获取图像及图像尺寸
            //获取图像及图像尺寸
            //dev_close_window(...);

            HOperatorSet.SetLineWidth(hWindow, 1);

            //标记测量位置
            //draw_line (WindowHandle, Row1, Column1, Row2, Column2)


            hv_shapeParam.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(PointRow);
                hv_shapeParam = hv_shapeParam.TupleConcat(PointColum);
                hv_shapeParam = hv_shapeParam.TupleConcat(cricle.Radius);
            }
            //创建测量句柄
            hv_MetrologyHandle.Dispose();
            HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
            //添加测量对象
            HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width[i], hv_Height[i]);
            hv_Index.Dispose();
            HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", hv_shapeParam, length, 3, 1, 50, new HTuple(), new HTuple(), out hv_Index);
            //执行测量，获取边缘点集
            HOperatorSet.SetColor(hWindow, "yellow");
            HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
            ho_Contours.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
            HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "negative", out hv_Row, out hv_Column);
            //HOperatorSet.DispObj(ho_Contours, hWindow);
            HOperatorSet.SetColor(hWindow, "red");
            ho_Cross.Dispose();
            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
            HOperatorSet.DispObj(ho_Cross, hWindow);
            //获取最终测量数据和轮廓线
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetLineWidth(hWindow, 1);
            hv_Parameter.Dispose();
            HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
            ho_Contour.Dispose();
            HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 1.5);

            hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
            HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
            HOperatorSet.SetDraw(hWindow, "margin");
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_Circle.Dispose();
                HOperatorSet.GenCircle(out ho_Circle, hv_Parameter.TupleSelect(0), hv_Parameter.TupleSelect(1), hv_Parameter.TupleSelect(2));
                cricle1.Row = hv_Parameter[0];
                cricle1.Colum = hv_Parameter[1];
                cricle1.Radius = hv_Parameter[2] * cricle.PixelResolution;

            }
            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispObj(ho_Circle, hWindow);
            //释放测量句柄
            HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
            ho_Contours.Dispose();
            ho_Cross.Dispose();
            ho_Contour.Dispose();
            ho_Circle.Dispose();
            hv_WindowHandle.Dispose();
            hv_shapeParam.Dispose();
            hv_MetrologyHandle.Dispose();
            hv_Index.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Parameter.Dispose();
            hv_RowBegin.Dispose();
            hv_ColBegin.Dispose();
            hv_RowEnd.Dispose();
            hv_ColEnd.Dispose();
            hv_Nr.Dispose();
            hv_Nc.Dispose();
            hv_Dist.Dispose();
            return true;
        }

        public static void DetectionCricle0(int camIndex, HWindow hWindow, HObject hImage, Parameter.Cricle cricle, ref bool result)
        {
            HObject ho_Circle, ho_ImageReduced;
            HObject ho_Regions, ho_ConnectedRegions;
            HObject ho_SelectedRegions;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Area1 = new HTuple();

            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            //dev_open_window(...);

            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            ho_Circle.Dispose();
            HOperatorSet.GenCircle(out ho_Circle, cricle.Row, cricle.Colum, cricle.Radius - 50);
            HOperatorSet.DispObj(ho_Circle, hWindow);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(hImage, ho_Circle, out ho_ImageReduced);
            ho_Regions.Dispose();


            HOperatorSet.Threshold(ho_ImageReduced, out ho_Regions, cricle.ThresholdLow, cricle.ThresholdHigh);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
            HOperatorSet.SetColor(hWindow, "red");
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", cricle.AreaLow, cricle.AreaHigh);
            HOperatorSet.DispObj(ho_SelectedRegions, hWindow); ;
            hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
            HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row1, out hv_Column1);
            hv_Number.Dispose();
            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
            HTuple end_val13 = hv_Number - 1;
            HTuple step_val13 = 1;
            for (hv_Index = 0; hv_Index.Continue(end_val13, step_val13); hv_Index = hv_Index.TupleAdd(step_val13))
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetTposition(hWindow, hv_Row1.TupleSelect(hv_Index), hv_Column1.TupleSelect(hv_Index));
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.WriteString(hWindow, hv_Area.TupleSelect(hv_Index));
                }
            }
            hv_Area1.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();

            if (hv_Number == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            ho_Circle.Dispose();
            ho_ImageReduced.Dispose();
            ho_Regions.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            hv_WindowHandle.Dispose();
            hv_Row1.Dispose();
            hv_Column1.Dispose();
            hv_Row2.Dispose();
            hv_Column2.Dispose();
            hv_Area.Dispose();
            hv_Number.Dispose();
            hv_Index.Dispose();
            hv_Area1.Dispose();
            //return true;
        }
        public static void DetectionCricle(int camIndex,HWindow hWindow, HObject hImage, Parameter.Cricle cricle, ref bool result)
        {
            // Local iconic variables 

            HObject ho_Circle, ho_ImageReduced;
            HObject ho_ImagePart, ho_ImageMean, ho_Region, ho_ConnectedRegions;
            HObject ho_SelectedRegions;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WindowHandle = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);

            //dev_open_window(...);
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            ho_Circle.Dispose();
            HOperatorSet.GenCircle(out ho_Circle, cricle.Row, cricle.Colum, cricle.Radius - 50);
            HOperatorSet.DispObj(ho_Circle, hWindow);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(hImage, ho_Circle, out ho_ImageReduced);
            ho_ImagePart.Dispose();
            ho_ImageMean.Dispose();
            HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, cricle.ThresholdLow, cricle.ThresholdHigh);
            ho_Region.Dispose();
            HOperatorSet.DynThreshold(ho_ImageReduced, ho_ImageMean, out ho_Region, cricle.ThresholdLow, "light");
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
            hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
            HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
            //HOperatorSet.DispObj(ho_ImagePart, hWindow);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", cricle.AreaLow, cricle.AreaHigh);
            HOperatorSet.SetColor(hWindow, "red");
            HOperatorSet.SetDraw(hWindow, "margin");

            HOperatorSet.DispObj(ho_SelectedRegions, hWindow);

            ho_Circle.Dispose();
            ho_ImageReduced.Dispose();
            ho_ImagePart.Dispose();
            ho_ImageMean.Dispose();
            ho_Region.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();

            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_WindowHandle.Dispose();
            hv_Area.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
        }

        public static void DetectionCricle2(int camIndex, HWindow hWindow, HObject hImage, Parameter.Cricle cricle, ref bool result)
        {
            HObject ho_Circle, ho_ImageReduced = null, ho_Region, ho_RegionDilation;
            HObject ho_RegionErosion, ho_ImageReduced1 = null, ho_Region1;
            HObject ho_ConnectedRegions, ho_SelectedRegions;

            // Local control variables 

            HTuple hv_Area = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            ho_Circle.Dispose();
            HOperatorSet.GenCircle(out ho_Circle, cricle.Row, cricle.Colum, cricle.Radius - 50);
            HOperatorSet.DispObj(ho_Circle,hWindow);
            HOperatorSet.ReduceDomain(hImage, ho_Circle,out ho_ImageReduced);
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 0, 80);
            ho_RegionDilation.Dispose();
            HOperatorSet.DilationCircle(ho_Region, out ho_RegionDilation, cricle.ThresholdHigh1);
            ho_RegionErosion.Dispose();
            HOperatorSet.ErosionCircle(ho_RegionDilation, out ho_RegionErosion, cricle.ThresholdLow1);
            HOperatorSet.ReduceDomain(hImage, ho_RegionErosion,out ho_ImageReduced1);
            ho_Region1.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, cricle.ThresholdLow, cricle.ThresholdHigh);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Region1, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area","and", cricle.AreaLow, cricle.AreaHigh);
            HOperatorSet.SetColor(hWindow, "red");
            HOperatorSet.SetDraw(hWindow, "fill");
            HOperatorSet.DispObj(ho_SelectedRegions, hWindow);
            hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
            HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row1, out hv_Column1);

            ho_Circle.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_RegionDilation.Dispose();
            ho_RegionErosion.Dispose();
            ho_ImageReduced1.Dispose();
            ho_Region1.Dispose();
            ho_SelectedRegions.Dispose();
            hv_Area.Dispose();
            hv_Row1.Dispose();
            hv_Column1.Dispose();
        }
       
        public static void DetectionDrawRect2AOI(HWindow hWindow, HObject hImage, ref Parameter.Rect2 rect2)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            hWindow.DispObj(hImage);
            hWindow.DrawRectangle2(out rect2.Row, out rect2.Colum, out rect2.Phi, out rect2.Length1, out rect2.Length2);

        }
        public static void DetectionDrawLineAOI(HWindow hWindow, HObject hImage, ref Parameter.Rect1 rect1)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            hWindow.DispObj(hImage);
            hWindow.DrawLine(out rect1.Row1, out rect1.Colum1, out rect1.Row2, out rect1.Colum2);
            HOperatorSet.DispLine(hWindow, rect1.Row1, rect1.Colum1, rect1.Row2, rect1.Colum2);

        }


        /// <summary>
        /// 直线卡尺工具
        /// </summary>
        /// <param name="hWindow"></param>
        /// <param name="hImage"></param>
        /// <param name="rect1"></param>
        /// <param name="PointXY"></param>
        /// <returns></returns>
        public static bool DetectionHalconLine(int i,HWindow hWindow, HObject hImage, Parameter.Rect1 rect1, HTuple length, ref HRect1 PointXY)
        {
            HObject ho_Contours, ho_Cross, ho_Contour;

            // Local control variables 

            HTuple hv_shapeParam = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Parameter = new HTuple();

            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Nr = new HTuple();
            try
            {
                HOperatorSet.SetLineWidth(hWindow, 1);
                //HOperatorSet.DispObj(hImage, hWindow);
                //标记测量位置         

                //HOperatorSet.CropPart(hImage, out ho_Rectangle, Parameter.specifications.矩形检测区域.Row1, Parameter.specifications.矩形检测区域.Colum1, Parameter.specifications.矩形检测区域.Colum2 - Parameter.specifications.矩形检测区域.Colum1, Parameter.specifications.矩形检测区域.Row2 - Parameter.specifications.矩形检测区域.Row1);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_shapeParam = new HTuple();
                    hv_shapeParam = hv_shapeParam.TupleConcat(rect1.Row1, rect1.Colum1, rect1.Row2, rect1.Colum2);
                }
                //创建测量句柄
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //添加测量对象
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width[i], hv_Height[i]);
                hv_Index.Dispose();
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, length, 3, rect1.simga, rect1.阈值, new HTuple(), new HTuple(), out hv_Index);

                //执行测量，获取边缘点集
                HOperatorSet.SetColor(hWindow, "yellow");
                HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
                hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.DispObj(ho_Contours, hWindow);
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                //获取最终测量数据和轮廓线
                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.SetLineWidth(hWindow, 2);
                hv_Parameter.Dispose();
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 1.5);
                HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out PointXY.Row1, out PointXY.Colum1, out PointXY.Row2, out PointXY.Colum2, out hv_Nr, out hv_Nc, out hv_Dist);
                HOperatorSet.DispObj(ho_Cross, hWindow);
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DispObj(ho_Contour, hWindow);                
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle); ;
                ho_Contours.Dispose();
                ho_Cross.Dispose();
                ho_Contour.Dispose();
                hv_shapeParam.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                if (PointXY.Row1.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch 
            {
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle); 
                hv_shapeParam.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Parameter.Dispose();
                return false;
            }
            
            // Initialize local and output iconic variables 

            
            //释放测量句柄
            
        }

        public static bool DetectionHalconReadDlModel(string path, out HTuple hv_DLModelHandle, out HTuple hv_DLPreprocessParam, out HTuple hv_InferenceClassificationThreshold, out HTuple hv_InferenceSegmentationThreshold)
        {
            HTuple hv_DLDeviceHandles = new HTuple(), hv_DLDeviceHandle = new HTuple();
            HTuple hv_MetaData = new HTuple();
            HTuple hv_DLDatasetInfo = new HTuple();

            HOperatorSet.ReadDlModel(path, out hv_DLModelHandle);
            hv_DLDeviceHandles.Dispose();
            HOperatorSet.QueryAvailableDlDevices(((new HTuple("runtime")).TupleConcat("runtime")).TupleConcat("id"), ((new HTuple("gpu")).TupleConcat("cpu")).TupleConcat(0), out hv_DLDeviceHandles);
            if ((int)(new HTuple(hv_DLDeviceHandles.TupleEqual(new HTuple()))) != 0)
            {
                throw new HalconException("No suitable CPU or GPU was found.");
            }
            hv_DLDeviceHandle.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DLDeviceHandle = hv_DLDeviceHandles.TupleSelect(0);
            }
            HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDeviceHandle);

            hv_MetaData.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "meta_data", out hv_MetaData);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_InferenceClassificationThreshold = ((hv_MetaData.TupleGetDictTuple("anomaly_classification_threshold"))).TupleNumber();
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_InferenceSegmentationThreshold = ((hv_MetaData.TupleGetDictTuple("anomaly_segmentation_threshold"))).TupleNumber();
            }
            HDevelopExport.create_dl_preprocess_param_from_model(hv_DLModelHandle, "none", "full_domain", new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);
            hv_DLDatasetInfo.Dispose();
            HOperatorSet.CreateDict(out hv_DLDatasetInfo);
            HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_names", (new HTuple("ok")).TupleConcat("ng"));
            HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_ids", (new HTuple(0)).TupleConcat(1));

            return true;
        }


        public static void DetectionHalconDeepLearning(int i, HWindow hWindow, HObject hImage, HTuple hv_DLModelHandle, HTuple hv_DLPreprocessParam, HTuple hv_InferenceClassificationThreshold, HTuple hv_InferenceSegmentationThreshold, HTuple RowY1, HTuple ColumX, double DeepLearningRate,ref bool result)
        {
            HObject ho_Rectangle;
            HObject ho_ImageReduced, ho_ImagePart;

            HTuple hv_DLDatasetInfo = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_anomaly_score = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.SetDraw(hWindow, "margin");
            if (0 == i)//N1区域设置
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, RowY1 - 50, ColumX + 80, RowY1 + 30, ColumX + 580);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispRectangle1(hWindow, RowY1 - 50, ColumX + 80, RowY1 + 30, ColumX + 580);
                }
            }
            else//N2区域设置
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, RowY1 - 30, ColumX + 80, RowY1 + 50, ColumX + 580);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispRectangle1(hWindow, RowY1 - 30, ColumX + 80, RowY1 + 50, ColumX + 580);
                }
            }
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(hImage, ho_Rectangle, out ho_ImageReduced);
            ho_ImagePart.Dispose();
            HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
            //write_image (ImagePart, 'bmp', 0, 'C:/Users/王印/Desktop/halconDPL/极耳/N1/N1训练图/OK/'+ Index +'.bmp')
            //HOperatorSet.WriteImage(ho_ImagePart, "bmp", 0, "C:/Users/王印/Desktop/halconDPL/极耳/N2/N2训练图/NG/0.bmp");
            //推理程序：调用model_best.hdl推理指定目录下测试图像
            //读取模型

            hv_DLSample.Dispose();
            HDevelopExport.gen_dl_samples_from_images(ho_ImagePart, out hv_DLSample);
            HDevelopExport.preprocess_dl_samples(hv_DLSample, hv_DLPreprocessParam);
            hv_DLResult.Dispose();
            HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSample, new HTuple(), out hv_DLResult);
            HDevelopExport.threshold_dl_anomaly_results(hv_InferenceSegmentationThreshold, hv_InferenceClassificationThreshold, hv_DLResult);
            HOperatorSet.SetTposition(hWindow, RowY1, ColumX);
            hv_anomaly_score.Dispose();
            HOperatorSet.GetDictTuple(hv_DLResult, "anomaly_score", out hv_anomaly_score);
            if (hv_anomaly_score <= DeepLearningRate)
            {
                result = true;
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetColor(hWindow, "green");
                    HOperatorSet.WriteString(hWindow, "OK" + hv_anomaly_score);
                }
            }
            else
            {
                result = false;
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    HOperatorSet.WriteString(hWindow, "NG" + hv_anomaly_score);
                }
            }
            ho_Rectangle.Dispose();
            ho_ImagePart.Dispose();
            hv_DLDatasetInfo.Dispose();

            hv_DLSample.Dispose();
            hv_DLResult.Dispose();
            hv_anomaly_score.Dispose();
        }

       

    }
}
