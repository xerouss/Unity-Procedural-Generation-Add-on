// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   14/03/2018
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
        #region old stuff
        #region Constants
        const float startY = 1;

        #endregion

        /*
        // TODO: Add more customisation for the user
        PNGridNode[,] m_grid;
        TerrainData m_terrainData;
        float m_maxHeight = 1;
        Vector2 m_size;

        public float MaxHeight
        {
            get
            {
                return m_maxHeight;
            }

            set
            {
                m_maxHeight = value;
            }
        }

        // TODO: Add extra parameters for the user to change
        /// <summary>
        /// Creates the grid for the perlin noise
        /// </summary>
        /// <param name="size">The size of the grid</param>
        public void CreateGrid(Vector2 size)
        {
            // Create the grid with the passed size
            m_grid = new PNGridNode[(int)size.x, (int)size.y];

            Vector3 gradient;

            // Fill the grid with random gradients
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    // Create the nodes for the grid position so the gradient can be set
                    m_grid[x, y] = new PNGridNode();

                    // TODO: Change the range of these to the user's inputted ones
                    gradient = new Vector3(Random.Range(0f, 1f),
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f));

                    m_grid[x, y].Gradient = gradient;
                }
            }

            m_size = size;

            m_terrainData = new TerrainData();

            // The size y value is used in the z since it is spawned in 3D space but the grid is 2D
            m_terrainData.size = new Vector3(size.x, startY, size.y);
            m_terrainData.heightmapResolution = 32;
        }

        /// <summary>
        /// Calculate the height of the position
        /// </summary>
        /// <param name="position">The position to calculate the height at</param>
        /// <param name="distanceBetweenCells">Distance between each grid position</param>
        /// <returns>Height</returns>
        public float CalculateHeight(Vector3 position, float distanceBetweenCells, float heightMapX, float heightMapY)
        {
            float gridConversionX = heightMapX / m_size.x;
            float gridConversionY = heightMapY / m_size.y;

            // Get the dot product of each grid location corners by using their direction from the position and gradient
            float topLeftDot = GetLerpValue(Mathf.Clamp(Mathf.FloorToInt(position.x / gridConversionX), 0, (int)m_size.x -1), Mathf.Clamp(Mathf.CeilToInt(position.y / gridConversionY), 0, (int)m_size.y - 1), gridConversionX, gridConversionY, position);
            float botLeftDot = GetLerpValue(Mathf.Clamp(Mathf.FloorToInt(position.x / gridConversionX), 0, (int)m_size.x - 1), Mathf.Clamp(Mathf.FloorToInt(position.y / gridConversionY), 0, (int)m_size.y - 1), gridConversionX, gridConversionY, position);
            float topRightDot = GetLerpValue(Mathf.Clamp(Mathf.CeilToInt(position.x / gridConversionX), 0, (int)m_size.x - 1), Mathf.Clamp(Mathf.CeilToInt(position.y / gridConversionY), 0, (int)m_size.y - 1), gridConversionX, gridConversionY, position);
            float botRightDot = GetLerpValue(Mathf.Clamp(Mathf.CeilToInt(position.x / gridConversionX), 0, (int)m_size.x - 1), Mathf.Clamp(Mathf.FloorToInt(position.y / gridConversionY), 0, (int)m_size.y - 1), gridConversionX, gridConversionY, position);

            // Lerp to get the final outcome of the height
            float lerpTopAndBotLeft = Mathf.Lerp(topLeftDot, botLeftDot, position.x);
            float lerpTopAndBotRight = Mathf.Lerp(topRightDot, botRightDot, position.x);

            float returnValue = Mathf.Lerp(lerpTopAndBotLeft, lerpTopAndBotRight, position.y);

            return -returnValue;
        }

        /// <summary>
        /// Get the value to use for the height lerp
        /// </summary>
        /// <param name="gridPointX">X position of the grid</param>
        /// <param name="gridPointY">Y position of the grid</param>
        /// <param name="position">The position to change</param>
        /// <returns>Lerp value</returns>
        public float GetLerpValue(int gridPointX, int gridPointY, float gridConversionX, float gridConversionY, Vector3 position)
        {

            Vector3 girdPosInHeightMap = new Vector3(gridPointX * gridConversionX, gridPointY * gridConversionY);

            // Get the gradient of the grid point
            Vector3 gradient = m_grid[gridPointX, gridPointY].Gradient;

            // Calculate the direction from the position to the grid position
            Vector3 direction = position - girdPosInHeightMap;

            // Return the scalar value from the dot product
            return Vector3.Dot(gradient, direction);
        }        
        
        /// <summary>
        /// Normalise the inputted values
        /// </summary>
        /// <param name="value">Value to normalise</param>
        /// <param name="min">Min value is can be</param>
        /// <param name="max">Max value it can be</param>
        /// <returns></returns>
        float NormaliseFloat(float value, float min, float max)
        {
            if (value == 0) return value;

            return Mathf.InverseLerp(min, max, value);
           // return (value - (min * value)) / ((max * value) - (min * value));
        }
        */
        #endregion

        #region Constants
        const float defaultTerrainXSize = 50;
        const float defaultTerrainYSize = 50;
        const float defaultTerrainZSize = 50;
        const int defaultHeightmapRes = 32;
        const int defaultMultiplyFade = 6;
        const int defaultMinusFade = 15;
        const int defaultAdditionFade = 10;
        const float defaultDistanceModifier = 40;
        const int arrayX = 0;
        const int arrayY = 1;

        const float normaliseMin = 0;
        const float normaliseMax = 1;
        const int fadeLeftPow = 5;
        const int fadeMidPow = 4;
        const int fadeRightPow = 3;

        const int terrainRes = 1024;
        const int terrainResPerBatch = 8;
        #endregion

        #region Variables

        PNGridNode[,] m_permutation;
        TerrainData m_terrainData;

        // Variables the user can change
        Vector3 m_terrainSize = new Vector3(defaultTerrainXSize, defaultTerrainYSize, defaultTerrainZSize);
        int m_heightmapResolution = defaultHeightmapRes;
        int m_multiplyFade = defaultMultiplyFade;
        int m_minusFade = defaultMinusFade;
        int m_additionFade = defaultAdditionFade;
        float m_distanceModifier = defaultDistanceModifier;

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

        public float DistanceModifier
        {
            get
            {
                return m_distanceModifier;
            }

            set
            {
                m_distanceModifier = value;
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

            m_permutation = new PNGridNode[permutationSize, permutationSize];

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
            int ceilX = Mathf.FloorToInt(x + 1);
            int ceilY = Mathf.FloorToInt(y + 1);
            int floorX = Mathf.FloorToInt(x);
            int floorY = Mathf.FloorToInt(y);

            // Each corner of the cube that the position is int
            Vector2 topLeft = new Vector2(floorX, ceilY);
            Vector2 topRight = new Vector2(ceilX, ceilY);
            Vector2 botLeft = new Vector2(floorX, floorY);
            Vector2 botRight = new Vector2(ceilX, floorY);

            // Get the distance from the position to each corner
            Vector2 pos = new Vector2(x, y);
            Vector2 distanceTopLeft = pos - topLeft;
            Vector2 distanceTopRight = pos - topRight;
            Vector2 distanceBotLeft = pos - botLeft;
            Vector2 distanceBotRight = pos - botRight;

            // The dot product of each distance and gradient
            float dotTopLeft = Vector2.Dot(m_permutation[floorX, ceilY].Gradient, distanceTopLeft);
            float dotTopRight = Vector2.Dot(m_permutation[ceilX, ceilY].Gradient, distanceTopRight);
            float dotBotLeft = Vector2.Dot(m_permutation[floorX, floorY].Gradient, distanceBotLeft);
            float dotBotRight = Vector2.Dot(m_permutation[ceilX, floorY].Gradient, distanceBotRight);

            // Lerp between the points to get a gradual gradient
            float leftSideLerp = Mathf.Lerp(dotTopLeft, dotBotLeft, y);
            float rightSideLerp = Mathf.Lerp(dotTopRight, dotBotRight, y);
            float lerpTotal = Mathf.Lerp(leftSideLerp, rightSideLerp, x);

            // Return the normalised value of the lerps
            return Mathf.InverseLerp(normaliseMin, normaliseMax, lerpTotal);
        }


        public float Fade(float value)
        {
            return (Mathf.Pow(m_multiplyFade * value, fadeLeftPow) - (Mathf.Pow(m_minusFade * value, fadeMidPow)) + (Mathf.Pow(m_additionFade * value, fadeRightPow)));
        }

        /// <summary>
        /// Set the gradients for the permutation
        /// </summary>
        public void SetGridGradients()
        {
            Vector3 gradient;

            for (int x = 0; x < m_permutation.GetLength(arrayX); x++)
            {
                for (int y = 0; y < m_permutation.GetLength(arrayY); y++)
                {
                    // Create the nodes for the grid position so the gradient can be set
                    m_permutation[x, y] = new PNGridNode();

                    // Set the gradient on every axis to either 0 or 1
                    // The random ranges has 2 as the max since it does not include that value
                    gradient = new Vector3(Random.Range(0, 2),
                        Random.Range(0, 2), 0);

                    m_permutation[x, y].Gradient = gradient;
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

            SetGridGradients();

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
                    xLocation = ((float)x / maxX) * m_distanceModifier;
                    yLocation = ((float)y / maxY) * m_distanceModifier;

                    // Get the height values using the perlin noise
                    heights[x, y] = perlin(xLocation, yLocation) / 10;
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(0, 0, heights);

            // Create the terrain with the new height map
            CreateTerrain();
        }
    }
}
