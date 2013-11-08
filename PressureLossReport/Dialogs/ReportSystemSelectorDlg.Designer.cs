namespace UserPressureLossReport
{
   partial class ReportSystemSelectorDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportSystemSelectorDlg));
         this.label1 = new System.Windows.Forms.Label();
         this.btnSystemTypeFilter = new System.Windows.Forms.Button();
         this.SystemCheckList = new System.Windows.Forms.CheckedListBox();
         this.btnSelectAll = new System.Windows.Forms.Button();
         this.btnNone = new System.Windows.Forms.Button();
         this.btnInvert = new System.Windows.Forms.Button();
         this.btnOK = new System.Windows.Forms.Button();
         this.btCancel = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // btnSystemTypeFilter
         // 
         resources.ApplyResources(this.btnSystemTypeFilter, "btnSystemTypeFilter");
         this.btnSystemTypeFilter.Name = "btnSystemTypeFilter";
         this.btnSystemTypeFilter.UseVisualStyleBackColor = true;
         this.btnSystemTypeFilter.Click += new System.EventHandler(this.btnSystemTypeFilter_Click);
         // 
         // SystemCheckList
         // 
         resources.ApplyResources(this.SystemCheckList, "SystemCheckList");
         this.SystemCheckList.CheckOnClick = true;
         this.SystemCheckList.FormattingEnabled = true;
         this.SystemCheckList.Name = "SystemCheckList";
         this.SystemCheckList.SelectedIndexChanged += new System.EventHandler(this.SystemCheckList_SelectedIndexChanged);
         // 
         // btnSelectAll
         // 
         resources.ApplyResources(this.btnSelectAll, "btnSelectAll");
         this.btnSelectAll.Name = "btnSelectAll";
         this.btnSelectAll.UseVisualStyleBackColor = true;
         this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
         // 
         // btnNone
         // 
         resources.ApplyResources(this.btnNone, "btnNone");
         this.btnNone.Name = "btnNone";
         this.btnNone.UseVisualStyleBackColor = true;
         this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
         // 
         // btnInvert
         // 
         resources.ApplyResources(this.btnInvert, "btnInvert");
         this.btnInvert.Name = "btnInvert";
         this.btnInvert.UseVisualStyleBackColor = true;
         this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
         // 
         // btnOK
         // 
         resources.ApplyResources(this.btnOK, "btnOK");
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Name = "btnOK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btCancel
         // 
         resources.ApplyResources(this.btCancel, "btCancel");
         this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btCancel.Name = "btCancel";
         this.btCancel.UseVisualStyleBackColor = true;
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // ReportSystemSelectorDlg
         // 
         this.AcceptButton = this.btnOK;
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btCancel;
         this.Controls.Add(this.btCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.btnInvert);
         this.Controls.Add(this.btnNone);
         this.Controls.Add(this.btnSelectAll);
         this.Controls.Add(this.SystemCheckList);
         this.Controls.Add(this.btnSystemTypeFilter);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label1);
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "ReportSystemSelectorDlg";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button btnSystemTypeFilter;
      private System.Windows.Forms.CheckedListBox SystemCheckList;
      private System.Windows.Forms.Button btnSelectAll;
      private System.Windows.Forms.Button btnNone;
      private System.Windows.Forms.Button btnInvert;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btCancel;
      private System.Windows.Forms.Label label3;
   }
}