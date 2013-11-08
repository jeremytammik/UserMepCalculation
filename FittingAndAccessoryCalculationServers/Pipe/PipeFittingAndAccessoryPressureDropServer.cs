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
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;

namespace UserFittingAndAccessoryCalculationServers.Pipe
{
   public static class PipeSchemaBuildingUtility
   {
      internal static readonly Guid KFactorSchemaGuid = new Guid("EA3E8AF6-3235-437D-95EE-D7EBEEE610B2");
      internal static readonly Guid SpecificLossSchemaGuid = new Guid("E520753E-B098-45F4-B2BE-8AC5C4BBB1F2");
      internal static readonly Guid CoefficientFromTableSchemaGuid = new Guid("1B022A2B-5722-4787-928A-4F29CA9D8811");

      public static readonly string fieldKFactor = "KFactor";
      public static readonly string fieldPressureLoss = "PressureLoss";
      public static readonly string fieldKFactorableName = "PipeFittingKFactorTableName";


   }



   public class UserKFactorTablePipePressureDropServer : IPipeFittingAndAccessoryPressureDropServer
   {
      /// <summary>
      /// Calculates the pressure Drop with the input data .
      /// </summary>
      /// <param name="data">
      /// The input for the pressure drop calculation. The result pressure drop is also return through the data to the caller. 
      /// </param>
      public bool Calculate(PipeFittingAndAccessoryPressureDropData data)
      {
         if (data == null)
            return false;

         /* calculation to modify data here */

         return true;
      }


      /// <summary>
      /// Returns the Id of the server. 
      /// </summary>
      public System.Guid GetServerId()
      {
         return new Guid("51DD5E98-A9DD-464B-B286-4A37953610B1");
      }

      /// <summary>
      /// Returns the Id of the service that the sever belongs to. 
      /// </summary>
      public ExternalServiceId GetServiceId()
      {
         return ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropService;
      }

      /// <summary>
      /// Returns the name of the server. 
      /// </summary>
      public System.String GetName()
      {
         return Properties.Resources.PipeKFactorTablePressureDropServerName;
      }

      /// <summary>
      /// Returns the Vendor Id of the server. 
      /// </summary>
      public System.String GetVendorId()
      {
         return "USER";
      }

      /// <summary>
      /// Returns the description of the server. 
      /// </summary>
      public System.String GetDescription()
      {
         return Properties.Resources.PipeKFactorTablePressureDropServerDescription;
      }

      /// <summary>
      /// Checks if the server is applicable for the pipe fitting and pipe accessory. 
      /// </summary>
      public bool IsApplicable(PipeFittingAndAccessoryPressureDropData data)
      {
         return true;
      }


      /// <summary>
      /// Server can have its own entity, and this function should return the entity schema. 
      /// </summary>
      public Schema GetDataSchema()
      {

         return null;
      }

      /// Gets the URL address which provides details about this server.
      /// </summary>
      public System.String GetInformationLink()
      {
         return Properties.Resources.PipeKFactorTablePressureDropServerInformationLink;
      }
   }

}
