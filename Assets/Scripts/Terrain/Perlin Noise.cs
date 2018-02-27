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
                    // TODO: Change the range of these to the user's inputted ones
                    gradient = new Vector3(Random.Range(0, 10), 
                        Random.Range(0, 10), 
                        Random.Range(0, 10));

                    m_grid[x, y].Gradient = gradient;
                }
            }
        }

        public bool Interpolate()
        {


            return true;
        }
    }
}
