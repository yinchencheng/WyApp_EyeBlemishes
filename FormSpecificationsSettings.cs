using System;
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
using static WY_App.Utility.Parameter;
using Parameter = WY_App.Utility.Parameter;

namespace WY_App
{
    public partial class FormSpecificationsSettings : Form
    {
        public FormSpecificationsSettings()
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
            num_总宽max.Enabled = true;
            num_总长max.Enabled = true;
            num_料宽max.Enabled = true;
            num_胶宽max.Enabled = true;
            num_长端max.Enabled = true;

            num_左短端max.Enabled = true;
            num_右短端max.Enabled = true;
            num_左肩宽max.Enabled = true;
            num_右肩宽max.Enabled = true;
            num_左肩高max.Enabled = true;
            num_右肩高max.Enabled = true;

            num_总宽value.Enabled = true;
            num_总长value.Enabled = true;
            num_料宽value.Enabled = true;
            num_胶宽value.Enabled = true;
            num_长端value.Enabled = true;

            num_左短端value.Enabled = true;
            num_右短端value.Enabled = true;
            num_左肩宽value.Enabled = true;
            num_右肩宽value.Enabled = true;
            num_左肩高value.Enabled = true;
            num_右肩高value.Enabled = true;

            num_总宽min.Enabled = true;
            num_总长min.Enabled = true;
            num_料宽min.Enabled = true;
            num_胶宽min.Enabled = true;
            num_长端min.Enabled = true;

            num_左短端min.Enabled = true;
            num_右短端min.Enabled = true;
            num_左肩宽min.Enabled = true;
            num_右肩宽min.Enabled = true;
            num_左肩高min.Enabled = true;
            num_右肩高min.Enabled = true;

            num_总宽adjustZ.Enabled = true;
            num_总长adjustZ.Enabled = true;
            num_料宽adjustZ.Enabled = true;
            num_胶宽adjustZ.Enabled = true;
            num_长端adjustZ.Enabled = true;

            num_左短端adjustZ.Enabled = true;
            num_右短端adjustZ.Enabled = true;
            num_左肩宽adjustZ.Enabled = true;
            num_右肩宽adjustZ.Enabled = true;
            num_左肩高adjustZ.Enabled = true;
            num_右肩高adjustZ.Enabled = true;

            num_总宽adjustF.Enabled = true;
            num_总长adjustF.Enabled = true;
            num_料宽adjustF.Enabled = true;
            num_胶宽adjustF.Enabled = true;
            num_长端adjustF.Enabled = true;

            num_左短端adjustF.Enabled = true;
            num_右短端adjustF.Enabled = true;
            num_左肩宽adjustF.Enabled = true;
            num_右肩宽adjustF.Enabled = true;
            num_左肩高adjustF.Enabled = true;
            num_右肩高adjustF.Enabled = true;
            btn_Save.Enabled = true;

        }

