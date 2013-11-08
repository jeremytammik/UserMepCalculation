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
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.ApplicationServices;


namespace UserStraightSegmentCalculationServers.Duct
{
  /// <summary>
  /// The external server that calculates the duct pressure drop.
  /// </summary>
  public class DuctPressureDropServer : IDuctPressureDropServer
  {
    private bool AlmostZero( double val )
    {
      return System.Math.Abs( val ) < double.Epsilon;
    }

    private bool AlmostEqual( double val1, double val2 )
    {
      double dSum = System.Math.Abs( val1 ) + System.Math.Abs( val2 );
      if( dSum < double.Epsilon )
        return true;

      return System.Math.Abs( val1 - val2 ) <= dSum * double.Epsilon;
    }

    /// <summary>
    /// Calculate the area of the duct.  
    /// </summary>
    /// <param name="eShape">
    /// The profile type of the duct.
    /// </param>
    /// <param name="dWidthOrDiameter">
    /// The diameter of duct with round profile, or the width of the duct with other profiles, Units: (ft).
    /// </param>
    /// <param name="dHeight">
    /// The height of the duct, Units: (ft).
    /// </param>
    /// <returns>
    /// The area of the duct, Units: (ft²).
    /// </returns>
    private double Area( ConnectorProfileType eShape, double dWidthOrDiameter, double dHeight )
    {
      double dArea = 0.0;

      if( eShape == ConnectorProfileType.Round )
        dArea = Math.PI * dWidthOrDiameter * dWidthOrDiameter / 4.0;
      else if( eShape == ConnectorProfileType.Rectangular )
        dArea = dWidthOrDiameter * dHeight;
      else if( eShape == ConnectorProfileType.Oval )
      {
        double dA;
        double dB;
        if( dWidthOrDiameter - dHeight > 0.0 )
        {
          dA = dWidthOrDiameter;
          dB = dHeight;
        }
        else
        {
          dA = dHeight;
          dB = dWidthOrDiameter;
        }

        dArea = ( Math.PI * dB * dB / 4.0 ) + dB * ( dA - dB );
      }

      return dArea;
    }

    /// <summary>
    /// Calculate the hydraulic diameter of the duct.  
    /// </summary>
    /// <param name="eShape">
    /// The profile type of the duct.
    /// </param>
    /// <param name="dWidthOrDiameter">
    /// The diameter of duct with round profile, or the width of the duct with other profiles, Units: (ft).
    /// </param>
    /// <param name="dHeight">
    /// The height of the duct, Units: (ft).
    /// </param>
    /// <returns>
    /// The hydraulic diameter of the duct, Units: (ft).
    /// </returns>
    private double HydraulicDiameter( ConnectorProfileType eShape, double dWidthOrDiameter, double dHeight )
    {
      double dHydraulicDiameter = dWidthOrDiameter;

      if( eShape == ConnectorProfileType.Oval )
      {
        double dA;
        double dB;
        if( dWidthOrDiameter - dHeight > 0.0 )
        {
          dA = dWidthOrDiameter;
          dB = dHeight;
        }
        else
        {
          dA = dHeight;
          dB = dWidthOrDiameter;
        }

        double dArea = ( Math.PI * dB * dB / 4.0 ) + dB * ( dA - dB );
        double dP = Math.PI * dB + 2.0 * ( dA - dB );

        if( !AlmostZero( dP ) )
          dHydraulicDiameter = 4.0 * dArea / dP;
        else
          dHydraulicDiameter = 0.0;
      }
      else if( eShape == ConnectorProfileType.Rectangular )
      {
        double dPerimeter = dWidthOrDiameter + dHeight;
        if( AlmostZero( dPerimeter ) )
          dHydraulicDiameter = 0.0;
        else
          dHydraulicDiameter = 2.0 * dWidthOrDiameter * dHeight / dPerimeter;
      }

      return dHydraulicDiameter;
    }

