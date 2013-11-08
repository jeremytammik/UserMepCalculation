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
using  Autodesk.Revit.DB.Mechanical;

namespace UserPressureLossReport
{
   class SectionsInfo : ReportInfo
   {
      private List<MEPSection> sections;

      public SectionsInfo(List<MEPSection> mepSections)
      {
         sections = mepSections;
      }

      public override void writeToHTML(HtmlStreamWriter writer)
      {
         if (writer == null || sections == null)
            return;
         
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         //section title
         List<string> strSectionFields = new List<string>();
         getFields(strSectionFields);
         writer.WriteElementString("SectionInfoTitle", ReportResource.sectionTitle);
         writer.writeTableTitle("Section", strSectionFields, true);

         //section info
         foreach(MEPSection section in sections)
         {
            string str = "Section";
            DataTable detailInfoTB = new DataTable("SectionDetailInfo");
            bool bTable = getSectionInfo(section, detailInfoTB, true);
            string totalPL = helper.getTotalPressureLossByType(section, SectionMemberType.Section);

            if (bTable)
               writer.writeTable(str, detailInfoTB, section.Number.ToString(), totalPL);
         }       
      }

      public override void writeToCsv(CsvStreamWriter writer)
      {
         if (writer == null || sections == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         writer.addTitleRow(ReportResource.sectionTitle);


         //section title
         List<string> strSectionFields = new List<string>();
         getFields(strSectionFields);
         DataTable tbTitle = new DataTable();
         helper.getTableTitle(tbTitle, strSectionFields, false, true);
         writer.AddData(tbTitle, 1);

         //each section
         foreach(MEPSection section in sections)
         {            
            DataTable tb = new DataTable();
            if (getSectionInfo(section, tb))
               writer.AddData(tb, 1);
         }
      }

      public bool getSectionInfo(MEPSection section, DataTable sectionTB, bool forHTML = false)
      {
         if (section == null || sectionTB == null)
            return false;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return false;

         PressureLossReportData reportData = helper.ReportData;
         if (reportData == null)
            return false;

         List<string> sectionFields = new List<string>();
         getFields(sectionFields);

         getSectionTable(section, sectionTB, sectionFields, reportData, forHTML);

         return true;
      }

      private void getFields(List<string> fileds)
      {
         if (fileds == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         if (helper.ReportData.AvailableFields != null)
            helper.getFieldsFromReportdata(helper.ReportData.AvailableFields, fileds);

         fileds.Insert(0, ReportResource.elementFiled);
         fileds.Add(ReportResource.totalPressureLoss);
      }

      private void getSectionTable(MEPSection section, DataTable tb, List<string> fileds, PressureLossReportData reportData, bool bForHtml = false)
      {
         if (tb == null || fileds == null || section == null || reportData == null)
            return;

         List<MEPCurve> curves = new List<MEPCurve>();
         List<FamilyInstance> fittings = new List<FamilyInstance>();
         List<FamilyInstance> airTerminals = new List<FamilyInstance>();
         List<FamilyInstance> equipments = new List<FamilyInstance>();

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         getSectionElements(section, curves, fittings, airTerminals, equipments);

         Dictionary<string, string> fieldAndValue = new Dictionary<string, string>();
         getSectionCommonInfo(section, fieldAndValue);

         if (bForHtml)
            helper.addColumns(tb, fileds.Count);
         else
            helper.addColumns(tb, fileds.Count+2);

         List<string> segmentVals = new List<string>();//segment row         
         List<string> fittingVals = new List<string>();//fitting row
         List<string> airTerminalVals = new List<string>();//air terminal row
         List<string> equipmentsVals = new List<string>();//equipment row

         if (!bForHtml) //for csv, the first column is the section number
         {

            segmentVals.Add(section.Number.ToString());
            if (curves.Count < 1)
               fittingVals.Add(section.Number.ToString());
            else
               fittingVals.Add(" ");

            if (curves.Count < 1 && fittings.Count < 1)
               airTerminalVals.Add(section.Number.ToString());
            else
               airTerminalVals.Add(" ");

            if (curves.Count < 1 && fittings.Count < 1 && airTerminals.Count < 1)
               equipmentsVals.Add(section.Number.ToString());
            else
               equipmentsVals.Add(" ");
         }

         segmentVals.Add(helper.Domain);
         fittingVals.Add(ReportResource.fittings);

         if (helper.Domain == ReportResource.pipeDomain)
            airTerminalVals.Add(ReportResource.plumbingFixtures);
         else
            airTerminalVals.Add(ReportResource.airTerminals);
         equipmentsVals.Add(ReportResource.equipments);

         foreach (string fieldName in fileds)
         {
            PressureLossParameter PLParam = helper.getPressureLossParamByName(reportData.AvailableFields, fieldName);
            if (PLParam == null)
               continue;

            if (fieldAndValue.ContainsKey(fieldName)) //section info
            {
               if ((PLParam.GetFrom & (int)SectionMemberType.Segment) > 0)
                  segmentVals.Add(fieldAndValue[fieldName]);
               else
                  segmentVals.Add(ReportConstants.emptyValue);

               if ((PLParam.GetFrom & (int)SectionMemberType.Fitting) > 0)
                  fittingVals.Add(fieldAndValue[fieldName]);
               else
                  fittingVals.Add(ReportConstants.emptyValue);

               if ((PLParam.GetFrom & (int)SectionMemberType.AirTerminal) > 0)               
                  airTerminalVals.Add(fieldAndValue[fieldName]);
               else
                  airTerminalVals.Add(ReportConstants.emptyValue);

               if ((PLParam.GetFrom & (int)SectionMemberType.Equipment) > 0)
                  equipmentsVals.Add(fieldAndValue[fieldName]);
               else
                  equipmentsVals.Add(ReportConstants.emptyValue);
            }
            else if (curves.Count > 0 && (PLParam.GetFrom & (int)SectionMemberType.Segment) > 0) //read the value from first segment
            {
               MEPCurve firstCrv = curves[0];
               if (firstCrv == null)
                  continue;

               string strVal = helper.getParamValue(firstCrv.get_Parameter(fieldName));
               segmentVals.Add(strVal);
               fittingVals.Add(ReportConstants.emptyValue);
               airTerminalVals.Add(ReportConstants.emptyValue);
               equipmentsVals.Add(ReportConstants.emptyValue);
            }
            else
            {
               segmentVals.Add(ReportConstants.emptyValue);
               fittingVals.Add(ReportConstants.emptyValue);
               airTerminalVals.Add(ReportConstants.emptyValue);
               equipmentsVals.Add(ReportConstants.emptyValue);
            }
         }

         //add total pressure loss
         segmentVals.Add(helper.getTotalPressureLossByType(section, SectionMemberType.Segment));
         fittingVals.Add(helper.getTotalPressureLossByType(section, SectionMemberType.Fitting));
         airTerminalVals.Add(helper.getTotalPressureLossByType(section, SectionMemberType.AirTerminal));
         equipmentsVals.Add(helper.getTotalPressureLossByType(section, SectionMemberType.Equipment));

         //add section pressure loss
         if (!bForHtml) //for csv, the last column is section pressure loss report
         {
            string sectionPL = fieldAndValue[ReportResource.sectionPressureLoss];

            segmentVals.Add(sectionPL);
            if (curves.Count < 1)
               fittingVals.Add(sectionPL);
            else
               fittingVals.Add(ReportConstants.mergeValue);

            if (curves.Count < 1 && fittings.Count < 1)
               airTerminalVals.Add(sectionPL);
            else
               airTerminalVals.Add(ReportConstants.mergeValue);

            if (curves.Count < 1 && fittings.Count < 1 && airTerminals.Count < 1)
               equipmentsVals.Add(sectionPL);
            else
               equipmentsVals.Add(ReportConstants.mergeValue);
         }

         if (curves.Count > 0)
            helper.addRow(tb, segmentVals);
         if (fittings.Count > 0)
            helper.addRow(tb, fittingVals);
         if (airTerminals.Count > 0)
            helper.addRow(tb, airTerminalVals);
         if (equipments.Count > 0)
            helper.addRow(tb, equipmentsVals);
      }

      static public void getSectionCommonInfo(MEPSection section, Dictionary<string, string> fieldAndValue)
      {
         if (fieldAndValue == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         Document doc = helper.Doc;
         if (doc == null)
            return;             

         if (helper.Domain == ReportResource.ductDomain)
         {
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FLOW_PARAM), FormatUtils.Format(doc, UnitType.UT_HVAC_Airflow, section.Flow));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_FRICTION), FormatUtils.Format(doc, UnitType.UT_HVAC_Friction, section.Friction));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY), FormatUtils.Format(doc, UnitType.UT_HVAC_Velocity, section.Velocity));
            fieldAndValue.Add(ReportResource.sectionPressureLoss, FormatUtils.Format(doc, UnitType.UT_HVAC_Pressure, section.TotalPressureLoss));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY_PRESSURE), FormatUtils.Format(doc, UnitType.UT_HVAC_Pressure, section.VelocityPressure));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_REYNOLDSNUMBER_PARAM), FormatUtils.Format(doc, UnitType.UT_Number, section.ReynoldsNumber));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT), FormatUtils.Format(doc, UnitType.UT_Number, getFittingsLossCoefficient(section)));
         }
         else
         {
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FLOW_PARAM), FormatUtils.Format(doc, UnitType.UT_Piping_Flow, section.Flow));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_FRICTION), FormatUtils.Format(doc, UnitType.UT_Piping_Friction, section.Friction));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY), FormatUtils.Format(doc, UnitType.UT_Piping_Velocity, section.Velocity));
            fieldAndValue.Add(ReportResource.sectionPressureLoss, FormatUtils.Format(doc, UnitType.UT_Piping_Pressure, section.TotalPressureLoss));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY_PRESSURE), FormatUtils.Format(doc, UnitType.UT_Piping_Pressure, section.VelocityPressure));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_REYNOLDS_NUMBER_PARAM), FormatUtils.Format(doc, UnitType.UT_Number, section.ReynoldsNumber));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FRICTION_FACTOR_PARAM), FormatUtils.Format(doc, UnitType.UT_Number, section.FrictionFactor));
            fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM), FormatUtils.Format(doc, UnitType.UT_Number, getFittingsLossCoefficient(section)));

            //need to check system type
            if (  helper.SystemClassification == MEPSystemClassification.DomesticColdWater
               || helper.SystemClassification == MEPSystemClassification.DomesticHotWater
               || helper.SystemClassification == MEPSystemClassification.Sanitary)
               fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM), FormatUtils.Format(doc, UnitType.UT_Number, section.FixtureUnit));
            else
               fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM), ReportConstants.emptyValue);
         }

         fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_ROUGHNESS_PARAM), FormatUtils.Format(doc, UnitType.UT_Piping_Roughness, section.Roughness));
         fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.CURVE_ELEM_LENGTH), FormatUtils.Format(doc, UnitType.UT_Length, section.TotalCurveLength));
         fieldAndValue.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_SECTION), section.Number.ToString());

      }

      static public double getFittingsLossCoefficient(MEPSection section)
      {
         double dVal = 0.0;
         List<FamilyInstance> fittings = new List<FamilyInstance>();
         SectionsInfo.getSectionElements(section, null, fittings, null, null);
         foreach (FamilyInstance famInst in fittings)
            dVal += section.GetCoefficient(famInst.Id);

         return dVal;
      }

      static public void generateSectionFields(PressureLossReportData reportData)
      {
         if (reportData == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         List<PressureLossParameter> sectionParameters = new List<PressureLossParameter>();

         //section fields
         //default selected ones
         //for section table: SectionMemberType means display the info for which part: segment or AirTerminal or Equipment
         int nOrder = 1;
         if (helper.Domain == ReportResource.pipeDomain)
         {
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FLOW_PARAM), true, nOrder++, (int)(SectionMemberType.Segment | SectionMemberType.AirTerminal | SectionMemberType.Fitting | SectionMemberType.Equipment)));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_SIZE_FORMATTED_PARAM), true, nOrder++, (int)(SectionMemberType.Segment | SectionMemberType.AirTerminal | SectionMemberType.Equipment)));
         }
         else
         {
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_FLOW_PARAM), true, nOrder++, (int)(SectionMemberType.Segment | SectionMemberType.AirTerminal | SectionMemberType.Fitting | SectionMemberType.Equipment)));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_SIZE_FORMATTED_PARAM), true, nOrder++, (int)(SectionMemberType.Segment | SectionMemberType.AirTerminal | SectionMemberType.Equipment)));
         }

         sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY), true, nOrder++, (int)(SectionMemberType.Segment | SectionMemberType.Fitting)));
         sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_VELOCITY_PRESSURE), true, nOrder++, (int)SectionMemberType.Fitting));
         sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.CURVE_ELEM_LENGTH), true, nOrder++, (int)SectionMemberType.Segment));
         if (helper.Domain == ReportResource.pipeDomain)
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM), true, nOrder++, (int)SectionMemberType.Fitting));
         else
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT), true, nOrder++, (int)SectionMemberType.Fitting));
         sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_FRICTION), true, nOrder++, (int)(SectionMemberType.Segment)));

         //from segment
         sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_REFERENCE_OVERALLSIZE), false, -1, (int)SectionMemberType.Segment));
         if (helper.Domain == ReportResource.pipeDomain)
         {
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_MATERIAL_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FRICTION_FACTOR_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_RELATIVE_ROUGHNESS_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_REYNOLDS_NUMBER_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_ROUGHNESS_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM), false, -1, (int)SectionMemberType.Segment));
         }
         else
         {
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_CURVE_WIDTH_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_EQ_DIAMETER_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_REFERENCE_FREESIZE), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_HYDRAULIC_DIAMETER_PARAM), false, -1, (int)SectionMemberType.Segment));
            sectionParameters.Add(new PressureLossParameter(LabelUtils.GetLabelFor(BuiltInParameter.RBS_REYNOLDSNUMBER_PARAM), false, -1, (int)SectionMemberType.Segment));
         }

         reportData.AvailableFields = sectionParameters;

      }

      static public void getSectionElements(MEPSection section, List<MEPCurve> curves, List<FamilyInstance> fittings, List<FamilyInstance> airterminals, List<FamilyInstance> equipments)
      {
         if (section == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         IList<ElementId> ids = new List<ElementId>();
         ids = section.GetElementIds();
         foreach (ElementId id in ids)
         {
            Element sectionMemElem = helper.Doc.GetElement(id);

            if (sectionMemElem is MEPCurve)
            {
               MEPCurve crv = sectionMemElem as MEPCurve;
               if (crv != null && curves != null)
                  curves.Add(crv);
            }
            else if ((sectionMemElem is FamilyInstance) && (fittings != null || airterminals != null || equipments != null))
            {
               FamilyInstance famInst = sectionMemElem as FamilyInstance;
               if (famInst == null)
                  continue;

               if (((BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_DuctFitting
                  || (BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_PipeFitting
                  || (BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_DuctAccessory
                  || (BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_PipeAccessory))
               {
                  if (fittings != null)
                     fittings.Add(famInst);
               }
               else if ((BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_DuctTerminal
                        || (BuiltInCategory)famInst.Category.Id.IntegerValue == BuiltInCategory.OST_PlumbingFixtures)
               {
                  if (airterminals != null)
                     airterminals.Add(famInst);
               }
               else 
               {
                  if (equipments != null)
                     equipments.Add(famInst);
               }
            }
         }
      }

      static public string getSectionInfoByParamName(MEPSection section, string paramName, int nGetFrom, ElementId id)
      {
         string paramVal = "";
         if (section == null || paramName == null || (nGetFrom & (int)SectionMemberType.Section) == 0)
            return paramVal;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return paramVal;

         if (paramName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PRESSURE_DROP)
            || paramName == ReportResource.pressureLoss)
         {
            double dVal = section.GetPressureDrop(id);

            if (helper.Domain == ReportResource.pipeDomain)
               paramVal = FormatUtils.Format(helper.Doc, UnitType.UT_Piping_Pressure, dVal);
            else
               paramVal = FormatUtils.Format(helper.Doc, UnitType.UT_HVAC_Pressure, dVal);
            return paramVal;
         }
         else if (paramName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_LOSS_COEFFICIENT)
                || paramName == LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPE_FITTING_LOSS_KFACTOR_PARAM))
         {
            double dVal = section.GetCoefficient(id);
            paramVal = FormatUtils.Format(helper.Doc, UnitType.UT_Number, dVal);
            return paramVal;
         }

         else if (paramName == LabelUtils.GetLabelFor(BuiltInParameter.CURVE_ELEM_LENGTH))
         {
            double dVal = section.GetSegmentLength(id);
            paramVal = FormatUtils.Format(helper.Doc, UnitType.UT_Length, dVal);
            return paramVal;
         }

         Dictionary<string, string> fieldAndValue = new Dictionary<string, string>();
         SectionsInfo.getSectionCommonInfo(section, fieldAndValue);

         if (fieldAndValue.ContainsKey(paramName))
            return fieldAndValue[paramName];

         return paramVal;
      }

   }
}
