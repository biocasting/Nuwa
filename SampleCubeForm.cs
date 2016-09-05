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
    public partial class SampleCubeForm : Form
    {
        public SampleCubeForm()
        {
            InitializeComponent();
        }

        private void ButtonCUOK_Click(object sender, EventArgs e)
        {
            xSample.Path.Clear();
             try
             {
                 double L = double.Parse(textBoxCUL.Text);
                 double W = double.Parse(textBoxCUW.Text);
                 double H = double.Parse(textBoxCUH.Text);
                 if (comboBoxCUF.SelectedIndex == 1)
                    xSample.CubeCoutourFill(L, W, H);
                 else
                    xSample.CubeRasterFill(L, W, H);
             }
             catch (Exception ex)
             {
                 labelCU1.Text = ex.Message;
             }
        }

        private void SampleCubeForm_Load(object sender, EventArgs e)
        {
            comboBoxCUF.SelectedIndex = 0;
        }
    }
}
