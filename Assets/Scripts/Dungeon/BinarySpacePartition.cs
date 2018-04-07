// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   07/04/2018
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
        const int arrayX = 0;
        const int arrayY = 1;
        const int trueValue = 0;
        const int falseValue = 2;
        const int makeBase0 = 1;
        const int getCentre = 2;
        const int topLeftCorner = 0;
        const int botRightCorner = 2;

        BSPTreeNode m_treeRootNode;

        // User variables
        Vector2 m_levelSize = new Vector2(10, 10); // Temp size, TODO: REMOVE
        int m_numOfRooms;
        // Change to desired rooms sizes?
        int m_splitAmount = 7; // Amount of times the grid is divided before the rooms are set
        GameObject m_floorTile;

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
            m_treeRootNode = new BSPTreeNode(new Vector2(25, 25), 50, 50, m_floorTile);
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

        public void CreateRooms()
        {
            m_treeRootNode.CreateRoom();
        }
    }
}
