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
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.ApplicationServices;

namespace UserStraightSegmentCalculationServers
{

   /// <summary>
   /// The external Application for the sever mainly for adding and registering the server to Revit.
   /// </summary>
   public class ServerApp : IExternalDBApplication
   {
      /// <summary>
      /// Add and register the sever on Revit startup.
      /// </summary>
      public ExternalDBApplicationResult OnStartup(ControlledApplication application)
      {
         ExternalService plumbingFixtureService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipePlumbingFixtureFlowService);
         Pipe.PlumbingFixtureFlowServer flowServer = new Pipe.PlumbingFixtureFlowServer();
         if (plumbingFixtureService != null)
            plumbingFixtureService.AddServer(flowServer);

         ExternalService pipePressureDropService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipePressureDropService);
         Pipe.PipePressureDropServer pressureDropServer = new Pipe.PipePressureDropServer();
         if (pipePressureDropService != null)
            pipePressureDropService.AddServer(pressureDropServer);

         ExternalService ductPressureDropService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DuctPressureDropService);
         Duct.DuctPressureDropServer ductPressureDropServer = new Duct.DuctPressureDropServer();
         if (ductPressureDropService != null)
            ductPressureDropService.AddServer(ductPressureDropServer);

         return ExternalDBApplicationResult.Succeeded;
      }

      public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
      {
         return ExternalDBApplicationResult.Succeeded;
      }
   }

}
