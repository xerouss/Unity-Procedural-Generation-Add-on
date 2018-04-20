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
        public const int emptyGridNum = 0;
        public const int roomGridNum = 1;
        public const int corridorGridNum = 2;
        public const int wallGridNum = 3;
        const int xAxis = 0;
        const int yAxis = 1;

        BSPTreeNode m_treeRootNode;

        // User variables
        Vector2 m_levelSize = new Vector2(20, 20); // Temp size, TODO: REMOVE
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
            CreateWalls();
            OutputDungeon();
        }

        /// <summary>
        /// Set up the settings for the dungeon
        /// </summary>
        public void InitialiseDungeon()
        {
            // +2 on both axis to get free space for the walls without going out of bounds
            // Its +2 instead of +1 since the bot left corner is set to 1,1 so the base is 1
            m_spawnGrid = new int[(int)m_levelSize.x + 2, (int)m_levelSize.y + 2];
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
            // Bot left corner is set to 1,1 to keep the bot and left parts of the grid free for walls
            m_treeRootNode = new BSPTreeNode(new Vector2(m_levelSize.x / 2, m_levelSize.y / 2), (int)m_levelSize.x, (int)m_levelSize.y, new Vector2(1, 1), m_floorTile, m_floorTile2, m_floorTile3, ref m_spawnGrid);
            splitQueue.Enqueue(m_treeRootNode);

            // For the amount of time the user wants to split the cells
            for (int i = 0; i < m_splitAmount; i++)
            {
                // Remove the node at the start of the queue but save it tp split it
                currentNode = splitQueue.Dequeue();
                currentNode.SplitCell();

                // For each child made from the split, add it to the queue
                for (int j = 0; j < currentNode.Children.Count; j++)
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
        /// Create walls around the rooms and corridors
        /// </summary>
        public void CreateWalls()
        {
            // Go through the gird if there is a room or corridor
            // Check the surroundings for empty spaces, if there is one create a wall there
            for (int x = 0; x < m_spawnGrid.GetLength(xAxis); x++)
            {
                for (int y = 0; y < m_spawnGrid.GetLength(yAxis); y++)
                {
                    if (m_spawnGrid[x, y] == roomGridNum || m_spawnGrid[x, y] == corridorGridNum)
                    {
                        WallTileCheck(x + 1, y);
                        WallTileCheck(x - 1, y);
                        WallTileCheck(x, y + 1);
                        WallTileCheck(x, y - 1);
                        WallTileCheck(x + 1, y + 1);
                        WallTileCheck(x - 1, y - 1);
                        WallTileCheck(x - 1, y + 1);
                        WallTileCheck(x + 1, y - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Check if there is a tile at the location, if not create a wall
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y Location</param>
        public void WallTileCheck(int x, int y)
        {
            if(m_spawnGrid[x, y] == emptyGridNum)
            {
                m_spawnGrid[x, y] = wallGridNum;
            }
        }

        /// <summary>
        /// Spawn the dungeon onto the scene
        /// </summary>
        public void OutputDungeon()
        {
            // Create parents to organise the hierarchy
            GameObject parentDungeon = new GameObject();
            parentDungeon.name = "Dungeon";

            GameObject parentRoom = new GameObject();
            parentRoom.name = "Rooms";
            parentRoom.transform.SetParent(parentDungeon.transform);

            GameObject parentCorridor = new GameObject();
            parentCorridor.name = "Corridors";
            parentCorridor.transform.SetParent(parentDungeon.transform);

            GameObject parentWall = new GameObject();
            parentWall.name = "Walls";
            parentWall.transform.SetParent(parentDungeon.transform);

            // Go through the gird and based on the number spawn the correct tile
            for (int x = 0; x < m_spawnGrid.GetLength(xAxis); x++)
            {
                for (int y = 0; y < m_spawnGrid.GetLength(yAxis); y++)
                {
                    switch (m_spawnGrid[x, y])
                    {
                        case roomGridNum:
                            GameObject.Instantiate(m_floorTile, new Vector3(x, 1, y), m_floorTile.transform.rotation, parentRoom.transform);
                            break;
                        case corridorGridNum:
                            GameObject.Instantiate(m_floorTile2, new Vector3(x, 1, y), m_floorTile2.transform.rotation, parentCorridor.transform);
                            break;
                        case wallGridNum:
                            GameObject.Instantiate(m_floorTile3, new Vector3(x, 1, y), m_floorTile3.transform.rotation, parentWall.transform);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

