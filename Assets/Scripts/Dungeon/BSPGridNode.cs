// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   05/04/2018
//	File:			   BSPGridNode.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// Class for the binary space partition node for the grid
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class BSPGridNode
    {
        bool m_walkable;
        bool m_cellBoundary;

        public bool Walkable
        {
            get
            {
                return m_walkable;
            }

            set
            {
                m_walkable = value;
            }
        }

        public bool CellBoundary
        {
            get
            {
                return m_cellBoundary;
            }

            set
            {
                m_cellBoundary = value;
            }
        }
    }
}
