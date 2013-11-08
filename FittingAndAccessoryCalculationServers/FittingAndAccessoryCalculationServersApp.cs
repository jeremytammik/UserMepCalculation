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

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace UserFittingAndAccessoryCalculationServers
{
   /// <summary>
   /// The external application for registering fitting and accessory pressure drop servers with Revit.
   /// </summary>
   public class ServerApp : IExternalDBApplication
   {
      /// <summary>
      /// Add and register the server on Revit startup.
      /// </summary>
      public ExternalDBApplicationResult OnStartup(ControlledApplication application)
      {
         AddDuctFittingAndAccessoryPressureDropServers();
         AddPipeFittingAndAccessoryPressureDropServers();
         return ExternalDBApplicationResult.Succeeded;
      }

      private void AddDuctFittingAndAccessoryPressureDropServers()
      {
         MultiServerService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropService) as MultiServerService;
         if (service == null)
            return;

         List<Guid> activeServerIds = new List<Guid>();

         // User table server is the default server, and it's already set to active, so it should be set active again.
         Duct.UserTableDuctPressureDropServer UserTableServer = new Duct.UserTableDuctPressureDropServer();
         if (UserTableServer != null)
         {
            service.AddServer(UserTableServer);
            activeServerIds.Add(UserTableServer.GetServerId());
         }



         IList<Guid> currentActiveServerIds = service.GetActiveServerIds();
        // currentActiveServerIds.Remove(service.GetDefaultServerId());
         activeServerIds.AddRange(currentActiveServerIds);
         service.SetActiveServers(activeServerIds);
      }

      private void AddPipeFittingAndAccessoryPressureDropServers()
      {
         MultiServerService service = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropService) as MultiServerService;
         if (service == null)
            return;

         List<Guid> activeServerIds = new List<Guid>();




         Pipe.UserKFactorTablePipePressureDropServer kFactorTablePipeServer = new Pipe.UserKFactorTablePipePressureDropServer();
         if (kFactorTablePipeServer != null)
         {
            service.AddServer(kFactorTablePipeServer);
         }

         activeServerIds.AddRange(service.GetActiveServerIds());
         service.SetActiveServers(activeServerIds);
      }

      public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
      {
         return ExternalDBApplicationResult.Succeeded;
      }
   }

}
