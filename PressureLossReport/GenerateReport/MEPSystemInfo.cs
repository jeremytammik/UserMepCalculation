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
   class MEPSystemInfo : ReportInfo
   {
      private MEPSystem system;

      public MEPSystemInfo(MEPSystem systemElem)
      {
         system = systemElem;
         if (system == null)
            return;

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;         

         MEPSystemType sysType = helper.Doc.GetElement(system.GetTypeId()) as MEPSystemType;
         if (sysType != null)
            helper.SystemClassification = sysType.SystemClassification;
            
         
      }

      public override void writeToHTML(HtmlStreamWriter writer)
      {
         if (writer == null || system == null)
            return;

         //system name
         string sysName = system.Name;
         if (sysName.Length > 0)
            writer.WriteElementString("SystemName", sysName);

         //system info
         if (needToWrite())
         {
            DataTable tb = new DataTable("SystemInfo");
            getInfoDataTable(tb);
            writer.WriteElementString("SystemInfoTitle", ReportResource.systemInformation);
            writer.writeDataTable(tb);
         }
      }

      public override void writeToCsv(CsvStreamWriter writer)
      {
         if (writer == null || system == null)
            return;

         //system name
         string sysName = system.Name;
         if (sysName.Length > 0)
         {
            writer.addTitleRow(sysName);
            writer.addOneEmptyRow();
         }

         if (needToWrite())
         {
            writer.addTitleRow(ReportResource.systemInformation);
            DataTable tb = new DataTable();
            getInfoDataTable(tb);
            writer.AddData(tb, 1);
         }
      }

      public void getInfoDataTable(DataTable systemTB)
      {
         if (systemTB == null || system == null)
            return;

         systemTB.Columns.Add("SystemInfoName");
         systemTB.Columns.Add("SystemInfoValue");

         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null)
            return;

         MEPSystemType sysType = helper.Doc.GetElement(system.GetTypeId()) as MEPSystemType;
         if (sysType != null)
         {
            systemTB.Rows.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM), system.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM).AsString());

            if (helper.Domain == ReportResource.ductDomain)
               systemTB.Rows.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM), sysType.Name);
            else
               systemTB.Rows.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM), sysType.Name);
            systemTB.Rows.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_SYSTEM_NAME_PARAM), system.Name);
            systemTB.Rows.Add(LabelUtils.GetLabelFor(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM), sysType.Abbreviation);

            if (helper.Domain == ReportResource.pipeDomain) // need to list fluid info
            {
               //Fluid type is an element id
               ElementId elemId = sysType.get_Parameter(BuiltInParameter.RBS_PIPE_FLUID_TYPE_PARAM).AsElementId();
               string strFluidType = "";
               if (elemId != null)
               {
                  Element elem = helper.Doc.GetElement(elemId);
                  if (elem != null)
                     strFluidType = elem.Name;
               }
               systemTB.Rows.Add(sysType.get_Parameter(BuiltInParameter.RBS_PIPE_FLUID_TYPE_PARAM).Definition.Name, strFluidType);               
               helper.addParameterNameAndValueToTable(systemTB, sysType.get_Parameter(BuiltInParameter.RBS_PIPE_FLUID_TEMPERATURE_PARAM), false);
               helper.addParameterNameAndValueToTable(systemTB, sysType.get_Parameter(BuiltInParameter.RBS_PIPE_FLUID_VISCOSITY_PARAM), false);
               helper.addParameterNameAndValueToTable(systemTB, sysType.get_Parameter(BuiltInParameter.RBS_PIPE_FLUID_DENSITY_PARAM), false);
            }
         }         

         return;
      }

      public override bool needToWrite()
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper.ReportData == null || helper.ReportData.DisplaySysInfo == false)
            return false;

         return true;
      }

      static public void getSectionsFromSystem(MEPSystem system, List<MEPSection> sections)
      {
         if (system == null || sections == null)
            return;

         int nSection = system.SectionsCount;

         if (nSection > 0)
         {
            for (int ii = 1; ii < nSection + 1; ++ii) //section number start from 1
            {
               MEPSection section = system.GetSectionByNumber(ii);
               if (section == null)
                  continue;

               sections.Add(section);
            }
         }
      }

      static public string getCriticalPath(MEPSystem elemSystem)
      {
         string strPath = "";
         if (elemSystem != null)
         {
            IList<int> paths = elemSystem.GetCriticalPathSectionNumbers();
            if (paths != null)
            {
               int nIndex = 0;
               foreach (int nn in paths)
               {
                  nIndex++;
                  strPath += nn;
                  if (nIndex < paths.Count)
                     strPath += ReportConstants.emptyValue;
               }
            }

         }
         return strPath;
      }

      static public string getCriticalPathPressureLoss(MEPSystem elemSystem)
      {
         string strVal = "";
         if (elemSystem != null)
         {
            if (PressureLossReportHelper.instance.Domain == ReportResource.pipeDomain)
               strVal = FormatUtils.Format(PressureLossReportHelper.instance.Doc, UnitType.UT_Piping_Pressure, elemSystem.PressureLossOfCriticalPath);
            else
               strVal = FormatUtils.Format(PressureLossReportHelper.instance.Doc, UnitType.UT_HVAC_Pressure, elemSystem.PressureLossOfCriticalPath);
            
         }
         return strVal;
      }

   }
}
