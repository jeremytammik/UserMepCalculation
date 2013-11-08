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
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;

namespace UserPressureLossReport
{
   public enum SectionMemberType
   {
      Invalid = 0,
      Segment = 1,
      Fitting = 2,
      AirTerminal = 4,
      Section = 8,
      Type = 16,
      Equipment = 32
   }

   public static class ReportConstants
   {
      public static string emptyValue = "-";
      public static string mergeValue = "~";
      public static string failed_to_delete = "failed_to_delete";
   }

   public class SortSystemTypeByNameCamparer : IComparer<MEPSystemType>
   {
      #region IComparer Members
      public int Compare(MEPSystemType obj1, MEPSystemType obj2)
      {
         if ((obj1 == null) && (obj2 == null))
            return 0;
         else if ((obj1 != null) && (obj2 == null))
            return 1;
         else if ((obj1 == null) && (obj2 != null))
            return -1;

         return obj1.Name.CompareTo(obj2.Name);
      }
      #endregion
   }

   public class SortSystemByNameCamparer : IComparer<MEPSystem>
   {
      #region IComparer Members
      public int Compare(MEPSystem obj1, MEPSystem obj2)
      {
         if ((obj1 == null) && (obj2 == null))
            return 0;
         else if ((obj1 != null) && (obj2 == null))
            return 1;
         else if ((obj1 == null) && (obj2 != null))
            return -1;

         return obj1.Name.CompareTo(obj2.Name);
      }
      #endregion
   }

   class PressureLossReportHelper
   {
      public static PressureLossReportHelper instance = new PressureLossReportHelper();

      private Autodesk.Revit.DB.Document doc;
      private string domain;
      private ElementSet selElems = new ElementSet();
      private PressureLossReportData reportData;

      private PressureLossReportHelper() { }
      public void initialize(Autodesk.Revit.DB.Document document, ElementSet allSelElems, string strDomain)
      {
         if (document != null)
            doc = document;

         if (allSelElems != null)
            selElems = allSelElems;

         domain = ReportResource.ductDomain;
         SystemClassification = MEPSystemClassification.UndefinedSystemClassification;
         domain = strDomain;

      }

      public string Domain
      {
         get { return domain; }
         set { domain = value; }
      }

      public MEPSystemClassification SystemClassification
      {
         get;
         set;
      }

      public Autodesk.Revit.DB.Document Doc
      {
         get { return doc; }
      }

      public PressureLossReportData ReportData
      {
         get { return reportData; }
         set { reportData = value; }
      }

      public bool getSelectedSystems(ElementSet selSystems)
      {
         if (doc == null)
            return false;

         if (selElems.Size > 0)
         {
            //check if the selected element is system
            foreach (Element elem in selElems)
            {
               if (elem == null)
                  continue;

               MEPSystem mepSys = elem as MEPSystem;
               if (mepSys == null || !isValidSystem(doc, mepSys, Domain))
                  continue;

               selSystems.Insert(elem);
            }

            if (selSystems.Size > 0)
               return true;            
         }

         return false;
      }

      public List<MEPSystemType> getSelectedSystemTypes(bool bOnlySelectedSystemType = false)
      {
         if (doc == null)
            return null;

         if (selElems.Size > 0)
         {
            List<MEPSystemType> sysTypes = new List<MEPSystemType>();
            foreach (Element elem in selElems)
            {
               if (elem == null)
                  continue;

               MEPSystemType mepSysType = null;

               if (!bOnlySelectedSystemType)
               {
                  MEPSystem mepSys = elem as MEPSystem;
                  if (mepSys != null && isValidSystem(doc, mepSys, Domain))
                     mepSysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;
               }

               if (mepSysType == null)
                  mepSysType = elem as MEPSystemType;
               if (mepSysType == null || !isValidSystemType(mepSysType))
                  continue;

               if (!isSystemTypeInList(sysTypes, mepSysType))
                  sysTypes.Add(mepSysType);
            }

            return sysTypes;
         }

         return null;
      }

