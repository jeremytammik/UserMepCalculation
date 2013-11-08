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
using System.Resources;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.ApplicationServices;


namespace UserStraightSegmentCalculationServers.Pipe
{
  /// <summary>
  /// The external server that calculates the plumbing fixture flow.
  /// </summary>
  public class PlumbingFixtureFlowServer : IPipePlumbingFixtureFlowServer
  {
    System.Collections.Generic.ICollection<KeyValuePair<double, double>> flushTanks = new System.Collections.Generic.Dictionary<double, double>();
    System.Collections.Generic.ICollection<KeyValuePair<double, double>> flushValves = new System.Collections.Generic.Dictionary<double, double>();

    private void initilizeflushTanks()
    {
      if( flushTanks.Count > 0 )
        return;

      //the key is Fixture Units
      //the value is Flow (gallon (U.S.) per minute (gpm) (gal/min))
      flushTanks.Add( new KeyValuePair<double, double>( 1, 3.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 2, 5.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 3, 6.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 4, 8.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 5, 9.4 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 6, 10.7 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 7, 11.8 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 8, 12.8 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 9, 13.7 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 10, 14.6 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 11, 15.4 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 12, 16.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 13, 16.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 14, 17.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 15, 17.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 16, 18.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 17, 18.4 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 18, 18.8 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 19, 19.2 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 20, 19.6 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 25, 21.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 30, 23.3 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 35, 24.9 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 40, 26.3 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 45, 27.7 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 50, 29.1 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 60, 32.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 70, 35.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 80, 38.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 90, 41.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 100, 43.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 120, 48.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 140, 52.5 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 160, 57.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 180, 61.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 200, 65.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 225, 70.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 250, 75.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 275, 80.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 300, 85.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 400, 105.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 500, 124.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 750, 170.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 1000, 208.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 1250, 239.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 1500, 269.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 1750, 297.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 2000, 325.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 2500, 380.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 3000, 433.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 4000, 525.0 ) );
      flushTanks.Add( new KeyValuePair<double, double>( 5000, 593.0 ) );
    }


    private void initilizeflushValves()
    {
      if( flushValves.Count > 0 )
        return;

      //the key is Fixture Units
      //the value is Flow (gallon (U.S.) per minute (gpm) (gal/min))
      flushValves.Add( new KeyValuePair<double, double>( 5, 15.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 6, 17.4 ) );
      flushValves.Add( new KeyValuePair<double, double>( 7, 19.8 ) );
      flushValves.Add( new KeyValuePair<double, double>( 8, 22.2 ) );
      flushValves.Add( new KeyValuePair<double, double>( 9, 24.6 ) );
      flushValves.Add( new KeyValuePair<double, double>( 10, 27.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 11, 27.8 ) );
      flushValves.Add( new KeyValuePair<double, double>( 12, 28.6 ) );
      flushValves.Add( new KeyValuePair<double, double>( 13, 29.4 ) );
      flushValves.Add( new KeyValuePair<double, double>( 14, 30.2 ) );
      flushValves.Add( new KeyValuePair<double, double>( 15, 31.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 16, 31.8 ) );
      flushValves.Add( new KeyValuePair<double, double>( 17, 32.6 ) );
      flushValves.Add( new KeyValuePair<double, double>( 18, 33.4 ) );
      flushValves.Add( new KeyValuePair<double, double>( 19, 34.2 ) );
      flushValves.Add( new KeyValuePair<double, double>( 20, 35.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 25, 38.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 30, 42.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 35, 44.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 40, 46.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 45, 48.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 50, 50.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 60, 54.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 70, 58.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 80, 61.2 ) );
      flushValves.Add( new KeyValuePair<double, double>( 90, 64.3 ) );
      flushValves.Add( new KeyValuePair<double, double>( 100, 67.5 ) );
      flushValves.Add( new KeyValuePair<double, double>( 120, 73.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 140, 77.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 160, 81.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 180, 85.5 ) );
      flushValves.Add( new KeyValuePair<double, double>( 200, 90.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 225, 95.5 ) );
      flushValves.Add( new KeyValuePair<double, double>( 250, 101.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 275, 104.5 ) );
      flushValves.Add( new KeyValuePair<double, double>( 300, 108.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 400, 127.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 500, 143.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 750, 177.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 1000, 208.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 1250, 239.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 1500, 269.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 1750, 297.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 2000, 325.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 2500, 380.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 3000, 433.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 4000, 525.0 ) );
      flushValves.Add( new KeyValuePair<double, double>( 5000, 593.0 ) );
    }

    private bool AlmostZero( double val )
    {
      return System.Math.Abs( val ) < double.Epsilon;
    }

    /// <summary>
    /// Calculate the flow with the dimension flow (GPM - gallon (U.S.) per minute (gpm) (gal/min) ) .
    /// </summary>
    /// <param name="data">
    /// The input for the flow calculation. The result flow is also return through the data to the caller. 
    /// </param>
    /// Return Value: Flow - the units is GPM (gallon (U.S.) per minute (gpm) (gal/min))
    private double presetFlow( double dDimensionFlow )
    {
      return dDimensionFlow;
    }

