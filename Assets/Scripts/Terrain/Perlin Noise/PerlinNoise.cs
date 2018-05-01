// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   29/04/2018
//	File:			   PerlinNoise.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// The class that includes the Perlin Noise functionality
/// </summary>
namespace ProceduralGenerationAddOn
{
	public class PerlinNoise : ProceduralGenerationAlgorithm
	{
		#region Constants
		// Defaults
		const float defaultTerrainXSize = 50;
		const float defaultTerrainYSize = 10;
		const float defaultTerrainZSize = 50;
		const int defaultXOffset = 0;
		const int defaultYOffset = 0;
		const int defaultHeightmapRes = 128;
		const int defaultMultiplyFade = 6;
		const int defaultMinusFade = 15;
		const int defaultAdditionFade = 10;
        const int defaultMinValue = 0;

        // FBM 
        const float defaultOctaves = 1;
		const float defaultFrequency = 6;
		const float defaultAmplitude = 1;
		const float defaultAmplitudeGain = 1;
		const float defaultLacunarity = 2;
        const int maxOctaves = 10;

		// Normalise
		const float normaliseMin = -1;
		const float normaliseMax = 1;

		// Terrain Settings
		const int terrainRes = 1024;
		const int terrainResPerBatch = 8;
		const int heightMapBaseX = 0;
		const int heightMapBaseY = 0;
		const int maxTerrainSize = 100000;
		const int minTerrainYSize = -100000;
        public const int heightmapResLowerBound = 0;
        public const int heightmapResHigherBound = 256;

        // Bit masks to prevent overflow of the array
        const int hashMask = 511;
		const int gradientMask = 7;

		// The positions of the points in the square
		readonly Vector2 topLeft = new Vector2(0, 1);
		readonly Vector2 topRight = new Vector2(1, 1);
		readonly Vector2 botLeft = new Vector2(0, 0);
		readonly Vector2 botRight = new Vector2(1, 0);

		// 8 different gradients the points on the cube can be
		readonly Vector2[] avaliableGradients = { new Vector2( 1f, 0f),
												  new Vector2(-1f, 0f),
												  new Vector2( 0f, 1f),
												  new Vector2( 0f,-1f),
												  new Vector2( 1f, 1f),
												  new Vector2(-1f, 1f),
												  new Vector2( 1f,-1f),
												  new Vector2(-1f,-1f) };

		// Hash has 256 values but is repeated to prevent overflow of the array
		// The total size is 512
		// The larger size also allows bigger values for the octaves and tile amount
		readonly int[] hash = { 151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
			129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
			49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
			129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
			49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};
		#endregion

		#region Variables
		// The terrain data used when creating the terrain
		TerrainData m_terrainData;

		PerlinNoiseSeed m_seed; // The seed

		// Variables the user can change
		Vector3 m_terrainSize = new Vector3(defaultTerrainXSize, defaultTerrainYSize, defaultTerrainZSize);
		Vector2 m_posOffset = new Vector2(defaultXOffset, defaultYOffset);
		int m_heightmapResolution = defaultHeightmapRes;
		int m_multiplyFade = defaultMultiplyFade;
		int m_minusFade = defaultMinusFade;
		int m_additionFade = defaultAdditionFade;

		// Fractal Brownian Motion variables
		float m_octaves = defaultOctaves;             // Amount of times it iterates
		float m_frequency = defaultFrequency;            // How much the bumps are spread out
		float m_amplitude = defaultAmplitude;           // How flat it is
		float m_amplitudeGain = defaultAmplitudeGain;    // How much the amplitude increases after each iteration
		float m_lacunarity = defaultLacunarity;         // How much the frequency is increased after each iteration
		#endregion

		#region Properties
		// Can't set the terrain size and the heightmap res in the
		// Property because it will randomly delete the level heightmap

		public Vector3 TerrainSize
		{
			get
			{
				return m_terrainSize;
			}

			set
			{
				TerrainSizeX = value.x;
				TerrainSizeY = value.y;
				TerrainSizeZ = value.z;
			}
		}

		// Need individual property for the x and y since they can't be set individually from the vector 3 one
		public float TerrainSizeX
		{
			set
			{
				// Prevent extreme values
				// Make sure the x don't go below 0 since it makes the terrain inside out
				m_terrainSize.x = CheckIfValueIsInbetween(value, defaultMinValue, maxTerrainSize);
			}
		}

