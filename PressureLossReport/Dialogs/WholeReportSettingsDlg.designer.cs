namespace UserPressureLossReport
{
   partial class WholeReportSettingsDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WholeReportSettingsDlg));
         this.labelFormat = new System.Windows.Forms.Label();
         this.comboBoxFormat = new System.Windows.Forms.ComboBox();
         this.buttonFormatDelete = new System.Windows.Forms.Button();
         this.labelAvailableFields = new System.Windows.Forms.Label();
         this.listBoxAvailableFields = new System.Windows.Forms.ListBox();
         this.labelReportFields = new System.Windows.Forms.Label();
         this.buttonAdd = new System.Windows.Forms.Button();
         this.buttonRemove = new System.Windows.Forms.Button();
         this.checkBoxDisplaySystemInfo = new System.Windows.Forms.CheckBox();
         this.checkBoxCriticalPath = new System.Windows.Forms.CheckBox();
         this.checkBoxSegmentInfo = new System.Windows.Forms.CheckBox();
         this.checkBoxFittingInfo = new System.Windows.Forms.CheckBox();
         this.buttonGenerate = new System.Windows.Forms.Button();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonSaveFormat = new System.Windows.Forms.Button();
         this.checkBoxOpenAfterCreated = new System.Windows.Forms.CheckBox();
         this.buttonDown = new System.Windows.Forms.Button();
         this.buttonUp = new System.Windows.Forms.Button();
         this.buttonSegmentSettings = new System.Windows.Forms.Button();
         this.buttonFittingSettings = new System.Windows.Forms.Button();
         this.listBoxReportFields = new System.Windows.Forms.ListBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.SuspendLayout();
         // 
         // labelFormat
         // 
         resources.ApplyResources(this.labelFormat, "labelFormat");
         this.labelFormat.Name = "labelFormat";
         // 
         // comboBoxFormat
         // 
         resources.ApplyResources(this.comboBoxFormat, "comboBoxFormat");
         this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBoxFormat.FormattingEnabled = true;
         this.comboBoxFormat.Name = "comboBoxFormat";
         this.comboBoxFormat.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormat_SelectedIndexChanged);
         // 
         // buttonFormatDelete
         // 
         resources.ApplyResources(this.buttonFormatDelete, "buttonFormatDelete");
         this.buttonFormatDelete.Name = "buttonFormatDelete";
         this.buttonFormatDelete.UseVisualStyleBackColor = true;
         this.buttonFormatDelete.Click += new System.EventHandler(this.buttonFormatDelete_Click);
         // 
         // labelAvailableFields
         // 
         resources.ApplyResources(this.labelAvailableFields, "labelAvailableFields");
         this.labelAvailableFields.Name = "labelAvailableFields";
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
         // labelReportFields
         // 
         resources.ApplyResources(this.labelReportFields, "labelReportFields");
         this.labelReportFields.Name = "labelReportFields";
         // 
         // buttonAdd
         // 
         resources.ApplyResources(this.buttonAdd, "buttonAdd");
         this.buttonAdd.Name = "buttonAdd";
         this.buttonAdd.UseVisualStyleBackColor = true;
         this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
         // 
         // buttonRemove
         // 
         resources.ApplyResources(this.buttonRemove, "buttonRemove");
         this.buttonRemove.Name = "buttonRemove";
         this.buttonRemove.UseVisualStyleBackColor = true;
         this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
         // 
         // checkBoxDisplaySystemInfo
         // 
         resources.ApplyResources(this.checkBoxDisplaySystemInfo, "checkBoxDisplaySystemInfo");
         this.checkBoxDisplaySystemInfo.Name = "checkBoxDisplaySystemInfo";
         this.checkBoxDisplaySystemInfo.UseVisualStyleBackColor = true;
         // 
         // checkBoxCriticalPath
         // 
         resources.ApplyResources(this.checkBoxCriticalPath, "checkBoxCriticalPath");
         this.checkBoxCriticalPath.Name = "checkBoxCriticalPath";
         this.checkBoxCriticalPath.UseVisualStyleBackColor = true;
         // 
         // checkBoxSegmentInfo
         // 
         resources.ApplyResources(this.checkBoxSegmentInfo, "checkBoxSegmentInfo");
         this.checkBoxSegmentInfo.Name = "checkBoxSegmentInfo";
         this.checkBoxSegmentInfo.UseVisualStyleBackColor = true;
         // 
         // checkBoxFittingInfo
         // 
         resources.ApplyResources(this.checkBoxFittingInfo, "checkBoxFittingInfo");
         this.checkBoxFittingInfo.Name = "checkBoxFittingInfo";
         this.checkBoxFittingInfo.UseVisualStyleBackColor = true;
         // 
         // buttonGenerate
         // 
         resources.ApplyResources(this.buttonGenerate, "buttonGenerate");
         this.buttonGenerate.Name = "buttonGenerate";
         this.buttonGenerate.UseVisualStyleBackColor = true;
         this.buttonGenerate.Click += new System.EventHandler(this.OnGenerateReport);
         // 
         // buttonCancel
         // 
         resources.ApplyResources(this.buttonCancel, "buttonCancel");
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         // 
         // buttonSaveFormat
         // 
         resources.ApplyResources(this.buttonSaveFormat, "buttonSaveFormat");
         this.buttonSaveFormat.Name = "buttonSaveFormat";
         this.buttonSaveFormat.UseVisualStyleBackColor = true;
         this.buttonSaveFormat.Click += new System.EventHandler(this.buttonSaveFormat_Click);
         // 
         // checkBoxOpenAfterCreated
         // 
         resources.ApplyResources(this.checkBoxOpenAfterCreated, "checkBoxOpenAfterCreated");
         this.checkBoxOpenAfterCreated.Name = "checkBoxOpenAfterCreated";
         this.checkBoxOpenAfterCreated.UseVisualStyleBackColor = true;
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
         // buttonSegmentSettings
         // 
         resources.ApplyResources(this.buttonSegmentSettings, "buttonSegmentSettings");
         this.buttonSegmentSettings.Name = "buttonSegmentSettings";
         this.buttonSegmentSettings.UseVisualStyleBackColor = true;
         this.buttonSegmentSettings.Click += new System.EventHandler(this.OnChangeSegmentReportSettings);
         // 
         // buttonFittingSettings
         // 
         resources.ApplyResources(this.buttonFittingSettings, "buttonFittingSettings");
         this.buttonFittingSettings.Name = "buttonFittingSettings";
         this.buttonFittingSettings.UseVisualStyleBackColor = true;
         this.buttonFittingSettings.Click += new System.EventHandler(this.OnChangeFittingReportSettings);
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
         // groupBox1
         // 
         resources.ApplyResources(this.groupBox1, "groupBox1");
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.TabStop = false;
         // 
         // WholeReportSettingsDlg
         // 
         this.AcceptButton = this.buttonGenerate;
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.buttonCancel;
         this.Controls.Add(this.buttonSaveFormat);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.buttonGenerate);
         this.Controls.Add(this.checkBoxOpenAfterCreated);
         this.Controls.Add(this.checkBoxFittingInfo);
         this.Controls.Add(this.checkBoxSegmentInfo);
         this.Controls.Add(this.checkBoxCriticalPath);
         this.Controls.Add(this.checkBoxDisplaySystemInfo);
         this.Controls.Add(this.buttonRemove);
         this.Controls.Add(this.buttonAdd);
         this.Controls.Add(this.labelReportFields);
         this.Controls.Add(this.listBoxAvailableFields);
         this.Controls.Add(this.labelAvailableFields);
         this.Controls.Add(this.listBoxReportFields);
         this.Controls.Add(this.buttonFittingSettings);
         this.Controls.Add(this.buttonSegmentSettings);
         this.Controls.Add(this.buttonUp);
         this.Controls.Add(this.buttonDown);
         this.Controls.Add(this.buttonFormatDelete);
         this.Controls.Add(this.comboBoxFormat);
         this.Controls.Add(this.labelFormat);
         this.Controls.Add(this.groupBox1);
         this.HelpButton = true;
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "WholeReportSettingsDlg";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.Activated += new System.EventHandler(this.WholeReportSettingsDlg_Activated);
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WholeReportSettingsDlg_KeyUp);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label labelFormat;
      private System.Windows.Forms.ComboBox comboBoxFormat;
      private System.Windows.Forms.Button buttonFormatDelete;
      private System.Windows.Forms.Label labelAvailableFields;
      private System.Windows.Forms.ListBox listBoxAvailableFields;
      private System.Windows.Forms.Label labelReportFields;
      private System.Windows.Forms.Button buttonAdd;
      private System.Windows.Forms.Button buttonRemove;
      private System.Windows.Forms.CheckBox checkBoxDisplaySystemInfo;
      private System.Windows.Forms.CheckBox checkBoxCriticalPath;
      private System.Windows.Forms.CheckBox checkBoxSegmentInfo;
      private System.Windows.Forms.CheckBox checkBoxFittingInfo;
      private System.Windows.Forms.Button buttonGenerate;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonSaveFormat;
      private System.Windows.Forms.CheckBox checkBoxOpenAfterCreated;
      private System.Windows.Forms.Button buttonDown;
      private System.Windows.Forms.Button buttonUp;
      private System.Windows.Forms.Button buttonSegmentSettings;
      private System.Windows.Forms.Button buttonFittingSettings;
      private System.Windows.Forms.ListBox listBoxReportFields;
      private System.Windows.Forms.GroupBox groupBox1;
   }
}