      public bool isSelectInValidSystemType()
      {
         if (doc == null)
            return false;

         if (selElems.Size > 0)
         {
            MEPSystemType mepSysType = null;
            foreach (Element elem in selElems)
            {
               if (elem == null)
                  continue;

               mepSysType = null;

               MEPSystem mepSys = elem as MEPSystem;
               if (mepSys != null && isValidSystem(doc, mepSys, Domain))
                  mepSysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;

               if (mepSysType == null)
                  mepSysType = elem as MEPSystemType;
               if (mepSysType == null) 
                  continue;
               if (!isValidSystemType(mepSysType))
                  return true;
            }
         }

         return false;
      }

      static public bool isSystemTypeInList(List<Autodesk.Revit.DB.MEPSystemType> mepSysTypes, Autodesk.Revit.DB.MEPSystemType mepSysType)
      {
         for (int ii = 0; ii < mepSysTypes.Count; ++ii)
         {
            if (mepSysType.Id == mepSysTypes[ii].Id)
            {
               return true;
            }
         }
         return false;
      }

      public List<MEPSystem> getSortedSystems()
      {
         ElementSet selSystems = new ElementSet();
         if (getSelectedSystems(selSystems))
         {
            List<MEPSystem> sortedSystems = new List<MEPSystem>();
            SortedDictionary<string, MEPSystem> systems = new SortedDictionary<string, MEPSystem>();

            foreach (MEPSystem sys in selSystems)
            {
               if (sys == null)
                  continue;

               string sysName = sys.Name;
               if (sysName != null)
                  systems.Add(sysName, sys);
            }

            foreach (KeyValuePair<string, MEPSystem> kvp in systems)
            {
               sortedSystems.Add(kvp.Value);
            }

            return sortedSystems;
         }

         return null;
      }

      public void getFieldsFromReportdata(List<PressureLossParameter> avaliableParams, List<string> strFields)
      {
         if (avaliableParams == null || strFields == null)
            return;

         SortedDictionary<int, string> displayFields = new SortedDictionary<int, string>();

         foreach (PressureLossParameter param in avaliableParams)
         {
            if (param.Selected == true)
            {
               //if displayFields already contains the key, but not contains the value, reset the display order  
               if (displayFields.ContainsKey(param.DisplayOrder) && !displayFields.ContainsValue(param.Name))
                  param.DisplayOrder = displayFields.Keys.Max() + 1;

               displayFields.Add(param.DisplayOrder, param.Name);
            }
         }

         foreach (KeyValuePair<int, string> kvp in displayFields)
            strFields.Add(kvp.Value);
      }

      public string getParamValue(Parameter param, bool bUseEmptySymbol = true)
      {
         string strVal = ReportConstants.emptyValue;
         if (!bUseEmptySymbol)
            strVal = "";

         if (param == null)
            return strVal;

         if (param.HasValue)
         {
            if (param.StorageType == StorageType.String)
               strVal = param.AsString();
            else if (param.StorageType == StorageType.Integer)
            {
               if (param.Definition.ParameterType == ParameterType.YesNo)
               {
                  strVal = ReportResource.no;
                  if (param.AsInteger() == 1)
                     strVal = ReportResource.yes;
               }
               else
                  strVal = param.AsInteger().ToString();
            }
            else if (param.StorageType == StorageType.Double)
            {
               strVal = param.AsValueString();
               if (strVal == null || strVal.Length < 1)
                  strVal = FormatUtils.Format(doc, UnitType.UT_Number, param.AsDouble());
            }
            else if (param.StorageType == StorageType.ElementId)
            {
               //get the element name
               ElementId id = param.AsElementId();
               strVal = ReportResource.none;
               if (id.IntegerValue != -1)
               {
                  Element elem = doc.GetElement(id);
                  if (elem != null)
                     strVal = elem.Name;
               }
            }
         }

         return strVal;
      }

      public void addParameterNameAndValueToTable(DataTable tb, Parameter param, bool bUseEmptySymbol = true)
      {
         if (tb == null || param == null)
            return;

         string strVal = getParamValue(param, bUseEmptySymbol);
         tb.Rows.Add(param.Definition.Name, strVal);
      }

