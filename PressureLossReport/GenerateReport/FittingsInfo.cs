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
using System.Data;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;

namespace UserPressureLossReport
{
   class FittingsInfo : ReportInfo
   {
      private List<MEPSection> sections;

      public FittingsInfo(List<MEPSection> mepSections)
      {
         sections = mepSections;
      }

      public override void writeToHTML(HtmlStreamWriter writer)
      {
         if (writer == null || sections == null || !needToWrite())
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         //Fitting title
         List<string> strFittingFields = new List<string>();
         getFields(strFittingFields);

         writer.WriteElementString("FittingInfoTitle", ReportResource.fittingDetailInfo);
         writer.writeTableTitle("Fitting", strFittingFields);

         //Fitting info
         foreach (MEPSection section in sections)
         {
            string str = "Fitting";
            DataTable detailInfoTB = new DataTable("FittingDetailInfo");
            bool bTable = getFittingInfo(section, detailInfoTB, true);
            string totalPL = helper.getTotalPressureLossByType(section, SectionMemberType.Fitting);

            if (bTable)
               writer.writeTable(str, detailInfoTB, section.Number.ToString(), totalPL);
         }
      }

      public override void writeToCsv(CsvStreamWriter writer)
      {
         if (writer == null || sections == null || !needToWrite())
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         writer.addTitleRow(ReportResource.fittingDetailInfo);

         //fitting title
         List<string> strFittingFields = new List<string>();
         getFields(strFittingFields);
         DataTable tbTitle = new DataTable();
         helper.getTableTitle(tbTitle, strFittingFields);
         writer.AddData(tbTitle, 1);

         foreach (MEPSection section in sections)
         {            
            DataTable tb = new DataTable();
            if (getFittingInfo(section, tb))
               writer.AddData(tb, 1);            
         }
      }

      public override bool needToWrite()
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper.ReportData == null || helper.ReportData.DisplayFittingLCSum == false || helper.ReportData.FittingFields == null)
            return false;

