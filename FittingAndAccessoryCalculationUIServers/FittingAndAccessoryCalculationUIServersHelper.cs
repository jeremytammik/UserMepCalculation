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

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Mechanical;
using Autodesk.Revit.UI.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.ExternalService;

namespace UserFittingAndAccessoryCalculationUIServers
{
   // Helper class for calculation.
   public static class CalculationUtility
   {
      // The maximum items in the history in the settings dialog.
      public static readonly int MAX_ITEMS_IN_HISTORY = 8;
      public static readonly double tol = 1e-9;

      public static bool IsNumeric(string strInput)
      {
         if (string.IsNullOrEmpty(strInput))
            return false;

         try
         {
            double dOutput = Convert.ToDouble(strInput);
         }
         catch (System.Exception /*ex*/)
         {
            return false;
         }

         return true;
      }

      /// <summary>
      /// Gets initial items for ComboBox in settings dialog. 
      /// </summary>
      /// <param name="history">
      /// The list containing the settings history.
      /// </param>
      /// <param name="initialValue">
      /// The initial value to be add to ComboBox.
      /// </param>
      /// <returns>
      /// Initial items for ComboBox.
      /// </returns>
      public static string[] GetInitialItemsForComboBox(List<string> history, string initialValue)
      {
         List<string> copyHistory = new List<string>(history.ToArray());

         if (copyHistory.Contains(initialValue))
            copyHistory.Remove(initialValue);

         copyHistory.Insert(0, initialValue);

         return copyHistory.ToArray();
      }

      /// <summary>
      /// Gets initial items for ComboBox in settings dialog. 
      /// </summary>
      /// <param name="history">
      /// The list containing the settings history.
      /// </param>
      /// <param name="initialValue">
      /// The initial value to be add to ComboBox.
      /// </param>
      /// <returns>
      /// Initial numeric items for ComboBox.
      /// </returns>
      public static string[] GetInitialItemsForComboBox(List<double> history, double initialValue, Units unit, UnitType unitType)
      {
         UpdateHistory(history, initialValue);

         List<string> strCopyHistory = new List<string>();
         foreach (double item in history)
         {
            if (double.IsNaN(item))
               continue;

            strCopyHistory.Add(UnitFormatUtils.FormatValueToString(unit, unitType, item, false, false));
         }
         return strCopyHistory.ToArray();
      }

      /// <summary>
      /// Updates the setting history in settings dialog. 
      /// </summary>
      /// <param name="history">
      /// The list containing the setting history.
      /// </param>
      /// <param name="newValue">
      /// The new value to be added to the history.
      /// </param>
      public static void UpdateHistory(List<string> history, string newValue)
      {
         if (!string.IsNullOrEmpty(newValue))
         {
            int index = history.IndexOf(newValue);
            if (index != -1) // The new value is already in the history, just move it to the first location in the history.
            {
               history.RemoveAt(index);
               history.Insert(0, newValue);
            }
            else // The new value is not in the history, insert it at the first location.
            {
               history.Insert(0, newValue);
               if (history.Count > MAX_ITEMS_IN_HISTORY) // If the items in the history exceeds the maximum count, just remove the oldest item.
                  history.RemoveAt(history.Count - 1);
            }
         }
      }

      /// <summary>
      /// Updates the setting history in settings dialog. 
      /// </summary>
      /// <param name="history">
      /// The list containing the setting history.
      /// </param>
      /// <param name="newValue">
      /// The new value to be added to the history.
      /// </param>
      public static void UpdateHistory(List<double> history, double newValue)
      {
         if (!double.IsNaN(newValue))
         {
            int index = -1;
            foreach (double item in history)
            {
               if (Math.Abs(item - newValue) < tol)
               {
                  index = history.IndexOf(item);
                  break;
               }
            }
            if (index != -1) // The new value is already in the history, just move it to the first location in the history.
            {
               history.RemoveAt(index);
               history.Insert(0, newValue);
            }
            else // The new value is not in the history, insert it at the first location.
            {
               history.Insert(0, newValue);
               if (history.Count > MAX_ITEMS_IN_HISTORY) // If the items in the history exceeds the maximum count, just remove the oldest item.
                  history.RemoveAt(history.Count - 1);
            }
         }
      }

