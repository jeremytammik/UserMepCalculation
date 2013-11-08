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
using Autodesk.Revit.DB;
using System.Diagnostics;

namespace UserPressureLossReport
{
   public partial class ReportSystemSelectorDlg : System.Windows.Forms.Form
   {
      private List<Autodesk.Revit.DB.MEPSystemType> checkedSystemType = new List<Autodesk.Revit.DB.MEPSystemType>();
      private List<Autodesk.Revit.DB.MEPSystemType> allValidSystemType = new List<Autodesk.Revit.DB.MEPSystemType>();
      private List<Autodesk.Revit.DB.BuiltInCategory> filterCategories = new List<Autodesk.Revit.DB.BuiltInCategory>();
      private List<Autodesk.Revit.DB.MEPSystem> allElementItems = new List<Autodesk.Revit.DB.MEPSystem>();

      private Autodesk.Revit.DB.ElementSet m_checkedElements = new Autodesk.Revit.DB.ElementSet();
      public Autodesk.Revit.DB.ElementSet CheckedElements
      {
         get { return m_checkedElements; }
         set { m_checkedElements = value; }
      }

      public ReportSystemSelectorDlg()
      {
         InitializeComponent();
         FillData();
      }

      private void FillData()
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         if (helper.Domain == ReportResource.ductDomain)
         {
            filterCategories.Add(Autodesk.Revit.DB.BuiltInCategory.OST_DuctSystem);
            this.Text = ReportResource.ductSystemSelectorDlgTitle;
         }
         else if (helper.Domain == ReportResource.pipeDomain)
         {
            filterCategories.Add(Autodesk.Revit.DB.BuiltInCategory.OST_PipingSystem);
            this.Text = ReportResource.pipeSystemSelectorDlgTitle;
         }

         //default add all valid system type to checked system type
         foreach (Autodesk.Revit.DB.BuiltInCategory categoryId in filterCategories)
         {
            ICollection<Autodesk.Revit.DB.Element> founds = helper.getCategoryTypeElements(categoryId);
            foreach (Autodesk.Revit.DB.MEPSystemType sysType in founds)
            {
               if (helper.isValidSystemType(sysType))
                  allValidSystemType.Add(sysType);
            }
         }

         checkedSystemType = helper.getSelectedSystemTypes();
         if (checkedSystemType == null || checkedSystemType.Count < 1)
         {
            if (checkedSystemType == null)
               checkedSystemType = new List<MEPSystemType>();
            foreach (Autodesk.Revit.DB.MEPSystemType sysType in allValidSystemType)
               checkedSystemType.Add(sysType);
         }

         //sort the system types
         SortSystemTypeByNameCamparer systemTypeCompare = new SortSystemTypeByNameCamparer();
         checkedSystemType.Sort(systemTypeCompare);
         allValidSystemType.Sort(systemTypeCompare);


         foreach (Autodesk.Revit.DB.BuiltInCategory categoryId in filterCategories)
         {
            ICollection<Autodesk.Revit.DB.Element> founds = helper.getCategoryElements(categoryId);
            addItemsToCheckList(helper, founds);
         }
      }

      private void addItemsToCheckList(PressureLossReportHelper helper, ICollection<Autodesk.Revit.DB.Element> founds)
      {
         //get the selected systems first
         ElementSet selSystems = new ElementSet();
         helper.getSelectedSystems(selSystems);

         List<string> checkedItems = new List<string>();
         foreach (Element elem in selSystems)
         {
            MEPSystem mepSys = elem as MEPSystem;
            if (mepSys == null)
               continue;

            checkedItems.Add(mepSys.Name);
         }

         List<MEPSystemType> selSystemTypes = helper.getSelectedSystemTypes(true);

         if ((selSystemTypes == null || selSystemTypes.Count < 1) && (checkedItems == null || checkedItems.Count < 1))
         {
            bool bCheckAll = true;
            if (helper.isSelectInValidSystemType())
               bCheckAll = false;

            addItemsToCheckList(helper, founds, null, bCheckAll);
         }
         else
         {
            foreach (Autodesk.Revit.DB.Element elem in founds)
            {
               Autodesk.Revit.DB.MEPSystem mepSysElem = elem as Autodesk.Revit.DB.MEPSystem;
               Autodesk.Revit.DB.MEPSystemType mepSysType = helper.getSystemType(helper.Doc, mepSysElem);

               if (isCheckedSystemType(mepSysType) && helper.isValidSystem(helper.Doc, mepSysElem, helper.Domain) && isCalculationOn(helper.Doc, mepSysElem))
               {
                  if (selSystemTypes != null && PressureLossReportHelper.isSystemTypeInList(selSystemTypes, mepSysType))
                     checkedItems.Add(mepSysElem.Name);
               }
            }

            addItemsToCheckList(helper, founds, checkedItems);
         }

      }

