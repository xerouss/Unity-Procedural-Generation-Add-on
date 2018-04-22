// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   22/04/2018
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
        #region constants

        public const int emptyGridNum = 0;
        public const int roomGridNum = 1;
        public const int corridorGridNum = 2;
        public const int wallGridNum = 3;
        const int xAxis = 0;
        const int yAxis = 1;
        const int getCentre = 2;
        const int botConerOffsetForWall = 1;
        const int offsetToPreventOutOfBounds = 2;

        // Defaults
        const int defaultDungeonSize = 20;
        const int defaultSplitAmount = 3;
        const int defaultMinCellSize = 3;
        const int defaultMinRoomSize = 3;

        // Minimum Values
        const int minimumSplitAmount = 0;
        const int minimumCellSize = 2;
        #endregion

        #region Variables
        BSPTreeNode m_treeRootNode;
        int[,] m_spawnGrid;

        // User Variables
        Vector3 m_dungeonSize = new Vector3(defaultDungeonSize, defaultDungeonSize, defaultDungeonSize);
        int m_splitAmount = defaultSplitAmount; // Amount of times the grid is divided before the rooms are set
        int m_minimumCellSize = defaultMinCellSize;
        int m_minimumRoomSize = defaultMinRoomSize;
        GameObject m_floorTile;
        GameObject m_corridorTile;
        GameObject m_wallTile;
        GameObject m_roofTile;

        #endregion

        #region Properties

        public Vector3 DungeonSize
        {
            get
            {
                return m_dungeonSize;
            }

            set
            {
                m_dungeonSize = value;
            }
        }

        public int SplitAmount
        {
            get
            {
                return m_splitAmount;
            }

            set
            {
                // Can't have the split amount be below 0
                // Or nothing will happen and will error
                if (value < minimumSplitAmount) value = minimumSplitAmount;
                m_splitAmount = value;
            }
        }

        public int MinimumCellSize
        {
            get
            {
                return m_minimumCellSize;
            }

            set
            {
                // Can't have the size be below 2 because it produce out of bound errors
                if (value < minimumCellSize) value = minimumCellSize;

                // Set the max value because if it goes above it, it can cause errors
                float lowestDungeonSize = m_dungeonSize.x;
                if (m_dungeonSize.z < lowestDungeonSize) lowestDungeonSize = m_dungeonSize.z;

                // The formula was found via trial and error and seems to work correctly
                lowestDungeonSize = (lowestDungeonSize / 2) - 1;
                if (value > lowestDungeonSize) value = (int)lowestDungeonSize;

                m_minimumCellSize = value;
            }
        }

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

        public GameObject CorridorTile
        {
            get
            {
                return m_corridorTile;
            }

            set
            {
                m_corridorTile = value;
            }
        }

        public GameObject WallTile
        {
            get
            {
                return m_wallTile;
            }

            set
            {
                m_wallTile = value;
            }
        }

        public GameObject RoofTile
        {
            get
            {
                return m_roofTile;
            }

            set
            {
                m_roofTile = value;
            }
        }

        public int MinimumRoomSize
        {
            get
            {
                return m_minimumRoomSize;
            }

            set
            {
                m_minimumRoomSize = value;
            }
        }

        #endregion

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
            m_spawnGrid = new int[(int)m_dungeonSize.x + offsetToPreventOutOfBounds, (int)m_dungeonSize.z + offsetToPreventOutOfBounds];
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
            // Need to +1 the centre or the child nodes won't line up correctly and won't connect to the root with corridors
            m_treeRootNode = new BSPTreeNode(new Vector2((m_dungeonSize.x / getCentre) + botConerOffsetForWall, (m_dungeonSize.z / getCentre) + botConerOffsetForWall), 
                (int)m_dungeonSize.x, (int)m_dungeonSize.z, new Vector2(botConerOffsetForWall, botConerOffsetForWall), ref m_spawnGrid, this);
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
            m_treeRootNode.CreateRoom();
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
            if (m_spawnGrid[x, y] == emptyGridNum)
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
            // TODO Need to add width/height to the loop increments
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
                            GameObject.Instantiate(m_corridorTile, new Vector3(x, 1, y), m_corridorTile.transform.rotation, parentCorridor.transform);
                            break;
                        case wallGridNum:
                            GameObject.Instantiate(m_wallTile, new Vector3(x, 1, y), m_wallTile.transform.rotation, parentWall.transform);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

