// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   27/02/2018
//	File:			   PN Grid Node.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// This class will be used as the grid nodes for the Perlin Noise
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class PNGridNode
    {
        Vector3 m_gradient;

        // Property to get/set the gradient
        public Vector3 Gradient
        {
            get
            {
                return m_gradient;
            }

            set
            {
                m_gradient = value;
            }
        }
    }
}
