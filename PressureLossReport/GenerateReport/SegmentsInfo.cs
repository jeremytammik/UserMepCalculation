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

namespace UserPressureLossReport
{
   class SegmentsInfo : ReportInfo
   {
      private List<MEPSection> sections;

      public SegmentsInfo(List<MEPSection> mepSections)
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

         //segment title
         List<string> strSegmentFields = new List<string>();
         getFields(strSegmentFields);
         writer.WriteElementString("SegmentInfoTitle", ReportResource.segmentDetailInfo);
         writer.writeTableTitle("Segment", strSegmentFields);

         //segment info
         foreach (MEPSection section in sections)
         {
            string str = "Segment";
            DataTable detailInfoTB = new DataTable("SegmentDetailInfo");

            bool bTable = getSegmentInfo(section, detailInfoTB, true);
            string totalPL = helper.getTotalPressureLossByType(section, SectionMemberType.Segment);

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

         writer.addTitleRow(ReportResource.segmentDetailInfo);

         //segment title
         List<string> strSegmentFields = new List<string>();
         getFields(strSegmentFields);
         DataTable tbTitle = new DataTable();
         helper.getTableTitle(tbTitle, strSegmentFields);
         writer.AddData(tbTitle, 1);

         foreach (MEPSection section in sections)
         {
            DataTable tb = new DataTable();
            if (getSegmentInfo(section, tb))
               writer.AddData(tb, 1);
         }
      }

      public override bool needToWrite()
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper.ReportData == null || helper.ReportData.DisplayDetailInfoForStraightSeg == false || helper.ReportData.StraightSegFields == null)
            return false;