      public PressureLossParameter getPressureLossParamByName(List<PressureLossParameter> PLParams, string paramName)
      {
         if (PLParams == null || paramName == null)
            return null;

         foreach (PressureLossParameter param in PLParams)
         {
            if (param.Name == paramName)
               return param;
         }

         return null;
      }

      public string getTotalPressureLossByType(MEPSection section, SectionMemberType eType)
      {
         string strVal = "";
         if (section != null)
         {
            double dVal = 0.0;
            if (eType == SectionMemberType.Segment)
            {
               List<MEPCurve> curves = new List<MEPCurve>();
               SectionsInfo.getSectionElements(section, curves, null, null, null);
               foreach (MEPCurve crv in curves)
                  dVal += section.GetPressureDrop(crv.Id);
            }
            else if (eType == SectionMemberType.Fitting)
            {
               List<FamilyInstance> fittings = new List<FamilyInstance>();
               SectionsInfo.getSectionElements(section, null, fittings, null, null);
               foreach (FamilyInstance famInst in fittings)
                  dVal += section.GetPressureDrop(famInst.Id);
            }
            else if (eType == SectionMemberType.AirTerminal)
            {
               List<FamilyInstance> airTerminals = new List<FamilyInstance>();
               SectionsInfo.getSectionElements(section, null, null, airTerminals, null);
               foreach (FamilyInstance famInst in airTerminals)
                  dVal += section.GetPressureDrop(famInst.Id);
            }
            else if (eType == SectionMemberType.Equipment)
            {
               List<FamilyInstance> equipments = new List<FamilyInstance>();
               SectionsInfo.getSectionElements(section, null, null, null, equipments);
               foreach (FamilyInstance famInst in equipments)
                  dVal += section.GetPressureDrop(famInst.Id);
            }

            if (eType == SectionMemberType.Section)
               dVal = section.TotalPressureLoss;

            if (domain == ReportResource.pipeDomain)
               strVal = FormatUtils.Format(doc, UnitType.UT_Piping_Pressure, dVal);
            else
               strVal = FormatUtils.Format(doc, UnitType.UT_HVAC_Pressure, dVal);

         }
         return strVal;
      }

      public void generateAviliableFields(PressureLossReportData reportData)
      {
         if (doc == null)
            return;

         SectionsInfo.generateSectionFields(reportData);
         SegmentsInfo.generateSegmentFields(reportData);
         FittingsInfo.generateFittingFields(reportData);
      }

      public void getTableTitle(DataTable tb, List<string> strings, bool forHTML = false, bool bForSection = false)
      {
         if (tb == null || strings == null)
            return;

         tb.Clear();
         tb.Columns.Clear();
         tb.Rows.Clear();

         int nAllCol = strings.Count + 2;

         if (forHTML)
            nAllCol = nAllCol - 2;

         object[] testVal = new object[nAllCol];

         if (!forHTML)
         {
            tb.Columns.Add();
            testVal[0] = ReportResource.section;
         }

         int ii = 1;
         if (forHTML)
            ii = ii - 1;
         foreach (string str in strings)
         {
            testVal[ii] = str;
            tb.Columns.Add();
            ii++;
         }
         if (!forHTML)
         {
            tb.Columns.Add();
            if (bForSection)
               testVal[strings.Count + 1] = ReportResource.sectionPressureLoss;
            else
               testVal[strings.Count + 1] = ReportResource.totalPressureLoss;
         }

         tb.Rows.Add(testVal);
      }

      public void addColumns(DataTable tb, int nCol)
      {
         if (tb == null)
            return;

         tb.Clear();
         tb.Columns.Clear();
         tb.Rows.Clear();

         for (int ii = 0; ii < nCol; ++ii)
            tb.Columns.Add();
      }

