//
// (C) Copyright 2003-2012 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace UserPressureLossReport
{
   public partial class WholeReportSettingsDlg : System.Windows.Forms.Form
   {
      private DataGenerator generator = null;
      private PressureLossReportData settings = null;

      public WholeReportSettingsDlg()
      {
         InitializeComponent();
         FillingData();
         if (!hasFittingsInSystem())
         {
            buttonFittingSettings.Enabled = false;
         }
         else if (buttonFittingSettings.Enabled == false)
         {
            buttonFittingSettings.Enabled = true;
         }
      }

      private void FillingData()
      {
         try
         {
            //Way 1:
            //1. Fill in the combo: report format names or "untitled format"
            //?: Which i=one is the default selected?
            PressureLossReportHelper helper = PressureLossReportHelper.instance;
            PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
            PressureLossReportFormats formats = reportDataMgr.getAllFormats();
            if (formats != null && formats.Count > 0)
            {
               PressureLossReportData lastData = reportDataMgr.getLastUsedReportData();
               string lastUsedName = "";
               foreach (PressureLossReportData data in formats)
               {
                  if (lastData != null && lastData.Name == data.Name)
                  {
                     lastUsedName = reportDataMgr.getLastUsedReportName();
                     if (formats.Count == 1)
                        this.comboBoxFormat.Items.Add(lastUsedName);
                  }
                  else
                     this.comboBoxFormat.Items.Add(data.Name);
               }

               if (lastUsedName.Length > 0 && this.comboBoxFormat.Items.IndexOf(lastUsedName) > -1)
                  this.comboBoxFormat.SelectedIndex = this.comboBoxFormat.Items.IndexOf(lastUsedName);
               else
                  this.comboBoxFormat.SelectedIndex = 0;
               settings = reportDataMgr.getData(this.comboBoxFormat.SelectedItem.ToString());

               resetSettings();
            }


            if (settings == null) //fill in default values
            {               
               settings = new PressureLossReportData();

               generateDefaultFormatData();
               settings.Name = ReportResource.formatDefaultName;
               reportDataMgr.save(settings);
               this.comboBoxFormat.Items.Add(settings.Name);
               this.comboBoxFormat.SelectedIndex = 0;
            }

            if (settings != null)
               fillSettingsControlsFromFormat(settings);

            this.buttonUp.Enabled = false;
            this.buttonDown.Enabled = false;

            this.listBoxAvailableFields.Focus();
            this.listBoxAvailableFields.SetSelected(0, true);

            //title
            if (helper.Domain == ReportResource.pipeDomain)
               this.Text = ReportResource.pipeSettingsDlgTitle;
            else
               this.Text = ReportResource.ductSettingsDlgTitle;
         }
         catch
         {
         	//do nothing
         }
      }

      private bool hasFittingsInSystem()
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return false;
         //find the first fitting
         List<MEPSystem> systems = helper.getSortedSystems();
         if (systems == null || systems.Count < 1)
            return false;
         foreach (MEPSystem system in systems)
         {
            List<MEPSection> sections = new List<MEPSection>();
            MEPSystemInfo.getSectionsFromSystem(system, sections);
            foreach (MEPSection section in sections)
            {
               //find one section which contains both segment and fitting
               List<FamilyInstance> fittings = new List<FamilyInstance>();

               SectionsInfo.getSectionElements(section, null, fittings, null, null);

               if (fittings.Count > 0)
                  return true;
            }
         }
         return false;
      }

      private void resetSettings()
      {
         if (settings == null)
            settings = new PressureLossReportData();

         if (settings.AvailableFields == null || settings.AvailableFields.Count < 1)
            SectionsInfo.generateSectionFields(settings);

         if (settings.StraightSegFields == null || settings.StraightSegFields.Count < 1)
            SegmentsInfo.generateSegmentFields(settings);

         if (settings.FittingFields == null || settings.FittingFields.Count < 1)
            FittingsInfo.generateFittingFields(settings);
      }

      private void doWork(object fileName)
      {
         try
         {
            System.Diagnostics.Process.Start(fileName.ToString());
         }
         catch (System.Exception ex)
         {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            UIHelperFunctions.ShowMsgWarning(ReportResource.CanotOpen, ReportResource.warningTitle);
         }
         
      }
      private void OnGenerateReport(object sender, EventArgs e)
      {
         SaveFileDialog saveFileDlg = new SaveFileDialog();

         saveFileDlg.Filter = ReportResource.webPageFileFormat +"|*.html|" + ReportResource.csvFileFormat + "|*.csv";
         saveFileDlg.FilterIndex = 1;
         saveFileDlg.RestoreDirectory = true;

         if (saveFileDlg.ShowDialog() == DialogResult.OK)
         {
            string fileName = saveFileDlg.FileName;
            if (!string.IsNullOrEmpty(fileName))
            {
               readSettingsDataFromDlg();
               
               // Save report data to a file.
               if (generator == null)
                  generator = new DataGenerator();
               SaveType eSaveType = (SaveType)(saveFileDlg.FilterIndex - 1);
               if (generator.SaveReportToFile(fileName, settings, eSaveType))
               {
                  this.Close();
                  //save last used report data              
                  PressureLossReportDataManager.Instance.saveLastUseReport(settings);
                  if (checkBoxOpenAfterCreated.Checked)
                  {
                     Thread paramThread = new Thread(new ParameterizedThreadStart(doWork));
                     paramThread.Start(fileName);
                  }
               }
            }            
         }
      }

      private void generateDefaultFormatData()
      {
         if (settings == null)
            settings = new PressureLossReportData();

         PressureLossReportHelper helper = PressureLossReportHelper.instance;

         settings.Domain = helper.Domain;
         settings.DisplayCriticalPath = true;
         settings.DisplayDetailInfoForStraightSeg = true;
         settings.DisplayFittingLCSum = true;
         settings.DisplaySysInfo = true;
         settings.OpenAfterCreated = true;

         //initialize the fields
         helper.generateAviliableFields(settings);
      }

      private void readSettingsDataFromDlg()
      {
         //current settings 
         if (settings == null)
            settings = new PressureLossReportData();
         settings.DisplayCriticalPath = checkBoxCriticalPath.Checked;
         settings.DisplayDetailInfoForStraightSeg = checkBoxSegmentInfo.Checked;
         settings.DisplayFittingLCSum = checkBoxFittingInfo.Checked;
         settings.DisplaySysInfo = checkBoxDisplaySystemInfo.Checked;
         settings.OpenAfterCreated = checkBoxOpenAfterCreated.Checked;

         UIHelperFunctions.getFieldsFromSelectedListBox(settings.AvailableFields, listBoxReportFields);
      }

      private void fillSettingsControlsFromFormat(PressureLossReportData data)
      {
         if (data == null)
            return;

         checkBoxDisplaySystemInfo.Checked = data.DisplaySysInfo;
         checkBoxCriticalPath.Checked = data.DisplayCriticalPath;
         checkBoxFittingInfo.Checked = data.DisplayFittingLCSum;
         checkBoxSegmentInfo.Checked = data.DisplayDetailInfoForStraightSeg;
         checkBoxOpenAfterCreated.Checked = data.OpenAfterCreated;

         listBoxAvailableFields.Items.Clear();
         listBoxReportFields.Items.Clear();

         //fill in the fields
         UIHelperFunctions.fillingListBoxFields(data.AvailableFields, listBoxAvailableFields, listBoxReportFields);
      }

      private void OnChangeSegmentReportSettings(object sender, EventArgs e)
      {
         if (settings == null)
            generateDefaultFormatData();

         PartialReportSettingsDlg segmentReportSettingsDlg = new PartialReportSettingsDlg();
         segmentReportSettingsDlg.initializeData(PartialReportSettingsDlgType.Segment, settings);

         segmentReportSettingsDlg.Text = ReportResource.segmentSettings;
         if (segmentReportSettingsDlg.ShowDialog() == DialogResult.OK)
         {
            segmentReportSettingsDlg.Close();
         }
      }

      private void OnChangeFittingReportSettings(object sender, EventArgs e)
      {
         if (settings == null)
            generateDefaultFormatData();

         PartialReportSettingsDlg fittingReportSettingsDlg = new PartialReportSettingsDlg();
         fittingReportSettingsDlg.initializeData(PartialReportSettingsDlgType.Fitting, settings);

         fittingReportSettingsDlg.Text = ReportResource.fittingSettings;
         if (fittingReportSettingsDlg.ShowDialog() == DialogResult.OK)
         {
            fittingReportSettingsDlg.Close();
         }
      }

      private void buttonSaveFormat_Click(object sender, EventArgs e)
      {
         ReportFormatNameDlg reportFormatDlg = new ReportFormatNameDlg();
         if (reportFormatDlg.ShowDialog() == DialogResult.OK)
         {
            readSettingsDataFromDlg();

            // get report name, and save the report format
            settings.Name = reportFormatDlg.ReportFormatName;
            PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
            reportDataMgr.save(settings);
            reportFormatDlg.Close();

            if (!this.comboBoxFormat.Items.Contains(settings.Name))
            {
               int nIndex = this.comboBoxFormat.Items.Add(settings.Name);
               this.comboBoxFormat.SelectedIndex = nIndex;
            }
         }
      }

      private void buttonFormatDelete_Click(object sender, EventArgs e)
      {
         PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
         PressureLossReportFormats allFormats = reportDataMgr.getAllFormats();

         if (allFormats == null || allFormats.Count < 1 || this.comboBoxFormat.Items.Count < 1)
            return;

         if (allFormats.Count == 1 || (allFormats.Count == 2 && reportDataMgr.getLastUsedReportData() != null))
         {
            UIHelperFunctions.postWarning(ReportResource.deleteFormatTitle, ReportResource.deleteLastFormatMsg, ReportResource.deleteLastFormatSubMsg);
            return;
         }

         TaskDialog tdlg = new TaskDialog(ReportResource.deleteFormatTitle);
         tdlg.MainInstruction = ReportResource.deleteFormatMsg;
         tdlg.AllowCancellation = true;
         tdlg.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
         tdlg.DefaultButton = TaskDialogResult.No;
         tdlg.TitleAutoPrefix = false; // suppress the prefix of title.
         if (tdlg.Show() == TaskDialogResult.Yes)
         {
            // delete the report format, and update the combo list, set the next one as current
            string name = this.comboBoxFormat.SelectedItem.ToString();
            if (name == reportDataMgr.getLastUsedReportName())
            {
               PressureLossReportData lastData = reportDataMgr.getLastUsedReportData();
               if (lastData != null)
                  reportDataMgr.remove(lastData.Name);
            }

            reportDataMgr.remove(name);

            this.comboBoxFormat.Items.Remove(name);

            if (this.comboBoxFormat.Items.Count > 0)
               this.comboBoxFormat.SelectedIndex = 0;
         }
      }

      private void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
      {
         PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
         settings = reportDataMgr.getData(this.comboBoxFormat.SelectedItem.ToString());
         if (hasFittingsInSystem() && (settings.FittingFields == null || settings.FittingFields.Count < 1))
         {
            FittingsInfo.generateFittingFields(settings);
         }

         fillSettingsControlsFromFormat(settings);
      }

      private void buttonAdd_Click(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxAvailableFields, listBoxReportFields);
      }

      private void buttonRemove_Click(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxReportFields, listBoxAvailableFields);
      }

      private void buttonUp_Click(object sender, EventArgs e)
      {
         UIHelperFunctions.moveSelectedField(listBoxReportFields, true);
      }

      private void buttonDown_Click(object sender, EventArgs e)
      {
         UIHelperFunctions.moveSelectedField(listBoxReportFields, false);
      }

      private void listBoxReportFields_SelectedIndexChanged(object sender, EventArgs e)
      {
         UIHelperFunctions.updateUpDownButtonEnable(listBoxReportFields, buttonUp, buttonDown);
      }

      private void listBoxAvailableFields_DoubleClick(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxAvailableFields, listBoxReportFields);
      }

      private void listBoxReportFields_DoubleClick(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxReportFields, listBoxAvailableFields);
      }

      private void WholeReportSettingsDlg_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.KeyData == Keys.Escape)
            this.Close();
      }

      private void WholeReportSettingsDlg_Activated(object sender, EventArgs e)
      {
         listBoxAvailableFields.Focus();
      }      
   }
}
