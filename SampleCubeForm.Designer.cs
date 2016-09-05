namespace Nuwa
{
    partial class SampleCubeForm
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
            this.textBoxCUL = new System.Windows.Forms.TextBox();
            this.textBoxCUW = new System.Windows.Forms.TextBox();
            this.textBoxCUH = new System.Windows.Forms.TextBox();
            this.labelCU2 = new System.Windows.Forms.Label();
            this.labelCU3 = new System.Windows.Forms.Label();
            this.labelCU4 = new System.Windows.Forms.Label();
            this.labelCU5 = new System.Windows.Forms.Label();
            this.comboBoxCUF = new System.Windows.Forms.ComboBox();
            this.labelCU1 = new System.Windows.Forms.Label();
            this.ButtonCUCancel = new System.Windows.Forms.Button();
            this.ButtonCUOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxCUL
            // 
            this.textBoxCUL.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCUL.Location = new System.Drawing.Point(137, 8);
            this.textBoxCUL.Name = "textBoxCUL";
            this.textBoxCUL.Size = new System.Drawing.Size(74, 26);
            this.textBoxCUL.TabIndex = 1;
            this.textBoxCUL.Text = "10";
            // 
            // textBoxCUW
            // 
            this.textBoxCUW.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCUW.Location = new System.Drawing.Point(299, 8);
            this.textBoxCUW.Name = "textBoxCUW";
            this.textBoxCUW.Size = new System.Drawing.Size(74, 26);
            this.textBoxCUW.TabIndex = 1;
            this.textBoxCUW.Text = "10";
            // 
            // textBoxCUH
            // 
            this.textBoxCUH.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCUH.Location = new System.Drawing.Point(455, 8);
            this.textBoxCUH.Name = "textBoxCUH";
            this.textBoxCUH.Size = new System.Drawing.Size(76, 26);
            this.textBoxCUH.TabIndex = 1;
            this.textBoxCUH.Text = "2";
            // 
            // labelCU2
            // 
            this.labelCU2.AutoSize = true;
            this.labelCU2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCU2.ForeColor = System.Drawing.Color.White;
            this.labelCU2.Location = new System.Drawing.Point(75, 8);
            this.labelCU2.Name = "labelCU2";
            this.labelCU2.Size = new System.Drawing.Size(56, 22);
            this.labelCU2.TabIndex = 2;
            this.labelCU2.Text = "长度 L";
            // 
            // labelCU3
            // 
            this.labelCU3.AutoSize = true;
            this.labelCU3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCU3.ForeColor = System.Drawing.Color.White;
            this.labelCU3.Location = new System.Drawing.Point(229, 8);
            this.labelCU3.Name = "labelCU3";
            this.labelCU3.Size = new System.Drawing.Size(64, 22);
            this.labelCU3.TabIndex = 2;
            this.labelCU3.Text = "宽度 W";
            // 
            // labelCU4
            // 
            this.labelCU4.AutoSize = true;
            this.labelCU4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCU4.ForeColor = System.Drawing.Color.White;
            this.labelCU4.Location = new System.Drawing.Point(388, 8);
            this.labelCU4.Name = "labelCU4";
            this.labelCU4.Size = new System.Drawing.Size(60, 22);
            this.labelCU4.TabIndex = 2;
            this.labelCU4.Text = "高度 H";
            // 
            // labelCU5
            // 
            this.labelCU5.AutoSize = true;
            this.labelCU5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCU5.ForeColor = System.Drawing.Color.White;
            this.labelCU5.Location = new System.Drawing.Point(547, 8);
            this.labelCU5.Name = "labelCU5";
            this.labelCU5.Size = new System.Drawing.Size(88, 22);
            this.labelCU5.TabIndex = 2;
            this.labelCU5.Text = "填充方式 F";
            // 
            // comboBoxCUF
            // 
            this.comboBoxCUF.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxCUF.FormattingEnabled = true;
            this.comboBoxCUF.Items.AddRange(new object[] {
            "扫描填充",
            "等距填充"});
            this.comboBoxCUF.Location = new System.Drawing.Point(642, 8);
            this.comboBoxCUF.Name = "comboBoxCUF";
            this.comboBoxCUF.Size = new System.Drawing.Size(76, 28);
            this.comboBoxCUF.TabIndex = 5;
            // 
            // labelCU1
            // 
            this.labelCU1.BackColor = System.Drawing.Color.SteelBlue;
            this.labelCU1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCU1.ForeColor = System.Drawing.Color.White;
            this.labelCU1.Location = new System.Drawing.Point(-1, -1);
            this.labelCU1.Name = "labelCU1";
            this.labelCU1.Size = new System.Drawing.Size(66, 41);
            this.labelCU1.TabIndex = 0;
            this.labelCU1.Text = "方块";
            this.labelCU1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonCUCancel
            // 
            this.ButtonCUCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCUCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonCUCancel.ForeColor = System.Drawing.Color.DimGray;
            this.ButtonCUCancel.Image = global::Nuwa.Properties.Resources.closecancel;
            this.ButtonCUCancel.Location = new System.Drawing.Point(940, 0);
            this.ButtonCUCancel.Name = "ButtonCUCancel";
            this.ButtonCUCancel.Size = new System.Drawing.Size(58, 40);
            this.ButtonCUCancel.TabIndex = 4;
            this.ButtonCUCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ButtonCUCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonCUOK
            // 
            this.ButtonCUOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonCUOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonCUOK.ForeColor = System.Drawing.Color.DimGray;
            this.ButtonCUOK.Image = global::Nuwa.Properties.Resources.checkOK;
            this.ButtonCUOK.Location = new System.Drawing.Point(876, 0);
            this.ButtonCUOK.Name = "ButtonCUOK";
            this.ButtonCUOK.Size = new System.Drawing.Size(58, 40);
            this.ButtonCUOK.TabIndex = 4;
            this.ButtonCUOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ButtonCUOK.UseVisualStyleBackColor = true;
            this.ButtonCUOK.Click += new System.EventHandler(this.ButtonCUOK_Click);
            // 
            // SampleCubeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1000, 40);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxCUF);
            this.Controls.Add(this.ButtonCUCancel);
            this.Controls.Add(this.ButtonCUOK);
            this.Controls.Add(this.labelCU5);
            this.Controls.Add(this.labelCU4);
            this.Controls.Add(this.labelCU3);
            this.Controls.Add(this.labelCU2);
            this.Controls.Add(this.textBoxCUH);
            this.Controls.Add(this.textBoxCUW);
            this.Controls.Add(this.textBoxCUL);
            this.Controls.Add(this.labelCU1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SampleCubeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SampleTube";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SampleCubeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCU1;
        private System.Windows.Forms.TextBox textBoxCUL;
        private System.Windows.Forms.TextBox textBoxCUW;
        private System.Windows.Forms.TextBox textBoxCUH;
        private System.Windows.Forms.Label labelCU2;
        private System.Windows.Forms.Label labelCU3;
        private System.Windows.Forms.Label labelCU4;
        private System.Windows.Forms.Label labelCU5;
        private System.Windows.Forms.Button ButtonCUOK;
        private System.Windows.Forms.Button ButtonCUCancel;
        private System.Windows.Forms.ComboBox comboBoxCUF;
    }
}