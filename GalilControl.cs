using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa
{
    public class GalilControl
    {

        #region 成员
        public static int XCountPmm = 1000; // 每mm多少脉冲， 这里是一个脉冲一个微米
        public static int YCountPmm = 1000;
        public static int ZCountPmm = 1000;
        public static int ECountPnL = 100; // 每nL多少脉冲，即100个脉冲一个nL
        public static int DCountPuL = 120; // 每uL多少脉冲，即120个脉冲一个uL
        public static int TimeInterval = 100; // ms;

        private Galil.Galil g;
        private static string GALIL_ADDRESS = "192.168.1.8";
        private string response = "";
        private double[] commandPosition = { 0, 0, 0, 0, 0, 0, 0, 0 };// ABCD EFGH 8轴命令位置  TP
        private double[] machinePosition = { 0, 0, 0};// ABCD EFGH 8轴命令位置  TP
        private double[] commandPositionOld = { 0, 0, 0, 0, 0, 0, 0, 0 };  // ABCD EFGH 8轴命令位置  TP
        private double[] axisSpeed = { 0, 0, 0, 0, 0, 0, 0, 0 };  // ABCD EFGH 8轴命令位置  TP
        private double[] encoderPosition = { 0, 0, 0, 0, 0, 0, 0, 0 };  // ABCD EFGH 8轴编码器位置  RP
        private double[] OldPosition = { 0, 0, 0, 0, 0, 0, 0, 0 };  // ABCD EFGH 8轴编码器位置  RP
        private bool[] enableStatus = { false, false, false, false, false, false, false, false };  // ABCD EFGH 8轴编码器位置  RP
        public double S = 0;//tDistance = 0;
        private bool isGalilConnected = false;
        private double jogSpeed = 0, moveSpeed = 0, moveDistance = 0;
        private string jogAxis = "";
        private System.Windows.Forms.ToolStripLabel timeLabel = null;

        #endregion

        #region 属性
        // 命令位置
        public double X
        {
            get { return commandPosition[0]; }
        }

        public double Y
        {
            get { return commandPosition[1]; }
        }

        public double Z
        {
            get { return commandPosition[2]; }
        }

        // 编码器位置
        public double Xe
        {
            get { return encoderPosition[0]; }
        }

        public double Ye
        {
            get { return encoderPosition[1]; }
        }

        public double Ze
        {
            get { return encoderPosition[2]; }
        }

        // 机器位置
        public double Xm // 机器位置
        {
            get { return machinePosition[0] + commandPosition[0]; }
        }

        public double Ym
        {
            get { return machinePosition[1] + commandPosition[0]; }
        }

        public double Zm
        {
            get { return machinePosition[2] + commandPosition[0]; }
        }


        // 机器位置
        public double Xom // 老的机器位置
        {
            get { return machinePosition[3]; }
        }

        public double Yom
        {
            get { return machinePosition[4]; }
        }

        public double Zom
        {
            get { return machinePosition[5]; }
        }

        public double D  // nl单位
        {
            get { return commandPosition[3]; }
        }

        public double E  // nl单位
        {
            get { return commandPosition[4]; }
        }

        public double F // nl单位
        {
            get { return commandPosition[5]; }
        }

        public bool[] EnableStatus
        {
            get { return this.enableStatus; }
        }

        public double JogSpeed // mm/s
        {
            set { this.jogSpeed = value; }
        }

        public double MoveSpeed // mm/s
        {
            set { this.moveSpeed = value; }
        }

       public double Es // nL/s
        {
            get { return axisSpeed[4]; }
        }

       public double Ds // uL/s
       {
           get { return axisSpeed[3]; }
       }

        public double MoveDistance // mm/s
        {
            set { this.moveDistance = value; }
        }

        public bool IsGalilConnected
        {
            get { return this.isGalilConnected; }
            set { this.isGalilConnected = value; }
        }

        public string Response
        {
            get { return this.response; }
        }

        public System.Windows.Forms.ToolStripLabel TimeLabel
        {
            set { this.timeLabel = value; }
        }

        #endregion

        public GalilControl()
        {
            this.g = new Galil.Galil();
            //this.g = null;//new Galil.Galil();
        }

        public void SetCommandPosition(double a, double b, double c, double d, double e, double f, double g, double h)
        {
            commandPosition[0] = a; commandPosition[1] = b; commandPosition[2] = c; commandPosition[3] = d;
            commandPosition[4] = e; commandPosition[5] = f; commandPosition[6] = g; commandPosition[7] = h;
        }

        public void SetEncoderPosition(double a, double b, double c, double d, double e, double f, double g, double h)
        {
            encoderPosition[0] = a; encoderPosition[1] = b; encoderPosition[2] = c; encoderPosition[3] = d;
            encoderPosition[4] = e; encoderPosition[5] = f; encoderPosition[6] = e; encoderPosition[7] = f;

        }

        public void SetEnableStatus(bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h)
        {
            enableStatus[0] = a; enableStatus[1] = b; enableStatus[2] = c; enableStatus[3] = d;
            enableStatus[4] = e; enableStatus[5] = f; enableStatus[6] = e; enableStatus[7] = f;

        }

        public void UpdateEnableStatus()
        {
            try
            {
                SetEnableStatus(g.commandValue("MG _MOA") == 0,
                                          g.commandValue("MG _MOB") == 0,
                                          g.commandValue("MG _MOC") == 0,
                                          g.commandValue("MG _MOD") == 0,
                                          g.commandValue("MG _MOE") == 0,
                                          g.commandValue("MG _MOF") == 0,
                                          g.commandValue("MG _MOG") == 0,
                                          g.commandValue("MG _MOH") == 0);
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
        }

        public void StartRecord()
        {
            if (!this.isGalilConnected)
                return;
            g.onRecord += new Galil.Events_onRecordEventHandler(g_onRecord); //hook up to event
            g.recordsStart(TimeInterval); //start records at 100 ms period
        }

        public void StopRecord()
        {
            if (!this.isGalilConnected)
                return;
            g.recordsStart(0);
            g.onRecord -= new Galil.Events_onRecordEventHandler(g_onRecord); //hook up to event
        }

        public void Command(string strCommand)
        {
            if (strCommand == "")
                return;
            try
            {
                g.command(strCommand);
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }
        }

        private void g_onRecord(object record)
        {
            if (this.timeLabel != null)
                this.timeLabel.Text = g.sourceValue(record, "TIME").ToString();
            // 命令位置更新
            SetCommandPosition(g.sourceValue(record, "_RPA") / (double)XCountPmm,
                                                g.sourceValue(record, "_RPB") / (double)YCountPmm,
                                                g.sourceValue(record, "_RPC") / (double)ZCountPmm,
                                                g.sourceValue(record, "_RPD") / (double)DCountPuL, // uL
                                                g.sourceValue(record, "_RPE") / (double)ECountPnL, // nL
                                                0, 0, 0);
            // 编码器位置更新 
            SetEncoderPosition(g.sourceValue(record, "_TPA") / (XCountPmm * 1.6384),
                                            g.sourceValue(record, "_TPB") / (YCountPmm * -1.6384),
                                            g.sourceValue(record, "_TPC") / (ZCountPmm * -1.6384),
                                            0, 0, 0, 0, 0);
            this.CalSpeed();
            //S距离更新
            this.S = g.sourceValue(record, "_AVS") / XCountPmm; // mm
            Console.WriteLine("x: {0}  y: {1}   z: {2} ", Xm, Ym, Zm);
            // 执行行数更新
            // Code.ConsumedLineNumber = Convert.ToInt32(g.sourceValue(record, "_CSS"));

        }  //OnRecord 实现函数

        public void CalSpeed()
        {
            for (int i = 0; i<8;i++)
            {
                axisSpeed[i] = (commandPosition[i] - commandPositionOld[i]) *10.0; 
                commandPositionOld[i] = commandPosition[i];
            }
        }

        public bool StartConnectGalil()
        {
            ConnectGalil();
            if (!isGalilConnected)
            {
                DelayMillSec(1000);
                ConnectGalil();
                if (!isGalilConnected)
                {
                    DelayMillSec(1000);
                    ConnectGalil();
                    if (!isGalilConnected)
                    {
                        response = "连接失败。。。。\n";
                        return false;
                    }
                }
            }
            return true;
        }

        private void ConnectGalil()
        {
            try
            {
                g.address = GALIL_ADDRESS; //Ethernet
                response = g.connection();
                this.isGalilConnected = true;
            }
            catch (Exception ex)
            {
                this.isGalilConnected = false;
                response = ex.Message;
            }
        }

        public void Jog(string axis, double jogSpeed)
        {
            if (!this.isGalilConnected)
                return;
            int jogSpeedCount = 0;
            string strJogCommand = "JG ";
            switch (axis)
            {
                case "X":
                    jogSpeedCount = Convert.ToInt32(XCountPmm * jogSpeed);
                    strJogCommand += jogSpeedCount.ToString() + ";BGA";
                    this.jogAxis = "A";
                    break;

                case "Y":
                    jogSpeedCount = Convert.ToInt32(YCountPmm * jogSpeed);
                    strJogCommand += ", " + jogSpeedCount.ToString() + ";BGB";
                    this.jogAxis = "B";
                    break;

                case "Z":
                    jogSpeedCount = Convert.ToInt32(ZCountPmm * jogSpeed);
                    strJogCommand += ", ," + jogSpeedCount.ToString() + ";BGC";
                    this.jogAxis = "C";
                    break;

                case "D":
                    jogSpeedCount = Convert.ToInt32(DCountPuL * jogSpeed);
                    strJogCommand += ", , ," + jogSpeedCount.ToString() + ";BGD";
                    this.jogAxis = "D";
                    break;


                case "E":
                    jogSpeedCount = Convert.ToInt32(ECountPnL * jogSpeed);
                    strJogCommand += ", , , ," + jogSpeedCount.ToString() + ";BGE";
                    this.jogAxis = "E";
                    break;

                case "STOP":
                    strJogCommand = "ST" + this.jogAxis;
                    break;
            }
            
            try
            {
                response = g.command(strJogCommand);
                response += strJogCommand + Environment.NewLine ;
            }
            catch (Exception ex)
            {
                response = "错误：" + ex.Message;
            }
            
        }   // 生成X，Y，Z轴点动指令

        public void Move(string axis, double moveSpeed, double moveDistance)
        {
            if (!this.isGalilConnected)
                return;
            int moveSpeedCount = 0;
            int moveDistanceCount = 0;
            string strMoveCommand = "";
            switch (axis)
            {
                case "X":
                    moveSpeedCount = (int)(moveSpeed * XCountPmm) ;
                    moveDistanceCount = (int)(moveDistance * XCountPmm);
                    strMoveCommand = "SP " + moveSpeedCount.ToString() + ";PR " + moveDistanceCount.ToString() + ";BGA"; ;
                    break;

                case "Y":
                    moveSpeedCount = (int)(moveSpeed * YCountPmm) ;
                    moveDistanceCount = (int)(moveDistance * YCountPmm);
                    strMoveCommand = "SP ," + moveSpeedCount.ToString() + ";PR ," + moveDistanceCount.ToString() + ";BGB"; ;
                    break;

                case "Z":
                    moveSpeedCount = (int)(moveSpeed * ZCountPmm) ;
                    moveDistanceCount = (int)(moveDistance * ZCountPmm);
                    strMoveCommand = "SP , ," + moveSpeedCount.ToString() + ";PR , ," + moveDistanceCount.ToString() + ";BGC"; ;
                    break;

            }
            try
            {
                response = g.command(strMoveCommand);
                response += strMoveCommand + Environment.NewLine;
                DMCWaitMotionComplete(axis);
            }
            catch (Exception ex)
            {
                response = "错误：" + ex.Message;
            }


        }   // 生成X，Y，Z轴移动指令

        public void Enable(string axis, bool enable)
        {
            if (!this.isGalilConnected)
                return;
            if (enable)
                g.command("SH" + axis);
            else
                g.command("MO" + axis);
            UpdateEnableStatus();
        } //

        public void Mill(bool ccw, int level)
        {
            try
            {
                g.command("CB5;CB6;CB7;CB8"); //先恢复
                switch (level)
                {
                    case 0:
                        break;
                    case 1:
                        if (ccw)
                            g.command("SB5");
                        else
                            g.command("SB6");
                        break;
                    case 2:
                        if (ccw)
                            g.command("SB5");
                        else
                            g.command("SB6");
                        g.command("SB7");
                        break;
                    case 3:
                        if (ccw)
                            g.command("SB5");
                        else
                            g.command("SB6");
                        g.command("SB8");
                        break;
                }
            }
            catch (Exception ex)
            {
                response = "错误：" + ex.Message;
            }


        }

        public bool DelayMillSec(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Milliseconds + spand.Seconds * 1000;
                System.Windows.Forms.Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }  // 暂停毫秒 ，不得大于60,000 即一分钟。

        public void DefineCurrentAllZero()
        {
            try
            {
                this.machinePosition[0] += this.commandPosition[0];
                this.machinePosition[1] += this.commandPosition[1];
                this.machinePosition[2] += this.commandPosition[2];

                response = g.command("DP 0,0,0,0,0,0");
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }
        } //定义当前位置为零点。。。。

        public void SetAxisToZero(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        response = g.command("DP 0");
                        this.machinePosition[0] += this.commandPosition[0];
                        break;
                    case 1:
                        response = g.command("DP ,0");
                        this.machinePosition[1] += this.commandPosition[1];
                        break;
                    case 2:
                        response = g.command("DP ,,0");
                        this.machinePosition[2] += this.commandPosition[2];
                        break;
                    case 3:
                        response = g.command("DP ,,,0");
                        break;
                }

            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }
        } //定义当前位置为零点。。。。

        public void MoveAxisToZero(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        Move("X", 10, -1*this.X);
                        break;
                    case 1:
                        Move("Y", 10, -1 * this.Y);
                        break;
                    case 2:
                        Move("Z", 10, -1 * this.Z);
                        break;
                }

            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }
        } //定义当前位置为零点。。。。


        public void HomeXYZ()
        {
            try
            {
                g.command("SP 5000,5000,5000");
                g.command("CN ,-1;HMA;BGA");
                DMCWaitMotionComplete("A");
                g.command("CN ,1;HMBC;BGBC");
                DMCWaitMotionComplete("B");
                DMCWaitMotionComplete("C");
                g.command("DE 0,0,0,0,0,0");
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message; ;
            }
        } // 回机器零点

        public void ReturnToZero()
        {
            if (!this.isGalilConnected)
                return;
            try
            {
                g.command("PA 0,0,0;SP 5000,5000,5000;BG");
                DMCWaitMotionComplete("X"); DMCWaitMotionComplete("Y"); DMCWaitMotionComplete("Z");
                g.command("ST");
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }


        }

        public void Abort()
        {
            if (!this.isGalilConnected)
                return;
            try
            {
                g.command("AB");
                g.command("GR , , , , 0");
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }

        }

        public void Downto10mm()
        {
            Move("Z", 5, -45);
        }

        public void Downto100um()
        {

            Move("Z", 5 , -9.9);
        }

        public void MoveToMill(double XOffset, double YOffset)
        {
            if (!this.isGalilConnected)
                return;
            try
            {
                g.command("PR 0,0,2000;SP 0,0,5000;BG");
                DMCWaitMotionComplete("Z");
                g.command("ST;ST");
                g.command("PA 0,0;SP 5000,5000;BG");
                DMCWaitMotionComplete("X"); DMCWaitMotionComplete("Y"); DMCWaitMotionComplete("Z");
                g.command("ST;ST");
                int x = Convert.ToInt32(XOffset * XCountPmm);
                int y = Convert.ToInt32(YOffset * YCountPmm);
                g.command("PR " + x.ToString() + "," + y.ToString() + ",0"); g.command(";SP 5000,5000"); g.command(";BG");
                DMCWaitMotionComplete("X"); DMCWaitMotionComplete("Y");
                g.command("ST"); g.command("ST"); g.command("ST");
                g.command("PR 0,0,-2000;SP 0,0,5000;BG");
                DMCWaitMotionComplete("Z");
                g.command("ST;ST");
            }
            catch (Exception ex)
            {
                response = "错误: " + ex.Message;
            }
        }

        public void RunProgramByFile(string code)
        {
            if (!this.isGalilConnected)
                return;
            try
            {
                g.programDownload(code); //Download the contents of the textbox
                g.command("XQ");// 执行程序
                DelayMillSec(1000);
                DMCWaitMotionComplete("S");
                response = "程序结束";
            }
            catch (Exception ex)
            {
                response = " 错误：" + ex.Message;
            }
        }

        public void RunProgramByLine(string line)
        {
            if (!this.isGalilConnected)
                return;
            try
            {
                    if (line.StartsWith("REM"))
                    {
                        //g.command(line);
                    }
                    else if (line.StartsWith("VP"))
                    {
                        DMCWaitBufferReady();
                        g.command(line);
                    }
                    else if (line.StartsWith("CR"))
                    {
                        DMCWaitBufferReady();
                        g.command(line);
                    }
                    else if (line.StartsWith("LI"))
                    {
                        DMCWaitBufferReady();
                        g.command(line);
                    }
                    else if (line.Contains("AMS"))
                    {
                        DMCWaitMotionComplete("S");
                    }
                    else if (line.Contains("AMC"))
                    {
                        DMCWaitMotionComplete("C");
                    }
                    else if (line.StartsWith("EN"))
                    {
                        g.command("ST");
                    }
                    else
                    {
                        g.command(line);
                    }
                }
            catch (Exception ex)
            {
                response = "错误信息: " + ex.Message ;
            }
        } // 

        private bool DMCIsBufferFull()
        {
            if (g.commandValue("MG_LM") <= 5)
                return true;
            else
                return false;
        }// 检测缓冲区是否已满。

        private void DMCWaitBufferReady()
        {
            while (DMCIsBufferFull())
            {
                DelayMillSec(10);
            }
        } // 等待缓冲区空出来

        private bool DMCIsMoving(string Axis)
        {
            if (g.commandValue("MG _BG" + Axis) == 0)
                return false;
            else
                return true;
        }// 检测运动是否已停止, 参数为"A","B","S"单个字符.......

        private void DMCWaitMotionComplete(string Axis)
        {
            while (DMCIsMoving(Axis))
            {
                DelayMillSec(10);
            }
        } // 等待运动停止, 参数为"A","B","S"单个字符.......

    }
}
