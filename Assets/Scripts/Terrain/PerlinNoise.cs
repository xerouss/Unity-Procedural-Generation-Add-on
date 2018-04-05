// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   05/04/2018
//	File:			   PerlinNoise.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// Original Class when first implementing Perlin Noise
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class PerlinNoise
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

        // FBM defaults
        const float defaultOctaves = 1;
        const float defaultFrequency = 6;
        const float defaultAmplitude = 1;
        const float defaultAmplitudeGain = 1;
        const float defaultLacunarity = 2;

        // Normalise
        const float normaliseMin = -1;
        const float normaliseMax = 1;

        // Terrain Settings
        const int terrainRes = 1024;
        const int terrainResPerBatch = 8;
        const int heightMapBaseX = 0;
        const int heightMapBaseY = 0;

        // Bit masks to prevent overflow of the array
        const int hashMask = 511;
        const int gradientMask = 7;

        // The positions of the points in the cube
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
        // The total size if 512
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
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,151,160,137,91,90,15,
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

        // Seed
        const int numOfVariablesUserCanChange = 14;
        const int makeBase0 = 1;
        const int goToNextVariable = 1;

        #endregion

        #region Variables

        // The terrain data used when creating the terrain
        TerrainData m_terrainData;

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

        string m_seed = "0";
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
                m_terrainSize = value;
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
                if (value.x < 0) value.x = 0;
                if (value.y < 0) value.y = 0;

                m_posOffset = value;
            }
        }

        public string Seed
        {
            get
            {
                return m_seed;
            }

            set
            {
                m_seed = value;
            }
        }
        #endregion

        public void SetSeedToVariables(string value)
        {
            // Don't need to change the variables if they are the same
            // Make sure the length is correct (is num of variable *2 because of the length num)
            if (m_seed == value || value.Length < numOfVariablesUserCanChange * 2) return;

            int numCheckValue = 0;
            bool numCheck;

            // Go through seed
            for (int i = 0; i < value.Length; i++)
            {
                // Check if it's a number
                numCheck = int.TryParse(value[i].ToString(), out numCheckValue);

                // If it isn't a number check if it is a - for negative number or . for decimal points
                // If it isn't one of them there is an incorrect character so don't set the variable to the seed
                if(!numCheck)
                {
                    if (value[i] != '-' && value[i] != '.') return;
                }
            }

            // -1 the length because it starts at base 0
            int indexLower = value.Length - makeBase0;
            int indexUpper = value.Length - makeBase0;
            string numberString = "";
            float numberFloat = 0;

            string valueString = value;

            // Go through all variables the user can change
            for (int i = numOfVariablesUserCanChange - 1; i >= 0; i--)
            {
                // Find where the variable value gets cut off
                indexLower -= (int)char.GetNumericValue(valueString[indexLower]);

                // Get the variable value
                for (int j = indexLower; j <= indexUpper - 1; j++)
                {
                    numberString += valueString[j];
                }

                // Change it to an int and set it to the correct variables
                numberFloat = float.Parse(numberString);
                SetUserVariable(i, numberFloat);

                // -1 from lower to move to the next variable's length
                // The upper is now the lower value since we are now on the next variable
                indexLower -= goToNextVariable;
                indexUpper = indexLower;

                // Reset the number since we don't want to keep adding to the previous variable
                numberString = "";
            }

            m_seed = value;
        }

        /// <summary>
        /// Updates the seed value with the current variable inputs
        /// </summary>
        /// <returns>The seed value</returns> 
        public string UpdateSeed()
        {
            string seedValue = "";
            string variableValueString = "";

            // Get all variable values
            for (int i = 0; i < numOfVariablesUserCanChange; i++)
            {
                // Convert the variable to string so the length of it can be added to the end of it
                variableValueString = GetUserVariable(i).ToString();
                variableValueString += variableValueString.Length.ToString();

                // Add it to the total seed value
                seedValue += variableValueString;
            }

            // Convert the value to an int and set it to the seed
             m_seed = seedValue;

            return m_seed;
        }

        /// <summary>
        /// Get the value of one of the variables that the user can change
        /// </summary>
        /// <param name="index">The variable to get the value from</param>
        /// <returns>Variable value</returns>
        float GetUserVariable(int index)
        {
            switch (index)
            {
                case 0:
                    return m_terrainSize.x;
                case 1:
                    return m_terrainSize.y;
                case 2:
                    return m_terrainSize.z;
                case 3:
                    return m_posOffset.x;
                case 4:
                    return m_posOffset.y;
                case 5:
                    return m_heightmapResolution;
                case 6:
                    return m_multiplyFade;
                case 7:
                    return m_minusFade;
                case 8:
                    return m_additionFade;
                case 9:
                    return m_octaves;
                case 10:
                    return m_frequency;
                case 11:
                    return m_amplitude;
                case 12:
                    return m_amplitudeGain;
                case 13:
                    return m_lacunarity;
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
        void SetUserVariable(int index, float value)
        {
            switch (index)
            {
                case 0:
                    m_terrainSize.x = value;
                    break;
                case 1:
                    m_terrainSize.y = value;
                    break;
                case 2:
                    m_terrainSize.z = value;
                    break;
                case 3:
                    m_posOffset.x = value;
                    break;
                case 4:
                    m_posOffset.y = value;
                    break;
                case 5:
                    m_heightmapResolution = (int)value;
                    break;
                case 6:
                    m_multiplyFade = (int)value;
                    break;
                case 7:
                    m_minusFade = (int)value;
                    break;
                case 8:
                    m_additionFade = (int)value;
                    break;
                case 9:
                    m_octaves = value;
                    break;
                case 10:
                    m_frequency = value;
                    break;
                case 11:
                    m_amplitude = value;
                    break;
                case 12:
                    m_amplitudeGain = value;
                    break;
                case 13:
                    m_lacunarity = value;
                    break;
                default:
                    Debug.Log("Incorrect index when setting user variable.");
                    break;
            }
        }

        /// <summary>
        /// Reset all variables the user can change back to their defaults
        /// </summary>
        public void ResetVariableValues()
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

        #region Perlin Noise Functions

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permutationSize">The size of the permutation grid</param>
        public PerlinNoise(int permutationSize)
        {
            m_terrainData = new TerrainData();
            m_terrainData.SetDetailResolution(terrainRes, terrainResPerBatch);
            m_terrainData.baseMapResolution = terrainRes;

            UpdateSeed();
        }

        /// <summary>
        /// Calculate the height value of the location using Perlin Noise
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <returns>Height</returns>
        public float Perlin(float x, float y)
        {
            // Get the lower bound of the square  
            int floorX = Mathf.FloorToInt(x);
            int floorY = Mathf.FloorToInt(y);

            // Get the hash values of the x axis of the square
            // Use the hash mask to prevent overflow
            // +1 to get the right side since the square is 1 in length

            //////////////////////////////////////////////
            // TODO WHY DO WE HASH THESE BUT NOT THE Y VALUES??
            //////////////////////////////////////////////
            int hashLeft = floorX & hashMask;
            int hashRight = hash[hashLeft + 1];
            hashLeft = hash[hashLeft];

            // Get the hash indexes of the y axis
            // Use the hash mask to prevent overflow
            // +1 to get the top side since the square is 1 in height
            int hashBot = floorY & hashMask;
            int hashTop = hashBot + 1;

            // Get the distance from the position to each corner
            // Convert the position into the cube location so each axis is between 0 and 1
            Vector2 pos = new Vector2(x - floorX, y - floorY);
            Vector2 distanceTopLeft = pos - topLeft;
            Vector2 distanceTopRight = pos - topRight;
            Vector2 distanceBotLeft = pos - botLeft;
            Vector2 distanceBotRight = pos - botRight;

            // Get the index to get the correct gradient
            // This is done by adding together the x and y axis hashes/indexes retrieved before
            // The mask is then used to prevent overflow
            int gradientBotLeftIndex = hash[hashLeft + hashBot] & gradientMask;
            int gradientBotRightIndex = hash[hashRight + hashBot] & gradientMask;
            int gradientTopLeftIndex = hash[hashLeft + hashTop] & gradientMask;
            int gradientTopRightIndex = hash[hashRight + hashTop] & gradientMask;

            // The dot product of each distance and gradient
            // The gradient is retrieved from the list of available gradients
            float dotTopLeft = Vector2.Dot(avaliableGradients[gradientBotLeftIndex], distanceBotLeft);
            float dotTopRight = Vector2.Dot(avaliableGradients[gradientBotRightIndex], distanceBotRight);
            float dotBotLeft = Vector2.Dot(avaliableGradients[gradientTopLeftIndex], distanceTopLeft);
            float dotBotRight = Vector2.Dot(avaliableGradients[gradientTopRightIndex], distanceTopRight);

            // Get the fade value to use in the lerp
            // Use the position locations so the values are not large
            // And so it matches up with the cube
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
            float frequency = Frequency;
            float amplitude = Amplitude;

            for (int i = 0; i < Octaves; i++)
            {
                // Make sure the position it looped tos prevent out of bound errors
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
        public void CreateTerrain()
        {
            // If there is no current terrain
            if (Terrain.activeTerrain == null)
            {
                // Create the terrain data with the default data
                Terrain.CreateTerrainGameObject(m_terrainData);
            }
            else Terrain.activeTerrain.terrainData = m_terrainData;
        }

        /// <summary>
        /// Create the terrain with Perlin Noise
        /// </summary>
        public void SetTerrainData()
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
                    xLocation = ((float)x / maxX) + m_posOffset.x;
                    yLocation = ((float)y / maxY) + m_posOffset.y;

                    // Get the height values using the perlin noise
                    heights[x, y] = Fractal(xLocation, yLocation);
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(heightMapBaseX, heightMapBaseY, heights);

            // Create the terrain with the new height map
            CreateTerrain();
        }

        #endregion
    }
}