        private void ParamSettings_Load(object sender, EventArgs e)
        {
            num_总宽max.Value = Parameter.specifications.总宽.max;
            num_总长max.Value = Parameter.specifications.总长.max;
            num_料宽max.Value = Parameter.specifications.料宽.max;
            num_胶宽max.Value = Parameter.specifications.胶宽.max;
            num_长端max.Value = Parameter.specifications.长端.max;

            num_左短端max.Value = Parameter.specifications.左短端.max;
            num_右短端max.Value = Parameter.specifications.右短端.max;
            num_左肩宽max.Value = Parameter.specifications.左肩宽.max;
            num_右肩宽max.Value = Parameter.specifications.右肩宽.max;
            num_左肩高max.Value = Parameter.specifications.左肩高.max;
            num_右肩高max.Value = Parameter.specifications.右肩高.max;

            num_总宽value.Value = Parameter.specifications.总宽.value;
            num_总长value.Value = Parameter.specifications.总长.value;
            num_料宽value.Value = Parameter.specifications.料宽.value;
            num_胶宽value.Value = Parameter.specifications.胶宽.value;
            num_长端value.Value = Parameter.specifications.长端.value;

            num_左短端value.Value = Parameter.specifications.左短端.value;
            num_右短端value.Value = Parameter.specifications.右短端.value;
            num_左肩宽value.Value = Parameter.specifications.左肩宽.value;
            num_右肩宽value.Value = Parameter.specifications.右肩宽.value;
            num_左肩高value.Value = Parameter.specifications.左肩高.value;
            num_右肩高value.Value = Parameter.specifications.右肩高.value;

            num_总宽min.Value = Parameter.specifications.总宽.min;
            num_总长min.Value = Parameter.specifications.总长.min;
            num_料宽min.Value = Parameter.specifications.料宽.min;
            num_胶宽min.Value = Parameter.specifications.胶宽.min;
            num_长端min.Value = Parameter.specifications.长端.min;

            num_左短端min.Value = Parameter.specifications.左短端.min;
            num_右短端min.Value = Parameter.specifications.右短端.min;
            num_左肩宽min.Value = Parameter.specifications.左肩宽.min;
            num_右肩宽min.Value = Parameter.specifications.右肩宽.min;
            num_左肩高min.Value = Parameter.specifications.左肩高.min;
            num_右肩高min.Value = Parameter.specifications.右肩高.min;

            num_总宽adjustZ.Value = Parameter.specifications.总宽.adjustZ;
            num_总长adjustZ.Value = Parameter.specifications.总长.adjustZ;
            num_料宽adjustZ.Value = Parameter.specifications.料宽.adjustZ;
            num_胶宽adjustZ.Value = Parameter.specifications.胶宽.adjustZ;
            num_长端adjustZ.Value = Parameter.specifications.长端.adjustZ;

            num_左短端adjustZ.Value = Parameter.specifications.左短端.adjustZ;
            num_右短端adjustZ.Value = Parameter.specifications.右短端.adjustZ;
            num_左肩宽adjustZ.Value = Parameter.specifications.左肩宽.adjustZ;
            num_右肩宽adjustZ.Value = Parameter.specifications.右肩宽.adjustZ;
            num_左肩高adjustZ.Value = Parameter.specifications.左肩高.adjustZ;
            num_右肩高adjustZ.Value = Parameter.specifications.右肩高.adjustZ;

            num_总宽adjustF.Value = Parameter.specifications.总宽.adjustF;
            num_总长adjustF.Value = Parameter.specifications.总长.adjustF;
            num_料宽adjustF.Value = Parameter.specifications.料宽.adjustF;
            num_胶宽adjustF.Value = Parameter.specifications.胶宽.adjustF;
            num_长端adjustF.Value = Parameter.specifications.长端.adjustF;

            num_左短端adjustF.Value = Parameter.specifications.左短端.adjustF;
            num_右短端adjustF.Value = Parameter.specifications.右短端.adjustF;
            num_左肩宽adjustF.Value = Parameter.specifications.左肩宽.adjustF;
            num_右肩宽adjustF.Value = Parameter.specifications.右肩宽.adjustF;
            num_左肩高adjustF.Value = Parameter.specifications.左肩高.adjustF;
            num_右肩高adjustF.Value = Parameter.specifications.右肩高.adjustF;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Parameter.specifications.总宽.max = num_总宽max.Value;
            Parameter.specifications.总长.max = num_总长max.Value;
            Parameter.specifications.料宽.max = num_料宽max.Value;
            Parameter.specifications.胶宽.max = num_胶宽max.Value;
            Parameter.specifications.长端.max = num_长端max.Value;
            Parameter.specifications.左短端.max = num_左短端max.Value;
            Parameter.specifications.右短端.max = num_右短端max.Value;
            Parameter.specifications.左肩宽.max = num_左肩宽max.Value;
            Parameter.specifications.右肩宽.max = num_右肩宽max.Value;
            Parameter.specifications.左肩高.max = num_左肩高max.Value;
            Parameter.specifications.右肩高.max = num_右肩高max.Value;

            Parameter.specifications.总宽.value = num_总宽value.Value;
            Parameter.specifications.总长.value = num_总长value.Value;
            Parameter.specifications.料宽.value = num_料宽value.Value;
            Parameter.specifications.胶宽.value = num_胶宽value.Value;
            Parameter.specifications.长端.value = num_长端value.Value;
            Parameter.specifications.左短端.value = num_左短端value.Value;
            Parameter.specifications.右短端.value = num_右短端value.Value;
            Parameter.specifications.左肩宽.value = num_左肩宽value.Value;
            Parameter.specifications.右肩宽.value = num_右肩宽value.Value;
            Parameter.specifications.左肩高.value = num_左肩高value.Value;
            Parameter.specifications.右肩高.value = num_右肩高value.Value;

            Parameter.specifications.总宽.min = num_总宽min.Value;
            Parameter.specifications.总长.min = num_总长min.Value;
            Parameter.specifications.料宽.min = num_料宽min.Value;
            Parameter.specifications.胶宽.min = num_胶宽min.Value;
            Parameter.specifications.长端.min = num_长端min.Value;
            Parameter.specifications.左短端.min = num_左短端min.Value;
            Parameter.specifications.右短端.min = num_右短端min.Value;
            Parameter.specifications.左肩宽.min = num_左肩宽min.Value;
            Parameter.specifications.右肩宽.min = num_右肩宽min.Value;
            Parameter.specifications.左肩高.min = num_左肩高min.Value;
            Parameter.specifications.右肩高.min = num_右肩高min.Value;

            Parameter.specifications.总宽.adjustZ = num_总宽adjustZ.Value;
            Parameter.specifications.总长.adjustZ = num_总长adjustZ.Value;
            Parameter.specifications.料宽.adjustZ = num_料宽adjustZ.Value;
            Parameter.specifications.胶宽.adjustZ = num_胶宽adjustZ.Value;
            Parameter.specifications.长端.adjustZ = num_长端adjustZ.Value;
            Parameter.specifications.左短端.adjustZ = num_左短端adjustZ.Value;
            Parameter.specifications.右短端.adjustZ = num_右短端adjustZ.Value;
            Parameter.specifications.左肩宽.adjustZ = num_左肩宽adjustZ.Value;
            Parameter.specifications.右肩宽.adjustZ = num_右肩宽adjustZ.Value;
            Parameter.specifications.左肩高.adjustZ = num_左肩高adjustZ.Value;
            Parameter.specifications.右肩高.adjustZ = num_右肩高adjustZ.Value;

            Parameter.specifications.总宽.adjustF = num_总宽adjustF.Value;
            Parameter.specifications.总长.adjustF = num_总长adjustF.Value;
            Parameter.specifications.料宽.adjustF = num_料宽adjustF.Value;
            Parameter.specifications.胶宽.adjustF = num_胶宽adjustF.Value;
            Parameter.specifications.长端.adjustF = num_长端adjustF.Value;
            Parameter.specifications.左短端.adjustF = num_左短端adjustF.Value;
            Parameter.specifications.右短端.adjustF = num_右短端adjustF.Value;
            Parameter.specifications.左肩宽.adjustF = num_左肩宽adjustF.Value;
            Parameter.specifications.右肩宽.adjustF = num_右肩宽adjustF.Value;
            Parameter.specifications.左肩高.adjustF = num_左肩高adjustF.Value;
            Parameter.specifications.右肩高.adjustF = num_右肩高adjustF.Value;

            FormMain.specifications = new Parameter.specification[11] {
            Parameter.specifications.总宽, Parameter.specifications.总长, Parameter.specifications.料宽, Parameter.specifications.胶宽,
            Parameter.specifications.长端, Parameter.specifications.左短端, Parameter.specifications.右短端, Parameter.specifications.左肩宽,
            Parameter.specifications.右肩宽, Parameter.specifications.左肩高, Parameter.specifications.右肩高};
            XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, "Specifications.xml");
            MessageBox.Show("系统参数修改，请重启软件");
            this.Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
