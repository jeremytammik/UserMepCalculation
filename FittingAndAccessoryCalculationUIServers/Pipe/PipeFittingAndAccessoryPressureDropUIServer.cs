using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.ExtensibleStorage;
using UserFittingAndAccessoryCalculationServers.Pipe;
using UserFittingAndAccessoryCalculationUIServers.Duct;
using Autodesk.Revit.DB.Plumbing;

namespace UserFittingAndAccessoryCalculationUIServers.Pipe
{

   public class KFactorTablePipePressureDropUIServer : IPipeFittingAndAccessoryPressureDropUIServer
   {

      /// <summary>
      /// Returns the id of the corresponding DB server for which this server provides an optional UI. 
      /// </summary>
      public System.Guid GetDBServerId()
      {
         return new Guid("51DD5E98-A9DD-464B-B286-4A37953610B1");
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
      /// The pipe fitting and accessory pressure drop UI data.
      /// It is used as in/out param, the user can get the old values from it and also can set the new values from the setting UI back to it.
      /// </param>
      /// <returns>
      /// True if the user does change something in the UI (i.e. the user changes something in the entity in the data that was given as the argument into this method.), False otherwise.
      /// </returns>
      public bool ShowSettings(PipeFittingAndAccessoryPressureDropUIData data)
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
         return new Guid("CDA19B6F-FBD5-4725-A0CE-F159BF5D0266");
      }

      /// <summary>
      /// Returns the Id of the service that the sever belongs to. 
      /// </summary>
      public ExternalServiceId GetServiceId()
      {
         return ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropUIService;
      }

      /// <summary>
      /// Returns the server's name. 
      /// </summary>
      public System.String GetName()
      {
         return Properties.Resources.PipeKFactorFromTablePressureDropUIServerName;
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
         return Properties.Resources.PipeKFactorFromTablePressureDropUIServerDescription;
      }
   }
}
