using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using Nuwa.xClass;

namespace Nuwa
{
    partial class Path2DViewForm : Form
    {

        // Drawing Parameter
        private float offsetX = 0;
        private float offsetY = 0;
        private float scale = 1f;
        bool rightMouseButtonDown = false;
        PointF mouseDownLocation = new PointF(0, 0);
        private Matrix matrix = null;
        private MainForm main;
        private xLayer CurrentLayer;
        private Property property = new Property();

        public Path2DViewForm()
        {
            InitializeComponent();
            main = (MainForm)this.Parent;
        }

        public int ScrollMax
        {
            get { return this.trackBarLayer.Maximum; }
            set { this.trackBarLayer.Maximum = value; }
        }

        public int NumberOfLayers
        {
            get { return Path.Layers.Count; }
        }

        public int ScrollNumber
        {
            get { return trackBarLayer.Value; }
        }


        # region TabPage1 -  TabPage 12 -  2D绘画实现

        private void statusBarPath_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
        {
            Rectangle rc = new Rectangle(sbdevent.Bounds.X, sbdevent.Bounds.Y, sbdevent.Bounds.Width, sbdevent.Bounds.Height);

            StatusBar statusbarpanels = (StatusBar)sender;

            Font aFont = new Font("Microsoft Sans Serif ", (float)8.25,
            FontStyle.Regular, GraphicsUnit.Point);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;

            sbdevent.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(199, 216, 236)), rc);
            rc.Y += 3;
            sbdevent.Graphics.DrawString(sbdevent.Panel.Text, aFont,
            new SolidBrush(Color.Black), rc, sf);
            rc.Y -= 3;
            rc.Height--;
            rc.Width--;
            sbdevent.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(10, 47, 115))), rc);
            rc.Height++;
            rc.Width++;
            sbdevent.DrawFocusRectangle();
        } // Paths状态栏界面美化

        private void picBoxPath_MouseDown(object sender, MouseEventArgs e)
        {
            picBoxPath.Focus();
            if (e.Button == MouseButtons.Right)
            {
                mouseDownLocation = new PointF((float)e.X, (float)e.Y);
                rightMouseButtonDown = true;
            }
        } // Paths窗口，获得鼠标按下位置

        private void picBoxPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (rightMouseButtonDown)
            {
                //calculate dx
                float dx = (float)e.X - mouseDownLocation.X;
                float dy = (float)e.Y - mouseDownLocation.Y;
                offsetX += dx / scale;
                offsetY -= dy / scale;
                mouseDownLocation = new PointF((float)e.X, (float)e.Y);
                picBoxPath.Refresh();
            }
            else
            {
                PointF[] pf = new PointF[1];
                pf[0] = new PointF((float)(e.X), (float)(e.Y));
                if (matrix != null)
                {
                    Matrix m = matrix.Clone();
                    m.Invert();
                    m.TransformPoints(pf);
                }
                labelX.Text = "X=" + pf[0].X.ToString("f3");
                labelY.Text = "Y=" + pf[0].Y.ToString("f3");
            }
        } //Paths窗口，如果右键按下，移动图像。未按下，标记XY位置

        private void picBoxPath_MouseUp(object sender, MouseEventArgs e)
        {
            rightMouseButtonDown = false;
        } // Paths窗口，表明右键未按下

        private void picBoxPath_MouseWheel(object sender, MouseEventArgs e)
        {
            //float oldScale = scale;
            scale *= e.Delta > 0 ? 1.41f : 0.72f;
            if (scale < 0.01f)
                scale = 0.01f;
            if (scale > 10000)
                scale = 10000;
            labelS.Text = " Scale: " + scale.ToString("f3");
            picBoxPath.Refresh();
        } // Paths窗口，图像缩放

        private void picBoxPath_SizeChanged(object sender, EventArgs e)
        {
            CalcSizeOffset();
        } // Paths窗口，如果窗口改变则计算缩放和重点

        public void CalcSizeOffset()
        {
            double Width = Path.Bounds[1] - Path.Bounds[0];
            double Height = Path.Bounds[3] - Path.Bounds[2];
            double CenterX = Path.Bounds[0] + Width / 2;
             double CenterY = Path.Bounds[2] + Height / 2;
            if (Path.Layers == null || Path.Layers.Count == 0)
                return;
            int PicBoxMinDimension = picBoxPath.Width > picBoxPath.Height ? picBoxPath.Height : picBoxPath.Width;
            double AssemblyMaxDimension = Width > Height ? Width : Height;
            scale = (float)PicBoxMinDimension / (float)AssemblyMaxDimension;
            offsetX = (float)(picBoxPath.Width / 2) - (float)CenterX;
            offsetY = (float)(picBoxPath.Height / 2) - (float)CenterY;
        } // Paths窗口，计算缩放和中点实际算法

        private void trackBarLayer_Scroll(object sender, EventArgs e)
        {
            if (NumberOfLayers == 0)
                return;
            CurrentLayer = Path.Layers[(trackBarLayer.Value)];
            labelZ.Text = "Z= " + CurrentLayer.Z.ToString("f3");
            picBoxPath.Refresh();
            labelLayerNumber.Text= (trackBarLayer.Value + 1).ToString() + "/" + (trackBarLayer.Maximum + 1).ToString();
        }// Paths窗口，移动滑块，浏览不同层

        public void PaintLayer()
        {
            if (this.NumberOfLayers == 0)
                return;
            CalcSizeOffset();
            trackBarLayer.Maximum = this.NumberOfLayers - 1;
            CurrentLayer = Path.Layers[trackBarLayer.Value];
            picBoxPath.Focus();
        }  // Paths窗口，传递Layer信息

        private Matrix MatrixCenter()
        {
            Matrix m = new Matrix();
            m.Translate((float)picBoxPath.Width / 2F, (float)picBoxPath.Height / 2F);
            m.Scale(scale, -1 * scale);
            m.Translate(offsetX, offsetY);
            m.Translate(-(float)picBoxPath.Width / 2F, -(float)picBoxPath.Height / 2F);
            return m;
        }

        private void picBoxPath_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //图像置中，保存matrix
            Graphics g = e.Graphics;
            g.Transform = MatrixCenter();
            matrix = g.Transform;
            // 实线路径
            Pen pen = new Pen(Color.Blue, 0);
            //虚线跳跃
            Pen pen2 = new Pen(Color.Gray, 0);
            pen2.DashStyle = DashStyle.DashDot;
            pen2.DashPattern = new float[] { 0.05F, 0.05F };
            // 中心线
            g.DrawLine(pen2, -1000, 0, 1000, 0);
            g.DrawLine(pen2, 0, -1000, 0, 1000);
            if (CurrentLayer != null)
            {
                for (int i = 0; i < CurrentLayer.NumberOfToolPaths; i++)
                {
                    xToolPath polygon = CurrentLayer.GetToolPathAt(i);
                    //根据材料类型选择笔颜色
                    pen.Color = getColor(polygon.Material);
                    ObjSettings ps = this.property.GetObjSetAt(polygon.Material);
                    // 显示外周
                    //if (ps.PathShowPattern == 0 || ps.PathShowPattern == 2)
                    if(true)
                    {
                        for (int j = 0; j < polygon.Borders.Count; j++)
                        {
                            xLoop list = polygon.Borders[j];
                            if (j < polygon.Borders.Count - 1)
                            {
                                xLoop list1 = polygon.Borders[j + 1]; //后面一条线
                                xPoint2 pt00 = list.GetPointAt(list.NumberOfPoints - 1);
                                xPoint2 pt01 = list1.GetPointAt(0);
                                g.DrawLine(pen2, (float)pt00.X, (float)pt00.Y, (float)pt01.X, (float)pt01.Y);
                                //g.DrawString(j.ToString(), new Font("微软雅黑", 2, FontStyle.Bold), new SolidBrush(Color.Red), new PointF((float)pt00.X, (float)pt00.Y));
                            }
                            for (int k = 0; k < list.NumberOfPoints; k++)
                            {
                                xPoint2 pt0 = list.GetPointAt(k);
                                xPoint2 pt1 = list.GetPointAt((k + 1) % list.NumberOfPoints);
                                pen.Width = 1F / scale;
                                g.DrawLine(pen, (float)pt0.X, (float)pt0.Y, (float)pt1.X, (float)pt1.Y);
                            }
                        }
                    }
                    //显示路径
                    //if (ps.PathShowPattern == 1 || ps.PathShowPattern == 2)
                    if (true)
                    {
                        for (int j = 0; j < polygon.Paths.Count; j++)
                        {
                            xLoop list = polygon.Paths[j];
                            xPoint2 pt00 = list.GetPointAt(list.NumberOfPoints - 1);
                            e.Graphics.TranslateTransform((float)pt00.X, (float)pt00.Y);
                            g.RotateTransform(180);
                            g.ScaleTransform(-1, 1);//关于x轴镜像  ;
                            g.DrawString(j.ToString(), new Font("@微软雅黑", 20F / scale, FontStyle.Regular), new SolidBrush(Color.Gray), new PointF(0, 0));
                            g.ScaleTransform(-1, 1);//关于x轴镜像  ;
                            g.RotateTransform(-180);
                            e.Graphics.TranslateTransform(-(float)pt00.X, -(float)pt00.Y);
                            if (j < polygon.Paths.Count - 1)
                            {
                                xLoop list1 = polygon.Paths[j + 1]; //后面一条线
                                xPoint2 pt01 = list1.GetPointAt(0);
                                g.DrawLine(pen2, (float)pt00.X, (float)pt00.Y, (float)pt01.X, (float)pt01.Y);

                            }
                            for (int k = 0; k < list.NumberOfPoints - 1; k++)
                            {
                                xPoint2 pt0 = list.GetPointAt(k);
                                xPoint2 pt1 = list.GetPointAt((k + 1) % list.NumberOfPoints);
                                pen.Width = 3F / scale; ;
                                g.DrawLine(pen, (float)pt0.X, (float)pt0.Y, (float)pt1.X, (float)pt1.Y);
                            }
                        }
                    }

                }
            }
        } // Paths窗口，重画窗口

        Color getColor(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.Green;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.FromArgb(255, 0, 255);
                default:
                    return Color.Black;
            }
        }// 不同的材料，不同的颜色

        # endregion

        public void SetLayer(xLayer layer)
        {
            this.CurrentLayer = layer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }







    }
}