    /// <summary>
    /// Calculate the equivalent diameter of the duct.  
    /// </summary>
    /// <param name="eShape">
    /// The profile type of the duct.
    /// </param>
    /// <param name="dWidthOrDiameter">
    /// The diameter of duct with round profile, or the width of the duct with other profiles, Units: (ft).
    /// </param>
    /// <param name="dHeight">
    /// The height of the duct, Units: (ft).
    /// </param>
    /// <returns>
    /// The equivalent diameter of the duct, Units: (ft).
    /// </returns>
    double EquivalentDiameter( ConnectorProfileType eShape, double dWidthOrDiameter, double dHeight )
    {
      double dEqDiameter = dWidthOrDiameter;

      if( AlmostZero( dWidthOrDiameter + dHeight ) )
        return dEqDiameter;

      if( dWidthOrDiameter + dHeight < 0.0 )
        return 0.0;

      if( eShape == ConnectorProfileType.Oval )
      {
        if( !AlmostEqual( dWidthOrDiameter, dHeight ) )
        {
          double dA;
          double dB;
          if( dWidthOrDiameter - dHeight > 0.0 )
          {
            dA = dWidthOrDiameter;
            dB = dHeight;
          }
          else
          {
            dA = dHeight;
            dB = dWidthOrDiameter;
          }

          double dArea = ( Math.PI * dB * dB / 4.0 ) + dB * ( dA - dB );
          double dP = Math.PI * dB + 2.0 * ( dA - dB );

          dEqDiameter = 1.55 * Math.Pow( dArea, 0.625 ) / Math.Pow( dP, 0.250 );
        }
      }
      else if( eShape == ConnectorProfileType.Rectangular )
        dEqDiameter = 1.3 * Math.Pow( dWidthOrDiameter * dHeight, 0.625 ) / Math.Pow( dWidthOrDiameter + dHeight, 0.250 );

      return dEqDiameter;
    }

    /// <summary>
    /// Calculate the velocity of the duct.  
    /// </summary>
    /// <param name="eShape">
    /// The profile type of the duct.
    /// </param>
    /// <param name="dWidthOrDiameter">
    /// The diameter of duct with round profile, or the width of the duct with other profiles, Units: (ft).
    /// </param>
    /// <param name="dHeight">
    /// The height of the duct, Units: (ft).
    /// </param>
    /// <param name="dFlow">
    /// The flow of the duct, Units:(ft³/s).
    /// </param>
    /// <returns>
    /// The velocity of the duct, Units:(ft/s).
    /// </returns>
    private double Velocity( ConnectorProfileType eShape, double dWidthOrDiameter, double dHeight, double dFlow )
    {
      double dVelocity = 0.0;

      double dArea = Area( eShape, dWidthOrDiameter, dHeight );
      if( !AlmostZero( dArea ) )
        dVelocity = System.Math.Abs( dFlow ) / dArea;

      return dVelocity;
    }

    /// <summary>
    /// Calculate the velocity pressure of the duct.  
    /// </summary>
    /// <param name="dVelocity">
    /// The velocity of the duct, Units: (ft/s).
    /// </param>
    /// <param name="dDensity">
    /// The air density of the duct, Units: (kg/ft³).
    /// </param>
    /// <returns>
    /// The velocity pressure of the duct, Units: (kg/(ft·s²)).
    /// </returns>
    private double VelocityPressure( double dVelocity, double dDensity )
    {
      return dDensity * dVelocity * dVelocity / 2.0;
    }

    /// <summary>
    /// Calculate the Reynolds number of the duct.  
    /// </summary>
    /// <param name="dHydraulicDiameter">
    /// The hydraulic diameter of the duct, Units: (ft).
    /// </param>
    /// <param name="dVelocity">
    /// The velocity of the duct, Units: (ft/s).
    /// </param>
    /// <param name="dViscosity">
    /// The air viscosity of the duct, Units: (kg/(ft·s)).
    /// </param>
    /// <returns>
    /// The Reynolds number of the duct.
    /// </returns>
    private double ReynoldsNumber( double dHydraulicDiameter, double dVelocity, double dViscosity )
    {
      double dReynoldsNumber = 0.0;

      if( !AlmostZero( dViscosity ) )
        dReynoldsNumber = ( dHydraulicDiameter * dVelocity ) / dViscosity;

      return dReynoldsNumber;
    }

    /// <summary>
    /// Calculate the friction factor of the duct.  
    /// </summary>
    /// <param name="dHydraulicDiameter">
    /// The hydraulic diameter of the duct, Units: (ft).
    /// </param>
    /// <param name="dRoughness">
    /// The roughness of the duct, Units: (ft).
    /// </param>
    /// <param name="dReynoldsNumber">
    /// The Reynolds number of the duct.
    /// </param>
    /// <returns>
    /// The friction factor of the duct.
    /// </returns>
    private double AltshulTsalFrictionFactor( double dHydraulicDiameter, double dRoughness, double dReynoldsNumber )
    {
      double dFrictionFactor = 0.0;

      if( !AlmostZero( dHydraulicDiameter ) && !AlmostZero( dReynoldsNumber ) )
      {
        dFrictionFactor = 0.11 * Math.Pow( dRoughness / dHydraulicDiameter + 68.0 / dReynoldsNumber, 0.25 );
        if( dFrictionFactor < 0.018 )
          dFrictionFactor = 0.85 * dFrictionFactor + 0.0028;
      }

      return dFrictionFactor;
    }