      /// <summary>
      /// Shows a task dialog with warning message.
      /// </summary>
      /// <param name="title">
      /// The title of the task dialog.
      /// </param>
      /// <param name="instruction">
      /// The large primary text that appears at the top of the task dialog.
      /// </param>
      /// <param name="content">
      /// The smaller text that appears just below the main instructions.
      /// </param>
      public static TaskDialogResult PostWarning(string title, string instruction, string content = null)
      {
         if (title == null || instruction == null)
            return TaskDialogResult.None;

         TaskDialog tDlg = new TaskDialog(title);
         tDlg.MainInstruction = instruction;
         tDlg.MainContent = content;
         tDlg.AllowCancellation = true;
         tDlg.CommonButtons = TaskDialogCommonButtons.Close;
         tDlg.DefaultButton = TaskDialogResult.Close;
         tDlg.TitleAutoPrefix = false;
         return tDlg.Show();
      }

      /// <summary>
      /// Gets the initial value for the duct settings dialog
      /// </summary>
      /// <param name="data">
      /// The duct fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="schemaField">
      /// The schema field which is used to get the initial value.
      /// </param>
      /// <returns>
      /// The initial value which will be shown in the settings dialog.
      /// </returns>
      public static string GetInitialValue(DuctFittingAndAccessoryPressureDropUIData data, string schemaField)
      {
         string initialValue = String.Empty;

         IList<DuctFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (DuctFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {
            string tmpValue = String.Empty;

            Entity entity = uiDataItem.GetEntity();
            if (entity != null && entity.IsValid())
            {
               tmpValue = entity.Get<string>(schemaField);
            }
            else // If only one entity is null or invalid, the initial value should be empty.
            {
               return String.Empty;
            }

            if (uiDataItems.IndexOf(uiDataItem) == 0) // The first element
               initialValue = tmpValue;
            else
            {
               if (tmpValue != initialValue) // If all elements don't have the same old values, the settings dialog will show empty initial value.
               {
                  initialValue = String.Empty;
                  break;
               }
            }
         }

         return initialValue;
      }

      /// <summary>
      /// Gets the initial value for the settings dialog
      /// </summary>
      /// <param name="data">
      /// The pipe fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="dbServerId">
      /// The corresponding DB server Id of the UI server.
      /// </param>
      /// <param name="schemaField">
      /// The schema field which is used to get the initial value.
      /// </param>
      /// <returns>
      /// The initial value which will be shown in the settings dialog.
      /// </returns>
      public static string GetInitialValue(PipeFittingAndAccessoryPressureDropUIData data, string schemaField)
      {
         string initialValue = String.Empty;

         IList<PipeFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (PipeFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {
            string tableName = String.Empty;

            Entity entity = uiDataItem.GetEntity();
            if (entity != null && entity.IsValid())
            {
               tableName = entity.Get<string>(schemaField);
            }
            else // If only one entity is null or invalid, the initial value should be empty.
            {
               return String.Empty;
            }

            if (uiDataItems.IndexOf(uiDataItem) == 0) // The first element
               initialValue = tableName;
            else
            {
               if (tableName != initialValue) // If all elements don't have the same old values, the settings dialog will show empty initial value.
               {
                  initialValue = String.Empty;
                  break;
               }
            }

         }

         return initialValue;
      }

      /// <summary>
      /// Gets the initial value for the settings dialog
      /// </summary>
      /// <param name="data">
      /// The duct fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="dbServerId">
      /// The corresponding DB server Id of the UI server.
      /// </param>
      /// <param name="schemaField">
      /// The schema field which is used to get the initial value.
      /// </param>
      /// <returns>
      /// The initial numeric value which will be shown in the settings dialog.
      /// </returns>
      public static double GetInitialNumericValue(DuctFittingAndAccessoryPressureDropUIData data, string schemaField)
      {
         double initialValue = 0.0;

         IList<DuctFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (DuctFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {
            string value = String.Empty;

            Entity entity = uiDataItem.GetEntity();
            if (entity != null && entity.IsValid())
            {
               value = entity.Get<string>(schemaField);
            }
            else
            {
               return double.NaN;
            }

            if (CalculationUtility.IsNumeric(value))
            {
               double dValue = Convert.ToDouble(value);
               if (uiDataItems.IndexOf(uiDataItem) == 0) // The first element
               {
                  initialValue = dValue;
               }
               else
               {
                  if (!dValue.Equals(initialValue)) // If all elements don't have the same old values, the settings dialog will show empty initial value.
                  {
                     initialValue = double.NaN;
                     break;
                  }
               }
            }
         }

         return initialValue;
      }

      /// <summary>
      /// Gets the initial value for the settings dialog
      /// </summary>
      /// <param name="data">
      /// The duct fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="dbServerId">
      /// The corresponding DB server Id of the UI server.
      /// </param>
      /// <param name="schemaField">
      /// The schema field which is used to get the initial value.
      /// </param>
      /// <returns>
      /// The initial numeric value which will be shown in the settings dialog.
      /// </returns>
      public static double GetInitialNumericValue(PipeFittingAndAccessoryPressureDropUIData data, string schemaField)
      {
         double initialValue = 0.0;

         IList<PipeFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (PipeFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {
            string value = String.Empty;

            Entity entity = uiDataItem.GetEntity();
            if (entity != null && entity.IsValid())
            {
               value = entity.Get<string>(schemaField);
            }
            else
            {
               return double.NaN;
            }

            if (CalculationUtility.IsNumeric(value))
            {
               double dValue = Convert.ToDouble(value);
               if (uiDataItems.IndexOf(uiDataItem) == 0) // The first element
               {
                  initialValue = dValue;
               }
               else
               {
                  if (!dValue.Equals(initialValue)) // If all elements don't have the same old values, the settings dialog will show empty initial value.
                  {
                     initialValue = double.NaN;
                     break;
                  }
               }
            }
         }

         return initialValue;
      }

      /// <summary>
      /// Updates the entity in the duct fitting and accessory pressure drop UI data. 
      /// </summary>
      /// <param name="data">
      /// The duct fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="dbServerId">
      /// The corresponding DB server Id of the UI server.
      /// </param>
      /// <param name="schemaField">
      /// The schema field to be updated.
      /// </param>
      /// <param name="newValue">
      /// The new value to be set to the schema field.
      /// </param>
      /// <returns>
      /// True if the entity in the UI data is updated, false otherwise.
      /// </returns>
      public static bool UpdateEntities(DuctFittingAndAccessoryPressureDropUIData data, Guid dbServerId, string schemaField, string newValue)
      {
         bool isUpdated = false;

         ExternalService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropService) ;
         if (service == null)
            return isUpdated;

         IDuctFittingAndAccessoryPressureDropServer dbServer = service.GetServer(dbServerId) as IDuctFittingAndAccessoryPressureDropServer;
         if (dbServer == null)
         {
            return isUpdated;
         }

         Schema schema = dbServer.GetDataSchema();
         if (schema == null)
            return isUpdated;

         Field field = schema.GetField(schemaField);
         if (field == null)
            return isUpdated;

         Entity entity = new Entity(schema);
         entity.Set<string>(field, newValue);

         IList<DuctFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (DuctFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {

            Entity oldEntity = uiDataItem.GetEntity();
            if (oldEntity == null && entity == null)
            {
               continue;
            }

            if (oldEntity == null || entity == null)
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }

            if ((!oldEntity.IsValid()) && (!entity.IsValid()))
            {
               continue;
            }

            if ((!oldEntity.IsValid()) || (!entity.IsValid()))
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }

            string oldValue = oldEntity.Get<string>(schemaField);
            if (oldValue != newValue)
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }
         }

         return isUpdated;
      }

      /// <summary>
      /// Updates the entity in the pipe fitting and accessory pressure drop UI data. 
      /// </summary>
      /// <param name="data">
      /// The pipe fitting and accessory pressure drop UI data.
      /// </param>
      /// <param name="dbServerId">
      /// The corresponding DB server Id of the UI server.
      /// </param>
      /// <param name="schemaField">
      /// The schema field to be updated.
      /// </param>
      /// <param name="newValue">
      /// The new value to be set to the schema field.
      /// </param>
      /// <returns>
      /// True if the entity in the UI data is updated, false otherwise.
      /// </returns>
      public static bool UpdateEntities(PipeFittingAndAccessoryPressureDropUIData data, Guid dbServerId, string schemaField, string newValue)
      {
         bool isUpdated = false;

         ExternalService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropService);
         if (service == null)
            return isUpdated;

         IPipeFittingAndAccessoryPressureDropServer dbServer = service.GetServer(dbServerId) as IPipeFittingAndAccessoryPressureDropServer;
         if (dbServer == null)
         {
            return isUpdated;
         }

         Schema schema = dbServer.GetDataSchema();
         if (schema == null)
            return isUpdated;

         Field field = schema.GetField(schemaField);
         if (field == null)
            return isUpdated;

         Entity entity = new Entity(schema);
         entity.Set<string>(field, newValue);

         IList<PipeFittingAndAccessoryPressureDropUIDataItem> uiDataItems = data.GetUIDataItems();
         foreach (PipeFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {

            Entity oldEntity = uiDataItem.GetEntity();
            if (oldEntity == null && entity == null)
            {
               continue;
            }

            if (oldEntity == null || entity == null)
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }

            if ((!oldEntity.IsValid()) && (!entity.IsValid()))
            {
               continue;
            }

            if ((!oldEntity.IsValid()) || (!entity.IsValid()))
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }

            string oldValue = oldEntity.Get<string>(schemaField);
            if (oldValue != newValue)
            {
               uiDataItem.SetEntity(entity);
               isUpdated = true;
               continue;
            }
         }

