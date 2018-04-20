// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   20/04/2018
//	File:			   BSPTreeNode.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using System.Collections.Generic;
using UnityEngine;
////////////////////////////////////////////

/// <summary>
/// Class for the binary space partition tree which will store the cells
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class BSPTreeNode
    {
        #region Constants

        const int trueValue = 0;
        const int falseValue = 2;
        const int splitBound = 3;
        const int getCentre = 2;
        const int checkIfEvenNumber = 2;
        const int oddNumber = 1;

        #endregion

        #region Variables

        List<BSPTreeNode> m_children;
        Vector2 m_centre;
        Vector2 m_botLeftCorner;
        int m_width;
        int m_height;
        GameObject m_floorTile;
        GameObject m_floorTile2;
        GameObject m_floorTile3;
        int m_minimumCellSize = 3;
        int m_roomLeftBound;
        int m_roomRightBound;
        int m_roomTopBound;
        int m_roomBotBound;
        int[,] m_spawnGrid;
        #endregion

        #region Properties

        public Vector2 Centre
        {
            get
            {
                return m_centre;
            }

            set
            {
                m_centre = value;
            }
        }

        public int Width
        {
            get
            {
                return m_width;
            }

            set
            {
                m_width = value;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }

            set
            {
                m_height = value;
            }
        }

        public List<BSPTreeNode> Children
        {
            get
            {
                return m_children;
            }

            set
            {
                m_children = value;
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
                m_minimumCellSize = value;
            }
        }

        public int RoomLeftBound
        {
            get
            {
                return m_roomLeftBound;
            }

            set
            {
                m_roomLeftBound = value;
            }
        }

        public int RoomRightBound
        {
            get
            {
                return m_roomRightBound;
            }

            set
            {
                m_roomRightBound = value;
            }
        }

        public int RoomTopBound
        {
            get
            {
                return m_roomTopBound;
            }

            set
            {
                m_roomTopBound = value;
            }
        }

        public int RoomBotBound
        {
            get
            {
                return m_roomBotBound;
            }

            set
            {
                m_roomBotBound = value;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centre">The centre of the cell</param>
        /// <param name="width">Width of the cell</param>
        /// <param name="height">Height of the cell</param>
        public BSPTreeNode(Vector2 centre, int width, int height, Vector2 botLeftCorner, GameObject floor, GameObject floor2, GameObject floor3, ref int[,] grid)
        {
            m_children = new List<BSPTreeNode>();
            m_centre = centre;
            m_width = width;
            m_height = height;
            m_floorTile = floor;
            m_floorTile2 = floor2;
            m_floorTile3 = floor3;
            m_botLeftCorner = botLeftCorner;
            m_spawnGrid = grid;
        }

        public BSPTreeNode(Vector2 centre)
        {
            m_children = new List<BSPTreeNode>();
            m_centre = centre;
            m_width = 0;
            m_height = 0;
            m_botLeftCorner = Vector2.zero;
        }

        /// <summary>
        /// Attach another cell to this current cell
        /// </summary>
        /// <param name="node">The new cell that connects to this one</param>
        public void AttachNode(BSPTreeNode node)
        {
            m_children.Add(node);
        }

        /// <summary>
        /// Remove a cell connected to this cell
        /// </summary>
        /// <param name="node">The cell to remove</param>
        public void DetachNode(BSPTreeNode node)
        {
            m_children.Remove(node);
        }

        /// <summary>
        /// Split the current cell into 2
        /// </summary>
        public void SplitCell()
        {
            bool canSplitVertically = false;
            bool canSplitHorizontally = false;
            bool splitVertically = false;
            int maxSplitBound;

            // Check if the cell can be split vertically or horizontally
            // This is to prevent the newly created cells having a width/height smaller than the minimum
            if (m_width - splitBound > m_minimumCellSize) canSplitVertically = true;
            if (m_height - splitBound > m_minimumCellSize) canSplitHorizontally = true;

            // Can split either way so randomly chose it
            // TODO: let the user input the likelihood of the split?
            if (canSplitHorizontally && canSplitVertically)
            {
                splitVertically = (Random.Range(trueValue, falseValue) == trueValue);
            }
            // Can only split horizontally
            else if (canSplitHorizontally && !canSplitVertically)
            {
                splitVertically = false;
            }
            // Can only split vertically
            else if (!canSplitHorizontally && canSplitVertically)
            {
                splitVertically = true;

            }
            // Cannot split either way so don't
            else return;

            // Get the upper bound of the split based on how the cell will be cut
            if (splitVertically) maxSplitBound = m_width;
            else maxSplitBound = m_height;

            // Get where the split happens
            // The split bound is to prevent very small rooms
            int splitLocation = Random.Range(splitBound, maxSplitBound - splitBound);
            Debug.Log("SPLIT LOCATION: " + splitLocation);

            if (splitVertically)
            {
                #region old version which might be needed
                /*
                // Centre is just half of the new width and height
                // Uses splitLocation as the width because this is the left side of the split
                Vector2 centre = new Vector2(splitLocation / getCentre, m_height / getCentre);

                // Create and attach the new cell made
                BSPTreeNode node = new BSPTreeNode(centre, splitLocation, m_height);
                AttachNode(node);

                // Create the other cell which is made
                // Uses width - splitLocation because it is the right side of the split
                int newWidth = m_width - splitLocation;
                centre.x = newWidth / 2;
                node = new BSPTreeNode(centre, newWidth, m_height);
                AttachNode(node);
                */

                #endregion
                // The 1st node has split location as it's width since it was cut vertically at that location
                // And that is one side, while the other node has the other side of the split's width
                AddSplitNodesToTree(splitLocation, m_height, m_width - splitLocation, m_height, true, splitLocation);
            }
            else
            {
                #region old version which might be needed
                /*
                // Uses splitLocation as the height because this is the top side of the split
                Vector2 centre = new Vector2(m_width, splitLocation / getCentre);
                BSPTreeNode node = new BSPTreeNode(centre, m_width, splitLocation);
                AttachNode(node);

                // Uses splitLocation as the height because this is the bot side of the split
                int newHeight = m_height - splitLocation;
                centre.y = (newHeight) / 2;
                node = new BSPTreeNode(centre, m_width, newHeight);
                AttachNode(node);
                */
                #endregion
                // The 1st node has split location as it's height since it was cut horizontally at that location
                // And that is one side, while the other node has the other side of the split's height
                AddSplitNodesToTree(m_width, splitLocation, m_width, m_height - splitLocation, false, splitLocation);
            }
        }

        /// <summary>
        /// Create the nodes made by the split and add them to the tree
        /// </summary>
        /// <param name="node1Width">Width of the 1st node</param>
        /// <param name="node1Height">Height of the 1st node</param>
        /// <param name="node2Width">Width of the 2nd node</param>
        /// <param name="node2Height">Height of the 2nd node</param>
        void AddSplitNodesToTree(int node1Width, int node1Height, int node2Width, int node2Height, bool vertically, int splitPos)
        {
            // Centre is just half of the new width and height
            Vector2 centre = new Vector2(node1Width / getCentre, node1Height / getCentre);
            // Apply the offset to the centre so the outputted room is in the correct location
            // Without this it will spawn based on (0,0)
            centre.x += m_botLeftCorner.x;
            centre.y += m_botLeftCorner.y;

            // Create and attach the new cell made
            BSPTreeNode node = new BSPTreeNode(centre, node1Width, node1Height, m_botLeftCorner, m_floorTile, m_floorTile2, m_floorTile3, ref m_spawnGrid);
            AttachNode(node);

            // Create the other node
            centre = new Vector2(node2Width / getCentre, node2Height / getCentre);

            Vector2 botLeft = m_botLeftCorner;

            // Add the offset to the centre based on how the cell was split
            if (vertically)
            {
                centre.x += splitPos;
                botLeft.x += splitPos;
            }
            else
            {
                centre.y += splitPos;
                botLeft.y += splitPos;
            }

            // Apply the offset to the centre so the outputted room is in the correct location
            // Without this it will spawn based on (0,0)
            centre.x += m_botLeftCorner.x;
            centre.y += m_botLeftCorner.y;

            // Create and attach the other node
            node = new BSPTreeNode(centre, node2Width, node2Height, botLeft, m_floorTile, m_floorTile2, m_floorTile3, ref m_spawnGrid);
            AttachNode(node);
        }

        /// <summary>
        /// Create the room in the cell
        /// </summary>
        public void CreateRoom(int debugNum)
        {
            Debug.Log(debugNum + ": " + m_centre + " | WIDTH: " + m_width + " | HEIGHT: " + m_height);
            //int roomWidth = Mathf.FloorToInt(m_width / 2);
            //int roomHeight = Mathf.FloorToInt(m_height / 2);
            //Debug.Log("ROOMWIDTH: " + roomWidth + " | ROOMHEIGHT: " + roomHeight);
            debugNum++;

            // Check if the node has any children since we only create rooms with none
            if (m_children.Count > 0)
            {
                // Go through each child and check them to see if they can create rooms
                for (int i = 0; i < m_children.Count; i++)
                {
                    m_children[i].CreateRoom(debugNum);
                }
            }
            else
            {
                // TODO Add user input here
                // Get the width and height of the room
                // Is divided by 2 so they go across the centre of the room
                int roomWidth = Random.Range(m_minimumCellSize, m_width) / 2;
                int roomHeight = Random.Range(m_minimumCellSize, m_height) / 2;

                // Create parent object to organise the hierarchy
                //GameObject parent = new GameObject();
                //parent.transform.position = new Vector3(m_centre.x, 1, m_centre.y);
                //parent.name = "Room";

                // TODO Need to add width/height to the loop increments
                // Spawn the tiles in the room in the scene
                GameObject floor;

                switch (debugNum)
                {
                    case 1:
                        floor = m_floorTile;
                        break;
                    case 2:
                        floor = m_floorTile2;
                        break;
                    case 3:
                        floor = m_floorTile3;
                        break;
                    default:
                        floor = m_floorTile;
                        break;
                }

                // Save the bounds for th output of the room so these can be used when creating the corridors
                m_roomLeftBound = Mathf.FloorToInt(m_centre.x - roomWidth);
                m_roomRightBound = (int)m_centre.x + roomWidth;
                m_roomBotBound = Mathf.FloorToInt(m_centre.y - roomHeight);
                m_roomTopBound = (int)m_centre.y + roomHeight;

                // Check if the width/height is odd
                // If so add 1 to upper to output the correct size
                // Can't split the value both sides since its an odd number so add the remainder to the end
                if (m_width % checkIfEvenNumber == oddNumber) m_roomRightBound++;
                if (m_height % checkIfEvenNumber == oddNumber) m_roomTopBound++;

                // Go through room and output
                // TODO add tile width/height to the increments
                for (int x = m_roomLeftBound; x < m_roomRightBound; x++)
                {
                    for (int y = m_roomBotBound; y < m_roomTopBound; y++)
                    {
                        m_spawnGrid[x, y] = BinarySpacePartition.roomGridNum;
                        //GameObject.Instantiate(floor, new Vector3(x, 1, y), floor.transform.rotation, parent.transform);
                    }
                }
            }
        }

        /// <summary>
        /// Create the corridor to connect to the parent cell
        /// </summary>
        /// <param name="parentCentre">The parent's cell centre</param>
        public void CreateCorridorToParent(BSPTreeNode parent)
        {

            // Check if the node has any children since we want to start with the leaf nodes first
            if (m_children.Count > 0)
            {
                // Go through each child and connect them with this node
                for (int i = 0; i < m_children.Count; i++)
                {
                    m_children[i].CreateCorridorToParent(this);
                }
            }

            // For the parent node since it doesn't have anything to connect to
            if (parent.Centre == Vector2.zero) return;

            // Based on the parent's position to the current node
            // Spawn the corridor
            // Should only be in 4 straight line directions due to the splitting keeping one axis the same
            if (m_centre.x < parent.Centre.x)
            {
                SpawnCorridor(Mathf.FloorToInt(m_centre.x), Mathf.FloorToInt(parent.Centre.x), true, Mathf.FloorToInt(m_centre.y));
            }
            else if (m_centre.x > parent.Centre.x)
            {
                SpawnCorridor(Mathf.FloorToInt(parent.Centre.x), Mathf.FloorToInt(m_centre.x), true, Mathf.FloorToInt(m_centre.y));
            }
            else if (m_centre.y < parent.Centre.y)
            {
                SpawnCorridor(Mathf.FloorToInt(m_centre.y), Mathf.FloorToInt(parent.Centre.y), false, Mathf.FloorToInt(m_centre.x));
            }
            else
            {
                SpawnCorridor(Mathf.FloorToInt(parent.Centre.y), Mathf.FloorToInt(m_centre.y), false, Mathf.FloorToInt(m_centre.x));
            }
        }

        /// <summary>
        /// Spawn the corridor
        /// </summary>
        /// <param name="lowerBound">The start of the corridor</param>
        /// <param name="upperBound">The end of the corridor</param>
        /// <param name="xAxis">Is the corridor going across on the x axis?</param>
        /// <param name="otherAxisValue">The value for the axis which the corridor is NOT going across</param>
        public void SpawnCorridor(int lowerBound, int upperBound, bool xAxis, int otherAxisValue)
        {
            // Create the parent object to clean the hierarchy up
            //GameObject parent = new GameObject();
            //parent.transform.position = new Vector3(m_centre.x, 1, m_centre.y);
            //parent.name = "Corridor";

            int x = 0;
            int y = 0;

            // Set the value for the axis the corridor won't be across on
            if (xAxis) y = otherAxisValue;
            else x = otherAxisValue;

            for (int i = lowerBound; i < upperBound; i++)
            {
                // Set the correct axis
                if (xAxis) x = i;
                else y = i;

                if (m_spawnGrid[x, y] != BinarySpacePartition.roomGridNum) m_spawnGrid[x, y] = BinarySpacePartition.corridorGridNum;
                // GameObject.Instantiate(m_floorTile, new Vector3(x, 1, y), m_floorTile.transform.rotation, parent.transform);
            }
        }

        //public void SpawnDungeon()
        //{
        //    GameObject parentCorridor = new GameObject();
        //    parentCorridor.transform.position = new Vector3(m_centre.x, 1, m_centre.y);
        //    parentCorridor.name = "Corridor";

        //    GameObject parentRoom = new GameObject();
        //    parentRoom.transform.position = new Vector3(m_centre.x, 1, m_centre.y);
        //    parentRoom.name = "Rooms";

        //    for (int x = 0; x < m_width; x++)
        //    {
        //        for (int y = 0; y < m_height; y++)
        //        {
        //            if(m_spawnGrid[x, y] == roomGridNum)
        //            {
        //                GameObject.Instantiate(m_floorTile, new Vector3(x, 1, y), m_floorTile.transform.rotation, parentRoom.transform);
        //            }
        //            else if(m_spawnGrid[x, y] == corridorGridNum)
        //            {
        //                GameObject.Instantiate(m_floorTile2, new Vector3(x, 1, y), m_floorTile.transform.rotation, parentCorridor.transform);
        //            }
        //        }
        //    }
        //}
    }
}