         return true;
      }

      private bool getFittingInfo(MEPSection section, DataTable fittingTB, bool forHTML = false)
      {
         if (fittingTB == null || section == null)
            return false;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null || helper.ReportData == null)
            return false;

         List<string> fittingFields = new List<string>();
         if (helper.ReportData.FittingFields != null)
            getFields(fittingFields);

         if (forHTML)
            helper.addColumns(fittingTB, fittingFields.Count);
         else
            helper.addColumns(fittingTB, fittingFields.Count+2);

         List<FamilyInstance> fittings = new List<FamilyInstance>();
         SectionsInfo.getSectionElements(section, null, fittings, null, null);
         if (fittings.Count < 1)
            return false;

         int nIndex = 0;
         foreach (FamilyInstance fitting in fittings)
         {
            List<string> paramVals = new List<string>();
            if (!forHTML)
            {
               if (nIndex == 0)
                  paramVals.Add(section.Number.ToString());
               else
                  paramVals.Add(" ");
            }

            foreach (string fieldName in fittingFields)
            {
               try
               {
                  PressureLossParameter PLParam = helper.getPressureLossParamByName(helper.ReportData.FittingFields, fieldName);
                  if (PLParam == null)
                     continue;

                  string strVal = ReportConstants.emptyValue;
                  if ((PLParam.GetFrom & (int)SectionMemberType.Section) > 0)
                     strVal = SectionsInfo.getSectionInfoByParamName(section, fieldName, PLParam.GetFrom, fitting.Id);
                  else if ((PLParam.GetFrom & (int)SectionMemberType.Fitting) > 0)
                  {
                     if (helper.Domain == ReportResource.pipeDomain && fieldName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_METHOD_SERVER_PARAM))
                     {
                        string strValGUID = fitting.get_Parameter(fieldName).AsString();
                        Guid serverGUID = new Guid(strValGUID);

                        //convert the GUID to server name
                        //get the service first, and then get the server
                        MultiServerService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropService) as MultiServerService;
                        if (service != null && serverGUID != null)
                        {
                           IExternalServer server = service.GetServer(new Guid(strValGUID));
                           if (server != null)
                              strVal = server.GetName();
                        }
                     }
                     else if (helper.Domain == ReportResource.ductDomain && fieldName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FITTING_LOSS_METHOD_SERVER_PARAM))
                     {
                        string strValGUID = fitting.get_Parameter(fieldName).AsString();
                        Guid serverGUID = new Guid(strValGUID);

                        //convert the GUID to server name
                        //get the service first, and then get the server
                        MultiServerService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropService) as MultiServerService;
                        if (service != null && serverGUID != null)
                        {
                           IExternalServer server = service.GetServer(new Guid(strValGUID));
                           if (server != null)
                              strVal = server.GetName();
                        }
                     }
                     else if (fieldName == ReportResource.elementId)
                        strVal = fitting.Id.ToString();
                     else
                        strVal = helper.getParamValue(fitting.get_Parameter(fieldName));
                  }
                  else if ((PLParam.GetFrom & (int)SectionMemberType.Type) > 0)
                     strVal = getFittingSymbolInfoByParamName(fitting, fieldName);

                  paramVals.Add(strVal);
               }
               catch
               {
                  //...
               }
            }

            if (!forHTML) //for csv, the last column is section pressure loss report
            {
               string strVal = ReportConstants.mergeValue;
               if (nIndex == 0)
                 strVal = helper.getTotalPressureLossByType(section, SectionMemberType.Fitting);

               paramVals.Add(strVal);
            }

            nIndex++;

            helper.addRow(fittingTB, paramVals);
         }

         return true;
      }

      private string getFittingSymbolInfoByParamName(FamilyInstance fitting, string paramName)
      {
         string paramVal = ReportConstants.emptyValue;

         if (fitting == null || paramName == null)
            return paramVal;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return paramVal;

         FamilySymbol famSym = fitting.Symbol;
         if (famSym != null)
         {
            paramVal = helper.getFamilyOrTypeName(fitting.Id, paramName);
            if (paramVal == null || paramVal.Length < 1)
               paramVal = helper.getParamValue(famSym.get_Parameter(paramName));
         }

         return paramVal;
      }

      private void getFields(List<string> fileds)
      {
         if (fileds == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         if (helper.ReportData.AvailableFields != null)
            helper.getFieldsFromReportdata(helper.ReportData.FittingFields, fileds);

      }

      static public void generateFittingFields(PressureLossReportData reportData)
      {
         if (reportData == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         List<PressureLossParameter> fittingParameters = new List<PressureLossParameter>();

         //default fitting selected fields
         //SectionMemberType means where the value is from: section or fitting
         int nOrder = 1;
         fittingParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ALL_MODEL_MARK), false, -1, (int)SectionMemberType.Fitting));
         fittingParameters.Add(new PressureLossParameter(ReportResource.elementId, true, nOrder++, (int)SectionMemberType.Fitting));

         if (helper.Domain == ReportResource.pipeDomain)
            fittingParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_METHOD_SERVER_PARAM), true, nOrder++, (int)SectionMemberType.Fitting));
         else
            fittingParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FITTING_LOSS_METHOD_SERVER_PARAM), true, nOrder++, (int)SectionMemberType.Fitting));
         
         //pressure loss fields is not visible in settings dialog, the order 100 is to make sure it always is the last one field
         fittingParameters.Add(new PressureLossParameter(ReportResource.pressureLoss, true, 100, (int)SectionMemberType.Section, false));

         //find the first fitting
         List<MEPSystem> systems = helper.getSortedSystems();
         if (systems == null || systems.Count < 1)
            return;
         foreach (MEPSystem system in systems)
         {
            List<MEPSection> sections = new List<MEPSection>();
            MEPSystemInfo.getSectionsFromSystem(system, sections);
            foreach (MEPSection section in sections)
            {
               //find one section which contains both segment and fitting
               List<FamilyInstance> fittings = new List<FamilyInstance>();

               SectionsInfo.getSectionElements(section, null, fittings, null, null);

               if (fittings.Count < 1)
                  continue;

               //fitting's instance parameters

               PressureLossParameter PLParam1 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_PARAM), false, -1, (int)SectionMemberType.Type);
               if (!fittingParameters.Contains(PLParam1))
                  fittingParameters.Add(PLParam1);

               PressureLossParameter PLParam2 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM), false, -1, (int)SectionMemberType.Type);
               if (!fittingParameters.Contains(PLParam2))
                  fittingParameters.Add(PLParam2);

               PressureLossParameter PLParam3 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_SIZE_FORMATTED_PARAM), false, -1, (int)SectionMemberType.Fitting);
               if (!fittingParameters.Contains(PLParam3))
                  fittingParameters.Add(PLParam3);

               foreach (Parameter param in fittings[0].Parameters)
               {
                  //exclude the parameters under constrains/Graphics/Geometry and other groups
                  if (param.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                     || param.Definition.ParameterGroup == BuiltInParameterGroup.PG_GRAPHICS
                     || param.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY
                     || LabelUtils.GetLabelFor(param.Definition.ParameterGroup) == ReportResource.other
                     || param.Definition.Name == LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_PRESSURE_DROP))
                     continue;

                  PressureLossParameter PLParam = new PressureLossParameter(param.Definition.Name, false, -1, (int)SectionMemberType.Fitting);
                  if (!fittingParameters.Contains(PLParam))
                     fittingParameters.Add(PLParam);
               }

               //Fitting symbol parameters

               FamilySymbol famSym = fittings[0].Symbol;
               if (famSym != null)
               {

                  foreach (Parameter param in famSym.Parameters)
                  {
                     //exclude the parameters under construction and other groups
                     if ( param.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRUCTION
                        || LabelUtils.GetLabelFor(param.Definition.ParameterGroup) == ReportResource.other)
                        continue;

                     PressureLossParameter PLParam = new PressureLossParameter(param.Definition.Name, false, -1, (int)SectionMemberType.Type);
                     if (!fittingParameters.Contains(PLParam))
                        fittingParameters.Add(PLParam);
                  }
               }

               reportData.FittingFields = fittingParameters;

               return;
            }
         }
      }

   }
}
