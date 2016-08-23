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
using Kitware.VTK;
using Nuwa.xClass;

namespace Nuwa
{

    public partial class MainForm : CCSkinMain
    {
        private int MainMenuSelectedIndex;
        private xUnion union;
        private Path2DViewForm frmPath2DView;

        # region 属性

        public int NumberOfLayers
        {
            get { return Path.SlcZList.Count; }
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
            union.RenderWindowControl = this.renderWindowControl1;
            frmPath2DView = new Path2DViewForm();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabControlSeconaryMenu.ItemSize = new Size(0, 1);
            tabControlSeconaryMenu.Height = 48;
            skinSplitContainerMain.Panel2Collapsed = true;
        }

        # region MainMenu

        public void MainMenuItemUnSelected()
        {
                for (int i = 0; i < 8; i++)
                    this.panelMainMenu.Controls[i].BackColor = Color.DimGray;
        }

        public void MainMenuItemSelect(int index,object sender)
        {
            if (MainMenuSelectedIndex != index)
            {
                MainMenuSelectedIndex = index;
                MainMenuItemUnSelected();
                Label s = (Label)sender;
                s.BackColor = Color.SteelBlue;
                tabControlSeconaryMenu.SelectedIndex = index - 1;
                if (tabControlSeconaryMenu.Visible == false)
                    this.splitContainerRight.Height = this.splitContainerRight.Height - 48;
                tabControlSeconaryMenu.Visible = true;

            }
            else
            {
                MainMenuSelectedIndex = -1;
                MainMenuItemUnSelected();
                if (tabControlSeconaryMenu.Visible == true)
                    this.splitContainerRight.Height = this.splitContainerRight.Height +48;
                tabControlSeconaryMenu.Visible = false;

            }
        }

        public void MainMenuItemHover(int index,object sender)
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
            MainMenuItemSelect(1,sender);
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

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            this.union.RenderWindowControl = this.renderWindowControl1;
            this.union.SetUpScene();
        }

        private void tsbuttonImportObject_Click(object sender, EventArgs e)
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
                string FileType = System.IO.Path.GetExtension(fileName.ToLower());
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
                // 将物体参数传递给Path
                Path.NumberOfObjects = this.union.NumberOfObjects;
                this.union.Render();
            }

        }

        private void tsButtonCreatePath_Click(object sender, EventArgs e)
        {
            CreatePaths();
        }

        public void CreatePaths()
        {
            Path.GenerateSlcZList();
            if (NumberOfLayers == 0)
                return;
            for (int i = 0; i < NumberOfLayers; i++)
            {
                xLayer layer = new xLayer();
                for (int j = 0; j < NumberOfObjects; j++)
                {
                    layer.AddBoundry(Path.GetBoundary(GetSliceAt(i,j)));
                }
                Path.AddLayer(layer);
            }
            for (int i = 0; i < NumberOfLayers; i++)
            {
                xLayer layer =Path.Layers[i];
                layer.Z = Path.SlcZList[i];
                layer.ID = 1;
                if (i == 0)
                    layer.LayerThickness = layer.Z;
                else
                    layer.LayerThickness = layer.Z - Path.Layers[i - 1].Z;
                layer.GetToolPathsWithBorders();
                layer.FillToolPathsFromBorders(Config.FristOffset,Config.Offset,Config.FillPattern);
             }

            // 2D窗口更新
            frmPath2DView.PaintLayer();
            // 生成Toolpath
            //this.union.CreatePointsFromToolPath();
            //this.union.CreateToolPathActorFromPoints();
            //this.union.CreateToolPathActor();
            //for (int i = 0; i < NumberOfObjects;i++ )
           // {
                this.union.CreateToolPathActor();
            //}

                this.union.Render();
        }

        public vtkPolyData GetSliceAt(int SliceIndex, int ModuleIndex)
        {
            // 在位置列表的某点产生切片
            vtkPlane slcPlane = vtkPlane.New();
            slcPlane.SetOrigin(0, 0, Path.SlcZList[SliceIndex]);
            slcPlane.SetNormal(0.0, 0.0, 1.0);
            vtkCutter slicer = vtkCutter.New();
            slicer.SetInput(this.union.GetObjectAt(ModuleIndex).PolyData);
            slicer.SetCutFunction(slcPlane);
            slicer.Update();
            return slicer.GetOutput();
            //slicer.Dispose();
            //slcPlane.Dispose();
        }
        private void tsButtonInitView_Click(object sender, EventArgs e)
        {
            this.union.Ren.ResetCamera(); 
            this.union.Render();
        }

        private void tsMenuItemObjectShow_Click(object sender, EventArgs e)
        {
            tsMenuItemObjectShow.Checked = !tsMenuItemObjectShow.Checked;
            int visibility = 0; if (tsMenuItemObjectShow.Checked) visibility = 1; else visibility = 0;
            this.union.GetObjectAt(0).SetVisibility(visibility); ;
                
        }

        private void tsButtonPath2DView_Click(object sender, EventArgs e)
        {
            frmPath2DView.Visible = !frmPath2DView.Visible;
        }

        private void tsMenuItemPathShow_Click(object sender, EventArgs e)
        {
            tsMenuItemPathShow.Checked = !tsMenuItemPathShow.Checked;
            int visibility = 0; if (tsMenuItemPathShow.Checked) visibility = 1; else visibility = 0;
            this.union.GetSliceAt(0).SetVisibility(visibility); 
        }

        private void PaintLayer()
        {
            if (this.NumberOfLayers == 0)
                return;
            frmPath2DView.CalcSizeOffset();
            frmPath2DView.ScrollMax = this.NumberOfLayers - 1;
            frmPath2DView.SetLayer(Path.Layers[frmPath2DView.ScrollNumber]);
            //frmPath2DView.picBoxPath.Focus();
        }  // Paths窗口，传递Layer信息

    }
}
