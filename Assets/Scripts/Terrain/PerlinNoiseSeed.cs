// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   22/04/2018
//	File:			   PerlinNoiseSeed.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// This class is responsible for getting and setting the Perlin Noise variable values
/// </summary>
namespace ProceduralGenerationAddOn
{
	public class PerlinNoiseSeed: GeneratorSeed
	{
        #region Constants
        const int numOfVariables = 14;
        #endregion

        #region Variables
        PerlinNoise m_perlinNoise;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="perlin">The Perlin Noise script being used</param>
		public PerlinNoiseSeed(PerlinNoise perlin)
		{
            m_numOfVariablesUserCanChange = numOfVariables;
			m_perlinNoise = perlin;
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
					return m_perlinNoise.TerrainSize.x;
				case 1:
					return m_perlinNoise.TerrainSize.y;
				case 2:
					return m_perlinNoise.TerrainSize.z;
				case 3:
					return m_perlinNoise.PosOffset.x;
				case 4:
					return m_perlinNoise.PosOffset.y;
				case 5:
					return m_perlinNoise.HeightmapResolution;
				case 6:
					return m_perlinNoise.MultiplyFade;
				case 7:
					return m_perlinNoise.MinusFade;
				case 8:
					return m_perlinNoise.AdditionFade;
				case 9:
					return m_perlinNoise.Octaves;
				case 10:
					return m_perlinNoise.Frequency;
				case 11:
					return m_perlinNoise.Amplitude;
				case 12:
					return m_perlinNoise.AmplitudeGain;
				case 13:
					return m_perlinNoise.Lacunarity;
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
					m_perlinNoise.TerrainSizeX = value;
					break;
				case 1:
					m_perlinNoise.TerrainSizeY = value;
					break;
				case 2:
					m_perlinNoise.TerrainSizeZ = value;
					break;
				case 3:
					m_perlinNoise.PosOffsetX = value;
					break;
				case 4:
					m_perlinNoise.PosOffsetY = value;
					break;
				case 5:
					m_perlinNoise.HeightmapResolution = (int)value;
					break;
				case 6:
					m_perlinNoise.MultiplyFade = (int)value;
					break;
				case 7:
					m_perlinNoise.MinusFade = (int)value;
					break;
				case 8:
					m_perlinNoise.AdditionFade = (int)value;
					break;
				case 9:
					m_perlinNoise.Octaves = value;
					break;
				case 10:
					m_perlinNoise.Frequency = value;
					break;
				case 11:
					m_perlinNoise.Amplitude = value;
					break;
				case 12:
					m_perlinNoise.AmplitudeGain = value;
					break;
				case 13:
					m_perlinNoise.Lacunarity = value;
					break;
				default:
					Debug.Log("Incorrect index when setting user variable.");
					break;
			}
		}
	}
}