    /// <summary>
    /// Calculate the flow with the fixture units and flow conversion mode - need to read the table(flushValves and flushTanks) .
    /// </summary>
    /// <param name="data">
    /// The input for the flow calculation. The result flow is also return through the data to the caller. 
    /// </param>
    /// Return Value: Flow - the units is GPM (gallon (U.S.) per minute (gpm) (gal/min))
    private double getFlowFromFixtureUnits( double dFixtureUnits, FlowConversionMode eFlowConversionMode )
    {
      if( ( dFixtureUnits < 0 ) || AlmostZero( dFixtureUnits ) )
        return 0;

      initilizeflushTanks();
      initilizeflushValves();
      System.Collections.Generic.ICollection<KeyValuePair<double, double>> points = flushTanks;

      int size = flushTanks.Count;
      double dFlow = 0;

      if( eFlowConversionMode == FlowConversionMode.Valves )
      {
        points = flushValves;
        size = flushValves.Count;
      }
      else if( eFlowConversionMode == FlowConversionMode.Tanks )
      {
        points = flushTanks;
        size = flushTanks.Count;
      }
      else
      {
        return dFlow;
      }

      if( dFixtureUnits < points.ElementAt( 0 ).Key )
      {
        dFlow = points.ElementAt( 0 ).Value;
      }
      else
      {
        bool bInTable = false;
        int ii = 0;
        for( ; ii < size; ii++ )
        {
          if( dFixtureUnits <= points.ElementAt( ii ).Key )
          {
            bInTable = true;
            break;
          }
        }

        KeyValuePair<double, double> p1, p2;
        if( bInTable )
        {
          p1 = points.ElementAt( ii );
          if( ii == 0 )
            p2 = points.ElementAt( ii + 1 );
          else
            p2 = points.ElementAt( ii - 1 );
        }
        else
        {
          p1 = points.ElementAt( size - 2 );
          p2 = points.ElementAt( size - 1 );
        }

        double x1 = p1.Key;
        double y1 = p1.Value;
        double x2 = p2.Key;
        double y2 = p2.Value;
        double x = dFixtureUnits;
        double y = ( x * y2 - x1 * y2 + x2 * y1 - x * y1 ) / ( x2 - x1 );
        dFlow = y;
      }

      return dFlow;
    }

    /// <summary>
    /// Calculate the flow with the input data .
    /// </summary>
    /// <param name="data">
    /// The input for the flow calculation. The result flow is also return through the data to the caller. 
    /// </param>
    public void Calculate( PipePlumbingFixtureFlowData data )
    {
      double dResult = 0.0;

      //The flow configuration mode of the pipe: -1: invalid;  0: Preset; 1: Fixture Units
      if( data.FlowConfiguration == PipeFlowConfigurationType.Preset )
        dResult = presetFlow( data.DimensionFlow );
      else if( data.FlowConfiguration == PipeFlowConfigurationType.Demand )
        dResult = getFlowFromFixtureUnits( data.FixtureUnits, data.FlowConversionMode );

      data.Flow = dResult;
    }

    /// <summary>
    /// Return the method's name this server implements. 
    /// </summary>
    public string GetMethodName()
    {
      return Properties.Resources.PlumbingFixtureFlowMethodName;
    }

    /// <summary>
    /// Return the Id of the server. 
    /// </summary>
    public System.Guid GetServerId()
    {
      return new Guid( "56121D7D-E1D7-42A3-BED8-F4D1D3205833" );
    }

    /// <summary>
    /// Return the Id of the service that the sever belongs to. 
    /// </summary>
    public ExternalServiceId GetServiceId()
    {
      return ExternalServices.BuiltInExternalServices.PipePlumbingFixtureFlowService;
    }

    /// <summary>
    /// Return the server's name. 
    /// </summary>
    public System.String GetName()
    {
      return Properties.Resources.PlumbingFixtureFlowServerName;
    }

    /// <summary>
    /// Return the server's vendor Id. 
    /// </summary>
    public System.String GetVendorId()
    {
      return "USER";
    }

    /// <summary>
    /// Return the description of the server. 
    /// </summary>
    public System.String GetDescription()
    {
      return Properties.Resources.PlumbingFixtureFlowServerDescription;
    }

    /// <summary>
    /// Return the link of the server information. 
    /// </summary>
    public System.String GetInformationLink()
    {
      return Properties.Resources.PlumbingFixtureFlowServerInformationLink;
    }

  //  /// <summary>
  //  /// Return the server description in RTF format. 
  //  /// </summary>
  //  public System.String GetRTFDescription()
  //  {
  //    return Properties.Resources.PlumbingFixtureFlowServerDescriptionRTF;
  //  }

    /// <summary>
    /// Return the server description in HTML format. 
    /// </summary>
    public System.String GetHtmlDescription()
    {
      return "HTML description of plumbing fixture flow server";
    }
  }
}
