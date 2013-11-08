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
using Autodesk.Revit.DB;

namespace UserPressureLossReport
{
   //need to execute upgrade if the following changes happened:
   //1. Add/Remove/Change fields of section/segment/fitting
   //2. Add/Remove/Change properties of PressureLossReportData

   public sealed class ReportFormatUpgrades
   {
      public static readonly ReportFormatUpgrades Instance = new ReportFormatUpgrades();

      public void executeUpgrades()
      {
         try
         {
            //get format manager
            PressureLossReportDataManager reportDataMgr = PressureLossReportDataManager.Instance;
            PressureLossReportFormats formats = reportDataMgr.getAllFormats();
            if (formats != null && formats.Count > 0)
            {
               foreach (PressureLossReportData data in formats)
               {
                  //get format version
                  upgrades(data);
               }               
            }
         }
         catch
         {
         	
         }
      }

      public void upgrades(PressureLossReportData data)
      {
         upgrade1(data);
         upgrade2(data);
         upgrade3(data); // update old loss method and remove coefficient from duct fitting
         upgrade4(data); // update old loss method and remove coefficient from pipe fitting
         //add any upgrade here
      }

      public void upgrade1(PressureLossReportData data, int nVersion = 1) //add "Element ID" fields to segment and fitting fields
      {
         if (data == null || data.Version >= nVersion)
            return;

         //segment
         {
            PressureLossParameter PLParam = new PressureLossParameter(ReportResource.elementId, false, -1, (int)SectionMemberType.Segment);
            if (!data.StraightSegFields.Contains(PLParam))
               data.StraightSegFields.Add(PLParam);
         }

         //fitting
         {
            PressureLossParameter PLParam = new PressureLossParameter(ReportResource.elementId, false, -1, (int)SectionMemberType.Fitting);
            if (!data.FittingFields.Contains(PLParam))
               data.FittingFields.Add(PLParam);
         }

         data.Version = nVersion;
         PressureLossReportDataManager.Instance.save(data);
      }

      /************************************************************************/
      /* use "K Coefficient" for pipe and "Loss Coefficient" for duct.
       * pressure drop to pressure loss.
       * Element ID and Mark.
       */
      /************************************************************************/
      public void upgrade2(PressureLossReportData data, int nVersion = 2)
      {
         if (data == null || data.Version >= nVersion)
            return;

         string fieldName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT);
         string newName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM);

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         //total pressure loss table
         {
            if (data.Domain == ReportResource.pipeDomain)
            {
               PressureLossParameter PLParam = helper.getPressureLossParamByName(data.AvailableFields, fieldName);
               if (PLParam != null)
                  PLParam.Name = newName;
            }
         }

