using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using Kitware.VTK;
using Nuwa.xClass;

namespace Nuwa
{

    public partial class MainForm : CCSkinMain
    {
        private int MainMenuSelectedIndex;
        private xUnion union;
        public static GalilControl galil;
        private bool isProgramAbort = false;
        private bool isProgramRunning = false;
        private Path2DViewForm frmPath2DView;
        private ControlForm frmMainControl;
        private int TransformMode = 0;
        public int PathMode = 0;
        public static xModule SelectedModule = null;

        # region 属性

        public int NumberOfLayers
        {
            get { return xPath.SlcZList.Count; }
        }

        public int NumberOfObjects
        {
            get { return this.union.NumberOfObjects; }
        }

        #endregion
        public MainForm()
        {
            InitializeComponent();
            MainMenuSelectedIndex = -1;
            union = new xUnion();
            galil = new GalilControl();
            galil.TimeLabel = this.lblTime;
            union.RenderWindowControl = this.renderWindowControl1;
            frmPath2DView = new Path2DViewForm();
            frmMainControl = new ControlForm();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            tabControlSeconaryMenu.ItemSize = new Size(0, 1);
            tabControlSeconaryMenu.Height = 48;
            tabControlSeconaryMenu.Visible = true;
            MainMenuItemSelect(1, lableMainMenu1);
        }

        # region MainMenu

        public void MainMenuItemUnSelected()
        {
            for (int i = 0; i < 8; i++)
                this.panelMainMenu.Controls[i].BackColor = Color.DimGray;
        }

        public void SelectTabPage(int index)
        {
            foreach (TabPage tp in tabControlSeconaryMenu.TabPages)
            {
                if (tp.Text == index.ToString())
                    tabControlSeconaryMenu.SelectedTab = tp;
            }
        }

        public void MainMenuItemSelect(int index, object sender)
        {
            if (MainMenuSelectedIndex != index)
            {
                MainMenuSelectedIndex = index;
                MainMenuItemUnSelected();
                Label s = (Label)sender;
                s.BackColor = Color.SteelBlue;
                SelectTabPage(index);
                tabControlSeconaryMenu.Visible = true;
            }
            else
            {
                MainMenuSelectedIndex = -1;
                MainMenuItemUnSelected();
                //if (tabControlSeconaryMenu.Visible == false)
                tabControlSeconaryMenu.Visible = false;
            }
        }

        public void MainMenuItemHover(int index, object sender)
        {
            if (MainMenuSelectedIndex == index)
                return;
            Label s = (Label)sender;
            s.BackColor = Color.DarkGray;
        }

        public void MainMenuItemLeave(int index, object sender)
        {
            if (MainMenuSelectedIndex == index)
                return;
            Label s = (Label)sender;
            s.BackColor = Color.DimGray;
        }

        //-----------------------1-------------------------------
        private void lableMainMenu1_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(1, sender);
        }