		public float TerrainSizeY
		{
			set
			{
				// Prevent extreme values
				m_terrainSize.y = CheckIfValueIsInbetween(value, minTerrainYSize, maxTerrainSize);
			}
		}

		public float TerrainSizeZ
		{
			set
			{
				// Prevent extreme values
				// Make sure the z don't go below 0 since it makes the terrain inside out
				m_terrainSize.z = CheckIfValueIsInbetween(value, defaultMinValue, maxTerrainSize);
			}
		}


		public int HeightmapResolution
		{
			get
			{
				return m_heightmapResolution;
			}

			set
			{
				m_heightmapResolution = value;
			}
		}

		public int MultiplyFade
		{
			get
			{
				return m_multiplyFade;
			}

			set
			{
				m_multiplyFade = value;
			}
		}

		public int MinusFade
		{
			get
			{
				return m_minusFade;
			}

			set
			{
				m_minusFade = value;
			}
		}

		public int AdditionFade
		{
			get
			{
				return m_additionFade;
			}

			set
			{
				m_additionFade = value;
			}
		}

		public float Octaves
		{
			get
			{
				return m_octaves;
			}

			set
			{
				// Less than 0 does nothing so cap it
                // Cap the max value or else it can crash the editor
				value = CheckIfValueIsInbetween(value, defaultMinValue, maxOctaves);
				m_octaves = value;
			}
		}

		public float Frequency
		{
			get
			{
				return m_frequency;
			}

			set
			{
				// Having less than 0 produces an error
				value = CheckIfValueIsLess(value, defaultMinValue);
				m_frequency = value;
			}
		}

		public float Amplitude
		{
			get
			{
				return m_amplitude;
			}

			set
			{
                // Having less than 0 does nothing
                value = CheckIfValueIsLess(value, defaultMinValue);
                m_amplitude = value;
			}
		}

		public float AmplitudeGain
		{
			get
			{
				return m_amplitudeGain;
			}

			set
			{
				// Having less than 0 does nothing
				value = CheckIfValueIsLess(value, defaultMinValue);
				m_amplitudeGain = value;
			}
		}

		public float Lacunarity
		{
			get
			{
				return m_lacunarity;
			}

			set
			{
				// Causes errors if less than 0 so make sure it doesn't go below it
				value = CheckIfValueIsLess(value, defaultMinValue);
				m_lacunarity = value;
			}
		}

		public Vector2 PosOffset
		{
			get
			{
				return m_posOffset;
			}

			set
			{
				// Make sure the offset doesn't go less than 0 since it produces an error
				value.x = CheckIfValueIsLess(value.x, defaultMinValue);
				value.y = CheckIfValueIsLess(value.y, defaultMinValue);

				m_posOffset = value;
			}
		}

		// Need individual property for the x and y since they can't be set individually from the vector 2 one
		public float PosOffsetX
		{
			set
			{
				// Make sure the offset doesn't go less than 0 since it produces an error
				value = CheckIfValueIsLess(value, defaultMinValue);
				m_posOffset.x = value;
			}
		}

		public float PosOffsetY
		{
			set
			{
				// Make sure the offset doesn't go less than 0 since it produces an error
				value = CheckIfValueIsLess(value, defaultMinValue);
				m_posOffset.y = value;
			}
		}

		public PerlinNoiseSeed SeedClass
		{
			get
			{
				return m_seed;
			}
		}
		#endregion

		// Functions
		#region Perlin Noise Functions

		/// <summary>
		/// Constructor
		/// </summary>
		public PerlinNoise()
		{
			m_terrainData = new TerrainData();
			m_terrainData.SetDetailResolution(terrainRes, terrainResPerBatch);
			m_terrainData.baseMapResolution = terrainRes;

			m_seed = new PerlinNoiseSeed(this);
			m_seed.UpdateSeed();
		}

