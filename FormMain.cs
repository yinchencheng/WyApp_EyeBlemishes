using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using WY_App.Utility;
using TcpClient = WY_App.Utility.TcpClient;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Text;
using System.ComponentModel;
using System.Data;
using MvCamCtrl.NET;
using System.Drawing.Imaging;


namespace WY_App
{
    public partial class FormMain : Form
    {
        //static ObservableCollection<double> _wetSlope = new ObservableCollection<double> { };
        HslCommunication hslCommunication;
        public static string[] point_Location = new string[3] { "0", "0", "0" };
        public static string Alarm = "";
        public static List<string> AlarmList = new List<string>();
        Thread myThread;
        Thread MainThread;
        string[] pathSave = new string[8] { "OK","无产品", "尺寸NG", "气泡", "黑点", "刺伤", "划伤", "压痕"};
        string[] detectioresult;
        int[] count;
        int[] count2;
        double[] value = new double[19];
        public static Parameter.specification[] specifications;
        public FormMain()
        {
            InitializeComponent();

            try
            {
                Parameter.commministion = XMLHelper.BackSerialize<Parameter.Commministion>("Commministion.xml");
            }
            catch
            {
                Parameter.commministion = new Parameter.Commministion();
                XMLHelper.serialize<Parameter.Commministion>(Parameter.commministion, "Commministion.xml");
            }

            try
            {
                Parameter.counts = XMLHelper.BackSerialize<Parameter.Counts>("CountsParams.xml");
            }
            catch
            {
                Parameter.counts = new Parameter.Counts();
                XMLHelper.serialize<Parameter.Counts>(Parameter.counts, "CountsParams.xml");
            }

            try
            {
                Parameter.specifications = XMLHelper.BackSerialize<Parameter.Specifications>("Specifications.xml");               
            }
            catch
            {
                Parameter.specifications = new Parameter.Specifications();
                XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, "Specifications.xml");
            }
            specifications = new Parameter.specification[11] {
            Parameter.specifications.总宽, Parameter.specifications.总长, Parameter.specifications.料宽, Parameter.specifications.胶宽,
            Parameter.specifications.长端, Parameter.specifications.左短端, Parameter.specifications.右短端, Parameter.specifications.左肩宽,
            Parameter.specifications.右肩宽, Parameter.specifications.左肩高, Parameter.specifications.右肩高};
            count = new int[6] { Parameter.counts.Counts1, Parameter.counts.Counts2, Parameter.counts.Counts3, Parameter.counts.Counts4, Parameter.counts.Counts5, Parameter.counts.Counts6 };
            count2 = new int[6] { Parameter.counts.Counts21, Parameter.counts.Counts22, Parameter.counts.Counts23, Parameter.counts.Counts24, Parameter.counts.Counts25, Parameter.counts.Counts26 };