        private void lableMainMenu1_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(1, sender);
        }

        private void lableMainMenu1_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(1, sender);
        }

        //-----------------------2-------------------------------
        private void lableMainMenu2_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(2, sender);
        }

        private void lableMainMenu2_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(2, sender);
        }

        private void lableMainMenu2_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(2, sender);
        }

        //-----------------------3-------------------------------
        private void lableMainMenu3_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(3, sender);
        }
        private void lableMainMenu03_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(3, sender);
        }

        private void lableMainMenu03_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(3, sender);
        }

        //-----------------------4-------------------------------
        private void lableMainMenu4_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(4, sender);
        }

        private void lableMainMenu4_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(4, sender);
        }

        private void lableMainMenu4_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(4, sender);
        }

        //--------------5---------------------------

        private void lableMainMenu5_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(5, sender);
        }

        private void lableMainMenu5_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(5, sender);
        }

        private void lableMainMenu5_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(5, sender);
        }

        //---------------6-------------------
        private void lableMainMenu6_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(6, sender);
        }


        private void lableMainMenu6_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(6, sender);
        }

        private void lableMainMenu6_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(6, sender);
        }

        private void lableMainMenu7_Click(object sender, EventArgs e)
        {
            MainMenuItemSelect(7, sender);
        }

        private void lableMainMenu7_MouseHover(object sender, EventArgs e)
        {
            MainMenuItemHover(7, sender);
        }

        private void lableMainMenu7_MouseLeave(object sender, EventArgs e)
        {
            MainMenuItemLeave(7, sender);
        }

        #endregion

        #region Object

        private void tsbuttonObjectImport_Click(object sender, EventArgs e)
        {
            ImportObject();
        }

        public void ImportObject()
        {
            // 设置OpenFileDialog
            StringBuilder Filter = new StringBuilder();
            Filter.Append("Stereo lithgraphy (*.stl)|*.stl|");
            Filter.Append("PolyY (*.ply)|*.ply|");
            Filter.Append("All files (*.*)|*.*|");
            Filter.Append("|");
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = Filter.ToString();
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                // 添加工作module
                string fileName = fileDlg.FileName;
                xModule object_ = null;
                xModule2 slice = new xModule2();
                string FileType = Path.GetExtension(fileName.ToLower());
                switch (FileType)
                {
                    case ".stl":
                        object_ = this.union.GetObjectFromSTL(fileName);
                        this.union.AddObject(object_);
                        break;
                    case ".ply":
                        object_ = this.union.GetObjectFromPLY(fileName);
                        this.union.AddObject(object_);
                        break;
                }
                this.union.AddSlice(slice);
                // 将物体参数传递给xPath
                xPath.NumberOfObjects = this.union.NumberOfObjects;
                this.union.Render();
            }

        }


        private void tsButtonObjectRemove_Click(object sender, EventArgs e)
        {
            this.union.RemoveObject(this.union.SelectedModuleIndex);
            this.union.Render();
        }

        # endregion Object

        # region Path

        private void tsButtonPathSimulate_Click(object sender, EventArgs e)
        {


        }

        private void tsButtonPathImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "GCODE文件|*.GCODE|TAP文件|*.TAP|DMC文件|*.DMC|所有文件|*.*";
            ofd.RestoreDirectory = true;
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenGCode(ofd.FileName);
            }
        }  // Paths窗口，传递Layer信息

        private void tsButtonCreatePath_Click(object sender, EventArgs e)
        {
            CreatePaths();
        }

        public void CreatePaths()
        {
            xPath.GenerateSlcZList();
            if (NumberOfLayers == 0)
                return;
            // 得到带Boundry的Layer'
            for (int i = 0; i < NumberOfLayers; i++)
            {
                xLayer layer = new xLayer();
                for (int j = 0; j < NumberOfObjects; j++)
                {
                    layer.AddBoundry(xPath.GetBoundary(GetSliceAt(i, j)));
                }
                xPath.AddLayer(layer);
            }

            for (int i = 0; i < NumberOfLayers; i++)
            {
                xLayer layer = xPath.Layers[i];
                layer.Z = xPath.SlcZList[i];
                layer.ID = i;
                if (i == 0)
                    layer.LayerThickness = layer.Z;
                else
                    layer.LayerThickness = layer.Z - xPath.Layers[i - 1].Z;
                layer.CreateToolPathsWithBordersOnly();
                layer.FillToolPathsFromBorders(Config.Offset, Config.FillPattern);
            }

            // 2D窗口更新
            frmPath2DView.PaintLayer();
            // 生成Toolpath
            //this.union.CreatePointsFromToolPath();
            //this.union.CreateToolPathActorFromPoints();
            //this.union.CreateToolPathActor();
            for (int i = 0; i < NumberOfObjects;i++ )
             {
                this.union.CreateToolPathActor();
            }
            CreateProgramToRichTextBox_LI();
            this.union.Render();
        }

        public void CreateProgramToRichTextBox_LI()
        {
            if (this.NumberOfLayers == 0)
                return;
            // 将前面的点清除
            xGcLine line = new xGcLine();
            Ini.FeedFlow = 10;
            Ini.PrintSpeed = 10;
            Ini.PullDistance = 1; //3mm
            Ini.JumpSpeed = 10; // 8mm/s
            Ini.SuckAmount = 1; // 1ul
            StringBuilder sb = new StringBuilder();
            sb.Append((Code.StartProgram));
            for (int i = 0; i < this.NumberOfLayers; i++)
            {
                xLayer Layer = xPath.Layers[i];
                double z = Layer.Z;
                line.p1.z = (int)(z*1000);
                if (i != 0)
                {
                    sb.Append(Ini.StartPolyLine);
                    sb.AppendLine(line.LiString_XYZ);
                    sb.Append(Ini.EndPolyLine);
                }

                line.SetP2_Z();
                for (int j = 0; j < Layer.NumberOfToolPaths; j++)
                {
                    xToolPath toolPath = Layer.GetToolPathAt(j);
                    for (int k = 0; k < toolPath.Paths.Count; k++)
                    {
                        xLoop polyline = toolPath.Paths[k];
                        //for (int k = 0; k < toolPath.Borders.Count; k++)
                        //{
                        //    xLoop polyline = toolPath.Borders[k];
                        bool flag1 = true;
                        for (int m = 0; m < polyline.NumberOfPoints; m++)
                        {
                            xPoint2 pt = polyline.GetPointAt(m);
                            line.p1.x =pt.XL;
                            line.p1.y = pt.YL;
                            if (!line.IsLineZero())
                            {
                                if (m == 0) //先抬笔,快速走到第一个点
                                {
                                    if (line.Distance() > (int)(Config.LineSpacing * 2000))
                                    {
                                        sb.Append(Ini.PullPen);
                                        sb.Append(Ini.StartJump);
                                        sb.AppendLine(line.LiString_XYZ);
                                        sb.Append(Ini.EndJump);
                                        sb.Append(Ini.DropPen);
                                        sb.Append(Ini.StartPolyLine);
                                    }
                                    else
                                    {
                                        sb.Append(Ini.StartJump);
                                        sb.Append(Ini.EndJump);
                                        sb.Append(Ini.StartPolyLine);
                                        sb.AppendLine(line.LiString_XYZ);
                                    }

                                    line.SetP2_XY();
                                }
                                else
                                {
                                    sb.AppendLine(line.LiString_XYZ);
                                    if (flag1)
                                    {
                                        sb.AppendLine("BGS");
                                        flag1 = false;
                                    }
                                    line.SetP2_XY();
                                }
                            } // end if 
                            else
                                sb.AppendLine("LI 0,1,0");

                        } // end m
                        sb.Append((Ini.EndPolyLine));
                    } // end k
                } // end j
            } // end i
            sb.Append((Ini.EndProgram));
            this.frmMainControl.scRichTextBoxGCode.Text = sb.ToString();
        }  // 将Construt转换成DMC程序，填入listbox



        public vtkPolyData GetSliceAt(int SliceIndex, int ModuleIndex)
        {
            // 在位置列表的某点产生切片
            vtkPlane slcPlane = vtkPlane.New();
            slcPlane.SetOrigin(0, 0, xPath.SlcZList[SliceIndex]);
            slcPlane.SetNormal(0.0, 0.0, 1.0);
            vtkCutter slicer = vtkCutter.New();
            slicer.SetInput(this.union.GetObjectAt(ModuleIndex).PolyData);
            slicer.SetCutFunction(slcPlane);
            slicer.Update();
            return slicer.GetOutput();
            //slicer.Dispose();
            //slcPlane.Dispose();
        }




        private void tsButtonPath2DView_Click(object sender, EventArgs e)
        {
            frmPath2DView.Visible = true;
            frmPath2DView.Location = new Point(this.Location.X+20, this.Location.Y+150);
        }



        private void PaintLayer()
        {
            if (this.NumberOfLayers == 0)
                return;
            frmPath2DView.CalcSizeOffset();
            frmPath2DView.ScrollMax = this.NumberOfLayers - 1;
            frmPath2DView.SetLayer(xPath.Layers[frmPath2DView.ScrollNumber]);
            //frmPath2DView.picBoxPath.Focus();
        }
        # endregion Path

        # region view


        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            this.union.RenderWindowControl = this.renderWindowControl1;
            this.union.SetUpScene();

        }
        private void tsButtonViewColorRed_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            this.union.LastPickedProperty.SetColor(Color.Red.R / 100.0, Color.Red.G / 100.0, Color.Red.B / 100.0);
            SelectedModule.SetColor(Color.Red);
            this.union.Render();
        }

        private void tsButtonViewColorBlue_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            this.union.LastPickedProperty.SetColor(Color.Blue.R / 100.0, Color.Blue.G / 100.0, Color.Blue.B / 100.0);
            SelectedModule.SetColor(Color.Blue);
            this.union.Render();
        }

        private void tsButtonViewColorYellow_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            this.union.LastPickedProperty.SetColor(Color.Yellow.R / 100.0, Color.Yellow.G / 100.0, Color.Yellow.B / 100.0);
            SelectedModule.SetColor(Color.Yellow);
            this.union.Render();
        }

        private void tsButtonViewColorGreen_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            this.union.LastPickedProperty.SetColor(Color.Green.R / 100.0, Color.Green.G / 100.0, Color.Green.B / 100.0);
            SelectedModule.SetColor(Color.Green);
            this.union.Render();
        }


        private void tsMenuItemViewUp_Click(object sender, EventArgs e)
        {
            this.union.SetCamera(0, 0, 1, 0, 1, 0); //z+
        }

        private void tsMenuItemViewLeft_Click(object sender, EventArgs e)
        {
            this.union.SetCamera(0, -1, 0, 1, 0, 0); //y-
        }

        private void tsMenuItemViewFront_Click(object sender, EventArgs e)
        {
            this.union.SetCamera(1, 0, 0, 0, 0, 1); //x+
        }

        private void tsButtonViewFit_Click(object sender, EventArgs e)
        {
            this.union.Ren.ResetCamera();
            this.union.Render();
        }
        private void tsMenuItemViewObjectShow_Click(object sender, EventArgs e)
        {
            tsMenuItemViewObjectShow.Checked = !tsMenuItemViewObjectShow.Checked;
            int visibility = 0; if (tsMenuItemViewObjectShow.Checked) visibility = 1; else visibility = 0;
            this.union.GetObjectAt(0).SetVisibility(visibility); ;
            this.union.Render();
        }
        private void tsMenuItemViewPathShow_Click(object sender, EventArgs e)
        {
            tsMenuItemViewPathShow.Checked = !tsMenuItemViewPathShow.Checked;
            int visibility = 0; if (tsMenuItemViewPathShow.Checked) visibility = 1; else visibility = 0;
            this.union.GetSliceAt(0).SetVisibility(visibility);
            this.union.Render();
        }
        # endregion view

        # region Edit


        private void tsButtonObjectMove_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            ObjectTrasnformForm frm = new ObjectTrasnformForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            frm.TransformMode =1;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                TransformObject(1, frm.X, frm.Y, frm.Z);
            }
        }

        private void tsButtonObjectRotate_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            ObjectTrasnformForm frm = new ObjectTrasnformForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            frm.TransformMode = 2;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                TransformObject(2, frm.X, frm.Y, frm.Z);
            }
        }

        private void tsButtonObjectScale_Click(object sender, EventArgs e)
        {
            if (SelectedModule == null)
                return;
            ObjectTrasnformForm frm = new ObjectTrasnformForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            frm.TransformMode = 0;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                TransformObject(0, frm.X, frm.Y, frm.Z);
            }
        }

        public void TransformObject(int axis, double x, double y, double z)
        {
            if (NumberOfObjects == 0)
                return;
            switch (this.TransformMode)
            {
                case 0: // scale mode
                    double s = 1;
                    switch (axis)
                    {
                        case 0:  // x axis
                            s = x / (SelectedModule.Bounds[1] - SelectedModule.Bounds[0]);
                            SelectedModule.Transform(s, s, s, 2);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;

                        case 1:  // y axis
                            s = y / (SelectedModule.Bounds[3] - SelectedModule.Bounds[2]);
                            SelectedModule.Transform(s, s, s, 2);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;

                        case 2:  // zaxis
                            s = z / (SelectedModule.Bounds[5] - SelectedModule.Bounds[4]);
                            SelectedModule.Transform(s, s, s, 2);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;
                    }
                    break;
                case 1: // move mode
                    double m = 1;
                    switch (axis)
                    {
                        case 0:  // x axis
                            m = x - SelectedModule.Center[0];
                            SelectedModule.Transform(m, 0, 0, 0);
                            this.union.Render();
                            this.union.UpdateBounds();
                            break;

                        case 1:  // y axis
                            m = y - SelectedModule.Center[1];
                            SelectedModule.Transform(0, m, 0, 0);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;

                        case 2:  // zaxis
                            m = z - SelectedModule.Center[2];
                            SelectedModule.Transform(0, 0, m, 0);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;
                    }
                    break;
                case 2: //rotate mode
                    double a = 0;
                    switch (axis)
                    {
                        case 0:  // x axis
                            a = x - SelectedModule.Angle[0];
                            SelectedModule.Angle[0] = a;
                            SelectedModule.Transform(a, 0, 0, 1);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;

                        case 1:  // y axis
                            a = y - SelectedModule.Angle[1];
                            SelectedModule.Angle[1] = a;
                            SelectedModule.Transform(0, a, 0, 1);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;

                        case 2:  // zaxis
                            a = z - SelectedModule.Angle[2];
                            SelectedModule.Angle[2] = a;
                            SelectedModule.Transform(0, 0, a, 1);
                            this.union.Render(); ;
                            this.union.UpdateBounds();
                            break;
                    }








                    break;
            }
        }

        #endregion Edit

        #region Run




        private void tsButtonRunAbort_Click(object sender, EventArgs e)
        {
            this.isProgramAbort = true;
            galil.Abort();
        }
        private void tsButtonRunConnectMachine_Click(object sender, EventArgs e)
        {
            if (galil.IsGalilConnected)
                return;
            galil.StartConnectGalil();
            galil.StartRecord();
            ShowConnectStatus(galil.IsGalilConnected);
        }

        private void ShowConnectStatus(bool status)
        {
            if (status)
            {
                tsButtonRunConnectMachine.Image = global::Nuwa.Properties.Resources.Connected;
            }
            else
            {
                tsButtonRunConnectMachine.Image = global::Nuwa.Properties.Resources.NotConnected;
            }

        }

        public void ShowMainControlForm(bool show)
        {
            int fx = this.Location.X + this.Width - frmMainControl.Width;
            int fy = this.Location.Y + this.Height - frmMainControl.Height;
            frmMainControl.Location = new Point(fx, fy);
            if (show)
                frmMainControl.Visible = true;
            else
                frmMainControl.Visible = false;
        }


        public void OpenGCode(string filename)
        {
            frmMainControl.scRichTextBoxGCode.Clear();
            ReadFileToRichTextBox(frmMainControl.scRichTextBoxGCode, filename);
            ParseGCodeinRichTextBox();
            this.union.CreateToolPathActor_GCode();
        }

        public void ParseGCodeinRichTextBox()
        {
            foreach (string strline in frmMainControl.scRichTextBoxGCode.Lines)
            {
                if (strline != "")
                    xGCode.Parse(strline);
            }
        }

        private void ReadFileToRichTextBox(RichTextBox rtb, string spath)
        {
            try
            {
                FileStream fs = new FileStream(spath, FileMode.Open, FileAccess.Read);
                if (fs.CanRead)
                {
                    StreamReader sr = new StreamReader(fs, Encoding.Default);
                    string strline = sr.ReadLine();
                    StringBuilder sb = new StringBuilder();
                    while (strline != null)
                    {
                        strline = sr.ReadLine();
                        sb.Append(strline + "\n");
                        Application.DoEvents();
                    }
                    rtb.Text = sb.ToString();
                    sr.Close();
                }
                //fs.Flush();
                //fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //RichTextBox 读取txt文件通用函数

        private void tsButtonRunGCode_Click(object sender, EventArgs e)
        {
            string[] lines = frmMainControl.scRichTextBoxGCode.Lines;
            foreach (string line in lines)
            {
                xGCode.Parse(line);
                Application.DoEvents();
            }
        }


        private void tsButtonRunStart_Click(object sender, EventArgs e)
        {
            this.isProgramAbort = false;
            string[] lines = frmMainControl.scRichTextBoxGCode.Lines;   //bool flag = false; 
            //UpdateProgressBar(lines.Length);
           // this.isProgramRunning = true; ;
            foreach (string line in lines)
            {
                if (this.isProgramAbort)
                    break;
                galil.RunProgramByLine(line);
                //galil.RunProgramByFile(line);
                //progressBar1.Value++;
            }
           // this.isProgramRunning = false;
            //lblLineNumberDone.Text = (Ini.SendLineNumber + 4).ToString();
            MessageBox.Show("程序已完成");
        }

        #endregion Run

        #region Sample


        private void tsButtonSquareCrucible_Click(object sender, EventArgs e)
        {
            SampleSquareCrucibleForm frm = new SampleSquareCrucibleForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frmMainControl.scRichTextBoxGCode.Text = "";
                frmMainControl.scRichTextBoxGCode.Text += "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,,10000\r\nDP0,0\r\nLMABC\r\n"; ;
                frmMainControl.scRichTextBoxGCode.Text += "VS 10000\r\nGA,,,S\r\nGR,,," + Config.GearRatio.ToString() + "\r\n";
                frmMainControl.scRichTextBoxGCode.Text += xSample.LiString;
                frmMainControl.scRichTextBoxGCode.Text += "LE\r\nAMS\r\n";
                frmMainControl.scRichTextBoxGCode.Text += "ST\r\nGA,,,,T\r\nGR,,,,0\r\nEN";
            }
            this.union.CreateSamplePathActor();
        }

        private void tsButtonRoundCrucible_Click(object sender, EventArgs e)
        {
            SampleRoundCrucibleForm frm = new SampleRoundCrucibleForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frmMainControl.scRichTextBoxGCode.Text = "";
                frmMainControl.scRichTextBoxGCode.Text += "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,,10000\r\nDP0,0\r\nVMAB\r\n"; ;
                frmMainControl.scRichTextBoxGCode.Text += "VS 10000\r\nGA,,,S\r\nGR,,," + Config.GearRatio.ToString() + "\r\n";
                frmMainControl.scRichTextBoxGCode.Text += xSample.LiString;
                frmMainControl.scRichTextBoxGCode.Text += "VE\r\nAMS\r\n";
                frmMainControl.scRichTextBoxGCode.Text += "ST\r\nGA,,,,T\r\nGR,,,,0\r\nEN";
            }
            this.union.CreateSamplePathActor();
        }

        private void tsButtonSampleCylinder_Click(object sender, EventArgs e)
        {
            SampleCylinderForm frm = new SampleCylinderForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frmMainControl.scRichTextBoxGCode.Text = "";
                frmMainControl.scRichTextBoxGCode.Text += "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,,10000\r\nDP0,0\r\nVMAB\r\n"; ;
                frmMainControl.scRichTextBoxGCode.Text += "VS 10000\r\nGA,,,S\r\nGR,,," + Config.GearRatio.ToString() + "\r\n";
                frmMainControl.scRichTextBoxGCode.Text += xSample.LiString;
                //frmMainControl.scRichTextBoxGCode.Text += "VE\r\nAMS\r\n";
                frmMainControl.scRichTextBoxGCode.Text += "ST\r\nGA,,,,T\r\nGR,,,,0\r\nEN";
            }
            this.union.CreateSamplePathActor();
        }

        private void tsButtonSampleCube_Click(object sender, EventArgs e)
        {
            SampleCubeForm frm = new SampleCubeForm();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frmMainControl.scRichTextBoxGCode.Text = "";
                frmMainControl.scRichTextBoxGCode.Text += "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,,10000\r\nDP0,0\r\nLMABC\r\n"; ;
                frmMainControl.scRichTextBoxGCode.Text += "VS 10000\r\nGA,,,S\r\nGR,,," + Config.GearRatio.ToString() + "\r\n";
                frmMainControl.scRichTextBoxGCode.Text += xSample.LiString;
                frmMainControl.scRichTextBoxGCode.Text += "LE\r\nAMS\r\n";
                frmMainControl.scRichTextBoxGCode.Text += "ST\r\nGA,,,,T\r\nGR,,,,0\r\nEN";
            }
            this.union.CreateSamplePathActor();
        }