        /// <summary>
        /// Calculate the height value of the location using Perlin Noise
        /// Used http://catlikecoding.com/unity/tutorials/noise/ to help with the hash values
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <returns>Height</returns>
        public float Perlin(float x, float y)
		{
			// The perlin noise works by getting the square corners which the passed location is in
			// These square corners each have gradients and based on the distance between
			// The current location and each square corner will determine the height of the location

			// **********************************************************************
			// Get the square values

			// Get the lower bounds of the square  
			int floorX = Mathf.FloorToInt(x);
			int floorY = Mathf.FloorToInt(y);

			// Get the hash values of the x axis of the square
			// Use the hash mask to prevent overflow
			// +1 to get the right side since the square is 1 in length
			// The x axis is hashed because it is added to the y later
			int hashLeft = floorX & hashMask;
			int hashRight = hash[hashLeft + 1];
			hashLeft = hash[hashLeft];

			// Get the hash indexes of the y axis
			// Use the hash mask to prevent overflow
			// +1 to get the top side since the square is 1 in height
			int hashBot = floorY & hashMask;
			int hashTop = hashBot + 1;
			// **********************************************************************

			// **********************************************************************
			// Get the dot product of gradient and distance
			// This outputs the direction of the gradients for the corners

			// Get the distance from the position to each corner
			// Convert the position into the square location so each axis is between 0 and 1
			Vector2 pos = new Vector2(x - floorX, y - floorY);
			Vector2 distanceTopLeft = pos - topLeft;
			Vector2 distanceTopRight = pos - topRight;
			Vector2 distanceBotLeft = pos - botLeft;
			Vector2 distanceBotRight = pos - botRight;

			// Get the index of the correct gradient
			// This is done by adding together the x and y axis hashes/indexes retrieved before
			// The x hashed value is added to the y value to get the correct hash value
			// The mask is then used to prevent overflow
			int gradientBotLeftIndex = hash[hashLeft + hashBot] & gradientMask;
			int gradientBotRightIndex = hash[hashRight + hashBot] & gradientMask;
            int gradientTopLeftIndex = hash[hashLeft + hashTop] & gradientMask;
            int gradientTopRightIndex = hash[hashRight + hashTop] & gradientMask;

			// The dot product of each distance and gradient to get directions
			// The gradient is retrieved from the list of available gradients
			float dotTopLeft = Vector2.Dot(avaliableGradients[gradientBotLeftIndex], distanceBotLeft);
			float dotTopRight = Vector2.Dot(avaliableGradients[gradientBotRightIndex], distanceBotRight);
			float dotBotLeft = Vector2.Dot(avaliableGradients[gradientTopLeftIndex], distanceTopLeft);
			float dotBotRight = Vector2.Dot(avaliableGradients[gradientTopRightIndex], distanceTopRight);
			// **********************************************************************

			// **********************************************************************
			// Get the height value

			// Get the fade value to use in the lerp to smooth out the height change
			// This prevents artefacts (spikes in the terrain)
			// Use the position locations so the values are not large
			// And so it matches up with the square
			float fadeX = Fade(pos.x);
			float fadeY = Fade(pos.y);

			// Lerp between the points to get a gradual gradient
			float topSideLerp = Mathf.Lerp(dotTopLeft, dotTopRight, fadeX);
			float botSideLerp = Mathf.Lerp(dotBotLeft, dotBotRight, fadeX);
			float lerpTotal = Mathf.Lerp(topSideLerp, botSideLerp, fadeY);

			// Normalise height since the height map only takes values between 0 and 1s
			float normalisedHeight = NormaliseFloat(normaliseMin, normaliseMax, lerpTotal);

			// Return the normalised value of the lerps
			return normalisedHeight;
			// **********************************************************************
		}

		/// <summary>
		/// Normalise float between two values
		/// </summary>
		/// <param name="min">Minimum the value can be</param>
		/// <param name="max">Maximum the value can be</param>
		/// <param name="value">The value to normalise</param>
		/// <returns>Normalised value</returns>
		public float NormaliseFloat(float min, float max, float value)
		{
			return (value - min) / (max - min);
		}

		/// <summary>
		/// Fade the value to smooth the height
		/// </summary>
		/// <param name="value">Value to fade</param>
		/// <returns>Faded value</returns>
		public float Fade(float value)
		{
			// Original Equation: 6t^5-15t^4+10t^3
			return value * value * value * (value * (value * m_multiplyFade - m_minusFade) + m_additionFade);
		}

		#endregion

		#region Fractal Brownian Motion Functions

