namespace UserPressureLossReport
{
   partial class ReportFormatNameDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportFormatNameDlg));
         this.Btn_Cancel = new System.Windows.Forms.Button();
         this.Btn_Yes = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // Btn_Cancel
         // 
         this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         resources.ApplyResources(this.Btn_Cancel, "Btn_Cancel");
         this.Btn_Cancel.Name = "Btn_Cancel";
         this.Btn_Cancel.UseVisualStyleBackColor = true;
         // 
         // Btn_Yes
         // 
         resources.ApplyResources(this.Btn_Yes, "Btn_Yes");
         this.Btn_Yes.Name = "Btn_Yes";
         this.Btn_Yes.UseVisualStyleBackColor = true;
         this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // textBox1
         // 
         resources.ApplyResources(this.textBox1, "textBox1");
         this.textBox1.Name = "textBox1";
         // 
         // ReportFormatNameDlg
         // 
         this.AcceptButton = this.Btn_Yes;
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.Btn_Cancel;
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.Btn_Cancel);
         this.Controls.Add(this.Btn_Yes);
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "ReportFormatNameDlg";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ReportFormatNameDlg_KeyUp);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button Btn_Cancel;
      private System.Windows.Forms.Button Btn_Yes;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textBox1;

   }
}