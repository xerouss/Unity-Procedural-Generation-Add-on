// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   06/03/2018
//	File:			   Perlin Noise.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// This class will be used to create levels using Perlin Noise
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class PerlinNoise
    {
        const int permutationLength = 256;
        const int permutationLengthBase0 = 255;
        const int overFlowPermutaitonIncrease = 2;

        int m_repeatAmount = 0;
        TerrainData m_terrainData;

        // The permutation table which is stated by Ken Perlin
        // It is a hash table that has random number from 0-255 placed within
        readonly int[] m_permutation = { 151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};

        // Will determine gradients
        // The permutation table which will be used
        // Is the overflow permutation for the values that are needed one above the max in the permutation
        int[] m_overPerm;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerlinNoise()
        {
            // Double the length of the original permutation table so overlapping does give an error
            // E.g. doing the permutation length base 0 final location would case an error since it has no location to get gradients from
            m_overPerm = new int[permutationLength * overFlowPermutaitonIncrease];

            // Fill it with value from the permutation table
            for (int i = 0; i < permutationLength; i++)
            {
                m_overPerm[i] = m_permutation[i % permutationLength];
            }

            // Create the terrain data to output the perlin to
            m_terrainData = new TerrainData();

            // Y changes how high the spikes are
            m_terrainData.size = new Vector3(10, 1, 10);
        }

        /// <summary>
        /// Create the perlin noise
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <param name="z">Z location</param>
        /// <returns>Height</returns>
        public float perlin(float x, float y, float z)
        {
            // If the perlin noise is done again
            // Modular it so it is in the correct square
            if(m_repeatAmount > 0)
            {
                x = x % m_repeatAmount;
                y = y % m_repeatAmount;
                z = z % m_repeatAmount;
            }

            // The position in the cube
            float posX = x - (int)x;
            float posY = y - (int)y;
            float posZ = z - (int)z;

            // Calculate the fade amount for the position
            // This is used in the lerp
            float fadeX = fade(posX);
            float fadeY = fade(posY);
            float fadeZ = fade(posZ);

            // Find what cube the location is in
            int cubeX = (int)x & permutationLengthBase0;
            int cubeY = (int)y & permutationLengthBase0;
            int cubeZ = (int)z & permutationLengthBase0;

            //int aaa = m_overPerm[m_overPerm[m_overPerm[cubeX        ] + cubeY       ]   + cubeZ];       // Bot left front
            //int aba = m_overPerm[m_overPerm[m_overPerm[cubeX        ] + inc(cubeY)  ]   + cubeZ];       // Top left front
            //int aab = m_overPerm[m_overPerm[m_overPerm[cubeX        ] + cubeY       ]   + inc(cubeZ)];  // Bot left back
            //int abb = m_overPerm[m_overPerm[m_overPerm[cubeX        ] + inc(cubeY)  ]   + inc(cubeZ)];  // Top left back
            //int baa = m_overPerm[m_overPerm[m_overPerm[inc(cubeX)   ] + cubeY       ]   + cubeZ];       // Bot right front
            //int bba = m_overPerm[m_overPerm[m_overPerm[inc(cubeX)   ] + inc(cubeY)  ]   + cubeZ];       // Top right front
            //int bab = m_overPerm[m_overPerm[m_overPerm[inc(cubeX)   ] + cubeY       ]   + inc(cubeZ)];  // Bot right back
            //int bbb = m_overPerm[m_overPerm[m_overPerm[inc(cubeX)   ] + inc(cubeY)  ]   + inc(cubeZ)];  // Top right back

            // Get the hash values for each of the cube corners
            int aaa = GetHashValue(cubeX,       cubeY,      cubeZ);       // Bot left front
            int aba = GetHashValue(cubeX,       inc(cubeY), cubeZ);       // Top left front
            int aab = GetHashValue(cubeX,       cubeY,      inc(cubeZ));  // Bot left back
            int abb = GetHashValue(cubeX,       inc(cubeY), inc(cubeZ));  // Top left back
            int baa = GetHashValue(inc(cubeX),  cubeY,      cubeZ);       // Bot right front
            int bba = GetHashValue(inc(cubeX),  inc(cubeY), cubeZ);       // Top right front
            int bab = GetHashValue(inc(cubeX),  cubeY,      inc(cubeZ));  // Bot right back
            int bbb = GetHashValue(inc(cubeX),  inc(cubeY), inc(cubeZ));  // Top right back

            // Get the dot product of the gradient vector and the distance to the corner from the position
            float gradientAAA = grad(aaa, posX,     posY,       posZ);
            float gradientABA = grad(aba, posX,     posY - 1,   posZ);
            float gradientAAB = grad(aab, posX,     posY,       posZ - 1);
            float gradientABB = grad(abb, posX,     posY - 1,   posZ - 1);
            float gradientBAA = grad(baa, posX - 1, posY,       posZ);
            float gradientBBA = grad(bba, posX - 1, posY - 1,   posZ);
            float gradientBAB = grad(bab, posX - 1, posY,       posZ - 1);
            float gradientBBB = grad(bbb, posX - 1, posY - 1,   posZ - 1);

            // Linear interpolation of the points
            float x1, x2, y1, y2;

            // Front square
            x1 = Mathf.Lerp(gradientAAA, gradientBAA, fadeX);
            x2 = Mathf.Lerp(gradientABA, gradientBBA, fadeX);
            y1 = Mathf.Lerp(x1, x2, fadeY);

            // Back square
            x1 = Mathf.Lerp(gradientAAB, gradientBAB, fadeX);
            x2 = Mathf.Lerp(gradientABB, gradientBBB, fadeX);
            y2 = Mathf.Lerp(x1, x2, fadeY);

            // All
            return NormalisePerlinFloat(Mathf.Lerp(y1, y2, fadeZ));
        }

        /// <summary>
        /// Make the value between 0 and 1
        /// </summary>
        /// <param name="value">The value to normalise</param>
        /// <returns>The value between 0 and 1</returns>
        float NormalisePerlinFloat(float value)
        {
            // The value should be between -1 and 1
            // This should change it to between 0 and 1
            return (value + 1) / 2;
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
            return m_overPerm[m_overPerm[m_overPerm[x] + y] + z];
        }

        /// <summary>
        /// Fade the inputed value
        /// </summary>
        /// <param name="value">The value to fade</param>
        /// <returns>The value faded</returns>
        public float fade(float value)
        {
            // This formula was from Ken Perlin's implementation
            return value * value * value * (value * (value * 6 - 15) + 10);
        }

        /// <summary>
        /// Increment hash function
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int inc(int num)
        {
            num++;

            // Makes sure the number still repeats in the permutation
            if (m_repeatAmount > 0) num %= m_repeatAmount;

            return num;
        }

        /// <summary>
        /// Get the gradient of the position
        /// </summary>
        /// <param name="hash">The hash of the corner</param>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="z">Z Position</param>
        /// <returns>Gradient</returns>
        public float grad(int hash, float x, float y, float z)
        {
            switch (hash & 0xF)
            {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                case 0x4: return x + z;
                case 0x5: return -x + z;
                case 0x6: return x - z;
                case 0x7: return -x - z;
                case 0x8: return y + z;
                case 0x9: return -y + z;
                case 0xA: return y - z;
                case 0xB: return -y - z;
                case 0xC: return y + x;
                case 0xD: return -y + z;
                case 0xE: return y - x;
                case 0xF: return -y - z;
                default: return 0;
            }
        }

        /// <summary>
        /// Get the terrain in the scene, if there isn't spawn one
        /// </summary>
        public void SetTerrain()
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
        public void CreateTerrain()
        {
            // Get the width and height of the terrain's height map
            int maxX = m_terrainData.heightmapWidth;
            int maxY = m_terrainData.heightmapHeight;

            // Array to store the heights
            float[,] heights = new float[maxX, maxY];

            // Go through the height map
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    // Z had to be a random amount or else it does not work
                    heights[x, y] = perlin((float)x, (float)y, Random.Range(0f, 1000f));
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(0, 0, heights);

            // Create the terrain with the new height map
            SetTerrain();
        }

        public void SetHeightMapRes(int res)
        {
            m_terrainData.heightmapResolution = res;
        }
    }
}
