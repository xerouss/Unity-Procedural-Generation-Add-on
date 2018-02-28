// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   27/02/2018
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
        #region Constants
        const float startY = 1;

        #endregion


        // TODO: Add more customisation for the user
        PNGridNode[,] m_grid;
        TerrainData m_terrainData;
        float m_maxHeight;
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
                    gradient = new Vector3(Random.Range(0, 10), 
                        Random.Range(0, 10), 
                        Random.Range(0, 10));

                    m_grid[x, y].Gradient = gradient;
                }
            }

            m_terrainData = new TerrainData();

            // The size y value is used in the z since it is spawned in 3D space but the grid is 2D
            m_terrainData.size = new Vector3(size.x, startY, size.y);
        }

        /// <summary>
        /// Calculate the height of the position
        /// </summary>
        /// <param name="position">The position to calculate the height at</param>
        /// <param name="distanceBetweenCells">Distance between each grid position</param>
        /// <returns>Height</returns>
        public float CalculateHeight(Vector3 position, float distanceBetweenCells)
        {
            // Get the dot product of each grid location corners by using their direction from the position and gradient
            float topLeftDot = GetLerpValue(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y), position);
            float botLeftDot = GetLerpValue(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), position);
            float topRightDot = GetLerpValue(Mathf.CeilToInt(position.x), Mathf.CeilToInt(position.y), position);
            float botRightDot = GetLerpValue(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y), position);

            // Lerp to get the final outcome of the height
            float lerpTopAndBotLeft = Mathf.Lerp(topLeftDot, botLeftDot, position.x);
            float lerpTopAndBotRight = Mathf.Lerp(topRightDot, botRightDot, position.x);
            return Mathf.Lerp(lerpTopAndBotLeft, lerpTopAndBotRight, position.y);
        }

        /// <summary>
        /// Get the value to use for the height lerp
        /// </summary>
        /// <param name="gridPointX">X position of the grid</param>
        /// <param name="gridPointY">Y position of the grid</param>
        /// <param name="position">The position to change</param>
        /// <returns>Lerp value</returns>
        public float GetLerpValue(int gridPointX, int gridPointY, Vector3 position)
        {
            // Save the grid position
            Vector3 gridPos = new Vector3(gridPointX, gridPointY);

            // Get the gradient of the grid point
            Vector3 gradient = m_grid[gridPointX, gridPointY].Gradient;

            // Calculate the direction from the position to the grid position
            Vector3 direction = position - gridPos;

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

        float NormaliseFloat(float value, float min, float max)
        {
            return (value - (min * value)) / ((max * value) - (min * value));
        }

        public void CreateTerrain()
        {

            // Max y uses x bound because 
            int maxX = m_terrainData.heightmapWidth;
            int maxY = m_terrainData.heightmapHeight;

            float[,] heights = new float[maxX, maxY];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                   // float height = NormaliseFloat(CalculateHeight(new Vector3(x, y), 0), 0, m_maxHeight);
                    heights[x, y] = Random.Range(0.0f, 1.0f);
                }
            }

            m_terrainData.SetHeights(0, 0, heights);
            SetTerrain();
        }
    }
}
