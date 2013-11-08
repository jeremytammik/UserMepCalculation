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
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using System.Windows.Forms;

namespace UserPressureLossReport
{
   [Transaction(TransactionMode.Manual)]
   [Regeneration(RegenerationOption.Manual)]
   [Journaling(JournalingMode.NoCommandData)]
   class UserPressureLossReportApplication: IExternalApplication
   {
      private static int getCalculationElemSet(Document doc, string strDomain, ElementSet caculationOnElems, ElementSet elems)
      {
         if (doc == null)
            return 0;
         int nTotalCount = 0;
         foreach (Element elem in elems)
         {
            MEPSystem mepSys = elem as MEPSystem;
            if (mepSys == null)
               continue;
            Category category = mepSys.Category;
            BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;

            if ( (strDomain == ReportResource.pipeDomain && enumCategory == BuiltInCategory.OST_PipingSystem) ||
                (strDomain == ReportResource.ductDomain && enumCategory == BuiltInCategory.OST_DuctSystem))
            {
               ++nTotalCount;
               MEPSystemType sysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;
               if (sysType != null && sysType.CalculationLevel == SystemCalculationLevel.All)
               {
                  caculationOnElems.Insert(mepSys);
               }
            }
         }
         return nTotalCount;
      }

      public static TaskDialogResult popupWarning(string instruction, TaskDialogCommonButtons commonButton, TaskDialogResult defButton)
      {
         TaskDialog tdlg = new TaskDialog(ReportResource.plrSettings);
         tdlg.MainInstruction = instruction;
         tdlg.AllowCancellation = true;
         tdlg.CommonButtons = commonButton;
         tdlg.DefaultButton = defButton;
         tdlg.TitleAutoPrefix = false;
         return tdlg.Show();
      }

      public static void beginCommand(Document doc, string strDomain, bool bForAllSystems = false, bool bFitlerUnCalculationSystems = false)
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         UIDocument uiDocument = new UIDocument(doc);
         if (bFitlerUnCalculationSystems)
         {
            ElementSet calculationOnElems = new ElementSet();
            int nTotalCount = getCalculationElemSet(doc, strDomain, calculationOnElems, uiDocument.Selection.Elements);

            if (calculationOnElems.Size == 0)
            {//No item can be calculated
               popupWarning(ReportResource.allItemsCaculationOff, TaskDialogCommonButtons.Close, TaskDialogResult.Close);
               return;
            }
            else if (calculationOnElems.Size < nTotalCount)
            {//Part of them can be calculated
               if (popupWarning(ReportResource.partOfItemsCaculationOff, TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes, TaskDialogResult.Yes) == TaskDialogResult.No)
                  return;
            }

            helper.initialize(doc, calculationOnElems, strDomain);
            invokeCommand(doc, helper, bForAllSystems);
         }
         else
         {
            helper.initialize(doc, uiDocument.Selection.Elements, strDomain);
            invokeCommand(doc, helper, bForAllSystems);
         }


      }

