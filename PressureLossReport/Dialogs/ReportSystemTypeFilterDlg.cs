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

namespace UserPressureLossReport
{
   public partial class ReportSystemTypeFilterDlg : Form
   {
      public List<Autodesk.Revit.DB.MEPSystemType> checkedValidSystemsType = null;
      public List<Autodesk.Revit.DB.MEPSystemType> allValidSystemsType = null;

      public ReportSystemTypeFilterDlg(List<Autodesk.Revit.DB.MEPSystemType> allSysType, List<Autodesk.Revit.DB.MEPSystemType> checkedSysType)
      {
         allValidSystemsType = allSysType;
         checkedValidSystemsType = checkedSysType;

         InitializeComponent();

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         //title
         if (helper.Domain == ReportResource.pipeDomain)
            this.Text = ReportResource.pipeSystemTypeFilterDlgTitle;
         else
            this.Text = ReportResource.ductSystemTypeFilterDlgTitle;

         FillData();
      }

      private void FillData()
      {
         addItemsToCheckList(allValidSystemsType, checkedValidSystemsType);
      }

      private void addItemsToCheckList(List<Autodesk.Revit.DB.MEPSystemType> allSysType, List<Autodesk.Revit.DB.MEPSystemType> checkedSysType)
      {
         foreach (Autodesk.Revit.DB.MEPSystemType sysType in allSysType)
         {
            if (PressureLossReportHelper.isSystemTypeInList(checkedSysType, sysType))
               SystemTypeCheckList.Items.Add(sysType.Name, true);
            else
               SystemTypeCheckList.Items.Add(sysType.Name, false);
         }
      }

      private void btnSelectAll_Click(object sender, EventArgs e)
      {
         setAllItemsStatus(true);
      }

      private void btnNone_Click(object sender, EventArgs e)
      {
         setAllItemsStatus(false);
      }

      private void btnInvert_Click(object sender, EventArgs e)
      {
         for (int ii = 0; ii < SystemTypeCheckList.Items.Count; ++ii)
         {
            SystemTypeCheckList.SetItemChecked(ii, !SystemTypeCheckList.GetItemChecked(ii));
         }
      }

      private void setAllItemsStatus(bool bStatus)
      {
         for (int ii = 0; ii < SystemTypeCheckList.Items.Count; ++ii)
         {
            SystemTypeCheckList.SetItemChecked(ii, bStatus);
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         checkedValidSystemsType.Clear();
         for (int ii = 0; ii < SystemTypeCheckList.Items.Count; ++ii)
         {
            if (SystemTypeCheckList.GetItemChecked(ii))
            {
               checkedValidSystemsType.Add(allValidSystemsType[ii]);
            }
         }
         DialogResult = DialogResult.OK;
      }
   }
}
