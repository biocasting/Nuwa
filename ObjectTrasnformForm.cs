using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nuwa.xClass;

namespace Nuwa
{
    public partial class ObjectTrasnformForm : Form
    {
        public int TransformMode = 0;
        public xPoint3 xyz = new xPoint3(0,0,0);
        public ObjectTrasnformForm()
        {
            InitializeComponent();
        }

        public double X
        {
            get { return Convert.ToDouble(this.tsTextBoxTranformX.Text); }
        }

        public bool Changed_X
        {
            get { return true; }
        }

        public double Y
        {
            get { return Convert.ToDouble(this.tsTextBoxTranformY.Text); }
        }

        public double Z
        {
            get { return Convert.ToDouble(this.tsTextBoxTranformZ.Text); }
        }

        private void ObjectTrasnformForm_Load(object sender, EventArgs e)
        {
            UpdatePara();
        }

        public void UpdateDimesion()
        {
            this.tsTextBoxTranformX.Text = MainForm.SelectedModule.Dimesion[0].ToString("f3");
            this.tsTextBoxTranformY.Text = MainForm.SelectedModule.Dimesion[1].ToString("f3");
            this.tsTextBoxTranformZ.Text = MainForm.SelectedModule.Dimesion[2].ToString("f3");
        }


        public void UpdatePosition()
        {
            this.tsTextBoxTranformX.Text = MainForm.SelectedModule.Center[0].ToString("f3");
            this.tsTextBoxTranformY.Text = MainForm.SelectedModule.Center[1].ToString("f3");
            this.tsTextBoxTranformZ.Text = MainForm.SelectedModule.Center[2].ToString("f3");
        }

        public void UpdateAngle()
        {
            this.tsTextBoxTranformX.Text = MainForm.SelectedModule.Angle[0].ToString("f3");
            this.tsTextBoxTranformY.Text = MainForm.SelectedModule.Angle[1].ToString("f3");
            this.tsTextBoxTranformZ.Text = MainForm.SelectedModule.Angle[2].ToString("f3");
        }

        private void UpdatePara()
        {
            switch (TransformMode)
            {
                case 0:
                    UpdateDimesion();
                    break;

                case 1:
                    UpdatePosition();
                    break;

                case 2:
                    UpdateAngle();
                    break;
            }

        }

    }
}
