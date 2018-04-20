// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   20/04/2018
//	File:			   BinarySpacePartition.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using System.Collections.Generic;

////////////////////////////////////////////

/// <summary>
/// Class for binary space partition (BSP)
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class BinarySpacePartition
    {
        public const int roomGridNum = 1;
        public const int corridorGridNum = 2;

        BSPTreeNode m_treeRootNode;

        // User variables
        Vector2 m_levelSize = new Vector2(10, 10); // Temp size, TODO: REMOVE
        // Change to desired rooms sizes?
        int m_splitAmount = 7; // Amount of times the grid is divided before the rooms are set
        GameObject m_floorTile;
        int[,] m_spawnGrid;

        // TODO REMOVE THIS AFTER DEBUGGING
        GameObject m_floorTile2;
        GameObject m_floorTile3;

        public GameObject FloorTile
        {
            get
            {
                return m_floorTile;
            }

            set
            {
                m_floorTile = value;
            }
        }

        public GameObject FloorTile2
        {
            get
            {
                return m_floorTile2;
            }

            set
            {
                m_floorTile2 = value;
            }
        }

        public GameObject FloorTile3
        {
            get
            {
                return m_floorTile3;
            }

            set
            {
                m_floorTile3 = value;
            }
        }

        /// <summary>
        /// Create a new dungeon
        /// </summary>
        public void CreateDungeon()
        {
            InitialiseDungeon();
            CreateCells();
            CreateRooms();
            CreateCorridors();
            OutputDungeon();
        }
        
        /// <summary>
        /// Set up the settings for the dungeon
        /// </summary>
        public void InitialiseDungeon()
        {
            m_spawnGrid = new int[(int)m_levelSize.x, (int)m_levelSize.y];
        }

        /// <summary>
        /// Split up the grid to create cells for the rooms
        /// </summary>
        public void CreateCells()
        {
            // Create a queue of which nodes to split
            // Used a queue so it does all children in a row before starting a new row in the tree
            Queue<BSPTreeNode> splitQueue = new Queue<BSPTreeNode>();
            BSPTreeNode currentNode;
            // Create the root and add it to the tree
            // TODO: let the user change these values
            m_treeRootNode = new BSPTreeNode(new Vector2(10, 10), 20, 20, Vector2.zero, m_floorTile, m_floorTile2, m_floorTile3, ref m_spawnGrid);
            splitQueue.Enqueue(m_treeRootNode);

            // For the amount of time the user wants to split the cells
            for (int i = 0; i < m_splitAmount; i++)
            {
                // Remove the node at the start of the queue but save it tp split it
                currentNode = splitQueue.Dequeue();
                currentNode.SplitCell();

                // For each child made from the split, add it to the queue
                for (int j = 0; j < currentNode.Children.Count ; j++)
                {
                    splitQueue.Enqueue(currentNode.Children[j]);
                }
            }
        }

        /// <summary>
        /// Create the rooms instead the cells for the BSP
        /// </summary>
        public void CreateRooms()
        {
            m_treeRootNode.CreateRoom(1);
        }

        /// <summary>
        /// Create the corridors to connect the rooms together
        /// </summary>
        public void CreateCorridors()
        {
            m_treeRootNode.CreateCorridorToParent(new BSPTreeNode(Vector2.zero));
        }

        /// <summary>
        /// Spawn the dungeon onto the scene
        /// </summary>
        public void OutputDungeon()
        {
            GameObject parentCorridor = new GameObject();
            parentCorridor.name = "Corridor";

            GameObject parentRoom = new GameObject();
            parentRoom.name = "Rooms";

            // Go through the gird and based on the number spawn the correct tile
            for (int x = 0; x < m_treeRootNode.Width; x++)
            {
                for (int y = 0; y < m_treeRootNode.Height; y++)
                {
                    if (m_spawnGrid[x, y] == roomGridNum)
                    {
                        GameObject.Instantiate(m_floorTile, new Vector3(x, 1, y), m_floorTile.transform.rotation, parentRoom.transform);
                    }
                    else if (m_spawnGrid[x, y] == corridorGridNum)
                    {
                        GameObject.Instantiate(m_floorTile2, new Vector3(x, 1, y), m_floorTile.transform.rotation, parentCorridor.transform);
                    }
                }
            }
        }
    }
    }
}
