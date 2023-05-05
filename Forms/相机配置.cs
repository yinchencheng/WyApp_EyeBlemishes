using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WY_App.Utility;

namespace WY_App
{
    public partial class 相机配置 : Form
    {
        public 相机配置()
        {
            InitializeComponent();
        }

        private void 修改_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            txt_gain.Enabled = true;
            txt_Shutter.Enabled = true;
        }

        private void 保存_Click(object sender, EventArgs e)
        {
            try
            {
                Parameter.cameraParam.Gain[comboBox1.SelectedIndex]= (double)txt_gain.Value;
                Parameter.cameraParam.Shutter[comboBox1.SelectedIndex] = (double)txt_Shutter.Value;
                Halcon.SetFramegrabberParam(comboBox1.SelectedIndex, Halcon.hv_AcqHandle[comboBox1.SelectedIndex]);
                XMLHelper.serialize<Parameter.CameraParam>(Parameter.cameraParam, Parameter.commministion.productName + "/CameraParam.xml");
            }
            catch
            {
                MessageBox.Show("数据保存失败！");
            }
            this.Close();
        }

        private void Phi_TextChanged(object sender, EventArgs e)
        {

        }

        private void 相机配置_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            txt_gain.Value = (decimal)Parameter.cameraParam.Gain[0];
            txt_Shutter.Value = (decimal)Parameter.cameraParam.Shutter[0];
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_gain.Value = (decimal)Parameter.cameraParam.Gain[comboBox1.SelectedIndex];
            txt_Shutter.Value = (decimal)Parameter.cameraParam.Shutter[comboBox1.SelectedIndex];
        }
    }
}
