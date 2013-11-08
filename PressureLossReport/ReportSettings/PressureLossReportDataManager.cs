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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace UserPressureLossReport
{
   public sealed class PressureLossReportDataManager
   {
      private PressureLossReportDataManager()
      {
         formatFileName = getReportFormatFullName();
      }

      public static readonly PressureLossReportDataManager Instance =
         new PressureLossReportDataManager();

      private string formatFileName;
      public static string lastUsed = "(LastUsed)";

      public string FormatFileName
      {
         get { return formatFileName; }
         set { formatFileName = value; }
      }

      static public string getReportFormatFullName()
      {
         string filename = "";
         try
         {
            PressureLossReportHelper helper = PressureLossReportHelper.instance;
            if (helper == null || helper.Doc == null)
               return filename;

            //same location as record journaling (for journal replaying)
            filename = System.IO.Path.GetDirectoryName(helper.Doc.Application.RecordingJournalFilename);
            if (filename != null && filename.Length > 0)
            {
               string tempName = filename + "\\PressureLossReportFormats.xml";
               if (tempName != null && tempName.Length > 0 && File.Exists(tempName))
                  return tempName;

               filename = System.IO.Path.GetDirectoryName(filename + ".xml");
               filename = filename + "\\PressureLossReportFormats.xml";
            }

            //same location as RevitDB.dll
            if (filename == null || filename.Length < 1 || !File.Exists(filename))
            {
               string strTempfilename = System.IO.Path.GetDirectoryName(helper.Doc.Application.DefaultProjectTemplate);
               if (strTempfilename != null && strTempfilename.Length > 0)
                  filename = strTempfilename + "\\PressureLossReportFormats.xml";
            }
         }
         catch
         {
            //do nothing
         }
         return filename;
      }

      private bool isReportFormatReadOnly()
      {
         if (File.Exists(formatFileName))
         {
            FileAttributes att = File.GetAttributes(formatFileName);
            if (((int)att & (int)FileAttributes.ReadOnly) > 0)
               return true;
         }
         return false;
      }

      /// <summary>
      /// save the data to the xml file, please catch InvalidOperationException to 
      /// identify the existing file is broken.
      /// </summary>
      /// <param name="data">format data</param>
      public void save(PressureLossReportData data)
      {
         try
         {
            XmlSerializer serializer = new XmlSerializer(typeof(PressureLossReportFormats));
            PressureLossReportFormats formats = new PressureLossReportFormats();
            if (File.Exists(formatFileName))
            {
               formats = getAllFormats(false);
               //formats can't been null, it may throw exception. 
               //this format exists
               if (formats != null && formats.Contains(data))
               {
                  formats.Remove(data);
               }
            }

            formats.Add(data);
            using (TextWriter writer = new StreamWriter(formatFileName))
            {
               serializer.Serialize(writer, formats);
            }
         }
         catch
         {
            //do nothing
         }
      }

      /// <summary>
      /// save a data list--it means a PressureLossReportFormats obj
      /// </summary>
      /// <param name="path"></param>
      /// <param name="formats"></param>
      private void save(PressureLossReportFormats formats)
      {
         foreach (PressureLossReportData data in formats)
         {
            save(data);
         }
      }

      /// <summary>
      /// clear all the formats
      /// </summary>
      private void clear()
      {
         if (File.Exists(formatFileName))
            File.Delete(formatFileName);
      }

      /// <summary>
      /// deserialize the format xml file to get the PressureLossReportData object.
      /// </summary>
      /// <param name="formatName">format name</param>
      /// <returns></returns>
      public PressureLossReportData getData(string formatName)
      {
         PressureLossReportFormats formats = getAllFormats();
         if (formats != null)
         {
            foreach (PressureLossReportData data in formats)
            {
               if (0 == string.Compare(data.Name, formatName))
                  return data;
            }
         }
         return null;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
      public PressureLossReportFormats getAllFormats(bool bCheckDomain = true)
      {
         XmlSerializer serializer = new XmlSerializer(typeof(PressureLossReportFormats));
         PressureLossReportFormats formats = new PressureLossReportFormats();
         if (!File.Exists(formatFileName))
            return null;

         using (TextReader reader = new StreamReader(formatFileName))
         {
            try
            {
               PressureLossReportHelper helper = PressureLossReportHelper.instance;
               PressureLossReportFormats allformats = serializer.Deserialize(reader) as PressureLossReportFormats;
               if (allformats != null)
               {
                  foreach (PressureLossReportData data in allformats)
                  {
                     if ((bCheckDomain && data.Domain == helper.Domain) || !bCheckDomain)
                        formats.Add(data);

                  }
               }

               return formats;
            }
            catch (System.InvalidOperationException)
            {
               return formats;
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="path"></param>
      /// <param name="formatName"></param>
      public void remove(string formatName)
      {
         PressureLossReportFormats formats = getAllFormats(false);
         if (formats != null)
         {
            PressureLossReportHelper helper = PressureLossReportHelper.instance;
            foreach (PressureLossReportData data in formats)
            {
               if (0 == string.Compare(data.Name, formatName) && helper.Domain == data.Domain)
               {
                  formats.Remove(data);
                  break;
               }
            }
            clear();
            save(formats);
         }
      }

      public PressureLossReportData getLastUsedReportData()
      {
         PressureLossReportFormats formats = getAllFormats();
         if (formats != null)
         {
            foreach (PressureLossReportData data in formats)
            {
               if (data.Name.Contains(lastUsed))
                  return data;
            }
         }

         return null;
      }

      public string getLastUsedReportName()
      {
         string lastReportName = "";
         PressureLossReportData data = getLastUsedReportData();
         if (data != null)
         {
            if (data.Name.Contains("(LastUsed)"))
               return data.Name.Substring(0, data.Name.LastIndexOf("(LastUsed)"));
         }

         return lastReportName;
      }

      public void saveLastUseReport(PressureLossReportData data)
      {
         if (isReportFormatReadOnly())
            return;

         if (data == null)
            return;

         PressureLossReportData lastData = getLastUsedReportData();
         if (lastData != null)
            remove(lastData.Name);

         data.Name = data.Name + lastUsed;
         save(data);
      }
   }
}
