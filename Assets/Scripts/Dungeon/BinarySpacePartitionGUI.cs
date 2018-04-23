// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   22/04/2018
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
            base(seed, title, header2,  header3,  header4)
        {
            // Call the ProceduralGenerationAlgorithmGUI constructor

            m_binarySpacePartition = bsp;
        }

        /// <summary>
        /// The GUI that for the BSP
        /// </summary>
        public override void DisplayGUI()
        {
            base.DisplayGUI();

            // Size
            m_binarySpacePartition.DungeonSize = EditorGUILayout.Vector3Field(new GUIContent("Dungeon Size: ", "How large the dungeon will be. Y is used for the position of the roof."), m_binarySpacePartition.DungeonSize);
            EditorGUILayout.Space();

            // Split Variables
            m_binarySpacePartition.SplitAmount = EditorGUILayout.IntField(new GUIContent("Amount of times to split: ",
                "How many times the algorithm will split the dungeon into cells."), m_binarySpacePartition.SplitAmount);
            EditorGUILayout.Space();

            // Minimum Sizes
            GUILayout.Label("Minimum Sizes", m_header3Style);
            m_binarySpacePartition.MinimumCellSize = EditorGUILayout.IntField(new GUIContent("Minimum size of cells: ",
                "Cell = is the area which can be split to create more cells and where the rooms are spawned in. The size of the cell which prevents it from being split. Higher = less likely to get split. High values can cause errors.")
                , m_binarySpacePartition.MinimumCellSize);

            m_binarySpacePartition.MinimumRoomSize = EditorGUILayout.IntField(new GUIContent("Minimum size of rooms: ",
                "The minimum size the rooms can be. Make the value 1 or lower if you want dead ends to the corridors."), m_binarySpacePartition.MinimumRoomSize);
            EditorGUILayout.Space();

            // Tiles
            GUILayout.Label("Tiles", m_header3Style);
            GUILayout.Label("The tiles must be 1x1x1 for it to work correctly!", m_header4Style);
            m_binarySpacePartition.FloorTile = EditorGUILayout.ObjectField(new GUIContent("Floor Tile: ", "The tile used for the floor of the dungeon. The tiles must be 1x1x1 for it to work correctly."), m_binarySpacePartition.FloorTile, typeof(GameObject), false) as GameObject;
            m_binarySpacePartition.CorridorTile = EditorGUILayout.ObjectField(new GUIContent("Corridor Tile: ", "The tile used for the corridor floor of the dungeon. The tiles must be 1x1x1 for it to work correctly."), m_binarySpacePartition.CorridorTile, typeof(GameObject), false) as GameObject;
            m_binarySpacePartition.WallTile = EditorGUILayout.ObjectField(new GUIContent("Wall Tile: ", "The tile used for the walls of the dungeon. The tiles must be 1x1x1 for it to work correctly."), m_binarySpacePartition.WallTile, typeof(GameObject), false) as GameObject;
            m_binarySpacePartition.SpawnRoof = EditorGUILayout.Toggle(new GUIContent("Spawn dungeon roof?", "Whether to spawn a roof on top of the dungeon or not."), m_binarySpacePartition.SpawnRoof);

            if (m_binarySpacePartition.SpawnRoof)
            {
                m_binarySpacePartition.RoofTile = EditorGUILayout.ObjectField(new GUIContent("Roof Tile: ", "The tile used for the roof of the dungeon.The tiles must be 1x1x1 for it to work correctly"), m_binarySpacePartition.RoofTile, typeof(GameObject), false) as GameObject;
            }
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Create the terrain from the perlin noise
        /// </summary>
        public override void CreateLevel()
        {
            m_binarySpacePartition.CreateDungeon();
        }

        /// <summary>
        /// Re-use an already made terrain for a new level layout
        /// </summary>
        public override void ReCreateLevel()
        {
            m_binarySpacePartition.CreateDungeon();
        }

        /// <summary>
        /// Delete the currently used terrain
        /// </summary>
        public override void DeleteLevel()
        {
           
        }

        /// <summary>
        /// Reset the variables for the Perlin Noise back to the default
        /// </summary>
        public override void ResetVariablesForLevel()
        {
            m_binarySpacePartition.ResetVariableValues();
        }
    }
}