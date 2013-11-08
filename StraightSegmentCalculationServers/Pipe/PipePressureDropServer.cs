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

  public class PipePressureDropServer : IPipePressureDropServer
  {
    private bool AlmostZero( double val )
    {
      return System.Math.Abs( val ) < double.Epsilon;
    }
    /// <summary>
    /// Calculate the Velocity with the input data .
    /// </summary>
    /// <param name="dInnerDiameter">
    /// The inner diameter of the pipe, Units: (ft).
    /// </param>
    /// <param name="dFlow">
    /// The flow of the pipe, Units: (ft³/s).
    /// </param>
    /// Return Value: Velocity, Units: (ft/s).
    private double CalcVelocity( double dInnerDiameter, double dFlow )
    {
      double dResult = 0.0;
      if( !AlmostZero( dInnerDiameter ) )
      {
        dResult = System.Math.Abs( dFlow ) / ( System.Math.PI * dInnerDiameter * dInnerDiameter / 4.0 );
      }
      return dResult;
    }
    /// <summary>
    /// Calculate Reynolds number with the input data .
    /// </summary>
    /// <param name="dInnerDiameter">
    /// The inner diameter of the pipe, Units: (ft).
    /// </param>
    /// <param name="dVelocity">
    /// The velocity of the pipe, Units: (ft/s).
    /// </param>
    /// <param name="dDensity">
    /// The density of the pipe, Units: (kg/ft³).
    /// </param>
    /// <param name="dViscosity">
    /// The viscosity of the pipe, Units: (kg/(ft·s)).
    /// </param>
    /// Return Value: Reynolds number.
    private double CalcReynoldsNumber( double dInnerDiameter, double dVelocity, double dDensity, double dViscosity )
    {
      double dResult = 0.0;
      if( !AlmostZero( dViscosity ) )
      {
        dResult = dInnerDiameter * dVelocity * dDensity / dViscosity;
      }
      return dResult;
    }
    /// <summary>
    /// Calculate Flow State with the input data .
    /// </summary>
    /// <param name="dReynoldsNumber">
    /// The Reynolds number of the pipe.
    /// </param>
    /// Return Value: Flow State
    private PipeFlowState CalcFlowState( double dReynoldsNumber )
    {
      if( dReynoldsNumber < 2000.0 )
      {
        return PipeFlowState.LaminarState;
      }
      else if( dReynoldsNumber > 4000.0 )
      {
        return PipeFlowState.TurbulentState;
      }

      return PipeFlowState.TransitionState;
    }
    /// <summary>
    /// Calculate Relative Roughness with the input data .
    /// </summary>
    /// <param name="dInnerDiameter">
    /// The inner diameter of the pipe, Units: (ft).
    /// </param>
    /// <param name="dRoughness">
    /// The roughness of the pipe, Units: (ft).
    /// </param>
    /// Return Value: Relative Roughness
    private double CalcRelRoughness( double dInnerDiameter, double dRoughness )
    {
      double dResult = 0.0;
      if( !AlmostZero( dRoughness ) )
      {
        dResult = dInnerDiameter / dRoughness;
      }
      return dResult;
    }
    /// <summary>
    /// Calculate Friction Factor with the input data .
    /// </summary>
    /// <param name="dReynoldsNumber">
    /// The reynolds number of the pipe.
    /// </param>
    /// <param name="dRelativeRoughness">
    /// The relative roughness of the pipe.
    /// </param>
    /// <param name="eFlowState">
    /// The flow state of the pipe.
    /// </param>
    /// Return Value: Friction Factor
    private double CalcFrictionFactor( double dReynoldsNumber, double dRelativeRoughness, PipeFlowState eFlowState )
    {
      double dResult = 0.0;

      switch( eFlowState )
      {
        case PipeFlowState.LaminarState:
          {
            if( !AlmostZero( dReynoldsNumber ) )
            {
              dResult = 64.0 / dReynoldsNumber;
            }
          }
          break;
        case PipeFlowState.TurbulentState:
          {
            // Protect log10() exceptions against any zero or negative value. 
            if( dRelativeRoughness > double.Epsilon )
            {
              double dTemp = 2.0 * System.Math.Log10( 3.7 * dRelativeRoughness );
              if( !AlmostZero( dTemp ) )
              {
                dTemp = 1.0 / dTemp;
                dResult = dTemp * dTemp;
              }
            }
          }
          break;
      }
      return dResult;
    }
    /// <summary>
    /// Calculate Friction with the input data .
    /// </summary>
    /// <param name="dInnerDiameter">
    /// The inner diameter for the pipe, Units: (ft).
    /// </param>
    /// <param name="dFrictionFactor">
    /// The friction factor of the pipe.
    /// </param>
    /// <param name="dDensity">
    /// The density of the pipe, Units: (kg/ft³).
    /// </param>
    /// <param name="dVelocity">
    /// The velocity of the pipe, Units: (ft/s).
    /// </param>
    /// Return Value: Friction, Units: (kg/(ft²·s²)).
    private double CalcFriction( double dInnerDiameter, double dFrictionFactor, double dDensity, double dVelocity )
    {
      double dResult = 0.0;

      if( !AlmostZero( dInnerDiameter ) )
      {
        dResult = 1.0 * dFrictionFactor * dDensity * dVelocity * dVelocity / ( dInnerDiameter * 2.0 );
      }

      return dResult;
    }

    /// <summary>
    /// Calculate Velocity Pressure with the input data .
    /// </summary>
    /// <param name="dVelocity">
    /// The velocity of the pipe. Units: (ft/s).
    /// </param>
    /// <param name="dDensity">
    /// The density of the pipe, Units: (kg/ft³).
    /// </param>
    /// Return Value: Velocity Pressure, Units: (kg/(ft·s²)).
    private double CalcVelocityPressure( double dVelocity, double dDensity )
    {
      return dDensity * dVelocity * dVelocity / 2.0;
    }

    /// <summary>
    /// Calculate pressure drop with the input data .
    /// </summary>
    /// <param name="data">
    /// The input for the pressure drop calculation. The result is also return through the data to the caller. 
    /// </param>
    public void Calculate( PipePressureDropData data )
    {
      double dRelRoughness = 0.0;
      double dVelocity = 0.0;
      double dFrictionFactor = 0.0;
      double dFriction = 0.0;
      double dReynolds = 0.0;
      double dCoefficient = 0.0;
      double dVelocityPressure = 0.0;
      double dPressureDrop = 0.0;
      PipeFlowState eFlowState = PipeFlowState.LaminarState;
      if( data.KLevel != Autodesk.Revit.DB.Mechanical.SystemCalculationLevel.None )
      {
        dVelocity = CalcVelocity( data.InsideDiameter, data.Flow );
        dReynolds = CalcReynoldsNumber( data.InsideDiameter, dVelocity, data.Density, data.Viscosity );
        eFlowState = CalcFlowState( dReynolds );
      }
      dRelRoughness = CalcRelRoughness( data.InsideDiameter, data.Roughness );

      if( data.KLevel == Autodesk.Revit.DB.Mechanical.SystemCalculationLevel.All )
      {
        dFrictionFactor = CalcFrictionFactor( dReynolds, dRelRoughness, eFlowState );
        dFriction = CalcFriction( data.InsideDiameter, dFrictionFactor, data.Density, dVelocity );
        dPressureDrop = data.Length * dFriction;
        dVelocityPressure = CalcVelocityPressure( dVelocity, data.Density );
        if( !AlmostZero( dVelocityPressure ) )
          dCoefficient = dPressureDrop / dVelocityPressure;
      }

      data.Velocity = dVelocity;
      data.ReynoldsNumber = dReynolds;
      data.FlowState = eFlowState;
      data.RelativeRoughness = dRelRoughness;
      data.FrictionFactor = dFrictionFactor;
      data.Friction = dFriction;
      data.PressureDrop = dPressureDrop;
      data.VelocityPressure = dVelocityPressure;
      data.Coefficient = dCoefficient;
    }

    /// <summary>
    /// Return the method's name this server implements. 
    /// </summary>
    public string GetMethodName()
    {
      return Properties.Resources.PipePressureDropMethodName;
    }

    /// <summary>
    /// Return the Id of the server. 
    /// </summary>
    public System.Guid GetServerId()
    {
      return new Guid( "EA275FB1-5D7B-47D6-B828-BF856DF9BF22" );
    }

    /// <summary>rerere
    /// Return the Id of the service that the sever belongs to. 
    /// </summary>
    public ExternalServiceId GetServiceId()
    {
      return ExternalServices.BuiltInExternalServices.PipePressureDropService;
    }

    /// <summary>
    /// Return the server's name. 
    /// </summary>
    public System.String GetName()
    {
      return Properties.Resources.PipePressureDropServerName;
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
      return Properties.Resources.PipePressureDropServerDescription;
    }

    /// <summary>
    /// Return the link of the server information. 
    /// </summary>
    public System.String GetInformationLink()
    {
      return Properties.Resources.PipePressureDropServerInformationLink;
    }

    ///// <summary>
    ///// Return the server description in RTF format. 
    ///// </summary>
    //public System.String GetRTFDescription()
    //{
    //  return Properties.Resources.PipePressureDropServerDescriptionRTF;
    //}

    /// <summary>
    /// Return the server description in HTML format. 
    /// </summary>
    public System.String GetHtmlDescription()
    {
      return "HTML description of pipe pressure drop server";
    }
  }
}
