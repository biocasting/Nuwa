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
    public partial class SampleSquareCrucibleForm : Form
    {
        public SampleSquareCrucibleForm()
        {
            InitializeComponent();
        }


        private void buttonSCOK_Click(object sender, EventArgs e)
        {
            xSample.Path.Clear();
            xSample.LiString = "";
            try
            {
                double L = double.Parse(textBoxSCL.Text);
                double W = double.Parse(textBoxSCW.Text);
                double T = double.Parse(textBoxSCT.Text);
                double H1 = double.Parse(textBoxSCH1.Text);
                double H2 = double.Parse(textBoxSCH2.Text);
                xSample.SquareCrucibleCoutourFill(L, W, T,H1, H2);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
