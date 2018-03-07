// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   27/02/2018
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
        const float startY = 1;

        #endregion


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
        /// Get the terrain in the scene, if there isn't spawn one
        /// </summary>
        public void SetTerrain()
        {
            // If there is no current terrain
            if (Terrain.activeTerrain == null)
            {
                // Create the terrain data with the default data
                //m_terrainData = new TerrainData();
                Terrain.CreateTerrainGameObject(m_terrainData);
            }
            else Terrain.activeTerrain.terrainData = m_terrainData;
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
                    // Get the height values using the perlin noise
                    float heightTest = CalculateHeight(new Vector3(x, y), 0, maxX, maxY);
                    float height = NormaliseFloat(heightTest, 0.0f, m_maxHeight);
                    heights[x, y] = height;
                }
            }

            // Set the height map to the terrain data
            m_terrainData.SetHeights(0, 0, heights);

            // Create the terrain with the new height map
            SetTerrain();
        }
    }
}