		/// <summary>
		/// Fractal Brownian Motion to be applied on the perlin for better results
		/// </summary>
		/// <param name="x">X Position</param>
		/// <param name="y">Y Position</param>
		/// <returns>Height</returns>
		public float Fractal(float x, float y)
		{
			float height = 0;
			float frequency = Frequency; // How spread out the terrain is
			float amplitude = Amplitude; // How tall the terrain is

			for (int i = 0; i < Octaves; i++)
			{
				// Make sure the position it looped to prevent out of bound errors
				x = LoopPos(x * frequency, m_terrainData.heightmapWidth);
				y = LoopPos(y * frequency, m_terrainData.heightmapHeight);

				height = Perlin(x, y) * amplitude;

				// Change the values for the next octave
				frequency *= Lacunarity;
				amplitude *= AmplitudeGain;
			}

			return height;
		}

		/// <summary>
		/// Make sure the position loops back to the start of the terrain
		/// </summary>
		/// <param name="value">Position Value</param>
		/// <param name="max">Max it can be</param>
		/// <returns>Position</returns>
		public float LoopPos(float value, int max)
		{
            max--;
			if (value >= max)
			{
				// Reduce the max from it so it goes back to the start
				value = value % max;
			}

			return value;
		}

		#endregion

		#region Terrain Functions

		/// <summary>
		/// Get the terrain in the scene, if there isn't spawn one
		/// </summary>
		/// /// <param name="createNewTerrain">Whether to create a new terrain game object or not</param>
		public void CreateTerrain(bool createNewTerrain)
		{
			// If there is no current terrain
			if (Terrain.activeTerrain == null || createNewTerrain)
			{
				// Create the terrain with the data
				GameObject createdTerrain = Terrain.CreateTerrainGameObject(m_terrainData);

				// Attach the PerlinNoiseTerrain to the terrain so the editor can find it
				// When the user wants to delete it
				createdTerrain.AddComponent<GeneratedTerrain>();
			}
			// If there is a terrain just set the new data to it to change it
			else Terrain.activeTerrain.terrainData = m_terrainData;
		}

		/// <summary>
		/// Create the terrain with Perlin Noise
		/// <param name="createNewTerrain">Whether to create a new terrain game object or not</param>
		/// </summary>
		public void SetTerrainData(bool createNewTerrain)
		{
			// Set the heightmap res and terrain size here or else it won't change
			// Can't do it in the property because it will randomly delete the level heightmap
			m_terrainData.heightmapResolution = m_heightmapResolution;
			m_terrainData.size = m_terrainSize;

			// Get the width and height of the terrain's height map
			int maxX = m_terrainData.heightmapWidth;
			int maxY = m_terrainData.heightmapHeight;

			// Array to store the heights
			float[,] heights = new float[maxX, maxY];
			float xLocation = 0;
			float yLocation = 0;

			// Go through the height map
			for (int x = 0; x < maxX; x++)
			{
				for (int y = 0; y < maxY; y++)
				{
					// Make sure the height map doesn't follow the grid 1 for 1
					// Since we want multiple values in the same square to get a smoother terrain
					xLocation = ((float)x / maxX) + m_posOffset.x;
					yLocation = ((float)y / maxY) + m_posOffset.y;

					// Get the height values using the perlin noise
					heights[x, y] = Fractal(xLocation, yLocation);
				}
			}

			// Set the height map to the terrain data
			m_terrainData.SetHeights(heightMapBaseX, heightMapBaseY, heights);

			// Create the terrain with the new height map
			CreateTerrain(createNewTerrain);
		}

		#endregion

		#region Other Functions
		/// <summary>
		/// Reset all variables the user can change back to their defaults
		/// </summary>
		public override void ResetVariableValues()
		{
			m_terrainSize = new Vector3(defaultTerrainXSize, defaultTerrainYSize, defaultTerrainZSize);
			m_posOffset = new Vector2(defaultXOffset, defaultYOffset);
			m_heightmapResolution = defaultHeightmapRes;
			m_multiplyFade = defaultMultiplyFade;
			m_minusFade = defaultMinusFade;
			m_additionFade = defaultAdditionFade;
			m_octaves = defaultOctaves;
			m_frequency = defaultFrequency;
			m_amplitude = defaultAmplitude;
			m_amplitudeGain = defaultAmplitudeGain;
			m_lacunarity = defaultLacunarity;
		}
		#endregion
	}
}