         return true;
      }

      private bool getSegmentInfo(MEPSection section, DataTable segmentTB, bool forHTML = false)
      {
         if (segmentTB == null || section == null)
            return false;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null || helper.ReportData == null)
            return false;

         //get fields from reportData
         List<string> segmentFields = new List<string>();
         getFields(segmentFields);

         if (forHTML)
            helper.addColumns(segmentTB, segmentFields.Count);
         else
            helper.addColumns(segmentTB, segmentFields.Count+2);

         List<MEPCurve> curves = new List<MEPCurve>();
         SectionsInfo.getSectionElements(section, curves, null, null, null);
         if (curves.Count < 1)
            return false;

         int nIndex = 0;
         foreach (MEPCurve crv in curves)
         {
            List<string> paramVals = new List<string>();
            if (!forHTML)
            {
               if (nIndex == 0)
                  paramVals.Add(section.Number.ToString());
               else
                  paramVals.Add(" ");
            }

            foreach (string fieldName in segmentFields)
            {
               PressureLossParameter PLParam = helper.getPressureLossParamByName(helper.ReportData.StraightSegFields, fieldName);
               if (PLParam == null)
                  continue;

               string strVal = ReportConstants.emptyValue;
               if (((PLParam.GetFrom & (int)SectionMemberType.Section) > 0))
               {
                 strVal = SectionsInfo.getSectionInfoByParamName(section, fieldName, PLParam.GetFrom, crv.Id);
               }
               else if ((PLParam.GetFrom & (int)SectionMemberType.Segment) > 0)
               {
                  if (helper.Domain == ReportResource.pipeDomain
                     && fieldName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FLOW_STATE_PARAM))
                  {
                     int nVal = crv.get_Parameter(fieldName).AsInteger();
                     strVal = LabelUtils.GetLabelFor((Autodesk.Revit.DB.Plumbing.PipeFlowState)nVal, helper.Doc);
                  }
                  else if ((fieldName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_SIZE_FORMATTED_PARAM)
                     || fieldName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_SIZE_FORMATTED_PARAM))
                     && ((BuiltInCategory)crv.Category.Id.IntegerValue == BuiltInCategory.OST_FlexDuctCurves
                     || (BuiltInCategory)crv.Category.Id.IntegerValue == BuiltInCategory.OST_FlexPipeCurves))
                  {
                     //for flex duct/pipe, no size parameter, using diameter?
                     if (helper.Domain == ReportResource.pipeDomain)
                        strVal = helper.getParamValue(crv.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM));
                     else
                     {
                        //TBD: need to check round or rect
                        strVal = helper.getParamValue(crv.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM));
                     }
                  }
                  else if (fieldName == ReportResource.elementId)
                     strVal = crv.Id.ToString();
                  else
                     strVal = helper.getParamValue(crv.get_Parameter(fieldName));
               }
               else if ((PLParam.GetFrom & (int)SectionMemberType.Type) > 0)
                  strVal = getSegmentTypeInfoByParamName(crv, fieldName);

               paramVals.Add(strVal);
            }

            if (!forHTML) //for csv, the last column is section pressure loss report
            {
               string strVal = ReportConstants.mergeValue;
               if (nIndex == 0)
                  strVal = helper.getTotalPressureLossByType(section, SectionMemberType.Segment);

               paramVals.Add(strVal);
            }

            nIndex++;

            helper.addRow(segmentTB, paramVals);
         }

         return true;
      }

      private string getSegmentTypeInfoByParamName(MEPCurve crv, string paramName)
      {
         string paramVal = "";

         if (crv == null || paramName == null)
            return paramVal;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return paramVal;

         MEPCurveType crvType = helper.Doc.GetElement(crv.GetTypeId()) as MEPCurveType;
         if (crvType != null)
         {            
            paramVal = helper.getFamilyOrTypeName(crv.Id, paramName);
            if (paramVal == null || paramVal.Length < 1)
               paramVal = helper.getParamValue(crvType.get_Parameter(paramName));
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
            helper.getFieldsFromReportdata(helper.ReportData.StraightSegFields, fileds);

      }

      static public void generateSegmentFields(PressureLossReportData reportData)
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         List<PressureLossParameter> segParameters = new List<PressureLossParameter>();

         //default segment selected fields
         //SectionMemberType means where the value is from: segment or section
         int nOrder = 1;
         segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ALL_MODEL_MARK), false, -1, (int)SectionMemberType.Segment));
         segParameters.Add(new PressureLossParameter(ReportResource.elementId, true, nOrder++, (int)SectionMemberType.Segment));
         
         if (helper.Domain == ReportResource.pipeDomain)
         {
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FLOW_PARAM), true, nOrder++, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_SIZE_FORMATTED_PARAM), true, nOrder++, (int)SectionMemberType.Segment));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FRICTION_FACTOR_PARAM), false, -1, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM), false, -1, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_REYNOLDS_NUMBER_PARAM), false, -1, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM), false, -1, (int)SectionMemberType.Section));
         }
         else
         {
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FLOW_PARAM), true, nOrder++, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_SIZE_FORMATTED_PARAM), true, nOrder++, (int)SectionMemberType.Segment));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_REYNOLDSNUMBER_PARAM), false, -1, (int)SectionMemberType.Section));
            segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT), false, -1, (int)SectionMemberType.Section));
         }

         segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY), true, nOrder++, (int)SectionMemberType.Section));
         segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY_PRESSURE), true, nOrder++, (int)SectionMemberType.Section));
         segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.CURVE_ELEM_LENGTH), true, nOrder++, (int)SectionMemberType.Section));
         segParameters.Add(new PressureLossParameter(ReportResource.pressureLoss, true, 100, (int)SectionMemberType.Section, false));

         segParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_FRICTION), false, -1, (int)SectionMemberType.Section));

         //find the first curve
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
               List<MEPCurve> curves = new List<MEPCurve>();
               SectionsInfo.getSectionElements(section, curves, null, null, null);

               if (curves.Count < 1)
                  continue;

               //segment parameters
               PressureLossParameter PLParam1 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_PARAM), false, -1, (int)SectionMemberType.Type);
               if (!segParameters.Contains(PLParam1))
                  segParameters.Add(PLParam1);

               PressureLossParameter PLParam2 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM), false, -1, (int)SectionMemberType.Type);
               if (!segParameters.Contains(PLParam2))
                  segParameters.Add(PLParam2);

               PressureLossParameter PLParam3 = new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_SIZE_FORMATTED_PARAM), false, -1, (int)SectionMemberType.Segment);
               if (!segParameters.Contains(PLParam3))
                  segParameters.Add(PLParam3);

               foreach (Parameter param in curves[0].Parameters)
               {
                  //exclude the parameters under constrain and other groups
                  if (param.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                     || LabelUtils.GetLabelFor(param.Definition.ParameterGroup) == ReportResource.other
                     || param.Definition.Name == LabelUtils.GetLabelFor(BuiltInParameter.RBS_SECTION)
                     || param.Definition.Name == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PRESSURE_DROP))
                     continue;

                  PressureLossParameter PLParam = new PressureLossParameter(param.Definition.Name, false, -1, (int)SectionMemberType.Segment);
                  if (!segParameters.Contains(PLParam))
                     segParameters.Add(PLParam);
               }

               //segmentType parameters
               MEPCurveType crvType = helper.Doc.GetElement(curves[0].GetTypeId()) as MEPCurveType;
               if (crvType != null)
               {
                  foreach (Parameter param in crvType.Parameters)
                  {
                     //exclude the parameters under Fitting/Segments and other groups
                     if (param.Definition.ParameterGroup == BuiltInParameterGroup.PG_FITTING
                     || param.Definition.ParameterGroup == BuiltInParameterGroup.PG_SEGMENTS_FITTINGS
                     || LabelUtils.GetLabelFor(param.Definition.ParameterGroup) == ReportResource.other)
                        continue;

                     PressureLossParameter PLParam = new PressureLossParameter(param.Definition.Name, false, -1, (int)SectionMemberType.Type);
                     if (!segParameters.Contains(PLParam))
                        segParameters.Add(PLParam);
                  }
               }

               reportData.StraightSegFields = segParameters;

               return;
            }
         }
      }
   }

}
