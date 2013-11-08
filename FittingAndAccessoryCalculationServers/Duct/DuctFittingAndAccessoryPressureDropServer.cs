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
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;

namespace UserFittingAndAccessoryCalculationServers.Duct
{
   public static class SchemaBuildingUtility
   {
      internal static readonly Guid SpecificCoefficientSchemaGuid = new Guid("13DED697-D107-4B0D-8DC4-2A2E4C870096");
      internal static readonly Guid SpecificLossSchemaGuid = new Guid("5C8B4686-656E-4AB5-8382-EC43C3026B4E");
      internal static readonly Guid CoefficientFromTableSchemaGuid = new Guid("762A2314-1D1C-4087-A58F-BAB902F57BE5");

      public static readonly string fieldCoefficient = "Coefficient";
      public static readonly string fieldPressureLoss = "PressureLoss";
      public static readonly string fieldUserTableName = "UserTableName";

   }  


   public class UserTableDuctPressureDropServer : IDuctFittingAndAccessoryPressureDropServer
   {
      /// <summary>
      /// Calculates the pressure Drop with the input data .
      /// </summary>
      /// <param name="data">
      /// The input for the pressure drop calculation. The result pressure drop is also return through the data to the caller. 
      /// </param>
      public bool Calculate(DuctFittingAndAccessoryPressureDropData data)
      {
         if (data == null)
            return false;

         /* calculation to modify data here */
         
         return true;
         
      }

      /// <summary>
      /// Returns the method's name this server implements. 
      /// </summary>
      public string GetMethodName()
      {
         return Properties.Resources.DuctUserTablePressureDropServerName;
      }

      /// <summary>
      /// Returns the Id of the server. 
      /// </summary>
      public System.Guid GetServerId()
      {
         return new Guid("8BAF7D75-8B9B-46D0-B8CE-3AD1C19E6B12");
      }

      /// <summary>
      /// Returns the Id of the service that the server belongs to. 
      /// </summary>
      public ExternalServiceId GetServiceId()
      {
         return ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropService;
      }

      /// <summary>
      /// Returns the name of the server. 
      /// </summary>
      public System.String GetName()
      {
         return Properties.Resources.DuctUserTablePressureDropServerName;
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
         return Properties.Resources.DuctUserTablePressureDropServerDescription;
      }

      /// <summary>
      /// Checks if the server is applicable for the duct fitting and duct accessory. 
      /// </summary>
      public bool IsApplicable(DuctFittingAndAccessoryPressureDropData data)
      {
         //For not define server : applicable for all fittings and accessories
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
         return Properties.Resources.DuctUserTablePressureDropServerInformationLink;
      }
   }

}