         return isUpdated;
      }

      /// <summary>
      /// Checks if all pipe fittings or pipe accessories selected have the same PipeKFactorPartType. 
      /// See FittingAndAccessoryCalculationManaged.PipeKFactorPartType
      /// </summary>
      /// <param name="uiDataItems">
      /// The pipe fitting and accessory pressure drop UI data items.
      /// </param>
      /// <returns>
      /// True if all pipe fittings or pipe accessories selected have the same PipeKFactorPartType, false otherwise.
      /// </returns>
      public static bool HasSamePipeKFactorPartType(IList<PipeFittingAndAccessoryPressureDropUIDataItem> uiDataItems)
      {
         /*
         int count = 0;
         FittingAndAccessoryCalculationManaged.PipeKFactorPartType pipeKFactorPartType = FittingAndAccessoryCalculationManaged.PipeKFactorPartType.kUndefinedPipePartType;

         foreach (PipeFittingAndAccessoryPressureDropUIDataItem uiDataItem in uiDataItems)
         {
            PipeFittingAndAccessoryData fittingData = uiDataItem.GetPipeFittingAndAccessoryData();
            if (fittingData == null)
               return false;

            FittingAndAccessoryCalculationManaged.PipeKFactorPartType iterPipeKFactorPartType = FittingAndAccessoryCalculationManaged.KFactorTablePipePressureDropCalculator.getKFactorPartType(fittingData.PartType, fittingData.BehaviorType);
            if (pipeKFactorPartType != iterPipeKFactorPartType)
            {
               pipeKFactorPartType = iterPipeKFactorPartType;
               count++;

               if (count > 1)
                  return false;
            }
         }
         */
         return true;
      }
   }
}
