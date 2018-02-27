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
        // TODO: Add more customisation for the user
        PNGridNode[,] m_grid;
        TerrainData m_terrainData;

        // TODO: Add extra parameters for the user to change
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
        }

        public bool CalculateHeight(Vector3 position, float distanceBetweenCells)
        {
            // The positions of the closest grid positions
            Vector3 topLeftPos = new Vector2(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y));
            Vector3 topRightPos = new Vector2(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
            Vector3 botLeftPos = new Vector2(Mathf.CeilToInt(position.x), Mathf.CeilToInt(position.y));
            Vector3 botRightPos = new Vector2(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y));

            // Get the gradients of the nearest grid positions
            Vector3 topLeftGradient = m_grid[(int)topLeftPos.x, (int)topLeftPos.y].Gradient;
            Vector3 botLeftGradient = m_grid[(int)botLeftPos.x, (int)botLeftPos.y].Gradient;
            Vector3 topRightGradient = m_grid[(int)topRightPos.x, (int)topRightPos.y].Gradient;
            Vector3 botRightGradient = m_grid[(int)botRightPos.x, (int)botRightPos.y].Gradient;

            // Get the directions
            Vector3 topLeftDirection = position - topLeftPos;
            Vector3 botLeftDirection = position - botLeftPos;
            Vector3 topRightDirection = position - topRightPos;
            Vector3 botRightDirection = position - botRightPos;

            // Get the dot products of each one
            float topLeftDot = Vector3.Dot(topLeftGradient, topLeftDirection);
            float botLeftDot = Vector3.Dot(botLeftGradient, botLeftDirection);
            float topRightDot = Vector3.Dot(topRightGradient, topRightDirection);
            float botRightDot = Vector3.Dot(botRightGradient, botRightDirection);

            // Lerp to get the final outcome of the height
            float lerpTopAndBotLeft = Mathf.Lerp(topLeftDot, botLeftDot, position.x);
            float lerpTopAndBotRight = Mathf.Lerp(topLeftDot, botLeftDot, position.x);
            float allPointsLerp = Mathf.Lerp(lerpTopAndBotLeft, lerpTopAndBotRight, position.y);

            return true;
        }

        public void GetTerrain()
        {
            // If there is no current terrain
            if(Terrain.activeTerrain == null)
            {
                // Create the terrain data with the default data
                m_terrainData = new TerrainData();
                Terrain.CreateTerrainGameObject(m_terrainData);
            }
        }
    }
}
