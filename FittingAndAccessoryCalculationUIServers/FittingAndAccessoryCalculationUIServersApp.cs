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
using System.IO;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Mechanical;
using Autodesk.Revit.UI.Plumbing;
using Autodesk.Revit.DB.ExternalService;

namespace UserFittingAndAccessoryCalculationUIServers
{
   /// <summary>
   /// The external application for registering fitting and accessory pressure drop UI servers with Revit.
   /// </summary>
   public class ServerApp : IExternalApplication
   {
      /// <summary>
      /// Add and register the server on Revit startup.
      /// </summary>
      public Result OnStartup(UIControlledApplication application)
      {
         MultiServerService ductService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DuctFittingAndAccessoryPressureDropUIService) as MultiServerService;
         if (ductService == null)
            return Result.Succeeded;



         Duct.CoefficientFromTablePressureDropUIServer UserTableUIServer = new Duct.CoefficientFromTablePressureDropUIServer();
         ductService.AddServer(UserTableUIServer);

         //pipe UI servers
         MultiServerService pipeService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.PipeFittingAndAccessoryPressureDropUIService) as MultiServerService;
         if (pipeService == null)
            return Result.Succeeded;


         Pipe.KFactorTablePipePressureDropUIServer pipeKFactorUIServer = new Pipe.KFactorTablePipePressureDropUIServer();
         pipeService.AddServer(pipeKFactorUIServer);

         return Result.Succeeded;
      }

      public Result OnShutdown(UIControlledApplication application)
      {
         return Result.Succeeded;
      }
   }
}
