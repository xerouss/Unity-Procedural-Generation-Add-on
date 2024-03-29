// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   24/04/2018
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

        #region Constants
        const int header1TextSize = 30;
        const int header2TextSize = 20;
        const int header3TextSize = 12;
        const int header4TextSize = 10;
        const int minWindowSize = 480;
        const int dropdownSpaceHeight = 15;
        #endregion

        enum LevelTypes
        {
            TERRAIN, DUNGEON, NUMOFLEVELTYPES
        }

        #region Variables
        int m_levelType = 0;
        static string[] m_levelTypeOptions;
        ProceduralGenerationAlgorithmGUI m_gui;
        static PerlinNoise m_perlinNoise;
        static PerlinNoiseGUI m_perlinNoiseGUI;
        static BinarySpacePartition m_binarySpacePartition;
        static BinarySpacePartitionGUI m_binarySpacePartitionGUI;

        #region Style Variables
        static GUIStyle m_header1Style;
        static GUIStyle m_header2Style;
        static GUIStyle m_header3Style;
        static GUIStyle m_header4Style;
        #endregion
        #endregion

        // Where to access the window
        [MenuItem("Window/Procedurally Generate Level")]

        /// <summary>
        /// How to display the window
        /// Needs to be static or else it will not show in the editor
        /// </summary>
        public static void ShowWindow()
        {
            // Create the editor window
            EditorWindow procGenWindow = EditorWindow.GetWindow(typeof(ProceduralGenerationEditorWindow));

            // The min size the window can be
            // This is so the window shows the header text when it is created
            procGenWindow.minSize = new Vector2(minWindowSize, minWindowSize);

            // Set the different types of levels based on the LevelTypes enum values
            int numOfLevelTypes = (int)LevelTypes.NUMOFLEVELTYPES;
            m_levelTypeOptions = new string[numOfLevelTypes];
            for (int i = 0; i < numOfLevelTypes; i++)
            {
                // Convert the int to the LevelType enum value then to string
                m_levelTypeOptions[i] = ((LevelTypes)i).ToString();
            }

            // Create the styles for the window
            CreateStyles();

            // Create the perlin noise and GUI for it
            m_perlinNoise = new PerlinNoise();
            m_perlinNoiseGUI = new PerlinNoiseGUI(m_perlinNoise.SeedClass, m_perlinNoise,
                m_header2Style, m_header3Style, m_header4Style);

            // Create the binary space partition and GUI for it
            m_binarySpacePartition = new BinarySpacePartition();
            m_binarySpacePartitionGUI = new BinarySpacePartitionGUI(m_binarySpacePartition.SeedClass, m_binarySpacePartition,
                m_header2Style, m_header3Style, m_header4Style);

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

            m_header4Style = new GUIStyle();
            m_header4Style.fontSize = header4TextSize;
        }

        /// <summary>
        /// If it's enabled, auto generate the level
        /// </summary>
        private void Update()
        {
            // Check if there is a GUI or else there will be a null reference error
            if (m_gui == null) return;

            if (m_gui.RealTimeGenerationActive) m_gui.ReCreateLevel();
        }

        /// <summary>
        /// What to display in the window
        /// </summary>
        void OnGUI()
        {
            // Header
            GUILayout.Label("Procedurally Generate Level", m_header1Style);
            EditorGUILayout.Space();

            GUILayout.Label("Hover over the variable name for information on what they change.");
            EditorGUILayout.Space();

            // Drop down box to select the level 
            m_levelType = EditorGUILayout.Popup("Level Type:", m_levelType, m_levelTypeOptions);
            GUILayout.Space(dropdownSpaceHeight);

            // Based on the level selected, set the correct GUI to display
            switch (m_levelType)
            {
                case (int)LevelTypes.TERRAIN:
                    m_gui = m_perlinNoiseGUI;
                    break;
                case (int)LevelTypes.DUNGEON:
                    m_gui = m_binarySpacePartitionGUI;
                    break;
                default:
                    Debug.Log("ERROR: UNKOWN LEVEL TYPE");
                    break;
            }

            // Display the selected level type's GUI
            m_gui.DisplayGUI();

            // Don't need the buttons if the level is being updated in real-time
            if (!m_gui.RealTimeGenerationActive)
            {               
                // Create the level using the existing GameObject
                if (GUILayout.Button("Create Level"))
                    m_gui.ReCreateLevel();

                // Create a new GameObject that contains the level
                if (GUILayout.Button("Create New Level GameObject"))
                    m_gui.CreateLevel();

                // Delete the level's GameObject
                if (GUILayout.Button("Delete Level"))
                    m_gui.DeleteLevel();
            }

            // Reset all variables to the default values
            if (GUILayout.Button("Reset Variables"))
                m_gui.ResetVariablesForLevel();
        }
    }
}
