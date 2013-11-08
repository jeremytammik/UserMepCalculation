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
   public partial class ReportFormatNameDlg : Form
   {
      private string reportFormatName = "";
      public ReportFormatNameDlg()
      {
         InitializeComponent();
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;
      }

      private void Btn_Yes_Click(object sender, EventArgs e)
      {
         reportFormatName = textBox1.Text;
         //check if the name is valid
         if (reportFormatName.Length < 1)
         {
            UIHelperFunctions.postWarning(ReportResource.plrSettings, ReportResource.formatNameMsg); 
         }
         else
         {
            //check if the name exists
            PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
            PressureLossReportData reportData = reportDataMgr.getData(reportFormatName);
            if (reportData != null) //post task dialog
            {
               TaskDialog tdlg = new TaskDialog(ReportResource.plrSettings);
               tdlg.MainInstruction = ReportResource.formatNameDuplicateMsg;
               tdlg.AllowCancellation = true;
               tdlg.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
               tdlg.DefaultButton = TaskDialogResult.No;
               tdlg.TitleAutoPrefix = false; // suppress the prefix of title.
               if (tdlg.Show() != TaskDialogResult.Yes)
               {
                  textBox1.Focus();
                  return;
               }
            }

            DialogResult = DialogResult.OK;
         }         
      }

      public string ReportFormatName
      {
         get { return reportFormatName; }
         set { reportFormatName = value; }
      }

      private void ReportFormatNameDlg_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.KeyData == Keys.Escape)
            this.Close();
      }
   }
}
