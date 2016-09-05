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
    public partial class ControlForm : Form
    {

        private int index = 1;
        public ControlForm()
        {
            InitializeComponent();
        }

        private void buttonControlXLeft_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("X", -1*Config.JogSpeed);
        }

        private void buttonControlXLeft_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlXRight_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("X", Config.JogSpeed);
        }

        private void buttonControlXRight_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlYOut_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("Y", -1*Config.JogSpeed);
        }

        private void buttonControlYOut_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlYIn_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("Y", Config.JogSpeed);
        }

        private void buttonControlYIn_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlZUp_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("Z", Config.JogSpeed);
        }

        private void buttonControlZUp_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlDPull_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("D", -1*Config.JogSpeed);
        }

        private void buttonControlDPull_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlDPush_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("D", Config.JogSpeed);
        }

        private void buttonControlDPush_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlZDown_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("Z", -1 * Config.JogSpeed);
        }

        private void buttonControlZDown_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.galil.Jog("STOP", 0);
        }

        private void buttonControlSetXto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.SetAxisToZero(0);
        }

        private void buttonControlSetYto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.SetAxisToZero(1);
        }


        private void buttonControlSetZto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.SetAxisToZero(2);
        }

        private void buttonControlSetDto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.SetAxisToZero(3);
        }

        private void buttonControlMoveXto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.MoveAxisToZero(0);
        }

        private void buttonControlMoveYto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.MoveAxisToZero(1);
        }

        private void buttonControlMoveZto0_Click(object sender, EventArgs e)
        {
            MainForm.galil.MoveAxisToZero(2);
        }

        private void buttonSettingSpeedLevel_Click(object sender, EventArgs e)
        {
            index++;
            if (index == 4)
                index = 1;
            switch (index) 
            { 
            
                case 1:
                    Config.JogSpeed = 0.1;
                    buttonSettingSpeedLevel.Image = global::Nuwa.Properties.Resources.speed1;
                        break;
                case 2:
                    Config.JogSpeed = 1;
                    buttonSettingSpeedLevel.Image = global::Nuwa.Properties.Resources.speed2;
                        break;
                case 3:
                    Config.JogSpeed = 10;
                    buttonSettingSpeedLevel.Image = global::Nuwa.Properties.Resources.speed3;
                        break;
            }


        }


    }
}
