namespace Jack.FoodTracker_MDI
{
    partial class RecordBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbCurrent = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.of = new System.Windows.Forms.Label();
            this.lbRecords = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbCurrent
            // 
            this.lbCurrent.AutoSize = true;
            this.lbCurrent.Location = new System.Drawing.Point(9, 16);
            this.lbCurrent.Name = "lbCurrent";
            this.lbCurrent.Size = new System.Drawing.Size(13, 13);
            this.lbCurrent.TabIndex = 1;
            this.lbCurrent.Text = "0";
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.Location = new System.Drawing.Point(50, 16);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(13, 13);
            this.lbTotal.TabIndex = 2;
            this.lbTotal.Text = "0";
            // 
            // of
            // 
            this.of.AutoSize = true;
            this.of.Location = new System.Drawing.Point(28, 16);
            this.of.Name = "of";
            this.of.Size = new System.Drawing.Size(16, 13);
            this.of.TabIndex = 3;
            this.of.Text = "of";
            // 
            // lbRecords
            // 
            this.lbRecords.AutoSize = true;
            this.lbRecords.Location = new System.Drawing.Point(69, 16);
            this.lbRecords.Name = "lbRecords";
            this.lbRecords.Size = new System.Drawing.Size(65, 13);
            this.lbRecords.TabIndex = 4;
            this.lbRecords.Text = "Title records";
            // 
            // RecordBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbRecords);
            this.Controls.Add(this.of);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.lbCurrent);
            this.Name = "RecordBar";
            this.Size = new System.Drawing.Size(155, 43);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


    }
}
