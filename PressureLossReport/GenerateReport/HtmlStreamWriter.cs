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
using Autodesk.Revit.DB.Mechanical;

namespace UserPressureLossReport
{
  public class HtmlStreamWriter
  {
    XmlWriter xmlWriter = null;
    string xmlFileName = "";
    string xsltFileName = "";

    public HtmlStreamWriter()
    {
      try
      {
        xmlFileName = System.IO.Path.GetDirectoryName( PressureLossReportHelper.instance.Doc.Application.RecordingJournalFilename );
        if( xmlFileName != null && xmlFileName.Length > 0 )
          xmlFileName = xmlFileName + "\\UserPressureLossReport" + DateTime.Now.Millisecond.ToString() + ".xml";

        string strPath = typeof( UserPressureLossReport.WholeReportSettingsDlg ).Assembly.Location;
        xsltFileName = Path.Combine(
          Path.GetDirectoryName( Path.GetDirectoryName( strPath ) ),
          "output", "UserPressureLossReport.xslt" );
        //xmlFileName = strPath + "\\UserPressureLossReport" + DateTime.Now.Millisecond.ToString() + ".xml";
        //xsltFileName = strPath + "\\UserPressureLossReport.xslt";
        xmlWriter = XmlWriter.Create( xmlFileName );
      }
      catch
      {
        File.Delete( xmlFileName );
      }
    }

    public string XmlFileName
    {
      get { return xmlFileName; }
    }

    public string XsltFileName
    {
      get { return xsltFileName; }
    }

    public void ConvertXML( string XMLfilePath, string XSLTFilePath, string HTMLfilePath )
    {
      XslCompiledTransform trans = new XslCompiledTransform();
      trans.Load( XSLTFilePath );

      trans.Transform( XMLfilePath, HTMLfilePath );
    }

    public string ConvertBytesToString( byte[] bytes )
    {
      string output = String.Empty;
      MemoryStream stream = new MemoryStream( bytes );
      stream.Position = 0;
      using( StreamReader reader = new StreamReader( stream ) )
      {
        output = reader.ReadToEnd();
      }
      return output;
    }

    public void WriteStartDocument( bool bWrite = false )
    {
      xmlWriter.WriteStartDocument( bWrite );
    }

    public void WriteStartElement( string str )
    {
      xmlWriter.WriteStartElement( str );
    }

    public void WriteElementString( string name, string val )
    {
      xmlWriter.WriteElementString( name, val );
    }

    public void WriteEndElement()
    {
      xmlWriter.WriteEndElement();
    }

    public void WriteEndDocument()
    {
      xmlWriter.WriteEndDocument();
    }

    public void Save( string fileName )
    {
      Close();
      ConvertXML( xmlFileName, xsltFileName, fileName );
      File.Delete( xmlFileName );
    }

    public void Close()
    {
      xmlWriter.Flush();
      xmlWriter.Close();
    }

    public void writeDataTable( DataTable tb, bool bReplaceColumn = false )
    {
      if( xmlWriter == null || tb == null )
        return;

      MemoryStream stream = new MemoryStream();
      tb.WriteXml( stream );
      string str = ConvertBytesToString( stream.ToArray() );
      str = str.Replace( "<DocumentElement>", "" );
      str = str.Replace( "</DocumentElement>", "" );

      if( bReplaceColumn )
      {
        for( int ii = 1; ii < tb.Columns.Count + 1; ++ii )
        {
          string strCol = "<Column" + ii.ToString() + ">";
          string strCol2 = "</Column" + ii.ToString() + ">";
          str = str.Replace( strCol, "<Column>" );
          str = str.Replace( strCol2, "</Column>" );
        }
      }
      xmlWriter.WriteRaw( str );
    }

    public void writeTableTitle( string nodeType, List<string> strFields, bool bForSection = false )
    {
      if( xmlWriter == null || nodeType == null || strFields == null )
        return;

      string str = nodeType + "Info";
      xmlWriter.WriteStartElement( str );

      xmlWriter.WriteElementString( "SectionNumber", ReportResource.section );

      if( !bForSection )
        xmlWriter.WriteElementString( nodeType + "TotalPressuerLoss", ReportResource.totalPressureLoss );
      else
        xmlWriter.WriteElementString( nodeType + "TotalPressuerLoss", ReportResource.sectionPressureLoss );

      DataTable sectionDetailInfoTBTitle = new DataTable( nodeType + "DetailInfo" );
      PressureLossReportHelper.instance.getTableTitle( sectionDetailInfoTBTitle, strFields, true );
      writeDataTable( sectionDetailInfoTBTitle, true );

      xmlWriter.WriteEndElement();
    }

    public void writeTable( string nodeType, DataTable tb, string number, string totalPL )
    {
      if( tb == null || nodeType == null || number == null || totalPL == null )
        return;

      WriteStartElement( nodeType + "Info" );
      WriteElementString( "SectionNumber", number );
      WriteElementString( nodeType + "TotalPressuerLoss", totalPL );
      writeDataTable( tb, true );
      WriteEndElement();
    }
  }
}
