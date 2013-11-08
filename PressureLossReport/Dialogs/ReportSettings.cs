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
using System.Windows.Forms;
using Autodesk.Revit.UI;


namespace UserPressureLossReport
{
   public enum SaveType
   {
      HTML = 0,
      CSV
   }

   public class UIHelperFunctions
   {
      public static void addRemoveFields(ListBox listBoxToRemove, ListBox listBoxToAdd)
      {
         if (listBoxToRemove == null || listBoxToAdd == null)
            return;

         if (listBoxToRemove.SelectedItems.Count < 1)
         {
            return;
         }
         else
         {
            List<string> selTexts = new List<string>();
            listBoxToAdd.ClearSelected();
            foreach (object obj in listBoxToRemove.SelectedItems)
            {
               int nIndex = listBoxToAdd.Items.Add(obj);
               listBoxToAdd.SetSelected(nIndex, true);
               selTexts.Add(obj.ToString());
            }            

            foreach (string selText in selTexts)
            {
               listBoxToRemove.Items.Remove(selText);
            }

            listBoxToRemove.ClearSelected();            
         }
      }

      public static void fillingListBoxFields(List<PressureLossParameter> avaliableParams, ListBox listBoxUnSelected, ListBox listBoxSelected)
      {
         if (avaliableParams == null || listBoxSelected == null || listBoxUnSelected == null)
            return;

         SortedDictionary<int, string> displayFields = new SortedDictionary<int, string>();

         foreach (PressureLossParameter param in avaliableParams)
         {
            if (param.Selected == true && param.Display == true)
            {
               displayFields.Add(param.DisplayOrder, param.Name);
            }
            else if (param.Display == true)
               listBoxUnSelected.Items.Add(param.Name);
         }

         foreach (KeyValuePair<int, string> kvp in displayFields)
         {
            listBoxSelected.Items.Add(kvp.Value);
         }

      }

      public static void getFieldsFromSelectedListBox(List<PressureLossParameter> avaliableParams, ListBox listBoxSelected)
      {
         if (avaliableParams == null || listBoxSelected == null)
            return;

         foreach (PressureLossParameter param in avaliableParams)
         {
            if (listBoxSelected.Items.Contains(param.Name))
            {
               param.Selected = true;
               param.DisplayOrder = listBoxSelected.Items.IndexOf(param.Name);
            }
            else if (param.Display == false && param.Selected == true)
            {
               continue;
            }
            else
            {
               param.Selected = false;
               param.DisplayOrder = -1;
            }
         }
      }

      public static void moveSelectedField(ListBox listBoxSelected, bool bUp)
      {
         if (listBoxSelected == null)
            return;

         int nFirstSelndex = -1;
         int nSelCount = 0;
         if (bUp) //move up
         {
            nFirstSelndex = listBoxSelected.SelectedIndices[0];
            nSelCount = listBoxSelected.SelectedIndices.Count;
            string str = listBoxSelected.Items[nFirstSelndex - 1].ToString();
            listBoxSelected.Items.RemoveAt(nFirstSelndex - 1);
            listBoxSelected.Items.Insert(nFirstSelndex + nSelCount - 1, str);

            nFirstSelndex = nFirstSelndex - 1;
         }
         else //move down
         {
            nFirstSelndex = listBoxSelected.SelectedIndices[0];
            nSelCount = listBoxSelected.SelectedIndices.Count;
            int nLastSelIndex = nFirstSelndex + nSelCount - 1;

            string str = listBoxSelected.Items[nLastSelIndex + 1].ToString();
            listBoxSelected.Items.RemoveAt(nLastSelIndex + 1);
            listBoxSelected.Items.Insert(nFirstSelndex, str);

            nFirstSelndex = nFirstSelndex + 1;
         }

         listBoxSelected.SelectedItems.Clear();
         for (int ii = 0; ii < nSelCount; ++ii)
         {
            listBoxSelected.SetSelected(nFirstSelndex + ii, true);
         }         
      }

      public static void updateUpDownButtonEnable(ListBox listBoxSelected, Button buttonUp, Button buttonDown)
      {
         if (listBoxSelected == null || buttonUp == null || buttonDown == null)
            return;

         int nSelCount = listBoxSelected.SelectedIndices.Count;

         buttonUp.Enabled = true;
         buttonDown.Enabled = true;

         //no select item or select the first one: make up button disabled
         if (nSelCount < 1 || listBoxSelected.SelectedIndices[0] == 0)
            buttonUp.Enabled = false;

         //no select item or select the last one, make down button disabled
         if (nSelCount < 1 || listBoxSelected.SelectedIndices[nSelCount - 1] == listBoxSelected.Items.Count - 1)
            buttonDown.Enabled = false;

         //selection is not continues: make both up and down buttons disabled
         if (nSelCount < 1)
            return;

         int nFirstIndex = listBoxSelected.SelectedIndices[0];
         int ii = 0;
         foreach (int nIndex in listBoxSelected.SelectedIndices)
         {
            if (nIndex != (nFirstIndex + ii))
            {
               buttonUp.Enabled = false;
               buttonDown.Enabled = false;
               break;
            }
            ii++;
         }
      }

      public static TaskDialogResult postWarning(string title, string instruction, string content = null)
      {
         if (title == null || instruction == null)
            return TaskDialogResult.None;

         TaskDialog tdlg = new TaskDialog(title);
         tdlg.MainInstruction = instruction;
         tdlg.MainContent = content;
         tdlg.AllowCancellation = true;
         tdlg.CommonButtons = TaskDialogCommonButtons.Close;
         tdlg.DefaultButton = TaskDialogResult.Close;
         tdlg.TitleAutoPrefix = false;
         return tdlg.Show();
      }

      public static DialogResult ShowMsgWarning(string warning, string title)
      {
         return MessageBox.Show(
            warning,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
      }
   }
}
