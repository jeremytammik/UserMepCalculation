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
using System.Collections;
using System.IO;
using System.Data;
using System.Xml.Xsl;  
using System.Xml;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Mechanical;

namespace UserPressureLossReport
{
   /// <summary>
   /// Base class providing interface to save report data to a file.
   /// </summary>
   public abstract class SaveData
   {
      public virtual bool save(string fileName, PressureLossReportData reportData)
      {
         return false;
      }
   }

   /// <summary>
   /// Save report data to a HTML file.
   /// </summary> 
   public class SaveDataToHTML : SaveData
   {
      public override bool save(string fileName, PressureLossReportData reportData)
      {
         HtmlStreamWriter writer = new HtmlStreamWriter();
         try
         {
  // Check if the xslt file exists

  if (!File.Exists(writer.XsltFileName))
  {
      string subMsg = ReportResource.xsltFileSubMsg
        .Replace("%FULLPATH%", writer.XsltFileName );

      UIHelperFunctions.postWarning(
        ReportResource.htmlGenerateTitle, 
        ReportResource.xsltFileMsg, subMsg );

      return false;
  }

            PressureLossReportHelper helper = PressureLossReportHelper.instance;
            if (helper == null)
               return false;

            //xml head
            writer.WriteStartDocument(false); 

            //root node         
            string transXML = "UserPressureLossReport";
            writer.WriteStartElement(transXML);

            //title
            writer.WriteElementString("Title", ReportResource.reportName);

            //domain
            if (helper.Domain == ReportResource.pipeDomain)
               writer.WriteElementString("DomainName", ReportResource.pipeReportName);
            else
               writer.WriteElementString("DomainName", ReportResource.ductReportName);

            //write project info
            ReportProjectInfo proInfo = new ReportProjectInfo();
            proInfo.writeToHTML(writer); 

            //each system
            List<MEPSystem> systems = helper.getSortedSystems();
            if (systems == null || systems.Count < 1)
               return false;

            foreach (MEPSystem sysElem in systems)
            {
               if (sysElem == null)
                  continue;

               //system node
               string xmlString = "System";
               writer.WriteStartElement(xmlString);

               //system info: name and info
               MEPSystemInfo systemInfo = new MEPSystemInfo(sysElem);
               systemInfo.writeToHTML(writer);

               //critical path
               if (helper.ReportData.DisplayCriticalPath)
               {
                  string criticalInfo =ReportResource.criticalPath + " : " + MEPSystemInfo.getCriticalPath(sysElem);
                  criticalInfo +=  " ; "+ ReportResource.totalPressureLoss + " : " + MEPSystemInfo.getCriticalPathPressureLoss(sysElem);
                  writer.WriteElementString("CriticalPath", criticalInfo);
               }

               List<MEPSection> sections = new List<MEPSection>();
               MEPSystemInfo.getSectionsFromSystem(sysElem, sections);
   
               //sections: title and info
               SectionsInfo sectionInfo = new SectionsInfo(sections);
               sectionInfo.writeToHTML(writer);

               //segments: title and info
               SegmentsInfo segmentsInfo = new SegmentsInfo(sections);
               segmentsInfo.writeToHTML(writer);
  
               //fittings: title and info
               FittingsInfo fittingsInfo = new FittingsInfo(sections);
               fittingsInfo.writeToHTML(writer);

               writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Save(fileName);
            return true;
         }
         catch
         {
            writer.Close();
            //delete xml
            File.Delete(writer.XmlFileName);
            UIHelperFunctions.postWarning(ReportResource.htmlGenerateTitle, ReportResource.htmlMsg);
            return false;
         }         
      }
   }

   /// <summary>
   /// Save report data to a CSV file.
   /// </summary> 
   public class SaveDataToCSV : SaveData
   {
      // TODO
      public override bool save(string fileName, PressureLossReportData reportData)
      {

         try
         {
            PressureLossReportHelper helper = PressureLossReportHelper.instance;
            CsvStreamWriter writer = new CsvStreamWriter();

            //title
            string strTitle = ReportResource.reportName;
            if (helper.Domain == ReportResource.pipeDomain)
               strTitle = ReportResource.pipeReportName;
            else
               strTitle = ReportResource.ductReportName;

            DataTable titleTB = new DataTable();
            titleTB.Columns.Add();
            titleTB.Rows.Add(strTitle);
            writer.AddData(titleTB, 1);
            writer.addOneEmptyRow();

            DataTable tbTitle = new DataTable();
            DataTable tb = new DataTable();

            //Project info
            ReportProjectInfo proInfo = new ReportProjectInfo();
            proInfo.writeToCsv(writer);
            
            writer.addOneEmptyRow();

            //each system
            List<MEPSystem> systems = helper.getSortedSystems();
            if (systems == null || systems.Count < 1)
               return false;
            foreach (MEPSystem sysElem in systems)
            {
               if (sysElem == null)
                  continue;

               //system name and info
               MEPSystemInfo systemInfo = new MEPSystemInfo(sysElem);
               systemInfo.writeToCsv(writer);

               if (systemInfo.needToWrite())
                  writer.addOneEmptyRow();

               List<MEPSection> sections = new List<MEPSection>();
               MEPSystemInfo.getSectionsFromSystem(sysElem, sections);

               //sections: title and info
               SectionsInfo sectionInfo = new SectionsInfo(sections);
               sectionInfo.writeToCsv(writer);

               if (reportData.DisplayCriticalPath)
               {
                  string criticalInfo = ReportResource.criticalPath + " : " + MEPSystemInfo.getCriticalPath(sysElem);
                  criticalInfo += " ; " + ReportResource.totalPressureLoss + " : " + MEPSystemInfo.getCriticalPathPressureLoss(sysElem);

                  writer.addTitleRow(criticalInfo);
               }

               writer.addOneEmptyRow();

               //segments: title and info
               SegmentsInfo segmentsInfo = new SegmentsInfo(sections);
               segmentsInfo.writeToCsv(writer);
               if (segmentsInfo.needToWrite())
                  writer.addOneEmptyRow();

               //fittings: title and info
               FittingsInfo fittingsInfo = new FittingsInfo(sections);
               fittingsInfo.writeToCsv(writer);
               if (fittingsInfo.needToWrite())
                  writer.addOneEmptyRow();
            }

            writer.Save(fileName);
            return true;
         }
         catch(Exception e)
         {
            if (e.Message == ReportConstants.failed_to_delete)
            {
               string subMsg = ReportResource.csvSubMsg.Replace("%FULLPATH%", fileName);
               UIHelperFunctions.postWarning(ReportResource.csvGenerateTitle, ReportResource.csvMsg, subMsg);
            }
            else
               UIHelperFunctions.postWarning(ReportResource.csvGenerateTitle, ReportResource.csvMsg);
            return false;
         }
      }
   }
}


