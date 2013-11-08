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
using Autodesk.Revit.UI.Mechanical;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.ExtensibleStorage;
using UserFittingAndAccessoryCalculationServers.Duct;

namespace UserFittingAndAccessoryCalculationUIServers.Duct
{


   public class CoefficientFromTablePressureDropUIServer : IDuctFittingAndAccessoryPressureDropUIServer
   {
      /// <summary>
      /// Returns the id of the corresponding DB server for which this server provides an optional UI. 
      /// </summary>
      public System.Guid GetDBServerId()
      {
         return new Guid("8BAF7D75-8B9B-46D0-B8CE-3AD1C19E6B12");
      }

      /// <summary>
      /// Indicates that this UI server actually has some settings to be shown.
      /// It is required that DB servers have their corresponding UI servers, but it is not required that there is an actual UI to be shown.
      /// If the calculations server does not have any additional data and therefore does not need the optional UI, the UI will do nothing (in which case the return from this method will be False) 
      /// </summary>
      /// <returns>
      /// True if there is settings UI for the DB server, False otherwise.
      /// </returns>
      public bool HasSettings()
      {
         return true;
      }

      /// <summary>
      /// Prompts the setting UI for the user. 
      /// This method might be invoked only when the server has UI settings (HasSettings == True).
      /// </summary>
      /// <param name="data">
      /// The duct fitting and accessory pressure drop UI data.
      /// It is used as in/out param, the user can get the old values from it and also can set the new values from the setting UI back to it.
      /// </param>
      /// <returns>
      /// True if the user does change something in the UI (i.e. the user changes something in the entity in the data that was given as the argument into this method.), False otherwise.
      /// </returns>
      public bool ShowSettings(DuctFittingAndAccessoryPressureDropUIData data)
      {
         bool settingChanged = false;

         /* your configuration UI here */

         return settingChanged;
      }

      /// <summary>
      /// Returns the Id of the server. 
      /// </summary>
      public System.Guid GetServerId()
      {
         return new Guid("25FA8DE2-67C4-47D1-91F6-BD6F0803A588");
      }

      /// <summary>
      /// Returns the Id of the service that the sever belongs to. 
      /// </summary>
      public ExternalServiceId GetServiceId()
      {
         return ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropUIService;
      }

      /// <summary>
      /// Returns the server's name. 
      /// </summary>
      public System.String GetName()
      {
         return Properties.Resources.DuctCoefficientFromTablePressureDropUIServerName;
      }

      /// <summary>
      /// Returns the server's vendor Id. 
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
         return Properties.Resources.DuctCoefficientFromTablePressureDropUIServerDescription;
	  }
   }
}
