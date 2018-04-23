// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
//	File:			   PerlinNoiseGUI.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using UnityEditor;

////////////////////////////////////////////

/// <summary>
/// GUI Class for the Perlin Noise
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class BinarySpacePartitionGUI : ProceduralGenerationAlgorithmGUI
    {
        #region Constants

        const string title = "Dungeon";

        #endregion

        #region Variables

        BinarySpacePartition m_binarySpacePartition;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BinarySpacePartitionGUI(GeneratorSeed seed, BinarySpacePartition bsp,
            GUIStyle header2, GUIStyle header3, GUIStyle header4) :
            base(seed, title, header2, header3, header4)
        {
            // Call the ProceduralGenerationAlgorithmGUI constructor

            m_binarySpacePartition = bsp;
        }

        /// <summary>
        /// The GUI for the BSP
        /// </summary>
        public override void DisplayGUI()
        {
            base.DisplayGUI(); // Display the GUI fields from the base class
            string fieldText;
            string tooltip;

            // Create all the fields the user can change to modify the dungeon generation

            // Size
            fieldText = "Dungeon Size: ";
            tooltip = "How large the dungeon will be. Y is used for the position of the roof.";
            m_binarySpacePartition.DungeonSize = EditorGUILayout.Vector3Field(new GUIContent(fieldText, tooltip), m_binarySpacePartition.DungeonSize);
            EditorGUILayout.Space();

            // Split Variables
            fieldText = "Amount of times to split: ";
            tooltip = "How many times the algorithm will split the dungeon into cells.";
            m_binarySpacePartition.SplitAmount = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.SplitAmount);
            EditorGUILayout.Space();

            // *************************************************
            // Minimum Sizes
            GUILayout.Label("Minimum Sizes", m_header3Style);

            // Cell
            fieldText = "Minimum size of cells: ";
            tooltip = "Cell = is the area which can be split and where the rooms are spawned in. The size of the cell which prevents it from being split. Higher = less likely to get split. High values can cause errors.";
            m_binarySpacePartition.MinimumCellSize = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.MinimumCellSize);

            // Room
            fieldText = "Minimum size of rooms: ";
            tooltip = "The minimum size the rooms can be. Make the value 1 or lower if you want dead ends to the corridors.";
            m_binarySpacePartition.MinimumRoomSize = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.MinimumRoomSize);
            EditorGUILayout.Space();
            // *************************************************

            // *************************************************
            // Tiles
            GUILayout.Label("Tiles", m_header3Style);
            GUILayout.Label("The tiles must be 1x1x1 for it to work correctly!", m_header4Style);

            fieldText = "Floor Tile: ";
            tooltip = "The tile used for the floor of the dungeon. The tiles must be 1x1x1 for it to work correctly.";
            m_binarySpacePartition.FloorTile = EditorGUILayout.ObjectField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.FloorTile, typeof(GameObject), false) as GameObject;

            fieldText = "Corridor Tile: ";
            tooltip = "The tile used for the corridor floor of the dungeon. The tiles must be 1x1x1 for it to work correctly.";
            m_binarySpacePartition.CorridorTile = EditorGUILayout.ObjectField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.CorridorTile, typeof(GameObject), false) as GameObject;

            fieldText = "Wall Tile: ";
            tooltip = "The tile used for the walls of the dungeon. The tiles must be 1x1x1 for it to work correctly.";
            m_binarySpacePartition.WallTile = EditorGUILayout.ObjectField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.WallTile, typeof(GameObject), false) as GameObject;

            fieldText = "Spawn dungeon roof?";
            tooltip = "Whether to spawn a roof on top of the dungeon or not.";
            m_binarySpacePartition.SpawnRoof = EditorGUILayout.Toggle(new GUIContent(fieldText, tooltip), m_binarySpacePartition.SpawnRoof);

            // If the user wants a roof on top of the dungeon, let them assign a roof tile
            if (m_binarySpacePartition.SpawnRoof)
            {
                fieldText = "Roof Tile: ";
                tooltip = "The tile used for the roof of the dungeon.The tiles must be 1x1x1 for it to work correctly";
                m_binarySpacePartition.RoofTile = EditorGUILayout.ObjectField(new GUIContent(fieldText, tooltip), m_binarySpacePartition.RoofTile, typeof(GameObject), false) as GameObject;
            }
            EditorGUILayout.Space();
            // *************************************************
        }

        /// <summary>
        /// Create the terrain using the Binary Space Partition
        /// </summary>
        public override void CreateLevel()
        {
            m_binarySpacePartition.CreateDungeon(false);
        }

        /// <summary>
        /// Re-use an already made dungeon game object for a new level layout
        /// </summary>
        public override void ReCreateLevel()
        {
            m_binarySpacePartition.CreateDungeon(true);
        }

        /// <summary>
        /// Delete the current level from the scene
        /// </summary>
        public override void DeleteLevel()
        {
            MonoBehaviour.DestroyImmediate(GameObject.FindObjectOfType<GeneratedDungeon>().gameObject);
        }

        /// <summary>
        /// Reset the variables for the Binary Space Partition back to the default
        /// </summary>
        public override void ResetVariablesForLevel()
        {
            m_binarySpacePartition.ResetVariableValues();
        }
    }
}