    /// <summary>
    /// Calculate the pressure drop of the duct.  
    /// </summary>
    /// <param name="dHydraulicDiameter">
    /// The hydraulic diameter of the duct, Units: (ft).
    /// </param>
    /// <param name="dFrictionFactor">
    /// The friction factor of the duct.
    /// </param>
    /// <param name="dDensity">
    /// The air density of the duct, Units: (kg/ft³).
    /// </param>
    /// <param name="dVelocity">
    /// The velocity of the duct, Units: (ft/s).
    /// </param>
    /// <param name="dLength">
    /// The length of the duct, Units: (ft).
    /// </param>
    /// <returns>
    /// The pressure drop of the duct, Units: (kg/(ft·s²)).
    /// </returns>
    private double PressureDrop( double dHydraulicDiameter, double dFrictionFactor, double dDensity, double dVelocity, double dLength )
    {
      double dPressureDrop = 0.0;

      if( !AlmostZero( dHydraulicDiameter ) )
      {
        dPressureDrop = dFrictionFactor * dLength * dDensity * dVelocity * dVelocity / ( dHydraulicDiameter * 2.0 );
      }

      return dPressureDrop;
    }

    /// <summary>
    /// Calculate the duct pressure drop with the input data.
    /// </summary>
    /// <param name="data">
    /// The input for the pressure drop calculation. The output are also returned through the data to the caller. 
    /// </param>
    public void Calculate( DuctPressureDropData data )
    {
      ConnectorProfileType eShape = data.Shape;
      SystemCalculationLevel eLevel = data.Level;
      double dWidthOrDiameter = data.WidthOrDiameter;
      double dHeight = data.Height;

      double dHydraulicDiameter = 0.0;
      double dEquivalentDiameter = 0.0;
      double dVelocity = 0.0;
      double dVelocityPressure = 0.0;
      double dReynoldsNumber = 0.0;
      double dPressureDrop = 0.0;
      double dFriction = 0.0;
      double dCoefficient = 0.0;

      dHydraulicDiameter = HydraulicDiameter( eShape, dWidthOrDiameter, dHeight );
      dEquivalentDiameter = EquivalentDiameter( eShape, dWidthOrDiameter, dHeight );

      if( eLevel != SystemCalculationLevel.None )
      {
        double dDensity = data.Density;
        double dViscosity = data.Viscosity;

        dVelocity = Velocity( eShape, dWidthOrDiameter, dHeight, data.Flow );
        dVelocityPressure = VelocityPressure( dVelocity, dDensity );
        dReynoldsNumber = ReynoldsNumber( dHydraulicDiameter, dVelocity, dViscosity );

        if( eLevel == SystemCalculationLevel.All )
        {
          double dLength = data.Length;

          double dFrictionFactor = AltshulTsalFrictionFactor( dHydraulicDiameter, data.Roughness, dReynoldsNumber );
          dPressureDrop = PressureDrop( dHydraulicDiameter, dFrictionFactor, dDensity, dVelocity, dLength );

          if( !AlmostZero( dLength ) )
            dFriction = dPressureDrop / dLength; // Friction is the pressure drop at the unit length.

          if( !AlmostZero( dVelocityPressure ) )
            dCoefficient = dPressureDrop / dVelocityPressure;
        }
      }

      data.HydraulicDiameter = dHydraulicDiameter;
      data.EquivalentDiameter = dEquivalentDiameter;
      data.Velocity = dVelocity;
      data.VelocityPressure = dVelocityPressure;
      data.ReynoldsNumber = dReynoldsNumber;
      data.Friction = dFriction;
      data.PressureDrop = dPressureDrop;
      data.Coefficient = dCoefficient;
    }

    /// <summary>
    /// Return the method's name this server implements. 
    /// </summary>
    public string GetMethodName()
    {
      return Properties.Resources.DuctPressureDropMethodName;
    }

    /// <summary>
    /// Return the Id of the server. 
    /// </summary>
    public System.Guid GetServerId()
    {
      return new Guid( "042A10E0-8D24-46A4-9596-D192B3125D11" );
    }

    /// <summary>
    /// Return the Id of the service that the sever belongs to. 
    /// </summary>
    public ExternalServiceId GetServiceId()
    {
      return ExternalServices.BuiltInExternalServices.DuctPressureDropService;
    }

    /// <summary>
    /// Return the server's name. 
    /// </summary>
    public System.String GetName()
    {
      return Properties.Resources.DuctPressureDropServerName;
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
      return Properties.Resources.DuctPressureDropServerDescription;
    }

    /// <summary>
    /// Return the link of the server information. 
    /// </summary>
    public System.String GetInformationLink()
    {
      return Properties.Resources.DuctPressureDropServerInformationLink;
    }

    ///// <summary>
    ///// Return the server description in RTF format. 
    ///// </summary>
    //public System.String GetRTFDescription()
    //{
    //  return Properties.Resources.DuctPressureDropServerDescriptionRTF;
    //}

    /// <summary>
    /// Return the server description in HTML format. 
    /// </summary>
    public System.String GetHtmlDescription()
    {
      return "HTML description of duct pressure drop server";
    }
  }
}
