﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using WY_App.Utility;
using Parameter = WY_App.Utility.Parameter;

namespace WY_App
{
    public partial class 通讯设置 : Form
    {
        public 通讯设置()
        {
            InitializeComponent();
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        Point downPoint;
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void btn_Change_Click(object sender, EventArgs e)
        {
            txt_PlcIpAddress.Enabled = true;
            num_PLCPort.Enabled = true;
            txt_PlcDevice.Enabled = true;
            txt_PlcType.Enabled = true;
            chk_PLCEnabled.Enabled = true;

            txt_ServerIP.Enabled = true;
            num_ServerPort.Enabled = true;
            chk_ServerEnabled.Enabled = true;

            txt_ClientIP.Enabled = true;
            num_ClientPort.Enabled = true;
            chk_ClientEnabled.Enabled = true;

            txt_Trigger_Detection1.Enabled = true;
            txt_Trigger_Detection2.Enabled = true;
            txt_Trigger_Detection3.Enabled = true;
            txt_StartAdd.Enabled = true;
            txt_HeartBeatAdd.Enabled = true;
            txt_Completion_Add1.Enabled = true;
            txt_Completion_Add2.Enabled = true;
            txt_Completion_Add3.Enabled = true;
            num_LogSaveDays.Enabled = true;

            txt_ImagePath.Enabled = true;
            txt_ImageSavePath.Enabled = true;
            btn_Save.Enabled = true;
            Position1.Enabled = true;
            Position2.Enabled = true;
            Position3.Enabled = true;
            Position4.Enabled = true;
            DoubleUpDown1.Enabled = true;
            DoubleUpDown2.Enabled = true;
            DoubleUpDown3.Enabled = true;
            DoubleUpDown4.Enabled = true;
        }

        private void ParamSettings_Load(object sender, EventArgs e)
        {
            txt_PlcIpAddress.Text = Parameter.commministion.PlcIpAddress;
            num_PLCPort.Value = Parameter.commministion.PlcIpPort;
            txt_PlcDevice.Text = Parameter.commministion.PlcDevice;
            txt_PlcType.Text = Parameter.commministion.PlcType;
            chk_PLCEnabled.Checked = Parameter.commministion.PlcEnable;

            txt_ImagePath.Text = Parameter.commministion.ImagePath;
            txt_ImageSavePath.Text = Parameter.commministion.ImageSavePath;

            txt_ServerIP.Text = Parameter.commministion.TcpServerIpAddress;
            num_ServerPort.Value = Parameter.commministion.TcpServerIpPort;
            chk_ServerEnabled.Checked = Parameter.commministion.TcpServerEnable;

            txt_ClientIP.Text = Parameter.commministion.TcpClientIpAddress;
            num_ClientPort.Value = Parameter.commministion.TcpClientIpPort;
            chk_ClientEnabled.Checked = Parameter.commministion.TcpClientEnable;

            num_LogSaveDays.Value = Parameter.commministion.LogFileExistDay;

            txt_Trigger_Detection1.Text = Parameter.plcParams.Trigger_Detection1;
            txt_Trigger_Detection2.Text = Parameter.plcParams.Trigger_Detection2;
            txt_Trigger_Detection3.Text = Parameter.plcParams.Trigger_Detection3;
            txt_StartAdd.Text = Parameter.plcParams.StartAdd;
            txt_Completion_Add1.Text = Parameter.plcParams.Completion1;
            txt_Completion_Add2.Text = Parameter.plcParams.Completion2;
            txt_Completion_Add3.Text = Parameter.plcParams.Completion3;
            txt_HeartBeatAdd.Text = Parameter.plcParams.HeartBeatAdd;

            Position1.Text=Parameter.plcParams.Position1;
            Position2.Text=Parameter.plcParams.Position2;
            Position3.Text=Parameter.plcParams.Position3;
            Position4.Text=Parameter.plcParams.Position4;

            DoubleUpDown1.Value=Parameter.plcParams.Position1Value;
            DoubleUpDown2.Value=Parameter.plcParams.Position2Value;
            DoubleUpDown3.Value=Parameter.plcParams.Position3Value;
            DoubleUpDown4.Value=Parameter.plcParams.Position4Value;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Parameter.commministion.PlcIpAddress = txt_PlcIpAddress.Text;
            Parameter.commministion.PlcIpPort = (int)num_PLCPort.Value;
            Parameter.commministion.PlcDevice = txt_PlcDevice.Text;
            Parameter.commministion.PlcType = txt_PlcType.Text;
            Parameter.commministion.PlcEnable = chk_PLCEnabled.Checked;

            Parameter.commministion.ImagePath = txt_ImagePath.Text;
            Parameter.commministion.ImageSavePath = txt_ImageSavePath.Text; ;

            Parameter.commministion.TcpServerIpAddress = txt_ServerIP.Text;
            Parameter.commministion.TcpServerIpPort = (int)num_ServerPort.Value;
            Parameter.commministion.TcpServerEnable = chk_ServerEnabled.Checked;

            Parameter.commministion.TcpClientIpAddress = txt_ClientIP.Text;
            Parameter.commministion.TcpClientIpPort = (int)num_ClientPort.Value;
            Parameter.commministion.TcpClientEnable = chk_ClientEnabled.Checked;

            Parameter.commministion.LogFileExistDay = (int)num_LogSaveDays.Value;

            XMLHelper.serialize<Parameter.Commministion>(Parameter.commministion, "Parameter/Commministion.xml");

            Parameter.plcParams.StartAdd = txt_StartAdd.Text;
            Parameter.plcParams.HeartBeatAdd = txt_HeartBeatAdd.Text;
            Parameter.plcParams.Trigger_Detection1 = txt_Trigger_Detection1.Text;
            Parameter.plcParams.Trigger_Detection2 = txt_Trigger_Detection2.Text;
            Parameter.plcParams.Trigger_Detection3 = txt_Trigger_Detection3.Text;
            
            Parameter.plcParams.Completion1 = txt_Completion_Add1.Text;
            Parameter.plcParams.Completion2 = txt_Completion_Add2.Text;
            Parameter.plcParams.Completion3 = txt_Completion_Add3.Text;

            Parameter.plcParams.Position1 = Position1.Text;
            Parameter.plcParams.Position2 = Position2.Text;
            Parameter.plcParams.Position3 = Position3.Text;
            Parameter.plcParams.Position4 = Position4.Text;
            Parameter.plcParams.Position1Value = (int)DoubleUpDown1.Value;
            Parameter.plcParams.Position2Value = (int)DoubleUpDown2.Value;
            Parameter.plcParams.Position3Value = (int)DoubleUpDown3.Value;
            Parameter.plcParams.Position4Value = (int)DoubleUpDown4.Value;
            HslCommunication.Write(Parameter.plcParams.Position1, (int)DoubleUpDown1.Value);
            HslCommunication.Write(Parameter.plcParams.Position2, (int)DoubleUpDown2.Value);
            HslCommunication.Write(Parameter.plcParams.Position3, (int)DoubleUpDown3.Value);
            HslCommunication.Write(Parameter.plcParams.Position4, (int)DoubleUpDown4.Value);
            XMLHelper.serialize<Parameter.PLCParams>(Parameter.plcParams, "Parameter/PLCParams.xml");
            MessageBox.Show("系统参数修改，请重启软件");
            this.Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