# endregion Sample

        # region Setting

        private void tsButtonSettingBasic_Click(object sender, EventArgs e)
        {
            SettingBasicFormcs frm = new SettingBasicFormcs();
            frm.Location = new Point(this.Location.X + 20, this.Location.Y + 150);
             if (frm.ShowDialog() == DialogResult.OK)
              {
                 
              }
        }

        #endregion Setting

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ConfigForm frm = new ConfigForm();
            frm.Show();
            frm.loadmessage();
        }

        private void tsButtonRunShowControlPanel_Click(object sender, EventArgs e)
        {
            if (frmMainControl.Visible)
                ShowMainControlForm(false);
            else
                ShowMainControlForm(true);
        }

        private void lblTime_TextChanged(object sender, EventArgs e)
        {
            frmMainControl.lblXPosRP.Text = MainForm.galil.X.ToString("f3"); //mm
            frmMainControl.lblYPosRP.Text = MainForm.galil.Y.ToString("f3"); //mm
            frmMainControl.lblZPosRP.Text = MainForm.galil.Z.ToString("f3"); //mm
            frmMainControl.lblDPosRP.Text = MainForm.galil.D.ToString("f1"); //uL
            //lblEPosRP.Text = (galil.E / 1000).ToString("f1"); //uL

            //lblXPosTP.Text = MainForm.galil.Xe.ToString("f3"); //mm
            //lblYPosTP.Text = MainForm.galil.Ye.ToString("f3"); //mm
            //lblZPosTP.Text = galil.Ze.ToString("f3"); //mm
            //lblESpeedRP.Text = (galil.Es / 1000).ToString("f3"); //mm
            //lblDSpeedRP.Text = MainForm.galil.Ds.ToString("f3"); //mm
            //if (MainForm.isProgramRunning)
            //{
            //    MainForm.union.SetPenPosition(galil.X, galil.Y, galil.Z);
            //    lblSDistance.Text = galil.S.ToString("f3");
            //    lblSSpeedTP.Text = Ini.ExecutedSSpend.ToString("f1");
            //    lblLineNumberDone.Text = Ini.SendLineNumber.ToString();
            //}
        }



        //private void tsTextBoxTranformY_DoubleClick(object sender, EventArgs e)
        //{
        //    TransformObject(1);
        //    switch (this.TransformMode)
        //    {
        //        case 0:
        //            UpdateDimesion();
        //            break;
        //        case 1:
        //            UpdatePosition();
        //            break;
        //        case 2:
        //            UpdateAngle();
        //            break;
        //    }

        //}

        //private void tsTextBoxTranformZ_DoubleClick(object sender, EventArgs e)
        //{
        //    TransformObject(2);
        //    switch (this.TransformMode)
        //    {
        //        case 0:
        //            UpdateDimesion();
        //            break;
        //        case 1:
        //            UpdatePosition();
        //            break;
        //        case 2:
        //            UpdateAngle();
        //            break;
        //    }
        //}

        //private void tsTextBoxTranformX_DoubleClick(object sender, EventArgs e)
        //{
        //    TransformObject(0);
        //    switch (this.TransformMode)
        //    {
        //        case 0:
        //            UpdateDimesion();
        //            break;
        //        case 1:
        //            UpdatePosition();
        //            break;
        //        case 2:
        //            UpdateAngle();
        //            break;
        //    }
        //}




    }
}
