// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   08/03/2018
//	File:			   ProceduralGenerationEditorWindow.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using UnityEditor;

////////////////////////////////////////////

/// <summary>
/// This is responsible for the window which holds everything in the add-on
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class ProceduralGenerationEditorWindow : EditorWindow
    {

        #region Variables

        #region Constants
        const int header1TextSize = 30;
        const int header2TextSize = 20;
        const int header3TextSize = 12;
        const int minWindowSize = 480;
        const int dropdownSpaceHeight = 15;
        #endregion

        #region Private
        enum LevelTypes
        {
            TERRAIN, DUNGEON, NUMOFLEVELTYPES
        }

        int m_levelType = 0;
        string[] m_levelTypeOptions = { "Terrain", "Dungeon" };

        #region Terrain Variables
        static PerlinNoise m_perlinNoise;
        Vector3 m_terrainSize = new Vector3(10, 1, 10);
        int m_heightmapResolution = 32;
        int m_multiplyFade = 6;
        int m_minusFade = 15;
        int m_additionFade = 10;
        int m_repeatAmount = 0;
        float m_minZValue = 0;
        float m_maxZValue = 1000;
        #endregion

        #region Style Variables
        static GUIStyle m_header1Style;
        static GUIStyle m_header2Style;
        static GUIStyle m_header3Style;
        #endregion
        #endregion

        #endregion

        // Where to access the window
        [MenuItem("Window/Procedurally Generate Level")]

        /// <summary>
        /// How to display the window
        /// </summary>
        public static void ShowWindow()
        {
            // Create the editor window
            EditorWindow procGenWindow = EditorWindow.GetWindow(typeof(ProceduralGenerationEditorWindow));
            // The min size the window can be
            // This is so the window shows the header text when it is created
            procGenWindow.minSize = new Vector2(minWindowSize, minWindowSize);

            // Create the styles for the window
            CreateStyles();

            // Create the perlin noise so it can be used
            m_perlinNoise = new PerlinNoise();
        }

        /// <summary>
        /// Create all the styles used in the GUI
        /// </summary>
        static void CreateStyles()
        {
            m_header1Style = new GUIStyle();
            m_header1Style.fontSize = header1TextSize;
            m_header1Style.fontStyle = FontStyle.Bold;

            m_header2Style = new GUIStyle();
            m_header2Style.fontSize = header2TextSize;
            m_header2Style.fontStyle = FontStyle.Bold;

            m_header3Style = new GUIStyle();
            m_header3Style.fontSize = header3TextSize;
            m_header3Style.fontStyle = FontStyle.Bold;
        }

        /// <summary>
        /// What to display in the window
        /// </summary>
        void OnGUI()
        {
            // Header
            GUILayout.Label("Procedurally Generate Level", m_header1Style); //new Rect(10, 10, 300, 100), "Procedurally Generate Level", m_header1Style);
            EditorGUILayout.Space();

            // Dropdown box to select the level 
            m_levelType = EditorGUILayout.Popup("Level Type:", m_levelType, m_levelTypeOptions);
            GUILayout.Space(dropdownSpaceHeight);

            // Based on the level selected, select the correct level type variables
            switch (m_levelType)
            {
                case (int)LevelTypes.TERRAIN:
                    TerrainGUI();
                    break;
                case (int)LevelTypes.DUNGEON:
                    DungeonGUI();
                    break;
                case (int)LevelTypes.NUMOFLEVELTYPES:
                default:
                    Debug.Log("ERROR: UNKOWN LEVEL TYPE");
                    break;
            }

            // Create a new GameObject that contains the level
            if(GUILayout.Button("Create Level"))
            {
                m_perlinNoise.CreateTerrain();
            }

            // Create the level using the existing GameObject
            if (GUILayout.Button("Re-create Level"))
            {

            }

            // Delete the level's GameObject
            if (GUILayout.Button("Delete Level"))
            {

            }
        }

        /// <summary>
        /// Variables for the terrain generation
        /// </summary>
        void TerrainGUI()
        {
            // Header
            GUILayout.Label("Terrain", m_header2Style);
            EditorGUILayout.Space();

            // Terrain size
            m_terrainSize = EditorGUILayout.Vector3Field("Terrain Size:", m_terrainSize);
            EditorGUILayout.Space();

            // Heightmap Resolution
            m_heightmapResolution = EditorGUILayout.IntField("Heightmap Resolution: ", m_heightmapResolution);
            EditorGUILayout.Space();

            // Repeat
            m_repeatAmount = EditorGUILayout.IntField("Repeat Amount: ", m_repeatAmount);
            EditorGUILayout.Space();

            // Fade Change
            GUILayout.Label("Fade Change", m_header3Style);
            m_multiplyFade = EditorGUILayout.IntField("Multiply Value: ", m_multiplyFade);
            m_minusFade = EditorGUILayout.IntField("Minus Value: ", m_minusFade);
            m_additionFade = EditorGUILayout.IntField("Addition Value: ", m_additionFade);
            EditorGUILayout.Space();

            // Terrain Z
            GUILayout.Label("Terrain Z Value", m_header3Style);
            m_minZValue = EditorGUILayout.FloatField("Minimum: ", m_minZValue);
            m_maxZValue = EditorGUILayout.FloatField("Maximum: ", m_maxZValue);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Variables for the dungeon generation
        /// </summary>
        void DungeonGUI()
        {
            GUILayout.Label("Dungeon", m_header2Style);
        }
    }
}