         string pressureDrop = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PRESSURE_DROP);
         string strMark = LabelUtils.GetLabelFor(BuiltInParameter.ALL_MODEL_MARK);
         //segment table
         {
            //use "K Coefficient" for pipe and "Loss Coefficient" for duct
            PressureLossParameter PLParam = null;
            if (data.Domain == ReportResource.pipeDomain)
            {
               PLParam = helper.getPressureLossParamByName(data.StraightSegFields, fieldName);
               if (PLParam != null)
                  PLParam.Name = newName;
            }

            //pressure drop to pressure loss
            PLParam = helper.getPressureLossParamByName(data.StraightSegFields, pressureDrop);
            if (PLParam != null)
               PLParam.Name = ReportResource.pressureLoss;

            //Element ID and Mark
            PLParam = helper.getPressureLossParamByName(data.StraightSegFields, strMark);
            int nDisplayOrder = -1;
            if (PLParam != null)
            {
               nDisplayOrder = PLParam.DisplayOrder;
               PLParam.Selected = false;
               PLParam.DisplayOrder = -1;

               PLParam = helper.getPressureLossParamByName(data.StraightSegFields, ReportResource.elementId);
               if (PLParam != null)
               {
                  PLParam.Selected = true;
                  PLParam.DisplayOrder = nDisplayOrder;
               }
            }
         }

         //fitting table
         {
            //use "K Coefficient" for pipe and "Loss Coefficient" for duct
            PressureLossParameter PLParam = null;
            if (data.Domain == ReportResource.pipeDomain)
            {
               PressureLossParameter PLParam1 = helper.getPressureLossParamByName(data.FittingFields, newName);
               if (PLParam1 != null)
                  data.FittingFields.Remove(PLParam1);

               PLParam = helper.getPressureLossParamByName(data.FittingFields, fieldName);
               if (PLParam != null)
               {
                  PLParam.Name = newName;
               }

            }

            //Pressure Drop to Pressure Loss
            PLParam = helper.getPressureLossParamByName(data.FittingFields, pressureDrop);
            if (PLParam != null)
               PLParam.Name = ReportResource.pressureLoss;

            //Element ID and Mark
            PLParam = helper.getPressureLossParamByName(data.FittingFields, strMark);
            int nDisplayOrder = -1;
            if (PLParam != null)
            {
               nDisplayOrder = PLParam.DisplayOrder;
               PLParam.Selected = false;
               PLParam.DisplayOrder = -1;

               PLParam = helper.getPressureLossParamByName(data.FittingFields, ReportResource.elementId);
               if (PLParam != null)
               {
                  PLParam.Selected = true;
                  PLParam.DisplayOrder = nDisplayOrder;
               }
            }
         }

         data.Version = nVersion;
         PressureLossReportDataManager.Instance.save(data);
      }


      /************************************************************************/
      /* For duct fitting and accessory, remove coefficient
       * 
       * 
       */
      /************************************************************************/
      public void upgrade3(PressureLossReportData data, int nVersion = 3)
      {
         if (data == null || data.Version >= nVersion)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         string fieldLossMethodName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FITTING_LOSS_METHOD_PARAM);
         string fieldCoefficientName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT);
         string fieldAshareTableName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FITTING_LOSS_TABLE_PARAM);

         string fieldNewLossMethodName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FITTING_LOSS_METHOD_SERVER_PARAM);

         int nDisplayOrder = -1;
         if (data.Domain == ReportResource.ductDomain) //Remove the 3 old parameters for duct
         {
            PressureLossParameter PLParam1 = helper.getPressureLossParamByName(data.FittingFields, fieldLossMethodName);
            if (PLParam1 != null)
            {
               nDisplayOrder = PLParam1.DisplayOrder;
               data.FittingFields.Remove(PLParam1);
            }

            PressureLossParameter PLParam2 = helper.getPressureLossParamByName(data.FittingFields, fieldCoefficientName);
            if (PLParam2 != null)
               data.FittingFields.Remove(PLParam2);

            PressureLossParameter PLParam3 = helper.getPressureLossParamByName(data.FittingFields, fieldAshareTableName);
            if (PLParam3 != null)
               data.FittingFields.Remove(PLParam3);

            //Add the new loss method as selected field for duct
            PressureLossParameter PLParam = new PressureLossParameter(fieldNewLossMethodName, true, nDisplayOrder, (int)SectionMemberType.Fitting);
            if (!data.FittingFields.Contains(PLParam))
               data.FittingFields.Add(PLParam);
         }

         data.Version = nVersion;
         PressureLossReportDataManager.Instance.save(data);
      }


      /************************************************************************/
      /* For pipe fitting and accessory, remove coefficient
       * 
       * 
       */
      /************************************************************************/
      public void upgrade4(PressureLossReportData data, int nVersion = 4)
      {
         if (data == null || data.Version >= nVersion)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         string fieldLossMethodName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_METHOD_PARAM);
         string fieldKFactorName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM);
         string fieldKTableName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_TABLE_PARAM);

         string fieldNewLossMethodName = LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_METHOD_SERVER_PARAM);

         int nDisplayOrder = -1;
         if (data.Domain == ReportResource.pipeDomain) //Remove the 3 old parameters for pipe
         {
            PressureLossParameter PLParam1 = helper.getPressureLossParamByName(data.FittingFields, fieldLossMethodName);
            if (PLParam1 != null)
            {
               nDisplayOrder = PLParam1.DisplayOrder;
               data.FittingFields.Remove(PLParam1);
            }

            PressureLossParameter PLParam2 = helper.getPressureLossParamByName(data.FittingFields, fieldKFactorName);
            if (PLParam2 != null)
               data.FittingFields.Remove(PLParam2);

            PressureLossParameter PLParam3 = helper.getPressureLossParamByName(data.FittingFields, fieldKTableName);
            if (PLParam3 != null)
               data.FittingFields.Remove(PLParam3);

            //Add the new loss method as selected field for pipe
            PressureLossParameter PLParam = new PressureLossParameter(fieldNewLossMethodName, true, nDisplayOrder, (int)SectionMemberType.Fitting);
            if (!data.FittingFields.Contains(PLParam))
               data.FittingFields.Add(PLParam);
         }

         data.Version = nVersion;
         PressureLossReportDataManager.Instance.save(data);
      }
   }
}
