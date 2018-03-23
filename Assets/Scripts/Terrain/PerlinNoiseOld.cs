// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/03/2018
//	File:			   Perlin Noise Old.cs
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
    public class PerlinNoiseOld
    {
        #region Constants
        const float defaultTerrainXSize = 50;
        const float defaultTerrainYSize = 5;
        const float defaultTerrainZSize = 50;
        const int defaultHeightmapRes = 32;
        const int defaultMultiplyFade = 6;
        const int defaultMinusFade = 15;
        const int defaultAdditionFade = 10;
        const float defaultDistanceModifier = 6;
        const int arrayX = 0;
        const int arrayY = 1;
        const int arrayZ = 2;

        const float normaliseMin = -1;
        const float normaliseMax = 1;
        const int fadeLeftPow = 5;
        const int fadeMidPow = 4;
        const int fadeRightPow = 3;

        const int terrainRes = 1024;
        const int terrainResPerBatch = 8;

        const float octavesDefault = 1;
        const float frequencDefault = 1;
        const float amplitudeDefault = 0.5f;
        const float amplitudeGainDefault = 0.5f;
        const float lacunarityDefault = 2;

        const int hashMask = 255;
        const int gradientMask = 7;
        Vector2[] avaliableGradients = { new Vector2( 1f, 0f),
                                             new Vector2(-1f, 0f),
                                             new Vector2( 0f, 1f),
                                             new Vector2( 0f,-1f),
                                             new Vector2( 1f, 1f).normalized,
                                             new Vector2(-1f, 1f).normalized,
                                             new Vector2( 1f,-1f).normalized,
                                             new Vector2(-1f,-1f).normalized };
        int[] hash = { 151,160,137,91,90,15,
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

        // 3D Grid
        /*
        #region 3D
        #region Variables

        PNGridNode[,,] m_permutation;
        TerrainData m_terrainData;

        // Variables the user can change
        Vector3 m_terrainSize = new Vector3(defaultTerrainXSize, defaultTerrainYSize, defaultTerrainZSize);
        int m_heightmapResolution = defaultHeightmapRes;
        int m_multiplyFade = defaultMultiplyFade;
        int m_minusFade = defaultMinusFade;
        int m_additionFade = defaultAdditionFade;
        float m_tileAmount = defaultDistanceModifier;

        // Fractal Brownian Motion variables
        float m_octaves = octavesDefault;             // Amount of times it iterates
        float m_frequency = frequencDefault;            // How much the bumps are spread out
        float m_amplitude = amplitudeDefault;           // How flat it is
        float m_amplitudeGain = amplitudeGainDefault;    // How much the amplitude increases after each iteration
        float m_lacunarity = lacunarityDefault;         // How much the frequency is increased after each iteration

        float m_seed = 0;
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

        public float TileAmount
        {
            get
            {
                return m_tileAmount;
            }

            set
            {
                m_tileAmount = value;
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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permutationSize">The size of the permutation grid</param>
        public PerlinNoiseOld(int permutationSize)
        {
            m_terrainData = new TerrainData();
            m_terrainData.SetDetailResolution(terrainRes, terrainResPerBatch);
            m_terrainData.baseMapResolution = terrainRes;

            m_permutation = new PNGridNode[permutationSize, permutationSize, permutationSize];
        }

        /// <summary>
        /// Calculate the height value of the location using Perlin Noise
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <returns>Height</returns>
        public float perlin(float x, float y, float z)
        {
            // Floor/ceil for grid positions
            // + 1 the values then floor them to round up
            int floorX = Mathf.FloorToInt(x);
            int floorY = Mathf.FloorToInt(y);
            int floorZ = Mathf.FloorToInt(z);

            float posX = x - floorX;
            float posY = y - floorY;
            float posZ = z - floorZ;

            int gridX = floorX & 255;
            int gridY = floorY & 255;
            int gridZ = floorZ & 255;

            /*
            // Each corner of the cube that the position is int
            Vector3 forwardTopLeft =    new Vector3(floorX, ceilY, floorZ);
            Vector3 forwardTopRight =   new Vector3(ceilX, ceilY, floorZ);
            Vector3 forwardBotLeft =    new Vector3(floorX, floorY, floorZ);
            Vector3 forwardBotRight =   new Vector3(ceilX, floorY, floorZ);
            Vector3 backTopLeft =       new Vector3(floorX, ceilY, ceilZ);
            Vector3 backTopRight =      new Vector3(ceilX, ceilY, ceilZ);
            Vector3 backBotLeft =       new Vector3(floorX, floorY, ceilZ);
            Vector3 backBotRight =      new Vector3(ceilX, floorY, ceilZ);

            // Get the distance from the position to each corner
            Vector3 pos = new Vector3(x, y, z);
            Vector3 distanceForwardTopLeft =    pos - forwardTopLeft;
            Vector3 distanceForwardTopRight =   pos - forwardTopRight;
            Vector3 distanceForwardBotLeft =    pos - forwardBotLeft;
            Vector3 distanceForwardBotRight =   pos - forwardBotRight;
            Vector3 distanceBackTopLeft =       pos - backTopLeft;
            Vector3 distanceBackTopRight =      pos - backTopRight;
            Vector3 distanceBackBotLeft =       pos - backBotLeft;
            Vector3 distanceBackBotRight =      pos - backBotRight;
            

            // The dot product of each distance and gradient
            float dotForwardTopLeft =   Vector3.Dot(m_permutation[floorX, ceilY, floorZ].Gradient, distanceForwardTopLeft);
            float dotForwardTopRight =  Vector3.Dot(m_permutation[ceilX, ceilY, floorZ].Gradient, distanceForwardTopRight);
            float dotForwardBotLeft =   Vector3.Dot(m_permutation[floorX, floorY, floorZ].Gradient, distanceForwardBotLeft);
            float dotForwardBotRight =  Vector3.Dot(m_permutation[ceilX, floorY, floorZ].Gradient, distanceForwardBotRight);
            float dotBackTopLeft =      Vector3.Dot(m_permutation[floorX, ceilY, ceilZ].Gradient, distanceBackTopLeft);
            float dotBackTopRight =     Vector3.Dot(m_permutation[ceilX, ceilY, ceilZ].Gradient, distanceBackTopRight);
            float dotBackBotLeft =      Vector3.Dot(m_permutation[floorX, floorY, ceilZ].Gradient, distanceBackBotLeft);
            float dotBackBotRight =     Vector3.Dot(m_permutation[ceilX, floorY, ceilZ].Gradient, distanceBackBotRight);

            // Get the fade value to use in the lerp
            // X/Y is reduced from the floor to make it between 0 and 1
            float fadeX = Fade(x - floorX);
            float fadeY = Fade(y - floorY);
            float fadeZ = Fade(z - floorZ);

            // Lerp between the points to get a gradual gradient
            // Front
            float topForwardSideLerp = Mathf.Lerp(dotForwardTopLeft, dotForwardTopRight, fadeX);
            float botForwardSideLerp = Mathf.Lerp(dotForwardBotLeft, dotForwardBotRight, fadeX);
            float forwardLerpTotal = Mathf.Lerp(topForwardSideLerp, botForwardSideLerp, fadeY);

            // Back
            float topBackSideLerp = Mathf.Lerp(dotBackTopLeft, dotBackTopRight, fadeX);
            float botBackSideLerp = Mathf.Lerp(dotBackBotLeft, dotBackBotRight, fadeX);
            float backLerpTotal = Mathf.Lerp(topBackSideLerp, botBackSideLerp, fadeY);


            float lerpTotal = Mathf.Lerp(forwardLerpTotal, backLerpTotal, fadeZ);

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
            return (value + 1) / 2; //(value - min) / (max - min);
        }

        /// <summary>
        /// Fade the value to smooth the height
        /// </summary>
        /// <param name="value">Value to fade</param>
        /// <returns>Faded value</returns>
        public float Fade(float value)
        {
            return value * value * value * (value * (value * 6f - 15f) + 10f);
        }

        /// <summary>
        /// Fractal Brownian Motion to be applied on the perlin for better results
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <returns>Height</returns>
        public float Fractal(float x, float y, float z)
        {
            float height = 0;
            float frequency = Frequency;
            float amplitude = Amplitude;

            for (int i = 0; i < Octaves; i++)
            {
                // Make sure the position it looped tos prevent out of bound errors
                x = LoopPos(x * frequency, m_terrainData.heightmapWidth);
                y = LoopPos(y * frequency, m_terrainData.heightmapHeight); // TODO Add max height?
                z = LoopPos(z * frequency, m_terrainData.heightmapHeight);

                height = perlin(x, y, z) * amplitude;

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

            //SetGridGradients();

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
                    xLocation = ((float)x / maxX) * m_tileAmount;
                    yLocation = ((float)y / maxY) * m_tileAmount;

                    // Get the height values using the perlin noise
                    heights[x, y] = Fractal(xLocation, yLocation, xLocation + yLocation);
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(0, 0, heights);

            // Create the terrain with the new height map
            CreateTerrain();
        }
        #endregion

        */
        // 2D
       

        #region 2D
        #region Variables

        PNGridNode[,] m_gradients;
        TerrainData m_terrainData;

        // Variables the user can change
        Vector3 m_terrainSize = new Vector3(defaultTerrainXSize, defaultTerrainYSize, defaultTerrainZSize);
        int m_heightmapResolution = defaultHeightmapRes;
        int m_multiplyFade = defaultMultiplyFade;
        int m_minusFade = defaultMinusFade;
        int m_additionFade = defaultAdditionFade;
        float m_tileAmount = defaultDistanceModifier;

        // Fractal Brownian Motion variables
        float m_octaves = octavesDefault;             // Amount of times it iterates
        float m_frequency = frequencDefault;            // How much the bumps are spread out
        float m_amplitude = amplitudeDefault;           // How flat it is
        float m_amplitudeGain = amplitudeGainDefault;    // How much the amplitude increases after each iteration
        float m_lacunarity = lacunarityDefault;         // How much the frequency is increased after each iteration

        float m_seed = 0;
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

        public float TileAmount
        {
            get
            {
                return m_tileAmount;
            }

            set
            {
                m_tileAmount = value;
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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permutationSize">The size of the permutation grid</param>
        public PerlinNoiseOld(int permutationSize)
        {
            m_terrainData = new TerrainData();
            m_terrainData.SetDetailResolution(terrainRes, terrainResPerBatch);
            m_terrainData.baseMapResolution = terrainRes;

            m_gradients = new PNGridNode[permutationSize, permutationSize];

            SetGridGradients();

        }

        /// <summary>
        /// Calculate the height value of the location using Perlin Noise
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <returns>Height</returns>
        public float perlin(float x, float y)
        {
            // Floor/ceil for grid positions
            // + 1 the values then floor them to round up            
            int floorX = Mathf.FloorToInt(x);
            int floorY = Mathf.FloorToInt(y);
            int ceilX = floorX + 1;
            int ceilY = floorY + 1;


            int hashLeft = hash[floorX & hashMask];
            int hashRight = hash[hashLeft + 1];
            int hashBot = floorY & hashMask;
            int hashTop = hashBot + 1;

            // Each corner of the cube that the position is int
            Vector2 topLeft = new Vector2(0, 1);
            Vector2 topRight = new Vector2(1, 1);
            Vector2 botLeft = new Vector2(0, 0);
            Vector2 botRight = new Vector2(1, 0);

            // Get the distance from the position to each corner
            Vector2 pos = new Vector2(x - floorX, y - floorY);
            Vector2 distanceTopLeft = pos - topLeft;
            Vector2 distanceTopRight = pos - topRight;
            Vector2 distanceBotLeft = pos - botLeft;
            Vector2 distanceBotRight = pos - botRight;

            // The dot product of each distance and gradient
            float dotTopLeft = Vector2.Dot(avaliableGradients[hash[hashLeft + hashBot] & gradientMask], distanceTopLeft);
            float dotTopRight = Vector2.Dot(avaliableGradients[hash[hashRight + hashBot] & gradientMask], distanceTopRight);
            float dotBotLeft = Vector2.Dot(avaliableGradients[hash[hashLeft + hashTop] & gradientMask], distanceBotLeft);
            float dotBotRight = Vector2.Dot(avaliableGradients[hash[hashRight + hashTop] & gradientMask], distanceBotRight);

            // Get the fade value to use in the lerp
            // X/Y is reduced from the floor to make it between 0 and 1
            float fadeX = Fade(pos.x);
            float fadeY = Fade(pos.y);

            // Lerp between the points to get a gradual gradient
            float topSideLerp = Mathf.Lerp(dotTopLeft, dotTopRight, fadeX);
            float botSideLerp = Mathf.Lerp(dotBotLeft, dotBotRight, fadeX);
            float lerpTotal = Mathf.Lerp(topSideLerp, botSideLerp, fadeY);

            // Normalise height since the height map only takes values between 0 and 1s
            float normalisedHeight = lerpTotal * Mathf.Sqrt(2);

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
            return (value + 1) / 2; //(value - min) / (max - min);
        }

        /// <summary>
        /// Fade the value to smooth the height
        /// </summary>
        /// <param name="value">Value to fade</param>
        /// <returns>Faded value</returns>
        public float Fade(float value)
        {
            return value * value * value * (value * (value * 6f - 15f) + 10f);
            //return (Mathf.Pow(m_multiplyFade * value, fadeLeftPow) - (Mathf.Pow(m_minusFade * value, fadeMidPow)) + (Mathf.Pow(m_additionFade * value, fadeRightPow)));
        }

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

                height = perlin(x, y) * amplitude;

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

        /// <summary>
        /// Set the gradients for the permutation
        /// </summary>
        public void SetGridGradients()
        {
            // All the gradients that can be used for 2D perlin noise
            Vector3[] avaliableGradients = { new Vector2( 1f, 0f),
                                             new Vector2(-1f, 0f),
                                             new Vector2( 0f, 1f),
                                             new Vector2( 0f,-1f),
                                             new Vector2( 1f, 1f).normalized,
                                             new Vector2(-1f, 1f).normalized,
                                             new Vector2( 1f,-1f).normalized,
                                             new Vector2(-1f,-1f).normalized };

            // Loop through the permutation grid and assign a random gradient
            for (int x = 0; x < m_gradients.GetLength(arrayX); x++)
            {
                for (int y = 0; y < m_gradients.GetLength(arrayY); y++)
                {
                    // Create the nodes for the grid position so the gradient can be set
                    m_gradients[x, y] = new PNGridNode();

                    // Randomly select one of the gradients
                    m_gradients[x, y].Gradient = avaliableGradients[Random.Range(0, avaliableGradients.Length)];
                }
            }
        }

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

            //SetGridGradients();

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
                    xLocation = ((float)x / maxX) * m_tileAmount;
                    yLocation = ((float)y / maxY) * m_tileAmount;

                    // Get the height values using the perlin noise
                    heights[x, y] = perlin(xLocation, yLocation); //Fractal(xLocation, yLocation);
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(0, 0, heights);

            // Create the terrain with the new height map
            CreateTerrain();
        }
#endregion
    
    }
}
