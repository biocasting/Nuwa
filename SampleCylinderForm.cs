using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using Nuwa.xClass;

namespace Nuwa
{
    public partial class SampleCylinderForm : Form
    {
        public SampleCylinderForm()
        {
            InitializeComponent();
        }

        private void SampleTubeForm_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }


        private void buttonCYOK_Click(object sender, EventArgs e)
        {
            xSample.Path.Clear();
            xSample.LiString = "";
            try
            {
                double D1 = double.Parse(textBoxCYD1.Text);
                double D2 = double.Parse(textBoxCYD2.Text);
                double H = double.Parse(textBoxCYH.Text);
                xSample.CylinderContourFill(D1, D2, H);
            }
            catch (Exception ex)
            {
                labelCY1.Text = ex.Message; 
            }
        }
    }
}
