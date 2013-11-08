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
using System.Data;
using System.IO;

namespace UserPressureLossReport
{
   public class CsvStreamWriter
   {
      private ArrayList rowAL;        //Row list,each line is a list
      private string fileName;       //file name
      private Encoding encoding;

      public CsvStreamWriter()
      {
         this.rowAL = new ArrayList();
         this.fileName = "";
         this.encoding = Encoding.Default;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="fileName">file name, including file path</param>
      public CsvStreamWriter(string fileName)
      {
         this.rowAL = new ArrayList();
         this.fileName = fileName;
         this.encoding = Encoding.Default;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="fileName">file name, including file path</param>
      /// <param name="encoding">encoding</param>
      public CsvStreamWriter(string fileName, Encoding encoding)
      {
         this.rowAL = new ArrayList();
         this.fileName = fileName;
         this.encoding = encoding;
      }

      /// <summary>
      /// row:row = 1 is the first row
      /// col:col = 1 is the first column
      /// </summary>
      public string this[int row, int col]
      {
         set
         {
            //check row valid
            if (row <= 0)
            {
               throw new Exception("row can't be less than 0");
            }
            else if (row > this.rowAL.Count) //if the row is not big enough, need to add row
            {
               for (int i = this.rowAL.Count + 1; i <= row; i++)
               {
                  this.rowAL.Add(new ArrayList());
               }
            }

            //check column valid
            if (col <= 0)
            {
               throw new Exception("column can't be less than 0");
            }
            else
            {
               ArrayList colTempAL = (ArrayList)this.rowAL[row - 1];

               //extend column
               if (col > colTempAL.Count)
               {
                  for (int i = colTempAL.Count; i <= col; i++)
                  {
                     colTempAL.Add("");
                  }
               }
               this.rowAL[row - 1] = colTempAL;
            }
            //set value
            ArrayList colAL = (ArrayList)this.rowAL[row - 1];

            colAL[col - 1] = value;
            this.rowAL[row - 1] = colAL;
         }
      }


      /// <summary>
      /// file name, including file path
      /// </summary>
      public string FileName
      {
         set
         {
            this.fileName = value;
         }
      }

      /// <summary>
      /// 
      /// </summary>

      public Encoding FileEncoding
      {
         set
         {
            this.encoding = value;
         }
      }

      /// <summary>
      /// get max row
      /// </summary>
      public int CurMaxRow
      {
         get
         {
            return this.rowAL.Count;
         }
      }

      /// <summary>
      /// get max col
      /// </summary>
      public int CurMaxCol
      {
         get
         {
            int maxCol;

            maxCol = 0;
            for (int i = 0; i < this.rowAL.Count; i++)
            {
               ArrayList colAL = (ArrayList)this.rowAL[i];

               maxCol = (maxCol > colAL.Count) ? maxCol : colAL.Count;
            }

            return maxCol;
         }
      }

      /// <summary>
      /// add data to the file
      /// </summary>
      /// <param name="dataDT">datatable</param>
      /// <param name="beginCol">start column,beginCol = 1 is the first column</param>
      public void AddData(DataTable dataDT, int beginCol)
      {
         if (dataDT == null)
         {
            throw new Exception("the table is empty");
         }
         int curMaxRow;

         curMaxRow = this.rowAL.Count;
         for (int i = 0; i < dataDT.Rows.Count; i++)
         {
            for (int j = 0; j < dataDT.Columns.Count; j++)
            {
               this[curMaxRow + i + 1, beginCol + j] = dataDT.Rows[i][j].ToString();
            }
         }
      }

      public void addTitleRow(string title)
      {
         DataTable tb = new DataTable();
         tb.Columns.Add();
         tb.Rows.Add(title);

         AddData(tb, 1);
      }

      public void addOneEmptyRow()
      {
         DataTable tb = new DataTable();
         tb.Columns.Add();
         tb.Rows.Add(" ");

         AddData(tb, 1);
      }

      /// <summary>
      /// save file,it will replace the same name file
      /// </summary>
      public void Save()
      {
         //check the file name valid
         if (this.fileName == null)
         {
            throw new Exception("file name is empty");
         }
         else if (File.Exists(this.fileName))
         {
            try
            {
               File.Delete(this.fileName);
            }
            catch
            {
               throw new Exception(ReportConstants.failed_to_delete);
            }
               
         }
         if (this.encoding == null)
         {
            this.encoding = Encoding.Default;
         }
         System.IO.StreamWriter sw = new StreamWriter(this.fileName, false, this.encoding);

         for (int i = 0; i < this.rowAL.Count; i++)
         {
            sw.WriteLine(ConvertToSaveLine((ArrayList)this.rowAL[i]));
         }

         sw.Close();
      }

      /// <summary>
      /// save file,it will replace the same name file
      /// </summary>
      /// <param name="fileName"> file name, including file path</param>
      public void Save(string fileName)
      {
         this.fileName = fileName;
         Save();
      }

      /// <summary>
      /// save file,it will replace the same name file
      /// </summary>
      /// <param name="fileName">file name, including file path</param>
      /// <param name="encoding">encoding</param>
      public void Save(string fileName, Encoding encoding)
      {
         this.fileName = fileName;
         this.encoding = encoding;
         Save();
      }


      /// <summary>
      /// convert to line before save
      /// </summary>
      /// <param name="colAL">one row</param>
      /// <returns></returns>
      private string ConvertToSaveLine(ArrayList colAL)
      {
         string saveLine;

         saveLine = "";
         for (int i = 0; i < colAL.Count; i++)
         {
            saveLine += ConvertToSaveCell(colAL[i].ToString());
            //coma is the separator
            if (i < colAL.Count - 1)
            {
               saveLine += ",";
            }
         }

         return saveLine;
      }

      /// <summary>
      /// 
      /// add "" to the cell text
      /// 
      /// </summary>
      /// <param name="cell">cell text</param>
      /// <returns></returns>
      private string ConvertToSaveCell(string cell)
      {
         cell = cell.Replace("\"", "\"\"");

         return "\"" + cell + "\"";
      }
   }
}
