namespace Nuwa
{
    partial class Path2DViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBarLayer = new CCWin.SkinControl.SkinTrackBar();
            this.picBoxPath = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelZ = new System.Windows.Forms.Label();
            this.labelS = new System.Windows.Forms.Label();
            this.labelLayerNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxPath)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBarLayer
            // 
            this.trackBarLayer.BackColor = System.Drawing.Color.Transparent;
            this.trackBarLayer.Bar = null;
            this.trackBarLayer.BarStyle = CCWin.SkinControl.HSLTrackBarStyle.Opacity;
            this.trackBarLayer.Location = new System.Drawing.Point(946, 139);
            this.trackBarLayer.Name = "trackBarLayer";
            this.trackBarLayer.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarLayer.Size = new System.Drawing.Size(45, 395);
            this.trackBarLayer.TabIndex = 2;
            this.trackBarLayer.Track = null;
            this.trackBarLayer.Scroll += new System.EventHandler(this.trackBarLayer_Scroll);
            // 
            // picBoxPath
            // 
            this.picBoxPath.BackColor = System.Drawing.Color.DimGray;
            this.picBoxPath.Location = new System.Drawing.Point(29, 57);
            this.picBoxPath.Name = "picBoxPath";
            this.picBoxPath.Size = new System.Drawing.Size(911, 477);
            this.picBoxPath.TabIndex = 1;
            this.picBoxPath.TabStop = false;
            this.picBoxPath.SizeChanged += new System.EventHandler(this.picBoxPath_SizeChanged);
            this.picBoxPath.Paint += new System.Windows.Forms.PaintEventHandler(this.picBoxPath_Paint);
            this.picBoxPath.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoxPath_MouseDown);
            this.picBoxPath.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picBoxPath_MouseMove);
            this.picBoxPath.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBoxPath_MouseUp);
            this.picBoxPath.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.picBoxPath_MouseWheel);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.DarkGray;
            this.button1.Image = global::Nuwa.Properties.Resources.iconClose;
            this.button1.Location = new System.Drawing.Point(936, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 54);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelX, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelZ, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelS, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(29, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(677, 39);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelX.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelX.ForeColor = System.Drawing.Color.White;
            this.labelX.Location = new System.Drawing.Point(3, 0);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(94, 39);
            this.labelX.TabIndex = 0;
            this.labelX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelY.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelY.ForeColor = System.Drawing.Color.White;
            this.labelY.Location = new System.Drawing.Point(103, 0);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(94, 39);
            this.labelY.TabIndex = 0;
            this.labelY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelZ
            // 
            this.labelZ.AutoSize = true;
            this.labelZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelZ.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelZ.ForeColor = System.Drawing.Color.White;
            this.labelZ.Location = new System.Drawing.Point(203, 0);
            this.labelZ.Name = "labelZ";
            this.labelZ.Size = new System.Drawing.Size(94, 39);
            this.labelZ.TabIndex = 0;
            this.labelZ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelS
            // 
            this.labelS.AutoSize = true;
            this.labelS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelS.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelS.ForeColor = System.Drawing.Color.White;
            this.labelS.Location = new System.Drawing.Point(303, 0);
            this.labelS.Name = "labelS";
            this.labelS.Size = new System.Drawing.Size(144, 39);
            this.labelS.TabIndex = 0;
            this.labelS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelLayerNumber
            // 
            this.labelLayerNumber.AutoSize = true;
            this.labelLayerNumber.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelLayerNumber.ForeColor = System.Drawing.Color.White;
            this.labelLayerNumber.Location = new System.Drawing.Point(953, 73);
            this.labelLayerNumber.Name = "labelLayerNumber";
            this.labelLayerNumber.Size = new System.Drawing.Size(38, 22);
            this.labelLayerNumber.TabIndex = 19;
            this.labelLayerNumber.Text = "1/1";
            // 
            // Path2DViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1010, 554);
            this.Controls.Add(this.labelLayerNumber);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.trackBarLayer);
            this.Controls.Add(this.picBoxPath);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Path2DViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Path2DViewForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxPath)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox picBoxPath;
        private CCWin.SkinControl.SkinTrackBar trackBarLayer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelZ;
        private System.Windows.Forms.Label labelS;
        private System.Windows.Forms.Label labelLayerNumber;
    }
}