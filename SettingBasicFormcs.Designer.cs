namespace Nuwa
{
    partial class SettingBasicFormcs
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
            this.buttonSBCancel = new System.Windows.Forms.Button();
            this.buttonSBOK = new System.Windows.Forms.Button();
            this.textBoxBuildSpeed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLineSpacing = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLineHeight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelSC1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSBCancel
            // 
            this.buttonSBCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSBCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSBCancel.ForeColor = System.Drawing.Color.DimGray;
            this.buttonSBCancel.Image = global::Nuwa.Properties.Resources.closecancel;
            this.buttonSBCancel.Location = new System.Drawing.Point(953, -1);
            this.buttonSBCancel.Name = "buttonSBCancel";
            this.buttonSBCancel.Size = new System.Drawing.Size(45, 45);
            this.buttonSBCancel.TabIndex = 5;
            this.buttonSBCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSBCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSBOK
            // 
            this.buttonSBOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSBOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSBOK.ForeColor = System.Drawing.Color.DimGray;
            this.buttonSBOK.Image = global::Nuwa.Properties.Resources.checkOK;
            this.buttonSBOK.Location = new System.Drawing.Point(887, -1);
            this.buttonSBOK.Name = "buttonSBOK";
            this.buttonSBOK.Size = new System.Drawing.Size(45, 45);
            this.buttonSBOK.TabIndex = 6;
            this.buttonSBOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSBOK.UseVisualStyleBackColor = true;
            this.buttonSBOK.Click += new System.EventHandler(this.buttonSBOK_Click);
            // 
            // textBoxBuildSpeed
            // 
            this.textBoxBuildSpeed.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxBuildSpeed.Location = new System.Drawing.Point(510, 9);
            this.textBoxBuildSpeed.Name = "textBoxBuildSpeed";
            this.textBoxBuildSpeed.Size = new System.Drawing.Size(60, 26);
            this.textBoxBuildSpeed.TabIndex = 8;
            this.textBoxBuildSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxBuildSpeed_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(277, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 26);
            this.label2.TabIndex = 8;
            this.label2.Text = "线距LS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxLineSpacing
            // 
            this.textBoxLineSpacing.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxLineSpacing.Location = new System.Drawing.Point(354, 9);
            this.textBoxLineSpacing.Name = "textBoxLineSpacing";
            this.textBoxLineSpacing.Size = new System.Drawing.Size(60, 26);
            this.textBoxLineSpacing.TabIndex = 8;
            this.textBoxLineSpacing.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLineSpacing_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(118, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "层高 LH";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxLineHeight
            // 
            this.textBoxLineHeight.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxLineHeight.Location = new System.Drawing.Point(201, 9);
            this.textBoxLineHeight.Name = "textBoxLineHeight";
            this.textBoxLineHeight.Size = new System.Drawing.Size(60, 26);
            this.textBoxLineHeight.TabIndex = 8;
            this.textBoxLineHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLineHeight_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(431, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 26);
            this.label3.TabIndex = 8;
            this.label3.Text = "速度SP";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSC1
            // 
            this.labelSC1.BackColor = System.Drawing.Color.SteelBlue;
            this.labelSC1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSC1.ForeColor = System.Drawing.Color.White;
            this.labelSC1.Location = new System.Drawing.Point(1, 0);
            this.labelSC1.Name = "labelSC1";
            this.labelSC1.Size = new System.Drawing.Size(106, 45);
            this.labelSC1.TabIndex = 9;
            this.labelSC1.Text = "基础参数";
            this.labelSC1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingBasicFormcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1000, 45);
            this.ControlBox = false;
            this.Controls.Add(this.labelSC1);
            this.Controls.Add(this.textBoxBuildSpeed);
            this.Controls.Add(this.buttonSBCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSBOK);
            this.Controls.Add(this.textBoxLineSpacing);
            this.Controls.Add(this.textBoxLineHeight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingBasicFormcs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "BasicSettingFormcs";
            this.Load += new System.EventHandler(this.SettingBasicFormcs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSBCancel;
        private System.Windows.Forms.Button buttonSBOK;
        private System.Windows.Forms.TextBox textBoxBuildSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLineSpacing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLineHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelSC1;

    }
}