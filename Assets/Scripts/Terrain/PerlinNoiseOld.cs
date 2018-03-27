// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   27/03/2018
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
                                             new Vector2( 1f, 1f),
                                             new Vector2(-1f, 1f),
                                             new Vector2( 1f,-1f),
                                             new Vector2(-1f,-1f) };
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
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156, 151,160,137,91,90,15,
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

            int aaa = GetHashValue(gridX,       gridY,      gridZ);             // Bot left front
            int aba = GetHashValue(gridX,       gridY + 1,  gridZ);             // Top left front
            int aab = GetHashValue(gridX,       gridY,      gridZ + 1);  // Bot left back
            int abb = GetHashValue(gridX,       gridY + 1,  gridZ + 1);  // Top left back
            int baa = GetHashValue(gridX + 1,   gridY,      gridZ);             // Bot right front
            int bba = GetHashValue(gridX + 1,   gridY + 1,  gridZ);             // Top right front
            int bab = GetHashValue(gridX + 1,   gridY,      gridZ + 1);  // Bot right back
            int bbb = GetHashValue(gridX + 1,   gridY + 1,  gridZ + 1);  // Top right back

            float gradientAAA = Gradient(aaa, posX,     posY,       posZ);
            float gradientABA = Gradient(aba, posX,     posY - 1,   posZ);
            float gradientAAB = Gradient(aab, posX,     posY,       posZ - 1);
            float gradientABB = Gradient(abb, posX,     posY - 1,   posZ - 1);
            float gradientBAA = Gradient(baa, posX - 1, posY,       posZ);
            float gradientBBA = Gradient(bba, posX - 1, posY - 1,   posZ);
            float gradientBAB = Gradient(bab, posX - 1, posY,       posZ - 1);
            float gradientBBB = Gradient(bbb, posX - 1, posY - 1,   posZ - 1);

            // Get the fade value to use in the lerp
            // X/Y is reduced from the floor to make it between 0 and 1
            float fadeX = Fade(x - floorX);
            float fadeY = Fade(y - floorY);
            float fadeZ = Fade(z - floorZ);

            // Lerp between the points to get a gradual gradient
            // Front
            float topForwardSideLerp = Mathf.Lerp(gradientAAA, gradientBAA, fadeX);
            float botForwardSideLerp = Mathf.Lerp(gradientABA, gradientBBA, fadeX);
            float forwardLerpTotal = Mathf.Lerp(topForwardSideLerp, botForwardSideLerp, fadeY);

            // Back
            float topBackSideLerp = Mathf.Lerp(gradientAAB, gradientBAB, fadeX);
            float botBackSideLerp = Mathf.Lerp(gradientABB, gradientBBB, fadeX);
            float backLerpTotal = Mathf.Lerp(topBackSideLerp, botBackSideLerp, fadeY);


            float lerpTotal = Mathf.Lerp(forwardLerpTotal, backLerpTotal, fadeZ);

            // Normalise height since the height map only takes values between 0 and 1s
            float normalisedHeight = NormaliseFloat(normaliseMin, normaliseMax, lerpTotal);

            // Return the normalised value of the lerps
            return normalisedHeight;
        }

        /// <summary>
        /// Get the hash value of the location
        /// </summary>
        /// <param name="x">X pos</param>
        /// <param name="y">Y Pos</param>
        /// <param name="z">Z Pos</param>
        /// <returns>Hash value</returns>
        int GetHashValue(int x, int y, int z)
        {
            return hash[hash[hash[x] + y] + z];
        }

        /// <summary>
        /// Get the gradient of the position
        /// </summary>
        /// <param name="hash">The hash of the corner</param>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="z">Z Position</param>
        /// <returns>Gradient</returns>
        public float Gradient(int hash, float x, float y, float z)
        {
            const int bitwiseInvert = 0xF;
            const int hex0 = 0x0;
            const int hex1 = 0x1;
            const int hex2 = 0x2;
            const int hex3 = 0x3;
            const int hex4 = 0x4;
            const int hex5 = 0x5;
            const int hex6 = 0x6;
            const int hex7 = 0x7;
            const int hex8 = 0x8;
            const int hex9 = 0x9;
            const int hex10 = 0xA;
            const int hex11 = 0xB;
            const int hex12 = 0xC;
            const int hex13 = 0xD;
            const int hex14 = 0xE;
            const int hex15 = 0xF;

            // This is used instead of Ken Perlin's way of doing it
            // Because this is meant to be faster and easier to read

            switch (hash & bitwiseInvert)
            {
                case hex0: return x + y;
                case hex1: return -x + y;
                case hex2: return x - y;
                case hex3: return -x - y;
                case hex4: return x + z;
                case hex5: return -x + z;
                case hex6: return x - z;
                case hex7: return -x - z;
                case hex8: return y + z;
                case hex9: return -y + z;
                case hex10: return y - z;
                case hex11: return -y - z;
                case hex12: return y + x;
                case hex13: return -y + z;
                case hex14: return y - x;
                case hex15: return -y - z;
                default: return 0;
            }
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
        
        // 2D with permutation
        #region 2D with permutation
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

        float highestValue = 0;
        float lowestValue = 0;

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
            int hashRight = hash[(floorX & hashMask) + 1];
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

            int gradientBotLeft = hash[hashLeft + hashBot] & gradientMask;
            int gradientBotRight = hash[hashRight + hashBot] & gradientMask;
            int gradientTopLeft = hash[hashLeft + hashTop] & gradientMask;
            int gradientTopRight = hash[hashRight + hashTop] & gradientMask;

            // The dot product of each distance and gradient
            float dotTopLeft = Vector2.Dot(avaliableGradients[gradientBotLeft], distanceBotLeft);
            float dotTopRight = Vector2.Dot(avaliableGradients[gradientBotRight], distanceBotRight);
            float dotBotLeft = Vector2.Dot(avaliableGradients[gradientTopLeft], distanceTopLeft);
            float dotBotRight = Vector2.Dot(avaliableGradients[gradientTopRight], distanceTopRight);

            // Get the fade value to use in the lerp
            // X/Y is reduced from the floor to make it between 0 and 1
            float fadeX = Fade(pos.x);
            float fadeY = Fade(pos.y);

            // Lerp between the points to get a gradual gradient
            float topSideLerp = Mathf.Lerp(dotTopLeft, dotTopRight, fadeX);
            float botSideLerp = Mathf.Lerp(dotBotLeft, dotBotRight, fadeX);
            float lerpTotal = Mathf.Lerp(topSideLerp, botSideLerp, fadeY);

            // Normalise height since the height map only takes values between 0 and 1s
            float normalisedHeight = NormaliseFloat(0, 1, lerpTotal);

            if (normalisedHeight < lowestValue) lowestValue = normalisedHeight;
            if (normalisedHeight > highestValue) highestValue = normalisedHeight;

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

        // 2D my version
    
    }
}