      public static void beginCommand(Document doc, ElementSet elems)
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         helper.initialize(doc, elems, helper.Domain);
         invokeCommand(doc, helper, false);
      }

      private static void invokeCommand(Document doc, PressureLossReportHelper helper, bool bForAllSystems)
      {
         //upgrade the formats
         ReportFormatUpgrades.Instance.executeUpgrades();

         //post warning if some systems' calculation is not ALL
         ElementSet selSystems = new ElementSet();
         if (!bForAllSystems && helper.getSelectedSystems(selSystems))
         {
            WholeReportSettingsDlg settingsDlg = new WholeReportSettingsDlg();
            settingsDlg.ShowDialog();
         }
         else //post system filter
         {
            ReportSystemSelectorDlg rssDlg = new ReportSystemSelectorDlg();
            if (rssDlg.ShowDialog() == DialogResult.OK)
            {
               UserPressureLossReportApplication.beginCommand(PressureLossReportHelper.instance.Doc, rssDlg.CheckedElements);
            }
         }
      }

      public Result OnStartup(UIControlledApplication uiControlledApplication)
      {         
         try
         {
            //binding commands
            RevitCommandId commandId = RevitCommandId.LookupCommandId("ID_MEP_DUCT_PRESSURE_LOSS_REPORT");
            if (commandId != null)
            {


               AddInCommandBinding pressureLossCommand = uiControlledApplication.CreateAddInCommandBinding(commandId);
               if (pressureLossCommand != null)
               {
                  pressureLossCommand.CanExecute += new EventHandler<Autodesk.Revit.UI.Events.CanExecuteEventArgs>(CanDuctExecute);
                  pressureLossCommand.Executed += new EventHandler<Autodesk.Revit.UI.Events.ExecutedEventArgs>(DuctExecute);
               }
            }

            commandId = RevitCommandId.LookupCommandId("ID_MEP_PIPE_PRESSURE_LOSS_REPORT");
            if (commandId != null)
            {

              
               AddInCommandBinding pressureLossCommand = uiControlledApplication.CreateAddInCommandBinding(commandId);
               if (pressureLossCommand != null)
               {
                  pressureLossCommand.CanExecute += new EventHandler<Autodesk.Revit.UI.Events.CanExecuteEventArgs>(CanPipeExecute);
                  pressureLossCommand.Executed += new EventHandler<Autodesk.Revit.UI.Events.ExecutedEventArgs>(PipeExecute);
               }
            }
       

            commandId = RevitCommandId.LookupCommandId("ID_MEP_SELECT_DUCT_PRESSURE_LOSS_REPORT");
            if (commandId != null)
            {



               AddInCommandBinding pressureLossCommand = uiControlledApplication.CreateAddInCommandBinding(commandId);
               if (pressureLossCommand != null)
               {
                  pressureLossCommand.CanExecute += new EventHandler<Autodesk.Revit.UI.Events.CanExecuteEventArgs>(CanDuctSelectExecute);
                  pressureLossCommand.Executed += new EventHandler<Autodesk.Revit.UI.Events.ExecutedEventArgs>(DuctSelectExecute);
               }
            }

            commandId = RevitCommandId.LookupCommandId("ID_MEP_SELECT_PIPE_PRESSURE_LOSS_REPORT");
            if (commandId != null)
            {


               AddInCommandBinding pressureLossCommand = uiControlledApplication.CreateAddInCommandBinding(commandId);
               if (pressureLossCommand != null)
               {
                  pressureLossCommand.CanExecute += new EventHandler<Autodesk.Revit.UI.Events.CanExecuteEventArgs>(CanPipeSelectExecute);
                  pressureLossCommand.Executed += new EventHandler<Autodesk.Revit.UI.Events.ExecutedEventArgs>(PipeSelectExecute);
               }
            }            

            return Result.Succeeded;
         }
         catch
         {
            return Result.Failed;
         }
          
      }

      public Result OnShutdown(UIControlledApplication uiControlledApplication)
      {
         return Result.Succeeded;
      }

      private void CanDuctExecute(object obj, Autodesk.Revit.UI.Events.CanExecuteEventArgs avgs)
      {         
         if (avgs.ActiveDocument == null || avgs.ActiveDocument.Application.IsMechanicalAnalysisEnabled == false)
            avgs.CanExecute = false;
         else
            avgs.CanExecute = true;
      }

      private void DuctExecute(object obj, Autodesk.Revit.UI.Events.ExecutedEventArgs avgs)
      {
         try
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            if (uiDocument == null)
               return;

            beginCommand(uiDocument.Document, ReportResource.ductDomain, true);
         }
         catch
         {
         }
      }

      private void CanDuctSelectExecute(object obj, Autodesk.Revit.UI.Events.CanExecuteEventArgs avgs)
      {
         if (avgs.ActiveDocument == null || avgs.ActiveDocument.Application.IsMechanicalAnalysisEnabled == false)
            avgs.CanExecute = false;
         else
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            Autodesk.Revit.UI.Selection.Selection selection = uiDocument.Selection;
            avgs.CanExecute = PressureLossReportHelper.instance.hasValidSystem(avgs.ActiveDocument, selection.Elements, ReportResource.ductDomain);
         }
      }

      private void DuctSelectExecute(object obj, Autodesk.Revit.UI.Events.ExecutedEventArgs avgs)
      {
         try
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            if (uiDocument == null)
               return;

            beginCommand(uiDocument.Document, ReportResource.ductDomain, false, true);
         }
         catch
         {
         }
      }


      private void CanPipeExecute(object obj, Autodesk.Revit.UI.Events.CanExecuteEventArgs avgs)
      {
         if (avgs.ActiveDocument == null || avgs.ActiveDocument.Application.IsPipingAnalysisEnabled == false)
            avgs.CanExecute = false;
         else
            avgs.CanExecute = true;
      }

      private void PipeExecute(object obj, Autodesk.Revit.UI.Events.ExecutedEventArgs avgs)
      {
         try
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            if (uiDocument == null)
               return;

            beginCommand(uiDocument.Document, ReportResource.pipeDomain, true);
         }
         catch
         {
         }
      }


      private void CanPipeSelectExecute(object obj, Autodesk.Revit.UI.Events.CanExecuteEventArgs avgs)
      {
         if (avgs.ActiveDocument == null || avgs.ActiveDocument.Application.IsPipingAnalysisEnabled == false)
            avgs.CanExecute = false;
         else
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            Autodesk.Revit.UI.Selection.Selection selection = uiDocument.Selection;
            avgs.CanExecute = PressureLossReportHelper.instance.hasValidSystem(avgs.ActiveDocument, selection.Elements, ReportResource.pipeDomain);
         }
      }

      private void PipeSelectExecute(object obj, Autodesk.Revit.UI.Events.ExecutedEventArgs avgs)
      {
         try
         {
            UIDocument uiDocument = new UIDocument(avgs.ActiveDocument);
            if (uiDocument == null)
               return;

            beginCommand(uiDocument.Document, ReportResource.pipeDomain, false, true);
         }
         catch
         {
         }
      }
   }
}
