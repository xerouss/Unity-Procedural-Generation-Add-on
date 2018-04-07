// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   07/04/2018
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
        const int splitBound = 1;
        const int getCentre = 2;

        #endregion

        #region Variables

        List<BSPTreeNode> m_children;
        Vector2 m_centre;
        int m_width;
        int m_height;
        GameObject m_floorTile;

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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centre">The centre of the cell</param>
        /// <param name="width">Width of the cell</param>
        /// <param name="height">Height of the cell</param>
        public BSPTreeNode(Vector2 centre, int width, int height, GameObject floor)
        {
            m_children = new List<BSPTreeNode>();
            m_centre = centre;
            m_width = width;
            m_height = height;
            m_floorTile = floor;
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
            // Get random bool to choose whether to split horizontally or vertically
            // The false value is one more than the actual value since random range doesn't include the max value in the output value
            // TODO: let the user input the likelihood of the split?
            bool splitVertically = (Random.Range(trueValue, falseValue) == trueValue);

            // Get the highest value the split location can be by getting the width/height
            int maxSplitBound;
            if (splitVertically) maxSplitBound = m_height;
            else maxSplitBound = m_width;

            // Get where the split happens
            // The split bound is to prevent very small rooms
            int splitLocation = Random.Range(splitBound, maxSplitBound - splitBound);

            if(splitVertically)
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
                AddSplitNodesToTree(splitLocation, m_height, m_width - splitLocation, m_height);
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
                AddSplitNodesToTree(m_width, splitLocation, m_width, m_height - splitLocation);
            }
        }

        /// <summary>
        /// Create the nodes made by the split and add them to the tree
        /// </summary>
        /// <param name="node1Width">Width of the 1st node</param>
        /// <param name="node1Height">Height of the 1st node</param>
        /// <param name="node2Width">Width of the 2nd node</param>
        /// <param name="node2Height">Height of the 2nd node</param>
        void AddSplitNodesToTree(int node1Width, int node1Height, int node2Width, int node2Height)
        {
            // Centre is just half of the new width and height
            Vector2 centre = new Vector2(node1Width / getCentre, node1Height / getCentre);

            // Create and attach the new cell made
            BSPTreeNode node = new BSPTreeNode(centre, node2Width, node1Height, m_floorTile);
            AttachNode(node);

            // Create the other node
            centre = new Vector2(node2Width / getCentre, node2Height / getCentre);
            node = new BSPTreeNode(centre, node2Width, node2Height, m_floorTile);
            AttachNode(node);
        }

        /// <summary>
        /// Create the room in the cell
        /// </summary>
        public void CreateRoom()
        {
            // Check if the node has any children since we only create cells with none
            if(m_children.Count > 0)
            {
                // Go through each child and check them to see if they can create rooms
                for (int i = 0; i < m_children.Count; i++)
                {
                    m_children[i].CreateRoom();
                }
            }
            else
            {
                // TODO Add user input here
                // Get the width and height of the room
                // The 5 is for bound TODO: change this to a variable
                // Is divided by 2 so they go across the centre of the room
                int roomWidth = Random.Range(5, m_width - 5) / 2;
                int roomHeight = Random.Range(5, m_height - 5) / 2;

                // TODO Need to add width/height to the loop increments
                // Spawn the tiles in the room in the scene
                for (int x = (int)m_centre.x - roomWidth; x < m_centre.x + roomWidth; x++)
                {
                    for (int y = (int)m_centre.y - roomHeight; y < m_centre.y + roomHeight; y++)
                    {
                        GameObject.Instantiate(m_floorTile, new Vector3(x, 1, y), m_floorTile.transform.rotation);
                    }
                }
            }
        }
    }
}
