// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
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
    public class BinarySpacePartitionTreeNode
    {
        #region Constants
        const int trueValue = 0;
        const int falseValue = 2;
        const int getCentre = 2;
        const int checkIfEvenNumber = 2;
        const int oddNumber = 1;
        const int acrossCenter = 2;
        #endregion

        #region Variables
        List<BinarySpacePartitionTreeNode> m_children;
        Vector2 m_centre;
        Vector2 m_botLeftCorner;
        int m_width;
        int m_height;
        int[,] m_spawnGrid;
        BinarySpacePartition m_binarySpacePartition;
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

        public List<BinarySpacePartitionTreeNode> Children
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
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centre">The centre of the cell</param>
        /// <param name="width">Width of the cell</param>
        /// <param name="height">Height of the cell</param>
        /// <param name="botLeftCorner">The bot left corner of the cell</param>
        /// <param name="grid">The spawn grid which is used to output the dungeon</param>
        /// <param name="BSP">The Binary Space Partition class being used</param>
        public BinarySpacePartitionTreeNode(Vector2 centre, int width, int height, Vector2 botLeftCorner, ref int[,] grid, BinarySpacePartition BSP)
        {
            m_children = new List<BinarySpacePartitionTreeNode>();
            m_centre = centre;
            m_width = width;
            m_height = height;
            m_botLeftCorner = botLeftCorner;
            m_spawnGrid = grid;
            m_binarySpacePartition = BSP;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centre">The centre of the cell</param>
        public BinarySpacePartitionTreeNode(Vector2 centre)
        {
            m_children = new List<BinarySpacePartitionTreeNode>();
            m_centre = centre;
            m_width = 0;
            m_height = 0;
            m_botLeftCorner = Vector2.zero;
        }
        #endregion

        /// <summary>
        /// Attach another cell to this current cell
        /// </summary>
        /// <param name="node">The new cell that connects to this one</param>
        public void AttachNode(BinarySpacePartitionTreeNode node)
        {
            m_children.Add(node);
        }

        /// <summary>
        /// Remove a cell connected to this cell
        /// </summary>
        /// <param name="node">The cell to remove</param>
        public void DetachNode(BinarySpacePartitionTreeNode node)
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
            int minCellSize = m_binarySpacePartition.MinimumCellSize;

            // Check if the cell can be split vertically or horizontally
            // This is to prevent the newly created cells having a width/height smaller than the minimum size
            if (m_width - minCellSize > minCellSize) canSplitVertically = true;
            if (m_height - minCellSize > minCellSize) canSplitHorizontally = true;

            // Can split either way so randomly chose it
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

            // Get the max value of the split based on how the cell will be cut
            if (splitVertically) maxSplitBound = m_width;
            else maxSplitBound = m_height;

            // Get where the split happens
            int splitLocation = Random.Range(minCellSize, maxSplitBound - minCellSize);

            if (splitVertically)
            {
                // The 1st node has split location as it's width since it was cut vertically at that location
                // And that is one side, while the other node has the other side of the split's width
                AddSplitNodesToTree(splitLocation, m_height, m_width - splitLocation, m_height, true, splitLocation);
            }
            else
            {
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
        /// <param name="vertically">If the cell was cut vertically or horizontally</param>
        /// <param name="splitPos">Where the split happened in the cell</param>
        void AddSplitNodesToTree(int node1Width, int node1Height, int node2Width, int node2Height, bool vertically, int splitPos)
        {
            // *************************************************
            // How they are split and bot left corners
            // ____________________________________________ 
            // |                    |                      | 
            // |                    |                      |
            // |       1st Node     |        2nd Node      |
            // |                    |                      |
            // |                    |                      |
            // |____________________|______________________|
            // ^ 1st node bot left  ^ Split Position and new bot left corner for 2nd node

            //                                                       ____________________________________________
            //                                                       |                                           | 
            //                                                       |                                           |
            //                                                       |                2nd Node                   |
            // Split Position and new bot left corner for 2nd node > |-------------------------------------------|
            //                                                       |                1st Node                   |
            //                                                       |___________________________________________|
            //                                                       ^ 1st node bot left
            // *************************************************

            // *************************************************
            // First Node (Left or bot cell of the split)

            // Centre is just half of the new width and height
            Vector2 centre = new Vector2(node1Width / getCentre, node1Height / getCentre);

            // Apply the offset to the centre so the outputted room is in the correct location
            // Without this it will spawn always with the bot left corner at (0,0)
            centre.x += m_botLeftCorner.x;
            centre.y += m_botLeftCorner.y;

            // Create and attach the new cell made
            // Uses the same bot left corner at the current node since its the left/bot node so it would not have moved
            BinarySpacePartitionTreeNode node = new BinarySpacePartitionTreeNode(centre, node1Width, node1Height, m_botLeftCorner, ref m_spawnGrid, m_binarySpacePartition);
            AttachNode(node);
            // *************************************************

            // *************************************************
            // Second Node (Right or top cell of the split)

            centre = new Vector2(node2Width / getCentre, node2Height / getCentre);

            // Save where the new bot left will be
            Vector2 botLeft = m_botLeftCorner;

            // Set the right/top side node's centre position
            // Add the split position to the centre to make it the right/top side of the parent
            // If this is not added it will be outputted to the same location as the left/bot side
            // Have to apply it to the new corner since it has been moved from the parent
            // Due to being the right/top cell and is not using that corner anymore
            // ____________________________________________ 
            // |                    |                      | 
            // |         |          |           |          |
            // |   Without adding   | With adding splitPos |
            // |   splitPos         |           |          |
            // |         |          |                      |
            // |____________________|______________________|
            //           ^ Centre               ^ Centre

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
            // Without this it will spawn always with the bot left corner at (0,0)
            centre.x += m_botLeftCorner.x;
            centre.y += m_botLeftCorner.y;

            // Create and attach the other node
            node = new BinarySpacePartitionTreeNode(centre, node2Width, node2Height, botLeft, ref m_spawnGrid, m_binarySpacePartition);
            AttachNode(node);
            // *************************************************
        }

        /// <summary>
        /// Create the room in the cell
        /// </summary>
        public void CreateRoom()
        {

            // Check if the node has any children since we only create rooms with none
            if (m_children.Count > 0)
            {
                // Go through each child and check them to see if they can create rooms
                for (int i = 0; i < m_children.Count; i++)
                {
                    m_children[i].CreateRoom();
                }
            }
            // Is a leaf node
            else
            {
                // Get the width and height of the room
                // Is divided by 2 so they go across the centre of the room
                int roomWidth = Random.Range(m_binarySpacePartition.MinimumRoomSize, m_width) / acrossCenter;
                int roomHeight = Random.Range(m_binarySpacePartition.MinimumRoomSize, m_height) / acrossCenter;

                // Save the bounds for th output of the room so these can be used when creating the corridors
                int roomLeftBound = Mathf.FloorToInt(m_centre.x - roomWidth);
                int roomRightBound = (int)m_centre.x + roomWidth;
                int roomBotBound = Mathf.FloorToInt(m_centre.y - roomHeight);
                int roomTopBound = (int)m_centre.y + roomHeight;

                // Check if the width/height is odd
                // If so add 1 to upper to output the correct size
                // Can't split the value both sides since its an odd number and needs to be an int so add the remainder to the end
                if (m_width % checkIfEvenNumber == oddNumber) roomRightBound++;
                if (m_height % checkIfEvenNumber == oddNumber) roomTopBound++;

                // Add the room to the grid
                for (int x = roomLeftBound; x < roomRightBound; x++)
                {
                    for (int y = roomBotBound; y < roomTopBound; y++)
                    {
                        m_spawnGrid[x, y] = BinarySpacePartition.roomGridNum;
                    }
                }
            }
        }

        /// <summary>
        /// Create the corridor to connect to the parent cell
        /// </summary>
        /// <param name="parentNode">The node's parent node</param>
        public void CreateCorridorToParent(BinarySpacePartitionTreeNode parentNode)
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

            // For the root node since it doesn't have anything to connect to
            if (parentNode.Centre == Vector2.zero) return;

            // Based on the parent's position to the current node, spawn the corridor
            // Should only be in 4 straight line directions due to the splitting keeping one axis the same

            // Room is left of the parent room
            if (m_centre.x < parentNode.Centre.x)
            {
                CreateCorridor(m_centre.x, parentNode.Centre.x, true, m_centre.y);
            }
            // Room is right of the parent room
            else if (m_centre.x > parentNode.Centre.x)
            {
                CreateCorridor(parentNode.Centre.x, m_centre.x, true, m_centre.y);
            }
            // Room is below the parent room
            else if (m_centre.y < parentNode.Centre.y)
            {
                CreateCorridor(m_centre.y, parentNode.Centre.y, false, m_centre.x);
            }
            // Room is above the parent room
            else
            {
                CreateCorridor(parentNode.Centre.y, m_centre.y, false, m_centre.x);
            }
        }

        /// <summary>
        /// Spawn the corridor
        /// </summary>
        /// <param name="lowerBound">The start of the corridor</param>
        /// <param name="upperBound">The end of the corridor</param>
        /// <param name="xAxis">Is the corridor going across on the x axis?</param>
        /// <param name="otherAxisValue">The value for the axis which the corridor is NOT going across</param>
        public void CreateCorridor(float lowerBound, float upperBound, bool xAxis, float otherAxisValue)
        {
            int lowerBoundInt = Mathf.FloorToInt(lowerBound);
            int upperBoundInt = Mathf.FloorToInt(upperBound);
            int otherAxisValueInt = Mathf.FloorToInt(otherAxisValue);
            int x = 0;
            int y = 0;

            // Set the value for the axis the corridor that won't be changed
            // E.g. The room is on the left and the parent room on the right
            // The creation will iterate the placement on the x axis to connect the rooms
            // But the y axis will stay the same
            if (xAxis) y = otherAxisValueInt;
            else x = otherAxisValueInt;

            for (int i = lowerBoundInt; i < upperBoundInt; i++)
            {
                // Set the correct axis
                if (xAxis) x = i;
                else y = i;

                // Add the corridor to the grid if the location isn't part of a room
                if (m_spawnGrid[x, y] != BinarySpacePartition.roomGridNum)
                    m_spawnGrid[x, y] = BinarySpacePartition.corridorGridNum;
            }
        }
    }
}
