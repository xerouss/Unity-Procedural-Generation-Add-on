// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
//	File:			   BSPSeed.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// This class is responsible for getting and setting the Binary Space Partition variable values for the seed
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class BinarySpacePartitionSeed : GeneratorSeed
    {
        #region Constants
        const int numOfVariables = 6;
        #endregion

        #region Variables
        BinarySpacePartition m_binarySpacePartition;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BSP">The BSP script being used</param>
        public BinarySpacePartitionSeed(BinarySpacePartition BSP)
        {
            // Total number of variables, used when creating the seed and setting the seed value to the variables
            m_numOfVariablesUserCanChange = numOfVariables; 
            m_binarySpacePartition = BSP;
        }

        /// <summary>
        /// Get the value of one of the variables that the user can change
        /// </summary>
        /// <param name="index">The variable to get the value from</param>
        /// <returns>Variable value</returns>
        public override float GetUserVariable(int index)
        {
            switch (index)
            {
                case 0:
                    return m_binarySpacePartition.DungeonSize.x;
                case 1:
                    return m_binarySpacePartition.DungeonSize.y;
                case 2:
                    return m_binarySpacePartition.DungeonSize.z;
                case 3:
                    return m_binarySpacePartition.SplitAmount;
                case 4:
                    return m_binarySpacePartition.MinimumCellSize;
                case 5:
                    return m_binarySpacePartition.MinimumRoomSize;
                default:
                    Debug.Log("Incorrect index when getting user variable.");
                    return 0;
            }
        }

        /// <summary>
        /// Set a value to a variable that the user can change
        /// </summary>
        /// <param name="index">The variable to set the value to</param>
        /// <param name="value">The new value</param>
        public override void SetUserVariable(int index, float value)
        {
            switch (index)
            {
                case 0:
                    m_binarySpacePartition.DungeonSizeX= value;
                    break;
                case 1:
                    m_binarySpacePartition.DungeonSizeY = value;
                    break;
                case 2:
                    m_binarySpacePartition.DungeonSizeZ = value;
                    break;
                case 3:
                    m_binarySpacePartition.SplitAmount = (int)value;
                    break;
                case 4:
                    m_binarySpacePartition.MinimumCellSize = (int)value;
                    break;
                case 5:
                    m_binarySpacePartition.MinimumRoomSize = (int)value;
                    break;
                default:
                    Debug.Log("Incorrect index when setting user variable.");
                    break;
            }
        }
    }
}
