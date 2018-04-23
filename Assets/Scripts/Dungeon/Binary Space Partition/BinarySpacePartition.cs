// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
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
        const int botCornerOffsetForWall = 1;
        const int offsetToPreventOutOfBounds = 2;
        const int ySpawningPos = 1;
        const int empty = 0;

        // Defaults
        const int defaultDungeonSizeXZ = 20;
        const int defaultDungeonSizeY = 5;
        const int defaultSplitAmount = 3;
        const int defaultMinCellSize = 3;
        const int defaultMinRoomSize = 3;

        // Minimum Values
        const int minimumSplitAmount = 0;
        const int minimumCellSize = 2;
        #endregion

        #region Variables
        BinarySpacePartitionTreeNode m_treeRootNode;
        int[,] m_spawnGrid;
        BinarySpacePartitionSeed m_seed;
        GameObject m_dungeonParent;
        GameObject m_dungeonCorridorParent;
        GameObject m_dungeonWallParent;
        GameObject m_dungeonRoomParent;
        GameObject m_dungeonRoofParent;

        // User Variables
        Vector3 m_dungeonSize = new Vector3(defaultDungeonSizeXZ, defaultDungeonSizeY, defaultDungeonSizeXZ);
        int m_splitAmount = defaultSplitAmount; // Amount of times the grid is divided before the rooms are set
        int m_minimumCellSize = defaultMinCellSize;
        int m_minimumRoomSize = defaultMinRoomSize;
        GameObject m_floorTile;
        GameObject m_corridorTile;
        GameObject m_wallTile;
        GameObject m_roofTile;
        bool m_spawnRoof = false;
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

        // Need individual property for the x and y since they can't be set individually from the vector 3 one
        public float DungeonSizeX
        {
            set
            {
                m_dungeonSize.x = value;
            }
        }

        public float DungeonSizeY
        {
            set
            {
                m_dungeonSize.y = value;
            }
        }

        public float DungeonSizeZ
        {
            set
            {
                m_dungeonSize.z = value;
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

        public bool SpawnRoof
        {
            get
            {
                return m_spawnRoof;
            }

            set
            {
                m_spawnRoof = value;
            }
        }

        public BinarySpacePartitionSeed SeedClass
        {
            get
            {
                return m_seed;
            }
        }

        public string SeedValue
        {
            get
            {
                return m_seed.Seed;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BinarySpacePartition()
        {
            m_seed = new BinarySpacePartitionSeed(this);
            m_seed.UpdateSeed();
        }

        /// <summary>
        /// Reset all variables the user can change back to their defaults
        /// </summary>
        public void ResetVariableValues()
        {
            m_dungeonSize = new Vector3(defaultDungeonSizeXZ, defaultDungeonSizeY, defaultDungeonSizeXZ);
            m_splitAmount = defaultSplitAmount;
            m_minimumCellSize = defaultMinCellSize;
            m_minimumRoomSize = defaultMinRoomSize;
        }

        /// <summary>
        /// Create a new dungeon
        /// </summary>
        /// <param name="reUseGameObject">If the user wants to create a new dungeon using an exciting game object</param>
        public void CreateDungeon(bool reUseGameObject)
        {
            InitialiseDungeon(); // Clear the current dungeon
            CreateCells();
            CreateRooms();
            CreateCorridors();
            CreateWalls();
            OutputDungeon(reUseGameObject);
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
            Queue<BinarySpacePartitionTreeNode> splitQueue = new Queue<BinarySpacePartitionTreeNode>();
            BinarySpacePartitionTreeNode currentNode;

            // Bot left corner is set to (1,1) to keep the bot and left parts of the grid free for walls to prevent out of bounds errors
            Vector2 rootBotLeftCorner = new Vector2(botCornerOffsetForWall, botCornerOffsetForWall);

            // Need to +1 the centre or the child nodes won't line up correctly and won't connect to the root with corridors due to the (1, 1) bot corner
            Vector2 rootCentre = new Vector2((m_dungeonSize.x / getCentre) + botCornerOffsetForWall, (m_dungeonSize.z / getCentre) + botCornerOffsetForWall);

            // Create the root and add it to the tree
            m_treeRootNode = new BinarySpacePartitionTreeNode(rootCentre, (int)m_dungeonSize.x, (int)m_dungeonSize.z, rootBotLeftCorner, ref m_spawnGrid, this);
            splitQueue.Enqueue(m_treeRootNode);

            for (int i = 0; i < m_splitAmount; i++)
            {
                // Stop if queue is empty
                if (splitQueue.Count == empty) return;

                // Remove the node at the start of the queue but save it to split it
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
        /// Create the rooms inside the cells for the BSP
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
            // The passed node has a 0,0 center to exit the function before it tries to connect to nothing
            // This is due to the root node having no parents to connect to
            m_treeRootNode.CreateCorridorToParent(new BinarySpacePartitionTreeNode(Vector2.zero));
        }

        /// <summary>
        /// Create walls around the rooms and corridors
        /// </summary>
        public void CreateWalls()
        {
            // Go through the gird to check if there is a room or corridor
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

        #region Spawning functions

        /// <summary>
        /// Spawn the dungeon onto the scene
        /// </summary>
        /// <param name="reUseGameObject">If the user wants to create a new dungeon using an exciting game object</param>
        public void OutputDungeon(bool reUseGameObject)
        {
            // Check if the user has inputted the tiles or not
            if(m_floorTile == null || m_corridorTile == null || m_wallTile == null)
            {
                Debug.Log("** MISSING TILE **| Please input the desired tiles in the tile fields.");
                return;
            }

            // If the user deletes the previous dungeon so the  dungeonParent is now null
            // Check if there is any other dungeons to recreate on
            if(m_dungeonParent == null)
            {
                GeneratedDungeon otherDungeon = GameObject.FindObjectOfType<GeneratedDungeon>();

                if (otherDungeon != null)
                {
                    m_dungeonParent = otherDungeon.gameObject;
                }
            }

            // If the user wants to re-use the current dungeon game object
            // Go through the parent of the whole dungeon and remove the children
            // To get rid of the current dungeon tiles
            if(reUseGameObject && m_dungeonParent != null)
            {
                // Need to get the value before the loop since it changes each time a chikd is destroyed
                int numberOfChildren = m_dungeonParent.transform.childCount;

                for (int i = 0; i < numberOfChildren; i++)
                {
                    // Destroy at index 0 since every time a child is destroyed it moves the locations down
                    MonoBehaviour.DestroyImmediate(m_dungeonParent.transform.GetChild(0).gameObject);
                }
            }
            // If the user wants to create a new dungeon or if there is no current dungeon
            else if (reUseGameObject == false || m_dungeonParent == null)
            {
                m_dungeonParent = CreateParent("Dungeon", null);

                // Used to find the dungeon to delete when the delete button is pressed by the user
                m_dungeonParent.AddComponent<GeneratedDungeon>();
            }

            // Create the parent game objects to use in hierarchy to organise it
            m_dungeonRoomParent = CreateParent("Rooms", m_dungeonParent.transform);
            m_dungeonCorridorParent = CreateParent("Corridors", m_dungeonParent.transform);
            m_dungeonWallParent = CreateParent("Walls", m_dungeonParent.transform);   
            m_dungeonRoofParent = CreateParent("Roof", m_dungeonParent.transform);

            // Go through the gird and based on the number spawn the correct tile
            for (int x = 0; x < m_spawnGrid.GetLength(xAxis); x++)
            {
                for (int z = 0; z < m_spawnGrid.GetLength(yAxis); z++)
                {
                    switch (m_spawnGrid[x, z])
                    {
                        case roomGridNum:
                            SpawnFloorTile(m_floorTile, x, z, m_dungeonRoomParent.transform, m_dungeonRoofParent.transform);
                            break;
                        case corridorGridNum:
                            SpawnFloorTile(m_corridorTile, x, z, m_dungeonCorridorParent.transform, m_dungeonRoofParent.transform);
                            break;
                        case wallGridNum:
                            // Stack up the walls to make the rooms 3D
                            for (int y = 1; y < m_dungeonSize.y; y++)
                            {
                                GameObject.Instantiate(m_wallTile, new Vector3(x, y, z), m_wallTile.transform.rotation, m_dungeonWallParent.transform);
                            }
                            break;
                        default:
                            Debug.Log("ERROR: Incorrect spawn grid number!");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Create the floor tile in the scene
        /// </summary>
        /// <param name="tile">The tile to spawn</param>
        /// <param name="x">X Position</param>
        /// <param name="z">Z Position</param>
        /// <param name="parent">The parent to attach the tile to</param>
        /// <param name="roofParent">The parent to attach the roof to</param>
        void SpawnFloorTile(GameObject tile, int x, int z, Transform parent, Transform roofParent)
        {
            GameObject.Instantiate(tile, new Vector3(x, ySpawningPos, z), tile.transform.rotation, parent);
            SpawnRoofTile(x, z, roofParent);
        }

        /// <summary>
        /// Spawn the roof tile at the position
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="z">Z Position</param>
        /// <param name="parent">The parent to attach the spawned game obejct to</param>
        public void SpawnRoofTile(int x, int z, Transform parent)
        {
            if(m_spawnRoof)
            {
                GameObject.Instantiate(m_roofTile, new Vector3(x, m_dungeonSize.y, z), m_corridorTile.transform.rotation, parent);
            }
        }

        /// <summary>
        /// Create a game object to act as a parent for parts of the dungeon
        /// </summary>
        /// <param name="name">Name of the parent</param>
        /// <param name="dungeonParent">The parent to attach the made parent to</param>
        /// <returns>The created game object parent</returns>
        public GameObject CreateParent(string name, Transform dungeonParent)
        {
            GameObject parent = new GameObject();
            parent.name = name;

            // If check for the root parent since it has nothing to attach to
            if(dungeonParent != null) parent.transform.SetParent(dungeonParent);

            return parent;
        }

        #endregion
    }
}