            DataGridViewRow row0 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row0);
            DataGridViewRow row1 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row1);
            DataGridViewRow row2 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row2);
            DataGridViewRow row3 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row3);
            DataGridViewRow row4 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row4);

            DataGridViewRow row5 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row5);
            DataGridViewRow row6 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row6);
            DataGridViewRow row7 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row7);
            DataGridViewRow row8 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row8);
            DataGridViewRow row9 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row9);

            DataGridViewRow row10 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row10);
            DataGridViewRow row12 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row12);
            DataGridViewRow row13 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row13);
            DataGridViewRow row14 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row14);
            DataGridViewRow row15 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row15);

            uiDataGridView1.Rows[0].Cells[0].Value = "总宽";
            uiDataGridView1.Rows[1].Cells[0].Value = "总长";
            uiDataGridView1.Rows[2].Cells[0].Value = "料宽";
            uiDataGridView1.Rows[3].Cells[0].Value = "胶宽";
            uiDataGridView1.Rows[4].Cells[0].Value = "长端";

            uiDataGridView1.Rows[5].Cells[0].Value = "左短端";
            uiDataGridView1.Rows[6].Cells[0].Value = "右短端";
            uiDataGridView1.Rows[7].Cells[0].Value = "右肩高";
            uiDataGridView1.Rows[8].Cells[0].Value = "右肩宽";
            uiDataGridView1.Rows[9].Cells[0].Value = "左肩高";
            uiDataGridView1.Rows[10].Cells[0].Value ="左肩宽";

            uiDataGridView1.Rows[11].Cells[0].Value = "气泡";
            uiDataGridView1.Rows[12].Cells[0].Value = "黑点";
            uiDataGridView1.Rows[13].Cells[0].Value = "刺伤";
            uiDataGridView1.Rows[14].Cells[0].Value = "划伤";
            uiDataGridView1.Rows[15].Cells[0].Value = "压痕";
            
            uiDataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            
            
            for (int i = 0; i < 5; i++)
            {
                uiDataGridView1.Rows[i + 11].Cells[1].Value = count[i];
                uiDataGridView1.Rows[i + 11].Cells[2].Value = count2[i];
            }
            myThread = new Thread(initAll);
            myThread.IsBackground = true;
            myThread.Start();
        }

        private void initAll()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Task task = new Task(() =>
                {
                    MethodInvoker start = new MethodInvoker(() =>
                    {

                        if (Parameter.commministion.PlcEnable)
                        {
                            if (HslCommunication.plc_connect_result)
                            {
                                lab_PLCStatus.Text = "在线";
                                lab_PLCStatus.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_PLCStatus.Text = "离线";
                                lab_PLCStatus.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_PLCStatus.Text = "禁用";
                            lab_PLCStatus.BackColor = Color.Gray;
                        }

                        if (Parameter.commministion.TcpClientEnable)
                        {
                            if (TcpClient.TcpClientConnectResult)
                            {
                                lab_Client.Text = "在线";
                                lab_Client.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Client.Text = "离线";
                                lab_Client.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Client.Text = "禁用";
                            lab_Client.BackColor = Color.Gray;
                        }
                        if (Parameter.commministion.TcpServerEnable)
                        {
                            if (TcpServer.TcpServerConnectResult)
                            {
                                lab_Server.Text = "在线";
                                lab_Server.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Server.Text = "等待";
                                lab_Server.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Server.Text = "禁用";
                            lab_Server.BackColor = Color.Gray;
                        }
                        if (AlarmList.Count != 0)
                        {
                            lst_LogInfos.Items.Add(AlarmList[0]);
                            AlarmList.RemoveAt(0);
                        }
                        if (lst_LogInfos.Items.Count > 20)
                        {
                            lst_LogInfos.Items.RemoveAt(0);
                        }
                    });
                    this.BeginInvoke(start);
                });
                task.Start();
            }

        }
        static string movefile;
        bool testReslut = true;
        private void MainRun()
        {
            while (true)
            {
                ushort ushort100 = HslCommunication._NetworkTcpDevice.ReadUInt16("250").Content; // 读取寄存器100的ushort值              
                if (ushort100 == 49)
                {
                    testReslut = true;
                    HslCommunication._NetworkTcpDevice.Write("250", 0);
                    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "触发信号:" +ushort100.ToString());
                    TcpServer.TcpServerReceiveMsg = "";
                    string tcpReciveStr = TcpClient.tcpClientSend("S1");
                    LogHelper.Log.WriteInfo(tcpReciveStr);
                    detectioresult = tcpReciveStr.Split('+');

                    if ( detectioresult.Length > 5)
                    {
                        
                        for (int index = 0; index < detectioresult.Count() - 1; index++)
                        {
                            value[index] = Convert.ToDouble(detectioresult[index + 1]) / 1000;
                        }

                        if (value != null && detectioresult[2].Contains("1"))
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                uiDataGridView1.Rows[j].Cells[1].Value = value[j + 8] + specifications[j].adjustZ;
                                if (value[j + 8] + specifications[j].adjustZ <= specifications[j].value + specifications[j].max && value[j + 8] + specifications[j].adjustZ >= specifications[j].value + specifications[j].min)
                                {
                                    uiDataGridView1.Rows[j].Cells[1].Style.BackColor = Color.Green;
                                }
                                else
                                {
                                    uiDataGridView1.Rows[j].Cells[1].Style.BackColor = Color.Red;
                                    testReslut = false;
                                }
                            }
                            for (int j = 0; j < 5; j++)
                            {
                                if (!detectioresult[j + 4].Contains("1") )
                                {
                                    testReslut = false;
                                    count[j]++;
                                    uiDataGridView1.Rows[j + 11].Cells[1].Value = count[j];
                                    uiDataGridView1.Rows[j + 11].Cells[1].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    uiDataGridView1.Rows[j + 11].Cells[1].Style.BackColor = Color.Green;
                                }
                            }
                        }
                        else
                        {
                            testReslut = false;
                            for (int j = 0; j < 11; j++)
                            {
                                uiDataGridView1.Rows[j].Cells[1].Value = 0;
                                uiDataGridView1.Rows[j].Cells[1].Style.BackColor = Color.Black;
                            }
                            for (int j = 0; j < 5; j++)
                            {
                                //uiDataGridView1.Rows[j + 11].Cells[1].Value = count[j];
                                uiDataGridView1.Rows[j + 11].Cells[1].Style.BackColor = Color.Black;
                            }
                        }
                        if (detectioresult[2].Contains("1") && testReslut && detectioresult[2].Contains("1"))//模板匹配、瑕疵、尺寸OK
                        {
                            HslCommunication._NetworkTcpDevice.Write("200", 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "写入:" + "5");
                        }
                        else
                        {
                            HslCommunication._NetworkTcpDevice.Write("200", 6);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "写入:" + "6");
                        }
                        
                        Thread.Sleep(500);
                        if (detectioresult[2].Contains("1") && testReslut && detectioresult[2].Contains("1"))//模板匹配、瑕疵、尺寸OK
                        {                           
                            moveImage("正面", 0);
                        }
                        else
                        {                           
                            if (!detectioresult[2].Contains("1")) //无产品
                            {
                                moveImage("正面", 1);
                            }
                            else 
                            {
                                if (!testReslut)//尺寸NG
                                {
                                    moveImage("正面", 2);
                                }
                                if (!detectioresult[1].Contains("1"))//瑕疵NG
                                {
                                    for (int i = 0; i < 5; i++)//瑕疵分类
                                    {
                                        if (!detectioresult[i + 4].Contains("1"))
                                        {
                                            moveImage("正面", i + 3);
                                        }
                                    }
                                }
                            }
                            
                        }
                    }                   
                    detectioresult = null;                  
                }
                else if (HslCommunication._NetworkTcpDevice.ReadUInt16("250").Content == 50)
                {                   
                    HslCommunication._NetworkTcpDevice.Write("250", 0);
                    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress +"Port:" + Parameter.commministion.PlcIpPort + "触发信号:" + ushort100.ToString());
                    TcpServer.TcpServerReceiveMsg = "";

                    string tcpReciveStr = TcpClient.tcpClientSend("S2");
                    if(tcpReciveStr=="超时")
                    {
                        HslCommunication._NetworkTcpDevice.Write("200", 6);
                        return;
                    }
                    LogHelper.Log.WriteInfo(tcpReciveStr);
                    detectioresult = tcpReciveStr.Split('+');
                    if (detectioresult.Length > 5)
                    {

                        for (int index = 0; index < detectioresult.Count() - 1; index++)
                        {
                            value[index] = Convert.ToDouble(detectioresult[index + 1]) / 1000;
                        }

                        if (value != null && detectioresult[2].Contains("1"))
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                uiDataGridView1.Rows[j].Cells[2].Value = value[j + 8] + specifications[j].adjustF;
                                if (value[j + 8] + specifications[j].adjustF <= specifications[j].value + specifications[j].max && value[j + 8] + specifications[j].adjustF >= specifications[j].value + specifications[j].min)
                                {
                                    uiDataGridView1.Rows[j].Cells[2].Style.BackColor = Color.Green;
                                }
                                else
                                {
                                    uiDataGridView1.Rows[j].Cells[2].Style.BackColor = Color.Red;
                                    testReslut = false;
                                }
                            }
                            for (int j = 0; j < 5; j++)
                            {
                                if (!detectioresult[j + 4].Contains("1"))
                                {
                                    testReslut = false;
                                    count2[j]++;
                                    uiDataGridView1.Rows[j + 11].Cells[2].Value = count2[j];
                                    uiDataGridView1.Rows[j + 11].Cells[2].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    uiDataGridView1.Rows[j + 11].Cells[2].Style.BackColor = Color.Green;
                                }
                            }
                        }
                        else
                        {
                            testReslut = false;
                            for (int j = 0; j < 11; j++)
                            {
                                uiDataGridView1.Rows[j].Cells[2].Value = 0;
                                uiDataGridView1.Rows[j].Cells[2].Style.BackColor = Color.Black;
                            }
                            for (int j = 0; j < 5; j++)
                            {
                                uiDataGridView1.Rows[j + 11].Cells[2].Value = count2[j];
                                uiDataGridView1.Rows[j + 11].Cells[2].Style.BackColor = Color.Black;
                            }
                        }

                        if (detectioresult[2].Contains("1") && testReslut && detectioresult[2].Contains("1"))//模板匹配、瑕疵、尺寸OK
                        {
                            HslCommunication._NetworkTcpDevice.Write("200", 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "写入:" + "5");
                        }
                        else
                        {
                            HslCommunication._NetworkTcpDevice.Write("200", 6);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "写入:" + "6");
                        }

                        Thread.Sleep(500);
                        if (detectioresult[2].Contains("1") && testReslut && detectioresult[2].Contains("1"))//模板匹配、瑕疵、尺寸OK
                        {
                            moveImage("反面", 0);
                        }
                        else
                        {
                            if (!detectioresult[2].Contains("1")) //无产品
                            {
                                moveImage("正面", 1);
                            }
                            else
                            {
                                if (!testReslut)//尺寸NG
                                {
                                    moveImage("正面", 2);
                                }
                                if (!detectioresult[1].Contains("1"))//瑕疵NG
                                {
                                    for (int i = 0; i < 5; i++)//瑕疵分类
                                    {
                                        if (!detectioresult[i + 4].Contains("1"))
                                        {
                                            moveImage("正面", i + 3);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    detectioresult = null;
                }

                Thread.Sleep(50);
                
            }
        }
        public class FileTimeInfo
        {
            public string FileName;  //文件名
            public DateTime FileCreateTime; //创建时间
        }

        static void GetLatestFileTimeInfo(string dir, string ext)
        {
            DirectoryInfo folder = new DirectoryInfo(dir);
            List<FileTimeInfo> list = new List<FileTimeInfo>();
            string[] files = Directory.GetFiles(dir,"*.jpg", SearchOption.AllDirectories);
            //遍历文件夹中的
            foreach (FileInfo file in folder.GetFiles("*.jpg", SearchOption.AllDirectories))
            {
                if (file.Extension.ToUpper() == ext.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        FileCreateTime = file.CreationTime
                    });
                }
            }

            var f = from x in list
                    orderby x.FileCreateTime
                    select x;
            movefile = f.LastOrDefault().FileName;
        }


        private void  moveImage(string path , int  i)
        {
            Task.Run(() =>
            {
            JudegPathExists:
                if (Directory.Exists(Parameter.commministion.ImagePath + System.DateTime.Now.ToString("yyyy-MM-dd")))
                {
                JudegPicUsed:
                    try
                    {
                        GetLatestFileTimeInfo(Parameter.commministion.ImagePath + System.DateTime.Now.ToString("yyyy-MM-dd"), ".jpg");
                        string ImageNameOut = Parameter.commministion.ImageSavePath + path + "\\" + pathSave[i] + "\\" + "OUT" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                        string ImageNameIn = Parameter.commministion.ImageSavePath + path + "\\" + pathSave[i] + "\\" + "IN" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                        if (!Directory.Exists(Parameter.commministion.ImageSavePath + path + "\\" + pathSave[i]))
                        {
                            Directory.CreateDirectory(Parameter.commministion.ImageSavePath + path + "\\" + pathSave[i]);
                        }
                        if (movefile != null && movefile.Contains("out"))
                        {
                            Image image = Image.FromFile(movefile);
                            pictureBox1.Image = image;
                            File.Copy(movefile, ImageNameOut);
                            DirectoryInfo dir = new DirectoryInfo(Parameter.commministion.ImagePath + System.DateTime.Now.ToString("yyyy-MM-dd"));
                            if (dir.Exists)
                            {
                                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                foreach (FileSystemInfo ji in fileinfo)
                                {
                                    try
                                    {
                                        if (ji is DirectoryInfo)            //判断是否文件夹
                                        {
                                            DirectoryInfo subdir = new DirectoryInfo(ji.FullName);
                                            subdir.Delete(true);          //删除子目录和文件
                                        }
                                        else
                                        {
                                            File.Delete(ji.FullName);      //删除指定文件
                                        }
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                            Thread.Sleep(50);
                        }
                        else if (movefile != null && movefile.Contains("in"))
                        {
                            Thread.Sleep(50);
                            File.Copy(movefile, ImageNameIn);
                        }
                        else
                        {
                            goto JudegPicUsed;
                        }

                    }
                    catch
                    {
                        goto JudegPicUsed;
                    }
                }
                else
                {
                    goto JudegPathExists;
                }
            });
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定关闭程序吗？", "软件关闭提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                Parameter.counts.Counts1 = count[0];
                Parameter.counts.Counts2 = count[1];
                Parameter.counts.Counts3 = count[2];
                Parameter.counts.Counts4 = count[3];
                Parameter.counts.Counts5 = count[4];
                Parameter.counts.Counts6 = count[5];

                Parameter.counts.Counts21 = count2[0];
                Parameter.counts.Counts22 = count2[1];
                Parameter.counts.Counts23 = count2[2];
                Parameter.counts.Counts24 = count2[3];
                Parameter.counts.Counts25 = count2[4];
                Parameter.counts.Counts26 = count2[5];

                XMLHelper.serialize<Parameter.Counts>(Parameter.counts, "CountsParams.xml");


                //Parameter.specifications.右短端.value = 10;
                //XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, "Specifications.xml");
                myThread.Abort();
                LogHelper.Log.WriteInfo("软件关闭。");
                Process[] allProgresse = System.Diagnostics.Process.GetProcessesByName("SGVision");
                foreach (Process closeProgress in allProgresse)
                {
                    if (closeProgress.ProcessName.Equals("SGVision"))
                    {
                        closeProgress.Kill();
                        closeProgress.WaitForExit();
                        break;
                    }
                }

                this.Close();
            }
        }

        private void btn_Minimizid_System_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            LogHelper.Log.WriteInfo("窗体最小化。");
            FormMain.AlarmList.Add("窗体最小化。");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();//隐藏主窗体  
                LogHelper.Log.WriteInfo("主窗体隐藏。");
                FormMain.AlarmList.Add("主窗体隐藏。");
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//当鼠标点击为左键时  
            {
                this.Show();//显示主窗体  
                LogHelper.Log.WriteInfo("主窗体恢复。");
                FormMain.AlarmList.Add("主窗体恢复。");
                this.WindowState = FormWindowState.Normal;//主窗体的大小为默认  
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

            string shortcutName = this.Text;//快捷方式名称
            string targetPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Wy-App.exe";//目标可执行文件
            string iconLocation = AppDomain.CurrentDomain.BaseDirectory.ToString() + "logo.ico";//ico图标路径
            ShortCutHelper.CreateShortcutOnDesktop(shortcutName, targetPath, shortcutName, iconLocation);


            Process[] allProgresse = System.Diagnostics.Process.GetProcessesByName("SGVision");
            foreach (Process closeProgress in allProgresse)
            {
                if (closeProgress.ProcessName.Equals("SGVision"))
                {
                    closeProgress.Kill();
                    closeProgress.WaitForExit();                   
                }
            }

            Process exep = new Process(); //创建一个进程
            exep.StartInfo.FileName = "SGVision.exe";//启动程序
            exep.StartInfo.Arguments = "";//附加参数
            FileInfo info = new FileInfo("C:\\Program Files\\SGVision\\SGVision.exe");
            exep.StartInfo.WorkingDirectory = info.Directory.ToString(); //工作路径 d:\\test
            exep.StartInfo.CreateNoWindow = false;
            exep.StartInfo.UseShellExecute = true;
            exep.Start();

            if (Parameter.commministion.PlcEnable)
            {
                hslCommunication = new HslCommunication();
                Thread.Sleep(1000);
                if (HslCommunication.plc_connect_result)
                {
                    lab_PLCStatus.Text = "在线";
                    lab_PLCStatus.BackColor = Color.Green;
                }
                else
                {
                    lab_PLCStatus.Text = "离线";
                    lab_PLCStatus.BackColor = Color.Red;
                }
            }
            else
            {
                lab_PLCStatus.Text = "禁用";
                lab_PLCStatus.BackColor = Color.Gray;
            }
           if (Parameter.commministion.TcpClientEnable)
            {
                TcpClient tcpClient = new TcpClient();
                Thread.Sleep(1000);
                if (TcpClient.TcpClientConnectResult)
                {
                    lab_Client.Text = "在线";
                    lab_Client.BackColor = Color.Green;
                }
                else
                {
                    lab_Client.Text = "离线";
                    lab_Client.BackColor = Color.Red;
                }
            }
            else
            {
                lab_PLCStatus.Text = "禁用";
                lab_PLCStatus.BackColor = Color.Gray;
            }

            if (Parameter.commministion.TcpServerEnable)
            {
                TcpServer tcpServer = new TcpServer();
                Thread.Sleep(1000);
                if (TcpServer.TcpServerConnectResult)
                {
                    lab_Server.Text = "在线";
                    lab_Server.BackColor = Color.Green;
                }
                else
                {
                    lab_Server.Text = "等待";
                    lab_Server.BackColor = Color.Red;
                }
            }
            else
            {
                lab_PLCStatus.Text = "禁用";
                lab_PLCStatus.BackColor = Color.Gray;
            }

            LogHelper.Log.WriteInfo("初始化完成");
            FormMain.AlarmList.Add("初始化完成");
        }

        #region 点击panel控件移动窗口
        System.Drawing.Point downPoint;
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new System.Drawing.Point(e.X, e.Y);
        }
        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new System.Drawing.Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
        #endregion
        public static int formloadIndex = 0;
        private void btn_SettingMean_Click(object sender, EventArgs e)
        {
            formloadIndex = 1;
            FormLogin flg=new FormLogin();
            flg.ShowDialog();          
        }

        //    Directory.CreateDirectory(string path);//在指定路径中创建所有目录和子目录，除非已经存在
        //    Directory.Delete(string path);//从指定路径删除空目录
        //    Directory.Delete(string path, bool recursive);//布尔参数为true可删除非空目录
        //    Directory.Exists(string path);//确定路径是否存在
        //    Directory.GetCreationTime(string path);//获取目录创建日期和时间
        //    Directory.GetCurrentDirectory();//获取应用程序当前的工作目录
        //    Directory.GetDirectories(string path);//返回指定目录所有子目录名称，包括路径
        //    Directory.GetFiles(string path);//获取指定目录中所有文件的名称，包括路径
        //    Directory.GetFileSystemEntries(string path);//获取指定路径中所有的文件和子目录名称
        //    Directory.GetLastAccessTime(string path);//获取上次访问指定文件或目录的时间和日期
        //    Directory.GetLastWriteTime(string path);//返回上次写入指定文件或目录的时间和日期
        //    Directory.GetParent(string path);//检索指定路径的父目录，包括相对路径和绝对路径
        //    Directory.Move(string soureDirName, string destName);//将文件或目录及其内容移到新的位置
        //    Directory.SetCreationTime(string path);//为指定的目录或文件设置创建时间和日期
        //    Directory.SetCurrentDirectory(string path);//将应用程序工作的当前路径设为指定路径
        //    Directory.SetLastAccessTime(string path);//为指定的目录或文件设置上次访问时间和日期
        //    Directory.SetLastWriteTime(string path);//为指定的目录和文件设置上次访问时间和日期


        private void btn_Start_Click(object sender, EventArgs e)
        {
            if(HslCommunication.plc_connect_result&&TcpClient.TcpClientConnectResult)
            {
                HslCommunication._NetworkTcpDevice.Write("250", 0);
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.StartAdd, 1);
                MainThread = new Thread(MainRun);
                MainThread.IsBackground = true;
                MainThread.Start();
                btn_Start.Enabled = false;
                btn_SettingMean.Enabled = false;
                btn_SpecicationSetting.Enabled = false;
                btn_Stop.Enabled = true;
                btn_CameraSetting.Enabled = false;
                btn_Close_System.Enabled = false;
                string str = TcpClient.tcpClientSend("开始检测");
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection, 0);
            }
            else
            {
                MessageBox.Show("链接异常，请检查!");
            }    
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (HslCommunication.plc_connect_result && TcpClient.TcpClientConnectResult)
            {
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.StartAdd, 0);
                MainThread.Abort();
                btn_Start.Enabled = true;
                btn_SettingMean.Enabled = true;
                btn_Stop.Enabled = false;
                btn_Close_System.Enabled = true;
                btn_SpecicationSetting.Enabled = true;
                btn_CameraSetting.Enabled = true;
                string str = TcpClient.tcpClientSend("停止检测");
            }
            else
            {
                MessageBox.Show("链接异常，请检查!");
            }
        }

        private void uiDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void uiDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,e.RowBounds.Location.Y,uiDataGridView1.RowHeadersWidth - 4,e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,(e.RowIndex + 1).ToString(),uiDataGridView1.RowHeadersDefaultCellStyle.Font, rectangle,uiDataGridView1.RowHeadersDefaultCellStyle.ForeColor,TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void button1_Click(object sender, EventArgs e)
        {         
            for (int i = 0; i < 5; i++)
            {
                count[i] = 0;
                count2[i] = 0;
                uiDataGridView1.Rows[i + 11].Cells[1].Value = count[i];
                uiDataGridView1.Rows[i + 11].Cells[2].Value = count2[i];
            }
        }

        private void btn_SpecicationSetting_Click(object sender, EventArgs e)
        {
			formloadIndex = 2;
			FormLogin flg = new FormLogin();
			flg.ShowDialog();
		}

        private void btn_CameraSetting_Click(object sender, EventArgs e)
        {
            formloadIndex = 3;
            FormLogin flg = new FormLogin();
            flg.ShowDialog();
        }
    }
}
