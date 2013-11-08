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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace UserPressureLossReport
{
   public enum PartialReportSettingsDlgType
   {
      Segment = 0,
      Fitting
   }

   public partial class PartialReportSettingsDlg : Form
   {
      private PartialReportSettingsDlgType eType = PartialReportSettingsDlgType.Segment;
      private PressureLossReportData reportData = null;

      public PartialReportSettingsDlg()
      {
         InitializeComponent();
         this.buttonUp.Enabled = false;
         this.buttonDown.Enabled = false;
      }

      public void initializeData(PartialReportSettingsDlgType eInputType, PressureLossReportData inputReportData)
      {
         eType = eInputType;
         reportData = inputReportData;
         fillingFields(eType);
      }

      private void buttonOK_Click(object sender, EventArgs e)
      {
         if (reportData == null)
            return;

         List<PressureLossParameter> avaliableParams = null;
         if (eType == PartialReportSettingsDlgType.Segment)
            avaliableParams = reportData.StraightSegFields;
         else
            avaliableParams = reportData.FittingFields;
         UIHelperFunctions.getFieldsFromSelectedListBox(avaliableParams, listBoxReportFields);

         DialogResult = DialogResult.OK;
      }

      private void fillingFields(PartialReportSettingsDlgType eType)
      {
         if (reportData == null)
            return;

         List<PressureLossParameter> avaliableParams = new List<PressureLossParameter>();
         if (eType == PartialReportSettingsDlgType.Segment)
            avaliableParams = reportData.StraightSegFields;
         else
            avaliableParams = reportData.FittingFields;
         UIHelperFunctions.fillingListBoxFields(avaliableParams, listBoxAvailableFields, listBoxReportFields);

         listBoxAvailableFields.Focus();
         if (listBoxAvailableFields.Items.Count > 0)
            listBoxAvailableFields.SetSelected(0, true);
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

      private void listBoxAvailableFields_DoubleClick(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxAvailableFields, listBoxReportFields);
      }

      private void listBoxReportFields_DoubleClick(object sender, EventArgs e)
      {
         UIHelperFunctions.addRemoveFields(listBoxReportFields, listBoxAvailableFields);
      }

      private void listBoxReportFields_SelectedIndexChanged(object sender, EventArgs e)
      {
         UIHelperFunctions.updateUpDownButtonEnable(listBoxReportFields, buttonUp, buttonDown);
      }

      private void PartialReportSettingsDlg_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.KeyData == Keys.Escape)
            this.Close();
      }
   }
}
