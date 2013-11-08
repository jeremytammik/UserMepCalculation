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
   public class PressureLossReportFormats : List<PressureLossReportData>
   {
      //Intended to be empty for now.
   }

   [Serializable]
   public class PressureLossParameter
   {
      private string name;
      private bool selected;
      private int displayOrder;
      private int getFrom; //SectionMemberType
      private bool display;

      public PressureLossParameter()
      {
         name = "";
         selected = false;
         displayOrder = -1;
         getFrom = 0;
         display = true;
      }
      public PressureLossParameter(Parameter _pa)
      {
         name = _pa.Definition.Name;
         selected = false;
         displayOrder = -1;
         getFrom = 0;
         display = true;
      }

      public PressureLossParameter(string strName)
      {
         name = strName;
         selected = false;
         displayOrder = -1;
         getFrom = 0;
         display = true;
      }

      public PressureLossParameter(string strName, bool bSelected)
      {
         name = strName;
         selected = bSelected;
         displayOrder = -1;
         getFrom = 0;
         display = true;
      }

      public PressureLossParameter(string strName, bool bSelected, int nDisplayOrder, int nGetFrom = 0, bool bDisplay = true)
      {
         name = strName;
         selected = bSelected;
         displayOrder = nDisplayOrder;
         getFrom = nGetFrom;
         display = bDisplay;
      }

      public override bool Equals(object obj)
      {
         if (obj is PressureLossParameter)
         {
            PressureLossParameter param = obj as PressureLossParameter;
            if (param != null)
            {
               return (0 == string.Compare(param.Name, name));
            }
            return false;
         }
         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public string Name
      {
         get { return name; }
         set { name = value; }
      }

      public bool Selected
      {
         get { return selected; }
         set { selected = value; }
      }

      public int DisplayOrder
      {
         get { return displayOrder; }
         set { displayOrder = value; }
      }

      public int GetFrom
      {
         get { return getFrom; }
         set { getFrom = value; }
      }

      public bool Display
      {
         get { return display; }
         set { display = value; }
      }
   }

   public class PressureLossReportData
   {
      public static int DataVersion = 4;

      private List<PressureLossParameter> availableFields;
      private List<PressureLossParameter> straightSegFields;
      private List<PressureLossParameter> fittingFields;

      public PressureLossReportData()
      {
         Version = DataVersion;
         DisplayRunTime = true;
      }

      public override bool Equals(object obj)
      {
         if (obj is PressureLossReportData)
         {
            PressureLossReportData data = obj as PressureLossReportData;
            if (data != null)
            {
               return (0 == string.Compare(data.Name, Name) && Domain == data.Domain);
            }
            return false;
         }
         return base.Equals(obj);
      }
      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public string Name
      {
         get;
         set;
      }

      public string Domain
      {
         get;
         set;
      }

      public int Version
      {
         get;
         set;
      }

      public bool DisplaySysInfo
      {
         get;
         set;
      }

      public bool DisplayCriticalPath
      {
         get;
         set;
      }

      public bool DisplayDetailInfoForStraightSeg
      {
         get;
         set;
      }

      public bool DisplayFittingLCSum
      {
         get;
         set;
      }

      public bool OpenAfterCreated
      {
         get;
         set;
      }

      public bool DisplayRunTime
      {
         get;
         set;
      }

      public List<PressureLossParameter> AvailableFields
      {
         get { return availableFields; }
         set { availableFields = value; }
      }

      public List<PressureLossParameter> StraightSegFields
      {
         get { return straightSegFields; }
         set { straightSegFields = value; }
      }

      public List<PressureLossParameter> FittingFields
      {
         get { return fittingFields; }
         set { fittingFields = value; }
      }
   }
}