      public void addRow(DataTable tb, List<string> values)
      {
         if (tb == null)
            return;

         int nAllCol = tb.Columns.Count;

         object[] testVal = new object[nAllCol];

         for (int ii = 0; ii < nAllCol; ++ii)
         {
            if (values[ii] == null || values[ii].Length < 1)
               testVal[ii] = ReportConstants.emptyValue;
            else
               testVal[ii] = values[ii];
         }

         tb.Rows.Add(testVal);
      }

      public bool hasValidSystem(Document doc, ElementSet elems, string strDomain)
      {
         if (elems == null || doc == null || strDomain == null)
            return false;

         bool bHasValidSystem = false;
         foreach (Element elem in elems)
         {
            if (elem == null)
               continue;

            MEPSystem mepSys = elem as MEPSystem;
            if (!PressureLossReportHelper.instance.isValidSystem(doc, mepSys, strDomain))
               continue;
            
            bHasValidSystem = true;
            break;
         }

         return bHasValidSystem;
      }

      public bool isValidSystem(Document doc, MEPSystem mepSys, string strDomain)
      {
         if (mepSys == null || doc == null)
            return false;

         if (mepSys.IsEmpty || !mepSys.IsValid)
            return false;

         Category category = mepSys.Category;
         BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;
         if (strDomain == ReportResource.pipeDomain && enumCategory != BuiltInCategory.OST_PipingSystem)
            return false;

         if (strDomain == ReportResource.ductDomain && enumCategory != BuiltInCategory.OST_DuctSystem)
            return false;

         MEPSystemType sysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;
         if (sysType == null)
            return false;

         return isValidSystemType(sysType);
      }

      public MEPSystemType getSystemType(Document doc, MEPSystem mepSys)
      {
         MEPSystemType sysType = doc.GetElement(mepSys.GetTypeId()) as MEPSystemType;
         return sysType;
      }

      public bool isValidSystemType(MEPSystemType sysType)
      {
         if (sysType == null)
            return false;

         if (sysType.SystemClassification == MEPSystemClassification.OtherPipe ||
             sysType.SystemClassification == MEPSystemClassification.Vent ||
             sysType.SystemClassification == MEPSystemClassification.FireProtectWet || sysType.SystemClassification == MEPSystemClassification.FireProtectDry ||
             sysType.SystemClassification == MEPSystemClassification.FireProtectPreaction || sysType.SystemClassification == MEPSystemClassification.FireProtectOther ||
             sysType.SystemClassification == MEPSystemClassification.Sanitary)
            return false;
         return true;
      }

      public string getFamilyOrTypeName(ElementId id, string paramName)
      {
         string paramVal = "";

         if (doc == null)
            return paramVal;

         if (!(paramName == LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_PARAM)
            || paramName == LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM)
            ))
            return null;

         Element elem = doc.GetElement(id);
         if (elem == null)
            return paramVal;

         ElementType elemType = doc.GetElement(elem.GetTypeId()) as ElementType;
         if (elemType == null)
            return paramVal;

         if (paramName == LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_PARAM))
         {
            paramVal = getParamValue(elemType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM));
         }
         else if (paramName == LabelUtils.GetLabelFor(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM))
         {
            paramVal = getParamValue(elemType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM));
         }

         return paramVal;
      }


      public ICollection<Element> getCategoryElements(Autodesk.Revit.DB.BuiltInCategory categoryId)
      {
         return getElementOrType(categoryId, true);
      }

      public ICollection<Element> getCategoryTypeElements(Autodesk.Revit.DB.BuiltInCategory categoryId)
      {
         return getElementOrType(categoryId, false);
      }

      private ICollection<Element> getElementOrType(Autodesk.Revit.DB.BuiltInCategory categoryId, bool bElement)
      {
         FilteredElementCollector collector = new FilteredElementCollector(doc);
         ElementCategoryFilter categoryFilter = new ElementCategoryFilter(categoryId);
         LogicalAndFilter categoryTypeFilter = new LogicalAndFilter(categoryFilter, new ElementIsElementTypeFilter(bElement));
         return collector.WherePasses(categoryTypeFilter).ToElements();
      }
   }
}
