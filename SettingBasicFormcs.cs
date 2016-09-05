using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nuwa
{
    public partial class SettingBasicFormcs : Form
    {
        public SettingBasicFormcs()
        {
            InitializeComponent();
        }

        private void buttonSBOK_Click(object sender, EventArgs e)
        {
            Config.LineSpacing = Convert.ToDouble(textBoxLineSpacing.Text);
            Config.LineHeight = Convert.ToDouble(textBoxLineHeight.Text);
            Config.BuildSpeed = Convert.ToDouble(textBoxBuildSpeed.Text);
        }

        private void SettingBasicFormcs_Load(object sender, EventArgs e)
        {
            textBoxLineHeight.Text  = Config.LineHeight.ToString();
            textBoxLineSpacing.Text= Config.LineSpacing.ToString();
            textBoxBuildSpeed.Text = Config.BuildSpeed.ToString();
        }

        private void textBoxLineHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
            {
                e.Handled = true;
            }
        }

        private void textBoxLineSpacing_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
            {
                e.Handled = true;
            }
        }

        private void textBoxBuildSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
            {
                e.Handled = true;
            }
        }
    }
}
