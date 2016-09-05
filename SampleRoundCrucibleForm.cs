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
    public partial class SampleRoundCrucibleForm : Form
    {
        public SampleRoundCrucibleForm()
        {
            InitializeComponent();
        }

        private void buttonRCOK_Click(object sender, EventArgs e)
        {
            xSample.Path.Clear();
            xSample.LiString = "";
            try
            {
                double D1 = double.Parse(textBoxRCD1.Text);
                double D2 = double.Parse(textBoxRCD2.Text);
                double H1 = double.Parse(textBoxRCH1.Text);
                double H2 = double.Parse(textBoxRCH2.Text);
                xSample.RoundCrucibleContourFill(D1, D2, H1, H2);
            }
            catch (Exception ex)
            {
                labelRC1.Text = ex.Message;
            }

        }
    }
}
