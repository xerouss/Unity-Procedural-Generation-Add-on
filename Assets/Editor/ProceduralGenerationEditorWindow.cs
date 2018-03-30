// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   30/03/2018
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
        const int heightmapResLowerBound = 0;
        const int heightmapResHigherBound = 256;
        #endregion

        #region Private
        enum LevelTypes
        {
            TERRAIN, DUNGEON, NUMOFLEVELTYPES
        }

        int m_levelType = 0;
        bool m_realtimeGeneration = false;
        string[] m_levelTypeOptions = { "Terrain", "Dungeon" };
        static PerlinNoise m_perlinNoise;

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
            m_perlinNoise = new PerlinNoise(256 * 2);
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

        private void Update()
        {
            if(m_perlinNoise != null && m_realtimeGeneration) m_perlinNoise.SetTerrainData();
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

            // If the user wants to update the terrain as they change it
            m_realtimeGeneration = EditorGUILayout.Toggle(new GUIContent("Real-time Generation: ", "Update the terrain as you change it. Turn off for large terrains as it may cause problems"), m_realtimeGeneration);

            // Don't need the button if the terrain is being auto updated
            if (!m_realtimeGeneration)
            {

                // Create a new GameObject that contains the level
                // TODO: Implement this, does not create a new game object at the moment
                // STILL NEED WITH THE REALTIME UPDATING?
                if (GUILayout.Button("Create Level"))
                {
                    if (m_levelType == (int)LevelTypes.TERRAIN)
                    {
                        m_perlinNoise.SetTerrainData();
                    }
                }

                // Create the level using the existing GameObject
                if (GUILayout.Button("Re-create Level"))
                {
                    if (m_levelType == (int)LevelTypes.TERRAIN)
                    {
                        m_perlinNoise.SetTerrainData();
                    }
                }

                // Delete the level's GameObject
                if (GUILayout.Button("Delete Level"))
                {
                    DestroyImmediate(GameObject.FindObjectOfType<Terrain>().gameObject);
                }
            }

            // Reset all variables to the default values
            if (GUILayout.Button("Reset Variables"))
            {
                if (m_levelType == (int)LevelTypes.TERRAIN)
                {
                    m_perlinNoise.ResetVariableValues();
                }
            }
        }

        /// <summary>
        /// Variables for the terrain generation
        /// </summary>
        void TerrainGUI()
        {
            // TODO: Improve descriptions

            // Header
            GUILayout.Label("Terrain", m_header2Style);
            EditorGUILayout.Space();

            // Seed
            m_perlinNoise.Seed = EditorGUILayout.IntField(new GUIContent("Seed", "The seed of the current variables values"), m_perlinNoise.Seed);

            // Terrain size
            m_perlinNoise.TerrainSize = EditorGUILayout.Vector3Field(new GUIContent("Terrain Size:", "X = Width, Y = Higher the spikes in the level are, Z = Depth") , m_perlinNoise.TerrainSize);
            EditorGUILayout.Space();

            // Heightmap Resolution
            m_perlinNoise.HeightmapResolution = EditorGUILayout.IntSlider(new GUIContent("Heightmap Resolution: ", "Default = 32"), 
                m_perlinNoise.HeightmapResolution, heightmapResLowerBound, heightmapResHigherBound);
            EditorGUILayout.Space();

            // Offset
            m_perlinNoise.PosOffset = EditorGUILayout.Vector2Field(new GUIContent("Position Offset: ", "Change the position of the perlin noise"), m_perlinNoise.PosOffset);
            EditorGUILayout.Space();

            //// Fade Change
            GUILayout.Label("Fade Change", m_header3Style);
            m_perlinNoise.MultiplyFade = EditorGUILayout.IntField(new GUIContent("Multiply Value: ", "Default = 6"), m_perlinNoise.MultiplyFade);
            m_perlinNoise.MinusFade = EditorGUILayout.IntField(new GUIContent("Minus Value: ", "Default = 15"), m_perlinNoise.MinusFade);
            m_perlinNoise.AdditionFade = EditorGUILayout.IntField(new GUIContent("Addition Value: ", "Default = 10"), m_perlinNoise.AdditionFade);
            EditorGUILayout.Space();

            // Fractal
            GUILayout.Label("Fractal Brownian Motion", m_header3Style);
            m_perlinNoise.Octaves = EditorGUILayout.FloatField(new GUIContent("Octaves: ", "Amount of times it iterates. More = more detail"), m_perlinNoise.Octaves);
            m_perlinNoise.Frequency = EditorGUILayout.FloatField(new GUIContent("Frequency: ", "How much the bumps are spread out. Lower = flatter"), m_perlinNoise.Frequency);
            m_perlinNoise.Amplitude = EditorGUILayout.FloatField(new GUIContent("Amplitude: ", "How flat/tall it is"), m_perlinNoise.Amplitude);
            m_perlinNoise.AmplitudeGain = EditorGUILayout.FloatField(new GUIContent("Amplitude Gain: ", "How much the amplitude increases after each iteration"), m_perlinNoise.AmplitudeGain);
            m_perlinNoise.Lacunarity = EditorGUILayout.FloatField(new GUIContent("Lacunarity: ", "How much the frequency is increased after each iteration"), m_perlinNoise.Lacunarity);
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
