namespace Overkill
{
    partial class OverkillForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbMaterial = new System.Windows.Forms.CheckBox();
            this.cbPlotStyle = new System.Windows.Forms.CheckBox();
            this.cbTransparency = new System.Windows.Forms.CheckBox();
            this.cbThickness = new System.Windows.Forms.CheckBox();
            this.cbLineweight = new System.Windows.Forms.CheckBox();
            this.cbLinetypeScale = new System.Windows.Forms.CheckBox();
            this.cbLinetype = new System.Windows.Forms.CheckBox();
            this.cbLayer = new System.Windows.Forms.CheckBox();
            this.cbColor = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTolerance = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbMaintainAssoc = new System.Windows.Forms.CheckBox();
            this.cbCombineEndToEnd = new System.Windows.Forms.CheckBox();
            this.cbCombileOverlappings = new System.Windows.Forms.CheckBox();
            this.cbDontBreakPolylines = new System.Windows.Forms.CheckBox();
            this.cbIgnorePolylineWidths = new System.Windows.Forms.CheckBox();
            this.cbOptimizePolylines = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbMaterial);
            this.groupBox1.Controls.Add(this.cbPlotStyle);
            this.groupBox1.Controls.Add(this.cbTransparency);
            this.groupBox1.Controls.Add(this.cbThickness);
            this.groupBox1.Controls.Add(this.cbLineweight);
            this.groupBox1.Controls.Add(this.cbLinetypeScale);
            this.groupBox1.Controls.Add(this.cbLinetype);
            this.groupBox1.Controls.Add(this.cbLayer);
            this.groupBox1.Controls.Add(this.cbColor);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbTolerance);
            this.groupBox1.Location = new System.Drawing.Point(23, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 241);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Object Comparison Settings";
            // 
            // cbMaterial
            // 
            this.cbMaterial.AutoSize = true;
            this.cbMaterial.Location = new System.Drawing.Point(234, 180);
            this.cbMaterial.Name = "cbMaterial";
            this.cbMaterial.Size = new System.Drawing.Size(80, 21);
            this.cbMaterial.TabIndex = 11;
            this.cbMaterial.Text = "&Material";
            this.cbMaterial.UseVisualStyleBackColor = true;
            // 
            // cbPlotStyle
            // 
            this.cbPlotStyle.AutoSize = true;
            this.cbPlotStyle.Location = new System.Drawing.Point(234, 153);
            this.cbPlotStyle.Name = "cbPlotStyle";
            this.cbPlotStyle.Size = new System.Drawing.Size(87, 21);
            this.cbPlotStyle.TabIndex = 10;
            this.cbPlotStyle.Text = "Plot &style";
            this.cbPlotStyle.UseVisualStyleBackColor = true;
            // 
            // cbTransparency
            // 
            this.cbTransparency.AutoSize = true;
            this.cbTransparency.Location = new System.Drawing.Point(234, 126);
            this.cbTransparency.Name = "cbTransparency";
            this.cbTransparency.Size = new System.Drawing.Size(118, 21);
            this.cbTransparency.TabIndex = 9;
            this.cbTransparency.Text = "T&ransparency";
            this.cbTransparency.UseVisualStyleBackColor = true;
            // 
            // cbThickness
            // 
            this.cbThickness.AutoSize = true;
            this.cbThickness.Location = new System.Drawing.Point(234, 99);
            this.cbThickness.Name = "cbThickness";
            this.cbThickness.Size = new System.Drawing.Size(94, 21);
            this.cbThickness.TabIndex = 8;
            this.cbThickness.Text = "&Thickness";
            this.cbThickness.UseVisualStyleBackColor = true;
            // 
            // cbLineweight
            // 
            this.cbLineweight.AutoSize = true;
            this.cbLineweight.Location = new System.Drawing.Point(20, 207);
            this.cbLineweight.Name = "cbLineweight";
            this.cbLineweight.Size = new System.Drawing.Size(97, 21);
            this.cbLineweight.TabIndex = 7;
            this.cbLineweight.Text = "Line&weight";
            this.cbLineweight.UseVisualStyleBackColor = true;
            // 
            // cbLinetypeScale
            // 
            this.cbLinetypeScale.AutoSize = true;
            this.cbLinetypeScale.Location = new System.Drawing.Point(20, 180);
            this.cbLinetypeScale.Name = "cbLinetypeScale";
            this.cbLinetypeScale.Size = new System.Drawing.Size(121, 21);
            this.cbLinetypeScale.TabIndex = 6;
            this.cbLinetypeScale.Text = "Linet&ype scale";
            this.cbLinetypeScale.UseVisualStyleBackColor = true;
            // 
            // cbLinetype
            // 
            this.cbLinetype.AutoSize = true;
            this.cbLinetype.Location = new System.Drawing.Point(20, 153);
            this.cbLinetype.Name = "cbLinetype";
            this.cbLinetype.Size = new System.Drawing.Size(84, 21);
            this.cbLinetype.TabIndex = 5;
            this.cbLinetype.Text = "L&inetype";
            this.cbLinetype.UseVisualStyleBackColor = true;
            // 
            // cbLayer
            // 
            this.cbLayer.AutoSize = true;
            this.cbLayer.Location = new System.Drawing.Point(20, 126);
            this.cbLayer.Name = "cbLayer";
            this.cbLayer.Size = new System.Drawing.Size(66, 21);
            this.cbLayer.TabIndex = 4;
            this.cbLayer.Text = "&Layer";
            this.cbLayer.UseVisualStyleBackColor = true;
            // 
            // cbColor
            // 
            this.cbColor.AutoSize = true;
            this.cbColor.Location = new System.Drawing.Point(20, 99);
            this.cbColor.Name = "cbColor";
            this.cbColor.Size = new System.Drawing.Size(63, 21);
            this.cbColor.TabIndex = 3;
            this.cbColor.Text = "&Color";
            this.cbColor.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ignore object property:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tolera&nce";
            // 
            // tbTolerance
            // 
            this.tbTolerance.Location = new System.Drawing.Point(96, 33);
            this.tbTolerance.Name = "tbTolerance";
            this.tbTolerance.Size = new System.Drawing.Size(100, 22);
            this.tbTolerance.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbMaintainAssoc);
            this.groupBox2.Controls.Add(this.cbCombineEndToEnd);
            this.groupBox2.Controls.Add(this.cbCombileOverlappings);
            this.groupBox2.Controls.Add(this.cbDontBreakPolylines);
            this.groupBox2.Controls.Add(this.cbIgnorePolylineWidths);
            this.groupBox2.Controls.Add(this.cbOptimizePolylines);
            this.groupBox2.Location = new System.Drawing.Point(23, 273);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(428, 202);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // cbMaintainAssoc
            // 
            this.cbMaintainAssoc.AutoSize = true;
            this.cbMaintainAssoc.Location = new System.Drawing.Point(20, 167);
            this.cbMaintainAssoc.Name = "cbMaintainAssoc";
            this.cbMaintainAssoc.Size = new System.Drawing.Size(206, 21);
            this.cbMaintainAssoc.TabIndex = 5;
            this.cbMaintainAssoc.Text = "Maintain &associative objects";
            this.cbMaintainAssoc.UseVisualStyleBackColor = true;
            // 
            // cbCombineEndToEnd
            // 
            this.cbCombineEndToEnd.AutoSize = true;
            this.cbCombineEndToEnd.Location = new System.Drawing.Point(20, 140);
            this.cbCombineEndToEnd.Name = "cbCombineEndToEnd";
            this.cbCombineEndToEnd.Size = new System.Drawing.Size(356, 21);
            this.cbCombineEndToEnd.TabIndex = 4;
            this.cbCombineEndToEnd.Text = "Combine co-linear objects when aligned  &end to end";
            this.cbCombineEndToEnd.UseVisualStyleBackColor = true;
            // 
            // cbCombileOverlappings
            // 
            this.cbCombileOverlappings.AutoSize = true;
            this.cbCombileOverlappings.Location = new System.Drawing.Point(20, 113);
            this.cbCombileOverlappings.Name = "cbCombileOverlappings";
            this.cbCombileOverlappings.Size = new System.Drawing.Size(325, 21);
            this.cbCombileOverlappings.TabIndex = 3;
            this.cbCombileOverlappings.Text = "Combine co-linear objects that partially o&verlap";
            this.cbCombileOverlappings.UseVisualStyleBackColor = true;
            // 
            // cbDontBreakPolylines
            // 
            this.cbDontBreakPolylines.AutoSize = true;
            this.cbDontBreakPolylines.Location = new System.Drawing.Point(35, 86);
            this.cbDontBreakPolylines.Name = "cbDontBreakPolylines";
            this.cbDontBreakPolylines.Size = new System.Drawing.Size(171, 21);
            this.cbDontBreakPolylines.TabIndex = 2;
            this.cbDontBreakPolylines.Text = "Do not &break polylines";
            this.cbDontBreakPolylines.UseVisualStyleBackColor = true;
            // 
            // cbIgnorePolylineWidths
            // 
            this.cbIgnorePolylineWidths.AutoSize = true;
            this.cbIgnorePolylineWidths.Location = new System.Drawing.Point(35, 59);
            this.cbIgnorePolylineWidths.Name = "cbIgnorePolylineWidths";
            this.cbIgnorePolylineWidths.Size = new System.Drawing.Size(230, 21);
            this.cbIgnorePolylineWidths.TabIndex = 1;
            this.cbIgnorePolylineWidths.Text = "Ignore polyline segments wi&dths";
            this.cbIgnorePolylineWidths.UseVisualStyleBackColor = true;
            // 
            // cbOptimizePolylines
            // 
            this.cbOptimizePolylines.AutoSize = true;
            this.cbOptimizePolylines.Location = new System.Drawing.Point(20, 32);
            this.cbOptimizePolylines.Name = "cbOptimizePolylines";
            this.cbOptimizePolylines.Size = new System.Drawing.Size(240, 21);
            this.cbOptimizePolylines.TabIndex = 0;
            this.cbOptimizePolylines.Text = "Optimize segments witin &polylines";
            this.cbOptimizePolylines.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(160, 481);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(93, 29);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 481);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 29);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(358, 481);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(93, 29);
            this.btnHelp.TabIndex = 4;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // OverkillForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(467, 519);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverkillForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete Duplicate Objects";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverkillForm_FormClosing);
            this.Load += new System.EventHandler(this.OverkillForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbMaterial;
        private System.Windows.Forms.CheckBox cbPlotStyle;
        private System.Windows.Forms.CheckBox cbTransparency;
        private System.Windows.Forms.CheckBox cbThickness;
        private System.Windows.Forms.CheckBox cbLineweight;
        private System.Windows.Forms.CheckBox cbLinetypeScale;
        private System.Windows.Forms.CheckBox cbLinetype;
        private System.Windows.Forms.CheckBox cbLayer;
        private System.Windows.Forms.CheckBox cbColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTolerance;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbMaintainAssoc;
        private System.Windows.Forms.CheckBox cbCombineEndToEnd;
        private System.Windows.Forms.CheckBox cbCombileOverlappings;
        private System.Windows.Forms.CheckBox cbDontBreakPolylines;
        private System.Windows.Forms.CheckBox cbIgnorePolylineWidths;
        private System.Windows.Forms.CheckBox cbOptimizePolylines;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelp;
    }
}