      private void addItemsToCheckList(PressureLossReportHelper helper, ICollection<Autodesk.Revit.DB.Element> founds, List<string> checkedItems, bool bCheckAll = false)
      {
         SortedDictionary<string, bool> systemList = new SortedDictionary<string, bool>();

         foreach (Autodesk.Revit.DB.Element elem in founds)
         {
            Autodesk.Revit.DB.MEPSystem mepSysElem = elem as Autodesk.Revit.DB.MEPSystem;
            Autodesk.Revit.DB.MEPSystemType mepSysType = helper.getSystemType(helper.Doc, mepSysElem);

            if (isCheckedSystemType(mepSysType) && helper.isValidSystem(helper.Doc, mepSysElem, helper.Domain) && isCalculationOn(helper.Doc, mepSysElem))
            {
               if (bCheckAll || (checkedItems != null && checkedItems.Contains(mepSysElem.Name)))
                  systemList.Add(mepSysElem.Name, true);                  
               else
                  systemList.Add(mepSysElem.Name, false);                   
               
               allElementItems.Add(mepSysElem);
            }
         }

         bool bDisabled = true;
         foreach (KeyValuePair<string, bool> kvp in systemList)
         {
            SystemCheckList.Items.Add(kvp.Key, kvp.Value);
            if (bDisabled && kvp.Value == true)
               bDisabled = false;
         }
         btnOK.Enabled = !bDisabled;

         SortSystemByNameCamparer systemCompare = new SortSystemByNameCamparer();
         allElementItems.Sort(systemCompare);
      }

	  private bool isCalculationOn(Document doc, Autodesk.Revit.DB.MEPSystem mepSys)
      {
         if (mepSys == null)
            return false;
         MEPSystemType sysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;
         if (sysType == null)
            return false;

         return sysType.CalculationLevel == Autodesk.Revit.DB.Mechanical.SystemCalculationLevel.All;
      }
	  
      private bool isCheckedSystemType(Autodesk.Revit.DB.MEPSystemType mepSysType)
      {
         for (int ii = 0; ii < checkedSystemType.Count; ++ii)
         {
            if (mepSysType.Id == checkedSystemType[ii].Id)
            {
               return true;
            }
         }
         return false;
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
         if (SystemCheckList.Items.Count < 1)
            return;
         bool bDisabled = true;
         for (int ii = 0; ii < SystemCheckList.Items.Count; ++ii)
         {
            SystemCheckList.SetItemChecked(ii, !SystemCheckList.GetItemChecked(ii));
            if (bDisabled && SystemCheckList.GetItemChecked(ii) == true)
               bDisabled = false;
         }
         btnOK.Enabled = !bDisabled;
      }

      private void setAllItemsStatus(bool bStatus)
      {
         if (SystemCheckList.Items.Count < 1)
            return;

         btnOK.Enabled = bStatus;
         for (int ii = 0; ii < SystemCheckList.Items.Count; ++ii)
         {
            SystemCheckList.SetItemChecked(ii, bStatus);
         }
      }

      private void btnSystemTypeFilter_Click(object sender, EventArgs e)
      {
         ReportSystemTypeFilterDlg reportSTFilterDlg = new ReportSystemTypeFilterDlg(allValidSystemType, checkedSystemType);
         if (reportSTFilterDlg.ShowDialog() == DialogResult.OK)
         {
            List<string> checkedItems = new List<string>();
            for (int ii = 0; ii < SystemCheckList.Items.Count; ++ii)
            {
               if (SystemCheckList.GetItemChecked(ii))
               {
                  String itemName = SystemCheckList.Items[ii].ToString();
                  checkedItems.Add(itemName);
               }
            }
            SystemCheckList.Items.Clear();
            allElementItems.Clear();

            foreach (Autodesk.Revit.DB.BuiltInCategory categoryId in filterCategories)
            {
               ICollection<Autodesk.Revit.DB.Element> founds = PressureLossReportHelper.instance.getCategoryElements(categoryId);
               addItemsToCheckList(PressureLossReportHelper.instance, founds, checkedItems);
            }
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         CheckedElements.Clear();
         for (int ii = 0; ii < SystemCheckList.Items.Count; ++ii)
         {
            if (SystemCheckList.GetItemChecked(ii))
            {
               CheckedElements.Insert(allElementItems[ii]);
            }
         }
         if (CheckedElements.Size == 0)
         {
            //UIHelperFunctions.postWarning(ReportResource.invalidSystemMsg, ReportResource.selectSystemsMsg);
            this.DialogResult = DialogResult.None;
         }
         else
         {
            this.DialogResult = DialogResult.OK;
         }
      }

      private void SystemCheckList_SelectedIndexChanged(object sender, EventArgs e)
      {
         for (int i = 0; i < SystemCheckList.Items.Count; i++)
         {
            if (SystemCheckList.GetItemChecked(i))
            {
               btnOK.Enabled = true;
               return;
            }
         }
         btnOK.Enabled = false;
      }
   }
}
