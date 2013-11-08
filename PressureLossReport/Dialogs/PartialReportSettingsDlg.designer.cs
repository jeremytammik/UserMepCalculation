namespace UserPressureLossReport
{
   partial class PartialReportSettingsDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartialReportSettingsDlg));
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonOK = new System.Windows.Forms.Button();
         this.buttonDown = new System.Windows.Forms.Button();
         this.buttonUp = new System.Windows.Forms.Button();
         this.buttonRemove = new System.Windows.Forms.Button();
         this.buttonAdd = new System.Windows.Forms.Button();
         this.listBoxReportFields = new System.Windows.Forms.ListBox();
         this.labelReportFields = new System.Windows.Forms.Label();
         this.listBoxAvailableFields = new System.Windows.Forms.ListBox();
         this.labelAvailableFields = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // buttonCancel
         // 
         resources.ApplyResources(this.buttonCancel, "buttonCancel");
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         // 
         // buttonOK
         // 
         resources.ApplyResources(this.buttonOK, "buttonOK");
         this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.buttonOK.Name = "buttonOK";
         this.buttonOK.UseVisualStyleBackColor = true;
         this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
         // 
         // buttonDown
         // 
         resources.ApplyResources(this.buttonDown, "buttonDown");
         this.buttonDown.Name = "buttonDown";
         this.buttonDown.UseVisualStyleBackColor = true;
         this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
         // 
         // buttonUp
         // 
         resources.ApplyResources(this.buttonUp, "buttonUp");
         this.buttonUp.Name = "buttonUp";
         this.buttonUp.UseVisualStyleBackColor = true;
         this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
         // 
         // buttonRemove
         // 
         resources.ApplyResources(this.buttonRemove, "buttonRemove");
         this.buttonRemove.Name = "buttonRemove";
         this.buttonRemove.UseVisualStyleBackColor = true;
         this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
         // 
         // buttonAdd
         // 
         resources.ApplyResources(this.buttonAdd, "buttonAdd");
         this.buttonAdd.Name = "buttonAdd";
         this.buttonAdd.UseVisualStyleBackColor = true;
         this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
         // 
         // listBoxReportFields
         // 
         resources.ApplyResources(this.listBoxReportFields, "listBoxReportFields");
         this.listBoxReportFields.FormattingEnabled = true;
         this.listBoxReportFields.Name = "listBoxReportFields";
         this.listBoxReportFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.listBoxReportFields.SelectedIndexChanged += new System.EventHandler(this.listBoxReportFields_SelectedIndexChanged);
         this.listBoxReportFields.DoubleClick += new System.EventHandler(this.listBoxReportFields_DoubleClick);
         // 
         // labelReportFields
         // 
         resources.ApplyResources(this.labelReportFields, "labelReportFields");
         this.labelReportFields.Name = "labelReportFields";
         // 
         // listBoxAvailableFields
         // 
         resources.ApplyResources(this.listBoxAvailableFields, "listBoxAvailableFields");
         this.listBoxAvailableFields.FormattingEnabled = true;
         this.listBoxAvailableFields.Name = "listBoxAvailableFields";
         this.listBoxAvailableFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.listBoxAvailableFields.Sorted = true;
         this.listBoxAvailableFields.DoubleClick += new System.EventHandler(this.listBoxAvailableFields_DoubleClick);
         // 
         // labelAvailableFields
         // 
         resources.ApplyResources(this.labelAvailableFields, "labelAvailableFields");
         this.labelAvailableFields.Name = "labelAvailableFields";
         // 
         // PartialReportSettingsDlg
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.buttonOK);
         this.Controls.Add(this.buttonDown);
         this.Controls.Add(this.buttonUp);
         this.Controls.Add(this.buttonRemove);
         this.Controls.Add(this.buttonAdd);
         this.Controls.Add(this.listBoxReportFields);
         this.Controls.Add(this.labelReportFields);
         this.Controls.Add(this.listBoxAvailableFields);
         this.Controls.Add(this.labelAvailableFields);
         this.HelpButton = true;
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "PartialReportSettingsDlg";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PartialReportSettingsDlg_KeyUp);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonOK;
      private System.Windows.Forms.Button buttonDown;
      private System.Windows.Forms.Button buttonUp;
      private System.Windows.Forms.Button buttonRemove;
      private System.Windows.Forms.Button buttonAdd;
      private System.Windows.Forms.ListBox listBoxReportFields;
      private System.Windows.Forms.Label labelReportFields;
      private System.Windows.Forms.ListBox listBoxAvailableFields;
      private System.Windows.Forms.Label labelAvailableFields;


   }
}