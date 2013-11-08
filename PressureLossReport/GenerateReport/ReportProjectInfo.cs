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

namespace UserPressureLossReport
{
   class ReportProjectInfo : ReportInfo
   {
      public override void writeToHTML(HtmlStreamWriter writer)
      {
         if (writer == null || !needToWrite())
            return;
         
         DataTable tb = new DataTable("ProjectInfo");
         getInfoDataTable(tb);
         writer.writeDataTable(tb);
         
      }

      public override void writeToCsv(CsvStreamWriter writer)
      {
         if (writer == null || !needToWrite())
            return;

         DataTable tb = new DataTable();
         getInfoDataTable(tb);
         writer.AddData(tb, 1);
      }

      public void getInfoDataTable(DataTable projectInfoTB)
      {
         PressureLossReportHelper helper = PressureLossReportHelper.instance;
         if (helper == null || helper.Doc == null || projectInfoTB == null)
            return;

         //2 columns
         projectInfoTB.Columns.Add("ProjectInfoName");
         projectInfoTB.Columns.Add("ProjectInfoValue");

         ProjectInfo proInfo = helper.Doc.ProjectInformation;
         if (proInfo == null)
            return;

         List<Parameter> basicProjInfoParams = new List<Parameter>();
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_NAME));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_ISSUE_DATE));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_STATUS));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.CLIENT_NAME));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_ADDRESS));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_NUMBER));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_ORGANIZATION_NAME));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_ORGANIZATION_DESCRIPTION));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_BUILDING_NAME));
         basicProjInfoParams.Add(proInfo.get_Parameter(BuiltInParameter.PROJECT_AUTHOR));

         List<string> names = new List<string>();

         foreach (Parameter param in basicProjInfoParams)
         {
            if (param == null)
               continue;

            helper.addParameterNameAndValueToTable(projectInfoTB, param, false);
            names.Add(param.Definition.Name);
         }

         foreach (Parameter param in helper.Doc.ProjectInformation.Parameters)
         {
            if (param == null || names.Contains(param.Definition.Name))
               continue;

            if (param.StorageType != StorageType.None)
               helper.addParameterNameAndValueToTable(projectInfoTB, param, false);
         }

         if (helper.ReportData.DisplayRunTime)
            projectInfoTB.Rows.Add(ReportResource.runTime, DateTime.Now.ToString());

         return;
      }
   }
}
